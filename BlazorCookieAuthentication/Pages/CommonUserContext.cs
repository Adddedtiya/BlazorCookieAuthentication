using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Authorization;

using BlazorCookieAuthentication.Data;
using BlazorCookieAuthentication.Services;

namespace BlazorCookieAuthentication.Pages
{
    public class CommonUserContext : ComponentBase
    {
        // Im sorry i dont know how to deal with this warning, since both of them are not going to be null
        // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618
        [Inject] AuthenticationStateProvider Asp { get; set; }

        [Inject] IJSRuntime JsRuntime { get; set; }

        [Inject] IDatabaseInterface Database { get; set; }

        //[Inject] NavigationManager Navigator { get; set; }

        private CustomAuthentication Auth { get; set; }
#pragma warning restore CS8618

        public UserInformation UserInfo => Auth.UserInfo;

        // Initlizer Function
        protected override void OnInitialized()
        {
            Auth = (CustomAuthentication)Asp;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //Console.WriteLine("First render !");
                if (!Auth.IsAuthenticated())
                    await UserReAuthorize();
            }

            if (Auth.IsAuthenticated())
                await OnAfterRenderContextAsync(firstRender);
        }

        // In your page override this method to be sure user is authenticated
        public virtual Task OnAfterRenderContextAsync(bool firstRender)
        {
            return Task.CompletedTask;
        }

        // Cookie Writer
        public async Task WriteCookie(string cookieName, string cookieValue, int durationMinutes = 1)
        {
            await JsRuntime.InvokeVoidAsync("CookieWriter.Write", cookieName, cookieValue, DateTime.Now.AddMinutes(durationMinutes));
        }

        // Cookie Reader
        public async Task<string> ReadCookie(string cookieName)
        {
            return await JsRuntime.InvokeAsync<string>("CookieReader.Read", cookieName);
        }

        // Cookie Remover
        public async Task DeleteCookie(string cookieName)
        {
            await JsRuntime.InvokeVoidAsync("CookieRemover.Delete", cookieName);
        }


        //Handles the user login authentication logic
        public async Task<bool> UserLoginAsync(string username, string password)
        {
            // Check if the input is empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) { return false; }

            // try to load the user from database
            var user = await Database.UserInfoFromUsername(username);

            // if the the id == -1 then the user or password is wrong
            if (user.ID == -1) { return false; }

            // Do a password check !
            if (!Auth.VerifyPassword(user, password)) { return false; }

            // login the user into the ASP.NET Auth
            await Auth.LoginAsync(user);

            // Store the session key with other info in Cookie
            int cookieActiveDuration = 360;
            DateTime expiryTime = DateTime.Now.AddMinutes(cookieActiveDuration);
            string cookieKey = string.Format("{0}.{1}.{2}", user.ID, expiryTime.ToBinary(), user.SessionKey);
            await WriteCookie("SessionID", cookieKey, cookieActiveDuration);
            await Database.UpdateUserSessionKey(user.ID, cookieKey);

            return true;
        }

        // Handels the user logout logic 
        public async Task<bool> UserLogoutAsync()
        {
            await Database.UpdateUserSessionKey(UserInfo.ID, "");
            await DeleteCookie("SessionID");
            await Auth.LogoutAsync();
            return true;
        }

        public async Task<bool> UserReAuthorize()
        {
            string sessionKeyCookie = await ReadCookie("SessionID");

            // Check if empty
            if (string.IsNullOrEmpty(sessionKeyCookie)) { return false; }

            string[] keyParts = sessionKeyCookie.Split('.'); // ID, TIME, KEY

            // Check if the key is expired
            DateTime keyTime = DateTime.FromBinary(long.Parse(keyParts[1]));
            if (keyTime < DateTime.Now)
            {
                // Delete key form cookie and db and return false
                await UserLogoutAsync();
                return false;
            }

            int userID = int.Parse(keyParts[0]);

            UserInformation user = await Database.UserInfoFromID(userID);
            await Auth.LoginAsync(user);

            return true;
        }

    }
}
