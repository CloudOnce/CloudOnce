// <copyright file="PlistBuddyHelper.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

// Modified by Jan Ivar Z. Carlsen.
namespace CloudOnce.Internal.Editor.Utils
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Diagnostics;

    internal sealed class PlistBuddyHelper
    {
        private const string plistBuddyPath = "/usr/libexec/PlistBuddy";
        private readonly string mPlistPath;

        private PlistBuddyHelper(string plistPath)
        {
            mPlistPath = plistPath;
        }

        public static string ToEntryName(params object[] fields)
        {
            return string.Join(":", fields.Select(o => o.ToString()).ToArray());
        }

        internal static PlistBuddyHelper ForPlistFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return null;
            }

            return File.Exists(plistBuddyPath) ? new PlistBuddyHelper(filepath) : null;
        }

        internal bool AddArray(params object[] fieldPath)
        {
            return ExecuteCommand("add " + ToEntryName(fieldPath) + " array") != null;
        }

        internal bool AddDictionary(params object[] fieldPath)
        {
            return ExecuteCommand("add " + ToEntryName(fieldPath) + " dict") != null;
        }

        internal bool AddString(string fieldPath, string stringValue)
        {
            return ExecuteCommand("add " + fieldPath + " string " + stringValue) != null;
        }

        internal void RemoveEntry(params object[] fieldPath)
        {
            ExecuteCommand("remove " + ToEntryName(fieldPath));
        }

        internal string EntryValue(params object[] fieldPath)
        {
            var value = ExecuteCommand("print " + ToEntryName(fieldPath));

            // PlistBuddy adds a trailing newline onto the output - strip it here.
            if (value != null)
            {
                return value.Replace("\n", string.Empty);
            }

            return null;
        }

        private string ExecuteCommand(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "/usr/libexec/PlistBuddy";
                process.StartInfo.Arguments = string.Format("-c \"{0}\" \"{1}\"", command, mPlistPath);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                try
                {
                    process.Start();
                    process.StandardError.ReadToEnd();
                    var stdOutput = process.StandardOutput.ReadToEnd();

                    if (!process.WaitForExit(10 * 1000))
                    {
                        throw new Exception("PlistBuddy did not exit in a timely fashion");
                    }

                    if (process.ExitCode != 0)
                    {
                        return null;
                    }

                    return stdOutput.Replace("\n", string.Empty);
                }
                catch (Exception e)
                {
                    throw new Exception("Encountered unexpected error while editing Info.plist.", e);
                }
            }
        }
    }
}