// <copyright file="CloudDateTime.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using System;
    using Internal;

    public class CloudDateTime : PersistentValue<DateTime>
    {
        /// <summary>
        /// Used to store <see cref="DateTime"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the <see cref="DateTime"/> that was SAVED last.
        /// <see cref="PersistenceType.Highest"/> will prefer the newest <see cref="DateTime"/> value in ticks.
        /// <see cref="PersistenceType.Lowest"/> will prefer the oldest <see cref="DateTime"/> value in ticks.
        /// </param>
        /// <param name="value">The starting value for this <see cref="DateTime"/>.</param>
        public CloudDateTime(string key, PersistenceType persistenceType, DateTime value = default(DateTime))
            : base(key, persistenceType, value, value, DataManager.GetDateTime, DataManager.SetDateTime)
        {
        }

        /// <summary>
        /// Used to store <see cref="DateTime"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the <see cref="DateTime"/> that was SAVED last.
        /// <see cref="PersistenceType.Highest"/> will prefer the newest <see cref="DateTime"/> value in ticks.
        /// <see cref="PersistenceType.Lowest"/> will prefer the oldest <see cref="DateTime"/> value in ticks.
        /// </param>
        /// <param name="value">The starting value for this <see cref="DateTime"/>.</param>
        /// <param name="defaultValue">The value the <see cref="DateTime"/> will be set to if it is ever reset.</param>
        public CloudDateTime(string key, PersistenceType persistenceType, DateTime value, DateTime defaultValue)
            : base(key, persistenceType, value, defaultValue, DataManager.GetDateTime, DataManager.SetDateTime)
        {
        }
    }
}
