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

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSRequestAchievementsForPlayerResponse : AGSRequestAchievementsResponse {

    public string playerId;

    public new static AGSRequestAchievementsForPlayerResponse FromJSON(string json) {
        try {
            AGSRequestAchievementsForPlayerResponse response = new AGSRequestAchievementsForPlayerResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.achievements = new List<AGSAchievement>();
            if (hashtable.ContainsKey("achievements")) {
                foreach (Hashtable achievementHashtable in hashtable["achievements"] as ArrayList) {
                    response.achievements.Add(AGSAchievement.fromHashtable(achievementHashtable));
                }
            }
            response.playerId = hashtable.ContainsKey ("playerId") ? hashtable ["playerId"].ToString () : "";
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestAchievementsForPlayerResponse GetBlankResponseWithError(string error, string playerId = "", int userData = 0) {
        AGSRequestAchievementsForPlayerResponse response = new AGSRequestAchievementsForPlayerResponse ();
        response.error = error;
        response.playerId = playerId;
        response.userData = userData;
        response.achievements = new List<AGSAchievement>();
        return response;
    }

    public static AGSRequestAchievementsForPlayerResponse GetPlatformNotSupportedResponse (string playerId, int userData) {
        return GetBlankResponseWithError (PLATFORM_NOT_SUPPORTED_ERROR, playerId, userData);
    }

}
#endif
