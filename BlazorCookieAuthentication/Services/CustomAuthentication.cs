using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;
using BlazorCookieAuthentication.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorCookieAuthentication.Services
{
    public class CustomAuthentication : AuthenticationStateProvider
    {
        //the user data is stored here
        public UserInformation UserInfo { get; set; } = new();

        public readonly string[] RoleTypes = Enum.GetNames<Roles>();

        // This sets the Auth state with roles
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Default Auth state == no Identity
            AuthenticationState auth = new(new ClaimsPrincipal(new ClaimsIdentity()));

            if (IsAuthenticated())
            {
                //if not empty then populate the Claims with Data!
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, UserInfo.UserName),
                    new Claim(ClaimTypes.Role, RoleTypes[0])
                };

                // Fill roles of the user
                int roleIndex = Array.IndexOf(RoleTypes, UserInfo.UserRole.ToString());
                if (roleIndex != -1)
                {
                    for (int i = roleIndex; i > 0; i--)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, RoleTypes[i]));
                    }
                }

                // create a new state with the roles
                auth = new(new ClaimsPrincipal(new ClaimsIdentity(claims, "YourAppNameHere")));
            }

            return Task.FromResult(auth);
        }

        // Check if it is Authenticated
        public bool IsAuthenticated() => (UserInfo.ID != -1); // -1 is the default, and users ids are positive value

        public async Task LoginAsync(UserInformation userInfo)
        {
            await Task.Run(() => { }); // holder so it would be async
            UserInfo = userInfo;

            // Create Random User sessionKey
            byte[] byteArray = RandomNumberGenerator.GetBytes(8) ;
            string sessionKey = Convert.ToBase64String(byteArray);
            UserInfo.SessionKey = sessionKey;

            // Notify the ASP.NET services that a user change happend
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            await Task.Run(() => { }); // holder so it would be async 
            UserInfo = new();
            
            // Notify the ASP.NET services that a user change happend
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public bool VerifyPassword(UserInformation user, string password)
        {
            //please change this to the proper algo
            return PasswordProcessor.VerifyPassword(password, user.UserPassword);
        }

    }
}
