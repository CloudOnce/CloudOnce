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
using System.ComponentModel;

/// <summary>
/// AGS achievement.
/// </summary>
public class AGSAchievement
{
    public string title;
    public string id;
    public string description;
    public float progress;
    public int pointValue;
    public bool isHidden;
    public bool isUnlocked;
    public int position;
    public DateTime dateUnlocked;

    public static AGSAchievement fromHashtable (Hashtable hashtable) {
       
        try {
            AGSAchievement achievement = new AGSAchievement ();
            achievement.title = hashtable["achievementTitle"].ToString();
            achievement.id = hashtable["achievementId"].ToString();
            achievement.description = hashtable["achievementDescription"].ToString();
            achievement.progress = float.Parse(hashtable["achievementProgress"].ToString());
            achievement.pointValue = int.Parse(hashtable["achievementPointValue"].ToString());
            achievement.position = int.Parse(hashtable["achievementPosition"].ToString());
            achievement.isUnlocked = bool.Parse(hashtable["achievementUnlocked"].ToString());
            achievement.isHidden = bool.Parse(hashtable["achievementHidden"].ToString());
            achievement.dateUnlocked = getTimefromEpochTime(long.Parse(hashtable["achievementDateUnlocked"].ToString()));
            return achievement;
        } catch (Exception e) {
            AGSClient.LogGameCircleError("Returning blank achievement due to exception getting achievement from hashtable: " + e.ToString());
            return GetBlankAchievement();
        }
    }
   
    public static AGSAchievement GetBlankAchievement () {
        AGSAchievement achievement = new AGSAchievement ();
        achievement.title = "";
        achievement.id = "";
        achievement.description = "";
        achievement.pointValue = 0;
        achievement.isHidden = false;
        achievement.isUnlocked = false;
        achievement.progress = 0.0f;
        achievement.position = 0;
        achievement.dateUnlocked = System.DateTime.MinValue;
        return achievement;
    }

    private static DateTime getTimefromEpochTime (long javaTimeStamp) {
        DateTime dateTime = new DateTime (1970, 1, 1, 0, 0, 0, 0);
        return dateTime.AddMilliseconds (javaTimeStamp).ToLocalTime ();
    }

    public override string ToString () {
        return string.Format ("title: {0}, id: {1}, pointValue: {2}, hidden: {3}, unlocked: {4}, progress: {5}, position: {6}, date: {7} ", 
        title, id, pointValue, isHidden, isUnlocked, progress, position, dateUnlocked);
    }
}
#endif
