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

public class AGSRequestBatchFriendsResponse : AGSRequestResponse{

    public List<AGSPlayer> friends;

    public static AGSRequestBatchFriendsResponse FromJSON(string json) {
        try {
            AGSRequestBatchFriendsResponse response = new AGSRequestBatchFriendsResponse();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.friends = new List<AGSPlayer>();
            if (hashtable.ContainsKey("friends")) {
                foreach ( Hashtable playerHashtable in hashtable["friends"] as ArrayList ) {
                    response.friends.Add(AGSPlayer.fromHashtable(playerHashtable));
                }
            }
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestBatchFriendsResponse GetBlankResponseWithError(string error,
                                                                           List<String> friendIdsRequested = null,
                                                                           int userData = 0) {
        AGSRequestBatchFriendsResponse response = new AGSRequestBatchFriendsResponse ();
        response.error = error;
        response.friends = new List<AGSPlayer> ();
        if (friendIdsRequested != null) {
            foreach (string friendId in friendIdsRequested) {
                response.friends.Add(AGSPlayer.BlankPlayerWithID(friendId));
            }
        }
        response.userData = userData;
        return response;
    }

    public static AGSRequestBatchFriendsResponse GetPlatformNotSupportedResponse(List<String> friendIdsRequested, int userData) {
        return GetBlankResponseWithError (PLATFORM_NOT_SUPPORTED_ERROR, friendIdsRequested, userData);
    }

}
#endif
