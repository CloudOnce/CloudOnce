/**
 * © 2012-2014 Amazon Digital Services, Inc. All rights reserved.
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
 * Added CLOUDONCE_AMAZON build symbol.
 * Removed iOS support.
 * Added ShowLeaderboardsOverlay overload for showing specific leaderboard.
 */

#if UNITY_ANDROID && CLOUDONCE_AMAZON

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// enum for the available leaderboard scopes
/// </summary>
public enum LeaderboardScope{
    GlobalAllTime,
    GlobalWeek,
    GlobalDay,
    FriendsAllTime
}

/// <summary>
/// Client used to submit and read leaderboards for the current logged in or guest player
/// </summary>
public class AGSLeaderboardsClient : MonoBehaviour{


#if UNITY_ANDROID && !UNITY_EDITOR
    private static AmazonJavaWrapper JavaObject;
    private static AmazonJavaWrapper JavaObjectEx;

    private static readonly string PROXY_CLASS_NAME = "com.amazon.ags.api.unity.LeaderboardsClientProxyImpl";
    private static readonly string EX_PROXY_CLASS_NAME = "com.amazon.ags.api.unity.LeaderboardsClientProxyExImpl";
#endif

    /// <summary>
    /// Occurs when submit score completed.
    /// </summary>
    public static event Action<AGSSubmitScoreResponse> SubmitScoreCompleted;

    /// <summary>
    /// Occurs when request leaderboards competed.
    /// </summary>
    public static event Action<AGSRequestLeaderboardsResponse> RequestLeaderboardsCompleted;

    /// <summary>
    /// Occurs when a request for local player score has completed.
    /// </summary>
    public static event Action<AGSRequestScoreResponse> RequestLocalPlayerScoreCompleted;

    /// <summary>
    /// Occurs when score request for a player has completed.
    /// </summary>
    public static event Action<AGSRequestScoreForPlayerResponse> RequestScoreForPlayerCompleted;

    /// <summary>
    /// Occurs when a request for scores has completed.
    /// </summary>
    public static event Action<AGSRequestScoresResponse> RequestScoresCompleted;

    /// <summary>
    /// Occurs when a percentile ranks request has completed.
    /// </summary>
    public static event Action<AGSRequestPercentilesResponse> RequestPercentileRanksCompleted;

    /// <summary>
    /// Occurs when a percentile ranks request for a player has completed.
    /// </summary>
    public static event Action<AGSRequestPercentilesForPlayerResponse> RequestPercentileRanksForPlayerCompleted;

#pragma warning disable 0618

    /// <summary>
    /// Initializes the <see cref="AGSLeaderboardsClient"/> class.
    /// </summary>
    static AGSLeaderboardsClient(){
#if UNITY_ANDROID && !UNITY_EDITOR
        JavaObject = new AmazonJavaWrapper();
        JavaObjectEx = new AmazonJavaWrapper();
        using( var PluginClass = new AndroidJavaClass( PROXY_CLASS_NAME ) ){

            if (PluginClass.GetRawClass() == IntPtr.Zero)
            {
                AGSClient.LogGameCircleWarning("No java class " + PROXY_CLASS_NAME + " present, can't use AGSLeaderboardsClient" );
                return;
            }
            JavaObject.setAndroidJavaObject(PluginClass.CallStatic<AndroidJavaObject>( "getInstance" ));
        }

        using( var PluginClass = new AndroidJavaClass( EX_PROXY_CLASS_NAME ) ){

            if (PluginClass.GetRawClass() == IntPtr.Zero)
            {
                AGSClient.LogGameCircleWarning("No java class " + EX_PROXY_CLASS_NAME + " present, can't use AGSLeaderboardsClient" );
                return;
            }
            JavaObjectEx.setAndroidJavaObject(PluginClass.CallStatic<AndroidJavaObject>( "getInstance" ));
        }
#endif
    }

    /// <summary>
    /// submit a score to leaderboard
    /// </summary>
    /// <remarks>
    /// SubmitScoreCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="leaderboardId">the id of the leaderboard for the score request</param>
    /// <param name="score">player score</param>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    /// </remarks>
    public static void SubmitScore( string leaderboardId, long score, int userData = 0 ) {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "submitScore", leaderboardId, score, userData );
#else
        AGSSubmitScoreResponse response = AGSSubmitScoreResponse.GetPlatformNotSupportedResponse(leaderboardId, userData);
        if( SubmitScoreFailedEvent != null ){
            SubmitScoreFailedEvent( response.leaderboardId, response.error );
        }
        if (SubmitScoreCompleted != null) {
            SubmitScoreCompleted(response);
        }
#endif
    }


    /// <summary>
    /// show leaderboard in GameCircle overlay
    /// </summary>
    public static void ShowLeaderboardsOverlay(){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "showLeaderboardsOverlay" );
