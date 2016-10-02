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
 * Added CloudOnceAmazon build symbol.
 * Removed iOS support.
 */

#if UNITY_ANDROID && CloudOnceAmazon

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// GameCircle Unity Social API implementation
/// </summary>
public class GameCircleSocial : ISocialPlatform {
    
    #region local variables
    AGSSocialLocalUser gameCircleLocalUser = new AGSSocialLocalUser();
    private int requestID;
    private Action<bool> authenticationCallback;
    private Dictionary<int, Action<bool>> simpleCallbacks;
    private Dictionary<int, Action<IAchievementDescription[]>> loadAchievementDescriptionsCallbacks;
    private Dictionary<int, Action<IAchievement[]>> loadAchievementsCallbacks;
    private Dictionary<int, AGSSocialLeaderboard> leaderboardForRequest;
    private Dictionary<int, Action<IScore[]>> loadScoresCallbacks;
    #endregion
    
    #region static variables
    // keep a static instance of the GameCircleSocial plugin around.
    static GameCircleSocial socialInstance = new GameCircleSocial();
    #endregion
    
    #region Constructor
    private GameCircleSocial() {
        requestID = 1;

        simpleCallbacks = new Dictionary<int, Action<bool>> ();
        loadAchievementDescriptionsCallbacks = new Dictionary<int, Action<IAchievementDescription[]>> ();
        loadAchievementsCallbacks = new Dictionary<int, Action<IAchievement[]>> ();
        leaderboardForRequest = new Dictionary<int, AGSSocialLeaderboard> ();
        loadScoresCallbacks = new Dictionary<int, Action<IScore[]>> ();

        AGSClient.ServiceReadyEvent += OnServiceReady;
        AGSClient.ServiceNotReadyEvent += OnServiceNotReady;
        AGSAchievementsClient.UpdateAchievementCompleted += OnUpdateAchievementCompleted;
        AGSAchievementsClient.RequestAchievementsCompleted += OnRequestAchievementsCompleted;
        AGSLeaderboardsClient.SubmitScoreCompleted += OnSubmitScoreCompleted;
        AGSLeaderboardsClient.RequestScoresCompleted += OnRequestScoresCompleted;
        AGSLeaderboardsClient.RequestLocalPlayerScoreCompleted += OnRequestLocalPlayerScoreCompleted;
        AGSPlayerClient.RequestLocalPlayerCompleted += OnRequestPlayerCompleted;
        AGSPlayerClient.RequestFriendIdsCompleted += OnRequestFriendIdsCompleted;
        AGSPlayerClient.RequestBatchFriendsCompleted += OnRequestBatchFriendsCompleted;
    }
    #endregion

    #region static interface
    /// <summary>
    /// Gets the GameCircleSocial instance.
    /// </summary>
    /// <value>
    /// The GameCircleSocial instance.
    /// </value>
    public static GameCircleSocial Instance {
        get { return socialInstance; }
    }
    #endregion
    
    #region ISocialPlatform Overrides
    /// <summary>
    /// Gets the local user.
    /// </summary>
    /// <value>
    /// The local user.
    /// </value>
    public ILocalUser localUser {
        get { return gameCircleLocalUser; }
    }
    
