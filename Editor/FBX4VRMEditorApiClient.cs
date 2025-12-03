#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using DSGarage.UnityBuildServer.Api;
using DSGarage.UnityBuildServer.Models;
using DSGarage.UnityBuildServer.Utils;

namespace DSGarage.UnityBuildServer.Editor
{
    /// <summary>
    /// Editor-specific FBX4VRM API Client
    /// Provides synchronous-like API calls using EditorCoroutine
    /// </summary>
    public static class FBX4VRMEditorApiClient
    {
        private static string _serverUrl = "https://153.126.176.139:8443";
        private static FBX4VRMApiClient _client;

        /// <summary>
        /// Current server URL
        /// </summary>
        public static string ServerUrl
        {
            get => _serverUrl;
            set
            {
                _serverUrl = value;
                _client = null;
            }
        }

        /// <summary>
        /// API Client instance
        /// </summary>
        public static FBX4VRMApiClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new FBX4VRMApiClient(_serverUrl);
                    _client.VerboseLogging = EditorPrefs.GetBool("FBX4VRM_VerboseLogging", false);
                }
                return _client;
            }
        }

        /// <summary>
        /// Enable/disable verbose logging
        /// </summary>
        public static bool VerboseLogging
        {
            get => EditorPrefs.GetBool("FBX4VRM_VerboseLogging", false);
            set
            {
                EditorPrefs.SetBool("FBX4VRM_VerboseLogging", value);
                if (_client != null)
                {
                    _client.VerboseLogging = value;
                }
            }
        }

        static FBX4VRMEditorApiClient()
        {
            // Ensure certificate validation is skipped in editor
            CertificateConfig.SkipCertificateValidation = true;
        }

        #region Avatar List

        /// <summary>
        /// Get avatar list asynchronously
        /// </summary>
        public static void GetAvatarListAsync(
            Action<FBX4VRMAvatarListResponse> onSuccess,
            Action<string> onError = null)
        {
            EditorCoroutineRunner.StartCoroutine(Client.GetAvatarList(onSuccess, onError));
        }

        #endregion

        #region Bug Report Submission

        /// <summary>
        /// Submit bug report asynchronously
        /// </summary>
        public static void SubmitBugReportAsync(
            FBX4VRMBugReportRequest request,
            Action<FBX4VRMBugReportResponse> onSuccess,
            Action<string> onError = null)
        {
            EditorCoroutineRunner.StartCoroutine(Client.SubmitBugReport(request, onSuccess, onError));
        }

        /// <summary>
        /// Submit a simple bug report
        /// </summary>
        public static void SubmitSimpleBugReport(
            string modelName,
            bool success,
            string errorMessage = null,
            Action<FBX4VRMBugReportResponse> onSuccess = null,
            Action<string> onError = null)
        {
            var request = FBX4VRMApiClient.CreateBugReport(
                modelName,
                success,
                GetPackageVersion(),
                Application.unityVersion,
                errorMessage
            );

            SubmitBugReportAsync(request, onSuccess, onError);
        }

        #endregion

        #region Screenshot Capture

        /// <summary>
        /// Capture Game View screenshot
        /// </summary>
        public static Texture2D CaptureGameView()
        {
            try
            {
                var gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
                if (gameView != null)
                {
                    var rect = gameView.position;
                    int width = (int)rect.width;
                    int height = (int)rect.height;

                    var screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
                    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    screenshot.Apply();
                    return screenshot;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FBX4VRMEditorApiClient] Failed to capture Game View: {e.Message}");
            }
            return null;
        }

        /// <summary>
        /// Capture Scene View screenshot
        /// </summary>
        public static Texture2D CaptureSceneView()
        {
            try
            {
                var sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null && sceneView.camera != null)
                {
                    var camera = sceneView.camera;
                    int width = (int)sceneView.position.width;
                    int height = (int)sceneView.position.height;

                    var rt = new RenderTexture(width, height, 24);
                    camera.targetTexture = rt;
                    camera.Render();

                    RenderTexture.active = rt;
                    var screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
                    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    screenshot.Apply();

                    camera.targetTexture = null;
                    RenderTexture.active = null;
                    UnityEngine.Object.DestroyImmediate(rt);

                    return screenshot;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FBX4VRMEditorApiClient] Failed to capture Scene View: {e.Message}");
            }
            return null;
        }

        #endregion

        #region Connection Test

        /// <summary>
        /// Test API connection
        /// </summary>
        public static void TestConnection(Action<bool, string> callback)
        {
            EditorCoroutineRunner.StartCoroutine(TestConnectionCoroutine(callback));
        }

        private static IEnumerator TestConnectionCoroutine(Action<bool, string> callback)
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

        #endregion

        #region Utility

        /// <summary>
        /// Get FBX4VRM package version from package.json
        /// </summary>
        public static string GetPackageVersion()
        {
            // Try to find package.json
            var guids = AssetDatabase.FindAssets("package t:TextAsset");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("fbx4vrm") || path.Contains("FBX4VRM"))
                {
                    var json = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    if (json != null && json.text.Contains("\"version\""))
                    {
                        // Simple version extraction
                        var text = json.text;
                        int versionIndex = text.IndexOf("\"version\"");
                        if (versionIndex >= 0)
                        {
                            int colonIndex = text.IndexOf(":", versionIndex);
                            int quoteStart = text.IndexOf("\"", colonIndex + 1);
                            int quoteEnd = text.IndexOf("\"", quoteStart + 1);
                            if (quoteStart >= 0 && quoteEnd > quoteStart)
                            {
                                return text.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
                            }
                        }
                    }
                }
            }
            return "unknown";
        }

        #endregion
    }

    /// <summary>
    /// Editor coroutine runner for executing coroutines in editor mode
    /// </summary>
    public static class EditorCoroutineRunner
    {
        private static EditorCoroutineExecutor _executor;

        public static void StartCoroutine(IEnumerator coroutine)
        {
            if (_executor == null)
            {
                _executor = new EditorCoroutineExecutor();
            }
            _executor.StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Executes coroutines in editor using EditorApplication.update
    /// </summary>
    public class EditorCoroutineExecutor
    {
        private System.Collections.Generic.List<IEnumerator> _coroutines = new System.Collections.Generic.List<IEnumerator>();

        public EditorCoroutineExecutor()
        {
            EditorApplication.update += Update;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            _coroutines.Add(coroutine);
        }

        private void Update()
        {
            for (int i = _coroutines.Count - 1; i >= 0; i--)
            {
                var coroutine = _coroutines[i];
                bool hasNext = false;

                try
                {
                    // Handle nested coroutines and AsyncOperation
                    if (coroutine.Current is IEnumerator nested)
                    {
                        if (nested.MoveNext())
                        {
                            hasNext = true;
                        }
                        else
                        {
                            hasNext = coroutine.MoveNext();
                        }
                    }
                    else if (coroutine.Current is AsyncOperation asyncOp)
                    {
                        if (!asyncOp.isDone)
                        {
                            hasNext = true;
                        }
                        else
                        {
                            hasNext = coroutine.MoveNext();
                        }
                    }
                    else
                    {
                        hasNext = coroutine.MoveNext();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EditorCoroutineExecutor] Coroutine error: {e}");
                    hasNext = false;
                }

                if (!hasNext)
                {
                    _coroutines.RemoveAt(i);
                }
            }
        }
    }
}
#endif
