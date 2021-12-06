// <copyright file="PluginVersion.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Editor.Data
{
    /// <summary>
    /// Contains plug-in version information.
    /// </summary>
    public class PluginVersion
    {
        public const string VersionString = "2.7.5";

        public static readonly System.Version Version = new System.Version(VersionString);
    }
}
