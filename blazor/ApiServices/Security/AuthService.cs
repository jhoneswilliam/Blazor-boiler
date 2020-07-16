using blazor.Models;
using blazor.Models.Security;
using blazor.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace blazor.ApiServices.Security
{
    public class AuthService : BaseAPIService
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
            //catch (HttpRequestException httpEx)
            //{
                
            //}
            catch(Exception ex)
            {
                var a = ex.Message;
                return new Result<LoginResponse> { ValidOperation = false, Message = UnknownFailureMessage };
            }
        }
    }
}
