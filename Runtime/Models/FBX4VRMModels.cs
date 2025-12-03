using System;
using System.Collections.Generic;
using UnityEngine;

namespace DSGarage.UnityBuildServer.Models
{
    #region Environment & Settings

    /// <summary>
    /// Environment information for bug report
    /// </summary>
    [Serializable]
    public class FBX4VRMEnvironment
    {
        public string package_version;
        public string unity_version;
        public string platform;
        public string univrm_version;
        public string render_pipeline;
    }

    /// <summary>
    /// Export settings for FBX4VRM conversion
    /// </summary>
    [Serializable]
    public class FBX4VRMExportSettings
    {
        public int vrm_version;
        public string preset_name;
        public string output_path;
        public bool enable_liltoon_conversion;
        public bool enable_hdr_clamp;
        public bool enable_outline_conversion;
        public string transparent_mode;
        public bool enable_tpose_normalization;
        public bool enable_armature_rotation_bake;
        public bool enable_bone_orientation_normalization;
        public bool enable_expression_auto_mapping;
        public string expression_naming_convention;
        public bool enable_springbone_conversion;
        public bool enable_collider_conversion;
        public string output_folder;
        public string file_name_mode;
        public string custom_file_name;
    }

    #endregion

    #region VRM Metadata

    /// <summary>
    /// VRM 0.x metadata extracted from VRM file.
    /// These fields are defined in the VRM specification and embedded in VRM files.
    /// </summary>
    [Serializable]
    public class FBX4VRMVrmMeta
    {
        /// <summary>VRM title (required in VRM spec)</summary>
        public string title;

        /// <summary>VRM author (required in VRM spec)</summary>
        public string author;

        /// <summary>VRM version</summary>
        public string version;

        /// <summary>Author contact information</summary>
        public string contact_information;

        /// <summary>Reference URL (e.g., BOOTH URL)</summary>
        public string reference;
    }

    #endregion

    #region Source Model

    /// <summary>
    /// Source model information
    /// </summary>
    [Serializable]
    public class FBX4VRMSourceModel
    {
        public string name;
        public string asset_path;
        public string source_format;
        public long file_size_bytes;
        public string avatar_id;
        public bool is_new_avatar = true;

        /// <summary>
        /// Avatar package version (e.g., "5.3", "1.0.0").
        /// This is the version of the avatar/model itself, NOT the FBX4VRM tool version.
        /// </summary>
        public string package_version;

        /// <summary>
        /// VRM 0.x metadata (optional).
        /// If provided, server will use this for avatar identification.
        /// </summary>
        public FBX4VRMVrmMeta vrm_meta;
    }

    #endregion

    #region Conversion Result

    /// <summary>
    /// Conversion result information
    /// </summary>
    [Serializable]
    public class FBX4VRMResult
    {
        public bool success;
        public string stopped_at_processor;
        public string error_message;
        public int duration_ms;
    }

    #endregion

    #region Skeleton Analysis

    /// <summary>
    /// Bone count information
    /// </summary>
    [Serializable]
    public class FBX4VRMBoneInfo
    {
        public int found;
        public List<string> missing = new List<string>();
    }

    /// <summary>
    /// Armature rotation information
    /// </summary>
    [Serializable]
    public class FBX4VRMArmatureRotation
    {
        public float[] euler;
        public bool requires_normalization;
        public bool normalized;
    }

    /// <summary>
    /// Bone orientation issue
    /// </summary>
    [Serializable]
    public class FBX4VRMBoneOrientationIssue
    {
        public string bone;
        public float[] expected_forward;
        public float[] actual_forward;
        public float angle_diff_deg;
    }

    /// <summary>
    /// Bone orientations container
    /// </summary>
    [Serializable]
    public class FBX4VRMBoneOrientations
    {
        public List<FBX4VRMBoneOrientationIssue> issues = new List<FBX4VRMBoneOrientationIssue>();
    }

    /// <summary>
    /// Skeleton analysis information
    /// </summary>
    [Serializable]
    public class FBX4VRMSkeleton
    {
        public string avatar_name;
        public bool is_humanoid;
        public FBX4VRMBoneInfo required_bones;
        public FBX4VRMBoneInfo recommended_bones;
        public bool bone_hierarchy_valid;
        public bool t_pose_valid;
        public FBX4VRMArmatureRotation armature_rotation;
        public FBX4VRMBoneOrientations bone_orientations;
        public int total_bones;
    }

    #endregion

    #region Mesh Analysis

    /// <summary>
    /// Individual mesh information
    /// </summary>
    [Serializable]
    public class FBX4VRMMeshInfo
    {
        public string name;
        public int vertices;
        public int triangles;
        public int blendshapes;
        public int submeshes;
        public int material_slots;
    }

    /// <summary>
    /// Mesh analysis information
    /// </summary>
    [Serializable]
    public class FBX4VRMMeshes
    {
        public int skinned_mesh_count;
        public int mesh_filter_count;
        public int total_vertices;
        public int total_triangles;
        public int blendshape_count;
        public List<FBX4VRMMeshInfo> meshes = new List<FBX4VRMMeshInfo>();
    }

