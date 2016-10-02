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
/// Client used to submit and read achievements for the current logged in or guest player
/// </summary>
public class AGSAchievementsClient : MonoBehaviour{

    
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AmazonJavaWrapper JavaObject = null;
    
    private static readonly string PROXY_CLASS_NAME = "com.amazon.ags.api.unity.AchievementsClientProxyImpl"; 
#endif
    
    /// <summary>
    /// Occurs when an achievement update has completed.
    /// </summary>
    public static event Action<AGSUpdateAchievementResponse> UpdateAchievementCompleted;

    /// <summary>
    /// Occurs when a request for achievements has completed.
    /// </summary>
    public static event Action<AGSRequestAchievementsResponse> RequestAchievementsCompleted;

    /// <summary>
    /// Occurs when a request for a player's achievements has completed.
    /// </summary>
    public static event Action<AGSRequestAchievementsForPlayerResponse> RequestAchievementsForPlayerCompleted;

    static AGSAchievementsClient(){
#if UNITY_ANDROID && !UNITY_EDITOR
        JavaObject = new AmazonJavaWrapper(); 
        using( var PluginClass = new AndroidJavaClass( PROXY_CLASS_NAME ) ){
            if (PluginClass.GetRawClass() == IntPtr.Zero)
            {
                AGSClient.LogGameCircleWarning(string.Format("No java class {0} present, can't use AGSAchievementsClient",PROXY_CLASS_NAME ));
                return;
            }
            JavaObject.setAndroidJavaObject(PluginClass.CallStatic<AndroidJavaObject>( "getInstance" ));
        }
#endif
    }
    
#pragma warning disable 0618

