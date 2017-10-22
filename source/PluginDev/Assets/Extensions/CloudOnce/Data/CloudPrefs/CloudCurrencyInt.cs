// <copyright file="CloudCurrencyInt.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using Internal;

    /// <summary>
    /// Used to create virtual currencies that gets stored in the cloud. This type uses <see cref="int"/> values.
    /// It has a special conflict resolution system based on this article:
    /// <c>https://developer.android.com/training/cloudsave/conflict-res.html</c>
    /// </summary>
    public sealed class CloudCurrencyInt : PersistentCurrency
    {
        /// <summary>
        /// Used to create virtual currencies that gets stored in the cloud. This type uses <see cref="int"/> values.
        /// It has a special conflict resolution system based on this article:
        /// <c>https://developer.android.com/training/cloudsave/conflict-res.html</c>
        /// </summary>
        /// <param name="key">A unique identifier for this particular currency.</param>
        /// <param name="defaultValue">The currency's default/starting value.</param>
        /// <param name="allowNegative">If the value of this currency is allowed to be negative.</param>
        public CloudCurrencyInt(string key, int defaultValue = 0, bool allowNegative = false)
            : base(key, defaultValue, allowNegative)
        {
        }

        #region Properties

        /// <summary>
        /// Total additions made to this currency.
        /// </summary>
        public new int Additions
        {
            get { return (int)base.Additions; }
        }

        /// <summary>
        /// Total subtractions made to this currency.
        /// </summary>
        public new int Subtractions
        {
            get { return (int)base.Subtractions; }
        }

        /// <summary>
        /// The currency's default/starting value. Can't be changed after the currency is created.
        /// </summary>
        public new int DefaultValue
        {
            get { return (int)base.DefaultValue; }
        }

        /// <summary>
        /// Current value for this currency.
        /// </summary>
        public new int Value
        {
            get { return (int)base.Value; }
            set { base.Value = value; }
        }

        #endregion /Properties
    }
}
