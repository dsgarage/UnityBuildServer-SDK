using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSGarage.UnityBuildServer.Models;

namespace DSGarage.UnityBuildServer.Api
{
    /// <summary>
    /// FBX4VRM Bug Report API Client
    /// Runtime-compatible client for submitting bug reports and managing avatars.
    ///
    /// <para>
    /// This client provides easy access to the FBX4VRM Bug Report API for submitting
    /// conversion reports, retrieving avatar lists, and managing bug reports.
    /// </para>
    ///
    /// <example>
    /// <code>
    /// // Example 1: Using the singleton instance
    /// public class BugReporter : MonoBehaviour
    /// {
    ///     void Start()
    ///     {
    ///         // Get avatar list
    ///         StartCoroutine(FBX4VRMApiClient.Instance.GetAvatarList(
    ///             onSuccess: response => Debug.Log($"Found {response.avatars.Count} avatars"),
    ///             onError: error => Debug.LogError(error)
    ///         ));
    ///     }
    /// }
    ///
    /// // Example 2: Submit a bug report with screenshot
    /// public void SubmitBugReport(Texture2D screenshot)
    /// {
    ///     var request = FBX4VRMApiClient.CreateBugReport(
    ///         modelName: "MyAvatar_v1.0",
    ///         success: false,
    ///         packageVersion: "1.0.0",
    ///         unityVersion: Application.unityVersion,
    ///         errorMessage: "Material conversion failed"
    ///     );
    ///
    ///     // Add screenshot
    ///     FBX4VRMApiClient.AddScreenshot(request, screenshot);
    ///
    ///     StartCoroutine(FBX4VRMApiClient.Instance.SubmitBugReport(
    ///         request,
    ///         onSuccess: response => {
    ///             Debug.Log($"Report ID: {response.report_id}");
    ///             Debug.Log($"GitHub Issue: {response.github_issue_url}");
    ///         },
    ///         onError: error => Debug.LogError(error)
    ///     ));
    /// }
    ///
    /// // Example 3: Using a custom server URL
    /// void ConfigureCustomServer()
    /// {
    ///     FBX4VRMApiClient.Configure("https://my-server.com:8443");
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class FBX4VRMApiClient : ApiClientBase, IFBX4VRMApiClient
    {
        private const string DEFAULT_SERVER_URL = "https://153.126.176.139:8443";
        private const string API_PREFIX = "/api/v1/fbx4vrm";

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static FBX4VRMApiClient _instance;
        public static FBX4VRMApiClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FBX4VRMApiClient(DEFAULT_SERVER_URL);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Create a new API client with specified server URL
        /// </summary>
        public FBX4VRMApiClient(string serverUrl) : base(serverUrl)
        {
        }

        /// <summary>
        /// Configure the singleton instance with a custom server URL
        /// </summary>
        public static void Configure(string serverUrl)
        {
            _instance = new FBX4VRMApiClient(serverUrl);
        }

        /// <summary>
        /// Get the current server URL (for debugging)
        /// </summary>
        public static string GetServerUrl() => Instance.ServerUrl;

        #region API Info

        /// <summary>
        /// Get API information
        /// </summary>
        public IEnumerator GetApiInfo(Action<ApiResponse<FBX4VRMApiInfoResponse>> callback)
        {
            yield return GetAsync(API_PREFIX, callback);
        }

        #endregion

        #region Avatar List

        /// <summary>
        /// Get list of avatars with bug reports
        /// </summary>
        public IEnumerator GetAvatarList(Action<ApiResponse<FBX4VRMAvatarListResponse>> callback)
        {
            yield return GetAsync($"{API_PREFIX}/avatars", callback);
        }

        /// <summary>
        /// Get avatar list (simplified callback)
        /// </summary>
        public IEnumerator GetAvatarList(
            Action<FBX4VRMAvatarListResponse> onSuccess,
            Action<string> onError = null)
        {
            yield return GetAvatarList(response =>
            {
                if (response.Success && response.Data != null)
                {
                    onSuccess?.Invoke(response.Data);
                }
                else
                {
                    onError?.Invoke(response.Error ?? "Unknown error");
                }
            });
        }

        #endregion

        #region Bug Report Submission (Queue - Recommended)

        private const string QUEUE_PREFIX = "/bug-reports/queue";

        /// <summary>
        /// Submit a bug report to the queue system (recommended).
        /// Reports are processed asynchronously in the background.
        /// </summary>
        public IEnumerator SubmitToQueue(
            FBX4VRMBugReportRequest request,
            Action<ApiResponse<FBX4VRMQueueSubmitResponse>> callback)
        {
            // Ensure required fields are set
            if (string.IsNullOrEmpty(request.report_id))
            {
                request.report_id = FBX4VRMBugReportRequest.GenerateReportId();
            }
            if (string.IsNullOrEmpty(request.timestamp))
            {
                request.timestamp = FBX4VRMBugReportRequest.GetTimestamp();
            }

            yield return PostJsonAsync<FBX4VRMBugReportRequest, FBX4VRMQueueSubmitResponse>(
                $"{QUEUE_PREFIX}/submit",
                request,
                callback
            );
        }

        /// <summary>
        /// Submit a bug report to the queue system (simplified callback).
        /// This is the recommended method for submitting bug reports.
        /// </summary>
        public IEnumerator SubmitToQueue(
            FBX4VRMBugReportRequest request,
            Action<FBX4VRMQueueSubmitResponse> onSuccess,
            Action<string> onError = null)
        {
            yield return SubmitToQueue(request, response =>
            {
                if (response.Success && response.Data != null && response.Data.IsSuccess)
                {
                    onSuccess?.Invoke(response.Data);
                }
                else
                {
                    string error = response.Error ?? response.Data?.message ?? "Unknown error";
                    onError?.Invoke(error);
                }
            });
        }

        /// <summary>
        /// Get queue statistics
        /// </summary>
        public IEnumerator GetQueueStats(Action<ApiResponse<FBX4VRMQueueStatsResponse>> callback)
        {
            yield return GetAsync($"{QUEUE_PREFIX}/stats", callback);
        }

        /// <summary>
        /// Get queue statistics (simplified callback)
        /// </summary>
        public IEnumerator GetQueueStats(
            Action<FBX4VRMQueueStatsResponse> onSuccess,
            Action<string> onError = null)
        {
            yield return GetQueueStats(response =>
            {
                if (response.Success && response.Data != null)
                {
                    onSuccess?.Invoke(response.Data);
                }
                else
                {
                    onError?.Invoke(response.Error ?? "Unknown error");
                }
            });
        }

        #endregion

        #region Bug Report Submission (Direct - Legacy)

        /// <summary>
        /// Submit a bug report directly (legacy method).
        /// Consider using SubmitToQueue() instead for better reliability.
        /// </summary>
        public IEnumerator SubmitBugReport(
            FBX4VRMBugReportRequest request,
            Action<ApiResponse<FBX4VRMBugReportResponse>> callback)
        {
            // Ensure required fields are set
            if (string.IsNullOrEmpty(request.report_id))
            {
                request.report_id = FBX4VRMBugReportRequest.GenerateReportId();
            }
            if (string.IsNullOrEmpty(request.timestamp))
            {
                request.timestamp = FBX4VRMBugReportRequest.GetTimestamp();
            }

            yield return PostJsonAsync<FBX4VRMBugReportRequest, FBX4VRMBugReportResponse>(
                $"{API_PREFIX}/bug-reports",
                request,
                callback
            );
        }

        /// <summary>
        /// Submit a bug report directly (legacy method, simplified callback).
        /// Consider using SubmitToQueue() instead for better reliability.
        /// </summary>
        public IEnumerator SubmitBugReport(
            FBX4VRMBugReportRequest request,
            Action<FBX4VRMBugReportResponse> onSuccess,
            Action<string> onError = null)
        {
            yield return SubmitBugReport(request, response =>
            {
                if (response.Success && response.Data != null && response.Data.IsSuccess)
                {
                    onSuccess?.Invoke(response.Data);
                }
                else
                {
                    string error = response.Error ?? response.Data?.status ?? "Unknown error";
                    onError?.Invoke(error);
                }
            });
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create a minimal bug report request
        /// </summary>
        public static FBX4VRMBugReportRequest CreateBugReport(
            string modelName,
            bool success,
            string packageVersion,
            string unityVersion,
            string errorMessage = null,
            string stoppedAtProcessor = null)
        {
            return new FBX4VRMBugReportRequest
            {
                report_id = FBX4VRMBugReportRequest.GenerateReportId(),
                timestamp = FBX4VRMBugReportRequest.GetTimestamp(),
                platform = "fbx4vrm",
                environment = new FBX4VRMEnvironment
                {
                    package_version = packageVersion,
                    unity_version = unityVersion,
                    platform = Application.platform.ToString()
                },
                source_model = new FBX4VRMSourceModel
                {
                    name = modelName,
                    is_new_avatar = true
                },
                result = new FBX4VRMResult
                {
                    success = success,
                    error_message = errorMessage,
                    stopped_at_processor = stoppedAtProcessor
                }
            };
        }

        /// <summary>
        /// Create a bug report request for an existing avatar
        /// </summary>
        public static FBX4VRMBugReportRequest CreateBugReportForExistingAvatar(
            string avatarId,
            string modelName,
            bool success,
            string packageVersion,
            string unityVersion,
            string errorMessage = null,
            string stoppedAtProcessor = null)
        {
            var request = CreateBugReport(modelName, success, packageVersion, unityVersion, errorMessage, stoppedAtProcessor);
            request.source_model.avatar_id = avatarId;
            request.source_model.is_new_avatar = false;
            return request;
        }

        /// <summary>
        /// Add screenshot to bug report
        /// </summary>
        public static void AddScreenshot(
            FBX4VRMBugReportRequest request,
            Texture2D screenshot,
            int width = 0,
            int height = 0)
        {
            if (screenshot == null) return;

            request.screenshot = new FBX4VRMScreenshot
            {
                format = "PNG",
                width = width > 0 ? width : screenshot.width,
                height = height > 0 ? height : screenshot.height,
                base64 = Utils.JsonHelper.TextureToBase64(screenshot, true)
            };
        }

        /// <summary>
        /// Add screenshot from byte array
        /// </summary>
        public static void AddScreenshot(
            FBX4VRMBugReportRequest request,
            byte[] screenshotBytes,
            int width,
            int height)
        {
            if (screenshotBytes == null || screenshotBytes.Length == 0) return;

            request.screenshot = new FBX4VRMScreenshot
            {
                format = "PNG",
                width = width,
                height = height,
                base64 = Utils.JsonHelper.BytesToBase64(screenshotBytes)
            };
        }

        /// <summary>
        /// Add multi-angle screenshots to bug report
        /// </summary>
        /// <param name="request">Bug report request</param>
        /// <param name="screenshots">Dictionary of angle name to Texture2D</param>
        public static void AddMultiAngleScreenshots(
            FBX4VRMBugReportRequest request,
            Dictionary<string, Texture2D> screenshots)
        {
            if (screenshots == null || screenshots.Count == 0) return;

            if (request.screenshot == null)
            {
                request.screenshot = new FBX4VRMScreenshot { format = "PNG" };
            }

            foreach (var kvp in screenshots)
            {
                if (kvp.Value == null) continue;
                request.screenshot.angles.Add(kvp.Key);
                request.screenshot.base64_images.Add(Utils.JsonHelper.TextureToBase64(kvp.Value, true));

                // Set width/height from first image
                if (request.screenshot.width == 0)
                {
                    request.screenshot.width = kvp.Value.width;
                    request.screenshot.height = kvp.Value.height;
                }
            }
        }

        /// <summary>
        /// Add multi-angle screenshots from byte arrays
        /// </summary>
        /// <param name="request">Bug report request</param>
        /// <param name="screenshots">Dictionary of angle name to PNG byte array</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        public static void AddMultiAngleScreenshots(
            FBX4VRMBugReportRequest request,
            Dictionary<string, byte[]> screenshots,
            int width,
            int height)
        {
            if (screenshots == null || screenshots.Count == 0) return;

            if (request.screenshot == null)
            {
                request.screenshot = new FBX4VRMScreenshot
                {
                    format = "PNG",
                    width = width,
                    height = height
                };
            }

            foreach (var kvp in screenshots)
            {
                if (kvp.Value == null || kvp.Value.Length == 0) continue;
                request.screenshot.angles.Add(kvp.Key);
                request.screenshot.base64_images.Add(Utils.JsonHelper.BytesToBase64(kvp.Value));
            }
        }

        /// <summary>
        /// Add an additional image to bug report
        /// </summary>
        /// <param name="request">Bug report request</param>
        /// <param name="texture">Image texture</param>
        /// <param name="filename">Original filename</param>
        /// <param name="description">Optional description</param>
        public static void AddAdditionalImage(
            FBX4VRMBugReportRequest request,
            Texture2D texture,
            string filename,
            string description = null)
        {
            if (texture == null) return;

            request.additional_images.Add(new FBX4VRMAdditionalImage
            {
                filename = filename,
                format = "PNG",
                base64 = Utils.JsonHelper.TextureToBase64(texture, true),
                description = description
            });
        }

        /// <summary>
        /// Add an additional image from byte array
        /// </summary>
        /// <param name="request">Bug report request</param>
        /// <param name="imageBytes">Image data as byte array</param>
        /// <param name="filename">Original filename</param>
        /// <param name="format">Image format (PNG, JPG, etc.)</param>
        /// <param name="description">Optional description</param>
        public static void AddAdditionalImage(
            FBX4VRMBugReportRequest request,
            byte[] imageBytes,
            string filename,
            string format = "PNG",
            string description = null)
        {
            if (imageBytes == null || imageBytes.Length == 0) return;

            request.additional_images.Add(new FBX4VRMAdditionalImage
            {
                filename = filename,
                format = format,
                base64 = Utils.JsonHelper.BytesToBase64(imageBytes),
                description = description
            });
        }

        #endregion
    }
}
