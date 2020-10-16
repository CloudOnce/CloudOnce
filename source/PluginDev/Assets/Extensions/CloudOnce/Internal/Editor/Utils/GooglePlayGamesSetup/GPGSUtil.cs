// <copyright file="GPGSUtil.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

// Removed several methods. Refactored.
#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using UnityEditor;

    public static class GPGSUtil
    {
        /// <summary>
        /// Replaces / in file path to be the os specific separator.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="path">Path with correct separators.</param>
        public static string SlashesToPlatformSeparator(string path)
        {
            return path.Replace("/", Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <returns>The file contents.  The slashes are corrected.</returns>
        /// <param name="filePath">File path.</param>
        public static string ReadFile(string filePath)
        {
            filePath = SlashesToPlatformSeparator(filePath);
            if (!File.Exists(filePath))
            {
                Alert("Plugin error: file not found: " + filePath);
                return null;
            }

            var sr = new StreamReader(filePath);
            var body = sr.ReadToEnd();
            sr.Close();
            return body;
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="file">File path - the slashes will be corrected.</param>
        /// <param name="body">Body of the file to write.</param>
        public static void WriteFile(string file, string body)
        {
            file = SlashesToPlatformSeparator(file);
            DirectoryInfo dir = Directory.GetParent(file);
            dir.Create();
            using (var wr = new StreamWriter(file, false))
            {
                wr.Write(body);
            }
        }

        /// <summary>
        /// Looks the like valid app identifier.
        /// </summary>
        /// <returns><c>true</c>, if valid app identifier, <c>false</c> otherwise.</returns>
        /// <param name="s">the string to test.</param>
        public static bool LooksLikeValidAppId(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length < 5)
            {
                return false;
            }

            return s.All(c => c >= '0' && c <= '9');
        }

        /// <summary>
        /// Displays an error dialog.
        /// </summary>
        /// <param name="s">the message</param>
        public static void Alert(string s)
        {
            Alert(GPGSStrings.Error, s);
        }

        /// <summary>
        /// Displays a dialog with the given title and message.
        /// </summary>
        /// <param name="title">the title.</param>
        /// <param name="message">the message.</param>
        public static void Alert(string title, string message)
        {
            EditorUtility.DisplayDialog(title, message, GPGSStrings.Ok);
        }

        /// <summary>
        /// Gets the android sdk path.
        /// </summary>
        /// <returns>The android sdk path.</returns>
        public static string GetAndroidSdkPath()
        {
            var sdkPath = EditorPrefs.GetString("AndroidSdkRoot");
#if UNITY_2019 || UNITY_2020
            // Unity 2019.x added installation of the Android SDK in the AndroidPlayer directory
            // so fallback to searching for it there.
            if (string.IsNullOrEmpty(sdkPath) || EditorPrefs.GetBool("SdkUseEmbedded"))
            {
                var androidPlayerDir = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
                if (!string.IsNullOrEmpty(androidPlayerDir))
                {
                    var androidPlayerSdkDir = Path.Combine(androidPlayerDir, "SDK");
                    if (Directory.Exists(androidPlayerSdkDir))
                    {
                        sdkPath = androidPlayerSdkDir;
                    }
                }
            }
#endif
            if (sdkPath != null && (sdkPath.EndsWith("/") || sdkPath.EndsWith("\\")))
            {
                sdkPath = sdkPath.Substring(0, sdkPath.Length - 1);
            }

            return sdkPath;
        }

        /// <summary>
        /// Determines if the android sdk exists.
        /// </summary>
        /// <returns><c>true</c> if  android sdk exists; otherwise, <c>false</c>.</returns>
        public static bool HasAndroidSdk()
        {
            var sdkPath = GetAndroidSdkPath();
            return sdkPath != null && sdkPath.Trim() != string.Empty && Directory.Exists(sdkPath);
        }
    }
}
#endif
