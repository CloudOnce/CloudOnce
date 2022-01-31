// <copyright file="EditorJsonHelper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;

    /// <summary>
    /// Helper methods for serialization and deserialization
    /// </summary>
    public class EditorJsonHelper
    {
        public static T Convert<T>(JSONObject jsonObject)
        {
            return (T)Convert(jsonObject, typeof(T));
        }

        // ReSharper disable ConvertIfStatementToReturnStatement
        private static object Convert(JSONObject jsonObject, Type type)
        {
            if (type == typeof(List<PlatformIdData>))
            {
                return ConstructListOfType<PlatformIdData>(jsonObject);
            }

            if (type == typeof(List<CloudVariableData>))
            {
                return ConstructListOfType<CloudVariableData>(jsonObject);
            }

            return null;
        }

        private static List<T> ConstructListOfType<T>(JSONObject jsonObject)
            where T : class
        {
            // Get a ctor for the type with a JSONObject parameter.
            var ctorInfo = typeof(T).GetConstructor(new[] { typeof(JSONObject) });
            return ctorInfo != null ? jsonObject.List.Select(item => (T)ctorInfo.Invoke(new object[] { item })).ToList() : null;
        }
    }
}
#endif
