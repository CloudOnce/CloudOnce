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
 */

#if UNITY_ANDROID && CLOUDONCE_AMAZON

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSRequestLeaderboardsResponse : AGSRequestResponse {

    public List<AGSLeaderboard> leaderboards;

    public static AGSRequestLeaderboardsResponse FromJSON(string json) {
        try {
            AGSRequestLeaderboardsResponse response = new AGSRequestLeaderboardsResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.leaderboards = new List<AGSLeaderboard>();
            if (hashtable.ContainsKey("leaderboards")) {
                foreach (Hashtable leaderboardHashtable in hashtable["leaderboards"] as ArrayList) {
                    response.leaderboards.Add(AGSLeaderboard.fromHashtable(leaderboardHashtable));
                }
            }
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestLeaderboardsResponse GetBlankResponseWithError(string error, int userData = 0) {
        AGSRequestLeaderboardsResponse response = new AGSRequestLeaderboardsResponse ();
        response.error = error;
        response.userData = userData;
        response.leaderboards = new List<AGSLeaderboard>();
        return response;
    }

    public static AGSRequestLeaderboardsResponse GetPlatformNotSupportedResponse (int userData) {
        return GetBlankResponseWithError (PLATFORM_NOT_SUPPORTED_ERROR, userData);
    }

}
#endif
