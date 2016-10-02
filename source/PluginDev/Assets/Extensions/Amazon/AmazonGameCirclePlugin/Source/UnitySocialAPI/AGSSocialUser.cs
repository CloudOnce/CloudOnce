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

using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AGSSocialUser : IUserProfile {

    // A reference to the GameCircle player is kept, if available.
    AGSPlayer player;

    #region Constructor
    public AGSSocialUser() {
        player = AGSPlayer.GetBlankPlayer ();
    }

    public AGSSocialUser(AGSPlayer player) {
        this.player = player == null ? AGSPlayer.GetBlankPlayer () : player;
    }
    #endregion

    #region IUserPlayer implementation
    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string userName {
        get {
            return player.alias;
        }
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string id {
        get {
            return player.playerId;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="GameCircleLocalUser"/> is friend.
    /// </summary>
    /// <remarks>
    /// If friends of local user aren't loaded, isFriend will always return false.
    /// </remarks>
    /// <value>
    /// <c>true</c> if is friend; otherwise, <c>false</c>.
    /// </value>
    public bool isFriend {
        get {
            foreach (AGSSocialUser player in AGSSocialLocalUser.friendList) {
                if (player.id == id) {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public UserState state {
        get {
            AGSClient.LogGameCircleError("ILocalUser.state.get is not available for GameCircle");
            return UserState.Offline;
        }
    }

    /// <summary>
    /// Gets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public Texture2D image {
        get {
            AGSClient.LogGameCircleError("ILocalUser.image.get is not available for GameCircle");
            return null;
        }
    }
    #endregion

}
#endif
