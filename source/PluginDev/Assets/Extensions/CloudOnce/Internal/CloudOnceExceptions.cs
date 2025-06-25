// <copyright file="CloudOnceExceptions.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;

    /// <summary>
    /// Used by the <see cref="DataManager"/> when a value type is not as expected.
    /// This will usually mean that the same Key has been used for more than one cloud preference,
    /// or that something has gone wrong during serialization/deserialization.
    /// </summary>
    public class UnexpectedCollectionElementTypeException : Exception
    {
        /// <summary>
        /// Used by the <see cref="DataManager"/> when a value type is not as expected.
        /// This will usually mean that the same Key has been used for more than one cloud preference,
        /// or that something has gone wrong during serialization/deserialization.
        /// </summary>
        /// <param name="key">The Key that caused the exception.</param>
        /// <param name="type">The expected value type.</param>
        public UnexpectedCollectionElementTypeException(string key, Type type)
            : base(string.Format("Unexpected type at index {0}, expected {1} ", key, type))
        {
        }
    }
}