#endif
    }

    /// <summary>
    /// show leaderboard in GameCircle overlay
    /// </summary>
    public static void ShowLeaderboardsOverlay(string leaderboardID)
    {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObjectEx.Call( "showLeaderboardOverlay", leaderboardID );
#endif
    }

    /// <summary>
    /// request all leaderboards for this game
    /// </summary>
    /// <remarks>
    /// RequestLeaderboardsCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestLeaderboards( int userData = 0 ){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestLeaderboards", 0 );
#else
        AGSRequestLeaderboardsResponse response = AGSRequestLeaderboardsResponse.GetPlatformNotSupportedResponse(userData);
        if( RequestLeaderboardsFailedEvent != null ){
            RequestLeaderboardsFailedEvent( response.error );
        }
        if (RequestLeaderboardsCompleted != null) {
            RequestLeaderboardsCompleted(response);
        }
#endif
    }

    /// <summary>
    /// request current player's score for a given leaderboad and scope
    /// </summary>
    /// <remarks>
    /// RequestLocalPlayerScoreCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="leaderboardId">the id of the leaderboard for the score request</param>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestLocalPlayerScore( string leaderboardId, LeaderboardScope scope, int userData = 0 ){
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestLocalPlayerScore", leaderboardId, (int)scope , 0);
#else
        AGSRequestScoreResponse response = AGSRequestScoreResponse.GetPlatformNotSupportedResponse(leaderboardId, scope, userData);
        if( RequestLocalPlayerScoreFailedEvent != null ){
            RequestLocalPlayerScoreFailedEvent( response.leaderboardId, response.error );
        }
        if (RequestLocalPlayerScoreCompleted != null) {
            RequestLocalPlayerScoreCompleted (response);
        }
#endif
    }

    /// <summary>
    /// ANDROID ONLY
    /// Requests the score for player.
    /// </summary>
    /// <remarks>
    /// RequestScoreForPlayerCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="leaderboardId">Leaderboard identifier.</param>
    /// <param name="playerId">Player identifier.</param>
    /// <param name="scope">Scope.</param>
    /// <param name="userData">
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestScoreForPlayer( string leaderboardId, string playerId, LeaderboardScope scope, int userData = 0 ) {
#if UNITY_EDITOR && UNITY_ANDROID
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestScoreForPlayer" , leaderboardId, playerId, (int)scope, userData);
#else
        if (RequestScoreForPlayerCompleted != null) {
            RequestScoreForPlayerCompleted(AGSRequestScoreForPlayerResponse.GetPlatformNotSupportedResponse(leaderboardId, playerId, scope, userData));
        }
#endif
    }

    /// <summary>
    /// Request scores
    /// </summary>
    /// <remarks>
    /// RequestScoresCompleted will be called if event is registered.
    /// </remarks>
    /// <param name="leaderboardId">the id of the leaderboard for the score request.</param>
    /// <param name="scope">enum value of leaderboard scope</param>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestScores( string leaderboardId, LeaderboardScope scope, int userData = 0 ) {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestScores", leaderboardId, (int)scope, userData );
#else
        if (RequestScoresCompleted != null) {
            RequestScoresCompleted(AGSRequestScoresResponse.GetPlatformNotSupportedResponse(leaderboardId, scope, userData));
        }
#endif
    }

    /// <summary>
    /// request percentile ranks
    /// </summary>
    /// <remarks>
    /// RequestPercentileRanksCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="leaderboardId">the id of the leaderboard for the score request.</param>
    /// <param name="scope">enum value of leaderboard scope</param>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestPercentileRanks( string leaderboardId, LeaderboardScope scope, int userData = 0 ) {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestPercentileRanks", leaderboardId, (int)scope, userData );
#else
        AGSRequestPercentilesResponse response = AGSRequestPercentilesResponse.GetPlatformNotSupportedResponse(leaderboardId, scope, userData);
        if(RequestPercentileRanksFailedEvent != null ){
            RequestPercentileRanksFailedEvent( response.leaderboardId, response.error );
        }
        if (RequestPercentileRanksCompleted != null) {
            RequestPercentileRanksCompleted(response);
        }
