// <copyright file="DummyProvider.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Providers
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SocialPlatforms;
    using Utils;

    /// <summary>
    /// Dummy provider for unsupported platforms.
    /// </summary>
    public sealed class DummyProvider : CloudProviderBase<DummyProvider>
    {
        private CloudOnceEvents cloudOnceEvents;

        /// <summary>
        /// ID for currently signed in player. Will always return DummyPlayerID when using <see cref="DummyProvider"/>.
        /// </summary>
        public override string PlayerID
        {
            get { return "DummyPlayerID"; }
        }

        /// <summary>
        /// Display name for currently signed in player. Will always return DummyPlayerName when using <see cref="DummyProvider"/>.
        /// </summary>
        public override string PlayerDisplayName
        {
            get { return "DummyPlayerName"; }
        }

        /// <summary>
        /// Profile picture for currently signed in player. Will always return <see cref="Texture2D.whiteTexture"/> when using <see cref="DummyProvider"/>.
        /// </summary>
        public override Texture2D PlayerImage
        {
            get { return Texture2D.whiteTexture; }
        }

        /// <summary>
        /// Whether or not the user is currently signed in. Is always disabled when using <see cref="DummyProvider"/>.
        /// </summary>
        public override bool IsSignedIn
        {
            get { return false; }
        }

        /// <summary>
        /// Whether or not Cloud Save has been initialized. Is always <c>false</c> when using <see cref="DummyProvider"/>.
        /// </summary>
        public bool CloudSaveInitialized
        {
            get { return false; }
        }

        /// <summary>
        /// Whether or not Cloud Save is enabled. Is always disabled when using <see cref="DummyProvider"/>.
        /// </summary>
        public override bool CloudSaveEnabled
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Dummy storage wrapper.
        /// </summary>
        public override ICloudStorageProvider Storage { get; protected set; }

        /// <summary>
        /// Dummy Initialize method.
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
            cloudOnceEvents.RaiseOnInitializeComplete();
            cloudOnceEvents.RaiseOnPlayerImageDownloaded(Texture2D.whiteTexture);
            if (autoSignIn)
            {
                SignIn(autoCloudLoad);
            }
        }

        /// <summary>
        /// Dummy SignIn method.
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
            CloudOnceUtils.SafeInvoke(callback, false);
            if (autoCloudLoad)
            {
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
            }
        }

        /// <summary>
        /// Dummy SignOut method.
        /// </summary>
        public override void SignOut()
        {
        }

        /// <summary>
        /// Load the user profiles associated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
        }

        /// <summary>
        /// Method used by <see cref="Cloud"/> to initialize the Cloud provider.
        /// </summary>
        public void InternalInit(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
            Storage = new DummyStorageWrapper(events);
            ServiceName = "Dummy Provider";
#if CLOUDONCE_DEBUG
            Debug.Log("Using " + ServiceName);
#endif
        }
    }
}
