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

using System.Collections;
using System;
public class AGSLeaderboardPercentile {

    private const string percentileKey = "percentile";
    private const string scoreKey = "score";
    private const string playerKey = "player";

    public int percentile;
    public long score;
    public AGSPlayer player;

    public static AGSLeaderboardPercentile fromHashTable( Hashtable percentilesHashtable ){
        AGSLeaderboardPercentile leaderboardPercentile = new AGSLeaderboardPercentile();
        try {
            leaderboardPercentile.percentile = int.Parse(percentilesHashtable[percentileKey].ToString());
            leaderboardPercentile.score = long.Parse(percentilesHashtable[scoreKey].ToString());
        } catch (FormatException e){
            AGSClient.Log ("Unable to parse percentile item " + e.Message);
        }
        leaderboardPercentile.player = AGSPlayer.fromHashtable(percentilesHashtable[playerKey] as Hashtable);
        return leaderboardPercentile;
    }

    public override string ToString(){
        return string.Format( "player: {0}, score: {1}, percentile: {2}", player.ToString(), score, percentile);
    }
}
#endif
