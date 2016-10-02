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
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AGS latest string list.
/// </summary>
public class AGSLatestStringList : AGSSyncableStringList{
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AGSLatestStringList"/> class.
    /// </summary>
    /// <param name='javaObject'>
    /// Java object.
    /// </param>
    public AGSLatestStringList(AmazonJavaWrapper javaObject) : base(javaObject){}
    
    public AGSLatestStringList(AndroidJavaObject javaObject) : base(javaObject){}
    
}
#endif
