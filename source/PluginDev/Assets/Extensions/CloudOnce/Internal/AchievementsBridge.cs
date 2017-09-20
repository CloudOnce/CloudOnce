// <copyright file="AchievementsBridge.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_IOS || UNITY_TVOS
namespace CloudOnce.Internal
{
    using System.Runtime.InteropServices;

    public delegate void BoolCallbackDelegate(bool success);
    public delegate void LoadAchievementsDelegate(bool success, int count);

    /// <summary>
    ///  Provides a bridge between C# code and the <c>CloudOnceAchievements</c> iOS plug-in.
    /// </summary>
    public static class AchievementsBridge
    {
        private const string importInternal = "__Internal";

        #region Public methods

        /// <summary>
        /// Loads the local player’s current progress information from Game Center.
        /// Should be called immediately after the player is authenticated.
        /// </summary>
        /// <param name="callback">Callback will be <c>true</c> if the achievements are successfully loaded,
        /// <c>false</c> if there was a problem.</param>
        public static void LoadAchievements(LoadAchievementsDelegate callback)
        {
            _LoadAchievements(callback);
        }

        /// <summary>
        /// Unlocks an achievement.
        /// </summary>
        /// <param name="achievementId">The unique identifier for the achievement. Is defined in the developer console.</param>
        /// <param name="callback">Callback will be <c>true</c> if the achievement is successfully unlocked,
        /// <c>false</c> if there was a problem.</param>
        public static void UnlockAchievement(string achievementId, BoolCallbackDelegate callback)
        {
            _UnlockAchievement(achievementId, callback);
        }

        /// <summary>
        /// Reveals a hidden achievement.
        /// </summary>
        /// <param name="achievementId">The unique identifier for the achievement. Is defined in the developer console.</param>
        /// <param name="callback">Callback will be <c>true</c> if the achievement is successfully revealed,
        /// <c>false</c> if there was a problem.</param>
        public static void RevealAchievement(string achievementId, BoolCallbackDelegate callback)
        {
            _RevealAchievement(achievementId, callback);
        }

        /// <summary>
        /// Increments an achievement.
        /// </summary>
        /// <param name="achievementId">The unique identifier for the achievement. Is defined in the developer console.</param>
        /// <param name="progress">The current player progress, as a percentage. 100 unlocks the achievement. 0 reveals a hidden achievement.</param>
        /// <param name="callback">Callback will be <c>true</c> if the achievement is successfully incremented,
        /// <c>false</c> if there was a problem.</param>
        public static void IncrementAchievement(string achievementId, float progress, BoolCallbackDelegate callback)
        {
            _IncrementAchievement(achievementId, progress, callback);
        }

        #endregion /Public methods

        [DllImport(importInternal)]
        private static extern void _LoadAchievements(LoadAchievementsDelegate callback);

        [DllImport(importInternal)]
        private static extern void _UnlockAchievement(string achievementId, BoolCallbackDelegate callback);

        [DllImport(importInternal)]
        private static extern void _RevealAchievement(string achievementId, BoolCallbackDelegate callback);

        [DllImport(importInternal)]
        private static extern void _IncrementAchievement(string achievementId, float progress, BoolCallbackDelegate callback);
    }
}
#endif
