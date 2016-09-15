// <copyright file="SerializationUtils.cs" company="Jan Ivar Z. Carlsen & Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen & Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace Trollpants.CloudOnce.Internal.Editor.Utils
{
    using System;
    using System.IO;
    using System.Text;
    using Data;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Serialization utilities used by the CloudOnce editor.
    /// </summary>
    public static class SerializationUtils
    {
        #region Fields & properties

        private const string c_achievementsPath = CloudOncePaths.Data + "/Achievements.cs";
        private const string c_leaderboardsPath = CloudOncePaths.Data + "/Leaderboards.cs";
        private const string c_cloudVariablesPath = CloudOncePaths.Data + "/CloudVariables.cs";
        private const string c_cloudIDsPath = CloudOncePaths.Data + "/CloudIDs.cs";

        private const string c_achievementsTemplatePath = CloudOncePaths.Templates + "/AchievementsTemplate.cs.txt";
        private const string c_leaderboardsTemplatePath = CloudOncePaths.Templates + "/LeaderboardsTemplate.cs.txt";
        private const string c_cloudVariablesTemplatePath = CloudOncePaths.Templates + "/CloudVariablesTemplate.cs.txt";
        private const string c_cloudIDsTemplatePath = CloudOncePaths.Templates + "/CloudIDsTemplate.cs.txt";

        private const string c_achievementsPropertyTemplatePath = CloudOncePaths.Templates + "/AchievementsPropertyTemplate.cs.txt";
        private const string c_leaderboardsPropertyTemplatePath = CloudOncePaths.Templates + "/LeaderboardsPropertyTemplate.cs.txt";
        private const string c_cloudVariablesPropertyTemplatePath = CloudOncePaths.Templates + "/CloudVariablesPropertyTemplate.cs.txt";
        private const string c_cloudIDsPropertyTemplatePath = CloudOncePaths.Templates + "/CloudIDsPropertyTemplate.cs.txt";

        private const string c_defaultVariableTemplate = "        private static readonly VAR_TYPE FIELDNAME = new VAR_TYPE(\"VAR_ID\", PERSISTENCE_TYPE, VAR_DEFAULT_VALUE);";
        private const string c_currencyVariableTemplate = "        private static readonly VAR_TYPE FIELDNAME = new VAR_TYPE(\"VAR_ID\", VAR_DEFAULT_VALUE, ALLOW_NEGATIVE);";
        private const string c_dateTimeVariableTemplate = "        private static readonly VAR_TYPE FIELDNAME = new VAR_TYPE(\"VAR_ID\", PERSISTENCE_TYPE);";

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Loads the cloud configuration.
        /// </summary>
        /// <returns>Imported CloudConfig instance. Returns an empty instance if cloud config file doesn't exist.</returns>
        public static CloudConfig LoadCloudConfig()
        {
            var cloudConfig = ScriptableObject.CreateInstance<CloudConfig>();
            var settingsFile = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.Settings);
            if (File.Exists(settingsFile))
            {
                var sr = new StreamReader(settingsFile);
                var body = sr.ReadToEnd();
                sr.Close();
                cloudConfig.ImportSettingsFromJSON(new JSONObject(body));
            }

            return cloudConfig;
        }

        /// <summary>
        /// Serializes a CloudConfig instance to a text file and triggers generation of the CloudIDs.cs
        /// script that contains properties for each achievement- and leaderboard ID.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance that will be serialized.</param>
        /// <param name="onlySettingsFile">If you only want to save the settings file and not generate CloudIDs, Achievements and Leaderboards scripts.</param>
        public static void SerializeCloudConfig(CloudConfig cloudConfig, bool onlySettingsFile = false)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.Settings)))
            {
                writer.Write(cloudConfig.ToJSONObject().ToString(true));
            }

            if (!onlySettingsFile)
            {
                CreateCloudIDsScript(cloudConfig);
                CreateAchievementsScript(cloudConfig);
                CreateLeaderboardsScript(cloudConfig);
                CreateCloudVariablesScript(cloudConfig);
            }
        }

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Generates a static script that contains properties for getting achievement- and leaderboard IDs.
        /// If the CloudIDs.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateCloudIDsScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(c_cloudIDsPath)))
            {
                string newCloudIDsScript;
                string idPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_cloudIDsTemplatePath)))
                {
                    newCloudIDsScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_cloudIDsPropertyTemplatePath)))
                {
                    idPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                for (var i = 0; i < cloudConfig.AchievementIDs.Count; i++)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("INTERNALID", cloudConfig.AchievementIDs[i].InternalId);
                    propertyString = propertyString.Replace("AMAZONID", cloudConfig.AchievementIDs[i].AmazonId);
                    propertyString = propertyString.Replace("GOOGLEID", cloudConfig.AchievementIDs[i].GoogleId);
                    propertyString = propertyString.Replace("APPLEID", cloudConfig.AchievementIDs[i].AppleId);
                    builder.AppendLine(propertyString);
                    if (i != cloudConfig.AchievementIDs.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                newCloudIDsScript = newCloudIDsScript.Replace("// ACHIEVEMENT_ID_PROPERTIES", builder.ToString());

                builder = new StringBuilder();
                for (var i = 0; i < cloudConfig.LeaderboardIDs.Count; i++)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("INTERNALID", cloudConfig.LeaderboardIDs[i].InternalId);
                    propertyString = propertyString.Replace("AMAZONID", cloudConfig.LeaderboardIDs[i].AmazonId);
                    propertyString = propertyString.Replace("GOOGLEID", cloudConfig.LeaderboardIDs[i].GoogleId);
                    propertyString = propertyString.Replace("APPLEID", cloudConfig.LeaderboardIDs[i].AppleId);
                    builder.AppendLine(propertyString);
                    if (i != cloudConfig.LeaderboardIDs.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                newCloudIDsScript = newCloudIDsScript.Replace("// LEADERBOARD_ID_PROPERTIES", builder.ToString());

                writer.Write(newCloudIDsScript);
            }

            try
            {
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(c_cloudIDsPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(c_cloudIDsPath));
            }
        }

        /// <summary>
        /// Generates a static script that provides acces to all achievements created in the CloudOnce Editor.
        /// If the Achievements.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateAchievementsScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(c_achievementsPath)))
            {
                string newAchievementsScript;
                string idPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_achievementsTemplatePath)))
                {
                    newAchievementsScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_achievementsPropertyTemplatePath)))
                {
                    idPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                for (var i = 0; i < cloudConfig.AchievementIDs.Count; i++)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("FIELDNAME", "s_" + FirstLetterToLowerCase(cloudConfig.AchievementIDs[i].InternalId));
                    propertyString = propertyString.Replace("INTERNALID", cloudConfig.AchievementIDs[i].InternalId);
                    propertyString = propertyString.Replace("APPLEID", cloudConfig.AchievementIDs[i].AppleId);
                    propertyString = propertyString.Replace("GOOGLEID", cloudConfig.AchievementIDs[i].GoogleId);
                    propertyString = propertyString.Replace("AMAZONID", cloudConfig.AchievementIDs[i].AmazonId);
                    builder.AppendLine(propertyString);
                    if (i != cloudConfig.AchievementIDs.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                newAchievementsScript = newAchievementsScript.Replace("// ACHIEVEMENT_IDS", builder.ToString());

                writer.Write(newAchievementsScript);
            }

            try
            {
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(c_achievementsPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(c_achievementsPath));
            }
        }

        /// <summary>
        /// Generates a static script that provides acces to all leaderboards created in the CloudOnce Editor.
        /// If the Leaderboards.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateLeaderboardsScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(c_leaderboardsPath)))
            {
                string newLeaderboardsScript;
                string idPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_leaderboardsTemplatePath)))
                {
                    newLeaderboardsScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_leaderboardsPropertyTemplatePath)))
                {
                    idPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                for (var i = 0; i < cloudConfig.LeaderboardIDs.Count; i++)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("FIELDNAME", "s_" + FirstLetterToLowerCase(cloudConfig.LeaderboardIDs[i].InternalId));
                    propertyString = propertyString.Replace("INTERNALID", cloudConfig.LeaderboardIDs[i].InternalId);
                    propertyString = propertyString.Replace("APPLEID", cloudConfig.LeaderboardIDs[i].AppleId);
                    propertyString = propertyString.Replace("GOOGLEID", cloudConfig.LeaderboardIDs[i].GoogleId);
                    propertyString = propertyString.Replace("AMAZONID", cloudConfig.LeaderboardIDs[i].AmazonId);
                    builder.AppendLine(propertyString);
                    if (i != cloudConfig.LeaderboardIDs.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                newLeaderboardsScript = newLeaderboardsScript.Replace("// LEADERBOARD_IDS", builder.ToString());

                writer.Write(newLeaderboardsScript);
            }

            try
            {
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(c_leaderboardsPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(c_leaderboardsPath));
            }
        }

        /// <summary>
        /// Generates a static script that provides acces to all cloud variables created in the CloudOnce Editor.
        /// If the CloudVariables.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateCloudVariablesScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(c_cloudVariablesPath)))
            {
                string newCloudVariablesScript;
                string varPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_cloudVariablesTemplatePath)))
                {
                    newCloudVariablesScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(c_cloudVariablesPropertyTemplatePath)))
                {
                    varPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                for (var i = 0; i < cloudConfig.CloudVariables.Count; i++)
                {
                    string fieldString;
                    switch (cloudConfig.CloudVariables[i].Type)
                    {
                        case CloudVariableType.Int:
                        case CloudVariableType.Float:
                        case CloudVariableType.Bool:
                        case CloudVariableType.String:
                        case CloudVariableType.Double:
                        case CloudVariableType.UInt:
                        case CloudVariableType.Long:
                        case CloudVariableType.Decimal:
                            fieldString = c_defaultVariableTemplate;
                            fieldString = fieldString.Replace("PERSISTENCE_TYPE", GetPersistenceTypeString(cloudConfig.CloudVariables[i].PersistenceType));
                            break;
                        case CloudVariableType.CurrencyFloat:
                        case CloudVariableType.CurrencyInt:
                            fieldString = c_currencyVariableTemplate;
                            fieldString = fieldString.Replace("ALLOW_NEGATIVE", FirstLetterToLowerCase(cloudConfig.CloudVariables[i].AllowNegative.ToString()));
                            break;
                        case CloudVariableType.DateTime:
                            fieldString = c_dateTimeVariableTemplate;
                            fieldString = fieldString.Replace("PERSISTENCE_TYPE", GetPersistenceTypeString(cloudConfig.CloudVariables[i].PersistenceType));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    fieldString = fieldString.Replace("VAR_TYPE", GetClassNameString(cloudConfig.CloudVariables[i].Type));
                    var fieldName = "s_" + FirstLetterToLowerCase(cloudConfig.CloudVariables[i].Key);
                    fieldString = fieldString.Replace("FIELDNAME", fieldName);
                    fieldString = fieldString.Replace("VAR_ID", cloudConfig.CloudVariables[i].Key);
                    fieldString = fieldString.Replace("VAR_DEFAULT_VALUE", GetDefaultValueString(cloudConfig.CloudVariables[i].DefaultValueString, cloudConfig.CloudVariables[i].Type));

                    var propertyString = varPropertyTemplate;
                    propertyString = propertyString.Replace("VAR_SIMPLE", GetSimpleVariableTypeString(cloudConfig.CloudVariables[i].Type));
                    propertyString = propertyString.Replace("VAR_ID", cloudConfig.CloudVariables[i].Key);
                    propertyString = propertyString.Replace("FIELDNAME", fieldName);
                    builder.AppendLine(fieldString).AppendLine();
                    builder.AppendLine(propertyString);
                    if (i != cloudConfig.CloudVariables.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                newCloudVariablesScript = newCloudVariablesScript.Replace("// CLOUD_VARIABLES", builder.ToString());

                writer.Write(newCloudVariablesScript);
            }

            try
            {
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(c_cloudVariablesPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(c_cloudVariablesPath));
            }
        }

        private static string FirstLetterToLowerCase(string str)
        {
            return str.ToLowerInvariant()[0] + str.Substring(1);
        }

        private static string GetClassNameString(CloudVariableType type)
        {
            switch (type)
            {
                case CloudVariableType.Int:
                    return "CloudInt";
                case CloudVariableType.CurrencyInt:
                    return "CloudCurrencyInt";
                case CloudVariableType.Float:
                    return "CloudFloat";
                case CloudVariableType.CurrencyFloat:
                    return "CloudCurrencyFloat";
                case CloudVariableType.Bool:
                    return "CloudBool";
                case CloudVariableType.String:
                    return "CloudString";
                case CloudVariableType.Double:
                    return "CloudDouble";
                case CloudVariableType.UInt:
                    return "CloudUInt";
                case CloudVariableType.Long:
                    return "CloudLong";
                case CloudVariableType.DateTime:
                    return "CloudDateTime";
                case CloudVariableType.Decimal:
                    return "CloudDecimal";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static string GetDefaultValueString(string defaultValueString, CloudVariableType type)
        {
            switch (type)
            {
                case CloudVariableType.Int:
                case CloudVariableType.CurrencyInt:
                case CloudVariableType.Double:
                case CloudVariableType.UInt:
                case CloudVariableType.Long:
                case CloudVariableType.DateTime:
                return defaultValueString;
                case CloudVariableType.Float:
                case CloudVariableType.CurrencyFloat:
                    return defaultValueString + "f";
                case CloudVariableType.Decimal:
                    return defaultValueString + "m";
                case CloudVariableType.Bool:
                    return FirstLetterToLowerCase(defaultValueString);
                case CloudVariableType.String:
                    return "\"" + defaultValueString + "\"";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static string GetPersistenceTypeString(PersistenceType type)
        {
            switch (type)
            {
                case PersistenceType.Latest:
                    return "PersistenceType.Latest";
                case PersistenceType.Highest:
                    return "PersistenceType.Highest";
                case PersistenceType.Lowest:
                    return "PersistenceType.Lowest";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static string GetSimpleVariableTypeString(CloudVariableType type)
        {
            switch (type)
            {
                case CloudVariableType.Int:
                case CloudVariableType.CurrencyInt:
                    return "int";
                case CloudVariableType.Float:
                case CloudVariableType.CurrencyFloat:
                    return "float";
                case CloudVariableType.Bool:
                    return "bool";
                case CloudVariableType.String:
                    return "string";
                case CloudVariableType.Double:
                    return "double";
                case CloudVariableType.UInt:
                    return "uint";
                case CloudVariableType.Long:
                    return "long";
                case CloudVariableType.DateTime:
                    return "System.DateTime";
                case CloudVariableType.Decimal:
                    return "decimal";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        #endregion / Private methods
    }
}

#endif
