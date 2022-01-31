// <copyright file="CloudOncePaths.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

public static class CloudOncePaths
{
    public const string Data = assets + "/CloudOnce" + "/Data";
    public const string Templates = cloudOnce + "/Internal/Data/Templates";
    public const string GoogleTemplates = package + "/Editor/CloudOnce/Internal/Editor/Utils/GooglePlayGamesSetup";
    public const string Android = plugins + "/Android";
    public const string AndroidAssets = assets + "/Plugins" + "/Android";
    public const string GooglePlayLib = AndroidAssets + "/GooglePlayGamesManifest.androidlib";
    public const string SettingsProjectSettings = "ProjectSettings/CloudOnceSettings.txt";
    public const string SettingsCloudOnce = assets + "/CloudOnce" + "/CloudOnceSettings.txt";

    private const string package = "Packages/jizc.cloudonce.io";
    private const string assets = "Assets";

    private const string cloudOnce = package + "/Extensions/CloudOnce";
    private const string gpgs = package + "/Extensions/GooglePlayGames";
    private const string plugins = package + "/Plugins";
}
