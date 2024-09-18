
// Services/AuthenticationService.cs

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtualStoreApp.Models;

namespace VirtualStoreApp.Services;
public interface IAuthenticationService
{
    Task<string> ExchangeCodeForTokenAsync(string code);
    void SetToken(string token);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthSettings _authSettings;


    public AuthenticationService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, 
        IOptions<AuthSettings> authSettings)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _authSettings = authSettings.Value;
    }

    public async Task<string> ExchangeCodeForTokenAsync(string code)
    {
        var response = await _httpClient.PostAsync($"https://{_authSettings.Domain}/oauth/token", new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _authSettings.ClientId),
            new KeyValuePair<string, string>("client_secret", _authSettings.ClientSecret),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", _authSettings.RedirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
            
        }));

        if (response.IsSuccessStatusCode)
        {
            var tokenContent = await response.Content.ReadAsStringAsync();
            // Here deserialize content and get token
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenContent);
            var accessToken = tokenResponse.AccessToken;
            
            var decodedToken = DecodeToken(accessToken);//TODO remove this line
            Console.WriteLine(decodedToken);//TODO remove this

            return accessToken;
        }

        return null;
    }


    private static string DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        return jsonToken?.Payload.SerializeToJson() ?? string.Empty;
    }
    public void SetToken(string token)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
        }
    }
}
