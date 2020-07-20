using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace blazor.ApiServices.PolicyHandlers
{
    public class BaseAPIServiceHandler : DelegatingHandler
    {
        public string UnknownFailureMessage { get; } = "An unexpected error happened";
        public string UnAuthorizedMessage { get; } = "Not authorized to accomplish this task";
        public string UnAuthenticatedMessage { get; } = "Not authenticated to accomplish this task";
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public BaseAPIServiceHandler(ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

            var response = await  base.SendAsync(request, cancellationToken);

            //200
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response;
            }
            //401
            else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
            {
                _navigationManager.NavigateTo("/logout");
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
    }
}
