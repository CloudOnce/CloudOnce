// <copyright file="SerializationUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
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

        private const string achievementsPath = CloudOncePaths.Data + "/Achievements.cs";
        private const string leaderboardsPath = CloudOncePaths.Data + "/Leaderboards.cs";
        private const string cloudVariablesPath = CloudOncePaths.Data + "/CloudVariables.cs";
        private const string cloudIDsPath = CloudOncePaths.Data + "/CloudIDs.cs";

        private const string achievementsTemplatePath = CloudOncePaths.Templates + "/AchievementsTemplate.cs.txt";
        private const string leaderboardsTemplatePath = CloudOncePaths.Templates + "/LeaderboardsTemplate.cs.txt";
        private const string cloudVariablesTemplatePath = CloudOncePaths.Templates + "/CloudVariablesTemplate.cs.txt";
        private const string cloudIDsTemplatePath = CloudOncePaths.Templates + "/CloudIDsTemplate.cs.txt";

        private const string achievementsPropertyTemplatePath = CloudOncePaths.Templates + "/AchievementsPropertyTemplate.cs.txt";
        private const string leaderboardsPropertyTemplatePath = CloudOncePaths.Templates + "/LeaderboardsPropertyTemplate.cs.txt";
        private const string cloudVariablesPropertyTemplatePath = CloudOncePaths.Templates + "/CloudVariablesPropertyTemplate.cs.txt";
        private const string cloudIDsPropertyTemplatePath = CloudOncePaths.Templates + "/CloudIDsPropertyTemplate.cs.txt";

        private const string defaultVariableTemplate = "        private static readonly VAR_TYPE FIELDNAME = new VAR_TYPE(\"VAR_ID\", PERSISTENCE_TYPE, VAR_DEFAULT_VALUE);";
        private const string currencyVariableTemplate = "        private static readonly VAR_TYPE FIELDNAME = new VAR_TYPE(\"VAR_ID\", VAR_DEFAULT_VALUE, ALLOW_NEGATIVE);";
        private const string dateTimeVariableTemplate = "        private static readonly VAR_TYPE FIELDNAME = new VAR_TYPE(\"VAR_ID\", PERSISTENCE_TYPE);";
        private const string allAchievementsTemplate = "        public static readonly UnifiedAchievement[] All =";

        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Loads the cloud configuration.
        /// </summary>
        /// <returns>Imported CloudConfig instance. Returns an empty instance if cloud config file doesn't exist.</returns>
        public static CloudConfig LoadCloudConfig()
        {
            var cloudConfig = ScriptableObject.CreateInstance<CloudConfig>();
            var settingsPath = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.SettingsProjectSettings);
            var assetsPath = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.SettingsAssets);

            string settingsJson = null;
            if (File.Exists(settingsPath))
            {
                var sr = new StreamReader(settingsPath);
                settingsJson = sr.ReadToEnd();
                sr.Close();
            }
            else if (File.Exists(assetsPath))
            {
                var sr = new StreamReader(assetsPath);
                settingsJson = sr.ReadToEnd();
                sr.Close();
            }

            if (!string.IsNullOrEmpty(settingsJson))
            {
                cloudConfig.ImportSettingsFromJSON(new JSONObject(settingsJson));
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
            var projectSettingsPath = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.SettingsProjectSettings);
            var assetsPath = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.SettingsAssets);
            string settingsPath;
            switch (cloudConfig.SettingsLocation)
            {
                case SettingsLocation.ProjectSettings:
                    settingsPath = projectSettingsPath;
                    if (File.Exists(assetsPath))
                    {
                        try
                        {
                            File.Delete(assetsPath);
                            File.Delete(assetsPath + ".meta");
                            AssetDatabase.Refresh();
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to delete settings file: " + assetsPath + "\nIt should be deleted to avoid confusing CloudOnce.");
                        }
                    }

                    break;
                case SettingsLocation.Assets:
                    settingsPath = assetsPath;
                    if (File.Exists(projectSettingsPath))
                    {
                        try
                        {
                            File.Delete(projectSettingsPath);
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to delete settings file: " + projectSettingsPath + "\nIt should be deleted to avoid confusing CloudOnce.");
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using (var writer = new StreamWriter(settingsPath))
            {
                writer.Write(cloudConfig.ToJSONObject().ToString(true));
            }

            if (cloudConfig.SettingsLocation == SettingsLocation.Assets)
            {
                AssetDatabase.Refresh();
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
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(cloudIDsPath)))
            {
                string newCloudIDsScript;
                string idPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(cloudIDsTemplatePath)))
                {
                    newCloudIDsScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(cloudIDsPropertyTemplatePath)))
                {
                    idPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                for (var i = 0; i < cloudConfig.AchievementIDs.Count; i++)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("INTERNALID", cloudConfig.AchievementIDs[i].InternalId);
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
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(cloudIDsPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(cloudIDsPath));
            }
        }

        /// <summary>
        /// Generates a static script that provides access to all achievements created in the CloudOnce Editor.
        /// If the Achievements.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateAchievementsScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(achievementsPath)))
            {
                string newAchievementsScript;
                string idPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(achievementsTemplatePath)))
                {
                    newAchievementsScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(achievementsPropertyTemplatePath)))
                {
                    idPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                var dictionaryBuilder = new StringBuilder();
                foreach (var idData in cloudConfig.AchievementIDs)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("FIELDNAME", "s_" + FirstLetterToLowerCase(idData.InternalId));
                    propertyString = propertyString.Replace("INTERNALID", idData.InternalId);
                    propertyString = propertyString.Replace("APPLEID", idData.AppleId);
                    propertyString = propertyString.Replace("GOOGLEID", idData.GoogleId);
                    builder.AppendLine(propertyString).AppendLine();

                    var dictionaryString = "            { \"INTERNALID\", FIELDNAME },";
                    dictionaryString = dictionaryString.Replace("FIELDNAME", "s_" + FirstLetterToLowerCase(idData.InternalId));
                    dictionaryString = dictionaryString.Replace("INTERNALID", idData.InternalId);
                    dictionaryBuilder.AppendLine(dictionaryString);
                }

                if (cloudConfig.AchievementIDs.Count > 0)
                {
                    dictionaryBuilder.Remove(dictionaryBuilder.Length - 2 , 2);
                }

                builder.AppendLine(allAchievementsTemplate).AppendLine("        {");
                foreach (var idData in cloudConfig.AchievementIDs)
                {
                    builder.AppendLine("            s_" + FirstLetterToLowerCase(idData.InternalId) + ",");
                }

                builder.AppendLine("        };");

                newAchievementsScript = newAchievementsScript.Replace("// ACHIEVEMENT_IDS", builder.ToString());
                newAchievementsScript = newAchievementsScript.Replace("// ACHIEVEMENT_DICTIONARY", dictionaryBuilder.ToString());

                writer.Write(newAchievementsScript);
            }

            try
            {
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(achievementsPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(achievementsPath));
            }
        }

        /// <summary>
        /// Generates a static script that provides access to all leaderboards created in the CloudOnce Editor.
        /// If the Leaderboards.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateLeaderboardsScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(leaderboardsPath)))
            {
                string newLeaderboardsScript;
                string idPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(leaderboardsTemplatePath)))
                {
                    newLeaderboardsScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(leaderboardsPropertyTemplatePath)))
                {
                    idPropertyTemplate = reader.ReadToEnd();
                }

                var builder = new StringBuilder();
                var dictionaryBuilder = new StringBuilder();
                for (var i = 0; i < cloudConfig.LeaderboardIDs.Count; i++)
                {
                    var propertyString = idPropertyTemplate;
                    propertyString = propertyString.Replace("FIELDNAME", "s_" + FirstLetterToLowerCase(cloudConfig.LeaderboardIDs[i].InternalId));
                    propertyString = propertyString.Replace("INTERNALID", cloudConfig.LeaderboardIDs[i].InternalId);
                    propertyString = propertyString.Replace("APPLEID", cloudConfig.LeaderboardIDs[i].AppleId);
                    propertyString = propertyString.Replace("GOOGLEID", cloudConfig.LeaderboardIDs[i].GoogleId);
                    builder.AppendLine(propertyString);

                    var dictionaryString = "            { \"INTERNALID\", FIELDNAME },";
                    dictionaryString = dictionaryString.Replace("FIELDNAME", "s_" + FirstLetterToLowerCase(cloudConfig.LeaderboardIDs[i].InternalId));
                    dictionaryString = dictionaryString.Replace("INTERNALID", cloudConfig.LeaderboardIDs[i].InternalId);
                    dictionaryBuilder.AppendLine(dictionaryString);

                    if (i != cloudConfig.LeaderboardIDs.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                if (cloudConfig.LeaderboardIDs.Count > 0)
                {
                    dictionaryBuilder.Remove(dictionaryBuilder.Length - 2 , 2);
                }

                newLeaderboardsScript = newLeaderboardsScript.Replace("// LEADERBOARD_IDS", builder.ToString());
                newLeaderboardsScript = newLeaderboardsScript.Replace("// LEADERBOARD_DICTIONARY", dictionaryBuilder.ToString());

                writer.Write(newLeaderboardsScript);
            }

            try
            {
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(leaderboardsPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(leaderboardsPath));
            }
        }

        /// <summary>
        /// Generates a static script that provides access to all cloud variables created in the CloudOnce Editor.
        /// If the CloudVariables.cs script already exists it will be overwritten.
        /// </summary>
        /// <param name="cloudConfig">The CloudConfig instance to generate the static script from.</param>
        private static void CreateCloudVariablesScript(CloudConfig cloudConfig)
        {
            using (var writer = new StreamWriter(GPGSUtil.SlashesToPlatformSeparator(cloudVariablesPath)))
            {
                string newCloudVariablesScript;
                string varPropertyTemplate;
                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(cloudVariablesTemplatePath)))
                {
                    newCloudVariablesScript = reader.ReadToEnd();
                }

                using (TextReader reader = File.OpenText(GPGSUtil.SlashesToPlatformSeparator(cloudVariablesPropertyTemplatePath)))
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
                            fieldString = defaultVariableTemplate;
                            fieldString = fieldString.Replace("PERSISTENCE_TYPE", GetPersistenceTypeString(cloudConfig.CloudVariables[i].PersistenceType));
                            break;
                        case CloudVariableType.CurrencyFloat:
                        case CloudVariableType.CurrencyInt:
                            fieldString = currencyVariableTemplate;
                            fieldString = fieldString.Replace("ALLOW_NEGATIVE", FirstLetterToLowerCase(cloudConfig.CloudVariables[i].AllowNegative.ToString()));
                            break;
                        case CloudVariableType.DateTime:
                            fieldString = dateTimeVariableTemplate;
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
                AssetDatabase.ImportAsset(GPGSUtil.SlashesToPlatformSeparator(cloudVariablesPath));
            }
            catch (Exception)
            {
                Debug.LogError("Can't import asset: " + GPGSUtil.SlashesToPlatformSeparator(cloudVariablesPath));
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
