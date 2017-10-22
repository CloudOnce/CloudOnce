// <copyright file="iCloudListener.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_IOS || UNITY_TVOS
namespace CloudOnce.Internal
{
    using System;
    using UnityEngine;

    /// <summary>
    ///  Used to listen for messages from the <c>CloudOnceSave</c> iOS plug-in.
    /// </summary>
    public class iCloudListener : MonoBehaviour
    {
        #region Unity methods

        /// <summary>
        /// Make sure that the object does not get destroyed if the scene changes.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        #endregion /Unity methods

        #region Private methods

        /// <summary>
        /// Callback method for <c>CloudOnceCloudSave</c> iOS plug-in. Name MUST be "ExternalChange".
        /// </summary>
        /// <param name="message">The callback string.</param>
        private void ExternalChange(string message)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("Received data from iCloud: " + message);
#endif
            var data = message.Split('|');
            var reason = (KvStoreChangeReason)Enum.Parse(typeof(KvStoreChangeReason), data[0]);
            var devString = string.Empty;
            for (var i = 1; i < data.Length; i++)
            {
                if (data[i].StartsWith(DataManager.DevStringKey))
                {
                    var keyValuePair = data[i].Split('.');
                    devString = keyValuePair[1];
                    break;
                }
            }

            if (iCloudBridge.OnExternalChange != null && !string.IsNullOrEmpty(devString))
            {
                iCloudBridge.OnExternalChange(reason, devString);
            }
        }

        #endregion / Private methods
    }
}
#endif
