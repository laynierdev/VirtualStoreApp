using System.Text.Json.Serialization;

namespace VirtualStoreApp.Models;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}