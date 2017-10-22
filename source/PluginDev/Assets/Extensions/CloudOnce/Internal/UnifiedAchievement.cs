// <copyright file="UnifiedAchievement.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using Utils;

    /// <summary>
    /// A cross-platform achievement.
    /// Class has methods for unlocking, revealing and incrementing the achievement that will work for all supported platforms.
    /// Only intended to be used by CloudOnce internal classes.
    /// </summary>
    public class UnifiedAchievement
    {
        #region Fields & properties

        private readonly string internalID;
        private bool isAchievementHidden = true;
        private double achievementProgress;

        /// <summary>
        /// A cross-platform achievement.
        /// Class has methods for unlocking, revealing and incrementing the achievement that will work for all supported platforms.
        /// </summary>
        /// <param name="internalID">The internal ID for this achievement.</param>
        /// <param name="platformID">The current platform ID for this achievement. Set to <c>string.Empty</c> if platform is not supported.</param>
        public UnifiedAchievement(string internalID, string platformID)
        {
            this.internalID = internalID;
            ID = platformID;
        }

        /// <summary>
        /// The ID for the current platform.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Whether or not this achievement has been unlocked.
        /// </summary>
        public bool IsUnlocked { get; private set; }

        /// <summary>
        /// The current progress of this achievement, represented as a number between 0.0 and 100.0.
        /// </summary>
        public double Progress
        {
            get
            {
                return achievementProgress;
            }

            private set
            {
                if (value < achievementProgress)
                {
                    return;
                }

                achievementProgress = value > 100.0 ? 100.0 : value;
            }
        }

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Unlocks this achievement.
        /// </summary>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void Unlock(Action<CloudRequestResult<bool>> onComplete = null)
        {
            if (!IsUnlocked)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.Format("Attempting to unlock {0} ({1}).", internalID, ID));
#endif
                Action<CloudRequestResult<bool>> callback = response =>
                    {
                        OnUnlockCompleted(response, onComplete);
                    };

                CloudOnceUtils.AchievementUtils.Unlock(ID, callback, internalID);
            }
            else
            {
                var error = string.Format("Can't unlock {0}. Achievement has already been unlocked.", ID);
                ReportError(error, onComplete);
            }
        }

        /// <summary>
        /// Reveals this achievement.
        /// </summary>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void Reveal(Action<CloudRequestResult<bool>> onComplete = null)
        {
            if (isAchievementHidden)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.Format("Attempting to reveal {0} ({1}).", internalID, ID));
