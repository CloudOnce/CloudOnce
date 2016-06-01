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

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AGS syncable list.
/// </summary>
public class AGSSyncableList : AGSSyncable
{   
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableList(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }

#if UNITY_ANDROID
    public AGSSyncableList(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif    
    
    /// <summary>
    /// Sets the max size of this List..
    /// </summary>
    /// <remarks>
    /// Sets the max size of this List.  size must be positive
    /// and no greater than MAX_SIZE_LIMIT.  If size is smaller than the 
    /// current size of this SyncableNumberList in the cloud, then its size 
    /// will remain.  In other words, the size of SyncableNumberList can 
    /// never shrink.
    /// </remarks>
    /// <param name='size'>
    /// the max size of this List
    /// </param>    
    public void SetMaxSize(int size){
#if UNITY_ANDROID
        javaObject.Call( "setMaxSize", size );
#endif
    }
    

    /// <summary>
    /// Gets the size of the max.
    /// </summary>
    /// <returns>
    /// The max size.
    /// </returns>
    public int GetMaxSize(){
#if UNITY_ANDROID
        return javaObject.Call<int>( "getMaxSize" );
#else
        return 0;
#endif
    }
    
    /// <summary>
    /// bool indicating if a value is set
    /// </summary>
    /// <returns>
    /// bool indicating if a value is set
    /// </returns>
    public bool IsSet(){
#if UNITY_ANDROID
        return javaObject.Call<bool>( "isSet" );
#else
        return false;
#endif
    }
    
    /// <summary>
    /// Add the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Add(String val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
        javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));
#endif
    }
    
    /// <summary>
    /// Add the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Add(String val){
#if UNITY_ANDROID
        javaObject.Call( "add", val );
#endif
    }
}
#endif
