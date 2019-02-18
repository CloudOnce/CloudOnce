// <copyright file="DataManager.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
#if UNITY_EDITOR
    using System.Linq;
#endif
#if !UNITY_EDITOR && CLOUDONCE_GOOGLE
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.SavedGame;
#endif
#if !UNITY_EDITOR
    using Providers;
#endif
    using UnityEngine;
    using Utils;

    /// <summary>
    /// Stores and retrieves key-value pairs. Serializes and deserializes <see cref="GameData"/>
    /// to/from the cloud and the local disk. In case of conflicts it uses <see cref="ConflictResolver"/>
    /// to figure out what data to keep and what to discard.
    /// </summary>
    public static class DataManager
    {
        #region Fields & properties

        // Used to identify serialized strings of GameData
        public const string DevStringKey = "CloudOnceDevString";

        // Backing fields
        private static Dictionary<string, IPersistent> s_cloudPrefs;
        private static GameData s_localGameData = new GameData();

        private static bool s_isInitialized;

        /// <summary>
        /// Whether or not any of the local data has been changed since the last upload to the cloud.
        /// </summary>
        public static bool IsLocalDataDirty
        {
            get { return s_localGameData.IsDirty; }
            set { s_localGameData.IsDirty = value; }
        }

        /// <summary>
        /// Stores all the registered cloud preferences created by the user.
        /// </summary>
        public static Dictionary<string, IPersistent> CloudPrefs
        {
            get { return s_cloudPrefs ?? (s_cloudPrefs = new Dictionary<string, IPersistent>()); }
        }

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Loads any stored local data as part of the initialization.
        /// </summary>
        public static void InitDataManager()
        {
            if (!s_isInitialized)
            {
                LoadFromDisk();
                s_isInitialized = true;
            }
        }

        #region Set methods

        /// <summary>
        /// Set values for a currency.
        /// </summary>
        /// <param name="key">The ID for the currency. Is unique to a specific currency.</param>
        /// <param name="currencyValues"><c>Dictionary</c> containing device IDs and currency values for each.</param>
        public static void SetCurrencyValues(string key, Dictionary<string, CurrencyValue> currencyValues)
        {
            if (!s_localGameData.SyncableCurrencies.ContainsKey(key))
            {
                s_localGameData.SyncableCurrencies.Add(key, new SyncableCurrency(key));
            }

            s_localGameData.SyncableCurrencies[key].DeviceCurrencyValues = currencyValues;
            IsLocalDataDirty = true;
        }

        /// <summary>
        /// Used to set a <see cref="bool"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="bool"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetBool(string key, bool value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Bool, persistenceType);
                var syncableItem = new SyncableItem(value.ToString(), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Bool)
            {
                s_localGameData.SyncableItems[key].ValueString = value
                    ? 1.ToString(CultureInfo.InvariantCulture)
                    : 0.ToString(CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(bool));
            }
        }

        /// <summary>
        /// Used to set an <see cref="int"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="int"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetInt(string key, int value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Int, persistenceType);
                var syncableItem = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Int)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(int));
            }
        }

        /// <summary>
        /// Used to set a <see cref="uint"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="uint"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetUInt(string key, uint value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.UInt, persistenceType);
                var syncableItem = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.UInt)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(uint));
            }
        }

        /// <summary>
        /// Used to set a <see cref="float"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="float"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetFloat(string key, float value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Float, persistenceType);
                var syncableItem = new SyncableItem(value.ToString("R", CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Float)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToString("R", CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(float));
            }
        }

        /// <summary>
        /// Used to set a <see cref="double"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="double"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetDouble(string key, double value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Double, persistenceType);
                var syncableItem = new SyncableItem(value.ToString("R", CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Double)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToString("R", CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(double));
            }
        }

        /// <summary>
        /// Used to set a <see cref="string"/> that will be stored in the cloud. PersistenceType.Latest will be used in case of data conflict.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="string"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetString(string key, string value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.String, persistenceType);
                var syncableItem = new SyncableItem(value, metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.String)
            {
                s_localGameData.SyncableItems[key].ValueString = value;
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(string));
            }
        }

        /// <summary>
        /// Used to set a <see cref="long"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="long"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetLong(string key, long value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Long, persistenceType);
                var syncableItem = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Long)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(long));
            }
        }

        /// <summary>
        /// Used to set a <see cref="DateTime"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="DateTime"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetDateTime(string key, DateTime value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Long, persistenceType);
                var syncableItem = new SyncableItem(value.ToBinary().ToString(CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Long)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToBinary().ToString(CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(long));
            }
        }

        /// <summary>
        /// Used to set a <see cref="decimal"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="decimal"/>.</param>
        /// <param name="persistenceType">The persistence type to use in case of a data conflict (ignored if value has been set before).</param>
        public static void SetDecimal(string key, decimal value, PersistenceType persistenceType)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.Decimal, persistenceType);
                var syncableItem = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metaData);
                s_localGameData.SyncableItems.Add(key, syncableItem);
                IsLocalDataDirty = true;
            }

            if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Decimal)
            {
                s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
                IsLocalDataDirty = true;
            }
            else
            {
                throw new UnexpectedCollectionElementTypeException(key, typeof(decimal));
            }
        }

        #endregion /Set methods

        #region Get methods

        /// <summary>
        /// Get values for a currency.
        /// </summary>
        /// <param name="key">The ID for the currency. Is unique to a specific currency.</param>
        /// <returns><c>Dictionary</c> containing device IDs and currency values for each.</returns>
        public static Dictionary<string, CurrencyValue> GetCurrencyValues(string key)
        {
            SyncableCurrency value;
            return s_localGameData.SyncableCurrencies.TryGetValue(key, out value)
                ? value.DeviceCurrencyValues
                : null;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="bool"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="bool"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="bool"/>.</returns>
        public static bool GetBool(string key, bool defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Bool)
                {
                    int result;
                    if (int.TryParse(localItem.ValueString, out result))
                    {
                        return result == 1;
                    }

                    return Convert.ToBoolean(localItem.ValueString, CultureInfo.InvariantCulture);
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(bool));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="int"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="int"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="int"/>.</returns>
        public static int GetInt(string key, int defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Int)
                {
                    return Convert.ToInt32(localItem.ValueString);
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(int));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="uint"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="uint"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="uint"/>.</returns>
        public static uint GetUInt(string key, uint defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.UInt)
                {
                    return Convert.ToUInt32(localItem.ValueString, CultureInfo.InvariantCulture);
                }
#if CLOUDONCE_DEBUG
                Debug.LogError("DataType is not a uint");
#endif
                return default(uint);
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="float"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="float"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="float"/>.</returns>
        public static float GetFloat(string key, float defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Float)
                {
                    return Convert.ToSingle(localItem.ValueString, CultureInfo.InvariantCulture);
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(float));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="double"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="double"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="double"/>.</returns>
        public static double GetDouble(string key, double defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Double)
                {
                    return Convert.ToDouble(localItem.ValueString, CultureInfo.InvariantCulture);
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(double));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="string"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="string"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="string"/>.</returns>
        public static string GetString(string key, string defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.String)
                {
                    return localItem.ValueString;
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(string));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="long"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="long"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="long"/>.</returns>
        public static long GetLong(string key, long defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Long)
                {
                    return Convert.ToInt64(localItem.ValueString, CultureInfo.InvariantCulture);
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(long));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="DateTime"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTime(string key, DateTime defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Long)
                {
                    return DateTime.FromBinary(Convert.ToInt64(localItem.ValueString, CultureInfo.InvariantCulture));
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(long));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of a specified <see cref="decimal"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="decimal"/>.</param>
        /// <param name="defaultValue">Value to return if key does not exist.</param>
        /// <returns>The value of the specified <see cref="decimal"/>.</returns>
        public static decimal GetDecimal(string key, decimal defaultValue)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.Decimal)
                {
                    return Convert.ToDecimal(localItem.ValueString, CultureInfo.InvariantCulture);
                }

                throw new UnexpectedCollectionElementTypeException(key, typeof(decimal));
            }

            return defaultValue;
        }

        #endregion /Get methods

        /// <summary>
        /// Refreshes all the cloud preferences created by the user.
        /// Is used after merging the local data with data from the cloud.
        /// </summary>
        public static void RefreshCloudValues()
        {
            foreach (var playerPreference in CloudPrefs)
            {
                playerPreference.Value.Load();
            }
        }

        /// <summary>
        /// Will completely reset the specified <see cref="SyncableCurrency"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="SyncableCurrency"/> you want to reset.</param>
        public static void ResetSyncableCurrency(string key)
        {
            if (!s_localGameData.SyncableCurrencies.ContainsKey(key))
            {
                s_localGameData.SyncableCurrencies.Add(key, new SyncableCurrency(key));
            }
            else
            {
                s_localGameData.SyncableCurrencies[key].ResetCurrency();
            }
            
            IsLocalDataDirty = true;
        }

        /// <summary>
        /// Resets a CloudPref to its default value.
        /// </summary>
        /// <returns>Whether or not the CloudPref was successfully reset.</returns>
        public static bool ResetCloudPref(string key)
        {
            if (CloudPrefs.ContainsKey(key))
            {
                CloudPrefs[key].Reset();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes a specific cloud variable from local cache and the cloud.
        /// </summary>
        /// <param name="key">The unique identifier for the cloud variable you want to delete.</param>
        /// <returns>
        /// <c>true</c> if the cloud variable is found and deleted, <c>false</c> if the specified <paramref name="key"/> doesn't exist.
        /// </returns>
        public static bool DeleteCloudPref(string key)
        {
            if (s_localGameData.SyncableItems.ContainsKey(key))
            {
                s_localGameData.SyncableItems.Remove(key);
                return true;
            }

            if (s_localGameData.SyncableCurrencies.ContainsKey(key))
            {
                s_localGameData.SyncableCurrencies.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// WARNING! Resets all cloud variables back to their default values both locally and in the cloud!
        /// Should only be used while developing, not in production builds.
        /// Values are not saved automatically after the reset, and will go back to previously saved values
        /// if the game is restarted before saving.
        /// </summary>
        /// <returns>A list of all cloud variable keys from the local <see cref="GameData"/>.</returns>
        public static string[] ResetAllData()
        {
            foreach (var playerPreference in CloudPrefs)
            {
                playerPreference.Value.Reset();
            }

#if CLOUDONCE_DEBUG
            Debug.LogWarning("All cloud variables have been reset!");
#endif
            return s_localGameData.GetAllKeys();
        }

        /// <summary>
        /// WARNING! Deletes all cloud variables both locally and in the cloud (if logged into a cloud save service)!
        /// Should only be used while developing, not in production builds.
        /// </summary>
        public static void DeleteAllCloudVariables()
        {
            DeleteCloudData();
            ClearStowawayVariablesFromGameData();
            foreach (var playerPreference in CloudPrefs)
            {
                playerPreference.Value.Reset();
            }

#if CLOUDONCE_DEBUG
            Debug.LogWarning("All cloud variables have been deleted!");
#endif
        }

        /// <summary>
        /// Used after clearing cloud data, to make sure that unwanted variables don't get re-saved to the cloud.
        /// </summary>
        public static string[] ClearStowawayVariablesFromGameData()
        {
            var stowawayItems = new List<string>();
            foreach (var syncableItem in s_localGameData.SyncableItems)
            {
                if (!s_cloudPrefs.ContainsKey(syncableItem.Key))
                {
                    stowawayItems.Add(syncableItem.Key);
                }
            }

            var stowawayCurrencies = new List<string>();
            foreach (var syncableCurrency in s_localGameData.SyncableCurrencies)
            {
                if (!s_cloudPrefs.ContainsKey(syncableCurrency.Key))
                {
                    stowawayCurrencies.Add(syncableCurrency.Key);
                }
            }

            foreach (var item in stowawayItems)
            {
                s_localGameData.SyncableItems.Remove(item);
            }

            foreach (var currency in stowawayCurrencies)
            {
                s_localGameData.SyncableCurrencies.Remove(currency);

                // We add the currencies to the items list so we can simply return the items list as an array.
                stowawayItems.Add(currency);
            }

            return stowawayItems.ToArray();
        }

        #region Public serialization methods

        /// <summary>
        /// Goes through all the cloud preferences created by the user,
        /// and stores them all in <see cref="PlayerPrefs"/> as one serialized <see cref="string"/>.
        /// </summary>
        public static void SaveToDisk()
        {
            foreach (var playerPreference in CloudPrefs)
            {
                playerPreference.Value.Flush();
            }

            if (IsLocalDataDirty)
            {
                PlayerPrefs.SetString(DevStringKey, SerializeLocalData().ToBase64String());
                PlayerPrefs.Save();
#if CLOUDONCE_DEBUG
                Debug.Log("Data saved to local cache");
#endif
            }
        }

        /// <summary>
        /// Loads data stored in <see cref="PlayerPrefs"/>.
        /// </summary>
        public static void LoadFromDisk()
        {
            var devString = PlayerPrefs.GetString(DevStringKey);
            if (string.IsNullOrEmpty(devString))
            {
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
                    Debug.LogWarning("Unable to deserialize local data!");
                    return;
                }
            }

            if (!s_isInitialized)
            {
                s_localGameData = new GameData(devString);
            }
            else
            {
                var changedKeys = MergeLocalDataWith(devString);
                if (changedKeys.Length > 0)
                {
                    RefreshCloudValues();
                }
            }

#if CLOUDONCE_DEBUG
            Debug.Log("Data loaded from local cache");
#endif
        }

        /// <summary>
        /// Used by cloud save providers to serialize the local <see cref="GameData"/> for storing it in the cloud.
        /// </summary>
        /// <returns>The local <see cref="GameData"/> as a serialized <see cref="string"/>.</returns>
        public static string SerializeLocalData()
        {
            return s_localGameData.Serialize();
        }

        #endregion /Serialization methods

        #region Cloud methods

        /// <summary>
        /// Used by cloud save providers when <see cref="GameData"/> it downloaded from the cloud needs
        /// to be merged with the existing local <see cref="GameData"/>.
        /// </summary>
        /// <param name="otherData"><see cref="GameData"/> as a serialized <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> array of the changed keys. Will be empty if no data changed.</returns>
        public static string[] MergeLocalDataWith(string otherData)
        {
            var changedKeys = s_localGameData.MergeWith(new GameData(otherData));
            if (changedKeys.Length > 0)
            {
                RefreshCloudValues();
                SaveToDisk();
            }

            return changedKeys;
        }

        /// <summary>
        /// Only to be used when player has switched to a different account.
        /// </summary>
        /// <param name="otherData"><see cref="GameData"/> as a serialized <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> array of all the keys. Will be empty if the new <see cref="GameData"/> has no values.</returns>
        public static string[] ReplaceLocalDataWith(string otherData)
        {
            s_localGameData = new GameData(otherData);
            foreach (var playerPreference in CloudPrefs)
            {
                playerPreference.Value.Load(true);
            }

            SaveToDisk();
            return s_localGameData.GetAllKeys();
        }

        #endregion /Cloud methods

#if UNITY_EDITOR
        /// <summary>
        /// This method is only for simulating OnNewCloudValues in the Unity editor.
        /// </summary>
        /// <returns>A random selection of keys from the local <see cref="GameData"/>.</returns>
        public static string[] GetRandomKeysFromGameData()
        {
            Debug.Log("Simulating OnNewCloudValues with random keys.");
            var rnd = new System.Random();
            var rndItems =
                s_localGameData.SyncableItems.OrderBy(x => rnd.Next())
                    .Take(UnityEngine.Random.Range(0, s_localGameData.SyncableItems.Count + 1));
            var rndCurrencies =
                s_localGameData.SyncableCurrencies.OrderBy(x => rnd.Next())
                    .Take(UnityEngine.Random.Range(0, s_localGameData.SyncableCurrencies.Count + 1));
            var changedKeys = rndItems.Select(item => item.Key).ToList();
            changedKeys.AddRange(rndCurrencies.Select(currency => currency.Key));
            return changedKeys.ToArray();
        }
#endif

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Clears all cloud variables currently stored in the cloud.
        /// </summary>
        private static void DeleteCloudData()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
#if CLOUDONCE_GOOGLE
            if (GooglePlayGamesCloudProvider.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                    "GameData",
                    DataSource.ReadCacheOrNetwork,
                    ConflictResolutionStrategy.UseLongestPlaytime,
                    (status, metadata) =>
                    {
                        if (status == SavedGameRequestStatus.Success)
                        {
                            PlayGamesPlatform.Instance.SavedGame.Delete(metadata);
                        }
                    });
            }
#endif
#elif !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            iCloudBridge.DeleteString(DevStringKey);
#endif
            PlayerPrefs.DeleteKey(DevStringKey);
            PlayerPrefs.Save();
        }

        #endregion / Private methods
    }
}
