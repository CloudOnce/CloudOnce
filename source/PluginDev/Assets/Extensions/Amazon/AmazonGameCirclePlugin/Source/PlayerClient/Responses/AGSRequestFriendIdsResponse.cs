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

public class AGSRequestFriendIdsResponse : AGSRequestResponse {

    public List<string> friendIds;

    public static AGSRequestFriendIdsResponse FromJSON(string json) {
        try {
            AGSRequestFriendIdsResponse response = new AGSRequestFriendIdsResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.friendIds = new List<string> ();
			if (hashtable.ContainsKey ("friendIds")) {
				foreach (string friendPlayerId in hashtable ["friendIds"] as ArrayList) {
                    response.friendIds.Add(friendPlayerId);
                }
            }
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestFriendIdsResponse GetBlankResponseWithError (string error, int userData = 0) {
        AGSRequestFriendIdsResponse response = new AGSRequestFriendIdsResponse ();
        response.error = error;
        response.userData = userData;
        response.friendIds = new List<string> ();
        return response;
    }

    public static AGSRequestFriendIdsResponse GetPlatformNotSupportedResponse (int userData) {
        return GetBlankResponseWithError (PLATFORM_NOT_SUPPORTED_ERROR, userData);
    }

}
#endif
