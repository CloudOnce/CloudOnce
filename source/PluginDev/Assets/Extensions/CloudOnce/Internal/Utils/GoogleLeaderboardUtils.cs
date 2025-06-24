// <copyright file="GoogleLeaderboardUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#pragma warning disable CS0618 // Type or member is obsolete
#if UNITY_ANDROID && CLOUDONCE_GOOGLE
namespace CloudOnce.Internal.Utils
{
    using System;
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using Providers;
    using UnityEngine.SocialPlatforms;

    /// <summary>
    /// Google Play Game Services implementation of <see cref="ILeaderboardUtils"/>
    /// </summary>
    public class GoogleLeaderboardUtils : ILeaderboardUtils
    {
        #region Public methods

        /// <summary>
        /// Reports a score to a leaderboard.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="score">The score to submit.</param>
        /// <param name="onComplete">
        /// Callback that will be called to report the result of the operation: <c>true</c> on success, <c>false</c> otherwise.
        /// If <c>false</c>, an error message will be included in the callback.
        /// </param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ReportError($"Can't submit score to {internalID} leaderboard. Platform ID is null or empty!", onComplete);
                return;
            }

            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                const string errorMessage = "Can't submit score to leaderboard {0} ({1}). SubmitScore can only be called after authentication.";
                ReportError(string.Format(errorMessage, internalID, id), onComplete);
                return;
            }

            PlayGamesPlatform.Instance.ReportScore(score, id, Callback);
            return;

            void Callback(bool response) => OnSubmitScoreCompleted(response, score, onComplete, id, internalID);
        }

        /// <summary>
        /// Shows the native achievement user interface, allowing the player to browse achievements.
        /// </summary>
        /// <param name="id">Current platform's ID for the leaderboard.</param>
        /// <param name="internalID">Internal CloudOnce ID, if available.</param>
        public void ShowOverlay(string id = "", string internalID = "")
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.LogWarning("ShowOverlay can only be called after authentication.");
#endif
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Showing leaderboards overlay.");
#endif
                PlayGamesPlatform.Instance.ShowLeaderboardUI(null, OnShowOverlayCompleted);
            }
            else
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log(string.IsNullOrEmpty(internalID)
                    ? $"Showing {id} leaderboard overlay."
                    : $"Showing {internalID} ({id}) leaderboard overlay.");
#endif
                PlayGamesPlatform.Instance.ShowLeaderboardUI(id, OnShowOverlayCompleted);
            }
        }

        /// <summary>
        /// Load a default set of scores from the given leaderboard.
        /// </summary>
        /// <param name="leaderboardID">Current platform's ID for the leaderboard.</param>
        /// <param name="callback">Callback with scores.</param>
        public void LoadScores(string leaderboardID, Action<IScore[]> callback) => PlayGamesPlatform.Instance.LoadScores(leaderboardID, callback);

        #endregion /Public methods

        #region Private methods

        private static void OnShowOverlayCompleted(UIStatus callback)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Leaderboards overlay closed.");
#endif
            if (callback == UIStatus.NotAuthorized)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("User logged out from overlay, using guest user from now on.");
#endif
                GooglePlayGamesCloudProvider.Instance.ActivateGuestUserMode();
            }
        }

        private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
        {
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.LogWarning(errorMessage);
#endif
            CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
        }

        private static void OnSubmitScoreCompleted(bool response, long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
        {
            if (response)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log($"Successfully submitted a score of {score} to {internalID} ({id}) leaderboard.");
#endif
                CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
            }
            else
            {
                var error =
                    $"Native API failed to submit a score of {score} to {internalID} ({id}) leaderboard. Cause unknown.";
                ReportError(error, callbackAction);
            }
        }

        #endregion /Private methods
    }
}
#endif
