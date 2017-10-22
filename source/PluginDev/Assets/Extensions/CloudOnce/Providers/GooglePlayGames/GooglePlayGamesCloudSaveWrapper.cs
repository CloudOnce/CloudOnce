// <copyright file="GooglePlayGamesCloudSaveWrapper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_ANDROID && CLOUDONCE_GOOGLE
namespace CloudOnce.Internal.Providers
{
    using System;
    using System.Text;
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.SavedGame;
    using UnityEngine;
    using UnityEngine.Events;
    using Utils;
    using Logger = GooglePlayGames.OurUtils.Logger;

    /// <summary>
    /// This wrapper encapsulates the platform specific code needed to provide cloud storage on the Google Play platform.
    /// </summary>
    public class GooglePlayGamesCloudSaveWrapper : ICloudStorageProvider
    {
        private const string saveGameFileName = "GameData";

        private static float s_timeWhenCloudSaveWasLoaded;
        private static bool s_saveInitialized;
        private static bool s_loadInitialized;
        private static bool s_isSynchronising;

        private readonly CloudOnceEvents cloudOnceEvents;

        public GooglePlayGamesCloudSaveWrapper(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
        }

        #region Public methods

        /// <summary>
        /// Saves all cloud variables, to both disk and cloud.
        /// If <see cref="Cloud.CloudSaveEnabled"/> is <c>false</c>, it will only save to disk.
        /// Skips saving if no variables have been changed.
        /// </summary>
        public void Save()
        {
            if (s_saveInitialized)
            {
                return;
            }

            s_saveInitialized = true;

            DataManager.SaveToDisk();
            if (!GooglePlayGamesCloudProvider.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning(!GooglePlayGamesCloudProvider.Instance.CloudSaveInitialized
                    ? "Cloud Save has not been initialized, skipping upload and only saving to disk."
                    : "Cloud Save is currently disabled, skipping upload and only saving to disk.");
#endif
                s_saveInitialized = false;
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                return;
            }

            if (DataManager.IsLocalDataDirty)
            {
                if (GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
                {
                    PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                        saveGameFileName,
                        DataSource.ReadCacheOrNetwork,
                        ConflictResolutionStrategy.UseLongestPlaytime,
                        OnSavedGameOpenedForSave);
                }
                else
                {
#if CLOUDONCE_DEBUG
                    Debug.LogWarning("Save called, but user is not authenticated.");
#endif
                    s_saveInitialized = false;
                    cloudOnceEvents.RaiseOnCloudSaveComplete(false);
                }
            }
            else
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Save called, but no data has changed since last save.");
#endif
                s_saveInitialized = false;
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
            }
        }

        /// <summary>
        /// Loads saved game data from the server. File name is defined by <c>saveGameFileName</c>.
        /// </summary>
        public void Load()
        {
            if (!GooglePlayGamesCloudProvider.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
            {
#if CLOUDONCE_DEBUG
                Debug.LogWarning(!GooglePlayGamesCloudProvider.Instance.CloudSaveInitialized
                    ? "Cloud Save has not been initialized, aborting cloud load."
                    : "Cloud Save is currently disabled, aborting cloud load.");
#endif
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
                return;
            }

            if (s_loadInitialized)
            {
                return;
            }

            s_loadInitialized = true;

            if (GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Logger.d("Loading default save game.");

                PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                    saveGameFileName,
                    DataSource.ReadCacheOrNetwork,
                    ConflictResolutionStrategy.UseLongestPlaytime,
                    OnSavedGameOpenedForLoad);
            }
            else
            {
                Logger.w("Load can only be called after authentication.");
                OnSavedGameDataRead(SavedGameRequestStatus.AuthenticationError, null);
            }
        }

