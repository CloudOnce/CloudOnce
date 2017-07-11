// <copyright file="GoogleLeaderboardUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && CLOUDONCE_GOOGLE
namespace CloudOnce.Internal.Utils
{
    using System;
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using Providers;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Google Play Game Services implementation of <see cref="ILeaderboardUtils"/>
    /// </summary>
    public class GoogleLeaderboardUtils : ILeaderboardUtils
    {
        #region Public methods

        /// <summary>
        /// Reports a score to a leaderboard.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="score">The score to submit.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ReportError(string.Format("Can't submit score to {0} leaderboard. Platform ID is null or empty!", internalID), onComplete);
                return;
            }

            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                const string errorMessage = "Can't submit score to leaderboard {0} ({1}). SubmitScore can only be called after authentication.";
                ReportError(string.Format(errorMessage, internalID, id), onComplete);
                return;
            }

            Action<bool> callback = response => OnSubmitScoreCompleted(response, score, onComplete, id, internalID);
            PlayGamesPlatform.Instance.ReportScore(score, id, callback);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void ShowOverlay(string id = "", string internalID = "")
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
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
                PlayGamesPlatform.Instance.ShowLeaderboardUI(null, OnShowOverlayCompleted);
            }
            else
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.IsNullOrEmpty(internalID)
                    ? string.Format("Showing {0} leaderboard overlay.", id)
                    : string.Format("Showing {0} ({1}) leaderboard overlay.", internalID, id));
#endif
                PlayGamesPlatform.Instance.ShowLeaderboardUI(id, OnShowOverlayCompleted);
            }
        }

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            PlayGamesPlatform.Instance.LoadScores(leaderboardID, callback);
        }

        #endregion /Public methods

        #region Private methods

        private static void OnShowOverlayCompleted(UIStatus callback)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Leaderboards overlay closed.");
#endif
            if (callback == UIStatus.NotAuthorized)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("User logged out from overlay, using guest user from now on.");
#endif
                GooglePlayGamesCloudProvider.Instance.ActivateGuestUserMode();
            }
        }

        private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.LogWarning(errorMessage);
#endif
            CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
        }

        private static void OnSubmitScoreCompleted(bool response, long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
        {
            if (response)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.Format("Successfully submitted a score of {0} to {1} ({2}) leaderboard.", score, internalID, id));
#endif
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                var error = string.Format("Native API failed to submit a score of {0} to {1} ({2}) leaderboard. Cause unknown.", score, internalID, id);
                ReportError(error, callbackAction);
            }
        }

        #endregion /Private methods
    }
}
#endif
