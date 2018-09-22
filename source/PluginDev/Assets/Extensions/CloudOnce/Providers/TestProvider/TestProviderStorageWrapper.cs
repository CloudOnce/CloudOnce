// <copyright file="TestProviderStorageWrapper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Providers
{
    using Internal;
    using UnityEngine;

    /// <summary>
    /// For testing cloud save in the editor
    /// </summary>
    public class TestProviderStorageWrapper : ICloudStorageProvider
    {
        private readonly CloudOnceEvents cloudOnceEvents;
        private bool isSynchronizing;

        public TestProviderStorageWrapper(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
        }

        /// <summary>
        /// Saves all cloud variables, to both disk and cloud.
        /// If <see cref="Cloud.CloudSaveEnabled"/> is <c>false</c>, it will only save to disk.
        /// Skips saving if no variables have been changed.
        /// </summary>
        public void Save()
        {
            DataManager.SaveToDisk();
            if (!TestProvider.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning(!TestProvider.Instance.CloudSaveInitialized
                    ? "Cloud Save has not been initialized, skipping upload and only saving to disk."
                    : "Cloud Save is currently disabled, skipping upload and only saving to disk.");
#endif
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                return;
            }
#if CLOUDONCE_DEBUG
            Debug.Log("Simulating cloud save.");
#endif
            cloudOnceEvents.RaiseOnCloudSaveComplete(true);
        }

        /// <summary>
        /// Loads any available cloud data (if signed in and cloud saving is enabled).
        /// </summary>
        public void Load()
        {
            if (!TestProvider.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning(!TestProvider.Instance.CloudSaveInitialized
                    ? "Cloud Save has not been initialized, aborting cloud load."
                    : "Cloud Save is currently disabled, aborting cloud load.");
#endif
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                return;
            }

            if (Cloud.IsSignedIn)
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Simulating cloud load.");
#endif
                var changedKeys = DataManager.GetRandomKeysFromGameData();
                if (changedKeys.Length > 0)
                {
#if CLOUDONCE_DEBUG
                    Debug.Log("Simulating new cloud values (randomized).");
#endif
                    cloudOnceEvents.RaiseOnNewCloudValues(changedKeys);
                }

                cloudOnceEvents.RaiseOnCloudLoadComplete(true);
            }
            else
            {
                Debug.LogWarning("Attempted to load cloud data, but the local user is not signed in." +
                                 " Will try to re-initialize.");
                Cloud.Initialize();
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
            }
        }

        /// <summary>
        /// Calls <see cref="Load"/> and then <see cref="Save"/> as soon as the Cloud load is complete.
        /// </summary>
        public void Synchronize()
        {
            if (isSynchronizing)
            {
                return;
            }

            isSynchronizing = true;
            Cloud.OnCloudLoadComplete += OnCloudLoadComplete;
            Load();
        }

        /// <summary>
        /// Resets a Cloud variable to the default value.
        /// </summary>
        /// <param name="key">The unique identifier for the Cloud variable you want to reset.</param>
        /// <returns>Whether or not the variable was successfully reset.</returns>
        public bool ResetVariable(string key)
        {
            return DataManager.ResetCloudPref(key);
        }

        /// <summary>
        /// Deletes a specific Cloud variable from the Cloud.
        /// </summary>
        /// <param name="key">The unique identifier for the CloudPref you want to delete.</param>
        /// <returns>
        /// <c>true</c> if the CloudPref is found and deleted, <c>false</c> if the specified <paramref name="key"/> doesn't exist.
        /// </returns>
        public bool DeleteVariable(string key)
        {
            return DataManager.DeleteCloudPref(key);
        }

        /// <summary>
        /// Deletes all variables that exists in the cloud save, but have not been declared in the local data.
        /// Can be useful during development when variable names change, but use with caution.
        /// </summary>
        /// <returns>An array with the keys for the variables that was cleared.</returns>
        public string[] ClearUnusedVariables()
        {
            return DataManager.ClearStowawayVariablesFromGameData();
        }

        /// <summary>
        /// WARNING! Deletes all cloud variables both locally and in the cloud (if logged into a cloud save service)!
        /// Should only be used during development, not in production builds.
        /// </summary>
        public void DeleteAll()
        {
            DataManager.DeleteAllCloudVariables();
        }

        private void OnCloudLoadComplete(bool arg0)
        {
            Cloud.OnCloudLoadComplete -= OnCloudLoadComplete;
            Save();
            isSynchronizing = false;
        }
    }
}
#endif
