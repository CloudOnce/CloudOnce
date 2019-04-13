// <copyright file="GooglePlayGamesCloudProvider.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && CLOUDONCE_GOOGLE
namespace CloudOnce.Internal.Providers
{
    using System;
    using System.Collections;
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using UnityEngine;
    using UnityEngine.Events;
#if UNITY_2017_1_OR_NEWER
    using UnityEngine.Networking;
#endif
    using UnityEngine.SocialPlatforms;
    using Utils;
    using Logger = GooglePlayGames.OurUtils.Logger;

    /// <summary>
    /// Google Play Game Services implementation of <see cref="ICloudProvider"/>.
    /// </summary>
    public sealed class GooglePlayGamesCloudProvider : CloudProviderBase<GooglePlayGamesCloudProvider>
    {
        #region Fields, constructor & properties

        private const string guestPreferenceKey = "GooglePlayWantsToUseGuest";
        private CloudOnceEvents cloudOnceEvents;
        private bool cloudSaveEnabled = true;
        private Texture2D playerImage;
        private string playerIdCache;
        private bool autoLoadEnabled;

        /// <summary>
        /// Used to make sure we don't trigger another initialization of the Cloud Provider once one has started.
        /// </summary>
        private bool initializing;

        /// <summary>
        /// Gets or sets whether debug logs for the Google Play Games plugin are enabled.
        /// </summary>
        public static bool DebugLogEnabled
        {
            get { return Logger.DebugLogEnabled; }
            set { Logger.DebugLogEnabled = value; }
        }

        /// <summary>
        /// If game defaults to guest user or not.
        /// </summary>
        public static bool IsGuestUserDefault { get; private set; }

        /// <summary>
        /// ID for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public override string PlayerID
        {
            get { return IsGpgsInitialized ? PlayGamesPlatform.Instance.localUser.id : string.Empty; }
        }

        /// <summary>
        /// Display name for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public override string PlayerDisplayName
        {
            get { return IsGpgsInitialized ? PlayGamesPlatform.Instance.localUser.userName : string.Empty; }
        }

        /// <summary>
        /// Profile picture for currently signed in player. Will return <c>null</c> if the user has no avatar, or it has not loaded yet.
        /// </summary>
        public override Texture2D PlayerImage
        {
            get { return IsGpgsInitialized ? playerImage ?? Texture2D.whiteTexture : Texture2D.whiteTexture; }
        }

        /// <summary>
        /// Whether or not the user is currently signed in.
        /// </summary>
        public override bool IsSignedIn
        {
            get { return IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated(); }
        }

        /// <summary>
        /// Whether or not Cloud Save has been initialized. Is initilized with <see cref="Cloud.Initialize"/> method.
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
        /// Whether or not GPGS has been initlialized yet.
        /// Needed to make sure that it's not initialized by mistake with the wrong settings.
        /// </summary>
        public bool IsGpgsInitialized { get; private set; }

        /// <summary>
        /// Interface for accessing cloud save on Google Play Game Services.
        /// </summary>
        public override ICloudStorageProvider Storage { get; protected set; }

        #endregion /Fields, constructor & properties

        #region Public methods

