using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TicketToCode.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
}); 

// Add this configuration
builder.Services.AddScoped(sp => new HttpClient
{
    DefaultRequestHeaders = { { "Access-Control-Allow-Credentials", "true" } }
});

await builder.Build().RunAsync();
