![logo_small.png](.github/logo_small.png)

# CloudOnce - Unified Game Services API
CloudOnce is a [Unity](http://unity3d.com/) plug-in that provides a unified game services API for [Google Play Game Services](https://github.com/playgameservices/play-games-plugin-for-unity/) and Apple Game Center (iOS & tvOS). Ease of use is the primary focus for the plug-in, but it also attempts to satisfy the needs of power users.

## Getting Started
Download the [latest release](https://github.com/jizc/CloudOnce/releases/latest) and then check out the [Getting Started](http://jizc.github.io/CloudOnce/gettingStarted.html) guide for an overview of available features and how to implement them.

## API Documentation
If you need more technical or more complete documentation of the plug-in, you can check out the [API documentation](http://jizc.github.io/CloudOnce/api-docs/index.html).

## Building
If you want to modify CloudOnce and build your own unitypackage, there are a few required steps.

For the PluginDev Unity project to compile you need to download and import [External Dependency Manager for Unity (EDM4U)](https://github.com/googlesamples/unity-jar-resolver).

Because CloudOnce includes EDM4U, building a new unitypackage needs to be done from command-line or with a build script ([more info](https://github.com/googlesamples/unity-jar-resolver#getting-started)). Below is an example of how to do it on Windows. Change the paths to fit your environment.

**Windows example:**
1. Download the latest [EDM4U](https://github.com/googlesamples/unity-jar-resolver) unitypackage
2. Open cmd
3. Enter `"C:\Program Files\Unity531\Editor\Unity.exe" -batchmode -upgrader_disable -projectPath "C:\GitHub\CloudOnce\source\PluginDev" -exportPackage Assets/Extensions Assets/Plugins "C:\GitHub\CloudOnce\build\source.unitypackage" -quit`
4. Enter `"C:\Program Files\Unity531\Editor\Unity.exe" -batchmode -upgrader_disable -gvh_disable -createProject ./CloudOnce -importPackage "C:\GitHub\CloudOnce\build\external-dependency-manager-1.2.157.unitypackage" -quit`
5. Enter `"C:\Program Files\Unity531\Editor\Unity.exe" -batchmode -upgrader_disable -gvh_disable -projectPath "C:\GitHub\CloudOnce\build\CloudOnce" -importPackage "C:\GitHub\CloudOnce\build\source.unitypackage" -exportPackage Assets "C:\GitHub\CloudOnce\current-build\CloudOnce-v2.6.8.unitypackage" -quit`

## History
This plug-in was originally developed for internal use by Trollpants Game Studio. [Sindri Jóelsson](http://github.com/sindrijo) wrote most of the original code, and [Jan Ivar Z. Carlsen](http://github.com/jizc) assisted by writing the implementations for Google Play and iOS.

In January 2015 work began to make it into a commercial product. From that point, [Jan Ivar](http://github.com/jizc) has been the sole developer on the project. The first commercial version was released on the Asset Store on May 6th 2015.

CloudOnce has evolved a lot since then, and on June 1st 2016 it was made open-source. This was done both due to time constraints (both creators have other full time jobs), and that the financial incentive wasn't large enough to justify the support responsibilities that comes from selling a commercial product.

## Trollpants Game Studio games
All the games created by Trollpants Game Studio have been released as open source. They all use CloudOnce and have been updated to run in Unity 2017.3. Check them out for examples of how CloudOnce can be used.
* [Trollpants repository](https://github.com/jizc/Trollpants)
* [Trollpants WebGL builds](https://jizc.github.io/Trollpants)

## License
The contents of this project is licensed under the MIT license, unless other is specified in file header. See [LICENSE](./LICENSE) file in the project root for full license information.
