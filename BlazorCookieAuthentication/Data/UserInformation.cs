namespace BlazorCookieAuthentication.Data
{
    public enum Roles
    {
        Anonymous,
        Standard,
        Administrator
    }

    public class UserInformation
    {
        public int ID { get; set; } = -1;

        public string UserName { get; set; } = "";

        public string UserPassword { get; set; } = "";

        public Roles UserRole { get; set; } = Roles.Anonymous;

        public string SessionKey { get; set; } = "";
    }
}
