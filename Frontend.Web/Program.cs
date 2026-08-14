using Frontend.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// HTTP client to gateway
builder.Services.AddHttpClient<PatientApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GatewayUrl"]!);
});

// SESSION 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ORDER MATTERS (important)
app.UseRouting();

app.UseSession();

app.UseAuthentication();  //jwt cookies
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patient}/{action=Index}/{id?}");

app.Run();