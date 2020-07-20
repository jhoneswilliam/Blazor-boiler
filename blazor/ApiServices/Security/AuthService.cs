using blazor.Models;
using blazor.Models.Security;
using blazor.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace blazor.ApiServices.Security
{
    public class AuthService
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(IHttpClientFactory  _httpFactory, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _client = _httpFactory.CreateClient("BaseAPI");
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task SingOutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((BaseApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            //_httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<Result<LoginResponse>> SignInAsync(CreateLoginRequest loginRequest)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(loginRequest), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync("api/Login", content);

                response.EnsureSuccessStatusCode();

                var responseContent = response.Content;
               
                var strObj = await responseContent.ReadAsStringAsync();
               
                var loginResponse = JsonConvert.DeserializeObject<Result<LoginResponse>>(strObj);
                if (!loginResponse.ValidOperation)
                    return loginResponse;

                Console.WriteLine(loginResponse.Value.Token);
                Console.WriteLine((BaseApiAuthenticationStateProvider)_authenticationStateProvider);
                
               await _localStorage.SetItemAsync("authToken", loginResponse.Value.Token);
               ((BaseApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAuthenticated(loginResponse.Value.Token);
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

                return loginResponse;
            }
            catch (Exception ex)
            {
                return new Result<LoginResponse> { ValidOperation = false, Message = ex.Message };
            }
        }

        public async Task<Result> teste()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/Login");

                return new Result { ValidOperation = true, Message = null};
            }
            catch (Exception ex)
            {
                return new Result { ValidOperation = false, Message = ex.Message };
            }
        }
    }
}
