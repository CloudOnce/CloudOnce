// <copyright file="PlatformIdData.cs" company="Jan Ivar Z. Carlsen & Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen & Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace Trollpants.CloudOnce.Internal.Editor.Data
{
    using System;
    using System.Runtime.Serialization;
    using UnityEngine;

    /// <summary>
    /// Stores achievement/leaderboard IDs for each supported platform as well as an internal ID.
    /// </summary>
    [Serializable]
    public class PlatformIdData : IJsonSerializeable
    {
        private const string c_keyInternalID = "InternalID";
        private const string c_keyAppleID = "AppleID";
        private const string c_keyGoogleID = "GoogleID";
        private const string c_keyAmazonID = "AmazonID";

        [SerializeField] private string internalId;
        [SerializeField] private string appleId;
        [SerializeField] private string googleId;
        [SerializeField] private string amazonId;

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
            AmazonId = strings[3];
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

        /// <summary>
        /// The Amazon GameCircle ID for an achievement or leaderboard.
        /// </summary>
        public string AmazonId
        {
            get { return amazonId; }
            set { amazonId = value; }
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
                && GoogleId == otherData.GoogleId
                && AmazonId == otherData.AmazonId;
        }

        /// <summary>
        /// Converts the <see cref="PlatformIdData"/> into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the <see cref="PlatformIdData"/>.</returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);

            jsonObject.AddField(c_keyInternalID, InternalId);
            jsonObject.AddField(c_keyAppleID, AppleId);
            jsonObject.AddField(c_keyGoogleID, GoogleId);
            jsonObject.AddField(c_keyAmazonID, AmazonId);

            return jsonObject;
        }

        /// <summary>
        /// Reconstructs the <see cref="PlatformIdData"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the <see cref="PlatformIdData"/>.</param>
        private void FromJSONObject(JSONObject jsonObject)
        {
            if (!jsonObject.HasFields(c_keyInternalID, c_keyAppleID, c_keyGoogleID, c_keyAmazonID))
            {
                throw new SerializationException("JSONObject missing fields, cannot deserialize to " + typeof(PlatformIdData).Name);
            }

            InternalId = jsonObject[c_keyInternalID].String;
            AppleId = jsonObject[c_keyAppleID].String;
            GoogleId = jsonObject[c_keyGoogleID].String;
            AmazonId = jsonObject[c_keyAmazonID].String;
        }
    }
}
#endif
