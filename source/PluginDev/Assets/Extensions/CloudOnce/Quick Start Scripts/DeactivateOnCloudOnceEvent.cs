// <copyright file="DeactivateOnCloudOnceEvent.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.QuickStart
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Deactivate the GameObject that the scripts is attached to when the selected CloudOnce event is called.
    /// Could for example be used on a loading screen.
    /// </summary>
    [AddComponentMenu("CloudOnce/Deactivate On Event", 2)]
    public class DeactivateOnCloudOnceEvent : MonoBehaviour
    {
        #region Fields & enums

#pragma warning disable 649
        [SerializeField]
        private CloudOnceEvent cloudOnceEvent;
#pragma warning restore 649

        private enum CloudOnceEvent
        {
            OnInitializeComplete,
            OnCloudLoadComplete,
            OnSignedInChanged
        }

        #endregion /Fields & enums

        #region Unity methods

        private void Awake()
        {
            switch (cloudOnceEvent)
            {
                case CloudOnceEvent.OnInitializeComplete:
                    Cloud.OnInitializeComplete += OnInitializeComplete;
                    break;
                case CloudOnceEvent.OnCloudLoadComplete:
                    Cloud.OnCloudLoadComplete += OnCloudLoadComplete;
                    break;
                case CloudOnceEvent.OnSignedInChanged:
                    Cloud.OnSignedInChanged += OnSignedInChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion /Unity methods

        #region Private methods

        private void OnInitializeComplete()
        {
            UnsubscribeEvents();
            gameObject.SetActive(false);
        }

        private void OnCloudLoadComplete(bool result)
        {
            UnsubscribeEvents();
            gameObject.SetActive(false);
        }

        private void OnSignedInChanged(bool isSignedIn)
        {
            UnsubscribeEvents();
            gameObject.SetActive(false);
        }

        private void UnsubscribeEvents()
        {
            switch (cloudOnceEvent)
            {
                case CloudOnceEvent.OnInitializeComplete:
                    Cloud.OnInitializeComplete -= OnInitializeComplete;
                    break;
                case CloudOnceEvent.OnCloudLoadComplete:
                    Cloud.OnCloudLoadComplete -= OnCloudLoadComplete;
                    break;
                case CloudOnceEvent.OnSignedInChanged:
                    Cloud.OnSignedInChanged -= OnSignedInChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion /Private methods
    }
}
