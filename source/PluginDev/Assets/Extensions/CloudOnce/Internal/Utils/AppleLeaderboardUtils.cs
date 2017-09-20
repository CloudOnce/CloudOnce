// <copyright file="AppleLeaderboardUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_IOS || UNITY_TVOS
namespace CloudOnce.Internal.Utils
{
    using System;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;
    using UnityEngine.SocialPlatforms.GameCenter;

    /// <summary>
    /// iOS Game Center implementation of <see cref="ILeaderboardUtils"/>
    /// </summary>
    public class AppleLeaderboardUtils : ILeaderboardUtils
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

            if (!Social.localUser.authenticated)
            {
                ReportError(
                    string.Format("Can't submit score to leaderboard {0} ({1}). SubmitScore can only be called after authentication.", internalID, id), onComplete);
                return;
            }

            Action<bool> callback = response => OnSubmitScoreCompleted(response, score, onComplete, id, internalID);
            Social.ReportScore(score, id, callback);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void ShowOverlay(string id = "", string internalID = "")
        {
            if (!Social.localUser.authenticated)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning("ShowOverlay can only be called after authentication.");
#endif
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Showing leaderboards overlay.");
#endif
                Social.ShowLeaderboardUI();
            }
            else
            {
#if CLOUDONCE_DEBUG
                Debug.Log(string.Format("Showing {0} ({1}) leaderboard overlay.", internalID, id));
#endif
                GameCenterPlatform.ShowLeaderboardUI(id, TimeScope.AllTime);
            }
        }

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            Social.LoadScores(leaderboardID, callback);
        }

        #endregion /Public methods

        #region Private methods

        private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
        {
#if CLOUDONCE_DEBUG
            Debug.LogWarning(errorMessage);
#endif
            CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
        }

        private static void OnSubmitScoreCompleted(bool response, long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
        {
            if (response)
            {
#if CLOUDONCE_DEBUG
                Debug.Log(string.Format("Successfully submitted a score of {0} to {1} ({2}) leaderboard.", score, internalID, id));
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
