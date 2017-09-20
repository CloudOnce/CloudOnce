// <copyright file="LoadSceneOnCloudOnceEvent.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.QuickStart
{
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Loads a scene when the selected CloudOnce event is called.
    /// </summary>
    [AddComponentMenu("CloudOnce/Load Scene On Event", 1)]
    public class LoadSceneOnCloudOnceEvent : MonoBehaviour
    {
        #region Fields & enums

#pragma warning disable 649
        [SerializeField]
        private CloudOnceEvent cloudOnceEvent;

        [SerializeField]
        private string sceneName;

        [SerializeField]
        private bool loadAdditive;

        [SerializeField]
        private bool loadAsync;
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
            LoadScene();
        }

        private void OnCloudLoadComplete(bool result)
        {
            LoadScene();
        }

        private void OnSignedInChanged(bool isSignedIn)
        {
            LoadScene();
        }

        private void LoadScene()
        {
            UnsubscribeEvents();
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("Scene name was empty, aborting load.");
                return;
            }

            if (loadAdditive && loadAsync)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            else if (loadAdditive && !loadAsync)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
            else if (!loadAdditive && loadAsync)
            {
                SceneManager.LoadSceneAsync(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
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
