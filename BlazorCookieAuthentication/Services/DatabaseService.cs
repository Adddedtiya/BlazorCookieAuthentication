using BlazorCookieAuthentication.Data;

namespace BlazorCookieAuthentication.Services
{
    public interface IDatabaseInterface
    {
        public Task<UserInformation> UserInfoFromUsername(string username);

        public Task<UserInformation> UserInfoFromID(int id);
    }

    public class FakeDatabase : IDatabaseInterface
    {

        public FakeDatabase()
        {
            
        }

        public Task<UserInformation> UserInfoFromID(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserInformation> UserInfoFromUsername(string username)
        {
            await Task.Run(() => { Console.WriteLine(username); });
            return new UserInformation() { ID = 2, UserName = "Hello", UserRole = Roles.Administrator };
        }
    }

}
