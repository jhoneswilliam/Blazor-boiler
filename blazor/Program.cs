using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using blazor.ApiServices;
using blazor.ApiServices.Security;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using blazor.Providers;
using FluentValidation;
using blazor.Models.Security.Validators;
using blazor.Models.Security;
using blazor.InternalServices;
using blazor.Policies;
using Blazored.Modal;
using blazor.ApiServices.PolicyHandlers;
using blazor.Enums.Security;

namespace blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);


            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var host = builder.Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var baseAPI = configuration.GetSection("BaseApi").Value;

            // ***************************************** DI *****************************************

            // --------- API Services -----------
            builder.Services.AddTransient<AuthService>();

            // --------- Internal Services -----------
            builder.Services.AddTransient<ConfigServices>();

            // ------- DTO validators -------
            builder.Services.AddTransient<IValidator<CreateLoginRequest>, CreateLoginRequestValidator>();

            // --------- auth -----------
            builder.Services.AddScoped<BaseApiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(opt => opt.GetRequiredService<BaseApiAuthenticationStateProvider>());
            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(EnumPermissions.Perm1.ToString(), PoliciesConfig.Perm1());
                config.AddPolicy(EnumPermissions.Perm2.ToString(), PoliciesConfig.Perm2());
            });

            // ***************************************** Others *****************************************
            builder.Services.AddOptions();
            builder.Services.AddTransient<BaseAPIServiceHandler>();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddHttpClient("BaseAPI", client =>
            {
                client.BaseAddress = new Uri(baseAPI);
            })
            .AddHttpMessageHandler<BaseAPIServiceHandler>();



            builder.Services.AddBlazoredModal();

            builder.RootComponents.Add<App>("app");
            await builder.Build().RunAsync();
        }
    }
}
