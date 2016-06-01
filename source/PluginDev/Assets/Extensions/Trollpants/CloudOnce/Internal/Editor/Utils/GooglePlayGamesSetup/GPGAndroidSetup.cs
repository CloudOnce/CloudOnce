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

// Code has been refactored and repurposed by Trollpants Game Studio,
// is originally from GPGSAndroidSetupUI.cs in the GPG Unity plug-in.

#if UNITY_EDITOR
namespace Trollpants.CloudOnce.Internal.Editor.Utils
{
#if UNITY_ANDROID
    using GooglePlayGames;
    using GooglePlayGames.Editor;
    using GooglePlayServices;
#endif
    using UnityEditor;

    public static class GPGAndroidSetup
    {
        private const string c_manifestTemplate = CloudOncePaths.GoogleTemplates + "/template-AndroidManifest.txt";

        private const string c_appIdPlaceholder = "__APP_ID__";
        private const string c_pluginVersionPlaceholder = "__PLUGIN_VERSION__";
        private const string c_serviceIdPlaceholder = "__NEARBY_SERVICE_ID__";

        public static bool DoSetup(string appID)
        {
            var projAM = GPGSUtil.SlashesToPlatformSeparator(CloudOncePaths.GooglePlayLib + "/AndroidManifest.xml");

            // check for valid app id
            if (!GPGSUtil.LooksLikeValidAppId(appID))
            {
                GPGSUtil.Alert(GPGSStrings.Setup.AppIdError);
                return false;
            }

            // Generate AndroidManifest.xml
            var manifestBody = GPGSUtil.ReadFile(c_manifestTemplate);
            manifestBody = manifestBody.Replace(c_appIdPlaceholder, appID);
#if UNITY_ANDROID
            manifestBody = manifestBody.Replace(c_pluginVersionPlaceholder, PluginVersion.VersionString);
#endif
            manifestBody = manifestBody.Replace(c_serviceIdPlaceholder, string.Empty);
            GPGSUtil.WriteFile(projAM, manifestBody);

            // Resolve dependencies
#if UNITY_ANDROID
            PlayServicesResolver.Resolver.DoResolution(
                GPGSDependencies.svcSupport,
                CloudOncePaths.Android,
                PlayServicesResolver.HandleOverwriteConfirmation);
#endif

            // refresh assets, and we're done
            AssetDatabase.Refresh();
            return EditorUtility.DisplayDialog(GPGSStrings.Success, GPGSStrings.AndroidSetup.SetupComplete, GPGSStrings.Ok);
        }
    }
}
#endif
