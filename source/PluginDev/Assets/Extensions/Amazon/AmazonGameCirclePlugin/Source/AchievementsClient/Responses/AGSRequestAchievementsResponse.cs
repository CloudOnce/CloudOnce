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
 * Removed iOS support.
 */

#if UNITY_ANDROID && TP_AndroidAmazon

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSRequestAchievementsResponse : AGSRequestResponse {

    public List<AGSAchievement> achievements;

    public static AGSRequestAchievementsResponse FromJSON(string json) {
        try {
            AGSRequestAchievementsResponse response = new AGSRequestAchievementsResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.achievements = new List<AGSAchievement>();
            if (hashtable.ContainsKey("achievements")) {
                foreach (Hashtable achievementHashtable in hashtable["achievements"] as ArrayList) {
                    response.achievements.Add(AGSAchievement.fromHashtable(achievementHashtable));
                }
            }
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestAchievementsResponse GetBlankResponseWithError(string error, int userData = 0) {
        AGSRequestAchievementsResponse response = new AGSRequestAchievementsResponse ();
        response.error = error;
        response.userData = userData;
        response.achievements = new List<AGSAchievement>();
        return response;
    }

    public static AGSRequestAchievementsResponse GetPlatformNotSupportedResponse (int userData) {
        return GetBlankResponseWithError(PLATFORM_NOT_SUPPORTED_ERROR, userData);
    }

}
#endif
