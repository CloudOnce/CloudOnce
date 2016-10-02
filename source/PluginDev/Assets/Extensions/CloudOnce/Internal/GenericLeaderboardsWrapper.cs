// <copyright file="GenericLeaderboardsWrapper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Providers
{
    using System;
    using UnityEngine.SocialPlatforms;
    using Utils;

    /// <summary>
    /// Provides access to leaderboard functionality.
    /// </summary>
    public class GenericLeaderboardsWrapper
    {
        /// <summary>
        /// Reports a score to a leaderboard.
        /// </summary>
        /// <param name="leaderboardId">Current platform's ID for the leaderboard.</param>
        /// <param name="score">The score to submit.</param>
        /// <param name="onComplete">
        ///  Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        ///  If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void SubmitScore(string leaderboardId, long score, Action<CloudRequestResult<bool>> onComplete = null)
        {
            CloudOnceUtils.LeaderboardUtils.SubmitScore(leaderboardId, score, onComplete);
        }

        /// <summary>
        /// Shows the native leaderboards user interface, allowing the player to browse the leaderboards.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        public void ShowOverlay(string leaderboardID = "")
        {
            CloudOnceUtils.LeaderboardUtils.ShowOverlay(leaderboardID);
        }

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            CloudOnceUtils.LeaderboardUtils.LoadScores(leaderboardID, callback);
        }
    }
}
