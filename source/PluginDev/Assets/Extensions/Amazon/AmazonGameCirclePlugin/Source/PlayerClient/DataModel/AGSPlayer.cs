/**
 * © 2012-2013 Amazon Digital Services, Inc. All rights reserved.
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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// AGS player.
/// </summary>
public class AGSPlayer{
    /// <summary>
    /// player alias.
    /// </summary>
    public readonly string alias;
    /// <summary>
    /// The player identifier.
    /// </summary>
    public readonly string playerId;
    /// <summary>
    /// The player identifier.
    /// </summary>
    public readonly string avatarUrl;

    #region JSON keys
    private const string aliasKey = "alias";
    private const string playerIdKey = "playerId";
    private const string avatarUrlKey = "avatarUrl";
    #endregion
    
    #region public static functions
    /// <summary>
    /// creates a new instance of the <see cref="AGSPlayer"/> class from a hashtable
    /// </summary>
    /// <returns>
    /// A <see cref="AGSPlayer"/> class created from the passed in hashtable.
    /// </returns>
    /// <param name='hashtable'>  
    /// hashtable.
    /// </param>
    public static AGSPlayer fromHashtable( Hashtable playerDataAsHashtable ){
        try {
            return new AGSPlayer(playerDataAsHashtable.ContainsKey(aliasKey) ? playerDataAsHashtable[aliasKey].ToString() : "",
                                 playerDataAsHashtable.ContainsKey(playerIdKey) ? playerDataAsHashtable[playerIdKey].ToString() : "",
                                 playerDataAsHashtable.ContainsKey(avatarUrlKey) ? playerDataAsHashtable[avatarUrlKey].ToString() : "" );
        } catch (Exception e) {
            AGSClient.LogGameCircleError("Returning blank player due to exception getting player from hashtable: " + e.ToString());
            return GetBlankPlayer();
        }
    }

    public static AGSPlayer GetBlankPlayer(){
        return new AGSPlayer ("", "", "");
    }

    public static AGSPlayer BlankPlayerWithID(string playerId) {
        return new AGSPlayer ("", playerId, "");
    }

    #endregion
    
    #region private constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AGSPlayer"/> class.
    /// This constructor is private because this class should only be 
    /// instantiated through fromHashtable
    /// </summary>
    /// <param name='alias'>
    /// Alias.
    /// </param>
    /// <param name='playerId'>
    /// Player identifier.
    /// </param>
    /// <param name='avatarUrl'>
    /// Avatar Url.
    /// </param>
    private AGSPlayer(string alias, string playerId, string avatarUrl) {
        this.alias = alias;
        this.playerId = playerId;
        this.avatarUrl = avatarUrl;
    }
    #endregion
            
    #region overrides
    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="AGSPlayer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents the current <see cref="AGSPlayer"/>.
    /// </returns>
    public override string ToString(){
        return string.Format( "alias: {0}, playerId: {1}, avatarUrl: {2}",
            alias, playerId, avatarUrl);
    }
    #endregion
    
}
#endif
