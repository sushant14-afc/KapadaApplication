using Microsoft.JSInterop;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

namespace KapadaApplication.Services
{
    public class UserService
    {
        private readonly IJSRuntime _js;
        private readonly HttpClient _http;

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public string? Token { get; private set; }
        public string? Username { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public UserService(IJSRuntime js, HttpClient http)
        {
            _js = js;
            _http = http;
        }

        public async Task SetUserAsync(string token, string username)
        {
            Token = token;
            Username = username;

            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            await _js.InvokeVoidAsync("localStorage.setItem", "username", username);

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            NotifyStateChanged();
        }

        public async Task LoadUserFromStorage()
        {
            Token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            Username = await _js.InvokeAsync<string>("localStorage.getItem", "username");

            if (!string.IsNullOrEmpty(Token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }
            NotifyStateChanged();
        }

        public async Task Logout()
        {
            Token = null;
            Username = null;

            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "username");

            _http.DefaultRequestHeaders.Authorization = null;

            NotifyStateChanged();
        }
    }
}
