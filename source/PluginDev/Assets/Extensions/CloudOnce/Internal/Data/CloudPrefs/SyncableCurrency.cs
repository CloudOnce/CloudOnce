// <copyright file="SyncableCurrency.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utils;

    /// <summary>
    /// Handles virtual currencies. Keeps a record of all the earnings and purchases for every device the user has.
    /// This way the balance should always be accurate.
    /// </summary>
    public class SyncableCurrency : IJsonConvertible
    {
        #region Fields & properties

        private const string oldAliasCurrencyID = "cID";
        private const string oldAliasCurrencyData = "cData";

        // These strings are used for serializing this class
        private const string aliasCurrencyID = "i";
        private const string aliasCurrencyData = "d";

        private Dictionary<string, CurrencyValue> deviceCurrencyValues = new Dictionary<string, CurrencyValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncableCurrency"/> class.
        /// </summary>
        /// <param name="currencyID">A unique identifier for this currency.</param>
        public SyncableCurrency(string currencyID)
        {
            CurrencyID = currencyID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncableCurrency"/> class.
        /// Reconstructs the currency from a serialized string.
        /// </summary>
        /// <param name="jsonSerializedCurrency">A currency represented by a serialized string</param>
        public SyncableCurrency(JSONObject jsonSerializedCurrency)
        {
            FromJSONObject(jsonSerializedCurrency);
        }

        /// <summary>
        /// Unique identifier for this currency.
        /// </summary>
        public string CurrencyID { get; private set; }

        /// <summary>
        /// <c>Dictionary</c> containing device IDs and currency values for each.
        /// </summary>
        public Dictionary<string, CurrencyValue> DeviceCurrencyValues
        {
            get { return deviceCurrencyValues; }
            set { deviceCurrencyValues = value; }
        }

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Serializes the currency into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the currency.</returns>
        public JSONObject ToJSONObject()
        {
            var currencyValueObjects = new Dictionary<string, JSONObject>();
            foreach (var device in DeviceCurrencyValues)
            {
                currencyValueObjects.Add(device.Key, device.Value.ToJSONObject());
            }

            var jsonCurrencyValue = new JSONObject(currencyValueObjects);

            var jsonCurrencyId = JSONObject.CreateStringObject(CurrencyID);

            var container = JSONObject.Create(JSONObject.Type.Object);

            container.AddField(aliasCurrencyID, jsonCurrencyId);
            container.AddField(aliasCurrencyData, jsonCurrencyValue);

            return container;
        }

        /// <summary>
        /// Recreates the currency from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the currency data.</param>
        public void FromJSONObject(JSONObject jsonObject)
        {
            var idAlias = CloudOnceUtils.GetAlias(typeof(SyncableCurrency).Name, jsonObject, aliasCurrencyID, oldAliasCurrencyID);
            var dataAlias = CloudOnceUtils.GetAlias(typeof(SyncableCurrency).Name, jsonObject, aliasCurrencyData, oldAliasCurrencyData);

            CurrencyID = jsonObject[idAlias].String;
            DeviceCurrencyValues = JsonHelper.Convert<Dictionary<string, CurrencyValue>>(jsonObject[dataAlias]);
        }

        /// <summary>
        /// Merges the currency with another record of it, most likely gotten from the cloud
        /// </summary>
        /// <param name="otherData">The other record of the same currency</param>
        /// <returns><c>true</c> if there was any change to this <see cref="SyncableCurrency"/>, <c>false</c> if there was no new data.</returns>
        public bool MergeWith(SyncableCurrency otherData)
        {
            var newData = false;

            // Check if the other data contains the same currency
            if (otherData.CurrencyID != CurrencyID)
            {
                Debug.LogError("Attempted to merge two different currencies, this is not allowed!");
                return false;
            }

            // If we have no local currency values, we just use the new values
            if (DeviceCurrencyValues == null)
            {
                DeviceCurrencyValues = otherData.DeviceCurrencyValues;
                newData = true;
            }
            else
            {
                // Merge currency values
                foreach (var device in otherData.DeviceCurrencyValues)
                {
                    // Is the device listed in the currency data dictionary?
                    CurrencyValue localCurrencyData;
                    if (DeviceCurrencyValues.TryGetValue(device.Key, out localCurrencyData))
                    {
                        // If the additions in the other data is larger, update the local additions
                        if (device.Value.Additions > localCurrencyData.Additions)
                        {
                            localCurrencyData.Additions = device.Value.Additions;
                            newData = true;
                        }

                        // If the subtractions in the other data is larger, update the local subtraction
                        if (device.Value.Subtractions < localCurrencyData.Subtractions)
                        {
                            localCurrencyData.Subtractions = device.Value.Subtractions;
                            newData = true;
                        }
                    }
                    else
                    {
                        // If the device is not listed, add it and it's value
                        DeviceCurrencyValues.Add(device.Key, device.Value);
                        newData = true;
                    }
                }
            }

            return newData;
        }

        /// <summary>
        /// Use with caution! Completely resets currency. All additions and subtractions for every registered device are lost.
        /// </summary>
        public void ResetCurrency()
        {
            foreach (var kvp in DeviceCurrencyValues)
            {
                kvp.Value.Additions = 0f;
                kvp.Value.Subtractions = 0f;
            }
        }

        #endregion /Public methods
    }
}
