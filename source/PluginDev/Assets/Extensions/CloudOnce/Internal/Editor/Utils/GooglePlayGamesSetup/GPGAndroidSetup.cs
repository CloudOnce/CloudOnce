// <copyright file="GPGAndroidSetup.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
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

// Code has been refactored and repurposed by Jan Ivar Z. Carlsen,
// is originally from GPGSAndroidSetupUI.cs in the GPG Unity plug-in.

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
#if UNITY_ANDROID
    using GooglePlayGames;
    using GooglePlayServices;
#endif
    using UnityEditor;

    [InitializeOnLoad]
    public static class GPGAndroidSetup
    {
        static GPGAndroidSetup()
        {
            Google.VersionHandler.VerboseLoggingEnabled = false;
        }

        private const string manifestTemplate = CloudOncePaths.GoogleTemplates + "/template-AndroidManifest.txt";

        private const string appIdPlaceholder = "__APP_ID__";
        private const string pluginVersionPlaceholder = "__PLUGIN_VERSION__";
        private const string serviceIdPlaceholder = "__NEARBY_SERVICE_ID__";

        public static bool DoSetup(string appID)
        {
            // check for valid app id
            if (!GPGSUtil.LooksLikeValidAppId(appID))
            {
                GPGSUtil.Alert(GPGSStrings.Setup.AppIdError);
                return false;
            }

            if (!GPGSUtil.HasAndroidSdk())
            {
                EditorUtility.DisplayDialog(
                    GPGSStrings.AndroidSetup.SdkNotFound,
                    GPGSStrings.AndroidSetup.SdkNotFoundBlurb,
                    GPGSStrings.Ok);
                return false;
            }

            // Generate AndroidManifest.xml
            var destination = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.GooglePlayLib + "/AndroidManifest.xml");
            var manifestBody = GPGSUtil.ReadFile(manifestTemplate);
            manifestBody = manifestBody.Replace(appIdPlaceholder, appID);
#if UNITY_ANDROID
            manifestBody = manifestBody.Replace(pluginVersionPlaceholder, PluginVersion.VersionString);
#endif
            manifestBody = manifestBody.Replace(serviceIdPlaceholder, string.Empty);
            GPGSUtil.WriteFile(destination, manifestBody);

            // Resolve dependencies
            Google.VersionHandler.UpdateVersionedAssets(true);
            Google.VersionHandler.Enabled = true;
            AssetDatabase.Refresh();
#if UNITY_ANDROID
            PlayServicesResolver.MenuResolve();
#endif

            // refresh assets, and we're done
            AssetDatabase.Refresh();
            return EditorUtility.DisplayDialog(GPGSStrings.Success, GPGSStrings.AndroidSetup.SetupComplete, GPGSStrings.Ok);
        }
    }
}
#endif
