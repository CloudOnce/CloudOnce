// <copyright file="CloudOnceUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Utils
{
    using System;
    using System.Collections;
    using System.Runtime.Serialization;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Utilities used by CloudOnce's Internal systems.
    /// </summary>
    public static class CloudOnceUtils
    {
        static CloudOnceUtils()
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            AchievementUtils = new AppleAchievementUtils();
            LeaderboardUtils = new AppleLeaderboardUtils();
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            AchievementUtils = new GoogleAchievementUtils();
            LeaderboardUtils = new GoogleLeaderboardUtils();
#else
            AchievementUtils = new EditorAchievementUtils();
            LeaderboardUtils = new EditorLeaderboardUtils();
#endif
        }

        public static IAchievementUtils AchievementUtils { get; private set; }
        public static ILeaderboardUtils LeaderboardUtils { get; private set; }

        public static string ToBase64String(this string str)
        {
            if (str == null)
            {
                str = string.Empty;
            }

            var bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64StringToString(this string base64String)
        {
            if (base64String == null)
            {
                base64String = string.Empty;
            }

            var bytes = Convert.FromBase64String(base64String);
            return Encoding.Default.GetString(bytes);
        }

        public static IEnumerator InvokeUnscaledTime(UnityAction callback, float time)
        {
            if (callback != null)
            {
                var startTime = Time.unscaledTime;

                while ((Time.unscaledTime - startTime) < time)
                {
                    yield return null;
                }

                callback.Invoke();
            }
        }

        public static IEnumerator InvokeUnscaledTime<T>(UnityAction<T> callback, T parameter, float time)
        {
            if (callback != null)
            {
                var startTime = Time.unscaledTime;

                while ((Time.unscaledTime - startTime) < time)
                {
                    yield return null;
                }

                callback.Invoke(parameter);
            }
        }

        /// <summary>
        /// Used to safely invoke an <see cref="Action"/>, will not cause exception if the <see cref="Action"/> is <c>null</c>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to invoke.</param>
        public static void SafeInvoke(Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Used to safely invoke an <see cref="UnityAction"/>, will not cause exception if the <see cref="UnityAction"/> is <c>null</c>.
        /// </summary>
        /// <param name="unityAction">The <see cref="UnityAction"/> to invoke.</param>
        public static void SafeInvoke(UnityAction unityAction)
        {
            if (unityAction != null)
            {
                unityAction.Invoke();
            }
        }

        /// <summary>
        /// Used to safely invoke an <see cref="Action"/>, will not cause exception if the <see cref="Action"/> is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Action"/> to invoke.</typeparam>
        /// <param name="action">The <see cref="Action"/> to invoke.</param>
        /// <param name="param">Any parameters that the <see cref="Action"/> requires.</param>
        public static void SafeInvoke<T>(Action<T> action, T param)
        {
            if (action != null)
            {
                action.Invoke(param);
            }
        }

        /// <summary>
        /// Used to safely invoke an <see cref="UnityAction"/>, will not cause exception if the <see cref="UnityAction"/> is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="UnityAction"/> to invoke.</typeparam>
        /// <param name="unityAction">The <see cref="UnityAction"/> to invoke.</param>
        /// <param name="param">Any parameters that the <see cref="UnityAction"/> requires.</param>
        public static void SafeInvoke<T>(UnityAction<T> unityAction, T param)
        {
            if (unityAction != null)
            {
                unityAction.Invoke(param);
            }
        }

        /// <summary>
        /// Checks if a <see cref="string"/> is a JSON <see cref="string"/>.
        /// </summary>
        /// <param name="input">The <see cref="string"/> that might be a JSON <see cref="string"/>.</param>
        /// <returns>Whether or not the <see cref="string"/> is a JSON <see cref="string"/>.</returns>
        public static bool IsJson(this string input)
        {
            input = input.TrimStart();
            return input.StartsWith("{") || input.StartsWith("[");
        }

        /// <summary>
        /// Provides backwards compatibility for old aliases. Used when a serialization alias has been changed.
        /// </summary>
        /// <param name="className">The name of the class that is being deserialized.</param>
        /// <param name="jsonObject">The <see cref="JSONObject"/> to check for the aliases.</param>
        /// <param name="aliases">The aliases to check for.</param>
        /// <returns>The alias used in the <see cref="JSONObject"/>.</returns>
        /// <exception cref="SerializationException">Thrown when none of the aliases provided exist in the <see cref="JSONObject"/>.</exception>
        public static string GetAlias(string className, JSONObject jsonObject, params string[] aliases)
        {
            foreach (var alias in aliases)
            {
                if (jsonObject.HasFields(alias))
                {
                    return alias;
                }
            }

            throw new SerializationException("JSONObject missing fields, cannot deserialize to " + className);
        }
    }
}
