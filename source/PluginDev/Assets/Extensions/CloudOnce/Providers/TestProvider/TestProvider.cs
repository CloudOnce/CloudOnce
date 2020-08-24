// <copyright file="TestProvider.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Providers
{
    using System;
    using System.Globalization;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SocialPlatforms;
    using Utils;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Provides a minimal implementation of ICloudProvider, for development and testing.
    /// </summary>
    public sealed class TestProvider : CloudProviderBase<TestProvider>
    {
        private CloudOnceEvents cloudOnceEvents;
        private bool cloudSaveEnabled = true;
        private bool isSignedIn;

        /// <summary>
        /// ID for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public override string PlayerID
        {
            get { return "TestPlayerID"; }
        }

        /// <summary>
        /// Display name for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public override string PlayerDisplayName
        {
            get { return "Test Player"; }
        }

        /// <summary>
        /// Profile picture for currently signed in player. Will return <c>null</c> if the user has no avatar, or it has not loaded yet.
        /// </summary>
        public override Texture2D PlayerImage
        {
            get { return Texture2D.whiteTexture; }
        }

        /// <summary>
        /// Whether or not the user is currently signed in.
        /// </summary>
        public override bool IsSignedIn
        {
            get { return isSignedIn; }
        }

        /// <summary>
        /// Whether or not Cloud Save has been initialized. Is initialized with <see cref="Cloud.Initialize"/> method.
        /// </summary>
        public bool CloudSaveInitialized { get; private set; }

        /// <summary>
        /// Whether or not Cloud Save is enabled.
        /// Disabling Cloud Save will make <c>Cloud.Storage.Save</c> only save to disk.
        /// Can only be enabled if Cloud Save was initialized in <see cref="Cloud.Initialize"/> method.
        /// </summary>
        public override bool CloudSaveEnabled
        {
            get
            {
                return cloudSaveEnabled;
            }

            set
            {
                if (!CloudSaveInitialized)
                {
                    Debug.LogWarning("Cloud Save has not been initialized. Call Cloud.Initialize before attempting to set CloudSaveEnabled.");
                    return;
                }

                cloudSaveEnabled = value;
            }
        }

        /// <summary>
        /// Interface for accessing fake cloud save.
        /// </summary>
        public override ICloudStorageProvider Storage { get; protected set; }

        /// <summary>
        /// Emulates initialization with a randomized delay.
        /// </summary>
        /// <param name="activateCloudSave">Whether or not Cloud Saving should be activated.</param>
        /// <param name="autoSignIn">
        /// Whether or not <see cref="SignIn"/> will be called automatically once the cloud provider is initialized.
        /// </param>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically if the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        public override void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
        {
            CloudSaveInitialized = activateCloudSave;
            cloudSaveEnabled = activateCloudSave;

            var delay = Random.Range(0.5f, 2f);
            Debug.Log(string.Format("Simulating random Initialize delay of {0} seconds", delay));

            // Normal MonoBehaviour Invoke uses scalable delta time, some games use timescale to pause and use unscaled time in GUI
            // To Simulate Random Cloud Init Delay right we need to invoke at unscaled time
            if (autoSignIn)
            {
                var onInitializeComplete = new UnityAction(() =>
                {
                    SignIn(autoCloudLoad, arg0 => cloudOnceEvents.RaiseOnInitializeComplete());
                });
                StartCoroutine(CloudOnceUtils.InvokeUnscaledTime(onInitializeComplete, delay));
            }
            else
            {
                StartCoroutine(CloudOnceUtils.InvokeUnscaledTime(cloudOnceEvents.RaiseOnInitializeComplete, delay));
            }

            if (activateCloudSave && autoCloudLoad)
            {
                var loadDelay = Random.Range(delay, delay + 2f);
                Debug.Log(string.Format("Simulating random Cloud load delay of {0} seconds", loadDelay));
                StartCoroutine(CloudOnceUtils.InvokeUnscaledTime(Cloud.Storage.Load, loadDelay));
            }
        }

        /// <summary>
        /// Simulates signing in.
        /// </summary>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically when the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        /// <param name='callback'>
        /// The callback to call when authentication finishes. It will be called
        /// with <c>true</c> if authentication was successful, <c>false</c> otherwise.
        /// </param>
        public override void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
        {
            isSignedIn = true;
            CloudOnceUtils.SafeInvoke(callback, true);
            cloudOnceEvents.RaiseOnSignedInChanged(true);
            var delay = Random.Range(0.5f, 2f);
            Debug.Log(string.Format("Simulating random PlayerImageDownload delay of {0} seconds", delay));
            StartCoroutine(
                CloudOnceUtils.InvokeUnscaledTime(
                    cloudOnceEvents.RaiseOnPlayerImageDownloaded, Texture2D.whiteTexture, delay));
        }

        /// <summary>
        /// Simulates signing out.
        /// </summary>
        public override void SignOut()
        {
            isSignedIn = false;
            cloudOnceEvents.RaiseOnSignedInChanged(false);
        }

        /// <summary>
        /// Load the user profiles associated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            var profiles = new IUserProfile[userIDs.Length];
            for (var i = 0; i < profiles.Length; i++)
            {
                profiles[i] = new TestUserProfile();
            }

            CloudOnceUtils.SafeInvoke(callback, profiles);
        }

        /// <summary>
        /// Method used by <see cref="Cloud"/> to initialize the Cloud provider.
        /// </summary>
        public void InternalInit(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
            DataManager.InitDataManager();
            Storage = new TestProviderStorageWrapper(events);
            ServiceName = "Test Cloud Provider";
#if CLOUDONCE_DEBUG
            Debug.Log("Using " + ServiceName);
#endif
        }

        protected override void OnAwake()
        {
            gameObject.name = GetType().ToString();
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from a specified path.
        /// </summary>
        /// <param name="path">The path where the <see cref="Texture2D"/> is located.</param>
        /// <returns>A <see cref="Texture2D"/> from a specified path.</returns>
        private static Texture2D GetTexture2D(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(SlashesToPlatformSeparator(path));
        }

        /// <summary>
        /// Will convert any slashes ('/') to the current platform's directory separator.
        /// </summary>
        /// <param name="path">The string containing the slashes.</param>
        /// <returns>A string with all the slashes converted to the current platform's directory separator.</returns>
        private static string SlashesToPlatformSeparator(string path)
        {
            return path.Replace("/", Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Implementation of <see cref="IUserProfile"/> used for Unity editor testing.
        /// </summary>
        private class TestUserProfile : IUserProfile
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestUserProfile"/> class.
            /// </summary>
            public TestUserProfile()
            {
                var randomId = Random.Range(1, 99999);
                id = string.Format("{0:D5}", randomId);
#if UNITY_2020_1_OR_NEWER
                gameId = id;
#endif
                userName = "User" + id;
                isFriend = Random.Range(0, 2) == 1;
                state = (UserState)Random.Range(0, 5);
                image = Texture2D.whiteTexture;
            }

            /// <summary>
            /// <para>
            /// This user's username or alias.
            /// </para>
            /// </summary>
            public string userName { get; private set; }

            /// <summary>
            /// <para>
            /// This users unique identifier.
            /// </para>
            /// </summary>
            public string id { get; private set; }

#if UNITY_2020_1_OR_NEWER
            /// <summary>
            /// <para>
            /// This user's unique identifier.
            /// </para>
            /// </summary>
            public string gameId { get; private set; }
#endif

            /// <summary>
            /// <para>
            /// Is this user a friend of the current logged in user?
            /// </para>
            /// </summary>
            public bool isFriend { get; private set; }

            /// <summary>
            /// <para>
            /// Presence state of the user.
            /// </para>
            /// </summary>
            public UserState state { get; private set; }

            /// <summary>
            /// <para>
            /// Avatar image of the user.
            /// </para>
            /// </summary>
            public Texture2D image { get; private set; }
        }
    }
}
#endif