    #endregion

    #region Material Analysis

    /// <summary>
    /// Shader usage count
    /// </summary>
    [Serializable]
    public class FBX4VRMShaderCount
    {
        public string shader_name;
        public int count;
    }

    /// <summary>
    /// Material conversion warning
    /// </summary>
    [Serializable]
    public class FBX4VRMMaterialWarning
    {
        public string type;
        public string property;
        public string original_value;
        public string clamped_value;
    }

    /// <summary>
    /// Material conversion result
    /// </summary>
    [Serializable]
    public class FBX4VRMMaterialConversionResult
    {
        public string name;
        public string original_shader;
        public string target_shader;
        public bool success;
        public string error;
        public List<FBX4VRMMaterialWarning> warnings = new List<FBX4VRMMaterialWarning>();
    }

    /// <summary>
    /// Unsupported shader information
    /// </summary>
    [Serializable]
    public class FBX4VRMUnsupportedShader
    {
        public string name;
        public string shader;
        public string reason;
    }

    /// <summary>
    /// Material analysis information
    /// </summary>
    [Serializable]
    public class FBX4VRMMaterials
    {
        public int total_count;
        public List<FBX4VRMShaderCount> original_shaders_list = new List<FBX4VRMShaderCount>();
        public List<FBX4VRMMaterialConversionResult> conversion_results = new List<FBX4VRMMaterialConversionResult>();
        public List<FBX4VRMUnsupportedShader> unsupported_shaders = new List<FBX4VRMUnsupportedShader>();
    }

    #endregion

    #region Expression Analysis

    /// <summary>
    /// VRM expression mapping
    /// </summary>
    [Serializable]
    public class FBX4VRMExpressionMapping
    {
        public string vrm_expression;
        public string source;
        public string mesh;
    }

    /// <summary>
    /// Expression mapping conflict
    /// </summary>
    [Serializable]
    public class FBX4VRMExpressionConflict
    {
        public string vrm_expression;
        public List<string> candidates = new List<string>();
        public string selected;
        public string reason;
    }

    /// <summary>
    /// Expression analysis information
    /// </summary>
    [Serializable]
    public class FBX4VRMExpressions
    {
        public int total_blendshapes;
        public int mapped_count;
        public int unmapped_count;
        public List<FBX4VRMExpressionMapping> mappings = new List<FBX4VRMExpressionMapping>();
        public List<FBX4VRMExpressionConflict> conflicts = new List<FBX4VRMExpressionConflict>();
        public List<string> missing_recommended = new List<string>();
    }

    #endregion

    #region Dynamics Analysis

    /// <summary>
    /// Dynamics conversion result
    /// </summary>
    [Serializable]
    public class FBX4VRMDynamicsConversionResult
    {
        public string name;
        public string source_type;
        public bool success;
        public string error;
    }

    /// <summary>
    /// Physics dynamics information
    /// </summary>
    [Serializable]
    public class FBX4VRMDynamics
    {
        public string source_type;
        public int vrm_springbone_count;
        public int vrchat_physbone_count;
        public int dynamicbone_count;
        public int collider_count;
        public List<FBX4VRMDynamicsConversionResult> conversion_results = new List<FBX4VRMDynamicsConversionResult>();
    }

    #endregion

    #region Notifications

    /// <summary>
    /// Notification summary
    /// </summary>
    [Serializable]
    public class FBX4VRMNotificationSummary
    {
        public int info;
        public int warning;
        public int error;
    }

    /// <summary>
    /// Individual notification
    /// </summary>
    [Serializable]
    public class FBX4VRMNotificationItem
    {
        public string processor_id;
        public string message;
        public string details;
        public string timestamp;
    }

    /// <summary>
    /// Notification information
    /// </summary>
    [Serializable]
    public class FBX4VRMNotifications
    {
        public FBX4VRMNotificationSummary summary;
        public List<FBX4VRMNotificationItem> errors = new List<FBX4VRMNotificationItem>();
        public List<FBX4VRMNotificationItem> warnings = new List<FBX4VRMNotificationItem>();
    }

    #endregion

    #region Screenshot

    /// <summary>
    /// Screenshot information with multi-angle support
    /// </summary>
    [Serializable]
    public class FBX4VRMScreenshot
    {
        public string format = "PNG";
        public int width;
        public int height;

        /// <summary>Angle names for multi-angle screenshots (e.g., "front", "back", "left", "right")</summary>
        public List<string> angles = new List<string>();

        /// <summary>Single image base64 (for backward compatibility)</summary>
        public string base64;

        /// <summary>Multiple images base64 - one per angle (for multi-angle screenshots)</summary>
        public List<string> base64_images = new List<string>();
    }

    /// <summary>
    /// Additional image attached by user (e.g., reference images, error screenshots)
    /// </summary>
    [Serializable]
    public class FBX4VRMAdditionalImage
    {
        /// <summary>Original filename</summary>
        public string filename;

        /// <summary>Image format (PNG, JPG, etc.)</summary>
        public string format;

        /// <summary>Base64 encoded image data</summary>
        public string base64;

        /// <summary>Optional description</summary>
        public string description;
    }

