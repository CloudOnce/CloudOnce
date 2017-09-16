// <copyright file="CloudOnceUpgrader.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2017 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor
{
    using System;
    using System.IO;
    using System.Linq;
    using Data;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    [InitializeOnLoad]
    public class CloudOnceUpgrader
    {
        static CloudOnceUpgrader()
        {
            var config = SerializationUtils.LoadCloudConfig();
            if (string.IsNullOrEmpty(config.Version))
            {
                return;
            }

            var oldVersion = new Version(config.Version);
            if (PluginVersion.Version.Equals(oldVersion))
            {
                return;
            }

            var upgraded = false;
            if (oldVersion < new Version(2, 4))
            {
                Upgrade240();
                upgraded = true;
            }

            if (upgraded)
            {
                AssetDatabase.Refresh();
            }
        }

        private static void Upgrade240()
        {
            var obsoleteFiles = new[]
            {
                "Assets/Extensions/GooglePlayGames/Editor/GPGSDependencies.cs",
                "Assets/Extensions/GooglePlayGames/Editor/GPGSDependencies.cs.meta",
                "Assets/Plugins/Android/libs/armeabi-v7a/libgpg.so",
                "Assets/Plugins/Android/libs/armeabi-v7a/libgpg.so.meta",
                "Assets/Plugins/Android/libs/x86/libgpg.so",
                "Assets/Plugins/Android/libs/x86/libgpg.so.meta",
                "Assets/Plugins/Android/MainLibProj/libs/play-games-plugin-support.jar",
                "Assets/Plugins/Android/MainLibProj/libs/play-games-plugin-support.jar.meta",
                "Assets/Plugins/Android/MainLibProj/AndroidManifest.xml",
                "Assets/Plugins/Android/MainLibProj/AndroidManifest.xml.meta",
                "Assets/Plugins/Android/MainLibProj/project.properties",
                "Assets/Plugins/Android/MainLibProj/project.properties.meta"
            };

            foreach (var file in obsoleteFiles.Where(File.Exists))
            {
                Debug.Log("Deleting obsolete file: " + file);
                File.Delete(file);
                var directory = Path.GetDirectoryName(file);
                if (directory != null)
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    if (directoryInfo.GetFiles().Length == 0)
                    {
                        Debug.Log("Deleting obsolete directory: " + directory);
                        directoryInfo.Delete();
                        if (directoryInfo.Parent != null)
                        {
                            var meta = Path.Combine(directoryInfo.Parent.FullName, directoryInfo.Name + ".meta");
                            if (File.Exists(meta))
                            {
                                File.Delete(meta);
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif
