using blazor.Models;
using blazor.Models.Security;
using blazor.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace blazor.ApiServices.PolicyHandlers
{
    public class BaseAPIServiceHandler : DelegatingHandler
    {
        public string UnknownFailureMessage { get; } = "An unexpected error happened";
        public string UnAuthorizedMessage { get; } = "Not authorized to accomplish this task";
        public string UnAuthenticatedMessage { get; } = "Not authenticated to accomplish this task";

        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public BaseAPIServiceHandler(ILocalStorageService localStorage, NavigationManager navigationManager, IConfiguration configuration, AuthenticationStateProvider authenticationStateProvider)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _configuration = configuration;
            _authenticationStateProvider = authenticationStateProvider;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

            var response = await base.SendAsync(request, cancellationToken);
             
            //200
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response;
            }
            //401
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var result = await RefreshToken();
                if (result)
                {
                    savedToken = await _localStorage.GetItemAsync<string>("authToken");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                    response = await base.SendAsync(request, cancellationToken);

                    //200
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return response;
                    }
                    //403
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        throw new Exception(UnAuthorizedMessage);
                    } 
                    //others
                    else
                    {
                        throw new Exception(UnknownFailureMessage);
                    }
                }
                else
                {
                    _navigationManager.NavigateTo("/logout");
                }
                throw new Exception(UnAuthenticatedMessage);
            }
            //403
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception(UnAuthorizedMessage);
            }
            //others
            else
            {
                throw new Exception(UnknownFailureMessage);
            }
        }

        private async Task<bool> RefreshToken()
        {

            try
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(_configuration.GetSection("BaseApi").Value)
                };

                var authState = await ((BaseApiAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
                var Claims = authState.User.Claims;

                var userEmail = Claims.First(x => x.Type == "Email").Value;

                var refreshLoginRequest = new CreateRefreshLoginRequest
                {
                    AuthRefreshToken = await _localStorage.GetItemAsync<string>("authRefreshToken"),
                    AuthToken = await _localStorage.GetItemAsync<string>("authToken"),
                    Email = userEmail,
                    SignInType = Enums.Security.EnumSignInType.SignInSpa
                };

                StringContent content = new StringContent(JsonConvert.SerializeObject(refreshLoginRequest), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("api/Login/Refresh", content);

                response.EnsureSuccessStatusCode();

                var responseContent = response.Content;

                var strObj = await responseContent.ReadAsStringAsync();

                var loginResponse = JsonConvert.DeserializeObject<Result<LoginResponse>>(strObj);
                if (!loginResponse.ValidOperation)
                    return false;

                await _localStorage.SetItemAsync("authToken", loginResponse.Value.Token);
                await _localStorage.SetItemAsync("authRefreshToken", loginResponse.Value.RefreshToken);

                ((BaseApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAuthenticated(loginResponse.Value.Token);

                return true;
            }
            catch(Exception ex)
            {
                var a = ex;
                Console.WriteLine(a);
                
                return false;
            }
        }
    }
}
