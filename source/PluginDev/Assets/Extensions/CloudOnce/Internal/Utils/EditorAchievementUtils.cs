// <copyright file="EditorAchievementUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Utils
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Unity Editor implementation of <see cref="IAchievementUtils"/>
    /// </summary>
    public class EditorAchievementUtils : IAchievementUtils
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

            OnReportCompleted(true, onComplete, unlockAction, id, internalID);
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

            OnReportCompleted(true, onComplete, revealAction, id, internalID);
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

            OnReportCompleted(true, onComplete, incrementAction, id, internalID);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        public void ShowOverlay()
        {
            Debug.LogWarning("Achievements overlay is not supported in the Unity Editor.");
        }

        /// <summary>
        /// Loads the achievement descriptions associated with this application.
        /// </summary>
        /// <param name="callback">Callback to handle the achievement descriptions.</param>
        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            CloudOnceUtils.SafeInvoke(callback, GetTestAchievementDescriptions());
        }

        /// <summary>
        /// Load the achievements the logged in user has already achieved or reported progress on.
        /// </summary>
        /// <param name="callback">Callback to handle the achievements.</param>
        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            CloudOnceUtils.SafeInvoke(callback, GetTestAchievements());
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

        private static void OnReportCompleted(bool response, Action<CloudRequestResult<bool>> callbackAction, string action, string id, string internalID)
        {
            if (response)
            {
#if CLOUDONCE_DEBUG
                Debug.Log(string.Format("Achievement {0} ({1}) was successfully {2}ed.", internalID, id, action));
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

        private static IAchievementDescription[] GetTestAchievementDescriptions()
        {
            return (from property in typeof(Achievements).GetProperties()
                    where property.PropertyType == typeof(UnifiedAchievement)
                    select (IAchievementDescription)new TestAchievementDescription(property)).ToArray();
        }

        private static IAchievement[] GetTestAchievements()
        {
            return (from property in typeof(Achievements).GetProperties()
                where property.PropertyType == typeof(UnifiedAchievement)
                select (IAchievement)new TestAchievement(property)).ToArray();
        }

        #endregion / Private methods

        #region Test classes

        /// <summary>
        /// Implementation of <see cref="IAchievement"/> for Unity editor testing.
        /// </summary>
        private class TestAchievement : IAchievement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestAchievement"/> class.
            /// </summary>
            public TestAchievement(PropertyInfo property)
            {
                var achievement = (UnifiedAchievement)property.GetValue(null, null);
                id = achievement.ID;
                percentCompleted = achievement.Progress;
                completed = achievement.IsUnlocked;
                hidden = false;
                lastReportedDate = DateTime.Now;
            }

            /// <summary>
            /// The unique identifier of this achievement.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// Progress for this achievement.
            /// </summary>
            public double percentCompleted { get; set; }

            /// <summary>
            /// Set to true when percentCompleted is 100.0.
            /// </summary>
            public bool completed { get; private set; }

            /// <summary>
            /// This achievement is currently hidden from the user.
            /// </summary>
            public bool hidden { get; private set; }

            /// <summary>
            /// Set by server when percentCompleted is updated.
            /// </summary>
            public DateTime lastReportedDate { get; private set; }

            /// <summary>
            /// Send notification about progress on this achievement.
            /// </summary>
            /// <param name="callback">Callback indicating whether report is successful or not.</param>
            public void ReportProgress(Action<bool> callback)
            {
                CloudOnceUtils.SafeInvoke(callback, true);
            }
        }

        /// <summary>
        /// Implementation of <see cref="IAchievementDescription"/> for Unity editor testing.
        /// </summary>
        private class TestAchievementDescription : IAchievementDescription
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestAchievementDescription"/> class.
            /// </summary>
            public TestAchievementDescription(PropertyInfo property)
            {
                var achievement = (UnifiedAchievement)property.GetValue(null, null);
                id = achievement.ID;
                title = property.Name;
                image = Texture2D.whiteTexture;
                achievedDescription = "Test description for " + property.Name + ".";
                unachievedDescription = achievedDescription;
                hidden = false;
                points = 0;
            }

            /// <summary>
            /// Unique identifier for this achievement description.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// Human readable title.
            /// </summary>
            public string title { get; private set; }

            /// <summary>
            /// Image representation of the achievement.
            /// </summary>
            public Texture2D image { get; private set; }

            /// <summary>
            /// Description when the achievement is completed.
            /// </summary>
            public string achievedDescription { get; private set; }

            /// <summary>
            /// Description when the achievement has not been completed.
            /// </summary>
            public string unachievedDescription { get; private set; }

            /// <summary>
            /// Hidden achievement are not shown in the list until the percentCompleted has been touched (even if it's 0.0).
            /// </summary>
            public bool hidden { get; private set; }

            /// <summary>
            /// Point value of this achievement.
            /// </summary>
            public int points { get; private set; }
        }

        #endregion /Test classes
    }
}
