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
 * Removed iOS support.
 */

#if UNITY_ANDROID && CloudOnceAmazon

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// AGS syncable number list.
/// </summary>
public class AGSSyncableNumberList : AGSSyncableList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumberList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableNumberList(AmazonJavaWrapper javaObject) : base(javaObject)
    {
    }

#if UNITY_ANDROID
    public AGSSyncableNumberList(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif
    
      /// <summary>
      ///  Add the specified val. 
      /// </summary>
      /// <param name='val'>
      ///  Value. 
      /// </param>
    public void Add(long val){
#if UNITY_ANDROID
        javaObject.Call ( "add", val );
#endif
    }

    /// <summary>
    ///  Add the specified val. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    public void Add(double val){
#if UNITY_ANDROID
        javaObject.Call ("add", val );
#endif
    }

    /// <summary>
    ///  Add the specified val. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    public void Add(int val){
#if UNITY_ANDROID
        javaObject.Call( "add", val );
#endif
    }
    
    /// <summary>
    ///  Add the specified val and metadata. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    /// <param name='metadata'>
    ///  Metadata. 
    /// </param>
    public void Add(long val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));
#endif
    }    

    /// <summary>
    ///  Add the specified val and metadata. 
    /// </summary>
    /// <param name='val'>
    ///  Value. 
    /// </param>
    /// <param name='metadata'>
    ///  Metadata. 
    /// </param>
    public void Add(double val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));
#endif
    }

       /// <summary>
       ///  Add the specified val and metadata. 
       /// </summary>
       /// <param name='val'>
       ///  Value. 
       /// </param>
       /// <param name='metadata'>
       ///  Metadata. 
       /// </param>
    public void Add(int val, Dictionary<String, String> metadata){
#if UNITY_ANDROID
            javaObject.Call ("add", val, DictionaryToAndroidHashMap(metadata));
#endif
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <returns>
    /// The values.
    /// </returns>
    public AGSSyncableNumberElement[] GetValues(){
#if UNITY_ANDROID
        AndroidJNI.PushLocalFrame(10);
        AndroidJavaObject[] records = javaObject.Call<AndroidJavaObject[]>("getValues");
        
        if(records == null || records.Length == 0){
            return null;
        }
        
        AGSSyncableNumberElement[] returnElements =
                new AGSSyncableNumberElement[records.Length];
        
        for( int i = 0; i < records.Length; ++i){
            returnElements[i] = new AGSSyncableNumber(records[i]);
        }
        AndroidJNI.PopLocalFrame(System.IntPtr.Zero);

        return returnElements;
        
#else
        return null;
#endif
    }
}
#endif
