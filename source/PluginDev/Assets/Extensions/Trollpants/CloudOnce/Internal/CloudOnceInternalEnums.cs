// <copyright file="CloudOnceInternalEnums.cs" company="Trollpants Game Studio AS">
// Copyright (c) 2016 Trollpants Game Studio AS. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Trollpants.CloudOnce.Internal
{
    public enum CloudPlatform
    {
        NotSupported,
        iOS,
        GooglePlay,
        Amazon
    }

    public enum InternetConnectionStatus
    {
        Connected,
        Disconnected,
        Unstable
    }

    /// <summary>
    /// Used in serialization of <see cref="GameData"/>. All the data needs to be converted to <see cref="string"/>s,
    /// so we need to know what type each value was to be able to reconstruct it.
    /// </summary>
    public enum DataType : short
    {
        Bool,
        Double,
        Float,
        Int,
        String,
        UInt,
        Long,
        Decimal
    }

    public enum KvStoreChangeReason
    {
        ServerChange,
        InitialSyncChange,
        QuotaViolationChange,
        AccountChange
    }
}
