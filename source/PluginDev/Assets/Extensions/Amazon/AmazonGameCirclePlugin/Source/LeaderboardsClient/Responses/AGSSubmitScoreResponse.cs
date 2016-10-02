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

public class AGSSubmitScoreResponse : AGSRequestResponse {

    public string leaderboardId;

    public static AGSSubmitScoreResponse FromJSON(string json) {
        try {
            AGSSubmitScoreResponse response = new AGSSubmitScoreResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.leaderboardId = hashtable.ContainsKey("leaderboardId") ? hashtable ["leaderboardId"].ToString() : "";
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSSubmitScoreResponse GetBlankResponseWithError(string error, string leaderboardId = "", int userData = 0) {
        AGSSubmitScoreResponse response = new AGSSubmitScoreResponse ();
        response.error = error;
        response.userData = userData;
        response.leaderboardId = leaderboardId;
        return response;
    }

    public static AGSSubmitScoreResponse GetPlatformNotSupportedResponse (string leaderboardId, int userData) {
        return GetBlankResponseWithError (PLATFORM_NOT_SUPPORTED_ERROR, leaderboardId, userData);
    }

}
#endif
