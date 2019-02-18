// <copyright file="GenericAchievementsWrapper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Providers
{
    using System;
    using UnityEngine.SocialPlatforms;
    using Utils;

    /// <summary>
    /// Provides access to achievement functionality.
    /// </summary>
    public class GenericAchievementsWrapper
    {
        /// <summary>
        /// Unlocks an achievement.
        /// </summary>
        /// <param name="achievementId">Current platform's ID for the achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void UnlockAchievement(string achievementId, Action<CloudRequestResult<bool>> onComplete = null)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log(string.Format("Attempting to unlock {0}.", achievementId));
#endif
            Action<CloudRequestResult<bool>> callback = response =>
            {
                OnUpdateAchievementCompleted(response, onComplete);
            };

            CloudOnceUtils.AchievementUtils.Unlock(achievementId, callback);
        }

        /// <summary>
        /// Reveals a hidden achievement.
        /// </summary>
        /// <param name="achievementId">Current platform's ID for the achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void RevealAchievement(string achievementId, Action<CloudRequestResult<bool>> onComplete = null)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log(string.Format("Attempting to reveal {0}.", achievementId));
#endif
            Action<CloudRequestResult<bool>> callback = response =>
            {
                OnUpdateAchievementCompleted(response, onComplete);
            };

            CloudOnceUtils.AchievementUtils.Reveal(achievementId, callback);
        }

        /// <summary>
        /// Increments an achievement.
        /// <para>Will calculate progress from <paramref name="current"/> and <paramref name="goal"/>.</para>
        /// <para>Progress of 0.0 will reveal the achievement and 100.0 will unlock it.</para>
        /// </summary>
        /// <param name="achievementId">Current platform's ID for the achievement.</param>
        /// <param name="current">Current progress of achievement.</param>
        /// <param name="goal">Total amount of steps to unlock achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void IncrementAchievement(string achievementId, double current, double goal, Action<CloudRequestResult<bool>> onComplete = null)
        {
            IncrementAchievement(achievementId, current / goal * 100.0, onComplete);
        }

        /// <summary>
        /// Increments an achievement.
        /// <para>Can also be used to reveal or unlock this achievement.</para>
        /// <para>A <paramref name="progress"/> of 0.0 will reveal the achievement and 100.0 will unlock it.</para>
        /// </summary>
        /// <param name="achievementId">Current platform's ID for the achievement.</param>
        /// <param name="progress">
        /// The current progress of this achievement, represented as a number between 0.0 and 100.0.
        /// A progress of 0.0 will reveal the achievement and 100.0 will unlock it.
        /// <para> </para>
        /// This value is interpreted as the total percentage of the achievement's progress that the
        /// player should have as a result of this call (regardless of the progress they had before).
        /// So if the player's previous progress was 30% and this call specifies 50.0, the new progress
        /// will be 50% (not 80%).
        /// </param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void IncrementAchievement(string achievementId, double progress, Action<CloudRequestResult<bool>> onComplete)
        {
            if (progress < 0.0)
            {
                throw new ArgumentException("Value must not be negative!", "progress");
            }

            if (progress.Equals(0.0))
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Progress equals 0.0.");
#endif
                RevealAchievement(achievementId, onComplete);
            }
            else if (progress >= 100.0)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Progress equals 100.0 or more.");
#endif
                UnlockAchievement(achievementId, onComplete);
            }
            else
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.Format("Attempting to increment {0} to {1:F2}%.", achievementId, progress));
#endif
                Action<CloudRequestResult<bool>> callback = response =>
                {
                    OnUpdateAchievementCompleted(response, onComplete);
                };

                CloudOnceUtils.AchievementUtils.Increment(achievementId, progress, callback);
            }
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        public void ShowOverlay()
        {
            CloudOnceUtils.AchievementUtils.ShowOverlay();
        }

        /// <summary>
        /// Loads the achievement descriptions associated with this application.
        /// </summary>
        /// <param name="callback">Callback to handle the achievement descriptions.</param>
        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            CloudOnceUtils.AchievementUtils.LoadAchievementDescriptions(callback);
        }

        /// <summary>
        /// Load the achievements the logged in user has already achieved or reported progress on.
        /// </summary>
        /// <param name="callback">Callback to handle the achievements.</param>
        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            CloudOnceUtils.AchievementUtils.LoadAchievements(callback);
        }

        private void OnUpdateAchievementCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
        {
            var result = response.Result ? new CloudRequestResult<bool>(true) : new CloudRequestResult<bool>(false, response.Error);
            CloudOnceUtils.SafeInvoke(callbackAction, result);
        }
    }
}
