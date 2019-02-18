// <copyright file="ValidationUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Data;
    using UnityEditor;

    /// <summary>
    /// Validation utilities used by the CloudOnce editor.
    /// </summary>
    public static class ValidationUtils
    {
        #region Public methods

        /// <summary>
        /// Removes forbidden characters from a platform ID.
        /// </summary>
        /// <param name="text">The <see cref="string"/> that may contain forbidden characters.</param>
        /// <param name="platform">Type of platform ID</param>
        /// <returns>A <see cref="string"/> free from forbidden characters.</returns>
        public static string RemoveForbiddenCharactersFromPlatformID(string text, CloudPlatform platform)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (platform)
            {
                case CloudPlatform.iOS:
                    return string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text, @"[^a-zA-Z0-9_.]", string.Empty);
                case CloudPlatform.GooglePlay:
                    return string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text, @"[^a-zA-Z0-9-_]", string.Empty);
                default:
                    throw new ArgumentOutOfRangeException("platform", platform, null);
            }
        }

        /// <summary>
        /// Removes forbidden characters from an internal ID. (Allowed characters: a-z, A-Z, 0-9, -, _)
        /// </summary>
        /// <param name="text">The <see cref="string"/> that may contain forbidden characters.</param>
        /// <returns>A <see cref="string"/> free from forbidden characters.</returns>
        public static string RemoveForbiddenCharactersFromInternalID(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var returnString = text;
            while (returnString.Length > 0 && (returnString[0] == '-' || char.IsDigit(returnString[0])))
            {
                returnString = returnString.Remove(0, 1);
            }

            return string.IsNullOrEmpty(returnString) ? string.Empty : Regex.Replace(returnString, @"[^a-zA-Z0-9-_]", string.Empty);
        }

        /// <summary>
        /// Removes forbidden characters from a cloud variables default value.
        /// </summary>
        /// <param name="text">The <see cref="string"/> that may contain forbidden characters.</param>
        /// <param name="type">Cloud variable type.</param>
        /// <returns>A <see cref="string"/> free from forbidden characters.</returns>
        public static string RemoveForbiddenCharactersFromDefaultValue(string text, CloudVariableType type)
        {
            var returnString = text;

            switch (type)
            {
                case CloudVariableType.Int:
                case CloudVariableType.CurrencyInt:
                case CloudVariableType.Long:
                case CloudVariableType.DateTime:
                    if (string.IsNullOrEmpty(returnString))
                    {
                        return "0";
                    }

                    var isNegativeInt = returnString.StartsWith("-");
                    returnString = Regex.Replace(returnString, @"[^0-9]", string.Empty);
                    returnString = isNegativeInt ? "-" + returnString : returnString;
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        int.Parse(returnString);
                    }
                    catch (Exception ex)
                    {
                        if (ex is OverflowException)
                        {
                            returnString = isNegativeInt ? int.MinValue.ToString() : int.MaxValue.ToString();
                        }
                    }

                    break;
                case CloudVariableType.Float:
                case CloudVariableType.CurrencyFloat:
                case CloudVariableType.Double:
                case CloudVariableType.Decimal:
                    if (string.IsNullOrEmpty(returnString))
                    {
                        returnString = "0";
                    }
                    else
                    {
                        returnString = Regex.Replace(returnString, ",", ".");
                        returnString = Regex.Replace(returnString, @"[^0-9E+-.]", string.Empty);
                        if (string.IsNullOrEmpty(returnString))
                        {
                            returnString = "0";
                        }
                    }

                    break;
                case CloudVariableType.Bool:
                    bool result;
                    var parsable = bool.TryParse(returnString, out result);
                    returnString = parsable ? result.ToString() : false.ToString();
                    break;
                case CloudVariableType.String:
                    break;
                case CloudVariableType.UInt:
                    if (string.IsNullOrEmpty(returnString))
                    {
                        return "0";
                    }

                    returnString = Regex.Replace(returnString, @"[^0-9]", string.Empty);
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        uint.Parse(returnString);
                    }
                    catch (Exception ex)
                    {
                        if (ex is OverflowException)
                        {
                            returnString = uint.MaxValue.ToString();
                        }
                        else
                        {
                            returnString = "0";
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }

            return returnString;
        }

        /// <summary>
        /// Checks if all default values are valid according to their type. If an invalid value is found it will draw an
        /// <c>EditorGUILayout.HelpBox</c> informing the user about the problem.
        /// </summary>
        /// <param name="data">The cloud variable list to check.</param>
        /// <returns>Return <c>true</c> if all default values are valid.</returns>
        public static bool ConfigHasNoInvalidDefaultValues(List<CloudVariableData> data)
        {
            return data.Select(variableData => ValidateDefaultValue(variableData.DefaultValueString, variableData.Type)).All(result => result);
        }

        /// <summary>
        /// Checks for empty achievement- and leaderboard Internal IDs. If an empty field is found it will draw an
        /// <c>EditorGUILayout.HelpBox</c> informing the user about the problem.
        /// </summary>
        /// <param name="cloudConfig"><see cref="CloudConfig"/> instance to check.</param>
        /// <returns>Return <c>true</c> if no empty fields are found, <c>false</c> if at least one Internal ID field is empty.</returns>
        public static bool ConfigHasNoEmptyInternalIdFields(CloudConfig cloudConfig)
        {
            var idList = cloudConfig.AchievementIDs.Select(platformIdData => platformIdData.InternalId).ToList();
            idList.AddRange(cloudConfig.LeaderboardIDs.Select(platformIdData => platformIdData.InternalId));

            if (ListHasEmptyStrings(idList))
            {
                EditorGUILayout.HelpBox("All Internal Cloud ID fields must be filled out.", MessageType.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for empty achievement- and leaderboard platform IDs. If an empty field is found it will draw an
        /// <c>EditorGUILayout.HelpBox</c> informing the user about the issue.
        /// </summary>
        /// <param name="cloudConfig"><see cref="CloudConfig"/> instance to check.</param>
        /// <returns>Return <c>true</c> if no empty fields are found, <c>false</c> if at least one platform ID field is empty.</returns>
        public static bool ConfigHasNoEmptyPlatformIDs(CloudConfig cloudConfig)
        {
            var idList = new List<string>();
            foreach (var platformIdData in cloudConfig.AchievementIDs)
            {
                if (cloudConfig.AppleSupported)
                {
                    idList.Add(platformIdData.AppleId);
                }

                if (cloudConfig.GoogleSupported)
                {
                    idList.Add(platformIdData.GoogleId);
                }
            }

            foreach (var platformIdData in cloudConfig.LeaderboardIDs)
            {
                if (cloudConfig.AppleSupported)
                {
                    idList.Add(platformIdData.AppleId);
                }

                if (cloudConfig.GoogleSupported)
                {
                    idList.Add(platformIdData.GoogleId);
                }
            }

            if (ListHasEmptyStrings(idList))
            {
                EditorGUILayout.HelpBox("Not all Cloud ID fields for supported platforms have been filled out. " + "Remember to fix this before testing achievements and leaderboards your mobile devices.", MessageType.Info);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate achievement- or leaderboard IDs. If a duplicate is found it will draw an
        /// <c>EditorGUILayout.HelpBox</c> informing the user where the duplicate was detected.
        /// </summary>
        /// <param name="platformIdList">A list of either achievements or leaderboards.</param>
        /// <param name="checkAppleIDs">Check for duplicates in the Apple IDs.</param>
        /// <param name="checkGoogleIDs">Check for duplicates in the Google IDs.</param>
        /// <param name="listName">Should be either "achievement" or "leaderboard". Is included in the warning message.</param>
        /// <returns>Return <c>true</c> if no duplicates are found, <c>false</c> if at least one duplicate exists.</returns>
        public static bool ConfigHasNoDuplicateIDs(List<PlatformIdData> platformIdList, bool checkAppleIDs, bool checkGoogleIDs, string listName)
        {
            var idList = platformIdList.Select(platformIdData => platformIdData.InternalId).ToList();

            if (ListHasDuplicateStrings(idList))
            {
                EditorGUILayout.HelpBox("Duplicate internal " + listName + " Cloud ID detected.", MessageType.Warning);
                return false;
            }

            if (checkAppleIDs)
            {
                idList = platformIdList.Select(platformIdData => platformIdData.AppleId).ToList();

                if (ListHasDuplicateStrings(idList))
                {
                    EditorGUILayout.HelpBox("Duplicate Apple " + listName + " Cloud ID detected.", MessageType.Warning);
                    return false;
                }
            }

            if (checkGoogleIDs)
            {
                idList = platformIdList.Select(platformIdData => platformIdData.GoogleId).ToList();

                if (ListHasDuplicateStrings(idList))
                {
                    EditorGUILayout.HelpBox("Duplicate Google " + listName + " Cloud ID detected.", MessageType.Warning);
                    return false;
                }
            }

            return true;
        }

        public static bool ConfigHasNoDuplicateCloudVariableKeys(List<CloudVariableData> variableList)
        {
            var keyList = variableList.Select(data => data.Key).ToList();

            if (ListHasDuplicateStrings(keyList))
            {
                EditorGUILayout.HelpBox("Duplicate cloud variable ID detected.", MessageType.Warning);
                return false;
            }

            return true;
        }

        public static bool ConfigHasNoReservedIDs(List<PlatformIdData> platformIds, params string[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return true;
            }

            var result = true;
            foreach (var id in ids)
            {
                if (platformIds.Any(p => string.Equals(p.InternalId, id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    result = false;
                    EditorGUILayout.HelpBox("\"" + id + "\" internal achievement ID is reserved.", MessageType.Warning);
                }
            }

            return result;
        }

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Checks if default value is valid according to type.
        /// </summary>
        /// <param name="text">The <see cref="string"/> that needs to be validated.</param>
        /// <param name="type">Cloud variable type.</param>
        /// <returns>Return <c>true</c> if default value is valid.</returns>
        private static bool ValidateDefaultValue(string text, CloudVariableType type)
        {
            switch (type)
            {
                case CloudVariableType.Int:
                case CloudVariableType.CurrencyInt:
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        int.Parse(text);
                    }
                    catch (Exception)
                    {
                        EditorGUILayout.HelpBox("Invalid default value detected in Int cloud variable.", MessageType.Warning);
                        return false;
                    }

                    return true;
                case CloudVariableType.Long:
                case CloudVariableType.DateTime:
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        long.Parse(text);
                    }
                    catch (Exception)
                    {
                        EditorGUILayout.HelpBox("Invalid default value detected in Long cloud variable.", MessageType.Warning);
                        return false;
                    }

                    return true;
                case CloudVariableType.Bool:
                case CloudVariableType.String:
                case CloudVariableType.UInt:
                    return true;
                case CloudVariableType.Float:
                case CloudVariableType.CurrencyFloat:
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        float.Parse(text);
                    }
                    catch (Exception)
                    {
                        EditorGUILayout.HelpBox("Invalid default value detected in Float cloud variable.", MessageType.Warning);
                        return false;
                    }

                    return true;
                case CloudVariableType.Double:
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        double.Parse(text);
                    }
                    catch (Exception)
                    {
                        EditorGUILayout.HelpBox("Invalid default value detected in Double cloud variable.", MessageType.Warning);
                        return false;
                    }

                    return true;
                case CloudVariableType.Decimal:
                    try
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        decimal.Parse(text);
                    }
                    catch (Exception)
                    {
                        EditorGUILayout.HelpBox("Invalid default value detected in Decimal cloud variable.", MessageType.Warning);
                        return false;
                    }

                    return true;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        /// <summary>
        /// Checks a list of strings contains any null or empty strings.
        /// </summary>
        /// <param name="list">The list that might contain null or empty strings</param>
        /// <returns>Returns <c>true</c> if the list contains null or empty strings, <c>false</c> if not.</returns>
        private static bool ListHasEmptyStrings(IEnumerable<string> list)
        {
            return list != null && list.Any(string.IsNullOrEmpty);
        }

        /// <summary>
        /// Checks a list of strings for duplicates. Ignores strings that are null or empty.
        /// </summary>
        /// <param name="list">The list that might contain duplicates.</param>
        /// <returns>Returns <c>true</c> if the list contains at least one duplicate, <c>false</c> if no duplicates are found.</returns>
        private static bool ListHasDuplicateStrings(IList<string> list)
        {
            if (list == null || list.Count < 2)
            {
                return false;
            }

            for (var i = 0; i < list.Count - 1; i++)
            {
                for (var j = i + 1; j < list.Count; j++)
                {
                    if (string.IsNullOrEmpty(list[i]) || string.IsNullOrEmpty(list[j]))
                    {
                        continue;
                    }

                    if (Equals(list[i], list[j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion / Private methods
    }
}

#endif
