// <copyright file="SyncableItem.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using System.Globalization;
    using Utils;

    /// <summary>
    /// Data-class for regular cloud preferences. Takes care of serialization and deserialization of the data.
    /// The value can be reset to default or altered.
    /// </summary>
    public class SyncableItem : IEquatable<SyncableItem>, IJsonConvertible
    {
        #region Fields & properties

        private const string oldAliasValueString = "_vs";
        private const string oldAliasMetadata = "_md";

        // These two string are used in serialization
        private const string aliasValueString = "v";
        private const string aliasMetadata = "m";

        // Backing field
        private string valueString;

        #endregion /Fields & properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncableItem"/> class.
        /// Reconstructs the item from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="itemData">The <see cref="JSONObject"/> containing the <see cref="SyncableItem"/></param>
        public SyncableItem(JSONObject itemData)
        {
            FromJSONObject(itemData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncableItem"/> class.
        /// </summary>
        /// <param name="value">The value in <see cref="string"/> form</param>
        /// <param name="metadata">A collection of info about the <see cref="SyncableItem"/></param>
        public SyncableItem(string value, SyncableItemMetaData metadata)
        {
            valueString = value;
            Metadata = metadata;
        }

        #endregion /Constructors

        #region Properties

        /// <summary>
        /// The items meta data. Used for serialization and conflict resolution.
        /// </summary>
        public SyncableItemMetaData Metadata { get; private set; }

        /// <summary>
        /// The item's value as a <see cref="string"/>.
        /// </summary>
        public string ValueString
        {
            get
            {
                return valueString ?? (valueString = string.Empty);
            }

            set
            {
                valueString = value;
                if (Metadata.PersistenceType == PersistenceType.Latest)
                {
                    Metadata.UpdateDateTime();
                }
            }
        }

        #endregion /Properties

        #region Public methods

        /// <summary>
        /// Checks if this item is identical to another.
        /// </summary>
        /// <param name="other">The other item to check against.</param>
        /// <returns>Returns <c>true</c> if the items are identical, <c>false</c> if there are any differences.</returns>
        public bool Equals(SyncableItem other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(valueString, other.valueString) && Metadata.Equals(other.Metadata);
        }

        /// <summary>
        /// Converts the item into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the item.</returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);
            jsonObject.AddField(aliasValueString, ValueString.ToString(CultureInfo.InvariantCulture));
            jsonObject.AddField(aliasMetadata, Metadata.ToJSONObject());

            return jsonObject;
        }

        /// <summary>
        /// Reconstructs the item from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the item data.</param>
        public void FromJSONObject(JSONObject jsonObject)
        {
            var valueStringAlias = CloudOnceUtils.GetAlias(typeof(SyncableItem).Name, jsonObject, aliasValueString, oldAliasValueString);
            var metaDataAlias = CloudOnceUtils.GetAlias(typeof(SyncableItem).Name, jsonObject, aliasMetadata, oldAliasMetadata);

            valueString = jsonObject[valueStringAlias].String;
            Metadata = new SyncableItemMetaData(jsonObject[metaDataAlias]);
        }

        /// <summary>
        /// Returns the meta data as a formatted <see cref="string"/>.
        /// </summary>
        /// <returns>The meta data as a formatted <see cref="string"/>.</returns>
        public override string ToString()
        {
            return string.Format("Value: {0}" + Environment.NewLine + " Meta Data: {1}", ValueString, Metadata);
        }

        #endregion /Public methods
    }
}
