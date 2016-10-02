// <copyright file="ConflictResolver.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Handles data conflict resolution.
    /// </summary>
    public static class ConflictResolver
    {
        #region Public methods

        /// <summary>
        /// Takes two different <see cref="SyncableItem"/>s and returns one of them based on the persistence type. The two
        /// items must have the same data type and persistence type.
        /// </summary>
        /// <param name="localItem">The first of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <param name="otherItem">The second of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <returns>
        /// Returns one of the two <see cref="SyncableItem"/>s based on the persistence type. If the two <see cref="SyncableItem"/>s
        /// don't share the same data type or persistence type, <c>null</c> will be returned.
        /// </returns>
        public static SyncableItem ResolveConflict(SyncableItem localItem, SyncableItem otherItem)
        {
            if (localItem.Metadata.PersistenceType != otherItem.Metadata.PersistenceType)
            {
                Debug.LogWarning("Tried to resolve data conflict, but the two items did not have the same PersistenceType! Will use local data.");
                return localItem;
            }

            if (localItem.Metadata.DataType != otherItem.Metadata.DataType)
            {
                Debug.LogWarning("Tried to resolve data conflict, but the two items did not have the same DataType! Will use local data.");
                return localItem;
            }

            SyncableItem mergedItem;

            switch (localItem.Metadata.PersistenceType)
            {
                case PersistenceType.Latest:
                    mergedItem = MergeLatest(localItem, otherItem);
                    break;
                case PersistenceType.Highest:
                    mergedItem = MergeHighest(localItem, otherItem);
                    break;
                case PersistenceType.Lowest:
                    mergedItem = MergeLowest(localItem, otherItem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return mergedItem;
        }

        #endregion /Public methods

        #region Private methods

        /// <summary>
        /// Will return the latest/newest of two <see cref="SyncableItem"/>s.
        /// </summary>
        /// <param name="localItem">The first of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <param name="otherItem">The second of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <returns>Returns the latest/newest of the two <see cref="SyncableItem"/>s.</returns>
        private static SyncableItem MergeLatest(SyncableItem localItem, SyncableItem otherItem)
        {
            return localItem.Metadata.Timestamp.CompareTo(otherItem.Metadata.Timestamp) > 0 ? localItem : otherItem;
        }

        /// <summary>
        /// Will return the highest of two <see cref="SyncableItem"/>s.
        /// </summary>
        /// <param name="localItem">The first of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <param name="otherItem">The second of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <returns>Returns the highest of the two <see cref="SyncableItem"/>s.</returns>
        private static SyncableItem MergeHighest(SyncableItem localItem, SyncableItem otherItem)
        {
            switch (localItem.Metadata.DataType)
            {
                case DataType.Bool:
                    int result;
                    if (int.TryParse(otherItem.ValueString, out result))
                    {
                        return result == 1 ? otherItem : localItem;
                    }

                    return Convert.ToBoolean(otherItem.ValueString) ? otherItem : localItem;

                case DataType.Double:
                    return Convert.ToDouble(localItem.ValueString) > Convert.ToDouble(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Float:
                    return Convert.ToSingle(localItem.ValueString) > Convert.ToSingle(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Int:
                    return Convert.ToInt32(localItem.ValueString) > Convert.ToInt32(otherItem.ValueString) ? localItem : otherItem;

                case DataType.String:
                    return localItem.ValueString.Length > otherItem.ValueString.Length ? localItem : otherItem;

                case DataType.UInt:
                    return Convert.ToUInt32(localItem.ValueString) > Convert.ToUInt32(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Long:
                    return Convert.ToInt64(localItem.ValueString) > Convert.ToInt64(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Decimal:
                    return Convert.ToDecimal(localItem.ValueString) > Convert.ToDecimal(otherItem.ValueString) ? localItem : otherItem;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Will return the lowest of two <see cref="SyncableItem"/>s.
        /// </summary>
        /// <param name="localItem">The first of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <param name="otherItem">The second of two <see cref="SyncableItem"/>s that are in conflict.</param>
        /// <returns>Returns the lowest of the two <see cref="SyncableItem"/>s.</returns>
        private static SyncableItem MergeLowest(SyncableItem localItem, SyncableItem otherItem)
        {
            switch (localItem.Metadata.DataType)
            {
                case DataType.Bool:
                    int result;
                    if (int.TryParse(otherItem.ValueString, out result))
                    {
                        return result == 0 ? otherItem : localItem;
                    }

                    return !Convert.ToBoolean(otherItem.ValueString) ? otherItem : localItem;

                case DataType.Double:
                    return Convert.ToDouble(localItem.ValueString) < Convert.ToDouble(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Float:
                    return Convert.ToSingle(localItem.ValueString) < Convert.ToSingle(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Int:
                    return Convert.ToInt32(localItem.ValueString) < Convert.ToInt32(otherItem.ValueString) ? localItem : otherItem;

                case DataType.String:
                    return localItem.ValueString.Length < otherItem.ValueString.Length ? localItem : otherItem;

                case DataType.UInt:
                    return Convert.ToUInt32(localItem.ValueString) < Convert.ToUInt32(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Long:
                    return Convert.ToInt64(localItem.ValueString) < Convert.ToInt64(otherItem.ValueString) ? localItem : otherItem;

                case DataType.Decimal:
                    return Convert.ToDecimal(localItem.ValueString) < Convert.ToDecimal(otherItem.ValueString) ? localItem : otherItem;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion / Private methods
    }
}
