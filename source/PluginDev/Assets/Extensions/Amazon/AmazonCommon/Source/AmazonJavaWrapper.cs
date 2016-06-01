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
using UnityEngine;
using System;

public class AmazonJavaWrapper : System.IDisposable{
    
#if UNITY_ANDROID
    AndroidJavaObject jo;
#endif
    
    public AmazonJavaWrapper(){
        
    }

#if UNITY_ANDROID
    public AmazonJavaWrapper(AndroidJavaObject o){
        setAndroidJavaObject(o);
    }
#endif

#if UNITY_ANDROID
    public AndroidJavaObject getJavaObject(){
        return jo;    
    }
#else
    public System.Object getJavaObject(){
        return null;    
    }    
#endif

#if UNITY_ANDROID
    public void setAndroidJavaObject(AndroidJavaObject o){
        this.jo = o as AndroidJavaObject;
    }
#endif            
    
    public IntPtr GetRawObject(){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            return jo.GetRawObject();
        }else{
            return default(IntPtr);    
        }
#else    
        return default(IntPtr);
#endif
    }

    public IntPtr GetRawClass(){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            return jo.GetRawClass();
        }else{
            return default(IntPtr);    
        }
#else    
        return default(IntPtr);
#endif
    }
    
    public void Set<FieldType>(string fieldName, FieldType type) {
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            jo.Set<FieldType>(fieldName,type);
        }
#endif
    }
    
    public FieldType Get<FieldType>(string fieldName){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            return jo.Get<FieldType>(fieldName);
        }else{
            return default(FieldType);    
        }
#else    
        return default(FieldType);
#endif
    }
    
    public void SetStatic<FieldType>(string fieldName, FieldType type) {
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){    
            jo.SetStatic<FieldType>(fieldName,type);
        }
#endif
    }
    
    public FieldType GetStatic<FieldType>(string fieldName){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){    
            return jo.GetStatic<FieldType>(fieldName);
        }else{
            return default(FieldType);
        }
#else    
        return default(FieldType);
#endif
    }
    
    public void CallStatic(string method, params object[] args){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){    
            AndroidJNI.PushLocalFrame(args.Length+1);
            jo.CallStatic(method, args);
            AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
        }
#endif
        
    }    
    
    public void Call(string method, params object[] args){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            AndroidJNI.PushLocalFrame(args.Length+1);
            jo.Call(method, args);
            AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
        }
#endif    
    }    
    
    public ReturnType CallStatic<ReturnType>(string method, params object[] args){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            AndroidJNI.PushLocalFrame(args.Length+1);
            ReturnType retVal = jo.CallStatic<ReturnType>(method, args);
            AndroidJNI.PopLocalFrame(System.IntPtr.Zero);        
            return retVal;
        }else{
            return default(ReturnType);    
        }
#else    
        return default(ReturnType);
#endif
    }    
    
    public ReturnType Call<ReturnType>(string method, params object[] args){
#if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android){
            AndroidJNI.PushLocalFrame(args.Length+1);
            ReturnType retVal = jo.Call<ReturnType>(method, args);
            AndroidJNI.PopLocalFrame(System.IntPtr.Zero);        
            return retVal;
        }else{
            return default(ReturnType);     
        }
#else    
        return default(ReturnType);
#endif
    }
    
    public void Dispose(){
#if UNITY_ANDROID
        if(jo != null){
            jo.Dispose();
        }
#endif
    }
}
