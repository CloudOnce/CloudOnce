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
 */

#if UNITY_ANDROID && CLOUDONCE_AMAZON

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSUpdateAchievementResponse : AGSRequestResponse {

    public string achievementId;

    public static AGSUpdateAchievementResponse FromJSON(string json) {
        try {
            AGSUpdateAchievementResponse response = new AGSUpdateAchievementResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.achievementId = hashtable.ContainsKey("achievementId") ? hashtable ["achievementId"].ToString() : "";
            return response;
        } catch (Exception e){
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSUpdateAchievementResponse GetBlankResponseWithError(string error, string achievementId = "", int userData = 0){
        AGSUpdateAchievementResponse response = new AGSUpdateAchievementResponse ();
        response.error = error;
        response.userData = userData;
        response.achievementId = achievementId;
        return response;
    }

    public static AGSUpdateAchievementResponse GetPlatformNotSupportedResponse (string achievementId, int userData) {
        return GetBlankResponseWithError(PLATFORM_NOT_SUPPORTED_ERROR, achievementId, userData);
    }

}
#endif
