// <copyright file="AndroidManifestModifier.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR && !UNITY_2018_3_OR_NEWER
namespace CloudOnce.Internal.Editor.Utils
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEditor;

    /// <summary>
    /// Updates AndroidManifest.xml with the appropriate permissions and activities.
    /// </summary>
    public class AndroidManifestModifier : IXmlNamespaceResolver
    {
        #region Fields & properties

        private const string androidManifestFileName = "/AndroidManifest.xml";
        private const string mainAndroidManifestPath = CloudOncePaths.Android + androidManifestFileName;
        private const string mainAndroidManifestTemplatePath = CloudOncePaths.Templates + "/AndroidManifestTemplate.xml";

        private XDocument manifest;

        #endregion /Fields & properties

        #region IXmlNamespaceResolver implementation

        /// <summary>
        /// Not implemented. Method required by <see cref="IXmlNamespaceResolver"/>.
        /// </summary>
        /// <param name="scope">Scope</param>
        /// <returns>Namespaces</returns>
        public System.Collections.Generic.IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Only implemented for android.
        /// </summary>
        /// <param name="platform">Platform</param>
        /// <returns>Prefix</returns>
        public string LookupNamespace(string platform)
        {
            if (platform == "android")
            {
                return "http://schemas.android.com/apk/res/android";
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented. Method required by <see cref="IXmlNamespaceResolver"/>.
        /// </summary>
        /// <param name="ns">Namespace</param>
        /// <returns>Prefix</returns>
        public string LookupPrefix(string ns)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Will switch out code in the Android manifest to fit a Google Play build.
        /// </summary>
        public void EnableGoogleBuildPlatform()
        {
            EnsureMainManifestExists();
        }

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Checks if AndroidManifest.xml exists, if it doesn't it's created.
        /// </summary>
        private static void EnsureMainManifestExists()
        {
            if (!Directory.Exists(CloudOncePaths.Android))
            {
                var folders = CloudOncePaths.Android.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (folders.Length == 1)
                {
                    return;
                }

                var pluginsPath = folders[0];
                for (var i = 1; i < folders.Length - 1; i++)
                {
                    pluginsPath += "/" + folders[i];
                }

                AssetDatabase.CreateFolder(pluginsPath, folders[folders.Length - 1]);
            }

            if (!File.Exists(mainAndroidManifestPath))
            {
                using (var writer = new StreamWriter(mainAndroidManifestPath))
                {
                    string newAndroidManifest;
                    using (TextReader reader = File.OpenText(mainAndroidManifestTemplatePath))
                    {
                        newAndroidManifest = reader.ReadToEnd();
                    }

                    writer.Write(newAndroidManifest);
                }

                AssetDatabase.ImportAsset(mainAndroidManifestPath);
            }
        }

        #endregion / Private methods
    }
}
#endif