        /// <summary>
        /// Initializes Google Play Game Services.
        /// </summary>
        /// <param name="activateCloudSave">Whether or not Cloud Saving should be activated.</param>
        /// <param name="autoSignIn">
        /// Whether or not <see cref="SignIn"/> will be called automatically once Google Play Game Services is initialized.
        /// </param>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically if the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        public override void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
        {
            if (initializing)
            {
                return;
            }
#if CLOUDONCE_DEBUG
            Debug.Log("Initializing Google Play Game Services.");
#endif
            initializing = true;

            cloudSaveEnabled = activateCloudSave;

#if CLOUDONCE_DEBUG
            Debug.Log("Saved Games support " + (activateCloudSave ? "enabled." : "disabled."));
#endif
            var config = new PlayGamesClientConfiguration.Builder();
            if (activateCloudSave)
            {
                config.EnableSavedGames();
                CloudSaveInitialized = true;
            }

            PlayGamesPlatform.InitializeInstance(config.Build());

            SubscribeOnAuthenticatedEvent();

#if CLOUDONCE_DEBUG   // Enable/disable logs on the PlayGamesPlatform
            PlayGamesPlatform.DebugLogEnabled = true;
            Debug.Log("PlayGamesPlatform debug logs enabled.");
#else
            PlayGamesPlatform.DebugLogEnabled = false;
            Debug.Log("PlayGamesPlatform debug logs disabled.");
#endif
            IsGpgsInitialized = true;
            if (!IsGuestUserDefault && autoSignIn)
            {
                var onSignedIn = new UnityAction<bool>(arg0 =>
                {
                    cloudOnceEvents.RaiseOnInitializeComplete();
                    initializing = false;
                });
                SignIn(autoCloudLoad, onSignedIn);
            }
            else
            {
                if (IsGuestUserDefault && autoSignIn)
                {
                    Logger.d("Guest user mode active, ignoring auto sign-in. Please call SignIn directly.");
                }

                if (autoCloudLoad)
                {
                    cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                }

                cloudOnceEvents.RaiseOnInitializeComplete();
                initializing = false;
            }
        }

        /// <summary>
        /// Signs in to Google Play Game Services.
        /// </summary>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically if the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        /// <param name='callback'>
        /// The callback to call when authentication finishes. It will be called
        /// with <c>true</c> if authentication was successful, <c>false</c> otherwise.
        /// </param>
        public override void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
        {
            if (!IsGpgsInitialized)
            {
                Debug.LogWarning("SignIn called, but Google Play Game Services has not been initialized. Ignoring call.");
                CloudOnceUtils.SafeInvoke(callback, false);
                return;
            }

            autoLoadEnabled = autoCloudLoad;

            IsGuestUserDefault = false;
            Logger.d("Attempting to sign in to Google Play Game Services.");

            PlayGamesPlatform.Instance.Authenticate(success =>
            {
                // Success is handled by OnAuthenticated method
                if (!success)
                {
                    Logger.w("Failed to sign in to Google Play Game Services.");
                    bool hasNoInternet;
                    try
                    {
                        hasNoInternet = InternetConnectionUtils.GetConnectionStatus() != InternetConnectionStatus.Connected;
                    }
                    catch (NotSupportedException)
                    {
                        hasNoInternet = Application.internetReachability == NetworkReachability.NotReachable;
                    }

                    if (hasNoInternet)
                    {
                        Logger.d("Failure seems to be due to lack of Internet. Will try to connect again next time.");
                    }
                    else
                    {
                        Logger.d("Must assume the failure is due to player opting out"
                              + " of the sign-in process, setting guest user as default");
                        IsGuestUserDefault = true;
                    }

                    cloudOnceEvents.RaiseOnSignInFailed();
                    if (autoCloudLoad)
                    {
                        cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                    }
                }

                CloudOnceUtils.SafeInvoke(callback, success);
            });
        }

        /// <summary>
        /// Signs out of Google Play Game Services. If the user chooses to do this, auto sign-in parameter of the Initialize method is overidden.
        /// The user will have to sign in again manually. This behaviour is required by Google.
        /// </summary>
        public override void SignOut()
        {
            Logger.d("Signing out of Google Play Game Services.");
            PlayGamesPlatform.Instance.SignOut();
            ActivateGuestUserMode();
        }

        /// <summary>
        /// Load the user profiles accociated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            if (!IsGpgsInitialized)
            {
                Debug.LogWarning("LoadUsers called, but Google Play Game Services has not been initialized. Ignoring call.");
                CloudOnceUtils.SafeInvoke(callback, new IUserProfile[0]);
                return;
            }

            PlayGamesPlatform.Instance.LoadUsers(userIDs, callback);
        }

        /// <summary>
        /// Method used by <see cref="Cloud"/> to initialize the Cloud provider.
        /// </summary>
        public void InternalInit(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
            Storage = new GooglePlayGamesCloudSaveWrapper(events);
            ServiceName = "Google Play Game Services";
#if CLOUDONCE_DEBUG
            Debug.Log("Using " + ServiceName);
#endif
        }

