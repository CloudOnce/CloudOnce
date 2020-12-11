CloudOnce - Unified Game Services API
http://jizc.github.io/CloudOnce

Description
-----------

CloudOnce provides a unified game services API for Google Play Game Services
and iOS Game Center. Ease of use is the primary focus for the plug-in,
but it also attempts to satisfy the needs of power users.

Release Notes
-------------
Version 2.7.2
- Updated GPGS from v0.10.10 to v0.10.12
- Updated EDM4U from v1.2.157 to v1.2.162

Version 2.7.1
- Updated IUserProfile implementation for Unity 2020.1

Version 2.7.0
- Updated GPGS from v0.9.64 to v0.10.10
- Updated EDM4U from v1.2.111 to v1.2.157

Version 2.6.8
- Updated GPGS plugin from v0.9.63 to v0.9.64
- Updated Play Services Resolver from v1.2.104 to v1.2.111

Version 2.6.7
- Fixed AndroidManifest issue in Unity 2018.3
- Updated GPGS plugin from v0.9.62 to v0.9.63
- Updated Play Services Resolver from v1.2.102 to v1.2.104

Version 2.6.6
- Unity 2018.3 managed code stripping fix
- Replaced WWW with UnityWebRequest in GPGS provider
- Updated GPGS plugin from v0.9.61 to v0.9.62
- Fixed editor style error in Unity 2018.3

Version 2.6.5
- Updated GPGS plugin from v0.9.60 to v0.9.61
- Updated Play Services Resolver from v1.2.100 to v1.2.102

Version 2.6.4
- Updated GPGS plugin from v0.9.58 to v0.9.60
- Updated Play Services Resolver from v1.2.98 to v1.2.100

Version 2.6.3
- Updated GPGS plugin from v0.9.53 to v0.9.58
- Updated Play Services Resolver from v1.2.95 to v1.2.98

Version 2.6.2
- Fixed AndroidManifest issues
- Updated GPGS plugin from v0.9.50 to v0.9.53
- Updated Play Services Resolver from v1.2.90 to v1.2.95

Version 2.6.1
- Added method to get achievements and leaderboards by internal ID

Version 2.6.0
- Removed Amazon GameCircle support, due to Amazon discontinuing the service
- Updated Play Services Resolver from v1.2.79 to v1.2.90

Version 2.5.2
- Fixed tvOS support
- Updated Play Services Resolver from v1.2.66 to v1.2.79

Version 2.5.1
- Unity 2018 support
- Updated Play Services Resolver from v1.2.61 to v1.2.66

Version 2.5.0
- Updated GPGS plugin from v0.9.41 to v0.9.50
- Updated Play Services Resolver from v1.2.59 to v1.2.61
- Fixed editor override in DeactivateOnAwakeIfNotGoogle script

Version 2.4.6
- Fixed issues related to switching user on Google Play
- Updated Play Services Resolver from v1.2.56 to v1.2.59

Version 2.4.5
- Fixed exception related to Play Services Resolver on macOS
- Updated Play Services Resolver from v1.2.54 to v1.2.56

Version 2.4.4
- Added option to save settings in Assets, for Unity Collaborate syncing
- Updated Jar Resolver from v1.2.52 to v1.2.54

Version 2.4.3
- Fixed AndroidManifest template for GPGS
- Updated Jar Resolver from v1.2.50 to v1.2.52

Version 2.4.2
- Enabled Apple tvOS support
- Bumped requirements to Unity 5.3.1
- Updated Jar Resolver from v1.2.48 to v1.2.50

Version 2.4.1
- Added Cloud.Storage.ResetVariable method
- Improved compatibility with other plug-ins using Jar Resolver

Version 2.4.0
- Updated GPGS plugin from v0.9.39a to v0.9.41
- Updated Jar Resolver from v1.2.31 to v1.2.48

Version 2.3.8
- Updated Jar Resolver from v1.2.28 to v1.2.31

Version 2.3.7
- Fixed cloud load issue related to PersistenceType.Latest
- Updated GPGS plugin from v0.9.38a to v0.9.39a
- Updated Jar Resolver from v1.2.22 to v1.2.28

Version 2.3.6
- Updated GPGS plugin from v0.9.37 to v0.9.38a
- Updated Jar Resolver from v1.2.16 to v1.2.22

Version 2.3.5
- Updated Jar Resolver from v1.2.15 to v1.2.16

