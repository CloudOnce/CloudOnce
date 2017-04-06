// <copyright file="GPGSDependencies.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
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
#if UNITY_ANDROID

namespace GooglePlayGames.Editor
{
    using System;
    using System.Collections.Generic;
    using Google;
    using Google.JarResolver;
    using UnityEditor;

    /// <summary>
    /// Play-Services Dependencies for Google Play Games.
    /// </summary>
    [InitializeOnLoad]
    public class GPGSDependencies : AssetPostprocessor
    {
        /// <summary>Instance of the PlayServicesSupport resolver</summary>
        public static PlayServicesSupport svcSupport;

        /// <summary>
        /// Initializes static members of the class.
        /// </summary>
        static GPGSDependencies()
        {
            RegisterDependencies();
        }

        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        public static void RegisterDependencies()
        {
            // Setup the resolver using reflection as the module may not be
            // available at compile time.
            Type playServicesSupport = VersionHandler.FindClass(
                "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
            if (playServicesSupport == null)
            {
                return;
            }

            if (svcSupport == null)
            {
                svcSupport = (PlayServicesSupport)VersionHandler.InvokeStaticMethod(
                    playServicesSupport, "CreateInstance",
                    new object[]
                    {
                        "GooglePlayGames",
                        EditorPrefs.GetString("AndroidSdkRoot"),
                        "ProjectSettings"
                    });
            }

            VersionHandler.InvokeInstanceMethod(
                svcSupport, "DependOn",
                new object[]
                {
                    "com.google.android.gms", "play-services-games",
                    PluginVersion.PlayServicesVersionConstraint
                },
                new Dictionary<string, object>
                {
                    { "packageIds", new[] { "extra-google-m2repository" } }
                });

            VersionHandler.InvokeInstanceMethod(
                svcSupport, "DependOn",
                new object[]
                {
                    "com.google.android.gms", "play-services-nearby",
                    PluginVersion.PlayServicesVersionConstraint
                },
                new Dictionary<string, object>
                {
                    { "packageIds", new[] { "extra-google-m2repository" } }
                });

            // Auth is needed for getting the token and email.
            VersionHandler.InvokeInstanceMethod(
                svcSupport, "DependOn",
                new object[]
                {
                    "com.google.android.gms", "play-services-auth",
                    PluginVersion.PlayServicesVersionConstraint
                },
                new Dictionary<string, object>
                {
                    { "packageIds", new[] { "extra-google-m2repository" } }
                });

        }

        // Handle delayed loading of the dependency resolvers.
        private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromPath)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.Contains("JarResolver"))
                {
                    RegisterDependencies();
                    break;
                }
            }
        }
    }
}
#endif
