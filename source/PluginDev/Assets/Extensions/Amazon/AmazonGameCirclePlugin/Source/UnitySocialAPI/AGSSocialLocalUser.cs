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
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;

/// <summary>
/// GameCircle local user implemention for Unity Social API.
/// </summary>
public class AGSSocialLocalUser : AGSSocialUser, ILocalUser  {
    // A reference to the GameCircle player is kept, if available.
	public static AGSPlayer player = AGSPlayer.GetBlankPlayer();
    public static List<AGSSocialUser> friendList = new List<AGSSocialUser> ();

    #region ILocalUser implementation
    /// <summary>
    /// Authenticate the local user with the active Social plugin.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void Authenticate(System.Action<bool> callback) {
        // The Unity Social API implies a heavy connection between
        // initialization of the Social API and the local user.
        // http://docs.unity3d.com/Documentation/Components/net-SocialAPI.html
        // This means that the local player should be available as early as possible.
        GameCircleSocial.Instance.RequestLocalPlayer (callback);
        Social.Active.Authenticate(this, callback);
    }
    
    /// <summary>
    /// Loads this local user's friends list.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void LoadFriends(System.Action<bool> callback) {
        GameCircleSocial.Instance.RequestFriends (callback);
    }

    /// <summary>
    /// Gets the local user's friends.
    /// </summary>
    /// <value>
    /// The friends.
    /// </value>
    public IUserProfile[] friends {
        get {
            return friendList.ToArray();
        }
    }
 
    /// <summary>
    /// Gets a value indicating whether this <see cref="GameCircleLocalUser"/> is authenticated.
    /// </summary>
    /// <value>
    /// <c>true</c> if authenticated; otherwise, <c>false</c>.
    /// </value>
    public bool authenticated {
        get {
            return AGSPlayerClient.IsSignedIn();
        }
    }
 
    /// <summary>
    /// Gets a value indicating whether this <see cref="GameCircleLocalUser"/> is underage.
    /// </summary>
    /// <value>
    /// <c>true</c> if underage; otherwise, <c>false</c>.
    /// </value>
    public bool underage {
        get {
            AGSClient.LogGameCircleError("ILocalUser.underage.get is not available for GameCircle");
            return false;
        }
    }
    #endregion

}
#endif
