// <copyright file="CloudOncePaths.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

public static class CloudOncePaths
{
    public const string Data = c_cloudOnce + "/Data";
    public const string Templates = c_cloudOnce + "/Internal/Data/Templates";
    public const string GoogleTemplates = c_cloudOnce + "/Internal/Editor/Utils/GooglePlayGamesSetup";
    public const string Android = c_plugins + "/Android";
    public const string GameCircleLib = Android + "/gamecircle_lib";
    public const string GooglePlayLib = Android + "/MainLibProj";
    public const string Settings = "ProjectSettings/CloudOnceSettings.txt";

    private const string c_trollpants = "Assets/Extensions/Trollpants";
    private const string c_cloudOnce = c_trollpants + "/CloudOnce";
    private const string c_plugins = "Assets/Plugins";
}
