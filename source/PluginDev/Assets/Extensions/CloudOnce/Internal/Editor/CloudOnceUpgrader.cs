// <copyright file="CloudOnceUpgrader.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2017 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#if UNITY_EDITOR
namespace CloudOnce.Internal.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Data;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    [InitializeOnLoad]
    public class CloudOnceUpgrader
    {
        static CloudOnceUpgrader()
        {
            if (Environment.CommandLine.ToLower().Contains("-upgrader_disable"))
            {
                Debug.Log("CloudOnce Upgrader disabled");
                return;
            }

            var config = SerializationUtils.LoadCloudConfig();
            if (string.IsNullOrEmpty(config.Version))
            {
                return;
            }

            var oldVersion = new Version(config.Version);
            if (PluginVersion.Version.Equals(oldVersion))
            {
                return;
            }

            var upgraded = false;
            if (oldVersion < new Version(2, 4))
            {
                Upgrade240();
                upgraded = true;
            }

            if (oldVersion < new Version(2, 5))
            {
                Upgrade250();
                upgraded = true;
            }

            if (oldVersion < new Version(2, 7))
            {
                Upgrade270();
                upgraded = true;
            }

            if (upgraded)
            {
                AssetDatabase.Refresh();
            }
        }

        private static void Upgrade240()
        {
            var obsoleteFiles = new[]
            {
                "Assets/Extensions/GooglePlayGames/Editor/GPGSDependencies.cs",
                "Assets/Plugins/Android/libs/armeabi-v7a/libgpg.so",
                "Assets/Plugins/Android/libs/x86/libgpg.so",
                "Assets/Plugins/Android/MainLibProj/libs/play-games-plugin-support.jar",
                "Assets/Plugins/Android/MainLibProj/AndroidManifest.xml",
                "Assets/Plugins/Android/MainLibProj/project.properties"
            };

            DeleteObsoleteFiles(obsoleteFiles);
        }

        private static void Upgrade250()
        {
            var obsoleteFiles = new[]
            {
                "Assets/Extensions/GooglePlayGames/BasicApi/Quests/IQuest.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Quests/IQuestMilestone.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Quests/IQuestsClient.cs",
                "Assets/Extensions/GooglePlayGames/Editor/GPGSDependencies.xml",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Quest.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/QuestManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/QuestMilestone.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeQuestClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeQuest.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeQuestMilestone.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/QuestManager.cs",
                "Assets/Plugins/Android/play-games-plugin-support.aar"
            };

            DeleteObsoleteFiles(obsoleteFiles);
        }

        private static void Upgrade270()
        {
            var obsoleteFiles = new[]
            {
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/IRealTimeMultiplayerClient.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/ITurnBasedMultiplayerClient.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/Invitation.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/MatchOutcome.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/Participant.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/Player.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/RealTimeMultiplayerListener.cs",
                "Assets/Extensions/GooglePlayGames/BasicApi/Multiplayer/TurnBasedMatch.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Developers/JavaInterfaceProxy.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Developers/JavaObjWrapper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/Api/GoogleApiClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/Api/PendingResult.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/Api/Result.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/Api/ResultCallback.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/Api/ResultCallbackProxy.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/Api/Status.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Common/ConnectionResult.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Games/Games.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Games/Stats/LoadPlayerStatsResultObject.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Games/Stats/PlayerStats.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Games/Stats/PlayerStatsObject.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Games/Stats/Stats.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Android/Gms/Games/Stats/StatsObject.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/CallbackUtils.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/ConversionUtils.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Achievement.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/AchievementManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/AndroidPlatformConfiguration.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Builder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/CaptureOverlayStateListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/CommonErrorStatus.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/EndpointDiscoveryListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Event.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/EventManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/GameServices.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/InternalHooks.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Leaderboard.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/LeaderboardManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/MessageListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/MultiplayerInvitation.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/MultiplayerParticipant.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/NearbyConnectionTypes.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/NearbyConnections.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/NearbyConnectionsBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/NearbyConnectionsStatus.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/ParticipantResults.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Player.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/PlayerManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/PlayerStats.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/RealTimeEventListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/RealTimeMultiplayerManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/RealTimeRoom.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/RealTimeRoomConfig.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/RealTimeRoomConfigBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Score.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/ScorePage.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/ScoreSummary.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Sentinels.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/SnapshotManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/SnapshotMetadata.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/SnapshotMetadataChange.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/SnapshotMetadataChangeBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/StatsManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Status.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/SymbolLocation.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/TurnBasedMatch.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/TurnBasedMatchConfig.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/TurnBasedMatchConfigBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/TurnBasedMultiplayerManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/Types.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/VideoCapabilities.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/VideoCaptureState.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Cwrapper/VideoManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/JavaUtils.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeEventClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeNearbyConnectionClientFactory.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeNearbyConnectionsClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeRealtimeMultiplayerClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeSavedGameClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeTurnBasedMultiplayerClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/NativeVideoClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/AchievementManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/AndroidPlatformConfiguration.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/BaseReferenceHolder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/Callbacks.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/CaptureOverlayStateListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/EventManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/GameServices.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/GameServicesBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/LeaderboardManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/MultiplayerInvitation.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/MultiplayerParticipant.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeAchievement.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeAppIdentifier.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeConnectionRequest.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeConnectionResponse.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeEndpointDetails.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeEndpointDiscoveryListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeEvent.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeLeaderboard.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeMessageListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativePlayer.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativePlayerStats.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeRealTimeRoom.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeScore.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeScoreEntry.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeScorePage.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeScorePageToken.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeScoreSummary.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeSnapshotMetadata.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeSnapshotMetadataChange.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeStartAdvertisingResult.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeTurnBasedMatch.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeVideoCapabilities.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NativeVideoCaptureState.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NearbyConnectionsManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/NearbyConnectionsManagerBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/PInvokeUtilities.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/ParticipantResults.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/PlatformConfiguration.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/PlayerManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/PlayerSelectUIResponse.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/RealTimeEventListenerHelper.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/RealtimeManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/RealtimeRoomConfig.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/RealtimeRoomConfigBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/SnapshotManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/StatsManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/TurnBasedManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/TurnBasedMatchConfig.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/TurnBasedMatchConfigBuilder.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/PInvoke/VideoManager.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Platforms/Android/AndroidClient.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/Platforms/IClientImpl.cs",
                "Assets/Extensions/GooglePlayGames/Platforms/Native/UnsupportedSavedGamesClient.cs",
            };

            DeleteObsoleteFiles(obsoleteFiles);
        }

        private static void DeleteObsoleteFiles(IEnumerable<string> obsoleteFiles)
        {
            foreach (var file in obsoleteFiles.Where(File.Exists))
            {
                Debug.Log("Deleting obsolete file: " + file);
                File.Delete(file);
                var metaFile = file + ".meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }

                var directory = Path.GetDirectoryName(file);
                if (directory == null)
                {
                    continue;
                }

                var directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.GetFiles().Length != 0)
                {
                    continue;
                }

                Debug.Log("Deleting obsolete directory: " + directory);
                directoryInfo.Delete();
                if (directoryInfo.Parent != null)
                {
                    var meta = Path.Combine(directoryInfo.Parent.FullName, directoryInfo.Name + ".meta");
                    if (File.Exists(meta))
                    {
                        File.Delete(meta);
                    }
                }
            }
        }
    }
}
#endif
