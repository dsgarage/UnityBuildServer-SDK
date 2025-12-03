using System;
using System.Collections;
using DSGarage.UnityBuildServer.Models;

namespace DSGarage.UnityBuildServer.Api
{
    /// <summary>
    /// Interface for FBX4VRM API Client
    ///
    /// <para>
    /// This interface allows for easy mocking in unit tests and provides
    /// a clear contract for API operations.
    /// </para>
    ///
    /// <example>
    /// <code>
    /// // Basic usage with the default singleton instance
    /// public class MyComponent : MonoBehaviour
    /// {
    ///     private IFBX4VRMApiClient _apiClient;
    ///
    ///     void Start()
    ///     {
    ///         _apiClient = FBX4VRMApiClient.Instance;
    ///     }
    ///
    ///     public void SubmitReport()
    ///     {
    ///         var request = FBX4VRMApiClient.CreateBugReport(
    ///             modelName: "MyAvatar",
    ///             success: true,
    ///             packageVersion: "1.0.0",
    ///             unityVersion: Application.unityVersion
    ///         );
    ///         StartCoroutine(_apiClient.SubmitBugReport(request, OnSuccess, OnError));
    ///     }
    ///
    ///     void OnSuccess(FBX4VRMBugReportResponse response)
    ///     {
    ///         Debug.Log($"Report submitted: {response.report_id}");
    ///     }
    ///
    ///     void OnError(string error)
    ///     {
    ///         Debug.LogError($"Failed: {error}");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface IFBX4VRMApiClient
    {
        /// <summary>
        /// Server URL that this client connects to
        /// </summary>
        string ServerUrl { get; }

        /// <summary>
        /// Get API information
        /// </summary>
        /// <param name="callback">Callback with API response</param>
        /// <returns>Coroutine enumerator</returns>
        IEnumerator GetApiInfo(Action<ApiResponse<FBX4VRMApiInfoResponse>> callback);

        /// <summary>
        /// Get list of avatars with bug reports
        /// </summary>
        /// <param name="callback">Callback with API response</param>
        /// <returns>Coroutine enumerator</returns>
        IEnumerator GetAvatarList(Action<ApiResponse<FBX4VRMAvatarListResponse>> callback);

        /// <summary>
        /// Get avatar list (simplified callback)
        /// </summary>
        /// <param name="onSuccess">Called on success with avatar list</param>
        /// <param name="onError">Called on error with error message</param>
        /// <returns>Coroutine enumerator</returns>
        IEnumerator GetAvatarList(
            Action<FBX4VRMAvatarListResponse> onSuccess,
            Action<string> onError = null);

        /// <summary>
        /// Submit a bug report
        /// </summary>
        /// <param name="request">Bug report request data</param>
        /// <param name="callback">Callback with API response</param>
        /// <returns>Coroutine enumerator</returns>
        IEnumerator SubmitBugReport(
            FBX4VRMBugReportRequest request,
            Action<ApiResponse<FBX4VRMBugReportResponse>> callback);

        /// <summary>
        /// Submit a bug report (simplified callback)
        /// </summary>
        /// <param name="request">Bug report request data</param>
        /// <param name="onSuccess">Called on success with response</param>
        /// <param name="onError">Called on error with error message</param>
        /// <returns>Coroutine enumerator</returns>
        IEnumerator SubmitBugReport(
            FBX4VRMBugReportRequest request,
            Action<FBX4VRMBugReportResponse> onSuccess,
            Action<string> onError = null);
    }
}
