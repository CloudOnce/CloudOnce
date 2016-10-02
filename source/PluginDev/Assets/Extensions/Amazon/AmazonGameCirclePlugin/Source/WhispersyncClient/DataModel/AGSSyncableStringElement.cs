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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AGS syncable string element.
/// </summary>
public class AGSSyncableStringElement : AGSSyncableElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableStringElement"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableStringElement(AmazonJavaWrapper javaObject) : base(javaObject){
        
    }    
    
#if UNITY_ANDROID
    public AGSSyncableStringElement(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif
    
    
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>
    /// The value.
    /// </returns>
    public string GetValue(){
#if UNITY_ANDROID
        return javaObject.Call<string>("getValue");
#else
        return null;
#endif
    }    
    
}
#endif
