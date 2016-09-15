// <copyright file="AmazonCloudProvider.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && TP_AndroidAmazon
namespace Trollpants.CloudOnce.Internal.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Internal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SocialPlatforms;
    using Utils;

    /// <summary>
    /// Amazon GameCircle Services implementation of ICloudProvider
    /// </summary>
    public sealed class AmazonCloudProvider : CloudProviderBase<AmazonCloudProvider>
    {
        #region Fields, constructor & properties

        /// <summary>
        /// Name of GameObject that is used as the Amazon native callback target.
        /// </summary>
        private const string c_nativeCallbackTargetName = "GameCircleManager";

        private CloudOnceEvents cloudOnceEvents;
        private bool cloudSaveEnabled = true;
        private bool autoLoadOnSignInEnabled = true;
        private bool isInitializing;
        private string playerID;
        private string playerAlias;
        private Texture2D playerImage;

        /// <summary>
        /// Set to <c>true</c> when Amazon GameCircle is initialized.
        /// </summary>
        private bool firstInitializedFinished;

        /// <summary>
        /// Prevents a default instance of the <see cref="AmazonCloudProvider" /> class from being created.
        /// </summary>
        private AmazonCloudProvider()
            : base("Amazon GameCircle")
        {
        }

        /// <summary>
        /// Whether or not the GameCircle service as been initialized.
        /// </summary>
        public bool IsServiceReady
        {
            get { return AGSClient.IsServiceReady(); }
        }

        /// <summary>
        /// ID for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public override string PlayerID
        {
            get { return IsSignedIn ? playerID : string.Empty; }
        }

        /// <summary>
        /// Display name for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public override string PlayerDisplayName
        {
            get { return IsSignedIn ? playerAlias : string.Empty; }
        }

        /// <summary>
       /// Profile picture for currently signed in player. Will return a white texture if the user has no avatar, or it has not loaded yet.
        /// </summary>
        public override Texture2D PlayerImage
        {
            get { return playerImage ?? Texture2D.whiteTexture; }
        }

        /// <summary>
        /// Whether or not the user is currently signed in.
        /// </summary>
        public override bool IsSignedIn
        {
            get { return AGSPlayerClient.IsSignedIn(); }
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
        /// Interface for accessing cloud save on the current platform
        /// </summary>
        public override ICloudStorageProvider Storage { get; protected set; }

        #endregion /Fields, constructor & properties

        #region Public methods

        /// <summary>
        /// Initializes Amazon GameCircle.
        /// </summary>
        /// <param name="activateCloudSave">Whether or not Cloud Saving should be activated.</param>
        /// <param name="autoSignIn">
        /// Ignored on Amazon GameCircle as there is no way of avoiding auto sign in.
        /// </param>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically if the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        public override void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
        {
            if (isInitializing)
            {
                return;
            }

            isInitializing = true;
            cloudSaveEnabled = activateCloudSave;
            autoLoadOnSignInEnabled = autoCloudLoad;
#if CO_DEBUG
            Debug.Log("Initializing Amazon GameCircle.");
            Debug.Log(activateCloudSave ? "WhisperSync support enabled." : "WhisperSync support disabled.");
            AGSClient.errorLevel = AmazonLogging.AmazonLoggingLevel.Verbose;
#endif
            SubscribeEvents(autoCloudLoad);
            AGSClient.Init(true, true, activateCloudSave);
        }

        /// <summary>
        /// Shows the GameCircle sign in page. If GameCircle is not initialized,
        /// the <see cref="Initialize"/> method will be called.
        /// </summary>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically when the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        /// <param name='callback'>
        /// Due to differences in how the GameCircle platform handles sign in (there is no SignIn method, only Init),
        /// the callback will always be <c>true</c>.
        /// </param>
        public override void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
        {
            if (AGSClient.IsServiceReady())
            {
                AGSClient.ShowSignInPage();
            }
            else
            {
                Initialize(CloudSaveEnabled, true, autoCloudLoad);
            }

            CloudOnceUtils.SafeInvoke(callback, true);
        }

        /// <summary>
        /// Shuts down the GameCircle service.
        /// </summary>
        public override void SignOut()
        {
            if (!AGSClient.IsServiceReady())
            {
                // Not signed in, ignoring call.
                return;
            }

            AGSClient.Shutdown();
        }

        /// <summary>
        /// Load the user profiles accociated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            Debug.LogWarning("LoadUsers functionality does not exist on the Amazon GameCircle platform.");
            CloudOnceUtils.SafeInvoke(callback, new IUserProfile[0]);
        }

        /// <summary>
        /// Shows the Amazon GameCircle overlay.
        /// </summary>
        public void ShowNativeOverlay()
        {
            if (AGSClient.IsServiceReady())
            {
                AGSClient.ShowGameCircleOverlay();
            }
            else
            {
                Debug.LogWarning("Can't show native overlay. Service is not ready. Call Initialize first.");
            }
        }

        /// <summary>
        /// Method used by <see cref="Cloud"/> to initialize the Cloud provider.
        /// </summary>
        public void InternalInit(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
            Storage = new AmazonWhisperSyncWrapper(events);
        }

        #endregion /Public methods

        #region Amazon GameCircle WhisperSync Callback Targets

        /* --------------------------------------------------------------------------------------
         * Do NOT rename these functions, the correct naming convention for these is sampleName!
         * They are called by external code that depend on this specific naming!
         * --------------------------------------------------------------------------------------
         */

        public void serviceReady(string empty)
        {
            AGSClient.Log("AGSClient - ServiceReady");
            firstInitializedFinished = true;
            AGSClient.ServiceReady(empty);
        }

        public void serviceNotReady(string param)
        {
            AGSClient.Log("AGSClient - ServiceNotReady");
            AGSClient.ServiceNotReady(param);
        }

        public void playerReceived(string json)
        {
            AGSClient.Log("AGSPlayerClient - PlayerReceived");
            AGSPlayerClient.PlayerReceived(json);
        }

        public void playerFailed(string json)
        {
            AGSClient.Log("AGSPlayerClient - PlayerFailed");
            AGSPlayerClient.PlayerFailed(json);
        }

        public void localPlayerFriendRequestComplete(string json)
        {
            AGSClient.Log("AGSPlayerClient - LocalPlayerFriendsComplete");
            AGSPlayerClient.LocalPlayerFriendsComplete(json);
        }

        public void batchFriendsRequestComplete(string json)
        {
            AGSClient.Log("AGSPlayerClient - BatchFriendsRequestComplete");
            AGSPlayerClient.BatchFriendsRequestComplete(json);
        }

        public void onSignedInStateChange(string isSignedIn)
        {
            AGSClient.Log("AGSPlayerClient - OnSignedInStateChanged");
            AGSPlayerClient.OnSignedInStateChanged(bool.Parse(isSignedIn));
        }

        public void submitScoreFailed(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - SubmitScoreFailed");
            AGSLeaderboardsClient.SubmitScoreFailed(json);
        }

        public void submitScoreSucceeded(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - SubmitScoreSucceeded");
            AGSLeaderboardsClient.SubmitScoreSucceeded(json);
        }

        public void requestLeaderboardsFailed(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestLeaderboardsFailed");
            AGSLeaderboardsClient.RequestLeaderboardsFailed(json);
        }

        public void requestLeaderboardsSucceeded(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestLeaderboardsSucceeded");
            AGSLeaderboardsClient.RequestLeaderboardsSucceeded(json);
        }

        public void requestLocalPlayerScoreFailed(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestLocalPlayerScoreFailed");
            AGSLeaderboardsClient.RequestLocalPlayerScoreFailed(json);
        }

        public void requestLocalPlayerScoreSucceeded(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestLocalPlayerScoreSucceeded");
            AGSLeaderboardsClient.RequestLocalPlayerScoreSucceeded(json);
        }

        public void requestPlayerScoreCompleted(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestScoreForPlayerComplete");
            AGSLeaderboardsClient.RequestScoreForPlayerComplete(json);
        }

        public void requestScoresSucceeded(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestScoresSucceeded");
            AGSLeaderboardsClient.RequestScoresSucceeded(json);
        }

        public void requestScoresFailed(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestScoresFailed");
            AGSLeaderboardsClient.RequestScoresFailed(json);
        }

        public void requestPercentileRanksSucceeded(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestPercentileRanksSucceeded");
            AGSLeaderboardsClient.RequestPercentileRanksSucceeded(json);
        }

        public void requestPercentileRanksFailed(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestPercentileRanksFailed");
            AGSLeaderboardsClient.RequestPercentileRanksFailed(json);
        }

        public void requestPercentileRanksForPlayerSucceeded(string json)
        {
            AGSClient.Log("AGSLeaderboardsClient - RequestPercentileRanksForPlayerComplete");
            AGSLeaderboardsClient.RequestPercentileRanksForPlayerComplete(json);
        }

        public void updateAchievementSucceeded(string json)
        {
            AGSClient.Log("AGSAchievementsClient - UpdateAchievementSucceeded");
            AGSAchievementsClient.UpdateAchievementSucceeded(json);
        }

        public void updateAchievementFailed(string json)
        {
            AGSClient.Log("AGSAchievementsClient - UpdateAchievementFailed");
            AGSAchievementsClient.UpdateAchievementFailed(json);
        }

        public void requestAchievementsSucceeded(string json)
        {
            AGSClient.Log("AGSAchievementsClient - RequestAchievementsSucceeded");

            AGSAchievementsClient.RequestAchievementsSucceeded(json);
        }

        public void requestAchievementsFailed(string json)
        {
            AGSClient.Log("AGSAchievementsClient - RequestAchievementsFailed");
            AGSAchievementsClient.RequestAchievementsFailed(json);
        }

        public void requestAchievementsForPlayerCompleted(string json)
        {
            AGSClient.Log("AGSAchievementsClient - RequestAchievementsForPlayerComplete");
            AGSAchievementsClient.RequestAchievementsForPlayerComplete(json);
        }

        public void onNewCloudData(string empty)
        {
            AGSClient.Log("AGSWhispersyncClient - OnNewCloudData");
            AGSWhispersyncClient.OnNewCloudData();
        }

        public void onDataUploadedToCloud(string empty)
        {
            AGSClient.Log("AGSWhispersyncClient - OnDataUploadedToCloud");
            AGSWhispersyncClient.OnDataUploadedToCloud();
        }

        public void onThrottled(string empty)
        {
            AGSClient.Log("AGSWhispersyncClient - OnThrottled");
            AGSWhispersyncClient.OnThrottled();
        }

        public void onDiskWriteComplete(string empty)
        {
            AGSClient.Log("AGSWhispersyncClient - OnDiskWriteComplete");
            AGSWhispersyncClient.OnDiskWriteComplete();
        }

        public void onFirstSynchronize(string empty)
        {
            Debug.Log("AGSWhispersyncClient - OnFirstSynchronize");
            AGSWhispersyncClient.OnFirstSynchronize();
        }

        public void onAlreadySynchronized(string empty)
        {
            AGSClient.Log("AGSWhispersyncClient - OnAlreadySynchronized");
            AGSWhispersyncClient.OnAlreadySynchronized();
        }

        public void onSyncFailed(string failReason)
        {
            AGSClient.Log("AGSWhispersyncClient - OnSyncFailed");
            AGSWhispersyncClient.OnSyncFailed(failReason);
        }

        #endregion

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void OnAwake()
        {
            // Native Android code sends messages into Unity using a GameObject with a specific name
            name = c_nativeCallbackTargetName;
        }

        protected override void OnOnDestroy()
        {
            SignOut();
        }

        #region Private static methods

        private static void UpdateAchievementsData()
        {
            AGSAchievementsClient.RequestAchievementsCompleted -= OnRequestAchievementsCompleted;
            AGSAchievementsClient.RequestAchievementsCompleted += OnRequestAchievementsCompleted;

            if (!AGSPlayerClient.IsSignedIn())
            {
#if CO_DEBUG
                Debug.LogWarning("AGSAchievementsClient.RequestAchievements(); can only be called after authentication.");
#endif
                return;
            }

            AGSAchievementsClient.RequestAchievements();
        }

        private static void OnRequestAchievementsCompleted(AGSRequestAchievementsResponse agsRequestAchievementsResponse)
        {
            var type = typeof(Achievements);
            var allAchievements = new Dictionary<string, UnifiedAchievement>();
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(UnifiedAchievement))
                {
                    allAchievements[propertyInfo.Name] = (UnifiedAchievement)propertyInfo.GetValue(null, null);
                }
            }

            if (agsRequestAchievementsResponse.achievements.Count > 0)
            {
                foreach (var agsAchievement in agsRequestAchievementsResponse.achievements)
                {
                    try
                    {
                        var kvp = allAchievements.Single(pair => pair.Value.ID == agsAchievement.id);
                        allAchievements[kvp.Key].UpdateData(agsAchievement.isUnlocked, agsAchievement.progress, agsAchievement.isHidden);
                    }
                    catch
                    {
#if CO_DEBUG
                        Debug.Log(string.Format(
                            "An achievement ({0}) that doesn't exist in the Achievements class was loaded from native API.", agsAchievement.id));
#endif
                    }
                }
            }
        }

        #endregion /Private static methods

        #region Unity methods

        /// <summary>
        /// Called by Unity when the application loses or regains focus.
        /// </summary>
        /// <param name="focusStatus"><c>true</c> if the application has regained focus.
        /// <c>false</c> if it has lost focus.</param>
        private void OnApplicationFocus(bool focusStatus)
        {
            if (!firstInitializedFinished || !AGSClient.ReinitializeOnFocus)
            {
                return;
            }

            if (focusStatus)
            {
#if CO_DEBUG
                Debug.Log("AmazonCloudProvider.OnApplicationFocus(true) -> AGSClient.Init()");
#endif
                AGSClient.Init();
            }
            else
            {
#if CO_DEBUG
                Debug.Log("AmazonCloudProvider.OnApplicationFocus(false) -> AGSClient.release()");
#endif
                AGSClient.release();
            }
        }

        #endregion /Unity methods

        #region Private Methods

        private IEnumerator DownloadPlayerImage(string url)
        {
            // Start a download of the given URL
            var www = new WWW(url);

            // Wait for download to complete
            yield return www;

            // assign texture
            playerImage = www.texture;
            cloudOnceEvents.RaiseOnPlayerImageDownloaded(playerImage);
        }

        private void SubscribeEvents(bool autoLoadOnInitialized)
        {
            UnsubscribeEvents();

            if (cloudSaveEnabled)
            {
                var whisperSyncWrapper = (AmazonWhisperSyncWrapper)Storage;
                whisperSyncWrapper.SubscribeEvents(autoLoadOnInitialized);
            }

            AGSClient.ServiceReadyEvent += ServiceReadyEvent;
            AGSClient.ServiceNotReadyEvent += OnServiceNotReady;
            AGSPlayerClient.OnSignedInStateChangedEvent += OnSignedInStateChangedEvent;
            AGSPlayerClient.RequestLocalPlayerCompleted += RequestLocalPlayerCompleted;
        }

        private void UnsubscribeEvents()
        {
            AGSClient.ServiceReadyEvent -= ServiceReadyEvent;
            AGSClient.ServiceNotReadyEvent -= OnServiceNotReady;
            AGSPlayerClient.OnSignedInStateChangedEvent -= OnSignedInStateChangedEvent;
            AGSPlayerClient.RequestLocalPlayerCompleted -= RequestLocalPlayerCompleted;
        }

        private void ServiceReadyEvent()
        {
#if CO_DEBUG
            Debug.Log("OnServiceReady");
#endif
            if (cloudSaveEnabled)
            {
                // Due to some flaws in the Amazon SDK lib (static constructors,
                // where one depends on another being called first, AGSClient.Init must be called before
                // AGSWhispersyncClient) we put the initialization logic here.
                // Otherwise we will not get some of the event callbacks we need.
#if CO_DEBUG
                Debug.Log("Initializing WhisperSync");
#endif
                AGSWhispersyncClient.InitAGSWhispersyncClient();
                CloudSaveInitialized = true;
            }

            cloudOnceEvents.RaiseOnInitializeComplete();
            AGSClient.ServiceReadyEvent -= ServiceReadyEvent;
            isInitializing = false;
        }

        private void OnServiceNotReady(string error)
        {
            Debug.LogWarning("Amazon GameCircle initialization failed: " + error);
            cloudOnceEvents.RaiseOnSignInFailed();
            cloudOnceEvents.RaiseOnInitializeComplete();
            cloudOnceEvents.RaiseOnCloudLoadComplete(false);
            isInitializing = false;
        }

        private void OnSignedInStateChangedEvent(bool signedIn)
        {
            if (signedIn)
            {
#if CO_DEBUG
                Debug.Log("Signed in to Amazon GameCircle.");
#endif
                if (CloudSaveInitialized && cloudSaveEnabled && autoLoadOnSignInEnabled)
                {
                    Cloud.Storage.Load();
                }

                AGSPlayerClient.RequestLocalPlayer();
                UpdateAchievementsData();
            }
            else
            {
#if CO_DEBUG
                Debug.Log("Signed out of Amazon GameCircle.");
#endif
            }

            cloudOnceEvents.RaiseOnSignedInChanged(signedIn);
        }

        private void RequestLocalPlayerCompleted(AGSRequestPlayerResponse response)
        {
            var sb = new StringBuilder();
            sb.AppendLine("RequestLocalPlayerCompleted:");

            if (response.IsError())
            {
                sb.AppendLine("Error: " + response.error);
            }
            else
            {
                if (response.player != null)
                {
                    playerID = response.player.playerId;
                    playerAlias = response.player.alias;
                    if (string.IsNullOrEmpty(response.player.avatarUrl))
                    {
#if CO_DEBUG
                        Debug.LogWarning("Can't download profile picture; URL was null.");
#endif
                    }
                    else
                    {
                        StartCoroutine(DownloadPlayerImage(response.player.avatarUrl));
                    }

                    sb.AppendFormat("Player: {0} - Id: {1} ", response.player.alias, response.player.playerId);
                }
            }
#if CO_DEBUG
            Debug.Log(sb);
#endif
        }

        #endregion /Private methods
    }
}
#endif
