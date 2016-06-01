// <copyright file="InitializeCloudOnce.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Trollpants.CloudOnce.QuickStart
{
    using UnityEngine;

    /// <summary>
    /// Attach this anywhere in the scene you want the players to log in to the native services.
    /// </summary>
    [AddComponentMenu("CloudOnce/Initialize CloudOnce", 0)]
    public class InitializeCloudOnce : MonoBehaviour
    {
        [SerializeField]
        private bool cloudSaveEnabled = true;

        [SerializeField]
        private bool autoSignIn = true;

        [SerializeField]
        private bool autoCloudLoad = true;

        private void Start()
        {
            Cloud.Initialize(cloudSaveEnabled, autoSignIn, autoCloudLoad);
        }
    }
}
