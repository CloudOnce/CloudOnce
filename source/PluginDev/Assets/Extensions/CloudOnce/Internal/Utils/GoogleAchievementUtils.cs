// <copyright file="GoogleAchievementUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
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
    /// Google Play Game Services implementation of <see cref="IAchievementUtils"/>
    /// </summary>
    public class GoogleAchievementUtils : IAchievementUtils
    {
        #region Fields & properties

        private const string unlockAction = "unlock";
        private const string revealAction = "reveal";
        private const string incrementAction = "increment";

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Unlocks an achievement.
        /// </summary>
        /// <param name="id">Current platform's ID for the achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void Unlock(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ReportError("Can't unlock achievement. Supplied ID is null or empty!", onComplete);
                return;
            }

            if (!GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized)
            {
                var authenticationError = string.IsNullOrEmpty(internalID)
                    ? string.Format("Can't unlock {0}. UnlockAchievement can only be called after authentication.", id)
                    : string.Format("Can't unlock {0} ({1}). Unlock can only be called after authentication.", internalID, id);
                ReportError(authenticationError, onComplete);
                return;
            }

            Action<bool> callback = response => OnReportCompleted(response, onComplete, unlockAction, id, internalID);
            PlayGamesPlatform.Instance.ReportProgress(id, 100.0, callback);
        }

        /// <summary>
        /// Reveals an achievement.
        /// </summary>
        /// <param name="id">Current platform's ID for the achievement.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void Reveal(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ReportError("Can't reveal achievement. Supplied ID is null or empty!", onComplete);
                return;
            }

            if (!GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized)
            {
                var authenticationError = string.IsNullOrEmpty(internalID)
                    ? string.Format("Can't reveal {0}. RevealAchievement can only be called after authentication.", id)
                    : string.Format("Can't reveal {0} ({1}). Reveal can only be called after authentication.", internalID, id);
                ReportError(authenticationError, onComplete);
                return;
            }

            Action<bool> callback = response => OnReportCompleted(response, onComplete, revealAction, id, internalID);
            PlayGamesPlatform.Instance.ReportProgress(id, 0.0, callback);
        }

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
        public void Increment(string id, double progress, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ReportError("Can't increment achievement. Supplied ID is null or empty!", onComplete);
                return;
            }

            if (!GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized)
            {
                var authenticationError = string.IsNullOrEmpty(internalID)
                    ? string.Format("Can't increment {0}. IncrementAchievement can only be called after authentication.", id)
                    : string.Format("Can't increment {0} ({1}). Increment can only be called after authentication.", internalID, id);
                ReportError(authenticationError, onComplete);
                return;
            }

            Action<bool> callback = response => OnReportCompleted(response, onComplete, incrementAction, id, internalID);
            PlayGamesPlatform.Instance.ReportProgress(id, progress, callback);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        public void ShowOverlay()
        {
            if (!GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning("ShowOverlay can only be called after authentication.");
#endif
                return;
            }
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Showing achievements overlay.");
#endif
            PlayGamesPlatform.Instance.ShowAchievementsUI(OnShowOverlayCompleted);
        }

        /// <summary>
        /// Loads the achievement descriptions accociated with this application.
        /// </summary>
        /// <param name="callback">Callback to handle the achievement descriptions.</param>
        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            if (!GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning("LoadAchievementDescriptions can only be called after authentication.");
#endif
                return;
            }
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Loading achievement descriptions.");
#endif
            PlayGamesPlatform.Instance.LoadAchievementDescriptions(callback);
        }

        /// <summary>
        /// Load the achievements the logged in user has already achieved or reported progress on.
        /// </summary>
        /// <param name="callback">Callback to handle the achievements.</param>
        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            if (!GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning("LoadAchievements can only be called after authentication.");
#endif
                return;
            }
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Loading achievements.");
#endif
            PlayGamesPlatform.Instance.LoadAchievements(callback);
        }

        #endregion /Public methods

        #region Private methods

        private static void OnShowOverlayCompleted(UIStatus callback)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Achievements overlay closed.");
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

        private static void OnReportCompleted(bool response, Action<CloudRequestResult<bool>> callbackAction, string action, string id, string internalID)
        {
            if (response)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.Format("Achievement {0} ({1}) was successfully {2}ed.", internalID, id, action));
#endif
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                // Customize error message to fit either new or old achievement system.
                var error = string.IsNullOrEmpty(internalID)
                        ? string.Format("Native API failed to {0} achievement {1}. Cause unknown.", action, id)
                        : string.Format("Native API failed to {0} achievement {1} ({2}). Cause unknown.", action, internalID, id);
                ReportError(error, callbackAction);
            }
        }

        #endregion / Private methods
    }
}
#endif
