// <copyright file="DataEnums.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce
{
    /// <summary>
    /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
    /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
    /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
    /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
    /// </summary>
    public enum PersistenceType : short
    {
        Latest,
        Highest,
        Lowest
    }

    public enum Interval
    {
        Disabled,
        Every30Seconds = 30,
        Every60Seconds = 60,
        Every90Seconds = 90,
        Every120Seconds = 120,
        Every300Seconds = 300,
        Every600Seconds = 600
    }
}
