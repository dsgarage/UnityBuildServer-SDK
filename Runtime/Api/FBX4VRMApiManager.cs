using System;
using System.Collections;
using UnityEngine;
using DSGarage.UnityBuildServer.Models;
using DSGarage.UnityBuildServer.Utils;

namespace DSGarage.UnityBuildServer.Api
{
    /// <summary>
    /// MonoBehaviour wrapper for FBX4VRM API Client
    /// Use this component in your scene to interact with the API
    /// </summary>
    public class FBX4VRMApiManager : MonoBehaviour
    {
        [Header("Server Configuration")]
        [SerializeField]
        private string serverUrl = "https://153.126.176.139:8000";

        [SerializeField]
        private bool skipCertificateValidation = true;

        [Header("Debug")]
        [SerializeField]
        private bool verboseLogging = false;

        private FBX4VRMApiClient _client;

        /// <summary>
        /// API Client instance
        /// </summary>
        public FBX4VRMApiClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new FBX4VRMApiClient(serverUrl);
                    _client.VerboseLogging = verboseLogging;
                }
                return _client;
            }
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static FBX4VRMApiManager _instance;
        public static FBX4VRMApiManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FBX4VRMApiManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("FBX4VRMApiManager");
                        _instance = go.AddComponent<FBX4VRMApiManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Apply certificate settings
            CertificateConfig.SkipCertificateValidation = skipCertificateValidation;
        }

        /// <summary>
        /// Get avatar list
        /// </summary>
        public void GetAvatarList(
            Action<FBX4VRMAvatarListResponse> onSuccess,
            Action<string> onError = null)
        {
            StartCoroutine(Client.GetAvatarList(onSuccess, onError));
        }

        /// <summary>
        /// Submit bug report
        /// </summary>
        public void SubmitBugReport(
            FBX4VRMBugReportRequest request,
            Action<FBX4VRMBugReportResponse> onSuccess,
            Action<string> onError = null)
        {
            StartCoroutine(Client.SubmitBugReport(request, onSuccess, onError));
        }

        /// <summary>
        /// Submit a simple bug report
        /// </summary>
        public void SubmitSimpleBugReport(
            string modelName,
            bool success,
            string errorMessage = null,
            Action<FBX4VRMBugReportResponse> onSuccess = null,
            Action<string> onError = null)
        {
            var request = FBX4VRMApiClient.CreateBugReport(
                modelName,
                success,
                Application.version,
                Application.unityVersion,
                errorMessage
            );

            SubmitBugReport(request, onSuccess, onError);
        }

        /// <summary>
        /// Submit bug report with screenshot
        /// </summary>
        public void SubmitBugReportWithScreenshot(
            string modelName,
            bool success,
            string errorMessage,
            Action<FBX4VRMBugReportResponse> onSuccess = null,
            Action<string> onError = null)
        {
            StartCoroutine(SubmitWithScreenshotCoroutine(
                modelName, success, errorMessage, onSuccess, onError
            ));
        }

        private IEnumerator SubmitWithScreenshotCoroutine(
            string modelName,
            bool success,
            string errorMessage,
            Action<FBX4VRMBugReportResponse> onSuccess,
            Action<string> onError)
        {
            // Wait for end of frame to capture screenshot
            yield return new WaitForEndOfFrame();

            // Capture screenshot
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            // Create request
            var request = FBX4VRMApiClient.CreateBugReport(
                modelName,
                success,
                Application.version,
                Application.unityVersion,
                errorMessage
            );

            // Add screenshot
            FBX4VRMApiClient.AddScreenshot(request, screenshot);

            // Clean up
            Destroy(screenshot);

            // Submit
            yield return Client.SubmitBugReport(request, onSuccess, onError);
        }

        /// <summary>
        /// Check API connection
        /// </summary>
        public void CheckConnection(Action<bool, string> callback)
        {
            StartCoroutine(CheckConnectionCoroutine(callback));
        }

        private IEnumerator CheckConnectionCoroutine(Action<bool, string> callback)
        {
            yield return Client.GetApiInfo(response =>
            {
                if (response.Success && response.Data != null)
                {
                    callback?.Invoke(true, $"Connected to {response.Data.api} v{response.Data.version}");
                }
                else
                {
                    callback?.Invoke(false, response.Error ?? "Connection failed");
                }
            });
        }

#if UNITY_EDITOR
        [ContextMenu("Test Connection")]
        private void TestConnection()
        {
            CheckConnection((success, message) =>
            {
                if (success)
                {
                    Debug.Log($"[FBX4VRMApiManager] {message}");
                }
                else
                {
                    Debug.LogError($"[FBX4VRMApiManager] {message}");
                }
            });
        }

        [ContextMenu("Test Get Avatar List")]
        private void TestGetAvatarList()
        {
            GetAvatarList(
                response =>
                {
                    Debug.Log($"[FBX4VRMApiManager] Got {response.avatars.Count} avatars");
                    foreach (var avatar in response.avatars)
                    {
                        Debug.Log($"  - {avatar.name} (ID: {avatar.id}, Reports: {avatar.report_count})");
                    }
                },
                error =>
                {
                    Debug.LogError($"[FBX4VRMApiManager] Failed to get avatars: {error}");
                }
            );
        }
#endif
    }
}
