using Blazored.LocalStorage;
using cAPParel.BlazorApp;
//using cAPParel.BlazorApp.Client.Pages;
using cAPParel.BlazorApp.Components;
using cAPParel.BlazorApp.Helpers;
using cAPParel.BlazorApp.HttpClients;
using cAPParel.BlazorApp.Services.CategoryServices;
using cAPParel.BlazorApp.Services.ItemServices;
using cAPParel.BlazorApp.Services.OrderServices;
using cAPParel.BlazorApp.Services.UserServices;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddHttpClient<cAPParelAPIClient>();
builder.Services.AddSingleton<JsonSerializerOptionsWrapper>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services
  .AddBlazorise()
  .AddBootstrapProviders()
  .AddFontAwesomeIcons();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();
//    .AddAdditionalAssemblies();

app.Run();