#endif
    }

    /// <summary>
    /// ANDROID ONLY
    /// Request percentile ranks for a player.
    /// </summary>
    /// <remarks>
    /// RequestPercentileRanksForPlayerCompleted will be called if the event is registered.
    /// </remarks>
    /// <param name="leaderboardId">the id of the leaderboard for the score request.</param>
    /// <param name="playerId">The player identifier for the request.</para>
    /// <param name="scope">enum value of leaderboard scope</param>
    /// <param name="userData">
    /// ANDROID ONLY
    /// An optional code that will be returned in the response. Used to associate a function call to its response.
    /// A value of 0 is not recommended because 0 is the value returned when userData is not specified.
    /// </param>
    public static void RequestPercentileRanksForPlayer(string leaderboardId, string playerId, LeaderboardScope scope, int userData = 0 ) {
#if UNITY_EDITOR && UNITY_ANDROID
        // GameCircle only functions on device.
#elif UNITY_ANDROID
        JavaObject.Call( "requestPercentileRanksForPlayer" , leaderboardId, playerId, (int)scope, userData);
#else
        if (RequestPercentileRanksForPlayerCompleted != null) {
            RequestPercentileRanksForPlayerCompleted(AGSRequestPercentilesForPlayerResponse.GetPlatformNotSupportedResponse(leaderboardId, playerId, scope, userData));
        }
#endif
    }

    #region Callbacks from native SDK

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void SubmitScoreFailed( string json ){
        AGSSubmitScoreResponse response = AGSSubmitScoreResponse.FromJSON (json);

        if ( response.IsError() && SubmitScoreFailedEvent != null ){
            SubmitScoreFailedEvent( response.leaderboardId , response.error);
        }
        if (SubmitScoreCompleted != null) {
            SubmitScoreCompleted(response);
        }

    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void SubmitScoreSucceeded( string json ){
        AGSSubmitScoreResponse response = AGSSubmitScoreResponse.FromJSON (json);
        if ( !response.IsError() && SubmitScoreSucceededEvent != null ){
            SubmitScoreSucceededEvent( response.leaderboardId );
        }
        if (SubmitScoreCompleted != null) {
            SubmitScoreCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestLeaderboardsFailed( string json ){
        AGSRequestLeaderboardsResponse response = AGSRequestLeaderboardsResponse.FromJSON (json);
        if( response.IsError() && RequestLeaderboardsFailedEvent != null ){
            RequestLeaderboardsFailedEvent(response.error);
        }
        if (RequestLeaderboardsCompleted != null) {
            RequestLeaderboardsCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestLeaderboardsSucceeded( string json ){
        AGSRequestLeaderboardsResponse response = AGSRequestLeaderboardsResponse.FromJSON (json);
        if (!response.IsError() && RequestLeaderboardsSucceededEvent != null) {
            RequestLeaderboardsSucceededEvent(response.leaderboards);
        }
        if (RequestLeaderboardsCompleted != null) {
            RequestLeaderboardsCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestLocalPlayerScoreFailed( string json ){
        AGSRequestScoreResponse response = AGSRequestScoreResponse.FromJSON (json);
        if (response.IsError () && RequestLocalPlayerScoreFailedEvent != null) {
            RequestLocalPlayerScoreFailedEvent(response.leaderboardId, response.error);
        }
        if (RequestLocalPlayerScoreCompleted != null) {
            RequestLocalPlayerScoreCompleted(response);
        }
    }

    /// <summary>
    ///  callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestLocalPlayerScoreSucceeded( string json ){
        AGSRequestScoreResponse response = AGSRequestScoreResponse.FromJSON (json);
        if (!response.IsError () && RequestLocalPlayerScoreSucceededEvent != null) {
            RequestLocalPlayerScoreSucceededEvent(response.leaderboardId, response.rank, response.score);
        }
        if (RequestLocalPlayerScoreCompleted != null) {
            RequestLocalPlayerScoreCompleted(response);
        }
    }

    public static void RequestScoreForPlayerComplete (string json) {
        if (RequestScoreForPlayerCompleted != null) {
            RequestScoreForPlayerCompleted(AGSRequestScoreForPlayerResponse.FromJSON(json));
        }
    }

    /// <summary>
    /// Callback method for native to pass score information to Unity.
    /// </summary>
    public static void RequestScoresSucceeded(string json) {
        if (RequestScoresCompleted != null) {
            RequestScoresCompleted(AGSRequestScoresResponse.FromJSON (json));
        }
    }
 
    /// <summary>
    /// Callback method for when native failed to collect score information
    /// </summary>
    public static void RequestScoresFailed(string json) {
        if (RequestScoresCompleted != null) {
            RequestScoresCompleted(AGSRequestScoresResponse.FromJSON (json));
        }
    }

    /// <summary>
    /// callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestPercentileRanksFailed( string json ){
        AGSRequestPercentilesResponse response = AGSRequestPercentilesResponse.FromJSON (json);
        if (response.IsError () && RequestPercentileRanksFailedEvent != null) {
            RequestPercentileRanksFailedEvent( response.leaderboardId, response.error );
        }
        if (RequestPercentileRanksCompleted != null) {
            RequestPercentileRanksCompleted(response);
        }
    }

    /// <summary>
    /// callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestPercentileRanksSucceeded( string json ){
        AGSRequestPercentilesResponse response = AGSRequestPercentilesResponse.FromJSON (json);
        if ( !response.IsError() && RequestPercentileRanksSucceededEvent != null ){
            RequestPercentileRanksSucceededEvent(response.leaderboard, response.percentiles, response.userIndex);
        }
        if (RequestPercentileRanksCompleted != null) {
            RequestPercentileRanksCompleted(response);
        }
    }

    /// <summary>
    /// callback method for native code to communicate events back to unity
    /// </summary>
    public static void RequestPercentileRanksForPlayerComplete( string json ){
        if (RequestPercentileRanksForPlayerCompleted != null) {
            RequestPercentileRanksForPlayerCompleted(AGSRequestPercentilesForPlayerResponse.FromJSON (json));
        }
    }

    #endregion

#pragma warning restore 0618

    #region Obsolete events

    /// <summary>
    /// Event called when a score submission fails
    /// </summary>
    /// <param name="leaderboardId">the id of the leaderboard that failed to update</param>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("SubmitScoreFailedEvent is deprecated. Use SubmitScoreCompleted instead.")]
    public static event Action<string,string> SubmitScoreFailedEvent;
    
    /// <summary>
    /// Event called when a score submission succeeds
    /// </summary>
    /// <param name="leaderboardId">the id of the leaderboard that a score has been submitted to</param>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("SubmitScoreSucceededEvent is deprecated. Use SubmitScoreCompleted instead.")]
    public static event Action<string> SubmitScoreSucceededEvent;

    /// <summary>
    /// Event called when a request for all game leaderboards fails
    /// </summary>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("RequestLeaderboardsFailedEvent is deprecated. Use RequestLeaderboardsCompleted instead.")]
    public static event Action<string> RequestLeaderboardsFailedEvent;
    
    /// <summary>
    /// Event called when a request for all game leaderboards succeeds
    /// </summary>
    /// <param name="leaderboardList">list of leaderboards for this game</param>
    [Obsolete("RequestLeaderboardsSucceededEvent is deprecated. Use RequestLeaderboardsCompleted instead.")]
    public static event Action<List<AGSLeaderboard>> RequestLeaderboardsSucceededEvent;

    /// <summary>
    /// Event called when a score request fails
    /// </summary>
    /// <param name="leaderboardId">the id of the leaderboard for the failed request</param>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("RequestLocalPlayerScoreFailedEvent is deprecated. Use RequestLocalPlayerScoreCompleted instead.")]
    public static event Action<string,string> RequestLocalPlayerScoreFailedEvent;
    
    /// <summary>
    /// Event called when a score request succeeds
    /// </summary>
    /// <param name="leaderboardId">the id of the leaderboard for the score request</param>
    /// <param name="rank">player's rank in leaderboard</param>
    /// <param name="score">player's score in leaderboard</param>
    [Obsolete("RequestLocalPlayerScoreSucceededEvent is deprecated. Use RequestLocalPlayerScoreCompleted instead.")]
    public static event Action<string,int,long> RequestLocalPlayerScoreSucceededEvent;

    /// <summary>
    /// Event called when a percentile request fails
    /// </summary>
    /// <param name="leaderboardId">the id of the leaderboard for the failed request</param>
    /// <param name="failureReason">a string indicating the failure reason</param>
    [Obsolete("RequestPercentileRanksFailedEvent is deprecated. Use RequestPercentileRanksCompleted instead.")]
    public static event Action<string,string> RequestPercentileRanksFailedEvent;
    
    /// <summary>
    /// Event called when a percentile request succeeds
    /// </summary>
    /// <param name="leaderboard">Leaderboard object</param>
    /// <param name="ranks">Player percentile rank and alias</param>
    [Obsolete("RequestPercentileRanksSucceededEvent is deprecated. Use RequestPercentileRanksCompleted instead.")]
    public static event Action<AGSLeaderboard, List<AGSLeaderboardPercentile>, int> RequestPercentileRanksSucceededEvent;

    #endregion

}
#endif
