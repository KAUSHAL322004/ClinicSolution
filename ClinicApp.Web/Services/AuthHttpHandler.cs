using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using static ClinicApp.Web.Pages.Login;

namespace ClinicApp.Web;

public class AuthHttpHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    public AuthHttpHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            // check expiry (token expires soon)
            if (jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(1))
            {
                var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

                var refreshRequest = new { RefreshToken = refreshToken };
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:5001/");

                var response = await client.PostAsJsonAsync("api/auth/refresh", refreshRequest);
                if (response.IsSuccessStatusCode)
                {
                    var refreshResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    // save new token
                    object value = await _localStorage.SetItemAsync("authToken", refreshResponse.AccessToken);
                    token = refreshResponse.AccessToken;
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}