using System;
using UnityEngine;

namespace DSGarage.UnityBuildServer.Utils
{
    /// <summary>
    /// JSON serialization helper for Unity
    /// Wraps JsonUtility with additional features
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Serialize object to JSON string
        /// </summary>
        public static string ToJson<T>(T obj, bool prettyPrint = false)
        {
            if (obj == null) return "null";
            return JsonUtility.ToJson(obj, prettyPrint);
        }

        /// <summary>
        /// Deserialize JSON string to object
        /// </summary>
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Try to deserialize JSON string to object
        /// </summary>
        public static bool TryFromJson<T>(string json, out T result)
        {
            result = default;
            if (string.IsNullOrEmpty(json)) return false;

            try
            {
                result = JsonUtility.FromJson<T>(json);
                return result != null;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[JsonHelper] Failed to parse JSON: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Convert Texture2D to Base64 string
        /// </summary>
        public static string TextureToBase64(Texture2D texture, bool isPng = true)
        {
            if (texture == null) return null;

            byte[] bytes = isPng ? texture.EncodeToPNG() : texture.EncodeToJPG();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert byte array to Base64 string
        /// </summary>
        public static string BytesToBase64(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            return Convert.ToBase64String(bytes);
        }
    }
}
