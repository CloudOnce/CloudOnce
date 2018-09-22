// <copyright file="CloudProviderBase.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using Providers;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SocialPlatforms;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Base class for the Cloud service providers.
    /// </summary>
    /// <typeparam name="T">Type of singleton</typeparam>
    public abstract class CloudProviderBase<T> : MonoBehaviour, ICloudProvider
        where T : Component
    {
        private static T s_instance;
        private float currentLoadTimer;

        /// <summary>
        /// Makes sure only one instance of the type exists.
        /// </summary>
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (EditorApplication.isCompiling)
                {
                    return null;
                }
#endif
                if (!ReferenceEquals(s_instance, null))
                {
                    return s_instance;
                }

                var singletons = FindObjectsOfType(typeof(T));
                if (!ReferenceEquals(singletons, null) && singletons.Length > 0)
                {
                    s_instance = singletons[0] as T;
                    if (singletons.Length > 1)
                    {
                        for (var i = 1; i < singletons.Length; i++)
                        {
                            Destroy(singletons[i]);
                        }
                    }
                }

                if (!ReferenceEquals(s_instance, null))
                {
                    return s_instance;
                }

                // In case the singleton is referenced somewhere in a scene that doesn't have the singleton component on an object
                // we create one and hide it.
                var obj = new GameObject
                {
                    name = string.Format("NewTransient{0}Singleton", typeof(T)),
                    hideFlags = HideFlags.HideAndDontSave
                };

                s_instance = obj.AddComponent(typeof(T)) as T; // Calls ctor of T
                return s_instance;
            }
        }

        /// <summary>
        /// Name of the currently used service, e.g. "Apple Game Center".
        /// </summary>
        public string ServiceName { get; protected set; }

        /// <summary>
        /// ID for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public abstract string PlayerID { get; }

        /// <summary>
        /// Display name for currently signed in player. Will return an empty <see cref="string"/> if no player is signed in.
        /// </summary>
        public abstract string PlayerDisplayName { get; }

        /// <summary>
        /// Profile picture for currently signed in player. Will return <c>null</c> if the user has no avatar, or it has not loaded yet.
        /// </summary>
        public abstract Texture2D PlayerImage { get; }

        /// <summary>
        /// Whether or not the user is currently signed in.
        /// </summary>
        public abstract bool IsSignedIn { get; }

        /// <summary>
        /// Whether or not Cloud Save is enabled.
        /// Disabling Cloud Save will make <c>Cloud.Storage.Save</c> only save to disk.
        /// Can only be enabled if Cloud Save was initialized in <see cref="Cloud.Initialize"/> method.
        /// </summary>
        public abstract bool CloudSaveEnabled { get; set; }

        /// <summary>
        /// Interface for accessing Cloud Save on the current platform
        /// </summary>
        public abstract ICloudStorageProvider Storage { get; protected set; }

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
        public abstract void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true);

        /// <summary>
        /// Signs in to the current cloud provider.
        /// </summary>
        /// <param name="autoCloudLoad">
        /// Whether or not cloud data should be loaded automatically if the user is successfully signed in.
        /// Ignored if Cloud Saving is deactivated or the user fails to sign in.
        /// </param>
        /// <param name='callback'>
        /// The callback to call when authentication finishes. It will be called
        /// with <c>true</c> if authentication was successful, <c>false</c> otherwise.
        /// </param>
        public abstract void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null);

        /// <summary>
        /// Signs out of the current cloud provider.
        /// </summary>
        public abstract void SignOut();

        /// <summary>
        /// Load the user profiles associated with the given array of user IDs.
        /// </summary>
        /// <param name="userIDs">The users to retrieve profiles for.</param>
        /// <param name="callback">Callback to handle the user profiles.</param>
        public abstract void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnOnDestroy()
        {
        }

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this);
            OnAwake();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            currentLoadTimer = (int)Cloud.AutoLoadInterval;
        }

        /// <summary>
        /// Update is called every frame.
        /// </summary>
        private void Update()
        {
            if (Cloud.AutoLoadInterval == Interval.Disabled)
            {
                return;
            }

            if (currentLoadTimer > 0f)
            {
                currentLoadTimer -= Time.deltaTime;
            }
            else
            {
                Cloud.Storage.Load();
                currentLoadTimer = (int)Cloud.AutoLoadInterval;
            }
        }

        /// <summary>
        /// Called automatically by Unity when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            s_instance = null;
            OnOnDestroy();
        }
    }
}
