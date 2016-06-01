// <copyright file="iCloudBridge.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_IOS
namespace Trollpants.CloudOnce.Internal
{
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    ///  Provides a bridge between C# code and the <c>CloudOnceSave</c> iOS plug-in.
    /// </summary>
    public static class iCloudBridge
    {
        // The native iOS plug-in requires that the callback GameObject is called this, do not change!
        private const string c_callbackObject = "CloudOnceCallbackObject";

        private const string c_importInternal = "__Internal";

        private static bool s_platformChecked;
        private static bool s_platformSupportsCloud;

        /// <summary>
        /// Sets up the callback GameObject so the native iOS plug-in can send <c>UnitySendMessage</c>s back to Unity.
        /// </summary>
        static iCloudBridge()
        {
            var callbackGameObject = new GameObject { name = c_callbackObject };
            callbackGameObject.AddComponent<iCloudListener>();
        }

        public delegate void ExternalChangeCallback(KvStoreChangeReason reason, string devString);

        public static ExternalChangeCallback OnExternalChange { get; set; }

        /// <summary>
        /// Stores a <see cref="string"/> in iCloud.
        /// </summary>
        /// <param name="key">The unique identifier for this <see cref="string"/>.</param>
        /// <param name="value">The <see cref="string"/> to store in iCloud.</param>
        /// <returns>
        /// Returns <c>true</c> if the <see cref="string"/> was successfully saved to iCloud, <c>false</c> if any problems happened.
        /// </returns>
        public static bool SetString(string key, string value)
        {
            var returnValue = false;
            if (PlatformSupportsCloud())
            {
                returnValue = _SetDevString(key, value);
            }

            return returnValue;
        }

        /// <summary>
        /// Gets a <see cref="string"/> from iCloud.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="string"/>.</param>
        /// <returns>The <see cref="string"/> associated with the specified key.</returns>
        public static string GetString(string key)
        {
            var devString = string.Empty;
            if (PlatformSupportsCloud())
            {
                devString = _GetDevString(key);

                if (string.IsNullOrEmpty(devString))
                {
#if CO_DEBUG
                    Debug.LogWarning("iCloud dev string was null or empty.");
#endif
                }
            }
            else
            {
#if CO_DEBUG
                Debug.LogWarning("Platform does not support iCloud. Returning string.Empty.");
#endif
            }

            return devString;
        }

        /// <summary>
        /// Deletes a <see cref="string"/> from iCloud.
        /// </summary>
        /// <param name="key">The unique identifier for the <see cref="string"/>.</param>
        /// <returns>
        /// Returns <c>true</c> if the <see cref="string"/> was successfully deleted from iCloud, <c>false</c> if any problems happened.
        /// </returns>
        public static bool DeleteString(string key)
        {
            var returnValue = false;
            if (PlatformSupportsCloud())
            {
                returnValue = _DeleteDevString(key);
            }

            return returnValue;
        }

        /// <summary>
        /// Checks if the current platform supports iCloud.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the current device supports iCloud, <c>false</c> if not.
        /// When in the Unity editor this will always return <c>true</c>.
        /// </returns>
        private static bool PlatformSupportsCloud()
        {
            if (!s_platformChecked)
            {
#if !UNITY_EDITOR
                s_platformSupportsCloud = (SystemInfo.operatingSystem.Contains("iPhone OS")
                                         && !SystemInfo.operatingSystem.Contains("iPhone OS 3")
                                         && !SystemInfo.operatingSystem.Contains("iPhone OS 4"));
#else
                s_platformSupportsCloud = true;
#endif
                s_platformChecked = true;
            }

            return s_platformSupportsCloud;
        }

        [DllImport(c_importInternal)]
        private static extern bool _SetDevString(string key, string value);

        [DllImport(c_importInternal)]
        private static extern string _GetDevString(string key);

        [DllImport(c_importInternal)]
        private static extern bool _DeleteDevString(string key);
    }
}
#endif
