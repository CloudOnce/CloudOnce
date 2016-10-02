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
 */

#if UNITY_ANDROID && CloudOnceAmazon

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSRequestScoresResponse : AGSRequestResponse {

    public string leaderboardId;
    public AGSLeaderboard leaderboard;
    public LeaderboardScope scope;
    public List<AGSScore> scores;

    public static AGSRequestScoresResponse FromJSON(string json){
        try {
            AGSRequestScoresResponse response = new AGSRequestScoresResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.leaderboardId = hashtable.ContainsKey("leaderboardId") ? hashtable ["leaderboardId"].ToString() : "";
            if (hashtable.ContainsKey("leaderboard")) {
                response.leaderboard = AGSLeaderboard.fromHashtable (hashtable ["leaderboard"] as Hashtable);
            } else {
                response.leaderboard = AGSLeaderboard.GetBlankLeaderboard();
            }
            response.scores = new List<AGSScore> ();
            if (hashtable.Contains ("scores")) {
                foreach (Hashtable scoreHashtable in hashtable ["scores"] as ArrayList) {
                    response.scores.Add(AGSScore.fromHashtable(scoreHashtable));
                }
            }
            response.scope = (LeaderboardScope) Enum.Parse(typeof(LeaderboardScope), hashtable["scope"].ToString());
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestScoresResponse GetBlankResponseWithError(string error,
                                                                     string leaderboardId = "",
                                                                     LeaderboardScope scope = LeaderboardScope.GlobalAllTime,
                                                                     int userData = 0) {
        AGSRequestScoresResponse response = new AGSRequestScoresResponse ();
        response.error = error;
        response.userData = userData;
        response.leaderboardId = leaderboardId;
        response.scope = scope;
        response.scores = new List<AGSScore>();
        return response;
    }

    public static AGSRequestScoresResponse GetPlatformNotSupportedResponse (string leaderboardId, LeaderboardScope scope, int userData) {
        return GetBlankResponseWithError (PLATFORM_NOT_SUPPORTED_ERROR, leaderboardId, scope, userData);
    }

}
#endif
