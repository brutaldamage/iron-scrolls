using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using IronJournal.Services;

namespace IronJournal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // add internal services
            builder.Services.AddSingleton<IAuthHelper, AuthHelper>();

            // Use our CustomAuthenticationProvider as the 
            // AuthenticationStateProvider
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationProvider>();
            // Add Authentication support
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
