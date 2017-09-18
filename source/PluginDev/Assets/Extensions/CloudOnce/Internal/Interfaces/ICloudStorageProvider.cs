// <copyright file="ICloudStorageProvider.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Providers
{
    /// <summary>
    /// Interface for platform-agnostic access to cloud save.
    /// </summary>
    public interface ICloudStorageProvider
    {
        /// <summary>
        /// Saves all cloud variables, to both disk and cloud.
        /// If <see cref="Cloud.CloudSaveEnabled"/> is <c>false</c>, it will only save to disk.
        /// Skips saving if no variables have been changed.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads any available cloud data (if signed in and cloud saving is enabled).
        /// </summary>
        void Load();

        /// <summary>
        /// Calls <see cref="Load"/> and then <see cref="Save"/> as soon as the Cloud load operation is complete.
        /// </summary>
        void Synchronize();

        /// <summary>
        /// Resets a Cloud variable to the default value.
        /// </summary>
        /// <param name="key">The unique identifier for the Cloud variable you want to reset.</param>
        /// <returns>Whether or not the variable was successfully reset.</returns>
        bool ResetVariable(string key);

        /// <summary>
        /// Deletes a specific Cloud variable from the Cloud.
        /// </summary>
        /// <param name="key">The unique identifier for the CloudPref you want to delete.</param>
        /// <returns>
        /// <c>true</c> if the CloudPref is found and deleted, <c>false</c> if the specified <paramref name="key"/> doesn't exist.
        /// </returns>
        bool DeleteVariable(string key);

        /// <summary>
        /// Deletes all variables that exists in the cloud save, but have not been declared in the local data.
        /// Can be useful during development when variable names change, but use with caution.
        /// </summary>
        /// <returns>An array with the keys for the variables that was cleared.</returns>
        string[] ClearUnusedVariables();

        /// <summary>
        /// WARNING! Deletes all cloud variables both locally and in the cloud (if logged into a cloud save service)!
        /// Should only be used during development, not in production builds.
        /// </summary>
        void DeleteAll();
    }
}