        /// <summary>
        /// Calls <see cref="Load"/> and then <see cref="Save"/> as soon as the Cloud load is complete.
        /// </summary>
        public void Synchronize()
        {
            if (s_isSynchronising)
            {
                return;
            }

            s_isSynchronising = true;
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

        /// <summary>
        /// Used internally when switching user.
        /// </summary>
        /// <param name="onDataLoaded">Action to invoke when data has been loaded.</param>
        public static void LoadDataString(UnityAction<string> onDataLoaded)
        {
#if CLOUDONCE_DEBUG
            Logger.d("Switching user. Loading default save game.");
#endif
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                saveGameFileName,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(
                            metadata,
                            (requestStatus, bytes) =>
                            {
                                if (requestStatus == SavedGameRequestStatus.Success)
                                {
                                    s_timeWhenCloudSaveWasLoaded = Time.realtimeSinceStartup;
                                    var dataString = GetDataString(bytes);
                                    onDataLoaded.Invoke(dataString);
                                }
                                else
                                {
                                    onDataLoaded.Invoke(null);
                                }
                            });
                    }
                    else
                    {
                        onDataLoaded.Invoke(null);
                    }
                });
        }

        #endregion /Public methods

        #region Private methods

        private static byte[] StringToBytes(string s)
        {
            if (s == null)
            {
                s = string.Empty;
            }

            return Encoding.Default.GetBytes(s);
        }

        private static string BytesToString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        private void OnSavedGameOpenedForLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
            }
            else
            {
                s_loadInitialized = false;
                Logger.w("Failed to open saved game. Request status: " + status);
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
            }
        }

        private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                s_timeWhenCloudSaveWasLoaded = Time.realtimeSinceStartup;
                ProcessCloudData(data);
            }
            else
            {
                s_loadInitialized = false;
                Logger.w("Failed to load saved game. Request status: " + status);
                cloudOnceEvents.RaiseOnCloudLoadComplete(false);
            }
        }

        private void OnSavedGameOpenedForSave(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                var savedGamePlayTime = game.TotalTimePlayed;
                savedGamePlayTime += TimeSpan.FromSeconds(Time.realtimeSinceStartup - s_timeWhenCloudSaveWasLoaded);
                SaveGame(game, StringToBytes(DataManager.SerializeLocalData().ToBase64String()), savedGamePlayTime);
            }
            else
            {
                s_saveInitialized = false;
                Logger.w("Failed to open saved game. Request status: " + status);
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
            }
        }

        private void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
        {
            var savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            var builder = default(SavedGameMetadataUpdate.Builder)
                .WithUpdatedPlayedTime(totalPlaytime)
                .WithUpdatedDescription("Saved game at " + DateTime.Now);
            savedGameClient.CommitUpdate(game, builder.Build(), savedData, OnSavedGameWritten);
        }

        private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Logger.d("Save successful!");
                DataManager.IsLocalDataDirty = false;
                cloudOnceEvents.RaiseOnCloudSaveComplete(true);
            }
            else
            {
                Logger.w("Failed to write saved game. Request status: " + status);
                cloudOnceEvents.RaiseOnCloudSaveComplete(false);
            }

            s_saveInitialized = false;
        }

        private void ProcessCloudData(byte[] cloudData)
        {
            if (cloudData == null)
            {
#if CLOUDONCE_DEBUG
                Debug.Log("No data saved to the cloud yet.");
#endif
                s_loadInitialized = false;
                cloudOnceEvents.RaiseOnCloudLoadComplete(true);
                return;
            }

            var dataString = GetDataString(cloudData);
            if (!string.IsNullOrEmpty(dataString))
            {
                var changedKeys = DataManager.MergeLocalDataWith(dataString);
                if (changedKeys.Length > 0)
                {
                    cloudOnceEvents.RaiseOnNewCloudValues(changedKeys);
                }

                s_loadInitialized = false;
                cloudOnceEvents.RaiseOnCloudLoadComplete(true);
            }
            else
            {
#if CLOUDONCE_DEBUG
                if (dataString != null)
                {
                    Debug.Log("No data saved to the cloud yet.");
                }
#endif
                s_loadInitialized = false;
                cloudOnceEvents.RaiseOnCloudLoadComplete(true);
            }
        }

        private static string GetDataString(byte[] bytes)
        {
            var dataString = BytesToString(bytes);
            if (!string.IsNullOrEmpty(dataString))
            {
                if (dataString.IsJson())
                {
                    return dataString;
                }

                try
                {
                    return dataString.FromBase64StringToString();
                }
                catch
                {
                    Debug.LogWarning("Unable to deserialize cloud data!");
                    return null;
                }
            }

            return string.Empty;
        }

        private void OnCloudLoadComplete(bool arg0)
        {
            Cloud.OnCloudLoadComplete -= OnCloudLoadComplete;
            Save();
            s_isSynchronising = false;
        }

        #endregion /Private methods
    }
}
#endif
