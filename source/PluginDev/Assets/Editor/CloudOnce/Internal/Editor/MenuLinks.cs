// <copyright file="MenuLinks.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    ///  Adds helpful links as menu items.
    /// </summary>
    public class MenuLinks
    {
        [MenuItem("Window/CloudOnce/Getting Started Guide", false, 50)]
        private static void MenuItemGettingStartedGuides()
        {
            Application.OpenURL("http://jizc.github.io/CloudOnce/gettingStarted.html");
        }

        [MenuItem("Window/CloudOnce/API Documentation", false, 51)]
        private static void MenuItemAipDocumentation()
        {
            Application.OpenURL("http://jizc.github.io/CloudOnce/api-docs/index.html");
        }

        [MenuItem("Window/CloudOnce/GitHub Repository", false, 100)]
        private static void MenuItemGitHubRepo()
        {
            Application.OpenURL("http://github.com/jizc/CloudOnce");
        }
    }
}
#endif
