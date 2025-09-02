using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    public AuthStateProvider(ILocalStorageService localStorage) => _localStorage = localStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var identity = string.IsNullOrWhiteSpace(token)
            ? new ClaimsIdentity()
            : new ClaimsIdentity(ParseClaims(token), "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication(string token)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaims(token), "jwt"));
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}
