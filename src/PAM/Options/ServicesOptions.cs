namespace PAM.Options
{
    public class ServicesOptions 
    {
        public ServicesOptions()
        {
            UserService = "http://localhost:8001";
            AssetService = "http://localhost:8002";
        }

        public string UserService { get; set; }

        public string AssetService { get; set; }
    }
}
