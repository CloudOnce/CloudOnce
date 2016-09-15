// <copyright file="AmazonWhisperSyncWrapper.cs" company="Jan Ivar Z. Carlsen & Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen & Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && TP_AndroidAmazon
namespace Trollpants.CloudOnce.Internal.Providers
{
    using System;
    using Internal;
    using UnityEngine;
    using Utils;

    /// <summary>
    /// This wrapper encapsulates the platform specific code needed to provide cloud storage on the Amazon platform.
    /// </summary>
    public class AmazonWhisperSyncWrapper : ICloudStorageProvider
    {
        private readonly CloudOnceEvents cloudOnceEvents;
        private bool saveInitialized;
        private bool processingCloudData;
        private bool isSynchronising;

        public AmazonWhisperSyncWrapper(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
        }

        public void SubscribeEvents(bool autoLoadOnInitialized)
        {
            AGSWhispersyncClient.OnNewCloudDataEvent -= ProcessCloudData;
            AGSWhispersyncClient.OnNewCloudDataEvent += ProcessCloudData;
            AGSWhispersyncClient.OnInitializedEvent -= Load;

            if (autoLoadOnInitialized)
            {
                AGSWhispersyncClient.OnInitializedEvent += Load;
            }
        }

        /// <summary>
        /// Saves all cloud variables, to both disk and cloud.
        /// If <see cref="Cloud.CloudSaveEnabled"/> is <c>false</c>, it will only save to disk.
        /// Skips saving if no variables have been changed.
        /// </summary>
        public void Save()
        {
            if (saveInitialized)
            {
                return;
            }

            saveInitialized = true;

            DataManager.SaveToDisk();
            if (!AmazonCloudProvider.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
            {
#if CO_DEBUG
                Debug.LogWarning(!AmazonCloudProvider.Instance.CloudSaveInitialized
                    ? "Cloud Save has not been initialized, skipping upload and only saving to disk."
                    : "Cloud Save is currently disabled, skipping upload and only saving to disk.");
#endif
                saveInitialized = false;
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                return;
            }

            if (DataManager.IsLocalDataDirty)
            {
                if (AGSClient.IsServiceReady())
                {
#if CO_DEBUG
                    Debug.Log("Saving cloud data");
#endif
                    using (var dataMap = AGSWhispersyncClient.GetGameData())
                    {
                        using (var developerString = dataMap.getDeveloperString(DataManager.DevStringKey))
                        {
                            developerString.setValue(DataManager.SerializeLocalData().ToBase64String());
                            if (AGSPlayerClient.IsSignedIn())
                            {
                                AGSWhispersyncClient.Synchronize();
                            }
                            else
                            {
                                AGSWhispersyncClient.Flush();
                            }

                            saveInitialized = false;
                            DataManager.IsLocalDataDirty = false;
                            cloudOnceEvents.RaiseOnCloudSaveComplete(true);
                        }
                    }
                }
                else
                {
                    saveInitialized = false;
                    Debug.LogWarning("Attempted to save cloud data, but the AGS service is not ready.");
                    cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                }
            }
            else
            {
#if CO_DEBUG
                Debug.Log("Save called, but no data has changed since last save.");
#endif
                saveInitialized = false;
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
            }
        }

        /// <summary>
        /// Loads any available cloud data (if signed in and cloud saving is enabled).
        /// </summary>
        public void Load()
        {
            if (!AmazonCloudProvider.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
            {
#if CO_DEBUG
                Debug.LogWarning(!AmazonCloudProvider.Instance.CloudSaveInitialized
                    ? "Cloud Save has not been initialized, aborting cloud load."
                    : "Cloud Save is currently disabled, aborting cloud load.");
#endif
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                return;
            }

            if (AGSClient.IsServiceReady())
            {
#if CO_DEBUG
                Debug.Log("Loading cloud data");
#endif
                AGSWhispersyncClient.Synchronize();
                cloudOnceEvents.RaiseOnCloudLoadComplete(true);
            }
            else
            {
                Debug.LogWarning("Attempted to load cloud data, but the AGS service is not ready.");
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
            }
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
            isSynchronising = false;
        }

        private void ProcessCloudData()
        {
            if (processingCloudData)
            {
                return;
            }

            processingCloudData = true;

            if (AGSClient.IsServiceReady())
            {
                using (var dataMap = AGSWhispersyncClient.GetGameData())
                {
                    using (var developerString = dataMap.getDeveloperString(DataManager.DevStringKey))
                    {
                        var cloudValue = developerString.getCloudValue();
                        if (string.IsNullOrEmpty(cloudValue))
                        {
#if CO_DEBUG
                            Debug.Log("No data saved to the cloud yet.");
#endif
                            processingCloudData = false;
                            return;
                        }

                        if (!cloudValue.IsJson())
                        {
                            try
                            {
                                cloudValue = cloudValue.FromBase64StringToString();
                            }
                            catch (FormatException)
                            {
                                Debug.LogWarning("Unable to deserialize cloud data!");
                                return;
                            }
                        }
#if CO_DEBUG
                        Debug.Log("Processing cloud data");
#endif
                        var changedKeys = DataManager.MergeLocalDataWith(cloudValue);
                        if (changedKeys.Length > 0)
                        {
                            cloudOnceEvents.RaiseOnNewCloudValues(changedKeys);
                        }

                        processingCloudData = false;
                    }
                }
            }
            else
            {
                processingCloudData = false;
#if CO_DEBUG
                Debug.LogWarning("Attempting to process cloud data, but the AGS service is not ready!");
#endif
            }
        }
    }
}
#endif
