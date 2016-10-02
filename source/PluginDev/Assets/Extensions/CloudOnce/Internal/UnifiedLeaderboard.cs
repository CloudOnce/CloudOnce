// <copyright file="UnifiedLeaderboard.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using UnityEngine.SocialPlatforms;
    using Utils;

    /// <summary>
    /// A cross-platform leaderboard.
    /// Class has a method for submitting score that will work for all supported platforms.
    /// Only intended to be used by CloudOnce internal classes.
    /// </summary>
    public class UnifiedLeaderboard
    {
        #region Fields & properties

        private readonly string internalID;

        /// <summary>
        /// A cross-platform leaderboard.
        /// Class has a method for submitting score that will work for all supported platforms.
        /// Only intended to be used by CloudOnce internal classes.
        /// </summary>
        /// <param name="internalID">The internal ID for this leaderboard.</param>
        /// <param name="platformID">The current platform ID for this leaderboard. Set to <c>string.Empty</c> if platform is not supported.</param>
        public UnifiedLeaderboard(string internalID, string platformID)
        {
            this.internalID = internalID;
            ID = platformID;
        }

        /// <summary>
        /// The ID for the current platform.
        /// </summary>
        public string ID { get; private set; }

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Reports a score to this leaderboard.
        /// </summary>
        /// <param name="score">The score to submit.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void SubmitScore(long score, Action<CloudRequestResult<bool>> onComplete = null)
        {
            CloudOnceUtils.LeaderboardUtils.SubmitScore(ID, score, onComplete, internalID);
        }

        /// <summary>
        /// Shows the native leaderboards overlay for this specific leaderboard.
        /// </summary>
        public void ShowOverlay()
        {
            CloudOnceUtils.LeaderboardUtils.ShowOverlay(ID, internalID);
        }

        /// <summary>
        /// Load a default set of scores from this leaderboard.
        /// </summary>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(Action<IScore[]> callback)
        {
            CloudOnceUtils.LeaderboardUtils.LoadScores(ID, callback);
        }

        #endregion /Public methods
    }
}