Version 2.3.4
- Unity 5.6 compatibility
- Fix for achievements loading crash on iOS
- Updated GPGS plugin from v0.9.36 to v0.9.37
- Updated Jar Resolver from v1.2.2 to v1.2.15

Version 2.3.3
- Unity 5.5 compatibility
- Added build target check to GPGS setup
- Fixed GameCircle provider
- Fixed error from saving empty GameCircle API key
- Updated GPGS plugin from v0.9.35 to v0.9.36

Version 2.3.2
- Fixed Editor path

Version 2.3.1
- Fixed init of providers
- Updated GPGS plugin from v0.9.34 to v0.9.35

Version 2.3.0
- Removed Trollpants namespace
- Removed outdated iCloud platform check
- Renamed Amazon, Google and Debug build symbols
- Added latest GPGS changes

Version 2.2.5
- Fix for editor error in Unity 5.4
- Changed copyright
- Enforced UTF-8 file encoding
- Updated URLs

Version 2.2.4
- Updated menu URLs

Version 2.2.3
- Updated GPGS plugin from v0.9.33 to v0.9.34
- Changed license to MIT and made source available on GitHub

Version 2.2.2
- Updated GPGS plugin from v0.9.32 to v0.9.33, this should fix threading issues with Google callbacks
- Fixed rare case where OnInitializeComplete would not be called
- Put hard-coded paths in single class, for easier alteration of folder structure

Version 2.2.1
- Cloud saving can now be toggled on/off post initialization, will save to disk regardless
- Save method now saves to disk on unsupported platforms

Version 2.2.0
- Added CloudLong type
- Added CloudDateTime type
- Added CloudDecimal type
- Added ServiceName property to Cloud class
- Further reduced size of saved cloud data

Version 2.1.0
- Added LoadUsers method
- Added LoadAchievements method
- Added LoadAchievementDescriptions method
- Jar background resolution is now disabled OnLoad
- Changed DEBUG build symbol to CLOUDONCE_DEBUG

Version 2.0.0
- A lot of breaking API changes. Upgrade is not recommended for existing projects close to a deadline.
  If you choose to upgrade, the previous version must be deleted from the project before importing the new version.
- All commands that started with Cloud.Provider now just start with Cloud,
  for example Cloud.Provider.Storage.Load is now Cloud.Storage.Load
- Removed the need for the READ_PHONE_STATUS permission on Android
- Added autoSignIn and autoLoad parameters to the Initialize method to allow for more flexibility
- Added SignIn and SignOut methods
- Added Synchronize, DeleteVariable, ClearUnusedVariables and DeleteAll methods to Storage
- Added undo functionality to the CloudOnce Editor
- Reduced size of saved cloud data
- Removed hard-coded paths from the CloudOnce Editor
- Switched to Jar Resolver for importing Android SDK libraries. This makes CloudOnce compatible with other Google plug-ins like AdMob.
- Streamlined constructors for cloud variables
- Merged OnSignedIn and OnSignedOut events into OnSignedInChanged<bool>
- Renamed a few classes to better reflect their function
- Deleted obsolete files
- Code clean-up
- Updated GPGS plugin from v0.9.31 to v0.9.32
- Updated Jar Resolver to v1.1.1
- Updated GameCircle libraries to v2.5.3
- Sunsetting support for Unity 4

Version 1.8.1
- Added DeleteAllCloudVariables method
- Updated GPGS plugin from v0.9.30 to v0.9.31
- Fixed ResetAllData method
- Fixed GPGS being initialized with wrong settings if calling Save or Load before Initialize

Version 1.8.0
- Added OnSignedOut event
- Updated GPGS plugin from v0.9.27 to v0.9.30
- Changed OnCloudLoadComplete callback to return true when there is no cloud data
- Made Save method more consistent across providers
- Fixed threading problem with Google Play guest user
- Code cleanup

Version 1.7.4
- Unity 4.7 compatibility

Version 1.7.3
- Unity 5.3 compatibility

Version 1.7.2
- Added dummy cloud provider to avoid exceptions when building to unsupported platforms
- Updated GPGS plugin from v0.9.26 to v0.9.27

Version 1.7.1
- Added unified API for LoadScores method
- Updated GPGS plugin from v0.9.25 to v0.9.26

Version 1.7.0
- Added option to be sent directly to specified leaderboard overlay
- Added string array of changed internal IDs to OnNewCloudValues event,
  any methods subscribed to this event will have to be updated
