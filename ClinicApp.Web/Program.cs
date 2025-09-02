//using Microsoft.AspNetCore.Components.Web;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
//using Blazored.LocalStorage;
//using Microsoft.AspNetCore.Authorization;

//namespace ClinicApp.Web;

//public class Program
//{
//public static async Task Main(string[] args)
// {
// var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
// builder.RootComponents.Add<HeadOutlet>("head::after");
// builder.Services.AddBlazoredLocalStorage();
// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// builder.Services.AddScoped(sp => new HttpClient
// {
//  BaseAddress = new Uri("https://localhost:7149/") // address of your API
//   });

//   await builder.Build().RunAsync();
//  }
//}




using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using ClinicApp.Web;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// For same-origin requests (default)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();

// TokenHandler => har API call me JWT chipkane ke liye
builder.Services.AddScoped<TokenHandler>();

//Add httpClient with Auth handler
builder.Services.AddTransient<AuthHttpHandler>();

// Named HttpClient for your API
{
    c.BaseAddress = new Uri("https://localhost:7149/"); // **yaha apna API URL/port daalo**
}

// Resolve default HttpClient as the API client (easy inject)
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Custom auth state provider
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

await builder.Build().RunAsync();
