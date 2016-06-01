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
///  Base syncable type that supports metadata and timestamp information
/// </summary>
public class AGSSyncableElement :AGSSyncable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableElement"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableElement(AmazonJavaWrapper javaObject) : base(javaObject){

    }

#if UNITY_ANDROID
    public AGSSyncableElement(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif        
    
    /// <summary>
    ///  The time in which this element was set as the number of seconds 
    /// elapsed since January 1, 1970, 00:00:00 GMT.
    /// </summary>
    /// <returns>time this element was set</returns>    
    public long GetTimestamp(){
#if UNITY_ANDROID
        return javaObject.Call<long>( "getTimestamp" );
#else
        return 0;
#endif
    }

    /// <summary>
    /// Optional metadata associated with this SyncableElement.  A
    /// non-null, unmodifiable map is returned.
    /// </summary>
    /// <returns>dictionary containing key,value map of metadata</returns>        
    public Dictionary<string,string> GetMetadata(){
#if UNITY_ANDROID        
        Dictionary<string,string> dictionary = new Dictionary<string, string>();
        
        AndroidJNI.PushLocalFrame(10);
        AndroidJavaObject javaMap = javaObject.Call<AndroidJavaObject>("getMetadata");
        if(javaMap == null){
//            AGSClient.LogGameCircleError("Whispersync element was unable to retrieve metadata java map");
            return dictionary;    
        }
        
        AndroidJavaObject javaSet = javaMap.Call<AndroidJavaObject>("keySet");
        if(javaSet == null){
//            AGSClient.LogGameCircleError("Whispersync element was unable to retrieve java keyset");
            return dictionary;    
        }

                
        AndroidJavaObject javaIterator = javaSet.Call<AndroidJavaObject>("iterator");
        if(javaIterator == null){
//            AGSClient.LogGameCircleError("Whispersync element was unable to retrieve java iterator");
            return dictionary;    
        }
        
        string key, val;
        while( javaIterator.Call<bool>("hasNext") ){
            key = javaIterator.Call<string>("next");
            if(key != null){
                val = javaMap.Call<string>("get",key);
                if(val != null){
                    dictionary.Add (key,val);    
                }
            }    
        }    
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
        return dictionary;    
#else        
        return default(Dictionary<string,string>);
#endif
    }

}
#endif
