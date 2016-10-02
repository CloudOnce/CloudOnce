// <copyright file="iOSPostBuild.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    /// <summary>
    /// Automatic operations called after an iOS build.
    /// </summary>
    public static class iOSPostBuild
    {
        #region Fields & properties

        private const string c_requiredDeviceCapabilities = "UIRequiredDeviceCapabilities";
        private const string c_gameKit = "gamekit";

        #endregion /Fields & properties

        #region Public methods

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
            if (target != BuildTarget.iPhone)
            {
                return;
            }
#else
            if (target != BuildTarget.iOS)
            {
                return;
            }
#endif
            EnsureGameKitExist(pathToBuiltProject + "/Info.plist");
        }

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Updates the new project's Info.plist file to include GameKit as a required device capability.
        /// <para>We make use of the apple-provided PlistBuddy utility to edit the plist file.</para>
        /// </summary>
        /// <param name="pathToPlist">Path to plist.</param>
        private static void EnsureGameKitExist(string pathToPlist)
        {
            var buddy = PlistBuddyHelper.ForPlistFile(pathToPlist);
            if (buddy == null)
            {
                return;
            }

            // If the top-level UIRequiredDeviceCapabilities field doesn't exist, add it and the GameKit entry
            if (string.IsNullOrEmpty(buddy.EntryValue(c_requiredDeviceCapabilities)))
            {
#if CO_DEBUG
                Debug.Log("Adding GameKit to Info.plist.");
#endif
                buddy.AddArray(c_requiredDeviceCapabilities);
                buddy.AddString(PlistBuddyHelper.ToEntryName(c_requiredDeviceCapabilities, 0), c_gameKit);
                return;
            }

            // Check if GameKit already exist in UIRequiredDeviceCapabilities array
            var index = 0;
            while (buddy.EntryValue(c_requiredDeviceCapabilities, index) != null)
            {
                if (c_gameKit.Equals(buddy.EntryValue(c_requiredDeviceCapabilities, index)))
                {
                    // GameKit is already in UIRequiredDeviceCapabilities array, no need to add it
                    return;
                }

                index++;
            }
#if CO_DEBUG
            Debug.Log("Adding GameKit to Info.plist.");
#endif

            // GameKit was not detected, so we add it
            buddy.AddString(PlistBuddyHelper.ToEntryName(c_requiredDeviceCapabilities, 0), c_gameKit);
        }

        #endregion /Private methods
    }
}
#endif
