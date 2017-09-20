// <copyright file="iOSCloudSaveWrapper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_IOS || UNITY_TVOS
namespace CloudOnce.Internal.Providers
{
    using System;
    using UnityEngine;
    using Utils;

    /// <summary>
    ///  This wrapper encapsulates the platform specific code needed to provide cloud storage on the iOS platform.
    /// </summary>
    public class iOSCloudSaveWrapper : ICloudStorageProvider
    {
        private readonly CloudOnceEvents cloudOnceEvents;
        private bool isSynchronising;

        public iOSCloudSaveWrapper(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
            iCloudBridge.OnExternalChange += OnExternalCloudDataChanges;
        }

        /// <summary>
        /// Saves all cloud variables, to both disk and cloud.
        /// If <see cref="Cloud.CloudSaveEnabled"/> is <c>false</c>, it will only save to disk.
        /// Skips saving if no variables have been changed.
        /// </summary>
        public void Save()
        {
            DataManager.SaveToDisk();
            if (!Cloud.CloudSaveEnabled)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning("Cloud Save is currently disabled, skipping upload and only saving to disk.");
#endif
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                return;
            }

            if (DataManager.IsLocalDataDirty)
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Saving cloud data");
#endif
                var saveSuccess = SaveToCloud();
                if (saveSuccess)
                {
                    DataManager.IsLocalDataDirty = false;
                    cloudOnceEvents.RaiseOnCloudSaveComplete(true);
                }
                else
                {
                    cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                }
            }
            else
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Save called, but no data has changed since last save.");
#endif
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
            }
        }

        /// <summary>
        /// Loads any available cloud data (if signed in and cloud saving is enabled).
        /// </summary>
        public void Load()
        {
            if (!Cloud.CloudSaveEnabled)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning("Cloud Save is currently disabled, aborting cloud load.");
#endif
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                return;
            }

            var devString = iCloudBridge.GetString(DataManager.DevStringKey);

            if (string.IsNullOrEmpty(devString))
            {
#if CLOUDONCE_DEBUG
                Debug.Log("No data saved to the cloud yet.");
#endif
                cloudOnceEvents.RaiseOnCloudLoadComplete(true);
                return;
            }

            if (!devString.IsJson())
            {
                try
                {
                    devString = devString.FromBase64StringToString();
                }
                catch (FormatException)
                {
                    Debug.LogWarning("Unable to deserialize cloud data!");
                    cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                    return;
                }
            }

            LoadImpl(devString);
        }

        /// <summary>
        /// Calls <see cref="Load"/> and then <see cref="Save"/> as soon as the Cloud load is complete.
        /// </summary>
        public void Synchronize()
        {
            if (isSynchronising)
            {
                return;
            }

            isSynchronising = true;
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

        private static bool SaveToCloud()
        {
            return iCloudBridge.SetString(DataManager.DevStringKey, DataManager.SerializeLocalData().ToBase64String());
        }

        private void LoadImpl(string devString)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("Loading cloud data");
#endif
            var changedKeys = DataManager.MergeLocalDataWith(devString);
            if (changedKeys.Length > 0)
            {
                cloudOnceEvents.RaiseOnNewCloudValues(changedKeys);
            }

            cloudOnceEvents.RaiseOnCloudLoadComplete(true);
        }

        private void OnExternalCloudDataChanges(KvStoreChangeReason reason, string devString)
        {
            string cloudData;
            if (!devString.IsJson())
            {
                try
                {
                    cloudData = devString.FromBase64StringToString();
                }
                catch (FormatException)
                {
                    Debug.LogWarning("Unable to deserialize cloud data!");
                    return;
                }
            }
            else
            {
                cloudData = devString;
            }

            switch (reason)
            {
                case KvStoreChangeReason.InitialSyncChange:
                case KvStoreChangeReason.ServerChange:
                    LoadImpl(cloudData);
                    break;

                // Your app’s key-value store has exceeded its space quota on the iCloud server.
                case KvStoreChangeReason.QuotaViolationChange:
                    Debug.LogError("Unable to save to cloud. Key-value store has exeeded its space quota on the iCloud server.");
                    break;

                // The user has changed the primary iCloud account. The keys and values in the local key-value store have been
                // replaced with those from the new account, regardless of the relative timestamps.
                case KvStoreChangeReason.AccountChange:
                    var allKeys = DataManager.ReplaceLocalDataWith(cloudData);
                    cloudOnceEvents.RaiseOnNewCloudValues(allKeys);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCloudLoadComplete(bool arg0)
        {
            Cloud.OnCloudLoadComplete -= OnCloudLoadComplete;
            Save();
            isSynchronising = false;
        }
    }
}
#endif