    /// <summary>
    /// Loads the users.
    /// </summary>
    /// <param name='userIDs'>
    /// User IDs.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadUsers (string[] userIDs, Action<IUserProfile[]> callback) {
        AGSClient.LogGameCircleError("ISocialPlatform.LoadUsers is not available for GameCircle");
    }
    
    /// <summary>
    /// Reports the achievement progress.
    /// </summary>
    /// <param name='achievementID'>
    /// Achievement ID.
    /// </param>
    /// <param name='progress'>
    /// Progress.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void ReportProgress(string achievementID, double progress, System.Action<bool> callback) {
        simpleCallbacks.Add(requestID, callback);
        AGSAchievementsClient.UpdateAchievementProgress(achievementID, (float) progress, requestID++);
    }
    
    /// <summary>
    /// Loads the achievement descriptions.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadAchievementDescriptions(System.Action<IAchievementDescription[]> callback) {
        // The callback argument should not be null for this function.
        if(null == callback) {
            AGSClient.LogGameCircleError("LoadAchievementDescriptions \"callback\" argument should not be null");
            return;
        }
        loadAchievementDescriptionsCallbacks.Add (requestID, callback);
        AGSAchievementsClient.RequestAchievements(requestID++);
        
    }
    
    /// <summary>
    /// Loads the achievements.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadAchievements(System.Action<IAchievement[]> callback) {
        // The callback argument should not be null for this function.
        if(null == callback) {
            AGSClient.LogGameCircleError("LoadAchievements \"callback\" argument should not be null");
            return;
        }
        loadAchievementsCallbacks.Add (requestID, callback);
        AGSAchievementsClient.RequestAchievements(requestID++);
    }

    /// <summary>
    /// Creates the achievement.
    /// </summary>
    /// <returns>
    /// The achievement.
    /// </returns>
    public IAchievement CreateAchievement() {
        return new AGSSocialAchievement();
    }
    
    /// <summary>
    /// Reports the score.
    /// </summary>
    /// <param name='score'>
    /// Score.
    /// </param>
    /// <param name='board'>
    /// Board.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void ReportScore(long score, string board, System.Action<bool> callback) {
        simpleCallbacks.Add (requestID, callback);
        AGSLeaderboardsClient.SubmitScore (board, score, requestID++);
    }

    /// <summary>
    /// Loads the scores.
    /// </summary>
    /// <param name='leaderboardID'>
    /// Leaderboard I.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadScores(string leaderboardID, Action<IScore[]> callback) {
        loadScoresCallbacks.Add (requestID, callback);
        AGSLeaderboardsClient.RequestLeaderboards(requestID++);
    }
    
    /// <summary>
    /// Creates the leaderboard.
    /// </summary>
    /// <returns>
    /// The leaderboard.
    /// </returns>
    public ILeaderboard CreateLeaderboard() {
        return new AGSSocialLeaderboard();
    }
    
    /// <summary>
    /// Shows the achievements UI.
    /// </summary>
    public void ShowAchievementsUI() {
        AGSAchievementsClient.ShowAchievementsOverlay();
    }
    
    /// <summary>
    /// Shows the leaderboard UI.
    /// </summary>
    public void ShowLeaderboardUI() {
        AGSLeaderboardsClient.ShowLeaderboardsOverlay();
    }
    
    /// <summary>
    /// Authenticate the specified user and callback.
    /// </summary>
    /// <param name='user'>
    /// User.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void Authenticate(ILocalUser user, System.Action<bool> callback) {
        // Forward the AGSClient callbacks to the passed in callback.
        authenticationCallback = callback;
        // If using GameCircle with the Unity Social API, 
        // initialize it with leaderboards and achievements, but not whispersync.
        AGSClient.Init(/*Leaderboards*/true,/*Achievements*/true,/*Whispersync*/false);
    }
    
    /// <summary>
    /// Loads the friends.
    /// </summary>
    /// <param name='user'>
    /// User.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadFriends(ILocalUser user, System.Action<bool> callback) {
        if(user == null) {
            AGSClient.LogGameCircleError("LoadFriends \"user\" argument should not be null");
            return;
        }
        user.LoadFriends(callback);
    }
    
    /// <summary>
    /// Loads the scores.
    /// </summary>
    /// <param name='board'>
    /// Board.
    /// </param>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadScores(ILeaderboard board, System.Action<bool> callback) {
        // This function doesn't do anything with a null leaderboard.
        if(null == board) {
            AGSClient.LogGameCircleError("LoadScores \"board\" argument should not be null");
            return;
        }
        board.LoadScores(callback);
    }
    
    /// <summary>
    /// Gets the loading status of the leaderboard.
    /// </summary>
    /// <returns>
    /// The loading.
    /// </returns>
    /// <param name='board'>
    /// If set to <c>true</c> board.
    /// </param>
    public bool GetLoading(ILeaderboard board) {
        // This function doesn't do anything with a null leaderboard.
        if(null == board) {
            AGSClient.LogGameCircleError("GetLoading \"board\" argument should not be null");
            return false;
        }
        return board.loading;
    }
    #endregion

    #region SDK wrappers

    public void RequestScores(AGSSocialLeaderboard leaderboard, Action<bool> callback) {
        leaderboardForRequest.Add (requestID, leaderboard);
        simpleCallbacks.Add (requestID, callback);
        AGSLeaderboardsClient.RequestScores (leaderboard.id, fromTimeScope (leaderboard.timeScope), requestID++);
    }

    public void RequestLocalUserScore(AGSSocialLeaderboard leaderboard) {
        leaderboardForRequest.Add (requestID, leaderboard);
        AGSLeaderboardsClient.RequestScores (leaderboard.id, fromTimeScope (leaderboard.timeScope), requestID++);
    }

    public void RequestLocalPlayer(Action<bool> callback) {
        simpleCallbacks.Add (requestID, callback);
        AGSPlayerClient.RequestLocalPlayer (requestID++);
    }

    public void RequestFriends(Action<bool> callback) {
        simpleCallbacks.Add (requestID, callback);
        AGSPlayerClient.RequestFriendIds (requestID++);
    }

    #endregion

    #region SDK event delegates

    private void OnServiceReady() {
        if (authenticationCallback != null) {
            authenticationCallback(true);
        }
    }

    private void OnServiceNotReady(string error) {
        if (authenticationCallback != null) {
            authenticationCallback(false);
        }
    }

    private void OnUpdateAchievementCompleted( AGSUpdateAchievementResponse response ) {
        Action<bool> callback = simpleCallbacks.ContainsKey(response.userData) ? simpleCallbacks[response.userData] : null;
        if (null != callback) {
            callback(!response.IsError());
        }
        simpleCallbacks.Remove (response.userData);
    }

    private void OnRequestAchievementsCompleted( AGSRequestAchievementsResponse response ) {
        if (loadAchievementDescriptionsCallbacks.ContainsKey(response.userData)) {
            Action<IAchievementDescription[]> callback = loadAchievementDescriptionsCallbacks.ContainsKey (response.userData) ? loadAchievementDescriptionsCallbacks[response.userData] : null;
            if (callback != null) {
                AGSSocialAchievement[] descriptions = new AGSSocialAchievement[response.achievements.Count];
                for (int i = 0; i < response.achievements.Count; i++) {
                    descriptions[i] = new AGSSocialAchievement(response.achievements[i]);
                }
                callback(descriptions);
            }
        }
        if (loadAchievementsCallbacks.ContainsKey (response.userData)) {
            Action<IAchievement[]> callback = loadAchievementsCallbacks.ContainsKey(response.userData) ? loadAchievementsCallbacks[response.userData] : null;
            if (callback != null) {
                AGSSocialAchievement[] descriptions = new AGSSocialAchievement[response.achievements.Count];
                for (int i = 0; i < response.achievements.Count; i++) {
                    descriptions[i] = new AGSSocialAchievement(response.achievements[i]);
                }
                callback(descriptions);
            }
        }
        loadAchievementDescriptionsCallbacks.Remove (response.userData);
    }

    private void OnSubmitScoreCompleted (AGSSubmitScoreResponse response) {
        Action<bool> callback = simpleCallbacks.ContainsKey(response.userData) ? simpleCallbacks[response.userData] : null;
        if (null != callback) {
            callback(!response.IsError());
        }
        simpleCallbacks.Remove (response.userData);
    }

    private void OnRequestScoresCompleted (AGSRequestScoresResponse response) {
        // Put scores in leaderboard.
        AGSSocialLeaderboard leaderboard = leaderboardForRequest.ContainsKey (response.userData) ? leaderboardForRequest[response.userData] : null;
        if (null != leaderboard && !response.IsError()) {
            leaderboard.scores = new IScore[response.scores.Count];
            for (int i = 0; i < response.scores.Count; i++) {
                leaderboard.scores[i] = new AGSSocialLeaderboardScore(response.scores[i], response.leaderboard);
            }
        }
        // Handle any callbacks
        Action<bool> callback = simpleCallbacks.ContainsKey(response.userData) ? simpleCallbacks[response.userData] : null;
        if (null != callback) {
            callback(!response.IsError());
        }
        Action<IScore[]> scoreCallback = loadScoresCallbacks.ContainsKey (response.userData) ? loadScoresCallbacks[response.userData] : null;
        if (null != scoreCallback) {
            IScore[] scores = new IScore[response.scores.Count];
            for (int i = 0; i < response.scores.Count; i++) {
                scores[i] = new AGSSocialLeaderboardScore(response.scores[i], response.leaderboard);
            }
            scoreCallback(scores);
        }
        // cleanup
        leaderboardForRequest.Remove (response.userData);
        simpleCallbacks.Remove (response.userData);
    }

    private void OnRequestLocalPlayerScoreCompleted (AGSRequestScoreResponse response) {
        AGSSocialLeaderboard leaderboard = leaderboardForRequest.ContainsKey (response.userData) ? leaderboardForRequest[response.userData] : null;
        if (null != leaderboard) {
            leaderboard.localPlayerScore = response.score;
            leaderboard.localPlayerRank = response.rank;
        }
        leaderboardForRequest.Remove (response.userData);
    }

    private void OnRequestPlayerCompleted(AGSRequestPlayerResponse response) {
        AGSSocialLocalUser.player = response.player;
        Action<bool> callback = simpleCallbacks.ContainsKey(response.userData) ? simpleCallbacks[response.userData] : null;
        if (null != callback) {
            callback(!response.IsError());
        }
        simpleCallbacks.Remove (response.userData);
    }

    private void OnRequestFriendIdsCompleted (AGSRequestFriendIdsResponse response) {
        if (response.IsError()) {
            Action<bool> callback = simpleCallbacks.ContainsKey(response.userData) ? simpleCallbacks[response.userData] : null;
            if (callback != null) {
                callback(false);
            }
            simpleCallbacks.Remove(response.userData);
        } else {
            AGSPlayerClient.RequestBatchFriends(response.friendIds, response.userData);
        }
    }

    private void OnRequestBatchFriendsCompleted (AGSRequestBatchFriendsResponse response) {
        if (!response.IsError ()) {
            AGSSocialLocalUser.friendList = new List<AGSSocialUser>();
            foreach (AGSPlayer player in response.friends) {
                AGSSocialLocalUser.friendList.Add(new AGSSocialUser(player));
            }
        }
        Action<bool> callback = simpleCallbacks.ContainsKey(response.userData) ? simpleCallbacks[response.userData] : null;
        if (callback != null) {
            callback(!response.IsError());
        }
        simpleCallbacks.Remove(response.userData);
    }

    #endregion

    #region Private helpers
    private LeaderboardScope fromTimeScope (TimeScope scope) {
        switch (scope) {
        case TimeScope.Today:
            return LeaderboardScope.GlobalDay;
        case TimeScope.Week:
            return LeaderboardScope.GlobalWeek;
        case TimeScope.AllTime:
            return LeaderboardScope.GlobalAllTime;
        default:
            return LeaderboardScope.GlobalAllTime;
        }
    }
    #endregion
}
#endif
