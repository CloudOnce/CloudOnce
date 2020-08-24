// <copyright file="AchievementsButton.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.QuickStart
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Attach this to your Achievements GUI button.
    /// </summary>
    [AddComponentMenu("CloudOnce/Show Achievements Button", 3)]
    public class AchievementsButton : MonoBehaviour
    {
        private Button button;

        private static void OnSignedInChanged(bool isSignedIn)
        {
            Cloud.OnSignedInChanged -= OnSignedInChanged;
            if (isSignedIn)
            {
                Cloud.Achievements.ShowOverlay();
            }
        }

        private static void SubscribeEvent()
        {
            Cloud.OnSignedInChanged -= OnSignedInChanged;
            Cloud.OnSignedInChanged += OnSignedInChanged;
        }

        private static void OnButtonClicked()
        {
            if (Cloud.IsSignedIn)
            {
                Cloud.Achievements.ShowOverlay();
            }
            else
            {
#if CLOUDONCE_DEBUG
                Debug.Log("Can't show achievements overlay, user is not signed in!");
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
                Debug.LogError("Show Achievements Button script placed on GameObject that is not a button." +
                               " Script is only compatible with UI buttons created from GameObject menu (GameObjects -> UI -> Button).");
            }
        }

        private void Start()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClicked);
            Cloud.OnSignedInChanged -= OnSignedInChanged;
        }
    }
}
