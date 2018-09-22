// <copyright file="CloudOnceEvents.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using UnityEngine;
    using UnityEngine.Events;
    using Utils;

    public class CloudOnceEvents
    {
        /// <summary>
        /// Raised once the current platforms Initialize method is done.
        /// </summary>
        public event UnityAction OnInitializeComplete;

        /// <summary>
        /// Raised after player signed in status changed. Parameter indicates whether the player was signed in or out.
        /// </summary>
        public event UnityAction<bool> OnSignedInChanged;

        /// <summary>
        /// Raised after an unsuccessful sign in attempt.
        /// </summary>
        public event UnityAction OnSignInFailed;

        /// <summary>
        /// Raised if the currently signed in player's profile picture is successfully downloaded.
        /// </summary>
        public event UnityAction<Texture2D> OnPlayerImageDownloaded;

        /// <summary>
        /// Raised after an attempt has been made to save cloud data. Parameter indicates success.
        /// </summary>
        public event UnityAction<bool> OnCloudSaveComplete;

        /// <summary>
        /// Raised after an attempt has been made to load cloud data. Parameter indicates success.
        /// </summary>
        public event UnityAction<bool> OnCloudLoadComplete;

        /// <summary>
        /// Raised if local data is changed as a result of loading cloud data. Returns a <see cref="string"/> array of the changed internal IDs.
        /// </summary>
        public event UnityAction<string[]> OnNewCloudValues;

        /// <summary>
        /// Raises the <see cref="OnInitializeComplete"/> event.
        /// </summary>
        public void RaiseOnInitializeComplete()
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnInitializeComplete");
#endif
            CloudOnceUtils.SafeInvoke(OnInitializeComplete);
        }

        /// <summary>
        /// Raises the <see cref="OnSignedInChanged"/> event.
        /// </summary>
        public void RaiseOnSignedInChanged(bool isSignedIn)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnSignedInChanged: " + (isSignedIn ? "Signed In" : "Signed Out"));
#endif
            CloudOnceUtils.SafeInvoke(OnSignedInChanged, isSignedIn);
        }

        /// <summary>
        /// Raises the <see cref="OnSignInFailed"/> event.
        /// </summary>
        public void RaiseOnSignInFailed()
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnSignInFailed");
#endif
            CloudOnceUtils.SafeInvoke(OnSignInFailed);
        }

        /// <summary>
        /// Raises the <see cref="OnPlayerImageDownloaded"/> event.
        /// </summary>
        public void RaiseOnPlayerImageDownloaded(Texture2D playerImage)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnPlayerImageDownloaded");
#endif
            CloudOnceUtils.SafeInvoke(OnPlayerImageDownloaded, playerImage);
        }

        /// <summary>
        /// Raises the <see cref="OnCloudSaveComplete"/> event.
        /// </summary>
        /// <param name="success">If the save was successful or not.</param>
        public void RaiseOnCloudSaveComplete(bool success)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnCloudSaveComplete: " + (success ? "Cloud save was successful." : "Cloud save failed."));
#endif
            CloudOnceUtils.SafeInvoke(OnCloudSaveComplete, success);
        }

        /// <summary>
        /// Raises the <see cref="OnCloudLoadComplete"/> event.
        /// </summary>
        /// <param name="success">If the load was successful or not.</param>
        public void RaiseOnCloudLoadComplete(bool success)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnCloudLoadComplete: " + (success ? "Cloud load was successful." : "Cloud load failed."));
#endif
            CloudOnceUtils.SafeInvoke(OnCloudLoadComplete, success);
        }

        /// <summary>
        /// Raises the <see cref="OnNewCloudValues"/> event.
        /// </summary>
        /// <param name="changedKeys">A <see cref="string"/> array of the changed internal IDs.</param>
        public void RaiseOnNewCloudValues(string[] changedKeys)
        {
#if CLOUDONCE_DEBUG
            Debug.Log("OnNewCloudValues: " + changedKeys.Length + " values have changed.");
#endif
            CloudOnceUtils.SafeInvoke(OnNewCloudValues, changedKeys);
        }
    }
}
