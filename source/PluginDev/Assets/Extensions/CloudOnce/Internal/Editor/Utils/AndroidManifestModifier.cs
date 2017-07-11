// <copyright file="AndroidManifestModifier.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
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

        private const string androidManifestFileName = "/AndroidManifest.xml";
        private const string mainAndroidManifestPath = CloudOncePaths.Android + androidManifestFileName;
        private const string mainAndroidManifestTemplatePath = CloudOncePaths.Templates + "/AndroidManifestTemplate.xml";
        private const string amazonAndroidManifestPath = CloudOncePaths.GameCircleLib + androidManifestFileName;

        private const string gameCircleUserInterface = "manifest/application/activity[@android:name='com.amazon.ags.html5.overlay.GameCircleUserInterface']";
        private const string authorizationActivity = "manifest/application/activity[@android:name='com.amazon.identity.auth.device.authorization.AuthorizationActivity']";
        private const string gameCircleAlertUserInterface = "manifest/application/activity[@android:name='com.amazon.ags.html5.overlay.GameCircleAlertUserInterface']";
        private const string gameCircleApiKeyElement = "manifest/application/meta-data[@android:name='APIKey']";

        private readonly XNamespace xmlns = "http://schemas.android.com/apk/res/android";

        private readonly string[] amazonElements =
        {
            gameCircleUserInterface,
            gameCircleAlertUserInterface,
            authorizationActivity,
            gameCircleApiKeyElement
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

            manifest = XDocument.Load(amazonAndroidManifestPath);

            foreach (var element in amazonElements)
            {
                RemoveXElementFromManifest(element);
            }

            foreach (var element in amazonElements)
            {
                AddXElementToManifest(element);
            }

            manifest.Save(amazonAndroidManifestPath);
            AssetDatabase.ImportAsset(amazonAndroidManifestPath);
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
                case gameCircleUserInterface:
                {
                    var userInterface = new XElement("activity");
                    userInterface.Add(new XAttribute(xmlns + "name", "com.amazon.ags.html5.overlay.GameCircleUserInterface"));
                    userInterface.Add(new XAttribute(xmlns + "theme", "@style/GCOverlay"));
#if !UNITY_5_6_OR_NEWER
                    if (PlayerSettings.Android.minSdkVersion > AndroidSdkVersions.AndroidApiLevel10)
                    {
#endif
                        userInterface.Add(new XAttribute(xmlns + "hardwareAccelerated", "false"));
#if !UNITY_5_6_OR_NEWER
                    }
#endif
                    return userInterface;
                }

                case gameCircleAlertUserInterface:
                {
                    var alertUserInterface = new XElement("activity");
                    alertUserInterface.Add(new XAttribute(
                        xmlns + "name", "com.amazon.ags.html5.overlay.GameCircleAlertUserInterface"));
                    alertUserInterface.Add(new XAttribute(xmlns + "theme", "@style/GCAlert"));
#if !UNITY_5_6_OR_NEWER
                    if (PlayerSettings.Android.minSdkVersion > AndroidSdkVersions.AndroidApiLevel10)
                    {
#endif
                        alertUserInterface.Add(new XAttribute(xmlns + "hardwareAccelerated", "false"));
#if !UNITY_5_6_OR_NEWER
                    }
#endif
                    return alertUserInterface;
                }

                case authorizationActivity:
                {
                    var activity = new XElement("activity");
                    activity.Add(new XAttribute(
                        xmlns + "name", "com.amazon.identity.auth.device.authorization.AuthorizationActivity"));
                    activity.Add(new XAttribute(xmlns + "theme", "@android:style/Theme.NoDisplay"));
                    activity.Add(new XAttribute(xmlns + "allowTaskReparenting", "true"));
                    activity.Add(new XAttribute(xmlns + "launchMode", "singleTask"));

                    var authorizationIntentFilter = new XElement("intent-filter");
                    authorizationIntentFilter.Add(new XElement("action", new XAttribute(xmlns + "name", "android.intent.action.VIEW")));
                    authorizationIntentFilter.Add(new XElement(
                        "category",
                        new XAttribute(xmlns + "name", "android.intent.category.DEFAULT")));
                    authorizationIntentFilter.Add(new XElement(
                        "category",
                        new XAttribute(xmlns + "name", "android.intent.category.BROWSABLE")));

                    var ifData = new XElement("data");
#if UNITY_5_6_OR_NEWER
                    ifData.Add(new XAttribute(xmlns + "host", PlayerSettings.applicationIdentifier));
#else
                    ifData.Add(new XAttribute(xmlns + "host", PlayerSettings.bundleIdentifier));
#endif
                    ifData.Add(new XAttribute(xmlns + "scheme", "amzn"));

                    authorizationIntentFilter.Add(ifData);
                    activity.Add(authorizationIntentFilter);

                    return activity;
                }

                case gameCircleApiKeyElement:
                {
                    var apiKeyMetaData = new XElement("meta-data");
                    apiKeyMetaData.Add(new XAttribute(xmlns + "name", "APIKey"));
                    apiKeyMetaData.Add(new XAttribute(xmlns + "value", gameCircleApiKey ?? string.Empty));
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