#endif
                Action<CloudRequestResult<bool>> callback = response =>
                {
                    OnRevealCompleted(response, onComplete);
                };

                CloudOnceUtils.AchievementUtils.Reveal(ID, callback, internalID);
            }
            else
            {
                var error = string.Format("Can't reveal {0}. Achievement has already been revealed.", ID);
                ReportError(error, onComplete);
            }
        }

        /// <summary>
        ///  Increments this achievement.
        ///  <para>Will calculate progress from <paramref name="current"/> and <paramref name="goal"/>.</para>
        ///  <para>Progress of 0.0 will reveal the achievement and 100.0 will unlock it.</para>
        /// </summary>
        /// <param name="current">Current progress of achievement.</param>
        /// <param name="goal">Total amount of steps to unlock achievement.</param>
        /// <param name="onComplete">
        ///  Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        ///  If <c>false</c>, an error message will be included in the callback.
        /// </param>
        public void Increment(double current, double goal, Action<CloudRequestResult<bool>> onComplete = null)
        {
            Increment(current / goal * 100.0, onComplete);
        }

        /// <summary>
        /// Increments this achievement.
        /// <para>Can also be used to reveal or unlock this achievement.</para>
        /// <para>A <paramref name="progress"/> of 0.0 will reveal the achievement and 100.0 will unlock it.</para>
        /// </summary>
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
        public void Increment(double progress, Action<CloudRequestResult<bool>> onComplete = null)
        {
            if (IsUnlocked)
            {
                var error = string.Format("Can't increment {0} ({1}). Achievement is already unlocked.", internalID, ID);
                ReportError(error, onComplete);
            }
            else
            {
                if (progress < 0.0)
                {
                    throw new ArgumentException("Value must not be negative!", "progress");
                }

                if (progress.Equals(0.0))
                {
#if CLOUDONCE_DEBUG
                    UnityEngine.Debug.Log("Progress equals 0.0. Revealing achievement.");
#endif
                    Reveal(onComplete);
                }
                else if (progress >= 100.0)
                {
#if CLOUDONCE_DEBUG
                    UnityEngine.Debug.Log("Progress equals 100.0 or more. Unlocking achievement.");
#endif
                    Unlock(onComplete);
                }
                else if (progress <= Progress)
                {
                    var error = string.Format("Can't increment {0} ({1}) to {2:F2}%. Achievement is already at {3:F2}%.", internalID, ID, progress, Progress);
                    ReportError(error, onComplete);
                }
                else
                {
#if CLOUDONCE_DEBUG
                    UnityEngine.Debug.Log(string.Format("Attempting to increment {0} ({1}) to {2:F2}%.", internalID, ID, progress));
#endif
                    Action<CloudRequestResult<bool>> callback = response =>
                    {
                        OnIncrementCompleted(response, progress, onComplete);
                    };

                    CloudOnceUtils.AchievementUtils.Increment(ID, progress, callback, internalID);
                }
            }
        }

        /// <summary>
        /// Updates locked status and progress with data received from native API.
        /// <para>Only intended to be used by internal CloudOnce systems.</para>
        /// </summary>
        /// <param name="isUnlocked">Whether or not this achievement has been unlocked.</param>
        /// <param name="progress">The current progress of this achievement, represented as a number between 0.0 and 100.0.</param>
        /// <param name="isHidden">Whether or not this achievement is hidden.</param>
        public void UpdateData(bool isUnlocked, double progress, bool isHidden)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log(string.Format("Updating data for {0} ({1}) achievement.", internalID, ID));
#endif
            if (IsUnlocked && !isUnlocked)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning(
                    string.Format("Inconsistency detected between local and remote unlocked status. Achievement {0} ({1}) should be unlocked. Attempting to correct.", internalID, ID));
#endif
                Action<CloudRequestResult<bool>> callback = response =>
                {
                    OnUnlockCompleted(response, null);
                };

                CloudOnceUtils.AchievementUtils.Unlock(ID, callback, internalID);
                return;
            }

            if (Progress > progress)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning(
                    string.Format("Inconsistency detected between local and remote progress status. Achievement {0} ({1}) should be at {2:F2}%. Attempting to correct.", internalID, ID, Progress));
#endif
                Action<CloudRequestResult<bool>> callback = response =>
                {
                    OnIncrementCompleted(response, progress, null);
                };

                CloudOnceUtils.AchievementUtils.Increment(ID, progress, callback, internalID);
                return;
            }

            IsUnlocked = isUnlocked;
            Progress = progress;
            isAchievementHidden = isHidden;
            if (!IsUnlocked && Progress.Equals(100.0))
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning(
                    string.Format("Inconsistency detected between progress and unlocked status. Achievement {0} ({1}) should be unlocked. Attempting to correct.", internalID, ID));
#endif
                Action<CloudRequestResult<bool>> callback = response =>
                {
                    OnUnlockCompleted(response, null);
                };

                CloudOnceUtils.AchievementUtils.Unlock(ID, callback, internalID);
            }
        }

        public void ResetLocalState()
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log(string.Format("Resetting local state for achievement {0} ({1})", internalID, ID));
#endif
            IsUnlocked = false;
            isAchievementHidden = true;
            achievementProgress = 0.0;
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

        private void OnUnlockCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
        {
            if (response.Result)
            {
                IsUnlocked = true;
                isAchievementHidden = false;
                Progress = 100.0;
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, response.Error));
            }
        }

        private void OnRevealCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
        {
            if (response.Result)
            {
                isAchievementHidden = false;
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, response.Error));
            }
        }

        private void OnIncrementCompleted(CloudRequestResult<bool> response, double progress, Action<CloudRequestResult<bool>> callbackAction)
        {
            if (response.Result)
            {
                Progress = progress;
#if UNITY_IOS || UNITY_TVOS
                isAchievementHidden = false;
#endif
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, response.Error));
            }
        }

        #endregion /Private methods
    }
}
