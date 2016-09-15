// <copyright file="AutoAddDefineSymbols.cs" company="Jan Ivar Z. Carlsen & Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen & Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace Trollpants.CloudOnce.Internal.Editor.Utils
{
    using UnityEditor;

    /// <summary>
    /// Automatically adds the NO_GPGS scipt define symbol to iOS player settings.
    /// </summary>
    [InitializeOnLoad]
    public class AutoAddDefineSymbols
    {
        private const string c_defineSymbol = "NO_GPGS";

        static AutoAddDefineSymbols()
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Trim();
            if (!defineSymbols.Contains(c_defineSymbol))
            {
                if (string.IsNullOrEmpty(defineSymbols))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, c_defineSymbol);
                }
                else
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbols + ";" + c_defineSymbol);
                }
            }

            // Disable Jar background resolution
            EditorPrefs.SetBool("GooglePlayServices.AutoResolverEnabled", false);
        }
    }
}
#endif
