// <copyright file="ILeaderboardUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Utils
{
    using System;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Interface for platform-agnostic leaderboard utilities
    /// </summary>
    public interface ILeaderboardUtils
    {
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
        void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        void ShowOverlay(string id = "", string internalID = "");

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        void LoadScores(string leaderboardID, Action<IScore[]> callback);
    }
}
