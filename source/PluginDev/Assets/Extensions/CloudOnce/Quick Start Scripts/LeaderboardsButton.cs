// <copyright file="LeaderboardsButton.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.QuickStart
{
    using UnityEngine;
#if UNITY_2019_1_OR_NEWER
    using UnityEngine.UIElements;
#else
    using UnityEngine.UI;
#endif

    /// <summary>
    /// Attach this to your Leaderboards GUI button.
    /// </summary>
    [AddComponentMenu("CloudOnce/Show Leaderboards Button", 4)]
    public class LeaderboardsButton : MonoBehaviour
    {
        private Button button;

        private static void OnSignedInChanged(bool isSignedIn)
        {
            Cloud.OnSignedInChanged -= OnSignedInChanged;
            if (isSignedIn)
            {
                Cloud.Leaderboards.ShowOverlay();
            }
        }

        private static void SubscribeEvent()
        {
            Cloud.OnSignedInChanged -= OnSignedInChanged;
            Cloud.OnSignedInChanged += OnSignedInChanged;
        }

        private void OnButtonClicked()
        {
            if (Cloud.IsSignedIn)
            {
                Cloud.Leaderboards.ShowOverlay();
            }
            else
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Can't show leaderboard overlay, user is not signed in!");
#endif
                SubscribeEvent();
                Cloud.SignIn();
            }
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("Show Leaderboards Button script placed on GameObject that is not a button." +
                               " Script is only compatible with UI buttons created from GameObject menu (GameObjects -> UI -> Button).");
            }
        }

        private void Start()
        {
#if UNITY_2019_1_OR_NEWER
            button.clicked += OnButtonClicked;
#else
            button.onClick.AddListener(OnButtonClicked);
#endif
        }

        private void OnDestroy()
        {
#if UNITY_2019_1_OR_NEWER
            button.clicked -= OnButtonClicked;
#else
            button.onClick.RemoveListener(OnButtonClicked);
#endif
            Cloud.OnSignedInChanged -= OnSignedInChanged;
        }
    }
}
