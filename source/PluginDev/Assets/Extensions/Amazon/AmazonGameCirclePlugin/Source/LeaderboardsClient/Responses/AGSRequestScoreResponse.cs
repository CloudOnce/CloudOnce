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
 * Modified by Trollpants Game Studio AS.
 * Added TP_AndroidAmazon build symbol.
 */

#if UNITY_ANDROID && TP_AndroidAmazon

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSRequestScoreResponse : AGSRequestResponse {

    public string leaderboardId;
    public LeaderboardScope scope;
    public int rank;
    public long score;

    public static AGSRequestScoreResponse FromJSON(string json) {
        try {
            AGSRequestScoreResponse response = new AGSRequestScoreResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.leaderboardId = hashtable.ContainsKey("leaderboardId") ? hashtable ["leaderboardId"].ToString() : "";
            response.rank = hashtable.ContainsKey ("rank") ? int.Parse (hashtable ["rank"].ToString ()) : -1;
            response.score = hashtable.ContainsKey ("score") ? long.Parse (hashtable ["score"].ToString ()) : -1;
            response.scope = (LeaderboardScope) Enum.Parse(typeof(LeaderboardScope), hashtable["scope"].ToString());
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestScoreResponse GetBlankResponseWithError (string error,
                                                                     string leaderboardId = "",
                                                                     LeaderboardScope scope = LeaderboardScope.GlobalAllTime,
                                                                     int userData = 0) {
        AGSRequestScoreResponse response = new AGSRequestScoreResponse ();
        response.error = error;
        response.userData = userData;
        response.leaderboardId = leaderboardId;
        response.scope = scope;
        response.rank = -1;
        response.score = -1;
        return response;
    }

    public static AGSRequestScoreResponse GetPlatformNotSupportedResponse (string leaderboardId, LeaderboardScope scope, int userData) {
        return GetBlankResponseWithError(PLATFORM_NOT_SUPPORTED_ERROR, leaderboardId, scope, userData);
    }

}
#endif
