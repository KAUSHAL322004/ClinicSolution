using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static ClinicApp.Web.Components.Pages.Login;
public class TokenHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    public TokenHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Remove the duplicate SendAsync method. Keep only one implementation.
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        // If 401 Unauthorized → try refresh
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh")
                {
                    Content = JsonContent.Create(new { RefreshToken = refreshToken })
                };

                var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var result = await refreshResponse.Content.ReadFromJsonAsync<TokenResponse>();
                    if (result != null)
                    {
                        await _localStorage.SetItemAsync("authToken", result.AccessToken);

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }
            }
        }

        return response;
    }
}