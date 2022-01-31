// <copyright file="PlatformIdData.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Data
{
    using System;
    using System.Runtime.Serialization;
    using UnityEngine;

    /// <summary>
    /// Stores achievement/leaderboard IDs for each supported platform as well as an internal ID.
    /// </summary>
    [Serializable]
    public class PlatformIdData : IJsonSerializable
    {
        private const string internalIdName = "InternalID";
        private const string appleIdName = "AppleID";
        private const string googleIdName = "GoogleID";

        [SerializeField] private string internalId;
        [SerializeField] private string appleId;
        [SerializeField] private string googleId;

        #region Constructor & properties

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformIdData"/> class.
        /// Reconstructs the <see cref="PlatformIdData"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject">The <see cref="JSONObject"/> containing the <see cref="PlatformIdData"/></param>
        public PlatformIdData(JSONObject jsonObject)
        {
            FromJSONObject(jsonObject);
        }

        /// <summary>
        /// Reconstructs the <see cref="PlatformIdData"/> from a <see cref="string"/>.
        /// This is now only used to upgrade old settings to use new serialization.
        /// </summary>
        /// <param name="fromString"> A serialized <see cref="string"/> containing the <see cref="PlatformIdData"/>.</param>
        public PlatformIdData(string fromString)
        {
            var strings = fromString.Split(',');
            InternalId = strings[0];
            AppleId = strings[1];
            GoogleId = strings[2];
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="PlatformIdData"/>.
        /// </summary>
        public PlatformIdData()
        {
        }

        /// <summary>
        /// The internal ID for an achievement or leaderboard.
        /// </summary>
        public string InternalId
        {
            get { return internalId; }
            set { internalId = value; }
        }

        /// <summary>
        /// The Apple Game Center ID for an achievement or leaderboard.
        /// </summary>
        public string AppleId
        {
            get { return appleId; }
            set { appleId = value; }
        }

        /// <summary>
        /// The Google Play Games ID for an achievement or leaderboard.
        /// </summary>
        public string GoogleId
        {
            get { return googleId; }
            set { googleId = value; }
        }

        #endregion /Constructor & properties

        /// <summary>
        /// Check if IDs match.
        /// </summary>
        /// <param name="otherData">Other <see cref="PlatformIdData"/>.</param>
        /// <returns>If IDs match.</returns>
        public bool EqualsIDs(PlatformIdData otherData)
        {
            return InternalId == otherData.InternalId
                && AppleId == otherData.AppleId
                && GoogleId == otherData.GoogleId;
        }

        /// <summary>
        /// Converts the <see cref="PlatformIdData"/> into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the <see cref="PlatformIdData"/>.</returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);

            jsonObject.AddField(internalIdName, InternalId);
            jsonObject.AddField(appleIdName, AppleId);
            jsonObject.AddField(googleIdName, GoogleId);

            return jsonObject;
        }

        /// <summary>
        /// Reconstructs the <see cref="PlatformIdData"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the <see cref="PlatformIdData"/>.</param>
        private void FromJSONObject(JSONObject jsonObject)
        {
            if (!jsonObject.HasFields(internalIdName, appleIdName, googleIdName))
            {
                throw new SerializationException("JSONObject missing fields, cannot deserialize to " + typeof(PlatformIdData).Name);
            }

            InternalId = jsonObject[internalIdName].String;
            AppleId = jsonObject[appleIdName].String;
            GoogleId = jsonObject[googleIdName].String;
        }
    }
}
#endif