    #endregion

    #region Bug Report Request/Response

    /// <summary>
    /// Bug report submission request
    /// </summary>
    [Serializable]
    public class FBX4VRMBugReportRequest
    {
        public string report_id;
        public string timestamp;
        public string platform = "fbx4vrm";

        public FBX4VRMEnvironment environment;
        public FBX4VRMExportSettings export_settings;
        public FBX4VRMSourceModel source_model;
        public FBX4VRMResult result;

        public FBX4VRMSkeleton skeleton;
        public FBX4VRMMeshes meshes;
        public FBX4VRMMaterials materials;
        public FBX4VRMExpressions expressions;
        public FBX4VRMDynamics dynamics;
        public FBX4VRMNotifications notifications;

        /// <summary>Primary screenshot (single or grid image)</summary>
        public FBX4VRMScreenshot screenshot;

        /// <summary>Additional screenshots from different angles</summary>
        public List<FBX4VRMScreenshot> additional_screenshots = new List<FBX4VRMScreenshot>();

        /// <summary>Additional images attached by user (reference images, error screenshots, etc.)</summary>
        public List<FBX4VRMAdditionalImage> additional_images = new List<FBX4VRMAdditionalImage>();

        /// <summary>User comment or notes</summary>
        public string user_comment;

        /// <summary>
        /// Generate a unique report ID
        /// </summary>
        public static string GenerateReportId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        /// <summary>
        /// Get current timestamp in ISO 8601 format
        /// </summary>
        public static string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }

    /// <summary>
    /// Bug report submission response
    /// </summary>
    [Serializable]
    public class FBX4VRMBugReportResponse
    {
        public string status;
        public string report_id;
        public string avatar_id;
        public string avatar_name;
        public string tracking_url;
        public string github_issue_url;
        public bool is_new_issue;

        public bool IsSuccess => status == "accepted";
    }

    #endregion

    #region Avatar List

    /// <summary>
    /// Avatar information returned from the server API.
    /// Contains all metadata about an avatar including version info and Issue tracking.
    /// </summary>
    [Serializable]
    public class FBX4VRMAvatarInfo
    {
        /// <summary>Avatar ID (unique identifier)</summary>
        public string id;

        /// <summary>Internal name (matches FBX/VRM filename)</summary>
        public string name;

        /// <summary>Display name for UI (e.g., Japanese name like "響狐リク")</summary>
        public string display_name;

        /// <summary>Avatar package version (e.g., "1.0.0", "2.01")</summary>
        public string package_version;

        /// <summary>BOOTH sales page URL</summary>
        public string booth_url;

        /// <summary>Avatar number for Issue tracking</summary>
        public int avatar_number;

        /// <summary>Version index within avatar (for child issues)</summary>
        public int version_index;

        /// <summary>GitHub Issue number (child issue for this version)</summary>
        public string issue_number;

        /// <summary>GitHub Issue URL (child issue for this version)</summary>
        public string github_issue_url;

        /// <summary>Parent GitHub Issue number (for the avatar itself)</summary>
        public string parent_issue_number;

        /// <summary>Parent GitHub Issue URL (for the avatar itself)</summary>
        public string parent_issue_url;

        /// <summary>Number of bug reports for this avatar version</summary>
        public int report_count;

        /// <summary>Last reported timestamp (ISO 8601 format)</summary>
        public string last_reported;

        /// <summary>Platforms this avatar has been reported on (fbx4vrm, vrmloader, arapp)</summary>
        public List<string> platforms = new List<string>();

        /// <summary>Whether the last conversion test was successful</summary>
        public bool? result_success;

        /// <summary>Unique avatar identifier (booth:{id} or vrm:{hash})</summary>
        public string avatar_uid;

        /// <summary>Source type (legacy, booth, vrm, custom)</summary>
        public string source_type;

        /// <summary>VRM author name</summary>
        public string author;

        /// <summary>Reference URL from VRM metadata</summary>
        public string reference_url;

        /// <summary>
        /// Get the display name, falling back to internal name if not set
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(display_name) ? name : display_name;
        }
    }

    /// <summary>
    /// Avatar list response
    /// </summary>
    [Serializable]
    public class FBX4VRMAvatarListResponse
    {
        public bool success;
        public List<FBX4VRMAvatarInfo> avatars = new List<FBX4VRMAvatarInfo>();
    }

    #endregion

    #region API Info

    /// <summary>
    /// API information response
    /// </summary>
    [Serializable]
    public class FBX4VRMApiInfoResponse
    {
        public string api;
        public string version;
        public string documentation;
    }

    #endregion

    #region Queue System

    /// <summary>
    /// Queue submission response
    /// </summary>
    [Serializable]
    public class FBX4VRMQueueSubmitResponse
    {
        public string status;
        public string queue_id;
        public string message;

        public bool IsSuccess => status == "queued";
    }

    /// <summary>
    /// Queue statistics response
    /// </summary>
    [Serializable]
    public class FBX4VRMQueueStatsResponse
    {
        public int pending;
        public int processing;
        public int completed;
        public int failed;
    }

    #endregion
}