        /// <summary>
        /// Sets <see cref="IsGuestUserDefault"/> to <c>true</c> and raises the <see cref="CloudOnceEvents.OnSignedInChanged"/> event.
        /// </summary>
        public void ActivateGuestUserMode()
        {
            IsGuestUserDefault = true;
            cloudOnceEvents.RaiseOnSignedInChanged(false);
        }

        #endregion /Public methods

        protected override void OnAwake()
        {
            IsGuestUserDefault = PlayerPrefs.GetInt(guestPreferenceKey, 0) == 1;
        }

        protected override void OnOnDestroy()
        {
            PlayerPrefs.SetInt(guestPreferenceKey, IsGuestUserDefault ? 1 : 0);
        }

        private static void UpdateAchievementsData(IAchievement[] achievements)
        {
            foreach (var achievement in achievements)
            {
                if (achievement == null || string.IsNullOrEmpty(achievement.id))
                {
                    continue;
                }

                var achievementFound = false;
                foreach (var unifiedAchievement in Achievements.All)
                {
                    if (unifiedAchievement.ID == achievement.id)
                    {
                        unifiedAchievement.UpdateData(achievement.completed, achievement.percentCompleted, achievement.hidden);
                        achievementFound = true;
                        break;
                    }
                }

                if (!achievementFound)
                {
#if CLOUDONCE_DEBUG
                    Debug.Log(string.Format(
                        "An achievement ({0}) that doesn't exist in the Achievements class was loaded from native API.", achievement.id));
#endif
                }
            }
        }

        private void SubscribeOnAuthenticatedEvent()
        {
            PlayGamesPlatform.Instance.OnAuthenticated -= OnAuthenticated;
            PlayGamesPlatform.Instance.OnAuthenticated += OnAuthenticated;
        }

        private void OnAuthenticated()
        {
            GooglePlayGames.OurUtils.PlayGamesHelperObject.RunOnGameThread(
                () =>
                {
                    cloudOnceEvents.RaiseOnSignedInChanged(true);
                    Logger.d("Successfully signed in to Google Play Game Services. Player: " + PlayerDisplayName);
                    IsGuestUserDefault = false;
                    GetPlayerImage();
                    if (playerIdCache != null && !string.Equals(playerIdCache, PlayerID, StringComparison.InvariantCulture))
                    {
                        // Switching user
                        foreach (var achievement in Achievements.All)
                        {
                            achievement.ResetLocalState();
                        }

                        if (cloudSaveEnabled)
                        {
                            GooglePlayGamesCloudSaveWrapper.LoadDataString(OnDataStringLoaded);
                        }
                    }
                    else if (autoLoadEnabled && cloudSaveEnabled)
                    {
                        Cloud.Storage.Load();
                    }

                    playerIdCache = PlayerID;
                    if (Achievements.All.Length > 0)
                    {
                        PlayGamesPlatform.Instance.LoadAchievements(UpdateAchievementsData);
                    }
                });
        }

        private void OnDataStringLoaded(string dataString)
        {
            var allKeys = DataManager.ReplaceLocalDataWith(dataString);
            cloudOnceEvents.RaiseOnNewCloudValues(allKeys);
            cloudOnceEvents.RaiseOnCloudLoadComplete(true);
        }

        private void GetPlayerImage()
        {
            var url = PlayGamesPlatform.Instance.GetUserImageUrl();
            if (!string.IsNullOrEmpty(url))
            {
                StartCoroutine(DownloadPlayerImage(url));
            }
        }

        private IEnumerator DownloadPlayerImage(string url)
        {
#if UNITY_2017_1_OR_NEWER
            using (var request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    yield break;
                }

                playerImage = DownloadHandlerTexture.GetContent(request);
            }
#else
            var www = new WWW(url);
            yield return www;
            playerImage = www.texture;
#endif
            cloudOnceEvents.RaiseOnPlayerImageDownloaded(playerImage);
        }
    }
}
#endif
