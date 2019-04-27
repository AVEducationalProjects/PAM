namespace PAM.AssetService.Options
{
    public class JWTOptions
    {
        public string SigningPassword { get; set; }
        public string SigningCertificate { get; set; }

        public string EncryptionPassword { get; set; }
        public string EncryptionCertificate { get; set; }
    }
}
