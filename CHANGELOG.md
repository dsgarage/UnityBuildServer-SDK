# Changelog

All notable changes to this project will be documented in this file.

## [0.0.7] - 2025-12-04

### Added
- **Multi-angle Screenshot Support**
  - `FBX4VRMScreenshot.base64_images` - List of base64 images for each angle
  - `AddMultiAngleScreenshots()` - Helper to add multiple angle screenshots
- **Additional Images Support**
  - `FBX4VRMAdditionalImage` - Model for user-attached images
  - `FBX4VRMBugReportRequest.additional_images` - List of additional images
  - `FBX4VRMBugReportRequest.additional_screenshots` - List of additional screenshots
  - `AddAdditionalImage()` - Helper to add user images
- **Static ServerUrl Accessor**
  - `FBX4VRMApiClient.GetServerUrl()` - Get current server URL for debugging

---

## [0.0.6] - 2025-12-03

### Added
- **Queue System Support** - New recommended method for submitting bug reports
  - `SubmitToQueue()` - Submit reports to background processing queue
  - `GetQueueStats()` - Check queue status (pending, processing, completed, failed)
  - `FBX4VRMQueueSubmitResponse` - Response model for queue submissions
  - `FBX4VRMQueueStatsResponse` - Response model for queue statistics

### Changed
- Direct submission (`SubmitBugReport()`) is now marked as legacy
- Updated README with Queue System documentation

---

## [0.0.5] - 2025-12-03

### Changed
- Moved to standalone UPM repository for easier distribution
- Updated repository URL to UnityBuildServer-SDK

### Fixed
- Default server URL uses HTTPS (port 8443) - compatible with Unity 2022+

## [0.0.4] - 2025-12-02

### Added
- FBX4VRM Bug Report API client
- Avatar list API
- Editor API client for Editor scripts
- Async support with coroutines

### Features
- Self-signed certificate support for development
- Automatic retry on failure
- Screenshot attachment support
