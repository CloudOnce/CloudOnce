// <copyright file="CloudString.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using Internal;

    /// <summary>
    /// Used to store <see cref="string"/>s in the cloud.
    /// </summary>
    public sealed class CloudString : PersistentValue<string>
    {
        /// <summary>
        /// Used to store <see cref="string"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) <see cref="string"/>.
        /// <see cref="PersistenceType.Highest"/> will prefer the longest <see cref="string"/>.
        /// <see cref="PersistenceType.Lowest"/> will prefer the shortest <see cref="string"/>.
        /// If you use <see cref="PersistenceType.Lowest"/> and don't set an initial value, the <see cref="string"/> will always be empty.
        /// </param>
        /// <param name="value">The starting value for this <see cref="string"/>.</param>
        public CloudString(string key, PersistenceType persistenceType = PersistenceType.Latest, string value = "")
            : base(key, persistenceType, value, value, DataManager.GetString, DataManager.SetString)
        {
        }

        /// <summary>
        /// Used to store <see cref="string"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) <see cref="string"/>.
        /// <see cref="PersistenceType.Highest"/> will prefer the longest <see cref="string"/>.
        /// <see cref="PersistenceType.Lowest"/> will prefer the shortest <see cref="string"/>.
        /// If you use <see cref="PersistenceType.Lowest"/> and don't set an initial value, the <see cref="string"/> will always be empty.
        /// </param>
        /// <param name="value">The starting value for this <see cref="string"/>.</param>
        /// <param name="defaultValue">The value the <see cref="string"/> will be set to if it is ever reset.</param>
        public CloudString(string key, PersistenceType persistenceType, string value, string defaultValue)
            : base(key, persistenceType, value, defaultValue, DataManager.GetString, DataManager.SetString)
        {
        }
    }
}
