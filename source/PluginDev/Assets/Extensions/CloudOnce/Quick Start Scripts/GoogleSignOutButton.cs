// <copyright file="GoogleSignOutButton.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.QuickStart
{
    using CloudOnce;
    using UnityEngine;
#if UNITY_2019_1_OR_NEWER
    using UnityEngine.UIElements;
#else
    using UnityEngine.UI;
#endif

    /// <summary>
    /// Google requires that players are provided with a sign-out option:
    /// https://developers.google.com/games/services/checklist
    /// </summary>
    [AddComponentMenu("CloudOnce/Google Sign In-Out Button", 5)]
    public class GoogleSignOutButton : MonoBehaviour
    {
        private Button cachedButton;
#if UNITY_2019_1_OR_NEWER
        private TextElement textComponent;
#else
        private Text textComponent;
#endif

        private Button CachedButton
        {
            get { return cachedButton ?? (cachedButton = GetComponent<Button>()); }
        }

#if UNITY_2019_1_OR_NEWER
        private TextElement TextComponent
        {
            get { return textComponent ?? (textComponent = GetComponentInChildren<TextElement>()); }
        }
#else
        private Text TextComponent
        {
            get { return textComponent ?? (textComponent = GetComponentInChildren<Text>()); }
        }
#endif

        #region Private methods

        private void UpdateButtonText(bool isSignedIn)
        {
            TextComponent.text = isSignedIn ? "Sign out" : "Sign in";
        }

        private void Awake()
        {
            Cloud.OnSignedInChanged += UpdateButtonText;
            if (CachedButton != null)
            {
#if UNITY_2019_1_OR_NEWER
                CachedButton.clicked += OnButtonClicked;
#else
                CachedButton.onClick.AddListener(OnButtonClicked);
#endif
            }
            else
            {
                Debug.LogError("Google Sign In/Out Button script placed on GameObject that is not a button." +
                               " Script is only compatible with UI buttons created from GameObject menu (GameObjects -> UI -> Button).");
                return;
            }

            UpdateButtonText(Cloud.IsSignedIn);
        }

        private void OnButtonClicked()
        {
            if (Cloud.IsSignedIn)
            {
                // It would be wise to add some sort of confirmation pop-up here,
                // making sure the user actually wants to sign out
                Cloud.SignOut();
            }
            else
            {
                Cloud.SignIn();
            }
        }

        private void OnEnable()
        {
            UpdateButtonText(Cloud.IsSignedIn);
        }

        private void OnDestroy()
        {
            if (CachedButton != null)
            {
#if UNITY_2019_1_OR_NEWER
                CachedButton.clicked += OnButtonClicked;
#else
                CachedButton.onClick.RemoveListener(OnButtonClicked);
#endif
            }

            Cloud.OnSignedInChanged -= UpdateButtonText;
        }

#endregion /Private methods
    }
}