    /// <summary>
    /// updates an achievement
    /// </summary>
    /// <remarks>
    /// If a value outside of range is submitted, it is capped at 100 or 0.
    /// If submitted value is less than the stored value, the update is ignored.
    /// </remarks>
    /// <param name="achievementId">the id of the achievement to update</param>
    /// <param name="percentComplete">a float between 0.0f and 100.0f</param>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void UpdateAchievementProgress( string achievementId, float progress, int userData = 0 ){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID        
        JavaObject.Call( "updateAchievementProgress", achievementId, progress, userData );
#else
        AGSUpdateAchievementResponse response = AGSUpdateAchievementResponse.GetPlatformNotSupportedResponse(achievementId, userData);
        if( UpdateAchievementFailedEvent != null ){
            UpdateAchievementFailedEvent( response.achievementId, response.error);
        }
        if (UpdateAchievementCompleted != null) {
            UpdateAchievementCompleted(response);
        }
#endif
    }

    /// <summary>
    ///  requests a list of all achievements
    /// </summary>
    /// <remarks>
    /// Registered updateAchievementSucceededEvents will recieve response
    /// The list returned in the response includes all achievements for the game.
    /// Each Achievement object in the list includes the current players
    /// progress toward the Achievement.
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    /// </remarks>
    public static void RequestAchievements( int userData = 0 ){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID      
        JavaObject.Call( "requestAchievements" , userData);
#else
        AGSRequestAchievementsResponse response = AGSRequestAchievementsResponse.GetPlatformNotSupportedResponse(userData);
        if(RequestAchievementsFailedEvent != null){
            RequestAchievementsFailedEvent(response.error);
        }
        if (RequestAchievementsCompleted != null) {
            RequestAchievementsCompleted(response);
        }
#endif
    }

    /// <summary>
    /// ANDROID ONLY
    /// Requests a list of all of a player's achievements.
    /// </summary>
    /// <remarks>
    /// RequestAchievementsForPlayerCompleted will be called if the event is registered.
    /// The list returned in the response includes all achievements for the game.
    /// Each Achievement object in the list includes the requested player's
    /// progress toward the achievement.
    /// </remarks>
    /// <param name="playerId">Player identifier for the achievement request.</param>
    /// <param name="userData">
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestAchievementsForPlayer( string playerId, int userData = 0 ){

#if UNITY_EDITOR && UNITY_ANDROID
        // GameCircle only functions on device.
#elif UNITY_ANDROID      
        JavaObject.Call( "requestAchievementsForPlayer" , playerId, userData);
#else
        if (RequestAchievementsForPlayerCompleted != null) {
            RequestAchievementsForPlayerCompleted(AGSRequestAchievementsForPlayerResponse.GetPlatformNotSupportedResponse(playerId, userData));
        }
#endif
    }

    /// <summary>
    ///  shows the Amazon GameCircle Overlay
    /// </summary>
    public static void ShowAchievementsOverlay(){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID      
        JavaObject.Call( "showAchievementsOverlay" );
#endif
    }
    
    /**
     * callback method for native code
     **/
    public static void RequestAchievementsSucceeded( string json ){
        AGSRequestAchievementsResponse response = AGSRequestAchievementsResponse.FromJSON (json);
        if( !response.IsError() && RequestAchievementsSucceededEvent != null ){
            RequestAchievementsSucceededEvent(response.achievements);
        }
        if (RequestAchievementsCompleted != null) {
            RequestAchievementsCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestAchievementsFailed( string json ){
        AGSRequestAchievementsResponse response = AGSRequestAchievementsResponse.FromJSON (json);
        if ( response.IsError() && RequestAchievementsFailedEvent != null ){
            RequestAchievementsFailedEvent(response.error);
        }
        if (RequestAchievementsCompleted != null) {
            RequestAchievementsCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void UpdateAchievementFailed( string json ){
        AGSUpdateAchievementResponse response = AGSUpdateAchievementResponse.FromJSON (json);
        if( response.IsError() && UpdateAchievementFailedEvent != null ){
            UpdateAchievementFailedEvent(response.achievementId, response.error);
        }
        if (UpdateAchievementCompleted != null) {
            UpdateAchievementCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void UpdateAchievementSucceeded( string json ){
        AGSUpdateAchievementResponse response = AGSUpdateAchievementResponse.FromJSON (json);
        if( !response.IsError() && UpdateAchievementSucceededEvent != null ){
            UpdateAchievementSucceededEvent(response.achievementId);
        }
        if (UpdateAchievementCompleted != null) {
            UpdateAchievementCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestAchievementsForPlayerComplete( string json ) {
        if (RequestAchievementsForPlayerCompleted != null) {
            RequestAchievementsForPlayerCompleted(AGSRequestAchievementsForPlayerResponse.FromJSON(json));
        }
    }

#pragma warning restore 0618

    #region Obsolete events
    
    /// <summary>
    /// Event called when a request to update achievements fails
    /// </summary>
    /// <param name="achievementId">the id of the achievement that failed to update</param>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("UpdateAchievementFailedEvent is deprecated. Use UpdateAchievementCompleted instead.")]
    public static event Action<string,string> UpdateAchievementFailedEvent;
    
    /// <summary>
    /// Event called when a request to update achievements succeeds
    /// </summary>
    /// <param name="achievementId">the id of the achievement that has been updated</param>
    [Obsolete("UpdateAchievementSucceededEvent is deprecated. Use UpdateAchievementCompleted instead.")]
    public static event Action<string> UpdateAchievementSucceededEvent;

    /// <summary>
    /// Event called when a request to get all achievements succeeds
    /// </summary>
    /// <param name="achievementsList"></param>    
    [Obsolete("RequestAchievementsSucceededEvent is deprecated. Use RequestAchievementsCompleted instead.")]
    public static event Action<List<AGSAchievement>> RequestAchievementsSucceededEvent;
    
    /// <summary>
    /// Event called when a request to get all achievements has failed
    /// </summary>
    /// <param name="failureReason">a string indicating the reason for the request failure</param>    
    [Obsolete("RequestAchievementsFailedEvent is deprecated. Use RequestAchievementsCompleted instead.")]
    public static event Action<string> RequestAchievementsFailedEvent;

    #endregion
}
#endif
