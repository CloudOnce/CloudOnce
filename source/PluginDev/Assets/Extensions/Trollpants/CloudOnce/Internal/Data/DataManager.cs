// <copyright file="DataManager.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Trollpants.CloudOnce.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
#if UNITY_EDITOR
    using System.Linq;
#endif
#if !UNITY_EDITOR && TP_AndroidGoogle
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
        public const string DevStringKey = "TrollpantsDevString";

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

        #region Initialization methods

        /// <summary>
        /// Loads any stored local data as part of the initialization.
        /// </summary>
        public static void InitDataManager()
        {
            if (!s_isInitialized)
            {
                s_isInitialized = true;
                LoadFromDisk();
            }
        }

        /// <summary>
        /// Ensures that a specified currency exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">The unique identifier for this particular currency.</param>
        public static void InitializeCurrency(string key)
        {
            if (!s_localGameData.SyncableCurrencies.ContainsKey(key))
            {
                s_localGameData.SyncableCurrencies.Add(key, new SyncableCurrency(key));
                IsLocalDataDirty = true;
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="bool"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer <c>true</c>.
        /// <see cref="PersistenceType.Lowest"/> will prefer <c>false</c>.
        /// </param>
        /// <param name="value">The initial value for this <see cref="bool"/>.</param>
        public static void InitializeBool(string key, PersistenceType persistenceType, bool value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Bool,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString(),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="int"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The initial value for this <see cref="int"/>.</param>
        public static void InitializeInt(string key, PersistenceType persistenceType, int value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Int,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString(CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="uint"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The initial value for this <see cref="uint"/>.</param>
        public static void InitializeUInt(string key, PersistenceType persistenceType, uint value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.UInt,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString(CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="float"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The initial value for this <see cref="float"/>.</param>
        public static void InitializeFloat(string key, PersistenceType persistenceType, float value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Float,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString("R", CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="double"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The initial value for this <see cref="double"/>.</param>
        public static void InitializeDouble(string key, PersistenceType persistenceType, double value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Double,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString("R", CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="string"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) <see cref="string"/>.
        /// <see cref="PersistenceType.Highest"/> will prefer the longest <see cref="string"/>.
        /// <see cref="PersistenceType.Lowest"/> will prefer the shortest <see cref="string"/>.
        /// </param>
        /// <param name="value">The initial value for this <see cref="string"/>.</param>
        public static void InitializeString(string key, PersistenceType persistenceType, string value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(DataType.String, persistenceType);
                var syncableItem = new SyncableItem(value, metaData);
                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="long"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The initial value for this <see cref="long"/>.</param>
        public static void InitializeLong(string key, PersistenceType persistenceType, long value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Long,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString(CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="DateTime"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will prefer the <see cref="DateTime"/> that was SAVED last.
        /// <see cref="PersistenceType.Highest"/> will prefer the newest <see cref="DateTime"/> value in ticks.
        /// <see cref="PersistenceType.Lowest"/> will prefer the oldest <see cref="DateTime"/> value in ticks.
        /// </param>
        /// <param name="value">The initial value for this <see cref="DateTime"/>.</param>
        public static void InitializeDateTime(string key, PersistenceType persistenceType, DateTime value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Long,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToBinary().ToString(CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        /// <summary>
        /// Ensures that a specified <see cref="decimal"/> exists in the local <see cref="GameData"/>.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="persistenceType">
        /// The persistence type to use in case of a data conflict.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The initial value for this <see cref="decimal"/>.</param>
        public static void InitializeDecimal(string key, PersistenceType persistenceType, decimal value)
        {
            if (!s_localGameData.SyncableItems.ContainsKey(key))
            {
                var metaData = new SyncableItemMetaData(
                    DataType.Decimal,
                    persistenceType);

                var syncableItem = new SyncableItem(
                    value.ToString(CultureInfo.InvariantCulture),
                    metaData);

                CreateItem(key, syncableItem);
            }
        }

        #endregion /Initialization methods

        #region Set methods

        /// <summary>
        /// Set values for a currency.
        /// </summary>
        /// <param name="key">The ID for the currency. Is unique to a specific currency.</param>
        /// <param name="currencyValues"><c>Dictionary</c> containing device IDs and currency values for each.</param>
        public static void SetCurrencyValues(string key, Dictionary<string, CurrencyValue> currencyValues)
        {
            SyncableCurrency value;
            if (s_localGameData.SyncableCurrencies.TryGetValue(key, out value))
            {
                value.DeviceCurrencyValues = currencyValues;
                IsLocalDataDirty = true;
            }
            else
            {
                throw new KeyNotFoundException(key);
            }
        }

        /// <summary>
        /// Used to set a <see cref="bool"/> that will be stored in the cloud.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="value">The value for this <see cref="bool"/>.</param>
        public static void SetBool(string key, bool value)
        {
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
        public static void SetInt(string key, int value)
        {
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
        public static void SetUInt(string key, uint value)
        {
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
        public static void SetFloat(string key, float value)
        {
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
        public static void SetDouble(string key, double value)
        {
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
        public static void SetString(string key, string value)
        {
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
        public static void SetLong(string key, long value)
        {
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
        public static void SetDateTime(string key, DateTime value)
        {
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
        public static void SetDecimal(string key, decimal value)
        {
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
        /// <returns>The value of the specified <see cref="bool"/>.</returns>
        public static bool GetBool(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="int"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="int"/>.</param>
        /// <returns>The value of the specified <see cref="int"/>.</returns>
        public static int GetInt(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="uint"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="uint"/>.</param>
        /// <returns>The value of the specified <see cref="uint"/>.</returns>
        public static uint GetUInt(string key)
        {
            SyncableItem localItem;

            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                if (localItem.Metadata.DataType == DataType.UInt)
                {
                    return Convert.ToUInt32(localItem.ValueString, CultureInfo.InvariantCulture);
                }
#if CO_DEBUG
                Debug.LogError("DataType is not a uint");
#endif
                return default(uint);
            }

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="float"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="float"/>.</param>
        /// <returns>The value of the specified <see cref="float"/>.</returns>
        public static float GetFloat(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="double"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="double"/>.</param>
        /// <returns>The value of the specified <see cref="double"/>.</returns>
        public static double GetDouble(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="string"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="string"/>.</param>
        /// <returns>The value of the specified <see cref="string"/>.</returns>
        public static string GetString(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="long"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="long"/>.</param>
        /// <returns>The value of the specified <see cref="long"/>.</returns>
        public static long GetLong(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="DateTime"/>.</param>
        /// <returns>The value of the specified <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTime(string key)
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

            throw new KeyNotFoundException(key);
        }

        /// <summary>
        /// Returns the value of a specified <see cref="decimal"/>.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="decimal"/>.</param>
        /// <returns>The value of the specified <see cref="decimal"/>.</returns>
        public static decimal GetDecimal(string key)
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

            throw new KeyNotFoundException(key);
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
            SyncableCurrency localCurrency;

            if (s_localGameData.SyncableCurrencies.TryGetValue(key, out localCurrency))
            {
                localCurrency.ResetCurrency();
                IsLocalDataDirty = true;
            }
            else
            {
                throw new KeyNotFoundException(key);
            }
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

#if CO_DEBUG
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

#if CO_DEBUG
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
#if CO_DEBUG
                Debug.Log("Data saved to local cache");
#endif
            }
        }

        /// <summary>
        /// Loads data stored in <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <returns><c>true</c> if the local data was changed, <c>false</c> if there was no new data.</returns>
        public static string[] LoadFromDisk()
        {
            var devstring = PlayerPrefs.GetString(DevStringKey);
            if (!string.IsNullOrEmpty(devstring))
            {
                if (!devstring.IsJson())
                {
                    try
                    {
                        devstring = devstring.FromBase64StringToString();
                    }
                    catch (FormatException)
                    {
                        Debug.LogWarning("Unable to deserialize local data! Resetting it.");
                        devstring = string.Empty;
                    }
                }
            }

            string[] changedKeys;
            if (s_localGameData == null)
            {
                s_localGameData = new GameData(devstring);
                RefreshCloudValues();
                changedKeys = new string[0];
            }
            else
            {
                changedKeys = MergeLocalDataWith(devstring);
                if (changedKeys.Length > 0)
                {
                    RefreshCloudValues();
                }
            }
#if CO_DEBUG
            Debug.Log("Data loaded from local cache");
#endif
            return changedKeys;
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
                playerPreference.Value.Reset();
            }

            RefreshCloudValues();
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
        /// Used by above initialize methods to create any type of cloud preference.
        /// </summary>
        /// <param name="key">Must be a unique identifier for this specific value.</param>
        /// <param name="item">The <see cref="SyncableItem"/> you want to store in the local <see cref="GameData"/>.</param>
        private static void CreateItem(string key, SyncableItem item)
        {
            SyncableItem localItem;

            // If the item already exists, update it instead of creating a new item
            if (s_localGameData.SyncableItems.TryGetValue(key, out localItem))
            {
                // Ignore if the new item doesn't have the same persistence type as the current item
                if (localItem.Metadata.PersistenceType == item.Metadata.PersistenceType)
                {
                    // Only set item if the value is different from the current value
                    if (!localItem.Equals(item))
                    {
                        s_localGameData.SyncableItems[key] = ConflictResolver.ResolveConflict(localItem, item);
                        IsLocalDataDirty = true;
                    }
                }
            }
            else
            {
                // Create new item
                s_localGameData.SyncableItems.Add(key, item);
                IsLocalDataDirty = true;
            }
        }

        /// <summary>
        /// Clears all cloud variables currently stored in the cloud.
        /// </summary>
        private static void DeleteCloudData()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
#if TP_AndroidAmazon
            if (AGSClient.IsServiceReady())
            {
                using (var dataMap = AGSWhispersyncClient.GetGameData())
                {
                    using (var developerString = dataMap.getDeveloperString(DevStringKey))
                    {
                        developerString.setValue(string.Empty);
                        if (AGSPlayerClient.IsSignedIn())
                        {
                            AGSWhispersyncClient.Synchronize();
                        }
                        else
                        {
                            AGSWhispersyncClient.Flush();
                        }
                    }
                }
            }
#elif TP_AndroidGoogle
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
#elif !UNITY_EDITOR && UNITY_IOS
            iCloudBridge.DeleteString(DevStringKey);
#endif
            PlayerPrefs.DeleteKey(DevStringKey);
            PlayerPrefs.Save();
        }

        #endregion / Private methods
    }
}
