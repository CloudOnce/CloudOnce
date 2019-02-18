// <copyright file="Cloud.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce
{
    using System;
    using Internal;
    using Internal.Providers;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Access point to the current platform-specific Cloud service provider
    /// </summary>
    public static class Cloud
    {
        private static readonly CloudOnceEvents s_cloudOnceEvents;
        private static Interval s_autoLoadInterval = Interval.Disabled;
        private static bool s_isProviderInitialized;

        static Cloud()
        {
            s_cloudOnceEvents = new CloudOnceEvents();
            Achievements = new GenericAchievementsWrapper();
            Leaderboards = new GenericLeaderboardsWrapper();
        }

        /// <summary>
        /// Raised once the current platforms Initialize method is done.
        /// </summary>
        public static event UnityAction OnInitializeComplete
        {
            add { s_cloudOnceEvents.OnInitializeComplete += value; }
            remove { s_cloudOnceEvents.OnInitializeComplete -= value; }
        }

        /// <summary>
        /// Raised after player signed in status changed. Parameter indicates whether the player was signed in or out.
        /// </summary>
        public static event UnityAction<bool> OnSignedInChanged
        {
            add { s_cloudOnceEvents.OnSignedInChanged += value; }
            remove { s_cloudOnceEvents.OnSignedInChanged -= value; }
        }

        /// <summary>
        /// Raised after an unsuccessful sign in attempt.
        /// </summary>
        public static event UnityAction OnSignInFailed
        {
            add { s_cloudOnceEvents.OnSignInFailed += value; }
            remove { s_cloudOnceEvents.OnSignInFailed -= value; }
        }

        /// <summary>
        /// Raised if the currently signed in player's profile picture is successfully downloaded.
        /// </summary>
        public static event UnityAction<Texture2D> OnPlayerImageDownloaded
        {
            add { s_cloudOnceEvents.OnPlayerImageDownloaded += value; }
            remove { s_cloudOnceEvents.OnPlayerImageDownloaded -= value; }
        }

        /// <summary>
        /// Raised after an attempt has been made to save cloud data. Parameter indicates success.
        /// </summary>
        public static event UnityAction<bool> OnCloudSaveComplete
        {
            add { s_cloudOnceEvents.OnCloudSaveComplete += value; }
            remove { s_cloudOnceEvents.OnCloudSaveComplete -= value; }
        }

        /// <summary>
        /// Raised after an attempt has been made to load cloud data. Parameter indicates success.
        /// </summary>
        public static event UnityAction<bool> OnCloudLoadComplete
        {
            add { s_cloudOnceEvents.OnCloudLoadComplete += value; }
            remove { s_cloudOnceEvents.OnCloudLoadComplete -= value; }
        }

        /// <summary>
        /// Raised if local data is changed as a result of loading cloud data. Returns a <see cref="string"/> array of the changed internal IDs.
        /// </summary>
        public static event UnityAction<string[]> OnNewCloudValues
        {
            add { s_cloudOnceEvents.OnNewCloudValues += value; }
            remove { s_cloudOnceEvents.OnNewCloudValues -= value; }
        }

        /// <summary>
        /// Name of the currently used service, e.g. "Apple Game Center".
        /// </summary>
        public static string ServiceName
        {
            get { return Provider.ServiceName; }
        }

        /// <summary>
        /// ID for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public static string PlayerID
        {
            get { return Provider.PlayerID; }
        }

        /// <summary>
        /// Display name for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public static string PlayerDisplayName
        {
            get { return Provider.PlayerDisplayName; }
        }

        /// <summary>
        /// Profile picture for currently signed in player. Will return <c>null</c> if the user has no avatar, or it has not loaded yet.
        /// </summary>
        public static Texture2D PlayerImage
        {
            get { return Provider.PlayerImage; }
        }

        /// <summary>
        /// Whether or not the user is currently signed in.
        /// </summary>
        public static bool IsSignedIn
        {
            get { return Provider.IsSignedIn; }
        }

        /// <summary>
        /// Whether or not Cloud Save is enabled.
        /// Disabling Cloud Save will make <c>Cloud.Storage.Save</c> only save to disk.
        /// Can only be enabled if Cloud Save was initialized in <see cref="Cloud.Initialize"/> method.
        /// </summary>
        public static bool CloudSaveEnabled
        {
            get { return Provider.CloudSaveEnabled; }
            set { Provider.CloudSaveEnabled = value; }
        }

        /// <summary>
        /// How often to check for new cloud data.
        /// </summary>
        public static Interval AutoLoadInterval
        {
            get { return s_autoLoadInterval; }
            set { s_autoLoadInterval = value; }
        }

        /// <summary>
        /// Interface for accessing leaderboards on the current platform.
        /// </summary>
        public static GenericLeaderboardsWrapper Leaderboards { get; private set; }

        /// <summary>
        /// Interface for accessing achievements on the current platform.
        /// </summary>
        public static GenericAchievementsWrapper Achievements { get; private set; }

        /// <summary>
        /// Interface for accessing cloud save on the current platform.
        /// </summary>
        public static ICloudStorageProvider Storage
        {
            get { return Provider.Storage; }
        }

        private static ICloudProvider Provider
        {
            get
            {
#if UNITY_EDITOR // If running in the Unity Editor, use the TestProvider
                if (!s_isProviderInitialized)
                {
                    TestProvider.Instance.InternalInit(s_cloudOnceEvents);
                    s_isProviderInitialized = true;
                }

                return TestProvider.Instance;
#elif UNITY_ANDROID
#if CLOUDONCE_GOOGLE // If Google is selected as current Android platform in CloudOnce Editor
                if (!s_isProviderInitialized)
                {
                    GooglePlayGamesCloudProvider.Instance.InternalInit(s_cloudOnceEvents);
                    s_isProviderInitialized = true;
                }

                return GooglePlayGamesCloudProvider.Instance;
#else           // In case the configuration has not been saved yet
                Debug.LogWarning("Using dummy provider on Android. Configuration has not been saved correctly.\n"
                    + "Please go to the CloudOnce Editor and save the configuration.");
                if (!s_isProviderInitialized)
                {
                    DummyProvider.Instance.InternalInit(s_cloudOnceEvents);
                    s_isProviderInitialized = true;
                }

                return DummyProvider.Instance;
#endif
#elif UNITY_IOS || UNITY_TVOS // If iOS is selected as current platform
                if (!s_isProviderInitialized)
                {
                    iOSCloudProvider.Instance.InternalInit(s_cloudOnceEvents);
                    s_isProviderInitialized = true;
                }

                return iOSCloudProvider.Instance;
#else           // If the current platform is not supported, use the dummy provider
                if (!s_isProviderInitialized)
                {
                    DummyProvider.Instance.InternalInit(s_cloudOnceEvents);
                    s_isProviderInitialized = true;
                }

                return DummyProvider.Instance;
#endif
            }
        }

        /// <summary>
        /// Initializes the current cloud provider.
        /// </summary>
        /// <param name="activateCloudSave">Whether or not Cloud Saving should be activated.</param>
        /// <param name="autoSignIn">
        /// Whether or not <see cref="SignIn"/> will be called automatically once the cloud provider is initialized.
        /// </param>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically if the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        public static void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
        {
            Provider.Initialize(activateCloudSave, autoSignIn, autoCloudLoad);
        }

        /// <summary>
        /// Signs in to the current cloud provider.
        /// </summary>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically when the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        /// <param name='callback'>
        /// The callback to call when authentication finishes. It will be called
        /// with <c>true</c> if authentication was successful, <c>false</c> otherwise.
        /// </param>
        public static void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
        {
            Provider.SignIn(autoCloudLoad, callback);
        }

        /// <summary>
        /// Signs out of the current cloud provider.
        /// </summary>
        public static void SignOut()
        {
            Provider.SignOut();
        }

        /// <summary>
        /// Load the user profiles associated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        public static void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            Provider.LoadUsers(userIDs, callback);
        }
    }
}
