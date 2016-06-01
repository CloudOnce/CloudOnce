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
/// AGS syncable accumulating number.
/// </summary>
public class AGSSyncableAccumulatingNumber : AGSSyncable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableAccumulatingNumber"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableAccumulatingNumber(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }

#if UNITY_ANDROID
    public AGSSyncableAccumulatingNumber(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif    
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(long delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);
#endif
    }
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(double delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);
#endif
    }
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(int delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);
#endif
    }
    
    /// <summary>
    /// Increment by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Increment(String delta){
#if UNITY_ANDROID
        javaObject.Call ("increment", delta);
#endif
    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(long delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#endif

    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(double delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#endif

    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(int delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#endif
    }
    
    /// <summary>
    /// Decrement by the specified delta.
    /// </summary>
    /// <param name='delta'>
    /// Delta.
    /// </param>
    public void Decrement(String delta){
#if UNITY_ANDROID
        javaObject.Call ("decrement", delta);
#endif
    }
 
    /// <summary>
    ///  gets current value as a long
    /// </summary>
    /// <returns>
    ///  long value
    /// </returns>
    public long AsLong(){
#if UNITY_ANDROID
        return javaObject.Call<long>("asLong");
#else
        return 0;
#endif
    }

    
    /// <summary>
    ///  gets current value as a double
    /// </summary>
    /// <returns>
    ///  double value
    /// </returns>    
    public double AsDouble(){
#if UNITY_ANDROID
        return javaObject.Call<double>("asDouble");
#else
        return 0;
#endif
    }

    /// <summary>
    ///  gets current value as an int
    /// </summary>
    /// <returns>
    ///  int value
    /// </returns>
    public int AsInt(){
#if UNITY_ANDROID
        return javaObject.Call<int>("asInt");
#else
        return 0;
#endif
    }    

     /// <summary>
    ///  gets current value as a string
    /// </summary>
    /// <returns>
    ///  string value
    /// </returns>
    public string AsString(){
#if UNITY_ANDROID
        return javaObject.Call<string>("asString");
#else
        return null;
#endif
    }        
}
#endif
