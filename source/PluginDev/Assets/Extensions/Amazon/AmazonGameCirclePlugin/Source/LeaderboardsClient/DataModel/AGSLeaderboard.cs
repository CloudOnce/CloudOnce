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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AGS leaderboard.
/// </summary>
public class AGSLeaderboard{
    public string name;
    public string id;
    public string displayText;
    public string scoreFormat;
    public string imageUrl;

    public static AGSLeaderboard fromHashtable( Hashtable hashtable ){
        var board = new AGSLeaderboard();
        board.name = hashtable["leaderboardName"].ToString();
        board.id = hashtable["leaderboardId"].ToString();
        board.displayText = hashtable["leaderboardDisplayText"].ToString();
        board.scoreFormat = hashtable["leaderboardScoreFormat"].ToString();
        board.imageUrl = hashtable ["leaderboardImageUrl"].ToString ();
        
        return board;
    }

	public static AGSLeaderboard GetBlankLeaderboard() {
		AGSLeaderboard leaderboard = new AGSLeaderboard ();
		leaderboard.name = "";
		leaderboard.id = "";
		leaderboard.displayText = "";
		leaderboard.scoreFormat = "";
		leaderboard.imageUrl = "";

		return leaderboard;
	}

    public override string ToString(){
        return string.Format( "name: {0}, id: {1}, displayText: {2}, scoreFormat: {3}, imageUrl: {4}", name, id, displayText, scoreFormat, imageUrl );
    }

}
#endif
