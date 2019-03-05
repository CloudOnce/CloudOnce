// <copyright file="CloudOnceEditor.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Data;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    /// <summary>
    /// Editor window for setting CloudOnce configuration
    /// </summary>
    public class CloudOnceEditor : EditorWindow
    {
        #region Fields

        #region Consts

        #region Strings

        private const string

            // Folders
            fontsFolder = "/Fonts/",
            imagesFolder = "/Images/",

            // Fonts
            walkwayFontPath = fontsFolder + "Walkway-Bold.ttf",
            robotoSlabFontPath = fontsFolder + "RobotoSlab-Regular.ttf",
            robotoFontPath = fontsFolder + "Roboto-Regular.ttf",

            // Icons
            cloudOnceIconBlackPath = imagesFolder + "CloudOnceIconBlack.png",
            cloudOnceIconWhitePath = imagesFolder + "CloudOnceIconWhite.png",
            debugIconPath = imagesFolder + "DebugIcon.png",
            debugIconDarkPath = imagesFolder + "DebugIconDark.png",
            gameCenterIconPath = imagesFolder + "GameCenterIcon.png",
            playGamesIconPath = imagesFolder + "PlayGamesIcon.png",
            gameCenterDarkIconPath = imagesFolder + "GameCenterIconDark.png",
            playGamesDarkIconPath = imagesFolder + "PlayGamesIconDark.png",
            cloudVariableIconPath = imagesFolder + "CloudVariableIcon.png",
            cloudVariableIconDarkPath = imagesFolder + "CloudVariableIconDark.png",
            saveIconPath = imagesFolder + "SaveIcon.png",
            saveDarkIconPath = imagesFolder + "SaveIconDark.png",
            revertIconPath = imagesFolder + "RevertIcon.png",
            revertDarkIconPath = imagesFolder + "RevertIconDark.png",
            cloudOnceIconPath = imagesFolder + "CloudOnceIcon.png",
            cloudOnceIconDarkPath = imagesFolder + "CloudOnceIconDark.png",
            settingsIconPath = imagesFolder + "SettingsIcon.png",
            settingsIconDarkPath = imagesFolder + "SettingsIconDark.png",

            // Images
            logoPath = imagesFolder + "cloudonce-logo.png",
            whiteXSpritePath = imagesFolder + "whiteX.png",
            plusSpritePath = imagesFolder + "plus.png",
            toggleOnSpritePath = imagesFolder + "toggle_on.png",
            toggleOffSpritePath = imagesFolder + "toggle_off.png",
            toggleOffProSpritePath = imagesFolder + "toggle_off_pro.png",
            gameCenterOnSpritePath = imagesFolder + "ios-game-center.png",
            playGamesOnSpritePath = imagesFolder + "google-play-games.png",
            gameCenterOffSpritePath = imagesFolder + "ios-game-center-off.png",
            playGamesOffSpritePath = imagesFolder + "google-play-games-off.png",
            gameCenterOffDarkSpritePath = imagesFolder + "ios-game-center-off_Dark.png",
            playGamesOffDarkSpritePath = imagesFolder + "google-play-games-off_Dark.png",
            freeAchievementsSpritePath = imagesFolder + "achievements_lightTheme.png",
            proAchievementsSpritePath = imagesFolder + "achievements_darkTheme.png",
            freeLeaderboardsSpritePath = imagesFolder + "leaderboards_lightTheme.png",
            proLeaderboardsSpritePath = imagesFolder + "leaderboards_darkTheme.png",

            // Platform names
            internalName = " Internal",
            gameCenterName = "Apple Game Center",
            gameCenterNameSplit = "Apple\nGame Center",
            gameCenterNameShort = " Game Center",
            playGamesName = "Google Play Game Services",
            playGamesNameSplit = "Google Play\nGame Services",
            playGamesNameShort = " Game Services";

        #endregion /Strings

        #region Numbers

        private const int
            tinyFontSize = 12,
            platformButtonFontSize = 13,
            largeFontSize = 18,
            cloudOnceLogoFontSize = 42;

        private const float
            borderPaddingSmall = 2f,
            borderPadding = 5f,
            slimControlHeight = 16f,
            idPlatformLabelWidth = 102f,
            minimumWindowHeight = 360f,
            minimumWindowWidth = 375f,
            contentColumnWidth = minimumWindowWidth - indentationWidth - scrollbarWidth,
            idPanelWidth = contentColumnWidth - helpBoxMargin,
            platformButtonWidth = (contentColumnWidth - helpBoxMargin) / 3f,
            platformButtonHeight = platformButtonWidth * 1.15f,
            indentationWidth = 15f,
            scrollbarWidth = 15f,
            helpBoxMargin = 4f,
            footerButtonHeight = 35f;

        #endregion /Numbers

        #endregion /Consts

        private static string s_editorScriptPath;

        #region readonly fields

        private readonly GUIContent
            disabledSaveButtonContent = new GUIContent("Above warnings must be fixed\nbefore you can save the configuration."),
            cloudVariableKeyContent = new GUIContent("Internal ID"),
            cloudVariableTypeContent = new GUIContent("Variable Type"),
            cloudVariableDefaultValueContent = new GUIContent("Starting Value"),
            cloudVariableAllowNegativeContent = new GUIContent("Allow Negative"),
            cloudVariablePersistenceTypeContent = new GUIContent("Persistence Type");

        private readonly Color32
            addButtonColor = new Color32(133, 195, 0, 255),
            addButtonProColor = new Color32(40, 140, 40, 255),
            deleteButtonColor = new Color32(231, 76, 60, 255),
            deleteButtonProColor = new Color32(175, 45, 45, 255),
            achievementLightThemeColor = new Color32(90, 200, 255, 175),
            leaderboardLightThemeColor = new Color32(255, 140, 20, 175),
            cloudVariableLightThemeColor = new Color32(136, 228, 28, 150),
            achievementDarkThemeColor = new Color32(0, 192, 255, 75),
            leaderboardDarkThemeColor = new Color32(255, 100, 0, 75),
            cloudVariableDarkThemeColor = new Color32(0, 255, 192, 60);

        private readonly Color
            defaultDarkThemeTextColor = new Color(0.75f, 0.75f, 0.75f),
            headerDarkThemeColor = new Color(0.95f, 0.95f, 0.95f);

        #endregion /readonly fields

        #region GUIStyles & GUIContents

        private GUIStyle
            logoStyle,
            tinyLabelStyle,
            headerLabelStyle,
            cloudOnceLogoLabelStyle,
            addButtonStyle,
            deleteButtonStyle,
            idTitleStyle,
            idIconStyle,
            helpIconStyle,
            headerBackgroundStyle,
            footerBackgroundStyle,
            platformToggleLeftStyle,
            platformToggleRightStyle,
            debugToggleStyle,
            cloudVariableToggleStyle,
            versionLabelStyle,
            helpBoxStyle;

        private GUIContent
            addAchievementButtonContent,
            addLeaderboardButtonContent,
            addCloudVariableButtonContent,
            deleteButtonContent,
            gameCenterOnButtonContent,
            playGamesOnButtonContent,
            gameCenterOffButtonContent,
            playGamesOffButtonContent,
            internalIdAllowedCharactersTipContent,
            appleIdAllowedCharactersTipContent,
            googleIdAllowedCharactersTipContent,
            achievementIDsContent,
            leaderboardIDsContent,
            internalPlatformIDContent,
            gameCenterPlatformIDContent,
            playGamesPlatformIDContent,
            supportedPlatformsContent,
            debugLabelContent,
            googleAppIdContent,
            cloudVariablesHeaderContent,
            cloudVariableKeyTipContent,
            cloudVariableTypeTipContent,
            cloudVariableDefaultValueTipContent,
            cloudVariablePersistenceTypeTipContent,
            cloudVariableAllowNegativeTipContent,
            saveButtonContent,
            revertButtonContent,
            settingsContent;

        #endregion /GUIStyles & GUIContents

        #region Resources

        private Font walkwayFont, robotoSlabFont, robotoFont;

        private Texture2D
            whiteXSprite,
            plusSprite,
            logoSprite,
            achievementsSprite,
            leaderboardsSprite,
            gameCenterOnSprite,
            playGamesOnSprite,
            gameCenterOffSprite,
            playGamesOffSprite,
            cloudOnceIcon,
            gameCenterIcon,
            playGamesIcon,
            toggleOnSprite,
            toggleOffSprite,
            helpIcon,
            debugIcon,
            cloudVariableIcon,
            saveIcon,
            revertIcon,
            settingsIcon;

        #endregion /Resources

        private string[] toolbarStrings;

        [SerializeField]
        private CloudConfig tmpConfig;

        [SerializeField]
        private CloudConfig savedConfig;

        private int selectedTab, contentColumnCount;
        private Vector2 scrollPosition, apiKeyScrollPosition;
        private Color backgroundColorCache;
        private bool triggerRepaint;
        private bool isProSkin;

        // booleans used to validate the configuration
        private bool
            noErrorsFoundInPlatformSettings,
            noDuplicateAchievementIDs,
            noDuplicateLeaderboardIDs,
            noDuplicateCloudVariableKeys,
            noInvalidDefaultValues,
            noEmptyInternalIdFields,
            noEmptyPlatformIdFields,
            noReservedIdsUsed;

        #endregion /Fields

        private enum CloudIdType
        {
            Achievement,
            Leaderboard
        }

        private string EditorPath
        {
            get
            {
                if (!string.IsNullOrEmpty(s_editorScriptPath))
                {
                    return s_editorScriptPath;
                }

                return s_editorScriptPath =
                    Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
            }
        }

        // ReSharper disable once UnusedMember.Local
        [MenuItem("Window/CloudOnce/Editor", false, 1)]
        private static void ShowCloudConfigWindow()
        {
            // Get existing open window or if none, make a new one
            var window = GetWindow<CloudOnceEditor>();
            window.minSize = new Vector2(minimumWindowWidth, minimumWindowHeight);
        }

        #region Utility methods

        /// <summary>
        /// Attempts to separate words in a <see cref="string"/> and capitalize the first letter of each word.
        /// The <see cref="string"/> can be using lower or upper camel case, hyphens or underscores.
        /// It also attempts to preserve acronyms.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to change.</param>
        /// <returns>A <see cref="string"/> with separated words.</returns>
        private static string SplitCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var s = HyphensAndUnderscoresToCamelCase(str);
            s = Regex.Replace(s, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            if (s.Length > 1)
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }

            return char.ToUpper(s[0]).ToString();
        }

        /// <summary>
        /// Converts a <see cref="string"/> that uses underscores or hyphens to separate words into upper camel case.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to convert to upper camel case.</param>
        /// <returns>A upper camel case <see cref="string"/>.</returns>
        private static string HyphensAndUnderscoresToCamelCase(string str)
        {
            var returnString = str;
            RemoveCharAndSetNextCharToUpper(ref returnString, '-');
            RemoveCharAndSetNextCharToUpper(ref returnString, '_');
            return returnString;
        }

        /// <summary>
        /// Removes all instances of the specified <see cref="char"/> in a <see cref="string"/>
        /// and sets the next <see cref="char"/> in the <see cref="string"/> to upper.
        /// </summary>
        /// <param name="str">The <see cref="string"/> containing the <see cref="char"/>.</param>
        /// <param name="offendingChar">The <see cref="char"/> to remove.</param>
        private static void RemoveCharAndSetNextCharToUpper(ref string str, char offendingChar)
        {
            while (str.Contains(offendingChar.ToString()))
            {
                var index = str.IndexOf(offendingChar.ToString(), StringComparison.Ordinal);
                str = str.Remove(index, 1);
                if (index > 0 && index < str.Length - 1)
                {
                    str = str.Substring(0, index) + char.ToUpper(str[index]) +
                                   str.Substring(index + 1, str.Length - index - 1);
                }
            }
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from a specified path.
        /// </summary>
        /// <param name="path">The path where the <see cref="Texture2D"/> is located.</param>
        /// <returns>A <see cref="Texture2D"/> from a specified path.</returns>
        private Texture2D GetTexture2D(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(GPGSUtil.SlashesToPlatformSeparator(EditorPath + path));
        }

        /// <summary>
        /// Loads a <see cref="Font"/> from a specified path.
        /// </summary>
        /// <param name="path">The path where the <see cref="Font"/> is located.</param>
        /// <returns>A <see cref="Font"/> from a specified path.</returns>
        private Font GetFont(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Font>(GPGSUtil.SlashesToPlatformSeparator(EditorPath + path));
        }

        #endregion /Utility methods

        #region Unity methods

        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
            // Initialize GUI elements
            Initialize();

            // Loads configuration from file, if none exists it will create a new instance
            LoadConfiguration();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            // Cache the default GUI background color
            backgroundColorCache = GUI.backgroundColor;

            // Mark tabs that contain unsaved changes
            UpdateToolbarStrings();

            // Draw title, logo and toolbar
            DrawHeader();

            // Draw scroll area, affected by what tab is selected
            DrawContentArea();

            // Draw help box and save button
            DrawFooter();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnInspectorUpdate()
        {
            if (triggerRepaint)
            {
                Repaint();
            }

            if (EditorApplication.isCompiling || Application.isPlaying)
            {
                triggerRepaint = true;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (AllSettingsReadyForSave()
                && (!savedConfig.EqualsSettings(tmpConfig) || !savedConfig.EqualsCloudIDs(tmpConfig) || !savedConfig.EqualsCloudVariables(tmpConfig)))
            {
                if (EditorUtility.DisplayDialog(
                    "CloudOnce Configuration Has Been Modified",
                    "Do you want to save the changes you made to the CloudOnce configuration?\n\nYour changes will be lost if you do not save them now.",
                    "Save",
                    "Discard changes"))
                {
                    SaveConfiguration();
                }
            }
        }

        #endregion /Unity methods

        #region Private methods

        /// <summary>
        /// Sets the toolbar strings to reflect if changes has been made to each tab.
        /// </summary>
        private void UpdateToolbarStrings()
        {
            toolbarStrings = new[]
            {
                savedConfig.EqualsSettings(tmpConfig) ? "Settings" : "Settings*",
                savedConfig.EqualsCloudIDs(tmpConfig) ? "Cloud IDs" : "Cloud IDs*",
                savedConfig.EqualsCloudVariables(tmpConfig) ? "Cloud Variables" : "Cloud Variables*"
            };
        }

        /// <summary>
        /// Initializes GUI elements.
        /// </summary>
        private void Initialize()
        {
            EditorApplication.modifierKeysChanged -= Repaint;
            EditorApplication.modifierKeysChanged += Repaint;

            isProSkin = EditorGUIUtility.isProSkin;

            // Window title and icon
            var iconSprite = GetTexture2D(EditorGUIUtility.isProSkin ? cloudOnceIconDarkPath : cloudOnceIconPath);
            titleContent = new GUIContent("CloudOnce", iconSprite);

            // Fonts
            walkwayFont = GetFont(walkwayFontPath);
            robotoSlabFont = GetFont(robotoSlabFontPath);
            robotoFont = GetFont(robotoFontPath);

            // Textures
            whiteXSprite = GetTexture2D(whiteXSpritePath);
            plusSprite = GetTexture2D(plusSpritePath);
            logoSprite = GetTexture2D(logoPath);
            achievementsSprite = GetTexture2D(isProSkin ? proAchievementsSpritePath : freeAchievementsSpritePath);
            leaderboardsSprite = GetTexture2D(isProSkin ? proLeaderboardsSpritePath : freeLeaderboardsSpritePath);
            gameCenterOnSprite = GetTexture2D(gameCenterOnSpritePath);
            playGamesOnSprite = GetTexture2D(playGamesOnSpritePath);
            gameCenterOffSprite = GetTexture2D(isProSkin ? gameCenterOffDarkSpritePath : gameCenterOffSpritePath);
            playGamesOffSprite = GetTexture2D(isProSkin ? playGamesOffDarkSpritePath : playGamesOffSpritePath);
            cloudOnceIcon = GetTexture2D(isProSkin ? cloudOnceIconWhitePath : cloudOnceIconBlackPath);
            gameCenterIcon = GetTexture2D(isProSkin ? gameCenterDarkIconPath : gameCenterIconPath);
            playGamesIcon = GetTexture2D(isProSkin ? playGamesDarkIconPath : playGamesIconPath);
            toggleOnSprite = GetTexture2D(toggleOnSpritePath);
            toggleOffSprite = GetTexture2D(isProSkin ? toggleOffProSpritePath : toggleOffSpritePath);
            debugIcon = GetTexture2D(isProSkin ? debugIconDarkPath : debugIconPath);
            cloudVariableIcon = GetTexture2D(isProSkin ? cloudVariableIconDarkPath : cloudVariableIconPath);
            saveIcon = GetTexture2D(isProSkin ? saveDarkIconPath : saveIconPath);
            revertIcon = GetTexture2D(isProSkin ? revertDarkIconPath : revertIconPath);
            settingsIcon = GetTexture2D(isProSkin ? settingsIconDarkPath : settingsIconPath);

            var inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            // Editor header styles
            cloudOnceLogoLabelStyle = new GUIStyle
            {
                font = walkwayFont,
                fontSize = cloudOnceLogoFontSize,
                normal = new GUIStyleState { textColor = isProSkin ? headerDarkThemeColor : Color.black }
            };
            logoStyle = new GUIStyle
            {
                fixedHeight = cloudOnceLogoFontSize + 2,
                fixedWidth = (int)((cloudOnceLogoFontSize + 2) * 1.18f),
                contentOffset = new Vector2(0f, 4f)
            };
            versionLabelStyle = new GUIStyle
            {
                font = robotoFont,
                fontSize = tinyFontSize,
                normal = new GUIStyleState { textColor = isProSkin ? headerDarkThemeColor : Color.black }
            };

            // General styles
            tinyLabelStyle = new GUIStyle
            {
                font = robotoFont,
                fontSize = tinyFontSize,
                normal = new GUIStyleState { textColor = isProSkin ? defaultDarkThemeTextColor : Color.black }
            };
            headerLabelStyle = new GUIStyle
            {
                font = robotoSlabFont,
                fontSize = 20,
                normal = new GUIStyleState { textColor = isProSkin ? headerDarkThemeColor : Color.black },
                fixedHeight = 22f,
                contentOffset = new Vector2(7f, 0f)
            };
            idTitleStyle = new GUIStyle
            {
                font = robotoSlabFont,
                fontSize = largeFontSize,
                normal = new GUIStyleState { textColor = isProSkin ? headerDarkThemeColor : Color.black },
                clipping = TextClipping.Clip,
                contentOffset = new Vector2(0f, -4f),
                fixedHeight = 23f
            };
            idIconStyle = new GUIStyle
            {
                fixedWidth = 20f,
                fixedHeight = 20f,
                contentOffset = new Vector2(1f, 1f)
            };
            helpIconStyle = new GUIStyle
            {
                fixedWidth = 13f,
                fixedHeight = 13f,
                contentOffset = new Vector2(-1f, 3f)
            };
            addButtonStyle = new GUIStyle(inspectorSkin.button)
            {
                fixedWidth = 23f,
                fixedHeight = 21f,
                padding = new RectOffset(4, 4, 4, 4),
                margin = new RectOffset(10, 0, 3, 0)
            };
            deleteButtonStyle = new GUIStyle(inspectorSkin.FindStyle("minibutton"))
            {
                fixedWidth = 18f,
                fixedHeight = 16f,
                margin = new RectOffset(0, 0, 1, 0)
            };
            helpBoxStyle = new GUIStyle(inspectorSkin.FindStyle("HelpBox"));
            debugToggleStyle = new GUIStyle
            {
                fixedWidth = 50f,
                fixedHeight = 20f,
                margin = new RectOffset(4, 4, 4, 4)
            };
            cloudVariableToggleStyle = new GUIStyle
            {
                fixedWidth = 32.5f,
                fixedHeight = 13f,
                contentOffset = new Vector2(0f, 1f),
                margin = new RectOffset(4, 4, 2, 2)
            };

            if (isProSkin)
            {
                inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            }

            headerBackgroundStyle = new GUIStyle(inspectorSkin.FindStyle("Toolbar"))
            {
                fixedHeight = 0f
            };
            footerBackgroundStyle = new GUIStyle(inspectorSkin.FindStyle("CN Box"))
            {
                stretchHeight = false
            };

            // General content
            deleteButtonContent = new GUIContent(whiteXSprite, "Delete");

            helpIcon = (Texture2D)EditorGUIUtility.IconContent("_Help").image;
            saveButtonContent = new GUIContent(" Save configuration", saveIcon, "Saves the configuration to a txt file");
            revertButtonContent = new GUIContent(" Discard changes", revertIcon, "Discards all unsaved changes made to the configuration");

            // Settings content
            gameCenterOnButtonContent = new GUIContent(gameCenterNameSplit, gameCenterOnSprite, "Deactivate support for " + gameCenterName);
            playGamesOnButtonContent = new GUIContent(playGamesNameSplit, playGamesOnSprite, "Deactivate support for " + playGamesName);
            gameCenterOffButtonContent = new GUIContent(gameCenterNameSplit, gameCenterOffSprite, "Activate support for " + gameCenterName);
            playGamesOffButtonContent = new GUIContent(playGamesNameSplit, playGamesOffSprite, "Activate support for " + playGamesName);
            supportedPlatformsContent = new GUIContent(" Supported Platforms", cloudOnceIcon);
            debugLabelContent = new GUIContent(" Debug Mode", debugIcon);
            googleAppIdContent = new GUIContent(GPGSStrings.Setup.AppId, playGamesIcon);
            settingsContent = new GUIContent(" Settings Location", settingsIcon);

            // Cloud IDs content
            addAchievementButtonContent = new GUIContent(plusSprite, "Add an achievement to the list");
            addLeaderboardButtonContent = new GUIContent(plusSprite, "Add a leaderboard to the list");
            internalIdAllowedCharactersTipContent = new GUIContent(
                helpIcon,
                "Internal IDs must start with a letter or an underscore. They can contain alphanumeric characters (A-Z, a-z, 0-9), hyphens (-) and underscores (_).");
            appleIdAllowedCharactersTipContent = new GUIContent(
                helpIcon,
                "Game Center IDs can contain alphanumeric characters (A-Z, a-z, 0-9), periods (.) and underscores (_).");
            googleIdAllowedCharactersTipContent = new GUIContent(
                helpIcon,
                "Play Games IDs can contain alphanumeric characters (A-Z, a-z, 0-9), hyphens (-) and underscores (_).");
            achievementIDsContent = new GUIContent("Achievement IDs", achievementsSprite);
            leaderboardIDsContent = new GUIContent(" Leaderboard IDs", leaderboardsSprite);
            internalPlatformIDContent = new GUIContent(internalName, cloudOnceIcon);
            gameCenterPlatformIDContent = new GUIContent(gameCenterNameShort, gameCenterIcon);
            playGamesPlatformIDContent = new GUIContent(playGamesNameShort, playGamesIcon);

            // Cloud Variables content
            addCloudVariableButtonContent = new GUIContent(plusSprite, "Add a cloud variable to the list");
            cloudVariablesHeaderContent = new GUIContent(" Cloud Variables", cloudVariableIcon);
            cloudVariableKeyTipContent = new GUIContent(helpIcon, "A unique identifier used to identify this particular value.");
            cloudVariableTypeTipContent = new GUIContent(helpIcon, "Type of Cloud variable.");
            cloudVariableDefaultValueTipContent = new GUIContent(helpIcon, "The starting value for this variable.");
            cloudVariablePersistenceTypeTipContent = new GUIContent(helpIcon, "The method of conflict resolution to be used in case of a data conflict.");
            cloudVariableAllowNegativeTipContent = new GUIContent(helpIcon, "If the value of this currency is allowed to be negative.");
        }

        /// <summary>
        /// Draws the editors header. Including a title, version number, logo and toolbar.
        /// </summary>
        private void DrawHeader()
        {
            // Background style
            GUI.Box(new Rect(0f, 0f, position.width, 69f), string.Empty, headerBackgroundStyle);

            // Padding
            EditorGUILayout.BeginVertical();
            GUILayout.Space(borderPaddingSmall);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPadding);

            // Title
            EditorGUILayout.LabelField("CloudOnce", cloudOnceLogoLabelStyle);

            // Version number
            EditorGUILayout.BeginVertical();
            GUILayout.Space(28f);
            EditorGUILayout.LabelField("v" + PluginVersion.VersionString, versionLabelStyle, GUILayout.MaxWidth(30f));
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            // Logo
            GUILayout.Box(logoSprite, logoStyle);
            GUILayout.Space(borderPadding);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            // Disable all controls while editor is compiling or is in Play mode. Disabled group is ended in footer method.
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling || Application.isPlaying);

            // Toolbar
            selectedTab = GUILayout.Toolbar(selectedTab, toolbarStrings);
        }

        #region Content Area

        /// <summary>
        /// Draws the contents for the selected tab in a scroll view.
        /// </summary>
        private void DrawContentArea()
        {
            contentColumnCount = Mathf.Clamp((int)((position.width - scrollbarWidth - indentationWidth) / contentColumnWidth), 1, int.MaxValue);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginVertical();
            GUILayout.Space(borderPadding);
            switch (selectedTab)
            {
                default:
                    DrawSettingsTab();
                    break;
                case 1:
                    DrawCloudIDsTab();
                    break;
                case 2:
                    DrawCloudVariablesTab();
                    break;
            }

            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        #region Settings

        /// <summary>
        /// Draws the contents of the settings tab.
        /// </summary>
        private void DrawSettingsTab()
        {
            InitializeSettingsGUI();
            if (contentColumnCount > 1)
            {
                EditorGUILayout.BeginHorizontal();
            }

            EditorGUILayout.BeginVertical(GUILayout.Width(contentColumnWidth + 10f));

            DrawSupportedPlatforms();
            DrawDebugMode();

            EditorGUILayout.EndVertical();
            if (contentColumnCount > 1)
            {
                GUILayout.Space(borderPaddingSmall);
            }

            EditorGUILayout.BeginVertical(GUILayout.Width(contentColumnWidth + 11f));

            DrawGooglePlayGamesSetup();
            DrawSettingsLocation();

            EditorGUILayout.EndVertical();

            if (contentColumnCount > 1)
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws the supported platforms section.
        /// </summary>
        private void DrawSupportedPlatforms()
        {
            DrawSettingsHeader(supportedPlatformsContent);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentationWidth);

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Toggle on the platforms you want to support.");

            EditorGUILayout.BeginHorizontal();
            Undo.RecordObject(tmpConfig, "Toggle Apple Support");
            tmpConfig.AppleSupported = GUILayout.Toggle(tmpConfig.AppleSupported, tmpConfig.AppleSupported ? gameCenterOnButtonContent : gameCenterOffButtonContent, platformToggleLeftStyle);
            Undo.RecordObject(tmpConfig, "Toggle Google Support");
            tmpConfig.GoogleSupported = GUILayout.Toggle(tmpConfig.GoogleSupported, tmpConfig.GoogleSupported ? playGamesOnButtonContent : playGamesOffButtonContent, platformToggleRightStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws the debug mode section.
        /// </summary>
        private void DrawDebugMode()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(debugLabelContent, headerLabelStyle, GUILayout.Width(160f));
            if (GUILayout.Button(tmpConfig.DebugModeEnabled ? toggleOnSprite : toggleOffSprite, debugToggleStyle))
            {
                Undo.RecordObject(tmpConfig, "Toggle Debug Mode");
                tmpConfig.DebugModeEnabled = !tmpConfig.DebugModeEnabled;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentationWidth);
            GUILayout.Label("Activating the debug mode will enable debug logs. Recommended for debugging," +
                                    " but should be disabled when building production builds.");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws the controls for running the Google Play Games setup. An application ID is needed
        /// from the Developer Console to complete the setup.
        /// </summary>
        private void DrawGooglePlayGamesSetup()
        {
            EditorGUI.BeginDisabledGroup(!tmpConfig.GoogleSupported);
            DrawSettingsHeader(googleAppIdContent);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentationWidth);
            EditorGUILayout.BeginVertical();
            GUILayout.Label(GPGSStrings.Setup.AppIdBlurb);
            EditorGUILayout.BeginHorizontal();
            Undo.RecordObject(tmpConfig, "Set Google App ID");
            tmpConfig.GoogleAppID = EditorGUILayout.TextField(tmpConfig.GoogleAppID);
            GUI.enabled = !string.IsNullOrEmpty(tmpConfig.GoogleAppID);
            if (GUILayout.Button(GPGSStrings.Setup.SetupButton, EditorStyles.miniButton))
            {
                GUI.enabled = true;
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                {
                    var message = string.Format(
                            "Can only run setup when Platform in Build Settings (Ctrl+Shift+B) is set to Android. " +
                            "It is currently set to {0}. Please change it and run setup again.",
                            EditorUserBuildSettings.activeBuildTarget);
                    EditorUtility.DisplayDialog("Google Application ID setup", message, "OK");
                }
                else
                {
                    tmpConfig.GoogleSetupRun = GPGAndroidSetup.DoSetup(tmpConfig.GoogleAppID);
                    GUIUtility.ExitGUI();
                }
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawSettingsLocation()
        {
            EditorGUILayout.Space();
            DrawSettingsHeader(settingsContent);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentationWidth);
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Select where CloudOnce should save it's settings.");
            Undo.RecordObject(tmpConfig, "Set Settings Location");
            tmpConfig.SettingsLocation = (SettingsLocation)EditorGUILayout.EnumPopup(tmpConfig.SettingsLocation, GUILayout.Width(140f));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws the a settings header.
        /// </summary>
        /// <param name="label">The contents for the header.</param>
        private void DrawSettingsHeader(GUIContent label)
        {
            GUILayout.Label(label, headerLabelStyle);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Initializes the Settings GUI.
        /// </summary>
        private void InitializeSettingsGUI()
        {
            GUI.skin.label.wordWrap = true;
            EditorStyles.boldLabel.wordWrap = true;
            var padding = new RectOffset(0, 0, 5, 3);

            platformToggleLeftStyle = new GUIStyle(GUI.skin.FindStyle("ButtonLeft"))
            {
                fixedWidth = platformButtonWidth,
                fixedHeight = platformButtonHeight,
                imagePosition = ImagePosition.ImageAbove,
                font = robotoFont,
                fontSize = platformButtonFontSize,
                padding = padding
            };

            platformToggleRightStyle = new GUIStyle(GUI.skin.FindStyle("ButtonRight"))
            {
                fixedWidth = platformButtonWidth,
                fixedHeight = platformButtonHeight,
                imagePosition = ImagePosition.ImageAbove,
                font = robotoFont,
                fontSize = platformButtonFontSize,
                padding = padding
            };
        }

        #endregion /Settings

        #region CloudIDs

        /// <summary>
        /// Draws the contents of the Cloud IDs tab.
        /// </summary>
        private void DrawCloudIDsTab()
        {
            DrawIDTypeHeader(CloudIdType.Achievement);
            DrawIDs(CloudIdType.Achievement);

            DrawIDTypeHeader(CloudIdType.Leaderboard);
            DrawIDs(CloudIdType.Leaderboard);
        }

        /// <summary>
        /// Draws the achievement ID header or the leaderboard ID header and the respective add new button.
        /// </summary>
        /// <param name="cloudIdType">If the achievement ID header or the leaderboard ID header should be drawn.</param>
        private void DrawIDTypeHeader(CloudIdType cloudIdType)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(
                cloudIdType == CloudIdType.Achievement ? achievementIDsContent : leaderboardIDsContent,
                headerLabelStyle,
                GUILayout.ExpandWidth(false));
            GUI.backgroundColor = isProSkin ? addButtonProColor : addButtonColor;
            if (GUILayout.Button(
                cloudIdType == CloudIdType.Achievement ? addAchievementButtonContent : addLeaderboardButtonContent,
                addButtonStyle))
            {
                if (cloudIdType == CloudIdType.Achievement)
                {
                    Undo.RecordObject(tmpConfig, "Add new achievement");
                    tmpConfig.AchievementIDs.Add(new PlatformIdData());
                }
                else
                {
                    Undo.RecordObject(tmpConfig, "Add new leaderboard");
                    tmpConfig.LeaderboardIDs.Add(new PlatformIdData());
                }

                GUI.FocusControl(null);
            }

            GUI.backgroundColor = backgroundColorCache;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws all achievement ID or all leaderboard IDs in a grid.
        /// </summary>
        /// <param name="cloudIdType">If the achievement IDs or the leaderboard ID should be drawn.</param>
        private void DrawIDs(CloudIdType cloudIdType)
        {
            if (cloudIdType == CloudIdType.Achievement && tmpConfig.AchievementIDs.Count < 1)
            {
                EditorGUILayout.Space();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentationWidth);
            var contentRows = ((cloudIdType == CloudIdType.Achievement ? tmpConfig.AchievementIDs.Count : tmpConfig.LeaderboardIDs.Count) / contentColumnCount) + 1;
            for (var i = 0; i < contentColumnCount; i++)
            {
                if (i >= (cloudIdType == CloudIdType.Achievement ? tmpConfig.AchievementIDs.Count : tmpConfig.LeaderboardIDs.Count))
                {
                    break;
                }

                EditorGUILayout.BeginVertical();
                var index = i;
                for (var j = 0; j < contentRows; j++)
                {
                    if (index >= (cloudIdType == CloudIdType.Achievement ? tmpConfig.AchievementIDs.Count : tmpConfig.LeaderboardIDs.Count))
                    {
                        break;
                    }

                    DrawID(cloudIdType == CloudIdType.Achievement ? tmpConfig.AchievementIDs[index] : tmpConfig.LeaderboardIDs[index], cloudIdType);
                    index += contentColumnCount;
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a single cloud ID panel.
        /// </summary>
        /// <param name="platformIdData">The cloud ID to draw.</param>
        /// <param name="cloudIdType">If the cloud ID is an achievement or a leaderboard.</param>
        private void DrawID(PlatformIdData platformIdData, CloudIdType cloudIdType)
        {
            if (cloudIdType == CloudIdType.Achievement)
            {
                GUI.backgroundColor = isProSkin ? achievementDarkThemeColor : achievementLightThemeColor;
            }
            else
            {
                GUI.backgroundColor = isProSkin ? leaderboardDarkThemeColor : leaderboardLightThemeColor;
            }

            EditorGUILayout.BeginVertical(helpBoxStyle, GUILayout.Width(idPanelWidth));
            GUI.backgroundColor = backgroundColorCache;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box(cloudIdType == CloudIdType.Achievement ? achievementsSprite : leaderboardsSprite, idIconStyle);
            EditorGUILayout.LabelField(SplitCamelCase(platformIdData.InternalId), idTitleStyle, GUILayout.ExpandWidth(true));
            var deleteButtonRect = GUILayoutUtility.GetRect(deleteButtonContent, deleteButtonStyle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUILayout.LabelField(internalPlatformIDContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
            Undo.RecordObject(tmpConfig, "Set Internal ID");
            platformIdData.InternalId = EditorGUILayout.TextField(platformIdData.InternalId, GUILayout.ExpandWidth(true));
            platformIdData.InternalId = ValidationUtils.RemoveForbiddenCharactersFromInternalID(platformIdData.InternalId);
            GUILayout.Box(internalIdAllowedCharactersTipContent, helpIconStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUI.BeginDisabledGroup(!tmpConfig.AppleSupported);
            EditorGUILayout.LabelField(gameCenterPlatformIDContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
            Undo.RecordObject(tmpConfig, "Set Apple ID");
            platformIdData.AppleId = EditorGUILayout.TextField(platformIdData.AppleId, GUILayout.ExpandWidth(true));
            platformIdData.AppleId = ValidationUtils.RemoveForbiddenCharactersFromPlatformID(platformIdData.AppleId, CloudPlatform.iOS);
            GUILayout.Box(appleIdAllowedCharactersTipContent, helpIconStyle);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUI.BeginDisabledGroup(!tmpConfig.GoogleSupported);
            EditorGUILayout.LabelField(playGamesPlatformIDContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
            Undo.RecordObject(tmpConfig, "Set Google ID");
            platformIdData.GoogleId = EditorGUILayout.TextField(platformIdData.GoogleId, GUILayout.ExpandWidth(true));
            platformIdData.GoogleId = ValidationUtils.RemoveForbiddenCharactersFromPlatformID(platformIdData.GoogleId, CloudPlatform.GooglePlay);
            GUILayout.Box(googleIdAllowedCharactersTipContent, helpIconStyle);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(borderPadding);
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = isProSkin ? deleteButtonProColor : deleteButtonColor;
            if (GUI.Button(deleteButtonRect, deleteButtonContent, deleteButtonStyle))
            {
                if (cloudIdType == CloudIdType.Achievement)
                {
                    Undo.RecordObject(tmpConfig, "Delete achievement");
                    tmpConfig.AchievementIDs.Remove(platformIdData);
                }
                else
                {
                    Undo.RecordObject(tmpConfig, "Delete leaderboard");
                    tmpConfig.LeaderboardIDs.Remove(platformIdData);
                }

                GUI.FocusControl(null);
            }

            GUI.backgroundColor = backgroundColorCache;
        }

        #endregion /CloudIDs

        #region CloudVariables

        /// <summary>
        /// Draws the contents of the cloud variables tab.
        /// </summary>
        private void DrawCloudVariablesTab()
        {
            DrawCloudVariablesHeader();
            DrawCloudVariables();
        }

        /// <summary>
        /// Draws the cloud variables header and the add new button
        /// </summary>
        private void DrawCloudVariablesHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(cloudVariablesHeaderContent, headerLabelStyle);

            GUI.backgroundColor = isProSkin ? addButtonProColor : addButtonColor;
            if (GUILayout.Button(addCloudVariableButtonContent, addButtonStyle))
            {
                Undo.RecordObject(tmpConfig, "Add CloudVariable");
                tmpConfig.CloudVariables.Add(new CloudVariableData());
                GUI.FocusControl(null);
            }

            GUI.backgroundColor = backgroundColorCache;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws all cloud variables in a grid.
        /// </summary>
        private void DrawCloudVariables()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentationWidth);
            var contentRows = (tmpConfig.CloudVariables.Count / contentColumnCount) + 1;
            for (var i = 0; i < contentColumnCount; i++)
            {
                if (i >= tmpConfig.CloudVariables.Count)
                {
                    break;
                }

                EditorGUILayout.BeginVertical();
                var index = i;
                for (var j = 0; j < contentRows; j++)
                {
                    if (index >= tmpConfig.CloudVariables.Count)
                    {
                        break;
                    }

                    DrawCloudVariable(tmpConfig.CloudVariables[index]);
                    index += contentColumnCount;
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a single cloud variable panel.
        /// </summary>
        /// <param name="data">The cloud variable to draw.</param>
        private void DrawCloudVariable(CloudVariableData data)
        {
            GUI.backgroundColor = isProSkin ? cloudVariableDarkThemeColor : cloudVariableLightThemeColor;
            EditorGUILayout.BeginVertical(helpBoxStyle, GUILayout.Width(idPanelWidth));
            GUI.backgroundColor = backgroundColorCache;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box(cloudVariableIcon, idIconStyle);
            EditorGUILayout.LabelField(SplitCamelCase(data.Key), idTitleStyle, GUILayout.ExpandWidth(true));
            var deleteButtonRect = GUILayoutUtility.GetRect(deleteButtonContent, deleteButtonStyle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUILayout.LabelField(cloudVariableKeyContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
            Undo.RecordObject(tmpConfig, "Set CloudVariable Key");
            data.Key = EditorGUILayout.TextField(data.Key, GUILayout.ExpandWidth(true));
            data.Key = ValidationUtils.RemoveForbiddenCharactersFromInternalID(data.Key);
            GUILayout.Box(cloudVariableKeyTipContent, helpIconStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUILayout.LabelField(cloudVariableTypeContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
            Undo.RecordObject(tmpConfig, "Set CloudVariable Type");
            data.Type = (CloudVariableType)EditorGUILayout.EnumPopup(data.Type);
            GUILayout.Box(cloudVariableTypeTipContent, helpIconStyle);

            EditorGUILayout.EndHorizontal();
            if (data.Type != CloudVariableType.DateTime)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(borderPaddingSmall);

                EditorGUILayout.LabelField(cloudVariableDefaultValueContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
                if (data.Type == CloudVariableType.Bool)
                {
                    data.DefaultValueString = ValidationUtils.RemoveForbiddenCharactersFromDefaultValue(data.DefaultValueString, data.Type);
                    if (GUILayout.Button(bool.Parse(data.DefaultValueString) ? toggleOnSprite : toggleOffSprite, cloudVariableToggleStyle))
                    {
                        Undo.RecordObject(tmpConfig, "Set CloudVariable DefaultValueString");
                        data.DefaultValueString = (!bool.Parse(data.DefaultValueString)).ToString();
                    }

                    GUILayout.FlexibleSpace();
                }
                else
                {
                    Undo.RecordObject(tmpConfig, "Set CloudVariable DefaultValueString");
                    data.DefaultValueString = EditorGUILayout.TextField(data.DefaultValueString, GUILayout.ExpandWidth(true));
                    data.DefaultValueString = ValidationUtils.RemoveForbiddenCharactersFromDefaultValue(data.DefaultValueString, data.Type);
                }

                GUILayout.Box(cloudVariableDefaultValueTipContent, helpIconStyle);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(borderPaddingSmall);
            if (data.Type != CloudVariableType.CurrencyInt && data.Type != CloudVariableType.CurrencyFloat)
            {
                EditorGUILayout.LabelField(cloudVariablePersistenceTypeContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
                Undo.RecordObject(tmpConfig, "Set CloudVariable PersistenceType");
                data.PersistenceType = (PersistenceType)EditorGUILayout.EnumPopup(data.PersistenceType);
                GUILayout.Box(cloudVariablePersistenceTypeTipContent, helpIconStyle);
            }
            else
            {
                EditorGUILayout.LabelField(cloudVariableAllowNegativeContent, tinyLabelStyle, GUILayout.Width(idPlatformLabelWidth), GUILayout.Height(slimControlHeight));
                if (GUILayout.Button(data.AllowNegative ? toggleOnSprite : toggleOffSprite, cloudVariableToggleStyle))
                {
                    Undo.RecordObject(tmpConfig, "Set CloudVariable AllowNegative");
                    data.AllowNegative = !data.AllowNegative;
                }

                GUILayout.FlexibleSpace();
                GUILayout.Box(cloudVariableAllowNegativeTipContent, helpIconStyle);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(borderPadding);

            if (data.Type == CloudVariableType.DateTime && data.PersistenceType == PersistenceType.Latest)
            {
                EditorGUILayout.HelpBox("\"Latest\" persistence type prefers the DateTime that was SAVED last. For newest DateTime value, use \"Highest\".", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            GUI.backgroundColor = isProSkin ? deleteButtonProColor : deleteButtonColor;
            if (GUI.Button(deleteButtonRect, deleteButtonContent, deleteButtonStyle))
            {
                Undo.RecordObject(tmpConfig, "Delete cloud variable");
                tmpConfig.CloudVariables.Remove(data);
                GUI.FocusControl(null);
            }

            GUI.backgroundColor = backgroundColorCache;
        }

        #endregion /CloudSave

        #endregion /Content Area

        #region Footer

        /// <summary>
        /// Draws footer section
        /// </summary>
        private void DrawFooter()
        {
            EditorGUILayout.BeginVertical(footerBackgroundStyle);

            var noHelpBoxShown = DrawValidationArea();

            if (noHelpBoxShown)
            {
                GUILayout.Space(1f);
            }

            if (selectedTab == 2)
            {
                EditorGUI.BeginDisabledGroup(!PlayerPrefs.HasKey(DataManager.DevStringKey));
                if (GUILayout.Button("Delete editor test data", GUILayout.Height(footerButtonHeight)))
                {
                    if (EditorUtility.DisplayDialog(
                        "Delete Editor Test Data",
                        "Are you sure you want to delete editor test data for all cloud variables?\n\nThis can't be undone.",
                        "Delete test data",
                        "Cancel"))
                    {
                        PlayerPrefs.DeleteKey(DataManager.DevStringKey);
                        GUI.FocusControl(null);
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.BeginHorizontal();
            var changesHaveBeenMade = !savedConfig.EqualsSettings(tmpConfig) || !savedConfig.EqualsCloudIDs(tmpConfig) || !savedConfig.EqualsCloudVariables(tmpConfig);
            EditorGUI.BeginDisabledGroup(!changesHaveBeenMade);
            if (GUILayout.Button(revertButtonContent, GUILayout.Height(footerButtonHeight)))
            {
                if (EditorUtility.DisplayDialog(
                    "Discard Changes",
                    "Are you sure you want to discard all unsaved changes you have made to the configuration?\n\nThis can't be undone.",
                    "Discard changes",
                    "Cancel"))
                {
                    tmpConfig = SerializationUtils.LoadCloudConfig();
                    GUI.FocusControl(null);
                }
            }

            EditorGUI.EndDisabledGroup();

            DrawSaveButton();

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(borderPaddingSmall);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws an area for displaying any problems with validating the configuration.
        /// </summary>
        /// <returns><c>true</c> if no help box was shown.</returns>
        private bool DrawValidationArea()
        {
            GUILayout.Space(borderPadding);
            if (EditorApplication.isCompiling)
            {
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("The editor is compiling, please wait.", MessageType.Info);
                EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                return false;
            }

            if (Application.isPlaying)
            {
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("Settings are disabled while editor is in play mode.", MessageType.Info);
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                return false;
            }

            if (!tmpConfig.AppleSupported && !tmpConfig.GoogleSupported)
            {
                EditorGUILayout.HelpBox("Must support at least one platform.", MessageType.Warning);
                noErrorsFoundInPlatformSettings = false;
                return false;
            }

            noErrorsFoundInPlatformSettings = true;
            noEmptyInternalIdFields = ValidationUtils.ConfigHasNoEmptyInternalIdFields(tmpConfig);
            noDuplicateAchievementIDs = ValidationUtils.ConfigHasNoDuplicateIDs(
                tmpConfig.AchievementIDs, tmpConfig.AppleSupported, tmpConfig.GoogleSupported, "achievement");
            noDuplicateLeaderboardIDs = ValidationUtils.ConfigHasNoDuplicateIDs(
                tmpConfig.LeaderboardIDs, tmpConfig.AppleSupported, tmpConfig.GoogleSupported, "leaderboard");
            noDuplicateCloudVariableKeys = ValidationUtils.ConfigHasNoDuplicateCloudVariableKeys(tmpConfig.CloudVariables);
            noInvalidDefaultValues = ValidationUtils.ConfigHasNoInvalidDefaultValues(tmpConfig.CloudVariables);
            noEmptyPlatformIdFields = ValidationUtils.ConfigHasNoEmptyPlatformIDs(tmpConfig);
            noReservedIdsUsed = ValidationUtils.ConfigHasNoReservedIDs(tmpConfig.AchievementIDs, "All", "GetPlatformID", "AchievementDictionary")
                                && ValidationUtils.ConfigHasNoReservedIDs(tmpConfig.LeaderboardIDs, "GetPlatformID", "LeaderboardDictionary");
            if (tmpConfig.GoogleSupported && !tmpConfig.GoogleSetupRun)
            {
                EditorGUILayout.HelpBox("You have chosen to support Google Play, but you have not successfully run Google Application ID setup.", MessageType.Info);
                return false;
            }

            return noEmptyInternalIdFields && noDuplicateAchievementIDs && noDuplicateLeaderboardIDs
                && noDuplicateCloudVariableKeys && noInvalidDefaultValues && noEmptyPlatformIdFields
                && noReservedIdsUsed;
        }

        /// <summary>
        /// Draws the save button.
        /// </summary>
        private void DrawSaveButton()
        {
            var readyToSave = AllSettingsReadyForSave();
            EditorGUI.BeginDisabledGroup(!readyToSave);
            if (GUILayout.Button(readyToSave ? saveButtonContent : disabledSaveButtonContent, GUILayout.Height(footerButtonHeight), GUILayout.MinWidth(210f)))
            {
                GUI.FocusControl(null);
                SaveConfiguration();
                LoadConfiguration(true);
            }

            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Checks if there are any validation errors.
        /// </summary>
        /// <returns>Returns <c>true</c> if there are no errors, <c>false</c> if an error has been detected.</returns>
        private bool AllSettingsReadyForSave()
        {
            return noErrorsFoundInPlatformSettings && noDuplicateAchievementIDs && noDuplicateLeaderboardIDs
                && noDuplicateCloudVariableKeys && noInvalidDefaultValues && noEmptyInternalIdFields
                && noReservedIdsUsed;
        }

        #endregion /Footer

        /// <summary>
        /// When saving, the files CloudIDs.cs, Achievements.cs, Leaderboards.cs, SerializedCloudConfig.txt & AndroidManifest.xml are generated.
        /// </summary>
        private void SaveConfiguration()
        {
            if (tmpConfig.GoogleSupported)
            {
                BuildUtils.EnableGoogleBuildPlatform();
            }
            else
            {
                BuildUtils.DisableAndroidBuildSymbolConstraints();
            }

            if (tmpConfig.DebugModeEnabled)
            {
                BuildUtils.ToggleDebugBuildSymbolConstraints(tmpConfig.AppleSupported, tmpConfig.GoogleSupported);
            }
            else
            {
                BuildUtils.ToggleDebugBuildSymbolConstraints(false, false);
            }

            SerializationUtils.SerializeCloudConfig(tmpConfig);
        }

        /// <summary>
        /// Loads configuration from file in ProjectSettings folder
        /// </summary>
        private void LoadConfiguration(bool forceUpdate = false)
        {
            if (tmpConfig == null || forceUpdate)
            {
                tmpConfig = SerializationUtils.LoadCloudConfig();
            }

            if (savedConfig == null || forceUpdate)
            {
                savedConfig = SerializationUtils.LoadCloudConfig();
            }
        }

        #endregion /Private methods
    }
}
#endif
