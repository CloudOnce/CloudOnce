// <copyright file="iOSPostBuild.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
    using UnityEditor;
    using UnityEditor.Callbacks;

    /// <summary>
    /// Automatic operations called after an iOS build.
    /// </summary>
    public static class iOSPostBuild
    {
        #region Fields & properties

        private const string requiredDeviceCapabilities = "UIRequiredDeviceCapabilities";
        private const string gameKit = "gamekit";

        #endregion /Fields & properties

        #region Public methods

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS && target != BuildTarget.tvOS)
            {
                return;
            }

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
            if (string.IsNullOrEmpty(buddy.EntryValue(requiredDeviceCapabilities)))
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Adding GameKit to Info.plist.");
#endif
                buddy.AddArray(requiredDeviceCapabilities);
                buddy.AddString(PlistBuddyHelper.ToEntryName(requiredDeviceCapabilities, 0), gameKit);
                return;
            }

            // Check if GameKit already exist in UIRequiredDeviceCapabilities array
            var index = 0;
            while (buddy.EntryValue(requiredDeviceCapabilities, index) != null)
            {
                if (gameKit.Equals(buddy.EntryValue(requiredDeviceCapabilities, index)))
                {
                    // GameKit is already in UIRequiredDeviceCapabilities array, no need to add it
                    return;
                }

                index++;
            }
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Adding GameKit to Info.plist.");
#endif

            // GameKit was not detected, so we add it
            buddy.AddString(PlistBuddyHelper.ToEntryName(requiredDeviceCapabilities, 0), gameKit);
        }

        #endregion /Private methods
    }
}
#endif
