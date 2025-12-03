# DSGarage Unity Build Server SDK

Unity SDK for DSGarage Unity Build Server API.

## Features

- FBX4VRM Bug Report API client
- Runtime & Editor support
- Self-signed certificate support
- Easy-to-use API

## Installation

### Via Git URL (Package Manager)

1. Open Package Manager (Window > Package Manager)
2. Click "+" > "Add package from git URL..."
3. Enter: `https://github.com/dsgarage/UnityServer4Docker.git?path=UnitySDK`

### Manual Installation

Copy the `UnitySDK` folder to your project's `Packages` directory.

## Quick Start

### Runtime Usage

```csharp
using DSGarage.UnityBuildServer.Api;
using DSGarage.UnityBuildServer.Models;

// Get avatar list
FBX4VRMApiManager.Instance.GetAvatarList(
    response => {
        foreach (var avatar in response.avatars) {
            Debug.Log($"Avatar: {avatar.name}");
        }
    },
    error => Debug.LogError(error)
);

// Submit simple bug report
FBX4VRMApiManager.Instance.SubmitSimpleBugReport(
    "MyModel",
    success: true,
    errorMessage: null,
    onSuccess: response => Debug.Log($"Submitted: {response.github_issue_url}"),
    onError: error => Debug.LogError(error)
);

// Submit bug report with screenshot
FBX4VRMApiManager.Instance.SubmitBugReportWithScreenshot(
    "MyModel",
    success: false,
    errorMessage: "Conversion failed at MaterialProcessor",
    onSuccess: response => Debug.Log("Submitted with screenshot!"),
    onError: error => Debug.LogError(error)
);
```

### Editor Usage

```csharp
#if UNITY_EDITOR
using DSGarage.UnityBuildServer.Editor;
using DSGarage.UnityBuildServer.Models;

// Get avatar list
FBX4VRMEditorApiClient.GetAvatarListAsync(
    response => Debug.Log($"Found {response.avatars.Count} avatars"),
    error => Debug.LogError(error)
);

// Submit bug report
var request = FBX4VRMApiClient.CreateBugReport(
    "MyModel",
    success: true,
    packageVersion: "1.0.0",
    unityVersion: Application.unityVersion
);

FBX4VRMEditorApiClient.SubmitBugReportAsync(
    request,
    response => Debug.Log($"Submitted: {response.report_id}"),
    error => Debug.LogError(error)
);

// Test connection
FBX4VRMEditorApiClient.TestConnection((success, message) => {
    Debug.Log(message);
});
#endif
```

### Detailed Bug Report

```csharp
var request = new FBX4VRMBugReportRequest
{
    report_id = FBX4VRMBugReportRequest.GenerateReportId(),
    timestamp = FBX4VRMBugReportRequest.GetTimestamp(),
    platform = "fbx4vrm",

    environment = new FBX4VRMEnvironment
    {
        package_version = "1.0.0",
        unity_version = Application.unityVersion,
        univrm_version = "0.127.2"
    },

    source_model = new FBX4VRMSourceModel
    {
        name = "MyAvatar",
        is_new_avatar = true
    },

    result = new FBX4VRMResult
    {
        success = false,
        stopped_at_processor = "MaterialProcessor",
        error_message = "Unsupported shader: Custom/MyShader"
    },

    skeleton = new FBX4VRMSkeleton
    {
        is_humanoid = true,
        total_bones = 55
    },

    user_comment = "Additional notes..."
};

// Add screenshot
FBX4VRMApiClient.AddScreenshot(request, screenshotTexture);

// Submit
FBX4VRMApiManager.Instance.SubmitBugReport(request,
    onSuccess: response => { },
    onError: error => { }
);
```

## Configuration

### Server URL

```csharp
// Runtime
FBX4VRMApiClient.Configure("https://your-server.com:8443");

// Editor
FBX4VRMEditorApiClient.ServerUrl = "https://your-server.com:8443";
```

### Certificate Validation

For self-signed certificates:

```csharp
using DSGarage.UnityBuildServer.Utils;

// Enable/disable certificate validation skip
CertificateConfig.SkipCertificateValidation = true;
```

### Verbose Logging

```csharp
// Runtime
FBX4VRMApiManager.Instance.Client.VerboseLogging = true;

// Editor
FBX4VRMEditorApiClient.VerboseLogging = true;
```

## API Reference

### FBX4VRMApiManager (MonoBehaviour)

| Method | Description |
|--------|-------------|
| `GetAvatarList()` | Get list of avatars with bug reports |
| `SubmitBugReport()` | Submit detailed bug report |
| `SubmitSimpleBugReport()` | Submit simple bug report |
| `SubmitBugReportWithScreenshot()` | Submit with auto screenshot |
| `CheckConnection()` | Test API connection |

### FBX4VRMApiClient (Runtime)

| Method | Description |
|--------|-------------|
| `GetApiInfo()` | Get API information |
| `GetAvatarList()` | Get avatar list |
| `SubmitBugReport()` | Submit bug report |
| `CreateBugReport()` | Create bug report request |
| `AddScreenshot()` | Add screenshot to request |

### FBX4VRMEditorApiClient (Editor)

| Method | Description |
|--------|-------------|
| `GetAvatarListAsync()` | Get avatar list asynchronously |
| `SubmitBugReportAsync()` | Submit bug report asynchronously |
| `SubmitSimpleBugReport()` | Submit simple bug report |
| `CaptureGameView()` | Capture Game View screenshot |
| `CaptureSceneView()` | Capture Scene View screenshot |
| `TestConnection()` | Test API connection |

## DLL Build

To build as DLL for distribution:

1. Open the project in Visual Studio or Rider
2. Build the solution targeting .NET Standard 2.1
3. Copy the resulting DLLs:
   - `DSGarage.UnityBuildServer.Runtime.dll`
   - `DSGarage.UnityBuildServer.Editor.dll`

## License

Proprietary - DSGarage
