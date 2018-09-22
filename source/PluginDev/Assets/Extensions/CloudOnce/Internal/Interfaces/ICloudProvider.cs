// <copyright file="ICloudProvider.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Providers
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Interface for accessing platform-agnostic cloud functionality.
    /// </summary>
    public interface ICloudProvider
    {
        /// <summary>
        /// Name of the currently used service, e.g. "Apple Game Center".
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// ID for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        string PlayerID { get; }

        /// <summary>
        /// Display name for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        string PlayerDisplayName { get; }

        /// <summary>
        /// Profile picture for currently signed in player. Will return <c>null</c> if the user has no avatar, or it has not loaded yet.
        /// </summary>
        Texture2D PlayerImage { get; }

        /// <summary>
        /// Whether or not the user is currently signed in.
        /// </summary>
        bool IsSignedIn { get; }

        /// <summary>
        /// Whether or not Cloud Save is enabled.
        /// Disabling Cloud Save will make <c>Cloud.Storage.Save</c> only save to disk.
        /// Can only be enabled if Cloud Save was initialized in <see cref="Cloud.Initialize"/> method.
        /// </summary>
        bool CloudSaveEnabled { get; set; }

        /// <summary>
        /// Interface for accessing cloud save on the current platform.
        /// </summary>
        ICloudStorageProvider Storage { get; }

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
        void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true);

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
        void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null);

        /// <summary>
        /// Signs out of the current cloud provider.
        /// </summary>
        void SignOut();

        /// <summary>
        /// Load the user profiles associated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);
    }
}
