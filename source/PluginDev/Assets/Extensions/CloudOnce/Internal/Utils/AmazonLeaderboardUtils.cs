// <copyright file="AmazonLeaderboardUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && CLOUDONCE_AMAZON
namespace CloudOnce.Internal.Utils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Amazon GameCircle implementation of <see cref="ILeaderboardUtils"/>
    /// </summary>
    public class AmazonLeaderboardUtils : ILeaderboardUtils
    {
        private readonly Dictionary<int, Action<IScore[]>> loadScoresCallbacks;

        public AmazonLeaderboardUtils()
        {
            RequestID = 1;
            loadScoresCallbacks = new Dictionary<int, Action<IScore[]>>();
            Cloud.OnSignedInChanged += OnSignedInChanged;
        }

        public static int RequestID { get; set; }

        #region Public methods

        /// <summary>
        /// Reports a score to a leaderboard.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="score">The score to submit.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation.
        /// If unsuccessful, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void SubmitScore(
            string id,
            long score,
            Action<CloudRequestResult<bool>> onComplete,
            string internalID = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                var errorMessage = string.Format(
                    "Can't submit score to {0} leaderboard. Platform ID is null or empty!",
                    internalID);
                ReportError(errorMessage, onComplete);
                return;
            }

            if (!AGSPlayerClient.IsSignedIn())
            {
                const string errorMessage = "Can't submit score to leaderboard {0} ({1})." +
                                            " SubmitScore can only be called after authentication.";
                ReportError(string.Format(errorMessage, internalID, id), onComplete);
                return;
            }

            Action<AGSSubmitScoreResponse> callback = null;
            callback = response =>
            {
                OnSubmitScoreCompleted(response, score, onComplete, id, internalID);
                AGSLeaderboardsClient.SubmitScoreCompleted -= callback;
            };

            AGSLeaderboardsClient.SubmitScoreCompleted += callback;
            AGSLeaderboardsClient.SubmitScore(id, score);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void ShowOverlay(string id = "", string internalID = "")
        {
            if (!AGSPlayerClient.IsSignedIn())
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning("ShowOverlay can only be called after authentication.");
#endif
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Showing leaderboards overlay.");
#endif
                AGSLeaderboardsClient.ShowLeaderboardsOverlay();
            }
            else
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.IsNullOrEmpty(internalID)
                    ? string.Format("Showing {0} leaderboard overlay.", id)
                    : string.Format("Showing {0} ({1}) leaderboard overlay.", internalID, id));
#endif
                AGSLeaderboardsClient.ShowLeaderboardsOverlay(id);
            }
        }

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            loadScoresCallbacks.Add(RequestID, callback);
            AGSLeaderboardsClient.RequestScores(leaderboardID, LeaderboardScope.GlobalAllTime, RequestID++);
        }

        #endregion /Public methods

        #region Private methods

        private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.LogWarning(errorMessage);
#endif
            CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
        }

        private static void OnSubmitScoreCompleted(
            AGSRequestResponse response,
            long score,
            Action<CloudRequestResult<bool>> callbackAction,
            string id,
            string internalID)
        {
            if (!response.IsError())
            {
#if CLOUDONCE_DEBUG
                var debugMessage = string.Format(
                    "Successfully submitted a score of {0} to {1} ({2}) leaderboard.",
                    score,
                    internalID,
                    id);
                UnityEngine.Debug.Log(debugMessage);
#endif
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                var errorMessage = string.Format(
                    "Native API failed to submit a score of {0} to {1} ({2}) leaderboard.\nReported error: {3}",
                    score,
                    internalID,
                    id,
                    response.error);
                ReportError(errorMessage, callbackAction);
            }
        }

        private void OnSignedInChanged(bool isSignedIn)
        {
            if (isSignedIn)
            {
                AGSLeaderboardsClient.RequestScoresCompleted += OnRequestScoresCompleted;
            }
            else
            {
                AGSLeaderboardsClient.RequestScoresCompleted -= OnRequestScoresCompleted;
            }
        }

        private void OnRequestScoresCompleted(AGSRequestScoresResponse response)
        {
            var scoreCallback = loadScoresCallbacks.ContainsKey(response.userData)
                ? loadScoresCallbacks[response.userData]
                : null;
            if (scoreCallback != null)
            {
                var scores = new IScore[response.scores.Count];
                for (var i = 0; i < response.scores.Count; i++)
                {
                    scores[i] = new AGSSocialLeaderboardScore(response.scores[i], response.leaderboard);
                }

                scoreCallback(scores);
            }

            if (loadScoresCallbacks.ContainsKey(response.userData))
            {
                loadScoresCallbacks.Remove(response.userData);
            }
        }

        #endregion /Private methods
    }
}
#endif
