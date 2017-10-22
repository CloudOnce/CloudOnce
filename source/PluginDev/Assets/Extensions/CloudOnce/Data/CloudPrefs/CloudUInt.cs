// <copyright file="CloudUInt.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using Internal;

    /// <summary>
    /// Used to store <see cref="uint"/>s in the cloud.
    /// </summary>
    public sealed class CloudUInt : PersistentValue<uint>
    {
        /// <summary>
        /// Used to store <see cref="uint"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// If you use <see cref="PersistenceType.Lowest"/> and don't set <paramref name="value"/>, the <see cref="uint"/> will always be <c>0u</c>.
        /// </param>
        /// <param name="value">The starting value for this <see cref="uint"/>.</param>
        public CloudUInt(string key, PersistenceType persistenceType, uint value = 0u)
            : base(key, persistenceType, value, value, DataManager.GetUInt, DataManager.SetUInt)
        {
        }

        /// <summary>
        /// Used to store <see cref="uint"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// If you use <see cref="PersistenceType.Lowest"/> and don't set <paramref name="value"/>, the <see cref="uint"/> will always be <c>0u</c>.
        /// </param>
        /// <param name="value">The starting value for this <see cref="uint"/>.</param>
        /// <param name="defaultValue">The value the <see cref="uint"/> will be set to if it is ever reset.</param>
        public CloudUInt(string key, PersistenceType persistenceType, uint value, uint defaultValue)
            : base(key, persistenceType, value, defaultValue, DataManager.GetUInt, DataManager.SetUInt)
        {
        }
    }
}
