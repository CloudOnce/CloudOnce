// <copyright file="PersistentValue.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    using System;

    /// <summary>
    /// A preference that is stored in the cloud.
    /// </summary>
    /// <typeparam name="T">The type of preference.</typeparam>
    public abstract class PersistentValue<T> : IPersistent
    {
        private T value;

        /// <summary>
        /// A preference that is stored in the cloud. Automatically added to the <see cref="DataManager"/>.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="type">The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.</param>
        /// <param name="value">The starting value for this preference.</param>
        /// <param name="defaultValue">The value the preference will be set to if it is ever reset.</param>
        /// <param name="valueLoader"><c>delegate</c> used to get the preference.</param>
        /// <param name="valueSetter"><c>delegate</c> used to set the preference.</param>
        protected PersistentValue(string key, PersistenceType type, T value, T defaultValue, ValueLoaderDelegate valueLoader, ValueSetterDelegate valueSetter)
        {
            Key = key;
            Value = value;
            PersistenceType = type;
            DefaultValue = defaultValue;
            ValueLoader = valueLoader;
            ValueSetter = valueSetter;

            DataManager.CloudPrefs[key] = this;
            DataManager.InitDataManager();
            Load();
        }

        protected delegate T ValueLoaderDelegate(string key, T defaultValue);

        protected delegate void ValueSetterDelegate(string key, T value, PersistenceType persistenceType);

        #region Properties

        /// <summary>
        /// The unique identifier used to identify this particular value.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The current value for this preference. Takes <see cref="PersistenceType"/> into account when setting.
        /// </summary>
        public T Value
        {
            get
            {
                return value;
            }

            set
            {
                if (IsValidSet(value))
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// </summary>
        public PersistenceType PersistenceType { get; private set; }

        /// <summary>
        /// The value the preference will be set to if it is ever reset.
        /// </summary>
        public T DefaultValue { get; private set; }

        /// <summary>
        /// <c>delegate</c> used to get the preference.
        /// </summary>
        private ValueLoaderDelegate ValueLoader { get; set; }

        /// <summary>
        /// <c>delegate</c> used to set the preference.
        /// </summary>
        private ValueSetterDelegate ValueSetter { get; set; }

        #endregion /Properties

        #region Methods

        /// <summary>
        /// Invokes the <see cref="ValueLoader"/>.
        /// </summary>
        public void Load(bool force = false)
        {
            if (ValueLoader != null)
            {
                if (force)
                {
                    value = ValueLoader.Invoke(Key, DefaultValue);
                }
                else
                {
                    Value = ValueLoader.Invoke(Key, DefaultValue);
                }
            }
        }

        /// <summary>
        /// Invokes the <see cref="ValueSetter"/>.
        /// </summary>
        public void Flush()
        {
            if (ValueSetter != null)
            {
                ValueSetter.Invoke(Key, Value, PersistenceType);
            }
        }

        /// <summary>
        /// Resets the value back to it's default value.
        /// </summary>
        public void Reset()
        {
            value = DefaultValue;
            Flush();
        }

        /// <summary>
        /// Only used in value setter.
        /// Checks if the new value shall overwrite the previous value, according to its persistence type.
        /// </summary>
        /// <param name="newValue">The new value</param>
        /// <returns><c>true</c> if its okay to overwrite old value</returns>
        private bool IsValidSet(T newValue)
        {
            if (PersistenceType == PersistenceType.Latest)
            {
                return true;
            }

            if (newValue is DateTime)
            {
                var newDateTime = (DateTime)(object)newValue;
                var oldDateTime = (DateTime)(object)value;

                return PersistenceType == PersistenceType.Highest
                    ? newDateTime.Ticks > oldDateTime.Ticks
                    : newDateTime < oldDateTime;
            }

            if (newValue is long)
            {
                var newNumber = long.Parse(newValue.ToString());
                var oldNumber = long.Parse(value.ToString());

                return PersistenceType == PersistenceType.Highest
                    ? newNumber > oldNumber
                    : newNumber < oldNumber;
            }

            if (newValue is decimal)
            {
                var newNumber = decimal.Parse(newValue.ToString());
                var oldNumber = decimal.Parse(value.ToString());

                return PersistenceType == PersistenceType.Highest
                    ? newNumber > oldNumber
                    : newNumber < oldNumber;
            }

            if (!(newValue is bool) && !(newValue is string))
            {
                var newNumber = double.Parse(newValue.ToString());
                var oldNumber = double.Parse(value.ToString());

                return PersistenceType == PersistenceType.Highest
                    ? newNumber > oldNumber
                    : newNumber < oldNumber;
            }

            if (!(newValue is string))
            {
                var newBool = bool.Parse(newValue.ToString());
                var oldBool = bool.Parse(value.ToString());

                return PersistenceType == PersistenceType.Highest
                    ? newBool && !oldBool
                    : !newBool && oldBool;
            }

            var newStringLength = newValue.ToString().Length;
            var oldStringLength = value.ToString().Length;

            return PersistenceType == PersistenceType.Highest
                ? newStringLength > oldStringLength
                : newStringLength < oldStringLength;
        }

        #endregion /Methods
    }
}
