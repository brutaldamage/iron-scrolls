using IronJournal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
namespace IronJournal
{
    public class CustomAuthenticationProvider : AuthenticationStateProvider
    {
        private readonly IAuthHelper _authHelper;
        public CustomAuthenticationProvider(IAuthHelper authHelper)
        {
            _authHelper = authHelper;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Console.WriteLine("auth provider, get auth state");

            ClaimsPrincipal user = new ClaimsPrincipal();
            // Call the GetUser method to get the status
            // This only sets things like the AuthorizeView
            // and the AuthenticationState CascadingParameter
            var result = await _authHelper.GetCurrentUser(new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);

            // Was a UserName returned?
            if (result != null && !result.IsAnonymous && !string.IsNullOrEmpty(result.Email))
            {
                // Create a ClaimsPrincipal for the user
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Email, result.Email),
                   new Claim(ClaimTypes.Name, result.DisplayName)
                }, "Firebase");

                user = new ClaimsPrincipal(identity);
            }
            else
            {
                user = new ClaimsPrincipal(); // Not logged in
            }

            return new AuthenticationState(user);
        }
    }
}