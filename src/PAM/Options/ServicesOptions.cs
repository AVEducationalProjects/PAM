namespace PAM.Options
{
    public class ServicesOptions 
    {
        public ServicesOptions()
        {
            UserService = "http://localhost:8001";
        }

        public string UserService { get; set; }
    }
}
