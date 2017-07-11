// <copyright file="AmazonAchievementUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && CLOUDONCE_AMAZON
namespace CloudOnce.Internal.Utils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Amazon GameCircle implementation of <see cref="IAchievementUtils"/>
    /// </summary>
    public class AmazonAchievementUtils : IAchievementUtils
    {
        #region Fields & properties

        private const string unlockAction = "unlock";
        private const string incrementAction = "increment";
        private readonly Dictionary<int, Action<IAchievementDescription[]>> loadAchievementDescriptionsCallbacks;
        private readonly Dictionary<int, Action<IAchievement[]>> loadAchievementsCallbacks;

        #endregion /Fields & properties

        public AmazonAchievementUtils()
        {
            loadAchievementDescriptionsCallbacks = new Dictionary<int, Action<IAchievementDescription[]>>();
            loadAchievementsCallbacks = new Dictionary<int, Action<IAchievement[]>>();
            Cloud.OnSignedInChanged += OnSignedInChanged;
        }

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

            if (!AGSPlayerClient.IsSignedIn())
            {
                var authenticationError = string.IsNullOrEmpty(internalID)
                    ? string.Format("Can't unlock {0}. UnlockAchievement can only be called after authentication.", id)
                    : string.Format("Can't unlock {0} ({1}). Unlock can only be called after authentication.", internalID, id);
                ReportError(authenticationError, onComplete);
                return;
            }

            Action<AGSUpdateAchievementResponse> callback = null;
            callback = response =>
            {
                OnReportCompleted(response, onComplete, unlockAction, id, internalID);
                AGSAchievementsClient.UpdateAchievementCompleted -= callback;
            };

            AGSAchievementsClient.UpdateAchievementCompleted += callback;
            AGSAchievementsClient.UpdateAchievementProgress(id, 100f);
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
            var error = string.Format(
                "Can't reveal {0} ({1}). Reveal method is not supported on Amazon GameCircle."
              + "\nOnly way to reveal a hidden achievement on Amazon GameCircle is to unlock it.", internalID, id);
            ReportError(error, onComplete);
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

            if (!AGSPlayerClient.IsSignedIn())
            {
                var authenticationError = string.IsNullOrEmpty(internalID)
                    ? string.Format("Can't increment {0}. IncrementAchievement can only be called after authentication.", id)
                    : string.Format("Can't increment {0} ({1}). Increment can only be called after authentication.", internalID, id);
                ReportError(authenticationError, onComplete);
                return;
            }

            Action<AGSUpdateAchievementResponse> callback = null;
            callback = response =>
            {
                OnReportCompleted(response, onComplete, incrementAction, id, internalID);
                AGSAchievementsClient.UpdateAchievementCompleted -= callback;
            };

            AGSAchievementsClient.UpdateAchievementCompleted += callback;
            AGSAchievementsClient.UpdateAchievementProgress(id, (float)progress);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        public void ShowOverlay()
        {
            if (!AGSPlayerClient.IsSignedIn())
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning("ShowOverlay can only be called after authentication.");
#endif
                return;
            }
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Showing achievements overlay.");
#endif
            AGSAchievementsClient.ShowAchievementsOverlay();
        }

        /// <summary>
        /// Loads the achievement descriptions accociated with this application.
        /// </summary>
        /// <param name="callback">Callback to handle the achievement descriptions.</param>
        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            loadAchievementDescriptionsCallbacks.Add(AmazonLeaderboardUtils.RequestID, callback);
            AGSAchievementsClient.RequestAchievements(AmazonLeaderboardUtils.RequestID++);
        }

        /// <summary>
        /// Load the achievements the logged in user has already achieved or reported progress on.
        /// </summary>
        /// <param name="callback">Callback to handle the achievements.</param>
        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            loadAchievementsCallbacks.Add(AmazonLeaderboardUtils.RequestID, callback);
            AGSAchievementsClient.RequestAchievements(AmazonLeaderboardUtils.RequestID++);
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

        private static void OnReportCompleted(
            AGSRequestResponse response, Action<CloudRequestResult<bool>> callbackAction, string action, string id, string internalID)
        {
            if (!response.IsError())
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
                        ? string.Format("Native API failed to {0} achievement {1}. Reported error: {2}", action, id, response.error)
                        : string.Format("Native API failed to {0} achievement {1} ({2}). Reported error: {3}", action, internalID, id, response.error);
                ReportError(error, callbackAction);
            }
        }

        private void OnSignedInChanged(bool isSignedIn)
        {
            if (isSignedIn)
            {
                AGSAchievementsClient.RequestAchievementsCompleted += OnRequestAchievementsCompleted;
            }
            else
            {
                AGSAchievementsClient.RequestAchievementsCompleted -= OnRequestAchievementsCompleted;
            }
        }

        private void OnRequestAchievementsCompleted(AGSRequestAchievementsResponse response)
        {
            if (loadAchievementDescriptionsCallbacks.ContainsKey(response.userData))
            {
                var callback = loadAchievementDescriptionsCallbacks[response.userData];
                if (callback != null)
                {
                    var descriptions = new IAchievementDescription[response.achievements.Count];
                    for (var i = 0; i < response.achievements.Count; i++)
                    {
                        descriptions[i] = new AGSSocialAchievement(response.achievements[i]);
                    }

                    callback(descriptions);
                }

                loadAchievementDescriptionsCallbacks.Remove(response.userData);
            }
            else if (loadAchievementsCallbacks.ContainsKey(response.userData))
            {
                var callback = loadAchievementsCallbacks[response.userData];
                if (callback != null)
                {
                    var achievements = new IAchievement[response.achievements.Count];
                    for (var i = 0; i < response.achievements.Count; i++)
                    {
                        achievements[i] = new AGSSocialAchievement(response.achievements[i]);
                    }

                    callback(achievements);
                }

                loadAchievementsCallbacks.Remove(response.userData);
            }
        }

        #endregion / Private methods
    }
}
#endif
