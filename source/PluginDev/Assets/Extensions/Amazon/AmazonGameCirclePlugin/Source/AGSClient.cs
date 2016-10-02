/**
 * © 2012-2013 Amazon Digital Services, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except in compliance with the License. A copy
 * of the License is located at
 *
 * http://aws.amazon.com/apache2.0/
 *
 * or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */

/*
 * Modified by Jan Ivar Z. Carlsen.
 * Added CloudOnceAmazon build symbol.
 * Removed iOS support.
 * Changed errorLevel from const to static and set default to AmazonLoggingLevel.Errors.
 */

#if UNITY_ANDROID && CloudOnceAmazon

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enum used for positioning gamecircle toast messages
/// </summary>
public enum GameCirclePopupLocation
{
    BOTTOM_LEFT,
    BOTTOM_CENTER,
    BOTTOM_RIGHT,
    TOP_LEFT,
    TOP_CENTER,
    TOP_RIGHT
}

/// <summary>
/// AGSClient used for init and global GameCircle features
/// </summary>
public class AGSClient : MonoBehaviour
{
    #region Public static variables   
    /// <summary>
    /// Event called when GameCircle initialization succeeds and clients are ready to use
    /// </summary>
    public static event Action ServiceReadyEvent;
    
    /// <summary>
    /// Event called when GameCircle initialization fails
    /// </summary>
    /// <param name="failureReason">a string indicating the reason for the initialization failure</param>
    public static event Action<string> ServiceNotReadyEvent;
    
    // The name of this service, as a string. Used for reporting errors and other messaging.
    public const string serviceName = "AmazonGameCircle";
    
    // This is the level GameCircle will log messages.
    // It is recommended to keep this on verbose as you first implement GameCircle,
    // and then select a less verbose messaging to miminize GameCircle log output for day to day development.
    public static AmazonLogging.AmazonLoggingLevel errorLevel = AmazonLogging.AmazonLoggingLevel.Errors;
    
    // Tracks whether Init has been called. Once Init has been called, the app will begin to reinitialize when
    // the app regains focus.
    public static bool ReinitializeOnFocus = false;
    #endregion
    
    #region Private static variables
    // tracks if GameCircle is available yet.
    private static bool IsReady = false;
    private static bool supportsAchievements = false;
    private static bool supportsLeaderboards = false;
    private static bool supportsWhispersync = false;
    #endregion
    
#if UNITY_ANDROID && !UNITY_EDITOR
    // This is a Java Object in the GameCircle Proxy Java library.
    private static AmazonJavaWrapper JavaObject;

    private static readonly string PROXY_CLASS_NAME = "com.amazon.ags.api.unity.AmazonGamesClientProxyImpl";   
#endif
    
#if !UNITY_ANDROID
    // This is used when Unity is running on a platform that GameCircle does not support.
    private static readonly string serviceUnavailableOnPlatform = "Amazon GameCircle is not available on current platform.";
#endif
    
