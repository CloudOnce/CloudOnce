// <copyright file="BuildUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Build utilities used by the CloudOnce editor.
    /// </summary>
    public static class BuildUtils
    {
        #region Fields & properties

        private const string debugBuildSymbolConstraint = "CLOUDONCE_DEBUG";
        private const string googleBuildSymbolConstraint = "CLOUDONCE_GOOGLE";
#if !UNITY_2018_3_OR_NEWER
        private static readonly AndroidManifestModifier s_manifestModifier = new AndroidManifestModifier();
#endif
        #endregion /Fields & properties

        #region Public methods

        /// <summary>
        /// Enables the DEBUG build constraint symbol.
        /// </summary>
        public static void ToggleDebugBuildSymbolConstraints(bool enableiOSDebug, bool enableAndroidDebug)
        {
            if (enableiOSDebug)
            {
                if (!iOSBuildSymbolIsDefined(debugBuildSymbolConstraint))
                {
                    SetiOSBuildSymbolImpl(new[] { debugBuildSymbolConstraint }, null);
                }
            }
            else
            {
                SetiOSBuildSymbolImpl(null, new[] { debugBuildSymbolConstraint });
            }

            if (enableAndroidDebug)
            {
                if (!AndroidBuildSymbolIsDefined(debugBuildSymbolConstraint))
                {
                    SetAndroidBuildSymbolImpl(new[] { debugBuildSymbolConstraint }, null);
                }
            }
            else
            {
                SetAndroidBuildSymbolImpl(null, new[] { debugBuildSymbolConstraint });
            }
        }

        /// <summary>
        /// Enables Google Play as the Android build platform.
        /// </summary>
        public static void EnableGoogleBuildPlatform()
        {
#if !UNITY_2018_3_OR_NEWER
            s_manifestModifier.EnableGoogleBuildPlatform();
#endif
            if (!AndroidBuildSymbolIsDefined(googleBuildSymbolConstraint))
            {
                SetAndroidBuildSymbolImpl(new[] { googleBuildSymbolConstraint }, null);
            }
        }

        /// <summary>
        /// Removes all Android build symbol constraints.
        /// </summary>
        public static void DisableAndroidBuildSymbolConstraints()
        {
            if (AndroidBuildSymbolIsDefined(googleBuildSymbolConstraint))
            {
                SetAndroidBuildSymbolImpl(null, new[] { googleBuildSymbolConstraint });
            }
        }

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Get a list of all the build constraint symbols defined in the Android build target group.
        /// </summary>
        /// <returns>List of defined build constraint symbols.</returns>
        private static List<string> GetAndroidDefinesList()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
        }

        /// <summary>
        /// Get a list of all the build constraint symbols defined in the iOS build target group.
        /// </summary>
        /// <returns>List of defined build constraint symbols.</returns>
        private static List<string> GetiOSDefinesList()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();
        }

        /// <summary>
        /// Check if a build constraint symbol is defined in the Android build target group.
        /// </summary>
        /// <param name="symbol">Build constraint symbol to check.</param>
        /// <returns>Return <c>true</c> if symbol is defined, <c>false</c> if not.</returns>
        private static bool AndroidBuildSymbolIsDefined(string symbol)
        {
            return GetAndroidDefinesList().Contains(symbol);
        }

        /// <summary>
        /// Check if a build constraint symbol is defined in the iOS build target group.
        /// </summary>
        /// <param name="symbol">Build constraint symbol to check.</param>
        /// <returns>Return <c>true</c> if symbol is defined, <c>false</c> if not.</returns>
        private static bool iOSBuildSymbolIsDefined(string symbol)
        {
            return GetiOSDefinesList().Contains(symbol);
        }

        /// <summary>
        /// Enables/disables a list of build constraint symbols in the Android build target group.
        /// </summary>
        /// <param name="enableSymbols">List of build constraint symbols to enable.</param>
        /// <param name="disableSymbols">List of build constraint symbols to disable.</param>
        private static void SetAndroidBuildSymbolImpl(IEnumerable<string> enableSymbols, string[] disableSymbols)
        {
            var definedSymbols = GetAndroidDefinesList();

            if (enableSymbols != null)
            {
                foreach (var defineSymbol in enableSymbols.Where(enableSymbol => !definedSymbols.Contains(enableSymbol)))
                {
                    if (disableSymbols != null)
                    {
                        if (disableSymbols.Contains(defineSymbol))
                        {
                            Debug.LogWarning(string.Format(
                                    "Define Symbol \"{0}\" is being disabled and enabled in the same operation!", defineSymbol));
                        }
                    }

                    definedSymbols.Add(defineSymbol);
                }
            }

            if (disableSymbols != null)
            {
                foreach (var disableSymbol in disableSymbols)
                {
                    while (definedSymbols.Contains(disableSymbol))
                    {
                        definedSymbols.Remove(disableSymbol);
                    }
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, string.Join(";", definedSymbols.ToArray()));
        }

        /// <summary>
        /// Enables/disables a list of build constraint symbols in the iOS build target group.
        /// </summary>
        /// <param name="enableSymbols">List of build constraint symbols to enable.</param>
        /// <param name="disableSymbols">List of build constraint symbols to disable.</param>
        private static void SetiOSBuildSymbolImpl(IEnumerable<string> enableSymbols, string[] disableSymbols)
        {
            var definedSymbols = GetiOSDefinesList();

            if (enableSymbols != null)
            {
                foreach (var defineSymbol in enableSymbols.Where(enableSymbol => !definedSymbols.Contains(enableSymbol)))
                {
                    if (disableSymbols != null)
                    {
                        if (disableSymbols.Contains(defineSymbol))
                        {
                            Debug.LogWarning(string.Format(
                                    "Define Symbol \"{0}\" is being disabled and enabled in the same operation!", defineSymbol));
                        }
                    }

                    definedSymbols.Add(defineSymbol);
                }
            }

            if (disableSymbols != null)
            {
                foreach (var disableSymbol in disableSymbols)
                {
                    while (definedSymbols.Contains(disableSymbol))
                    {
                        definedSymbols.Remove(disableSymbol);
                    }
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, string.Join(";", definedSymbols.ToArray()));
        }

        #endregion / Private methods
    }
}
#endif
