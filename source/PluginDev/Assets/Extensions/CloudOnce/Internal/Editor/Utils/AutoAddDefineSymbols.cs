// <copyright file="AutoAddDefineSymbols.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor.Utils
{
    using UnityEditor;
#if UNITY_2017_1_OR_NEWER
    using UnityEditor.Build;
#endif

    /// <summary>
    /// Automatically adds the NO_GPGS script define symbol to iOS player settings.
    /// </summary>
    [InitializeOnLoad]
    public class AutoAddDefineSymbols
#if UNITY_2017_1_OR_NEWER
        : IActiveBuildTargetChanged
#endif
    {
        private const string defineSymbol = "NO_GPGS";

        static AutoAddDefineSymbols()
        {
            SetNoGPGS();
        }
#if UNITY_2017_1_OR_NEWER
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            if (newTarget == BuildTarget.iOS)
            {
                SetNoGPGS();
            }
        }
#endif
        private static void SetNoGPGS()
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Trim();
            if (!defineSymbols.Contains(defineSymbol))
            {
                if (string.IsNullOrEmpty(defineSymbols))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbol);
                }
                else
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(
                        BuildTargetGroup.iOS,
                        defineSymbols + ";" + defineSymbol);
                }
            }

            // Disable Jar background resolution
            EditorPrefs.SetBool("GooglePlayServices.AutoResolverEnabled", false);
        }
    }
}
#endif
