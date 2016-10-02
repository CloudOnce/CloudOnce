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
 */

#if UNITY_ANDROID && CloudOnceAmazon

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Player client used to get information on the currently logged in player
/// </summary>
public class AGSPlayerClient : MonoBehaviour{
    

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AmazonJavaWrapper JavaObject;
    private static readonly string PROXY_CLASS_NAME = "com.amazon.ags.api.unity.PlayerClientProxyImpl"; 
#endif
    
    /// <summary>
    /// Occurs when a request for the local player has completed.
    /// </summary>
    public static event Action<AGSRequestPlayerResponse> RequestLocalPlayerCompleted;

    /// <summary>
    /// Occurs when a request for the player ID's for the local player's friends has completed.
    /// </summary>
    public static event Action<AGSRequestFriendIdsResponse> RequestFriendIdsCompleted;

    /// <summary>
    /// Occurs when a request for player objects for the local player's friends has completed.
    /// </summary>
    public static event Action<AGSRequestBatchFriendsResponse> RequestBatchFriendsCompleted;

    /// <summary>
    /// Occurs when signed in state changed.
    /// </summary>
    public static event Action<Boolean> OnSignedInStateChangedEvent;

    static AGSPlayerClient(){
#if UNITY_ANDROID && !UNITY_EDITOR
        // find the plugin instance
        JavaObject = new AmazonJavaWrapper(); 
        using( var PluginClass = new AndroidJavaClass( PROXY_CLASS_NAME ) ){
            if (PluginClass.GetRawClass() == IntPtr.Zero)
            {
                AGSClient.LogGameCircleWarning("No java class " + PROXY_CLASS_NAME + " present, can't use AGSPlayerClient" );
                return;
            }
            JavaObject.setAndroidJavaObject(PluginClass.CallStatic<AndroidJavaObject>( "getInstance" ));
        }
#endif
    }
    
#pragma warning disable 0618

    /// <summary>
    /// Request the local player player information
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    /// </summary>
    public static void RequestLocalPlayer( int userData = 0 ){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestLocalPlayer" , userData);
#else
        AGSRequestPlayerResponse response = AGSRequestPlayerResponse.GetPlatformNotSupportedResponse(userData);
        if (PlayerFailedEvent != null) {
            PlayerFailedEvent(response.error);
        }
        if (RequestLocalPlayerCompleted != null){
            RequestLocalPlayerCompleted(response);
        }
#endif
    }

    /// <summary>
    /// ANDROID ONLY
    /// Requests the local player friend identifiers.
    /// </summary>
    /// <remarks>
    /// RequestFriendIdsCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="userData">
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestFriendIds( int userData = 0 ) {
#if UNITY_EDITOR && UNITY_ANDROID
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestLocalPlayerFriends" , userData);
#else
        if (RequestFriendIdsCompleted != null) {
            RequestFriendIdsCompleted(AGSRequestFriendIdsResponse.GetPlatformNotSupportedResponse(userData));
        }
#endif
    }


    /// <summary>
    /// ANDROID ONLY
    /// Requests the full player information for friends of the local player.
    /// Only friends of the local player will be included in the response.
    /// </summary>
    /// <remarks>
    /// RequestBatchFriendsCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="friendIds">Player ID's for the friends requested.</param>
    /// <param name="userData">
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestBatchFriends ( List<string> friendIds, int userData = 0 ) {
#if UNITY_EDITOR && UNITY_ANDROID
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        string json = MiniJSON.jsonEncode ( friendIds.ToArray() ) ; 
        JavaObject.Call( "requestBatchFriends" , json, userData);
#else
        if (RequestBatchFriendsCompleted != null) {
            RequestBatchFriendsCompleted(AGSRequestBatchFriendsResponse.GetPlatformNotSupportedResponse(friendIds, userData));
        }
#endif

    }

    public static void LocalPlayerFriendsComplete( string json ) {
        if (RequestFriendIdsCompleted != null) {
            RequestFriendIdsCompleted(AGSRequestFriendIdsResponse.FromJSON(json));
        }
    }

    public static void BatchFriendsRequestComplete ( string json ) {
        if (RequestBatchFriendsCompleted != null) {
            RequestBatchFriendsCompleted(AGSRequestBatchFriendsResponse.FromJSON(json));
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void PlayerReceived( string json ){
        AGSRequestPlayerResponse response = AGSRequestPlayerResponse.FromJSON (json);
        if( !response.IsError() && PlayerReceivedEvent != null ){
            PlayerReceivedEvent( response.player );
        }
        if (RequestLocalPlayerCompleted != null) {
            RequestLocalPlayerCompleted(response);
        }
    }
    
    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void PlayerFailed ( string json ){
        AGSRequestPlayerResponse response = AGSRequestPlayerResponse.FromJSON (json);
        if( response.IsError() && PlayerFailedEvent != null ){
            PlayerFailedEvent( response.error );
        }
        if (RequestLocalPlayerCompleted != null) {
            RequestLocalPlayerCompleted(response);
        }
    }
    
    /// <summary>
    /// Request whether the local player is signed in.
    /// </summary>
    public static bool IsSignedIn(){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
        return false;
#elif UNITY_ANDROID      
        return JavaObject.Call<bool>("isSignedIn");
#else
        return false;
#endif
    }

    /// <summary>
    /// The signed in state changed.
    /// </summary>
    /// <param name="isSignedIn">If set to <c>true</c>, the local player is signed in.</param>
    public static void OnSignedInStateChanged (Boolean isSignedIn) {
        if (OnSignedInStateChangedEvent != null) {
            OnSignedInStateChangedEvent (isSignedIn);
        }
    }

#pragma warning restore 0618

    #region Obsolete events
    
    /// <summary>
    /// Event called when player request succeeds
    /// </summary>
    [Obsolete("PlayerReceivedEvent is deprecated. Use RequestLocalPlayerCompleted instead.")]
    public static event Action<AGSPlayer> PlayerReceivedEvent;

    /// <summary>
    /// Event called when player request fails
    /// </summary>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("PlayerFailedEvent is deprecated. Use RequestLocalPlayerCompleted instead.")]
    public static event Action<string> PlayerFailedEvent;

    #endregion

}
#endif
