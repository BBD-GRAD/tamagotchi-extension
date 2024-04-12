using BlazorTamagotchi;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components;
using BlazorTamagotchi.Pages;
using BlazorTamagotchi.Models;
using BlazorTamagotchi.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<NavigationService>();
//builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<Pet>();
builder.Services.AddSingleton<User>();
builder.Services.AddSingleton<MemoryClass>(); 

await builder.Build().RunAsync();
