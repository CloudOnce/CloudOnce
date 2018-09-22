// <copyright file="JsonHelper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

// ReSharper disable LoopCanBeConvertedToQuery
namespace CloudOnce.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper methods for serialization and deserialization
    /// </summary>
    public static class JsonHelper
    {
        public static T Convert<T>(JSONObject jsonObject)
        {
            return (T)Convert(jsonObject, typeof(T));
        }

        public static JSONObject ToJsonObject<T>(Dictionary<string, T> serializableDictionary)
            where T : IJsonSerializable
        {
            var convertedDictionary = ConvertToSerializable(serializableDictionary);

            var jsonDictionary = new Dictionary<string, JSONObject>();
            foreach (var pair in convertedDictionary)
            {
                jsonDictionary.Add(pair.Key, pair.Value.ToJSONObject());
            }

            return new JSONObject(jsonDictionary);
        }

        public static JSONObject ToJsonObject<T>(List<T> serializableList)
            where T : IJsonSerializable
        {
            var list = new List<JSONObject>();
            foreach (var item in serializableList)
            {
                list.Add(item.ToJSONObject());
            }

            return new JSONObject(list);
        }

        // ReSharper disable ConvertIfStatementToReturnStatement
        private static object Convert(JSONObject jsonObject, Type type)
        {
            if (type == typeof(Dictionary<string, float>))
            {
                return ToStringFloatDictionary(jsonObject);
            }

            if (type == typeof(Dictionary<string, SyncableItem>))
            {
                return ConstructDictionaryOfType<SyncableItem>(jsonObject);
            }

            if (type == typeof(Dictionary<string, SyncableCurrency>))
            {
                return ConstructDictionaryOfType<SyncableCurrency>(jsonObject);
            }

            if (type == typeof(Dictionary<string, CurrencyValue>))
            {
                return ConstructDictionaryOfType<CurrencyValue>(jsonObject);
            }

            return null;
        }

        private static Dictionary<string, IJsonSerializable> ConvertToSerializable<T>(Dictionary<string, T> dictionary)
            where T : IJsonSerializable
        {
            var serializableDictionary = new Dictionary<string, IJsonSerializable>();
            foreach (var pair in dictionary)
            {
                serializableDictionary.Add(pair.Key, pair.Value);
            }

            return serializableDictionary;
        }

        private static Dictionary<string, float> ToStringFloatDictionary(JSONObject jObject)
        {
            var dictionary = new Dictionary<string, float>();
            foreach (var key in jObject.Keys)
            {
                dictionary.Add(key, jObject[key].F);
            }

            return dictionary;
        }

        private static Dictionary<string, T> ConstructDictionaryOfType<T>(JSONObject jsonObject)
            where T : class
        {
            // Get a ctor for the type with a JSONObject parameter.
            var ctorInfo = typeof(T).GetConstructor(new[] { typeof(JSONObject) });
            if (ctorInfo != null)
            {
                var dictionary = new Dictionary<string, T>();
                foreach (var key in jsonObject.Keys)
                {
                    dictionary.Add(key, (T)ctorInfo.Invoke(new object[] { jsonObject[key] }));
                }

                return dictionary;
            }

            return null;
        }
    }
}
