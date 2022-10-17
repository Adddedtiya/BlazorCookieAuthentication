using BlazorCookieAuthentication.Data;

namespace BlazorCookieAuthentication.Services
{
    public interface IDatabaseInterface
    {
        public Task<UserInformation> UserInfoFromUsername(string username);

        public Task<UserInformation> UserInfoFromID(int id);

        public Task<bool> UpdateUserSessionKey(int id, string sessionKey);
    }

    public class FakeDatabase : IDatabaseInterface
    {

        private readonly List<UserInformation> FakeUsersList = new();

        public async Task<UserInformation> UserInfoFromID(int id)
        {
            UserInformation user = new();
            await Task.Run(() => {
                foreach (var usr in FakeUsersList)
                {
                    if (usr.ID == id) { user = usr; break; }
                }
            });
            return user;
        }

        public async Task<UserInformation> UserInfoFromUsername(string username)
        {
            UserInformation user = new();
            await Task.Run(() => {
                foreach (var usr in FakeUsersList)
                {
                    if (usr.UserName == username) { user = usr; break; }
                }
            });
            return user;
        }

        public async Task<bool> UpdateUserSessionKey(int id, string sessionKey)
        {
            bool updated = false;
            await Task.Run(() => {
                for (int i = 0; i < FakeUsersList.Count; i++)
                {
                    if (FakeUsersList[i].ID == id)
                    {
                        FakeUsersList[i].SessionKey = sessionKey;
                        updated = true;
                        break;
                    }
                }
            });
            return updated;
        }

        public FakeDatabase()
        {
            FakeUsersList.Add(new UserInformation() {
                ID = 1,
                UserName = "Admin1",
                UserPassword = "10000.SHA512.J81TXlqzaBz26PhCyHvbQQ==.HqjP6Cxw/7PKwCkLi+TjtycTUjLWjsnaR6sSRUvUP6Y=", // pswd1
                UserRole = Roles.Administrator
            });

            FakeUsersList.Add(new UserInformation()
            {
                ID = 2,
                UserName = "User1",
                UserPassword = "10000.SHA512.Qok6ceMlnKMnVYg4cGyIFg==.QbC0HcxG6iZYOdojwxltYV8BhdRJ8LyiyKmy61BvdTs=", // pswd2
                UserRole = Roles.Standard
            });
        }

    }

}
