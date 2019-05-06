namespace PAM.Infrastructure.Options
{
    public class JWTSigninOptions
    {
        public string SigningPassword { get; set; }
        public string SigningCertificate { get; set; }
        public string EncryptionCertificate { get; set; }
    }
}
