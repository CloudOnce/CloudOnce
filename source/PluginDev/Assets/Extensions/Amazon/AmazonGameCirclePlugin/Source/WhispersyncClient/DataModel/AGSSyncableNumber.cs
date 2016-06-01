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
/// AGS syncable number.
/// </summary>
public class AGSSyncableNumber : AGSSyncableNumberElement
{  
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumber"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableNumber(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }

#if UNITY_ANDROID
    public AGSSyncableNumber(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif    
    /// <summary>
    /// Set the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Set(long val){        
#if UNITY_ANDROID
        javaObject.Call( "set", val );                
#endif
    }

      /// <summary>
      /// Set the specified val.
      /// </summary>
      /// <param name='val'>
      /// Value.
      /// </param>
    public void Set(double val){
#if UNITY_ANDROID
        javaObject.Call( "set", val );            
#endif
    }    

    /// <summary>
    /// Set the specified val.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void Set(int val){
#if UNITY_ANDROID
        javaObject.Call( "set", val );   
#endif
    }
    
     /// <summary>
     /// Set the specified val.
     /// </summary>
     /// <param name='val'>
     /// Value.
     /// </param>
    public void Set(string val){
#if UNITY_ANDROID
     javaObject.Call( "set", val );
#endif
    }

    
    /// <summary>
    /// Set the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Set(long val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call( "set", val, DictionaryToAndroidHashMap(metadata) );
#endif
    }

    /// <summary>
    /// Set the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Set(double val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));
#endif
    }
    
    /// <summary>
    /// Set the specified val and metadata.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='metadata'>
    /// Metadata.
    /// </param>
    public void Set(int val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));
#endif
    }    

      /// <summary>
      /// Set the specified val and metadata.
      /// </summary>
      /// <param name='val'>
      /// Value.
      /// </param>
      /// <param name='metadata'>
      /// Metadata.
      /// </param>
    public void Set(string val, Dictionary<string,string> metadata){
#if UNITY_ANDROID
        javaObject.Call ("set", val, DictionaryToAndroidHashMap(metadata));
#endif
    }

      /// <summary>
      /// returns whether a value is set
      /// </summary>
      /// <returns>
      /// bool indicating if a value has been set
      /// </returns>
    public bool IsSet(){
#if UNITY_ANDROID
        return javaObject.Call<bool>("isSet");
#else
        return false;
#endif
    }
 
}
#endif
