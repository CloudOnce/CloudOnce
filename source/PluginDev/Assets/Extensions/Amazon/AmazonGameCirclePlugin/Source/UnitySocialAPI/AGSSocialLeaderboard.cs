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

using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;

/// <summary>
/// GameCircle leaderboard implemention for Unity Social API.
/// </summary>
public class AGSSocialLeaderboard : ILeaderboard {
    // keep a reference to the GameCircle leaderboard, if available.
    readonly AGSLeaderboard leaderboard;
    public long localPlayerScore;
    public int localPlayerRank;
    private TimeScope _timeScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSocialLeaderboard"/> class.
    /// </summary>
    /// <param name='leaderboard'>
    /// Leaderboard.
    /// </param>
    public AGSSocialLeaderboard(AGSLeaderboard leaderboard) {
        if(null == leaderboard) {
            AGSClient.LogGameCircleError("AGSSocialLeaderboard constructor \"leaderboard\" argument should not be null");
            this.leaderboard = AGSLeaderboard.GetBlankLeaderboard();
        } else {
            this.leaderboard = leaderboard;
        }
        id = leaderboard.id;
        scores = new AGSSocialLeaderboardScore[0];
        localPlayerScore = -1;
        localPlayerRank = -1;
        _timeScope = TimeScope.AllTime;
    }
    
    /// <summary>
    /// Prevent people from calling default constructor.
    /// </summary>
    public AGSSocialLeaderboard() {
        leaderboard = AGSLeaderboard.GetBlankLeaderboard ();
        localPlayerScore = -1;
        localPlayerRank = -1;
        _timeScope = TimeScope.AllTime;
    }
    
    /// <summary>
    /// Checks if scores are available for this leaderboard
    /// </summary>
    /// <returns>
    /// if scores are available
    /// </returns>
    public bool ScoresAvailable() {
        return null != leaderboard && null != scores && scores.Length > 0 && localPlayerScore > -1 && localPlayerRank > -1;
    }
    
    #region ILeaderboard Implementation
    /// <summary>
    /// Gets a value indicating whether this <see cref="AGSSocialLeaderboard"/> is loading.
    /// </summary>
    /// <value>
    /// <c>true</c> if loading; otherwise, <c>false</c>.
    /// </value>
    public bool loading {
        get {   
            return !ScoresAvailable();
        }
    }
    
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string id {
        get;
        set;
    }
    
    /// <summary>
    /// Gets or sets the user scope.
    /// </summary>
    /// <value>
    /// The user scope.
    /// </value>
    public UserScope userScope {
        get;
        set;
    }
    
    /// <summary>
    /// Gets or sets the range.
    /// </summary>
    /// <value>
    /// The range.
    /// </value>
    public Range range {
        get;
        set;
    }
    
    /// <summary>
    /// Gets or sets the time scope.
    /// </summary>
    /// <value>
    /// The time scope.
    /// </value>
    public TimeScope timeScope {
        get {
            return _timeScope;
        }
        set {
            localPlayerScore = -1;
            localPlayerRank = -1;
            scores = new AGSSocialLeaderboardScore[0];
            _timeScope = value;
            LoadScores(null);
            GameCircleSocial.Instance.RequestLocalUserScore(this);
        }
    }
    
    /// <summary>
    /// Gets the local user score.
    /// </summary>
    /// <value>
    /// The local user score.
    /// </value>
    public IScore localUserScore {
        get {
            AGSScore score = new AGSScore();
            score.player = AGSSocialLocalUser.player;
            score.scoreValue = localPlayerScore;
            // Score string is not fully supported for local user.
            score.scoreString = localPlayerScore.ToString();
            score.rank = localPlayerRank;
            return new AGSSocialLeaderboardScore(score, leaderboard);
        }
    }
    
    /// <summary>
    /// Gets the max range.
    /// </summary>
    /// <value>
    /// The max range.
    /// </value>
    /// <exception cref='System.NotImplementedException'>
    /// Is thrown when a requested operation is not implemented for a given type.
    /// </exception>
    public uint maxRange {
        get {   
            AGSClient.LogGameCircleError("ILeaderboard.maxRange.get is not available for GameCircle");
            return 0;
        }
    }
    
    /// <summary>
    /// Gets the scores.
    /// </summary>
    /// <value>
    /// The scores.
    /// </value>
    public IScore[] scores {
        get;
        set;
    }
    
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string title {
        get {
            if (null == leaderboard) {
                return null;
            }
            return leaderboard.name;
        }
    }
    
    /// <summary>
    /// Sets the user filter.
    /// </summary>
    /// <param name='userIDs'>
    /// User I ds.
    /// </param>
    public void SetUserFilter(string[] userIDs) {   
        AGSClient.LogGameCircleError("ILeaderboard.SetUserFilter is not available for GameCircle");
    }
    
    /// <summary>
    /// Loads the scores.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadScores(System.Action<bool> callback) {   
        // if the GameCircle leaderboard is not set, then
        // the scores were not loaded.
        if(null == leaderboard) {
            callback(false);
            return;
        }
        GameCircleSocial.Instance.RequestScores (this, callback);
    }
    #endregion

}
#endif 
