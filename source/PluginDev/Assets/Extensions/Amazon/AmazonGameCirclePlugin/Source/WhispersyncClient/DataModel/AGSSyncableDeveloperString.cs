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
 * Added CLOUDONCE_AMAZON build symbol.
 * Removed iOS support.
 */

#if UNITY_ANDROID && CLOUDONCE_AMAZON

using UnityEngine;

/// <summary>
/// AGS syncable developer string.
/// </summary>
public class AGSSyncableDeveloperString : AGSSyncable {

    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableDeveloperString"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableDeveloperString(AmazonJavaWrapper javaObject) : base(javaObject){

    }

#if UNITY_ANDROID
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSSyncableDeveloperString"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSSyncableDeveloperString(AndroidJavaObject javaObject) : base(javaObject)
    {
    }
#endif
        
    /// <summary>
    /// Gets the cloud value.
    /// </summary>
    /// <returns>
    /// The cloud value.
    /// </returns>
    public string getCloudValue() {
#if UNITY_ANDROID
        return javaObject.Call<string>( "getCloudValue" );
#else
        return null;
#endif        
        
    }
    
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>
    /// The value.
    /// </returns>
    public string getValue() {
#if UNITY_ANDROID
        return javaObject.Call<string>( "getValue" );
#else
        return null;
#endif        
    }
    
    /// <summary>
    /// Ins the conflict.
    /// </summary>
    /// <returns>
    /// The conflict.
    /// </returns>
    public bool inConflict() {
#if UNITY_ANDROID
        return javaObject.Call<bool>( "inConflict" );
#else
        return false;
#endif        
    }
    
    /// <summary>
    /// Ises the set.
    /// </summary>
    /// <returns>
    /// The set.
    /// </returns>
    public bool isSet() {
#if UNITY_ANDROID
        return javaObject.Call<bool>( "isSet" );
#else
        return false;
#endif        
    }
    
    /// <summary>
    /// Marks as resolved.
    /// </summary>
    public void markAsResolved() {
#if UNITY_ANDROID
        javaObject.Call( "markAsResolved" );
#endif        
        
    }
    
    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    public void setValue(string val) {
#if UNITY_ANDROID
        javaObject.Call( "setValue", val );
#endif        
    }
}
#endif
