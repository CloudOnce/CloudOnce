// <copyright file="CloudConfig.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using UnityEngine;

    /// <summary>
    /// Used in <see cref="CloudConfig"/> for serialization
    /// </summary>
    public enum DataTypeID
    {
        Achievement,
        Leaderboard,
        AppleSupport,
        GoogleSupport,
        AndroidPlatform,
        GoogleAppID,
        GoogleSetupRun,
        DebugMode,
        SettingsLocation
    }

    public enum SettingsLocation
    {
        ProjectSettings,
        Assets
    }

    /// <summary>
    /// Data-class for storing CloudOnce configuration.
    /// </summary>
    [Serializable]
    public class CloudConfig : ScriptableObject, IJsonSerializable
    {
        private const string achievementIDsName = "AchievementIDs";
        private const string leaderboardIDsName = "LeaderboardIDs";
        private const string cloudVariablesName = "CloudVariables";
        private const string appleSupportedName = "AppleSupported";
        private const string googleSupportedName = "GoogleSupported";
        private const string googleAppIDName = "GoogleAppID";
        private const string googleSetupRunName = "GoogleSetupRun";
        private const string debugModeEnabledName = "DebugModeEnabled";
        private const string versionName = "Version";
        private const string settingsLocationName = "SettingsLocation";

        [SerializeField] private List<PlatformIdData> achievementIDs;
        [SerializeField] private List<PlatformIdData> leaderboardIDs;
        [SerializeField] private List<CloudVariableData> cloudVariables;
        [SerializeField] private bool appleSupported;
        [SerializeField] private bool googleSupported;
        [SerializeField] private string googleAppID;
        [SerializeField] private bool googleSetupRun;
        [SerializeField] private bool debugModeEnabled;
        [SerializeField] private string version;
        [SerializeField] private SettingsLocation settingsLocation;

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

        public SettingsLocation SettingsLocation
        {
            get { return settingsLocation; }
            set { settingsLocation = value; }
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
                && GoogleAppID == otherConfig.GoogleAppID
                && GoogleSetupRun == otherConfig.GoogleSetupRun
                && DebugModeEnabled == otherConfig.DebugModeEnabled
                && SettingsLocation == otherConfig.SettingsLocation;
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

            jsonObject.AddField(achievementIDsName, JsonHelper.ToJsonObject(AchievementIDs));
            jsonObject.AddField(leaderboardIDsName, JsonHelper.ToJsonObject(LeaderboardIDs));
            jsonObject.AddField(cloudVariablesName, JsonHelper.ToJsonObject(CloudVariables));
            jsonObject.AddField(appleSupportedName, AppleSupported);
            jsonObject.AddField(googleSupportedName, GoogleSupported);
            jsonObject.AddField(googleAppIDName, GoogleAppID);
            jsonObject.AddField(googleSetupRunName, GoogleSetupRun);
            jsonObject.AddField(debugModeEnabledName, DebugModeEnabled);
            jsonObject.AddField(versionName, Version = PluginVersion.VersionString);
            jsonObject.AddField(settingsLocationName, Enum.Format(typeof(SettingsLocation), SettingsLocation, "D"));

            return jsonObject;
        }

        #endregion /Public methods

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
            if (!jsonObject.HasFields(achievementIDsName, leaderboardIDsName, cloudVariablesName, appleSupportedName, googleSupportedName,
                googleAppIDName, googleSetupRunName, debugModeEnabledName, versionName))
            {
                throw new SerializationException("JSONObject missing fields, cannot deserialize to " + typeof(CloudConfig).Name);
            }

            AchievementIDs = EditorJsonHelper.Convert<List<PlatformIdData>>(jsonObject[achievementIDsName]);
            LeaderboardIDs = EditorJsonHelper.Convert<List<PlatformIdData>>(jsonObject[leaderboardIDsName]);
            CloudVariables = EditorJsonHelper.Convert<List<CloudVariableData>>(jsonObject[cloudVariablesName]);
            AppleSupported = jsonObject[appleSupportedName].B;
            GoogleSupported = jsonObject[googleSupportedName].B;
            GoogleAppID = jsonObject[googleAppIDName].String;
            GoogleSetupRun = jsonObject[googleSetupRunName].B;
            DebugModeEnabled = jsonObject[debugModeEnabledName].B;
            Version = jsonObject[versionName].String;

            if (jsonObject.HasFields(settingsLocationName))
            {
                SettingsLocation = (SettingsLocation)Enum.Parse(typeof(SettingsLocation), jsonObject[settingsLocationName].String);
            }
        }
    }
}
#endif
