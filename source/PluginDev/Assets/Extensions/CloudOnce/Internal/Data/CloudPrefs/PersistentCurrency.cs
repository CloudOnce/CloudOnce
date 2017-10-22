// <copyright file="PersistentCurrency.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A currency that is stored in the cloud.
    /// </summary>
    public class PersistentCurrency : IPersistent
    {
        #region Fields, constructors & delegates

        private const string deviceIdKey = "CloudOnceDeviceID";
        private static string s_deviceIdCache;

        private Dictionary<string, CurrencyValue> deviceCurrencyValues;
        private CurrencyValue thisDeviceCurrencyValue;
        private float otherDevicesValueCache;

        protected PersistentCurrency(string key, float defaultValue, bool allowNegative)
        {
            Key = key;
            DefaultValue = defaultValue;
            AllowNegative = allowNegative;

            DataManager.CloudPrefs[key] = this;
            DataManager.InitDataManager();
            Load();
        }

        #endregion /Fields, constructors & delegates

        #region Properties

        /// <summary>
        /// The unique identifier for this particular currency.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Total additions made to this currency.
        /// </summary>
        public float Additions
        {
            get
            {
                var additions = thisDeviceCurrencyValue.Additions;
                if (deviceCurrencyValues != null)
                {
                    // Add together the additions for each registered device
                    foreach (var device in deviceCurrencyValues)
                    {
                        // Skip if current device
                        if (device.Key == DeviceID)
                        {
                            continue;
                        }

                        additions += device.Value.Additions;
                    }
                }

                return additions;
            }
        }

        /// <summary>
        /// Total subtractions made to this currency.
        /// </summary>
        public float Subtractions
        {
            get
            {
                var subtractions = thisDeviceCurrencyValue.Subtractions;
                if (deviceCurrencyValues != null)
                {
                    // Add together the subtractions for each registered device
                    foreach (var device in deviceCurrencyValues)
                    {
                        // Skip if current device
                        if (device.Key == DeviceID)
                        {
                            continue;
                        }

                        subtractions += device.Value.Subtractions;
                    }
                }

                return subtractions;
            }
        }

        /// <summary>
        /// Current value for this currency.
        /// </summary>
        public float Value
        {
            get
            {
                var value = thisDeviceCurrencyValue.Value + DefaultValue;
                if (deviceCurrencyValues != null)
                {
                    // Add together the values for each registered device
                    foreach (var device in deviceCurrencyValues)
                    {
                        // Skip if current device
                        if (device.Key == DeviceID)
                        {
                            continue;
                        }

                        value += device.Value.Value;
                    }
                }

                // Set balance to 0 if negative
                if (!AllowNegative && value < 0f)
                {
                    Value = 0f;
                    return 0f;
                }

                return value;
            }

            set
            {
                if (AllowNegative || value >= 0f)
                {
                    thisDeviceCurrencyValue.Value = value - otherDevicesValueCache - DefaultValue;
                }
                else
                {
                    // Set balance to 0
                    thisDeviceCurrencyValue.Value = -otherDevicesValueCache - DefaultValue;
                }
            }
        }

        /// <summary>
        /// The currency's default/starting value. Can't be changed after the currency is created.
        /// </summary>
        public float DefaultValue { get; private set; }

        /// <summary>
        /// If the value of this currency is allowed to be negative.
        /// </summary>
        public bool AllowNegative { get; private set; }

        private static string DeviceID
        {
            get
            {
                if (!string.IsNullOrEmpty(s_deviceIdCache))
                {
                    return s_deviceIdCache;
                }

                if (PlayerPrefs.HasKey(deviceIdKey))
                {
                    s_deviceIdCache = PlayerPrefs.GetString(deviceIdKey);
                    return s_deviceIdCache;
                }

                s_deviceIdCache = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(deviceIdKey, s_deviceIdCache);
                PlayerPrefs.Save();
                return s_deviceIdCache;
            }
        }

        #endregion /Properties

        #region Methods

        public void Flush()
        {
            if (deviceCurrencyValues == null)
            {
                deviceCurrencyValues = new Dictionary<string, CurrencyValue>();
            }

            deviceCurrencyValues[DeviceID] = thisDeviceCurrencyValue;
            DataManager.SetCurrencyValues(Key, deviceCurrencyValues);
        }

        public void Load(bool force = false)
        {
            deviceCurrencyValues = DataManager.GetCurrencyValues(Key);
            if (deviceCurrencyValues != null)
            {
                thisDeviceCurrencyValue = deviceCurrencyValues.ContainsKey(DeviceID)
                    ? deviceCurrencyValues[DeviceID]
                    : new CurrencyValue();
                CacheValueFromOtherDevices();
            }
            else
            {
                thisDeviceCurrencyValue = new CurrencyValue();
            }
        }

        /// <summary>
        /// Use with caution! Completely resets currency. All additions and subtractions for every registered device are lost.
        /// </summary>
        public void Reset()
        {
            DataManager.ResetSyncableCurrency(Key);
            Load();
        }

        private void CacheValueFromOtherDevices()
        {
            otherDevicesValueCache = 0f;

            // Add together the values for each registered device
            foreach (var device in deviceCurrencyValues)
            {
                // Skip if current device
                if (device.Key == DeviceID)
                {
                    continue;
                }

                otherDevicesValueCache += device.Value.Value;
            }
        }

        #endregion /Methods
    }
}
