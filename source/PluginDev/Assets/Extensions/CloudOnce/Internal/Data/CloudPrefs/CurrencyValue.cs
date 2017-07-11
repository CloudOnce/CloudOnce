// <copyright file="CurrencyValue.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using Utils;

    /// <summary>
    ///  Data-class to store currency value.
    /// </summary>
    public class CurrencyValue : IJsonConvertible
    {
        private const string oldAliasAdditions = "cdAdd";
        private const string oldAliasSubtractions = "cdSub";

        private const string aliasAdditions = "a";
        private const string aliasSubtractions = "s";

        /// <summary>
        /// Data-class to store currency value.
        /// </summary>
        public CurrencyValue()
        {
        }

        /// <summary>
        /// Data-class to store currency value.
        /// </summary>
        /// <param name="additions">Total additions made to this currency.</param>
        /// <param name="subtractions">Total subtractions made to this currency.</param>
        public CurrencyValue(float additions, float subtractions)
        {
            Additions = additions;
            Subtractions = subtractions;
        }

        /// <summary>
        /// Data-class to store currency value.
        /// </summary>
        /// <param name="value">Total current value of this currency.</param>
        public CurrencyValue(float value)
        {
            Value = value;
        }

        /// <summary>
        /// Data-class to store currency value.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing a <see cref="CurrencyValue"/></param>
        public CurrencyValue(JSONObject jsonObject)
        {
            FromJSONObject(jsonObject);
        }

        /// <summary>
        /// Total additions made to this currency.
        /// </summary>
        public float Additions { get; set; }

        /// <summary>
        /// Total subtractions made to this currency.
        /// </summary>
        public float Subtractions { get; set; }

        /// <summary>
        /// Current value of this currency.
        /// </summary>
        public float Value
        {
            get
            {
                return Additions + Subtractions;
            }

            set
            {
                var delta = value - Value;
                if (delta > 0f)
                {
                    Additions += delta;
                }
                else
                {
                    Subtractions += delta;
                }
            }
        }

        /// <summary>
        /// Converts the <see cref="CurrencyValue"/> into a <see cref="JSONObject"/>.
        /// </summary>
        /// <returns><see cref="JSONObject"/> containing a <see cref="CurrencyValue"/></returns>
        public JSONObject ToJSONObject()
        {
            var jsonObject = new JSONObject(JSONObject.Type.Object);

            jsonObject.AddField(aliasAdditions, Additions);
            jsonObject.AddField(aliasSubtractions, Subtractions);

            return jsonObject;
        }

        /// <summary>
        /// Reconstructs a <see cref="CurrencyValue"/> from a <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="jsonObject"><see cref="JSONObject"/> containing a <see cref="CurrencyValue"/></param>
        public void FromJSONObject(JSONObject jsonObject)
        {
            var addAlias = CloudOnceUtils.GetAlias(typeof(CurrencyValue).Name, jsonObject, aliasAdditions, oldAliasAdditions);
            var subAlias = CloudOnceUtils.GetAlias(typeof(CurrencyValue).Name, jsonObject, aliasSubtractions, oldAliasSubtractions);

            Additions = jsonObject[addAlias].F;
            Subtractions = jsonObject[subAlias].F;
        }
    }
}