- GameCircle API key is now added through CloudOnce editor
- Updated GPGS plugin from v0.9.21 to v0.9.25
- Added simulation of Google Sign In/Out button in editor
- Fixed compatibility issues with new GameCircle Android Library
- Fixed login bug on Google Play when checking IsSignedIn before calling Initialize method
- Cleaned up Amazon GameCircle debug logs
- Required Unity version raised to v4.5.5 to ensure GPGS compatibility

Version 1.6.0
- New dockable editor with responsive design
- Cloud variables can now be declared in editor and are easily accessed from any script in any scene
- Added support for Unity 4.3.4 and up
- New automatic upgrader to delete obsolete files
- New automatic addition of GameKit to plist after iOS builds
- New unified API for PlayerID, PlayerDisplayName and PlayerImage
- New "Deactivate On Event" quick start script
- New "Load Scene On Event" quick start script
- Re-added default value support for cloud currency types
- Added Highest and Lowest persistence types support to CloudString
- Improved Achievement- and Leaderboard Overlay button scripts
- Unity Editor test data can now be easily deleted from CloudOnce Editor
- NO_GPGS build symbol now automatically added on load for iOS
- Moved GameCircle plug-in assets into Android Library project
- Moved settings file to ProjectSettings folder

Version 1.5.2
- Improved Android device compatibility
- Fixed deserialization of old cloud currency data
- Fixed bug related to saving cloud currency
- Prepared more files for Unity 4.3 compatibility update

Version 1.5.1
- Added more debug logs to initialization to help with troubleshooting
- Renamed some events to make their function clearer
- Improved "Show Overlay" Quick Start scripts
- Improved API documentation
- Improved WhisperSync integration
- Improved Initialize methods
- Improved handling of deserialization failure
- Improved handling of unstable Google Play authentication

Version 1.5.0
- Rebuilt Achievements and Leaderboards systems to be more intuitive, and require less code.
  The Getting Started guides will be updated to reflect the changes.
- Added DeleteCloudPref method to Cloud class
- Added overload constructor to CloudUInt
- Improved allowed characters validation in editor
- Prepared files for Unity 4.3 compatibility
- Fixed bugs in Currency CloudPrefs
- Fixed Reset methods
- Removed DES encryption due to performance and compatibility issues,
  will replace with hash + salt system in a later update

Version 1.4.0
- Added DES encryption to both local and cloud data
- Reworked CloudCurrency types to support checking total additions and subtractions
- Added option to allow for currencies to be negative
- Added more overload constructors to all CloudPrefs, no longer necessary to supply value
- Improved handling of allowed characters in the ID fields of the editor
- Removed broken "initial value" feature from CloudCurrency types
- Optimized data serialization

Version 1.3.2
- Improved implementation of Cloud Save deactivation
- Improved handling of Guest User on Google Play
- Improvements to several Quick Start scripts

Version 1.3.1
- Updated GPGS plugin from v0.9.20 to v0.9.21

Version 1.3.0
- Added OnSaveComplete and OnLoadComplete events
- Added optional parameter to Initialize for disabling Cloud Save

Version 1.2.2
- Improved iCloud implementation, no longer dependent on Game Center sign in
- Added Quick Start Scripts to Component menu
- Fixed support for Mono2x scripting backend on iOS

Version 1.2.1
- Fixed problem with building project before saving CloudOnce configuration
- Fixed GameCircle support when targeting APIs below 11

Version 1.2.0
- AutoLoad system
- OnNewCloudValues now only gets called when there are new/changed cloud values
- iCloud wrapper now checks if signed in before accessing cloud data
- Fixed editor debug warning from CloudIDs
- Fixed saving CloudPrefs when in editor

Version 1.1.1
- Improved WhisperSync implementation

Version 1.1.0
- Updated GPGS plugin from v0.9.15 to v0.9.20
- Fixed bug when building Amazon build before running Google App ID setup
- Added missing Amazon GameCircle resource files
- More stable Amazon GameCircle login, achievement and WhisperSync handling
- Exposed Load method in storage wrappers

Version 1.0.3
- Improved editor visuals when using dark theme
- Improved Unity 5 compatibility
- Removed unnecessary files from package

Version 1.0.2
- Improved editor visuals when using dark theme

Version 1.0.1
- Fixed bug where new local data would not get uploaded to the cloud
- Improved Google Play Sign In/Out Quick Start script

Version 1.0.0
- Initial release