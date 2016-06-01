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
 * Modified by Trollpants Game Studio AS.
 * Added TP_AndroidAmazon build symbol.
 * Removed iOS support.
 */

#if UNITY_ANDROID && TP_AndroidAmazon

using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// AGS social leaderboard score.
/// </summary>
public class AGSSocialLeaderboardScore : IScore {
    // keep a reference to the GameCircle score, if available.
    readonly AGSScore score;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSocialLeaderboardScore"/> class.
    /// </summary>
    /// <param name='score'>
    /// Score.
    /// </param>
    /// <param name='leaderboard'>
    /// Leaderboard.
    /// </param>
    public AGSSocialLeaderboardScore(AGSScore score, AGSLeaderboard leaderboard) {
        if(null == score) {
            AGSClient.LogGameCircleError("AGSSocialLeaderboardScore constructor \"score\" argument should not be null");
            return;
        }
        if(null == leaderboard) {
            AGSClient.LogGameCircleError("AGSSocialLeaderboardScore constructor \"leaderboard\" argument should not be null");
            return;
        }
        this.score = score;
        leaderboardID = leaderboard.id;
        value = score.scoreValue;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSocialLeaderboardScore"/> class.
    /// </summary>
    public AGSSocialLeaderboardScore() {
        this.score = null;
        leaderboardID = null;
    }
    
    #region IScore implementation
    /// <summary>
    /// Gets or sets the leaderboard ID.
    /// </summary>
    /// <value>
    /// The leaderboard ID.
    /// </value>
    public string leaderboardID {
        get;
        set;
    }
    
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public long value {
        get;
        set;
    }
    
    /// <summary>
    /// Gets the date.
    /// </summary>
    /// <value>
    /// The date.
    /// </value>
    public System.DateTime date {
        get {
            AGSClient.LogGameCircleError("IScore.date.get is not available for GameCircle");
            return System.DateTime.MinValue;
        }
    }
    
    /// <summary>
    /// Gets the formatted value.
    /// </summary>
    /// <value>
    /// The formatted value.
    /// </value>
    public string formattedValue {
        get {
            if(null == score) {
                return null;
            }
            return score.scoreString;
        }
    }
    
    /// <summary>
    /// Gets the user ID.
    /// </summary>
    /// <value>
    /// The user ID.
    /// </value>
    public string userID {
        get {
            if(null == score) {
                return null;
            }
            return score.player.alias;
        }
    }
    
    /// <summary>
    /// Gets the rank.
    /// </summary>
    /// <value>
    /// The rank.
    /// </value>
    public int rank {
        get {
            if(null == score) {
                return 0;
            }
            return score.rank;
        }
    }
    
    /// <summary>
    /// Reports the score.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void ReportScore(System.Action<bool> callback) {
        GameCircleSocial.Instance.ReportScore (value, leaderboardID, callback);
        AGSLeaderboardsClient.SubmitScore(leaderboardID,value);
    }
    #endregion
}
#endif
