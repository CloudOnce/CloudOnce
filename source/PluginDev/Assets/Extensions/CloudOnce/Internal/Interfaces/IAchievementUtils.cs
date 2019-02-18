// <copyright file="IAchievementUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Utils
{
    using System;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Interface for platform-agnostic achievement utilities
    /// </summary>
    public interface IAchievementUtils
    {
        /// <summary>
        /// Unlocks an achievement.
        /// </summary>
        /// <param name="id">Current platform's ID for the achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        void Unlock(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

        /// <summary>
        /// Reveals an achievement.
        /// </summary>
        /// <param name="id">Current platform's ID for the achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        void Reveal(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

        /// <summary>
        /// Increments an achievement.
        /// </summary>
        /// <param name="id">Current platform's ID for the achievement.</param>
        /// <param name="progress">
        /// The current progress of this achievement, represented as a number between 0.0 and 100.0.
        /// </param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        void Increment(string id, double progress, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        void ShowOverlay();

        /// <summary>
        /// Loads the achievement descriptions associated with this application.
        /// </summary>
        /// <param name="callback">Callback to handle the achievement descriptions.</param>
        void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback);

        /// <summary>
        /// Load the achievements the logged in user has already achieved or reported progress on.
        /// </summary>
        /// <param name="callback">Callback to handle the achievements.</param>
        void LoadAchievements(Action<IAchievement[]> callback);
    }
}