// <copyright file="AndroidManifestModifier.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace Trollpants.CloudOnce.Internal.Editor.Utils
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using UnityEditor;

    /// <summary>
    /// Updates AndroidManifest.xml with the appropriate permissions and activities.
    /// </summary>
    public class AndroidManifestModifier : IXmlNamespaceResolver
    {
        #region Fields & properties

        private const string c_androidManifestFileName = "/AndroidManifest.xml";
        private const string c_mainAndroidManifestPath = CloudOncePaths.Android + c_androidManifestFileName;
        private const string c_mainAndroidManifestTemplatePath = CloudOncePaths.Templates + "/AndroidManifestTemplate.xml";
        private const string c_amazonAndroidManifestPath = CloudOncePaths.GameCircleLib + c_androidManifestFileName;

        private const string c_gameCircleUserInterface = "manifest/application/activity[@android:name='com.amazon.ags.html5.overlay.GameCircleUserInterface']";
        private const string c_authorizationActivity = "manifest/application/activity[@android:name='com.amazon.identity.auth.device.authorization.AuthorizationActivity']";
        private const string c_gameCircleAlertUserInterface = "manifest/application/activity[@android:name='com.amazon.ags.html5.overlay.GameCircleAlertUserInterface']";
        private const string c_gameCircleApiKey = "manifest/application/meta-data[@android:name='APIKey']";

        private readonly XNamespace xmlns = "http://schemas.android.com/apk/res/android";

        private readonly string[] amazonElements =
        {
            c_gameCircleUserInterface,
            c_gameCircleAlertUserInterface,
            c_authorizationActivity,
            c_gameCircleApiKey
        };

        private XDocument manifest;
        private string gameCircleApiKey;

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
        /// Will switch out code in the Android manifest to fit an Amazon build.
        /// </summary>
        public void EnableAmazonBuildPlatform(string apiKey)
        {
            gameCircleApiKey = apiKey;
            EnsureMainManifestExists();

            manifest = XDocument.Load(c_amazonAndroidManifestPath);

            foreach (var element in amazonElements)
            {
                RemoveXElementFromManifest(element);
            }

            foreach (var element in amazonElements)
            {
                AddXElementToManifest(element);
            }

            manifest.Save(c_amazonAndroidManifestPath);
            AssetDatabase.ImportAsset(c_amazonAndroidManifestPath);
        }

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

            if (!File.Exists(c_mainAndroidManifestPath))
            {
                using (var writer = new StreamWriter(c_mainAndroidManifestPath))
                {
                    string newAndroidManifest;
                    using (TextReader reader = File.OpenText(c_mainAndroidManifestTemplatePath))
                    {
                        newAndroidManifest = reader.ReadToEnd();
                    }

                    writer.Write(newAndroidManifest);
                }

                AssetDatabase.ImportAsset(c_mainAndroidManifestPath);
            }
        }

        private void AddXElementToManifest(string elementString)
        {
            if (manifest.XPathSelectElement(elementString, this) == null)
            {
                manifest.XPathSelectElement("manifest/application", this).Add(GetXElement(elementString));
            }
        }

        private void RemoveXElementFromManifest(string elementString)
        {
            var xElement = manifest.XPathSelectElement(elementString, this);
            if (xElement != null)
            {
                xElement.Remove();
            }
        }

        private XElement GetXElement(string element)
        {
            switch (element)
            {
                case c_gameCircleUserInterface:
                {
                    var gameCircleUserInterface = new XElement("activity");
                    gameCircleUserInterface.Add(new XAttribute(xmlns + "name", "com.amazon.ags.html5.overlay.GameCircleUserInterface"));
                    gameCircleUserInterface.Add(new XAttribute(xmlns + "theme", "@style/GCOverlay"));
                    if (PlayerSettings.Android.minSdkVersion > AndroidSdkVersions.AndroidApiLevel10)
                    {
                        gameCircleUserInterface.Add(new XAttribute(xmlns + "hardwareAccelerated", "false"));
                    }

                    return gameCircleUserInterface;
                }

                case c_gameCircleAlertUserInterface:
                {
                    var gameCircleAlertUserInterface = new XElement("activity");
                    gameCircleAlertUserInterface.Add(new XAttribute(
                        xmlns + "name", "com.amazon.ags.html5.overlay.GameCircleAlertUserInterface"));
                    gameCircleAlertUserInterface.Add(new XAttribute(xmlns + "theme", "@style/GCAlert"));
                    if (PlayerSettings.Android.minSdkVersion > AndroidSdkVersions.AndroidApiLevel10)
                    {
                        gameCircleAlertUserInterface.Add(new XAttribute(xmlns + "hardwareAccelerated", "false"));
                    }

                    return gameCircleAlertUserInterface;
                }

                case c_authorizationActivity:
                {
                    var authorizationActivity = new XElement("activity");
                    authorizationActivity.Add(new XAttribute(
                        xmlns + "name", "com.amazon.identity.auth.device.authorization.AuthorizationActivity"));
                    authorizationActivity.Add(new XAttribute(xmlns + "theme", "@android:style/Theme.NoDisplay"));
                    authorizationActivity.Add(new XAttribute(xmlns + "allowTaskReparenting", "true"));
                    authorizationActivity.Add(new XAttribute(xmlns + "launchMode", "singleTask"));

                    var authorizationIntentFilter = new XElement("intent-filter");
                    authorizationIntentFilter.Add(new XElement("action", new XAttribute(xmlns + "name", "android.intent.action.VIEW")));
                    authorizationIntentFilter.Add(new XElement(
                        "category",
                        new XAttribute(xmlns + "name", "android.intent.category.DEFAULT")));
                    authorizationIntentFilter.Add(new XElement(
                        "category",
                        new XAttribute(xmlns + "name", "android.intent.category.BROWSABLE")));

                    var ifData = new XElement("data");
                    ifData.Add(new XAttribute(xmlns + "host", PlayerSettings.bundleIdentifier));
                    ifData.Add(new XAttribute(xmlns + "scheme", "amzn"));

                    authorizationIntentFilter.Add(ifData);
                    authorizationActivity.Add(authorizationIntentFilter);

                    return authorizationActivity;
                }

                case c_gameCircleApiKey:
                {
                    var apiKeyMetaData = new XElement("meta-data");
                    apiKeyMetaData.Add(new XAttribute(xmlns + "name", "APIKey"));
                    apiKeyMetaData.Add(new XAttribute(xmlns + "value", gameCircleApiKey));
                    return apiKeyMetaData;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion / Private methods
    }
}
#endif
