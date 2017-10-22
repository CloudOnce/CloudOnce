// <copyright file="CloudInt.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using Internal;

    /// <summary>
    /// Used to store <see cref="int"/>s in the cloud.
    /// </summary>
    public sealed class CloudInt : PersistentValue<int>
    {
        /// <summary>
        /// Used to store <see cref="int"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The starting value for this <see cref="int"/>.</param>
        public CloudInt(string key, PersistenceType persistenceType, int value = 0)
            : base(key, persistenceType, value, value, DataManager.GetInt, DataManager.SetInt)
        {
        }

        /// <summary>
        /// Used to store <see cref="int"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The starting value for this <see cref="int"/>.</param>
        /// <param name="defaultValue">The value the <see cref="int"/> will be set to if it is ever reset.</param>
        public CloudInt(string key, PersistenceType persistenceType, int value, int defaultValue)
            : base(key, persistenceType, value, defaultValue, DataManager.GetInt, DataManager.SetInt)
        {
        }
    }
}
