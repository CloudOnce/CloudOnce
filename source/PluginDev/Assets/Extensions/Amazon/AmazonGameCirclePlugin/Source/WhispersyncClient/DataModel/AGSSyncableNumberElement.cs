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
/// AGS syncable number element.
/// </summary>
public class AGSSyncableNumberElement : AGSSyncableElement
{   
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableNumberElement"/> class.
    /// </summary>
    /// <param name='JavaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableNumberElement(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }    

#if UNITY_ANDROID
    public AGSSyncableNumberElement(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif   
    
    /// <summary>
    /// returns long represnation of this element
    /// </summary>
    /// <returns>
    /// The long.
    /// </returns>
    public long AsLong(){
#if UNITY_ANDROID
        return javaObject.Call<long>("asLong");
#else
        return 0;
#endif
    }

    /// <summary>
    /// returns double representation of this element
    /// </summary>
    /// <returns>
    /// The double.
    /// </returns>
    public double AsDouble(){
#if UNITY_ANDROID
        return javaObject.Call<double>("asDouble");
#else
        return 0;
#endif
    }


    /// <summary>
    /// returns int representation of this element
    /// </summary>
    /// <returns>
    /// The int.
    /// </returns>
    public int AsInt(){
#if UNITY_ANDROID
        return javaObject.Call<int>("asInt");
#else
        return 0;
#endif
    }    

       /// <summary>
       /// returns string representation of this element
       /// </summary>
       /// <returns>
       /// The string.
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
