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

using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// GameCircle Unity Social API achievement implementation.
/// </summary>
public class AGSSocialAchievement : IAchievement, IAchievementDescription {
    // Track a readonly reference to the achievement from the plugin (if available)
    public readonly AGSAchievement achievement;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSocialAchievement"/> class.
    /// </summary>
    /// <param name='achievement'>
    /// Achievement.
    /// </param>
    public AGSSocialAchievement(AGSAchievement achievement) {
        if(null == achievement) {
            AGSClient.LogGameCircleError("AGSSocialAchievement constructor \"achievement\" argument should not be null");
            achievement = AGSAchievement.GetBlankAchievement();
        } else {
            this.achievement = achievement;
        }
        id = achievement.id;

        percentCompleted = achievement.progress;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSocialAchievement"/> class.
    /// </summary>
    public AGSSocialAchievement() {
        achievement = AGSAchievement.GetBlankAchievement ();   
    }
 
    /// <summary>
    /// Gets or sets the achievement ID.
    /// </summary>
    /// <value>
    /// The achievement ID.
    /// </value>
    public string id {
        get;
        set;
    }
    
    /// <summary>
    /// Gets or sets the percent completed.
    /// </summary>
    /// <value>
    /// The percent completed.
    /// </value>
    public double percentCompleted {
        get;
        set;
    }
    
    /// <summary>
    /// Gets a value indicating whether this <see cref="AGSSocialAchievement"/> is completed.
    /// </summary>
    /// <value>
    /// <c>true</c> if completed; otherwise, <c>false</c>.
    /// </value>
    public bool completed {
        get {
            return achievement.isUnlocked;
        }
    }
    
    /// <summary>
    /// Gets a value indicating whether this <see cref="AGSSocialAchievement"/> is hidden.
    /// </summary>
    /// <value>
    /// <c>true</c> if hidden; otherwise, <c>false</c>.
    /// </value>
    public bool hidden {
        get {
            return achievement.isHidden;
        }
    }
    
    /// <summary>
    /// Gets the date an achievement was unlocked
    /// </summary>
    /// <value>
    /// The date an achievement was unlocked
    /// </value>
    public System.DateTime lastReportedDate {
        get {
            return achievement.dateUnlocked;
        }
    }
    
    /// <summary>
    /// Reports progress made for this achievement.
    /// </summary>
    /// <param name='callback'>
    /// Callback.
    /// </param>
    public void ReportProgress(System.Action<bool> callback) {
        GameCircleSocial.Instance.ReportProgress (id, percentCompleted, callback);
    }
    
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string title {
        get {
            return achievement.title;
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
            AGSClient.LogGameCircleError("IAchievementDescription.image.get is not available for GameCircle");
            return null;
        }
    }
    
    /// <summary>
    /// Gets the achieved description. 
    /// The achievement description achieved / unachieved is handled
    /// by GameCircle internally, so this just returns the available
    /// description for this achievement (the locked description if
    /// the achievement is locked, the unlocked if it is unlocked)
    /// </summary>
    /// <value>
    /// The achieved description.
    /// </value>
    public string achievedDescription {
        get {
            return achievement.description;
        }
    }
    
    /// <summary>
    /// Gets the unachieved description.
    /// The achievement description achieved / unachieved is handled
    /// by GameCircle internally, so this just returns the available
    /// description for this achievement (the locked description if
    /// the achievement is locked, the unlocked if it is unlocked)
    /// </summary>
    /// <value>
    /// The unachieved description.
    /// </value>
    public string unachievedDescription {
        get {
            return achievement.description;
        }
    }
    
    /// <summary>
    /// Gets the point value of this achievement.
    /// </summary>
    /// <value>
    /// The point value of this achievement.
    /// </value>
    public int points {
        get {
            return achievement.pointValue;
        }
    }
}
#endif
