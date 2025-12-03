using UnityEngine.Networking;

namespace DSGarage.UnityBuildServer.Utils
{
    /// <summary>
    /// Certificate handler that accepts all certificates including self-signed ones.
    /// Use only in development/testing environments.
    /// </summary>
    public class AcceptAllCertificatesHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Accept all certificates (including self-signed)
            // WARNING: This is a security risk in production
            return true;
        }
    }

    /// <summary>
    /// Certificate handler configuration
    /// </summary>
    public static class CertificateConfig
    {
        /// <summary>
        /// Whether to skip certificate validation (for self-signed certificates)
        /// Default: true in Editor, false in builds
        /// </summary>
        public static bool SkipCertificateValidation { get; set; } =
#if UNITY_EDITOR
            true;
#else
            false;
#endif

        /// <summary>
        /// Apply certificate handler to UnityWebRequest if needed
        /// </summary>
        public static void ApplyCertificateHandler(UnityWebRequest request)
        {
            if (SkipCertificateValidation)
            {
                request.certificateHandler = new AcceptAllCertificatesHandler();
            }
        }
    }
}
