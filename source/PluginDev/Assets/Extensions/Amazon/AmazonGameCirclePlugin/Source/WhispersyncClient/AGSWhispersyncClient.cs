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
 * Initialization switched from static constructor to a static method that can be called at a specified point.
 * Added CloudOnceAmazon build symbol.
 * Removed iOS support.
 * Removed redundant code.
 * Renamed OnSyncFailed parameter.
 * Tweaked spacing to improve readability.
 */

#if UNITY_ANDROID && CloudOnceAmazon

using System;
using UnityEngine;

/// <summary>
/// WhispersyncClient used for interacting with synchronized game objects
/// </summary>
public class AGSWhispersyncClient : MonoBehaviour
{
    
#if UNITY_ANDROID
    private static AmazonJavaWrapper javaObject;
#if !UNITY_EDITOR
    private static readonly string PROXY_CLASS_NAME = "com.amazon.ags.api.unity.WhispersyncClientProxyImpl"; 
#endif

#endif
    
    /// <summary>
    /// This event will occur when game data is downloaded after installation, or when another device registered
    /// to the player has uploaded new game data.
    /// </summary>
    public static event Action OnNewCloudDataEvent;

    /// <summary>
    /// This event will occur when game data has successfully been uploaded to the cloud.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnDataUploadedToCloudEvent;

    /// <summary>
    /// This event will occur when a cloud synchronization attempt is made too soon after a previous synchronization.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnThrottledEvent;

    /// <summary>
    /// This event will occur when game data has been written to the local file system.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnDiskWriteCompleteEvent;

    /// <summary>
    /// This event will occur the very first time the game successfully attempts to download game data from the cloud,
    /// regardless of whether game data existed in or not.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnFirstSynchronizeEvent;

    /// <summary>
    /// This event will occur after attempting to synchronize with the cloud but the game already has the latest
    /// version of game data.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnAlreadySynchronizedEvent;

    /// <summary>
    /// This event will occur any time a synchronize attempt with the cloud cannot be completed.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnSyncFailedEvent;

    /// <summary>
    /// This event will occur when WhisperSync is initialized.
    /// ANDROID ONLY
    /// </summary>
    public static event Action OnInitializedEvent;

    /// <summary>
    /// The failReason string represents the reason that the most recent OnSyncFailedEvent occurred.
    /// It should be one of five possible values:
    ///     null (Null indicates that no error has occured)
    ///     "OFFLINE"
    ///     "WHISPERSYNC_DISABLED"
    ///     "SERVICE_ERROR"
    ///     "CLIENT_ERROR"
    /// </summary>
    public static string failReason;

    /// <summary>
    /// Initializes WhisperSync client. Calls OnInitializedEvent when finished.
    /// </summary>
    public static void InitAGSWhispersyncClient()
    {
#if UNITY_ANDROID
        javaObject = new AmazonJavaWrapper(); 
#if !UNITY_EDITOR
        using( var PluginClass = new AndroidJavaClass( PROXY_CLASS_NAME ) ){
            if (PluginClass.GetRawClass() == IntPtr.Zero)
            {
                AGSClient.LogGameCircleWarning("No java class " + PROXY_CLASS_NAME + " present, can't use AGSWhispersyncClient" );
                return;
            }        
            javaObject.setAndroidJavaObject(PluginClass.CallStatic<AndroidJavaObject>( "getInstance" ));
        }
#endif
#endif
        if (OnInitializedEvent != null)
        {
            OnInitializedEvent();
        }
    }
    
     /// <summary>
     /// gets the root game datamap 
     /// </summary>
     /// <returns>Game datamap</returns>
    public static AGSGameDataMap GetGameData( )
    {
#if UNITY_ANDROID
        AndroidJavaObject jo = javaObject.Call<AndroidJavaObject>("getGameData");
        return jo != null ? new AGSGameDataMap(new AmazonJavaWrapper(jo)) : null;
#else
        return null;
#endif
    }
    
     /// <summary>
     /// Manually triggers a background thread to synchronize in-memory game data with local storage and the cloud.
     /// </summary>
    public static void Synchronize(){
#if UNITY_ANDROID            
        javaObject.Call("synchronize");
#endif
    }

     /// <summary>
     /// Manually triggers a background thread to write in-memory game data to only the local storage.
     /// </summary>    
    public static void Flush(){
#if UNITY_ANDROID                    
        javaObject.Call("flush");
#endif
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnNewCloudData()
    {
        if(OnNewCloudDataEvent != null)
        {        
            OnNewCloudDataEvent();
        }
    }


    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnDataUploadedToCloud()
    {
        if (OnDataUploadedToCloudEvent != null)
        {        
            OnDataUploadedToCloudEvent();
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnThrottled()
    {
        if (OnThrottledEvent != null)
        {        
            OnThrottledEvent();
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnDiskWriteComplete()
    {
        if (OnDiskWriteCompleteEvent != null)
        {        
            OnDiskWriteCompleteEvent();
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnFirstSynchronize()
    {
        if (OnFirstSynchronizeEvent != null)
        {        
            OnFirstSynchronizeEvent();
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnAlreadySynchronized()
    {
        if (OnAlreadySynchronizedEvent != null)
        {        
            OnAlreadySynchronizedEvent();
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>            
    public static void OnSyncFailed(string reason)
    {
        failReason = reason;
        if (OnSyncFailedEvent != null)
        {        
            OnSyncFailedEvent();
        }
    }
}
#endif
