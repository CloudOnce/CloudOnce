// <copyright file="SyncableItemMetaData.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using System.Globalization;
    using Utils;

    /// <summary>
    /// Holds data about a <see cref="SyncableItem"/>.
    /// </summary>
    public class SyncableItemMetaData : IEquatable<SyncableItemMetaData>, IJsonConvertible
    {
        // Short names for serialization
        private const string oldAliasDataType = "dT";
        private const string oldAliasPersistenceType = "pT";
        private const string oldAliasTimestamp = "tS";

        private const string aliasDataType = "d";
        private const string aliasPersistenceType = "p";
        private const string aliasTimestamp = "t";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncableItemMetaData"/> class.
        /// </summary>
        /// <param name="dataType">The data type</param>
        /// <param name="persistenceType">The selected conflict resolution type</param>
        public SyncableItemMetaData(
            DataType dataType,
            PersistenceType persistenceType)
        {
            DataType = dataType;
            PersistenceType = persistenceType;
            if (persistenceType == PersistenceType.Latest)
            {
                Timestamp = new DateTime(2014, 06, 30);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncableItemMetaData"/> class.
        /// Reconstructs the meta data from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the meta data.</param>
        public SyncableItemMetaData(JSONObject jsonObject)
        {
            FromJSONObject(jsonObject);
        }

        #endregion /Constructors

        #region Properties

        /// <summary>
        /// The <see cref="SyncableItem"/>'s data type.
        /// </summary>
        public DataType DataType { get; private set; }

        /// <summary>
        /// The <see cref="SyncableItem"/>'s persistence type. Used for conflict resolution.
        /// </summary>
        public PersistenceType PersistenceType { get; private set; }

        /// <summary>
        /// The time that the <see cref="SyncableItem"/> was last updated.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        #endregion /Properties

        #region Public methods

        /// <summary>
        /// Updates the timestamp.
        /// </summary>
        public void UpdateDateTime()
        {
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if this meta data is identical to another.
        /// </summary>
        /// <param name="other">The other meta data to check against.</param>
        /// <returns>Returns <c>true</c> if the meta data is identical, <c>false</c> if there are any differences.</returns>
        public bool Equals(SyncableItemMetaData other)
        {
            if (other == null)
            {
                return false;
            }

            var dataTypeSame = Equals(DataType, other.DataType);
            var persistenceTypeSame = Equals(PersistenceType, other.PersistenceType);
            if (PersistenceType == PersistenceType.Latest)
            {
                return Timestamp.Equals(other.Timestamp) && dataTypeSame && persistenceTypeSame;
            }

            return dataTypeSame && persistenceTypeSame;
        }

        /// <summary>
        /// Returns the meta data as a formatted <see cref="string"/>.
        /// </summary>
        /// <returns>The meta data as a formatted <see cref="string"/>.</returns>
        public override string ToString()
        {
            if (PersistenceType == PersistenceType.Latest)
            {
                return string.Format(
                    "DataType: {0}, PersistenceType: {1}, TimeStamp: {2}",
                    DataType,
                    PersistenceType,
                    Timestamp);
            }

            return string.Format(
                "DataType: {0}, PersistenceType: {1}",
                DataType,
                PersistenceType);
        }

        /// <summary>
        /// Reconstructs the meta data from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the meta data.</param>
        public void FromJSONObject(JSONObject jsonObject)
        {
            var dataTypeAlias = CloudOnceUtils.GetAlias(typeof(SyncableItemMetaData).Name, jsonObject, aliasDataType, oldAliasDataType);
            var persistenceTypeAlias = CloudOnceUtils.GetAlias(typeof(SyncableItemMetaData).Name, jsonObject, aliasPersistenceType, oldAliasPersistenceType);

            if (!string.IsNullOrEmpty(jsonObject[dataTypeAlias].String))
            {
                DataType = (DataType)Enum.Parse(typeof(DataType), jsonObject[dataTypeAlias].String);
            }
            else
            {
                DataType = (DataType)(short)jsonObject[dataTypeAlias].F;
            }

            if (!string.IsNullOrEmpty(jsonObject[persistenceTypeAlias].String))
            {
                PersistenceType = (PersistenceType)Enum.Parse(typeof(PersistenceType), jsonObject[persistenceTypeAlias].String);
            }
            else
            {
                PersistenceType = (PersistenceType)(short)jsonObject[persistenceTypeAlias].F;
            }

            if (jsonObject.HasFields(aliasTimestamp))
            {
                Timestamp = DateTime.FromBinary(Convert.ToInt64(jsonObject[aliasTimestamp].String));
            }
            else if (jsonObject.HasFields(oldAliasTimestamp))
            {
                Timestamp = DateTime.FromBinary(Convert.ToInt64(jsonObject[oldAliasTimestamp].String));
            }
        }

        /// <summary>
        /// Converts the meta data into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the meta data</returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);

            jsonObject.AddField(aliasDataType, (short)DataType);
            jsonObject.AddField(aliasPersistenceType, (short)PersistenceType);
            if (PersistenceType == PersistenceType.Latest)
            {
                jsonObject.AddField(aliasTimestamp, Timestamp.ToBinary().ToString(CultureInfo.InvariantCulture));
            }

            return jsonObject;
        }

        #endregion /Public methods
    }
}