    static AGSClient()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        JavaObject = new AmazonJavaWrapper(); 
        using( var PluginClass = new AndroidJavaClass( PROXY_CLASS_NAME ) ){
            if (PluginClass.GetRawClass() == IntPtr.Zero)
            {
                LogGameCircleWarning("No java class " + PROXY_CLASS_NAME + " present, can't use AGSClient" );
                return;
            }
            JavaObject.setAndroidJavaObject(PluginClass.CallStatic<AndroidJavaObject>( "getInstance" ));
        }
#endif
    }
    
    /// <summary>
    /// Initializes this AGSClient.  The serviceReadyEvent or the serviceNotReady event will be called
    /// upon completion
    /// </summary>
    public static void Init()
    {
        Init(supportsLeaderboards, supportsAchievements, supportsWhispersync );
    }
    
    /// <summary>
    /// Initializes this AGSClient.  The serviceReadyEvent or the serviceNotReady event will be called
    /// upon completion
    /// </summary>
    /// <param name="supportsLeaderboards">bool indicating if this game uses leaderboards</param>
    /// <param name="supportsAchievements">bool indicating if this game uses achievements</param>
    /// <param name="supportsWhispersync">bool indicating if this game uses whispersync</param>
    public static void Init(bool supportsLeaderboards, bool supportsAchievements, bool supportsWhispersync )
    {
        AGSClient.ReinitializeOnFocus = true;
        AGSClient.supportsAchievements = supportsAchievements;
        AGSClient.supportsLeaderboards = supportsLeaderboards;
        AGSClient.supportsWhispersync = supportsWhispersync;
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // fake a success in editor mode, to allow for easier testing.
        ServiceReady(string.Empty);
#elif UNITY_ANDROID            
        JavaObject.Call( "init", supportsLeaderboards, supportsAchievements, supportsWhispersync );
#else
        ServiceNotReady(serviceUnavailableOnPlatform);
#endif
    }
    
     /// <summary>
     /// Sets the pop up location for toast notifications
     /// </summary>
     /// <param name="location">location enum value indicating the preferred position of toast</param>
    public static void SetPopUpEnabled( bool enabled )
    {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID                  
        JavaObject.Call( "setPopupEnabled", enabled );
#endif
    }
    
    /// <summary>
    /// Sets the pop up location for toast notifications
    /// </summary>
    /// <param name="location">location enum value indicating the preferred position of toast</param>
    public static void SetPopUpLocation( GameCirclePopupLocation location )
    {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID      
        JavaObject.Call( "setPopUpLocation", location.ToString() );
#endif
    }
    
    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void ServiceReady( string empty )
    {
        IsReady = true;
        if( ServiceReadyEvent != null )
            ServiceReadyEvent();
    }
    
    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static bool IsServiceReady(){
        return IsReady;    
    }
    
    /// <summary>
    /// Pauses game time played tracking.  This should be called when the game leaves the foreground
    /// </summary>
    public static void release(){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID      
        JavaObject.Call ("release");
#endif
    }
    
    /// <summary>
    /// Fully releases all resources and operations associated with an initialized AmazonGamesClient. 
    /// Call this method when you are finished interacting with the GameCircle SDK (typically, when your game is exiting). 
    /// Any subsequent interactions with the SDK will require a new call to initialize(). 
    /// Shutdown() should be called when you are ready to shut down your application.
    /// </summary>
    public static void Shutdown(){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID      
        JavaObject.Call ("shutdown");
#endif
    }
    
    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void ServiceNotReady( string param )
    {
        IsReady = false;
        if( ServiceNotReadyEvent != null )
            ServiceNotReadyEvent( param );
    }
    
    /// <summary>
    /// Shows the GameCircle overlay.
    /// </summary>
    /// <param name='isAnimated'>
    /// Is animated.
    /// </param>
    public static void ShowGameCircleOverlay() {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call("showGameCircleOverlay");
#endif
    }
 
    
    /// <summary>
    /// Shows the GameCircle sign in page.
    /// </summary>
    /// <param name='isAnimated'>
    /// Is animated.
    /// </param>
    public static void ShowSignInPage() {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call("showSignInPage");
#endif
    }
    
    #region error and warning messaging
    /// <summary>
    /// Logs the GameCircle error.
    /// </summary>
    /// <param name='errorMessage'>
    /// Error message.
    /// </param>
    public static void LogGameCircleError(string errorMessage) {
        AmazonLogging.LogError(errorLevel,serviceName,errorMessage);
    }
    
    /// <summary>
    /// Logs the GameCircle warning.
    /// </summary>
    /// <param name='errorMessage'>
    /// Error message.
    /// </param>
    public static void LogGameCircleWarning(string errorMessage) {
        AmazonLogging.LogWarning(errorLevel,serviceName,errorMessage);
    }
    
    /// <summary>
    /// Log the GameCircle message.
    /// </summary>
    /// <param name='message'>
    /// Message.
    /// </param>
    public static void Log(string message) {
        AmazonLogging.Log(errorLevel,serviceName,message);
    }
    #endregion
        
}
#endif
