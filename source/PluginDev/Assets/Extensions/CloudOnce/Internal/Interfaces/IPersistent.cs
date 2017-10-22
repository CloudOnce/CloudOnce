// <copyright file="IPersistent.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    /// <summary>
    /// Used by cloud preferences to facilitate loading and saving of data.
    /// </summary>
    public interface IPersistent
    {
        /// <summary>
        /// Invokes the cloud preference's <c>delegate</c> used to save data.
        /// </summary>
        void Flush();

        /// <summary>
        /// Invokes the cloud preference's <c>delegate</c> used to load data.
        /// </summary>
        /// <param name="force">Force load, ignoring <see cref="PersistenceType"/>.</param>
        void Load(bool force = false);

        /// <summary>
        /// Resets the <see cref="IPersistent"/> to default/initial value.
        /// </summary>
        void Reset();
    }
}