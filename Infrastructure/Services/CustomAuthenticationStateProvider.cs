using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace RailwayTicketBooking.Infrastructure.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IJwtService _jwtService;

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, IJwtService jwtService)
        {
            _jsRuntime = jsRuntime;
            _jwtService = jwtService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Get token from localStorage
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Validate and parse token
                var principal = _jwtService.ValidateToken(token);

                if (principal == null)
                {
                    // Token is invalid, clear it
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                return new AuthenticationState(principal);
            }
            catch
            {
                // If there's any error, return unauthenticated state
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthentication(string token)
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal != null)
            {
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            }
        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }
    }
}

