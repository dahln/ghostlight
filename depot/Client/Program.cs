using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazored.Toast;
using Blazored.LocalStorage;
using depot.Client.Services;
using Blazored.Modal;

namespace depot.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped<API>();
            builder.Services.AddScoped<AppState>();
            builder.Services.AddScoped<LoadingBarService>();

            builder.RootComponents.Add<App>("app");

            builder.Services.AddApiAuthorization();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredModal();

            await builder.Build().RunAsync();
        }
    }
}
