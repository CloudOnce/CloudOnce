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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AGS score.
/// </summary>
public class AGSScore{
    public AGSPlayer player;
    public int rank;
    public string scoreString;
    public long scoreValue;
    
    public static AGSScore fromHashtable( Hashtable scoreHashTable ){
        var score = new AGSScore();
        score.player = AGSPlayer.fromHashtable(scoreHashTable["player"] as Hashtable);
        score.rank = int.Parse( scoreHashTable["rank"].ToString() );
		score.scoreString = scoreHashTable["scoreString"].ToString();
        score.scoreValue = long.Parse( scoreHashTable["score"].ToString() );
		return score;
    }

    public static List<AGSScore> fromArrayList( ArrayList list ){
        var scores = new List<AGSScore>();

        foreach( Hashtable ht in list ){
            scores.Add( AGSScore.fromHashtable( ht ) );
        }
        
        return scores;
    }

    public override string ToString(){
        return string.Format( "player: {0}, rank: {1}, scoreValue: {2}, scoreString: {3}", player.ToString(), rank, scoreValue, scoreString );
    }

}
#endif
