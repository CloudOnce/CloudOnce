// <copyright file="GPGSStrings.cs" company="Google Inc.">
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
    public class GPGSStrings
    {
        public const string Error = "Error";
        public const string Ok = "OK";
        public const string Cancel = "Cancel";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string Success = "Success";
        public const string Warning = "Warning";

        public class Setup
        {
            public const string AppIdTitle = "Google Play Games Application ID";
            public const string AppId = " Google Application ID";

            public const string AppIdBlurb =
                "Enter your application ID below. This is the " +
                "numeric identifier provided by the Google " +
                "Play Developer Console.\nExample: 123456789012";

            public const string AppIdError =
                "The Google App Id does not appear to be valid. " + "It must consist solely of digits, usually 10 or more.";

            public const string NearbyServiceId = "Nearby Connection Service ID";
            public const string NearbyServiceBlurb = "Enter the service id that identifies the " +
                                                     "nearby connections service scope";

            public const string SetupButton = "Run setup";
        }

        public class NearbyConnections
        {
            public const string Title = "Google Play Games - Nearby Connections Setup";
            public const string Blurb = "To configure Nearby Connections in this project,\n" +
                                        "please enter the information below and click on the Setup button.";

            public const string SetupComplete = "Nearby connections configured successfully.";
        }

        public class AndroidSetup
        {
            public const string Title = "Google Play Games - Android Configuration";

            public const string Blurb =
                "To configure Google Play Games in this project,\n" + "please enter the information below and click on the Setup button.";

            public const string PkgName = "Package name";
            public const string PkgNameBlurb = "Enter your application's package name below.\n" + "(for example, com.example.lorem.ipsum).";

            public const string PackageNameError =
                "The package name does not appear to be valid. "
                + "Enter a valid Android package name (for example, com.example.lorem.ipsum).";

            public const string SdkNotFound = "Android SDK Not found";

            public const string SdkNotFoundBlurb =
                "The Android SDK path was not found. " + "Please configure it in the Unity preferences window (under External Tools).";

            public const string LibProjNotFound = "Google Play Services Library Project Not Found";

            public const string LibProjNotFoundBlurb =
                "Google Play Services library project " + "could not be found your SDK installation. Make sure it is installed (open "
                + "the SDK manager and go to Extras, and select Google Play Services).";

            public const string SupportJarNotFound = "Android Support Library v4 Not Found";

            public const string SupportJarNotFoundBlurb =
                "Android Support Library v4 " + "could not be found your SDK installation. Make sure it is installed (open "
                + "the SDK manager and go to Extras, and select 'Android Support Library').";

            public const string LibProjVerNotFound =
                "The version of your copy of the Google Play " + "Services Library Project could not be determined. Please make sure it is "
                + "at least version {0}. Continue?";

            public const string LibProjVerTooOld =
                "Your copy of the Google Play " + "Services Library Project is out of date. Please launch the Android SDK manager "
                + "and upgrade your Google Play Services bundle to the latest version (your version: "
                + "{0}; required version: {1}). Proceeding may cause problems. Proceed anyway?";

            public const string SetupComplete = "Google Play Games configured successfully.";
        }
    }
}
