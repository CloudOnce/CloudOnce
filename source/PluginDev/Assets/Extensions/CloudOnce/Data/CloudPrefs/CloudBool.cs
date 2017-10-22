// <copyright file="CloudBool.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using Internal;

    /// <summary>
    /// Used to store <see cref="bool"/>s in the cloud.
    /// </summary>
    public sealed class CloudBool : PersistentValue<bool>
    {
        /// <summary>
        /// Used to store <see cref="bool"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer <c>true</c>.
        /// <see cref="PersistenceType.Lowest"/> will prefer <c>false</c>.
        /// </param>
        /// <param name="value">The starting value for this <see cref="bool"/>.</param>
        public CloudBool(string key, PersistenceType persistenceType, bool value = false)
            : base(key, persistenceType, value, value, DataManager.GetBool, DataManager.SetBool)
        {
        }

        /// <summary>
        /// Used to store <see cref="bool"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will always prefer the newest data.
        /// <see cref="PersistenceType.Highest"/> will prefer <c>true</c>.
        /// <see cref="PersistenceType.Lowest"/> will prefer <c>false</c>.
        /// </param>
        /// <param name="value">The starting value for this <see cref="bool"/>.</param>
        /// <param name="defaultValue">The value the <see cref="bool"/> will be set to if it is ever reset.</param>
        public CloudBool(string key, PersistenceType persistenceType, bool value, bool defaultValue)
            : base(key, persistenceType, value, defaultValue, DataManager.GetBool, DataManager.SetBool)
        {
        }
    }
}
