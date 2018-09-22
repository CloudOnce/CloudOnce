// <copyright file="CloudVariableData.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Data
{
    using System;
    using System.Runtime.Serialization;
    using UnityEngine;

    public enum CloudVariableType
    {
        Int,
        CurrencyInt,
        Float,
        CurrencyFloat,
        Bool,
        String,
        Double,
        UInt,
        Long,
        DateTime,
        Decimal
    }

    /// <summary>
    /// Stores info about cloud variables declared in the CloudOnce Editor
    /// </summary>
    [Serializable]
    public class CloudVariableData : IJsonSerializable
    {
        private const string keyName = "Key";
        private const string typeName = "Type";
        private const string defaultValueName = "DefaultValue";
        private const string persistenceTypeName = "PersistenceType";
        private const string allowNegativeName = "AllowNegative";

        [SerializeField] private string key;
        [SerializeField] private CloudVariableType type;
        [SerializeField] private string defaultValueString;
        [SerializeField] private int persistenceType;
        [SerializeField] private bool allowNegative;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudVariableData"/> class.
        /// Reconstructs the <see cref="CloudVariableData"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject">The <see cref="JSONObject"/> containing the <see cref="CloudVariableData"/></param>
        public CloudVariableData(JSONObject jsonObject)
        {
            FromJSONObject(jsonObject);
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="CloudVariableData"/>.
        /// </summary>
        public CloudVariableData()
        {
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public CloudVariableType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string DefaultValueString
        {
            get { return defaultValueString; }
            set { defaultValueString = value; }
        }

        public PersistenceType PersistenceType
        {
            get { return (PersistenceType)persistenceType; }
            set { persistenceType = (int)value; }
        }

        public bool AllowNegative
        {
            get { return allowNegative; }
            set { allowNegative = value; }
        }

        /// <summary>
        /// Converts the <see cref="CloudVariableData"/> into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the <see cref="CloudVariableData"/>.</returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);

            jsonObject.AddField(keyName, Key);
            jsonObject.AddField(typeName, Enum.Format(typeof(CloudVariableType), Type, "D"));
            jsonObject.AddField(defaultValueName, DefaultValueString);
            jsonObject.AddField(persistenceTypeName, Enum.Format(typeof(PersistenceType), PersistenceType, "D"));
            jsonObject.AddField(allowNegativeName, AllowNegative);

            return jsonObject;
        }

        /// <summary>
        /// Check if cloud variables match.
        /// </summary>
        /// <param name="otherData">Other <see cref="CloudVariableData"/>.</param>
        /// <returns>If cloud variables match.</returns>
        public bool EqualsCloudVariable(CloudVariableData otherData)
        {
            return Key == otherData.Key
                && Type == otherData.Type
                && DefaultValueString == otherData.DefaultValueString
                && PersistenceType == otherData.PersistenceType
                && AllowNegative == otherData.AllowNegative;
        }

        /// <summary>
        /// Reconstructs the <see cref="CloudVariableData"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the <see cref="CloudVariableData"/>.</param>
        private void FromJSONObject(JSONObject jsonObject)
        {
            if (!jsonObject.HasFields(keyName, typeName, defaultValueName, persistenceTypeName, allowNegativeName))
            {
                throw new SerializationException("JSONObject missing fields, cannot deserialize to " + typeof(CloudVariableData).Name);
            }

            Key = jsonObject[keyName].String;
            Type = (CloudVariableType)Enum.Parse(typeof(CloudVariableType), jsonObject[typeName].String);
            DefaultValueString = jsonObject[defaultValueName].String;
            PersistenceType = (PersistenceType)Enum.Parse(typeof(PersistenceType), jsonObject[persistenceTypeName].String);
            AllowNegative = jsonObject[allowNegativeName].B;
        }
    }
}
#endif
