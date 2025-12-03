using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using DSGarage.UnityBuildServer.Utils;

namespace DSGarage.UnityBuildServer.Api
{
    /// <summary>
    /// API response wrapper
    /// </summary>
    /// <typeparam name="T">Response data type</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public long ResponseCode { get; set; }
        public string RawResponse { get; set; }

        public static ApiResponse<T> CreateSuccess(T data, long responseCode, string rawResponse)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                ResponseCode = responseCode,
                RawResponse = rawResponse
            };
        }

        public static ApiResponse<T> CreateError(string error, long responseCode = 0, string rawResponse = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = error,
                ResponseCode = responseCode,
                RawResponse = rawResponse
            };
        }
    }

    /// <summary>
    /// Base class for API clients.
    /// Provides common HTTP request functionality for derived API clients.
    /// </summary>
    public abstract class ApiClientBase
    {
        protected string BaseUrl { get; private set; }
        protected int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Gets the server URL that this client connects to
        /// </summary>
        public string ServerUrl => BaseUrl;

        /// <summary>
        /// Enable verbose logging for debugging API requests and responses
        /// </summary>
        public bool VerboseLogging { get; set; } = false;

        protected ApiClientBase(string baseUrl)
        {
            BaseUrl = baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Set the base URL
        /// </summary>
        public void SetBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Perform GET request
        /// </summary>
        protected IEnumerator GetAsync<T>(string endpoint, Action<ApiResponse<T>> callback)
        {
            string url = $"{BaseUrl}{endpoint}";
            LogVerbose($"GET {url}");

            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = TimeoutSeconds;
                CertificateConfig.ApplyCertificateHandler(request);

                yield return request.SendWebRequest();

                var response = ProcessResponse<T>(request);
                callback?.Invoke(response);
            }
        }

        /// <summary>
        /// Perform POST request with JSON body
        /// </summary>
        protected IEnumerator PostJsonAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data,
            Action<ApiResponse<TResponse>> callback)
        {
            string url = $"{BaseUrl}{endpoint}";
            string jsonBody = JsonHelper.ToJson(data);

            LogVerbose($"POST {url}");
            LogVerbose($"Request body: {jsonBody}");

            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = TimeoutSeconds;
                CertificateConfig.ApplyCertificateHandler(request);

                yield return request.SendWebRequest();

                var response = ProcessResponse<TResponse>(request);
                callback?.Invoke(response);
            }
        }

        /// <summary>
        /// Process UnityWebRequest response
        /// </summary>
        protected ApiResponse<T> ProcessResponse<T>(UnityWebRequest request)
        {
            string responseText = request.downloadHandler?.text;
            long responseCode = request.responseCode;

            LogVerbose($"Response code: {responseCode}");
            LogVerbose($"Response body: {responseText}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (JsonHelper.TryFromJson<T>(responseText, out T data))
                {
                    return ApiResponse<T>.CreateSuccess(data, responseCode, responseText);
                }
                else
                {
                    return ApiResponse<T>.CreateError(
                        $"Failed to parse response JSON",
                        responseCode,
                        responseText
                    );
                }
            }
            else
            {
                string error = request.error;
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    error = $"Connection error: {request.error}";
                }
                else if (request.result == UnityWebRequest.Result.ProtocolError)
                {
                    error = $"HTTP error {responseCode}: {request.error}";
                }

                LogError($"Request failed: {error}");
                return ApiResponse<T>.CreateError(error, responseCode, responseText);
            }
        }

        protected void LogVerbose(string message)
        {
            if (VerboseLogging)
            {
                Debug.Log($"[{GetType().Name}] {message}");
            }
        }

        protected void LogError(string message)
        {
            Debug.LogError($"[{GetType().Name}] {message}");
        }

        protected void LogWarning(string message)
        {
            Debug.LogWarning($"[{GetType().Name}] {message}");
        }
    }
}
