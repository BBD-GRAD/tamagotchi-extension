using Microsoft.AspNetCore.Authentication.Cookies;
using System.Timers;
using WebTamagotchi.Data;
using WebTamagotchi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/account/google-login";
}).AddGoogle(options =>
{
    options.ClientId = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

builder.Services.AddSingleton<ViewModel>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ITamagotchiRepository, TamagotchiRepository>(c =>
c.BaseAddress = new Uri("https://localhost:7163/"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Test}");

app.Run();