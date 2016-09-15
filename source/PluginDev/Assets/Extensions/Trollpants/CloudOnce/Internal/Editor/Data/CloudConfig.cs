// <copyright file="CloudConfig.cs" company="Jan Ivar Z. Carlsen & Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen & Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace Trollpants.CloudOnce.Internal.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using UnityEngine;

    /// <summary>
    /// For differentiating between Google Play and Amazon on Android
    /// </summary>
    public enum AndroidBuildPlatform
    {
        Amazon,
        GooglePlay
    }

    /// <summary>
    /// Used in <see cref="CloudConfig"/> for serialization
    /// </summary>
    public enum DataTypeID
    {
        Achievement,
        Leaderboard,
        AppleSupport,
        GoogleSupport,
        AmazonSupport,
        AndroidPlatform,
        GoogleAppID,
        GoogleSetupRun,
        DebugMode
    }

    /// <summary>
    /// Data-class for storing CloudOnce configuration.
    /// </summary>
    [Serializable]
    public class CloudConfig : ScriptableObject, IJsonSerializeable
    {
        private const string c_keyAchievementIDs = "AchievementIDs";
        private const string c_keyLeaderboardIDs = "LeaderboardIDs";
        private const string c_keyCloudVariables = "CloudVariables";
        private const string c_keyAppleSupported = "AppleSupported";
        private const string c_keyGoogleSupported = "GoogleSupported";
        private const string c_keyAmazonSupported = "AmazonSupported";
        private const string c_keyAndroidPlatform = "AndroidPlatform";
        private const string c_keyGoogleAppID = "GoogleAppID";
        private const string c_keyGoogleSetupRun = "GoogleSetupRun";
        private const string c_keyDebugModeEnabled = "DebugModeEnabled";
        private const string c_keyVersion = "Version";
        private const string c_apiKey = "APIKey";

        [SerializeField] private List<PlatformIdData> achievementIDs;
        [SerializeField] private List<PlatformIdData> leaderboardIDs;
        [SerializeField] private List<CloudVariableData> cloudVariables;
        [SerializeField] private bool appleSupported;
        [SerializeField] private bool googleSupported;
        [SerializeField] private bool amazonSupported;
        [SerializeField] private AndroidBuildPlatform androidPlatform;
        [SerializeField] private string googleAppID;
        [SerializeField] private bool googleSetupRun;
        [SerializeField] private bool debugModeEnabled;
        [SerializeField] private string version;
        [SerializeField] private string apiKey;

        #region Constructor & properties

        public List<PlatformIdData> AchievementIDs
        {
            get { return achievementIDs; }
            private set { achievementIDs = value; }
        }

        public List<PlatformIdData> LeaderboardIDs
        {
            get { return leaderboardIDs; }
            private set { leaderboardIDs = value; }
        }

        public List<CloudVariableData> CloudVariables
        {
            get { return cloudVariables; }
            private set { cloudVariables = value; }
        }

        public bool AppleSupported
        {
            get { return appleSupported; }
            set { appleSupported = value; }
        }

        public bool GoogleSupported
        {
            get { return googleSupported; }
            set { googleSupported = value; }
        }

        public bool AmazonSupported
        {
            get { return amazonSupported; }
            set { amazonSupported = value; }
        }

        public AndroidBuildPlatform AndroidPlatform
        {
            get { return androidPlatform; }
            set { androidPlatform = value; }
        }

        public string GoogleAppID
        {
            get { return googleAppID; }
            set { googleAppID = value; }
        }

        public bool GoogleSetupRun
        {
            get { return googleSetupRun; }
            set { googleSetupRun = value; }
        }

        public bool DebugModeEnabled
        {
            get { return debugModeEnabled; }
            set { debugModeEnabled = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }

        #endregion /Constructor & properties

        #region Public methods

        public void ImportSettingsFromJSON(JSONObject jsonObject)
        {
            FromJSONObject(jsonObject);
        }

        /// <summary>
        /// Checks if settings match.
        /// </summary>
        /// <param name="otherConfig">Other <see cref="CloudConfig"/>.</param>
        /// <returns>If settings match.</returns>
        public bool EqualsSettings(CloudConfig otherConfig)
        {
            return AppleSupported == otherConfig.AppleSupported
                && GoogleSupported == otherConfig.GoogleSupported
                && AmazonSupported == otherConfig.AmazonSupported
                && AndroidPlatform == otherConfig.AndroidPlatform
                && GoogleAppID == otherConfig.GoogleAppID
                && GoogleSetupRun == otherConfig.GoogleSetupRun
                && DebugModeEnabled == otherConfig.DebugModeEnabled
                && ApiKey == otherConfig.ApiKey;
        }

        /// <summary>
        /// Checks if Cloud IDs match.
        /// </summary>
        /// <param name="otherConfig">Other <see cref="CloudConfig"/>.</param>
        /// <returns>If Cloud IDs match.</returns>
        public bool EqualsCloudIDs(CloudConfig otherConfig)
        {
            return AchievementIDs.Count == otherConfig.AchievementIDs.Count
                && LeaderboardIDs.Count == otherConfig.LeaderboardIDs.Count
                && !AchievementIDs.Where((t, i) => !t.EqualsIDs(otherConfig.AchievementIDs[i])).Any()
                && !LeaderboardIDs.Where((t, i) => !t.EqualsIDs(otherConfig.LeaderboardIDs[i])).Any();
        }

        /// <summary>
        /// Checks if Cloud Variables match.
        /// </summary>
        /// <param name="otherConfig">Other <see cref="CloudConfig"/>.</param>
        /// <returns>If Cloud Variables match.</returns>
        public bool EqualsCloudVariables(CloudConfig otherConfig)
        {
            return CloudVariables.Count == otherConfig.CloudVariables.Count
                && !CloudVariables.Where((t, i) => !t.EqualsCloudVariable(otherConfig.CloudVariables[i])).Any();
        }

        /// <summary>
        /// Converts the <see cref="CloudConfig"/> into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing the <see cref="CloudConfig"/>.</returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);

            jsonObject.AddField(c_keyAchievementIDs, JsonHelper.ToJsonObject(AchievementIDs));
            jsonObject.AddField(c_keyLeaderboardIDs, JsonHelper.ToJsonObject(LeaderboardIDs));
            jsonObject.AddField(c_keyCloudVariables, JsonHelper.ToJsonObject(CloudVariables));
            jsonObject.AddField(c_keyAppleSupported, AppleSupported);
            jsonObject.AddField(c_keyGoogleSupported, GoogleSupported);
            jsonObject.AddField(c_keyAmazonSupported, AmazonSupported);
            jsonObject.AddField(c_keyAndroidPlatform, Enum.Format(typeof(AndroidBuildPlatform), AndroidPlatform, "D"));
            jsonObject.AddField(c_keyGoogleAppID, GoogleAppID);
            jsonObject.AddField(c_keyGoogleSetupRun, GoogleSetupRun);
            jsonObject.AddField(c_keyDebugModeEnabled, DebugModeEnabled);
            jsonObject.AddField(c_keyVersion, Version = PluginVersion.VersionString);
            jsonObject.AddField(c_apiKey, ApiKey);

            return jsonObject;
        }

        #endregion /Public methods

        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;

            if (AchievementIDs == null)
            {
                AchievementIDs = new List<PlatformIdData>();
            }

            if (LeaderboardIDs == null)
            {
                LeaderboardIDs = new List<PlatformIdData>();
            }

            if (CloudVariables == null)
            {
                CloudVariables = new List<CloudVariableData>();
            }

            if (Version == null)
            {
                Version = string.Empty;
            }
        }

        /// <summary>
        /// Reconstructs the <see cref="CloudConfig"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing the <see cref="CloudConfig"/>.</param>
        private void FromJSONObject(JSONObject jsonObject)
        {
            if (!jsonObject.HasFields(c_keyAchievementIDs, c_keyLeaderboardIDs, c_keyCloudVariables, c_keyAppleSupported, c_keyGoogleSupported,
                c_keyAmazonSupported, c_keyAndroidPlatform, c_keyGoogleAppID, c_keyGoogleSetupRun, c_keyDebugModeEnabled, c_keyVersion))
            {
                throw new SerializationException("JSONObject missing fields, cannot deserialize to " + typeof(CloudConfig).Name);
            }

            AchievementIDs = EditorJsonHelper.Convert<List<PlatformIdData>>(jsonObject[c_keyAchievementIDs]);
            LeaderboardIDs = EditorJsonHelper.Convert<List<PlatformIdData>>(jsonObject[c_keyLeaderboardIDs]);
            CloudVariables = EditorJsonHelper.Convert<List<CloudVariableData>>(jsonObject[c_keyCloudVariables]);
            AppleSupported = jsonObject[c_keyAppleSupported].B;
            GoogleSupported = jsonObject[c_keyGoogleSupported].B;
            AmazonSupported = jsonObject[c_keyAmazonSupported].B;
            AndroidPlatform = (AndroidBuildPlatform)Enum.Parse(typeof(AndroidBuildPlatform), jsonObject[c_keyAndroidPlatform].String);
            GoogleAppID = jsonObject[c_keyGoogleAppID].String;
            GoogleSetupRun = jsonObject[c_keyGoogleSetupRun].B;
            DebugModeEnabled = jsonObject[c_keyDebugModeEnabled].B;
            Version = jsonObject[c_keyVersion].String;
            if (jsonObject.HasFields(c_apiKey))
            {
                ApiKey = jsonObject[c_apiKey].String;
            }
        }
    }
}
#endif
