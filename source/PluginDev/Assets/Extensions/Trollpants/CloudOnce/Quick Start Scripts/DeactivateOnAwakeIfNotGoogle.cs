// <copyright file="DeactivateOnAwakeIfNotGoogle.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Trollpants.CloudOnce.QuickStart
{
    using UnityEngine;

    /// <summary>
    /// Attach this to any object that should be deactivated if it's not on Google Play.
    /// For example the Google Sign out/in button.
    /// </summary>
    [AddComponentMenu("CloudOnce/Deactivate If Not Google", 6)]
    public class DeactivateOnAwakeIfNotGoogle : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool activeInEditor = true;
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            if (activeInEditor)
            {
                return;
            }
#elif !TP_AndroidGoogle
            gameObject.SetActive(false);
#endif
        }
    }
}
