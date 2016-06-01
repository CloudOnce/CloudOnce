/**
 * © 2012-2013 Amazon Digital Services, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except in compliance with the License. A copy
 * of the License is located at
 *
 * http://aws.amazon.com/apache2.0/
 *
 * or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */
using UnityEngine;

/// <summary>
/// Plugins can often produce unwanted and noisy log output.
/// AmazonLogging provides a common interface for Amazon plugins so
/// developers can turn down the messaging output when necessary.
/// </summary>
/// <exception cref='System.Exception'>
/// Represents errors that occur during application execution.
/// </exception>
public class AmazonLogging {
    
    // This is the logging level used by Amazon plugins.
    public enum AmazonLoggingLevel {
        Silent,             // All message output is disabled.
        Critical,           // Only critical messages are displayed.
        ErrorsAsExceptions, // Errors (in Unity) are reported as exceptions
        Errors,             // Only error and critical messages are displayed.
        Warnings,           // Warning, error, and critical messages are displayed.
        Verbose,            // All output is displayed.
    }
        
    // These are the logging values used by the SDK internally.
    public enum SDKLoggingLevel {
        LogOff,
        LogCritical,
        LogError,
        LogWarning,
    }
    
    // {0} is the Amazon service, {1} is the error message.
    private const string errorMessage = "{0} error: {1}"; 
    private const string warningMessage = "{0} warning: {1}"; 
    private const string logMessage = "{0}: {1}"; 
    
    #region Public functions
    /// <summary>
    /// Logs the error.
    /// </summary>
    /// <param name='reportLevel'>
    /// Report level.
    /// </param>
    /// <param name='service'>
    /// Service.
    /// </param>
    /// <param name='message'>
    /// Message.
    /// </param>
    public static void LogError(AmazonLoggingLevel reportLevel, string service, string message) {
        if(reportLevel == AmazonLoggingLevel.Silent) {
            return;
        }
            
        string serviceAndMessage = string.Format(errorMessage,service,message);
        
        switch(reportLevel) {
        case AmazonLoggingLevel.ErrorsAsExceptions:
            throw new System.Exception(serviceAndMessage);
        case AmazonLoggingLevel.Critical:
        case AmazonLoggingLevel.Errors:
        case AmazonLoggingLevel.Warnings:
        case AmazonLoggingLevel.Verbose:
            Debug.LogError(serviceAndMessage);
            break;
        default:
            break;
        }
    }
    
    /// <summary>
    /// Logs the warning.
    /// </summary>
    /// <param name='reportLevel'>
    /// Report level.
    /// </param>
    /// <param name='service'>
    /// Service.
    /// </param>
    /// <param name='message'>
    /// Message.
    /// </param>
    public static void LogWarning(AmazonLoggingLevel reportLevel, string service, string message) {
        switch(reportLevel) {
        case AmazonLoggingLevel.Silent:
        case AmazonLoggingLevel.Critical:
        case AmazonLoggingLevel.ErrorsAsExceptions:
        case AmazonLoggingLevel.Errors:
            // Do not log warnings at these levels.
            break;
        case AmazonLoggingLevel.Warnings:
        case AmazonLoggingLevel.Verbose:
            Debug.LogWarning(string.Format(warningMessage,service,message));
            break;
        }
    }
    
    /// <summary>
    /// Log the specified reportLevel, service and message.
    /// </summary>
    /// <param name='reportLevel'>
    /// Report level.
    /// </param>
    /// <param name='service'>
    /// Service.
    /// </param>
    /// <param name='message'>
    /// Message.
    /// </param>
	public static void Log(AmazonLoggingLevel reportLevel, string service, string message) {
        if(reportLevel != AmazonLoggingLevel.Verbose) {
            return;
        }
            
    	Debug.Log(string.Format(logMessage,service,message));
    }
    
    /// <summary>
    /// Converts the plugin logging level to the SDK logging level.
    /// </summary>
    /// <returns>
    /// The to SDK logging level.
    /// </returns>
    /// <param name='pluginLoggingLevel'>
    /// Plugin logging level.
    /// </param>
    public static SDKLoggingLevel pluginToSDKLoggingLevel(AmazonLoggingLevel pluginLoggingLevel) {
        switch(pluginLoggingLevel) {   
        case AmazonLoggingLevel.Silent:
            return SDKLoggingLevel.LogOff;
        case AmazonLoggingLevel.Critical:
            return SDKLoggingLevel.LogCritical;
        case AmazonLoggingLevel.Errors:
        case AmazonLoggingLevel.ErrorsAsExceptions:
            return SDKLoggingLevel.LogError;
        case AmazonLoggingLevel.Warnings:
        case AmazonLoggingLevel.Verbose:
            return SDKLoggingLevel.LogWarning;
        default:
            // The default SDK logging level is warning
            return SDKLoggingLevel.LogWarning;
        }
    }
    #endregion
}
