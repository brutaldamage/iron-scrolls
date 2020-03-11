using IronJournal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
namespace IronJournal
{
    public class CustomAuthenticationProvider : AuthenticationStateProvider
    {
        readonly ICurrentUser _currentUser;

        public CustomAuthenticationProvider(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Console.WriteLine("auth provider, get auth state");

            ClaimsPrincipal user = new ClaimsPrincipal();

            // Call the GetUser method to get the status
            // This only sets things like the AuthorizeView
            // and the AuthenticationState CascadingParameter
            var result = await _currentUser.GetCurrentUser();

            // Was a UserName returned?
            if (result != null && !result.IsAnonymous && !string.IsNullOrEmpty(result.Email))
            {
                // Create a ClaimsPrincipal for the user
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Email, result.Email),
                   new Claim("Avatar", result.PhotoUrl),
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