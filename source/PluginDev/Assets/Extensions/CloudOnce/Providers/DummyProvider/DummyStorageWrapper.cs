// <copyright file="DummyStorageWrapper.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Providers
{
    /// <summary>
    /// Dummy storage wrapper for unsupported platforms.
    /// </summary>
    public class DummyStorageWrapper : ICloudStorageProvider
    {
        private readonly CloudOnceEvents cloudOnceEvents;

        public DummyStorageWrapper(CloudOnceEvents events)
        {
            cloudOnceEvents = events;
        }

        /// <summary>
        /// Dummy Save method. Will save cloud variables to disk, but because Cloud Save can't be enabled
        /// when using <see cref="DummyProvider"/>, <see cref="Cloud.OnCloudSaveComplete"/> will always be <c>false</c>.
        /// </summary>
        public void Save()
        {
            DataManager.SaveToDisk();
            cloudOnceEvents.RaiseOnCloudSaveComplete(false);
        }

        /// <summary>
        /// Dummy Load method.
        /// </summary>
        public void Load()
        {
            cloudOnceEvents.RaiseOnCloudLoadComplete(false);
        }

        /// <summary>
        /// Calls <see cref="Load"/> and then <see cref="Save"/> as soon as the Cloud load is complete.
        /// </summary>
        public void Synchronize()
        {
            Load();
            Save();
        }

        /// <summary>
        /// Resets a Cloud variable to the default value.
        /// </summary>
        /// <param name="key">The unique identifier for the Cloud variable you want to reset.</param>
        /// <returns>Whether or not the variable was successfully reset.</returns>
        public bool ResetVariable(string key)
        {
            return DataManager.ResetCloudPref(key);
        }

        /// <summary>
        /// Deletes a specific Cloud variable from the Cloud.
        /// </summary>
        /// <param name="key">The unique identifier for the CloudPref you want to delete.</param>
        /// <returns>
        /// <c>true</c> if the CloudPref is found and deleted, <c>false</c> if the specified <paramref name="key"/> doesn't exist.
        /// </returns>
        public bool DeleteVariable(string key)
        {
            return DataManager.DeleteCloudPref(key);
        }

        /// <summary>
        /// Deletes all variables that exists in the cloud save, but have not been declared in the local data.
        /// Can be useful during development when variable names change, but use with caution.
        /// </summary>
        /// <returns>An array with the keys for the variables that was cleared.</returns>
        public string[] ClearUnusedVariables()
        {
            return DataManager.ClearStowawayVariablesFromGameData();
        }

        /// <summary>
        /// WARNING! Deletes all cloud variables both locally and in the cloud (if logged into a cloud save service)!
        /// Should only be used during development, not in production builds.
        /// </summary>
        public void DeleteAll()
        {
            DataManager.DeleteAllCloudVariables();
        }
    }
}
