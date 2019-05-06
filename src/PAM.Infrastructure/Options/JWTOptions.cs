namespace PAM.Infrastructure.Options
{
    public class JWTOptions
    {
        public string SigningCertificate { get; set; }
        public string EncryptionPassword { get; set; }
        public string EncryptionCertificate { get; set; }
    }
}
