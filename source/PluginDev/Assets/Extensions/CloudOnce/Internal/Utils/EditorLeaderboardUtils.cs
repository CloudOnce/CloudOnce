// <copyright file="EditorLeaderboardUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Utils
{
    using System;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Unity Editor implementation of <see cref="ILeaderboardUtils"/>
    /// </summary>
    public class EditorLeaderboardUtils : ILeaderboardUtils
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

#if CLOUDONCE_DEBUG
            Debug.Log(string.Format("Successfully submitted a score of {0} to {1} ({2}) leaderboard.", score, internalID, id));
#endif
            CloudOnceUtils.SafeInvoke(onComplete, new CloudRequestResult<bool>(true));
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void ShowOverlay(string id = "", string internalID = "")
        {
            Debug.LogWarning("Leaderboards overlay is not supported in the Unity Editor.");
        }

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            Debug.LogWarning("Leaderboards overlay is not supported in the Unity Editor.");
            CloudOnceUtils.SafeInvoke(callback, new IScore[0]);
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

        #endregion /Private methods
    }
}
