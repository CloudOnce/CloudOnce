// <copyright file="InternetConnectionUtils.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal.Utils
{
    using System.Net;
    using System.IO;
    using System.Linq;

    public static class InternetConnectionUtils
    {
        /// <summary>
        /// Get Internet connection status.
        /// </summary>
        /// <returns>Connection status.</returns>
        public static InternetConnectionStatus GetConnectionStatus()
        {
            var htmlText = GetHtmlFromUrl("http://google.com");
            if (string.IsNullOrEmpty(htmlText))
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Internet connection status: Disconnected");
#endif
                return InternetConnectionStatus.Disconnected;
            }

            if (!htmlText.Contains("schema.org/WebPage"))
            {
                // Expected phrase was not found
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("Internet connection status: Unstable connection");
#endif
                return InternetConnectionStatus.Unstable;
            }

#if CLOUDONCE_DEBUG
            UnityEngine.Debug.Log("Internet connection status: Connected");
#endif
            return InternetConnectionStatus.Connected;
        }

        /// <summary>
        /// Returns 80 characters of HTML from a specified URL.
        /// </summary>
        /// <param name="url">The URL to get the HTML from.</param>
        /// <param name="charCount">How many characters of the HTML to retrieve. Default is 80.</param>
        /// <returns>A specified number of characters of HTML from a specified URL.</returns>
        private static string GetHtmlFromUrl(string url, int charCount = 80)
        {
            var html = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                using (var resp = (HttpWebResponse)req.GetResponse())
                {
                    var isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                    if (isSuccess)
                    {
                        var responseStream = resp.GetResponseStream();
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                var cs = new char[charCount];
                                reader.Read(cs, 0, cs.Length);
                                html = cs.Aggregate(html, (current, ch) => current + ch);
                            }
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return html;
        }
    }
}
