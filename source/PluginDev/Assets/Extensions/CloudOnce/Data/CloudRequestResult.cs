// <copyright file="CloudRequestResult.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce
{
    /// <summary>
    /// Used for callbacks when calling on methods in the native bridge plug-ins
    /// </summary>
    /// <typeparam name="T">Type of result to send, e.g. long for score, bool for success/failure</typeparam>
    public class CloudRequestResult<T>
        where T : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudRequestResult{T}"/> class of type <c>T</c> with an empty error
        /// </summary>
        /// <param name="result">Type of result to send, e.g. <c>long</c> for score, <see cref="bool"/> for success/failure.</param>
        public CloudRequestResult(T result)
        {
            Error = string.Empty;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudRequestResult{T}"/> class of type <c>T</c> with a specified error
        /// </summary>
        /// <param name="result">Type of result to send, e.g. <c>long</c> for score, <see cref="bool"/> for success/failure.</param>
        /// <param name="error">text of the error component</param>
        public CloudRequestResult(T result, string error)
        {
            Error = error;
            Result = result;
        }

        /// <summary>
        /// Error contained in the result. Empty if no error
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The result of the request.
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// Returns <c>true</c> if an error occurred with the request, <c>false</c> if it was successful.
        /// </summary>
        public bool HasError
        {
            get { return !string.IsNullOrEmpty(Error); }
        }
    }
}
