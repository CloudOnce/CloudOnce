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
 * Modified by Trollpants Game Studio AS.
 * Added TP_AndroidAmazon build symbol.
 * Removed iOS support.
 */

#if UNITY_ANDROID && TP_AndroidAmazon

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AGS syncable string list.
/// </summary>
public class AGSSyncableStringList : AGSSyncableList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableStringList(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }
    
#if UNITY_ANDROID
    public AGSSyncableStringList(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif
    /// <summary>
    /// Returns an immutable copy of the elements of this list as an array
    /// </summary>
    /// <returns>
    /// The values.
    /// </returns>
    public AGSSyncableString[] GetValues(){
#if UNITY_ANDROID
        AndroidJNI.PushLocalFrame(10);

        AndroidJavaObject[] records = javaObject.Call<AndroidJavaObject[]>("getValues");
        
        if(records == null || records.Length == 0){
            return null;
        }
        
        AGSSyncableString[] returnElements =
                new AGSSyncableString[records.Length];
        
        for( int i = 0; i < records.Length; ++i){
            returnElements[i] = new AGSSyncableString(records[i]);
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);

        return returnElements;  
#else
        return null;
#endif
    }
}
#endif
