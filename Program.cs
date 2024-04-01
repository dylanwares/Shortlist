var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "SignUp",
        pattern: "signup",
        defaults: new { controller = "Home", action = "SignUp" });

    endpoints.MapControllerRoute(
        name: "Login",
        pattern: "login",
        defaults: new { controller = "Home", action = "Login" });

    endpoints.MapControllerRoute(
        name: "Dashboard",
        pattern: "dashboard",
        defaults: new { controller = "Home", action = "Dashboard" });

    endpoints.MapControllerRoute(
        name: "Shortlist",
        pattern: "shortlist",
        defaults: new { controller = "Home", action = "Shortlist" });

    endpoints.MapControllerRoute(
        name: "Account",
        pattern: "account",
        defaults: new { controller = "Home", action = "Account" });

    endpoints.MapControllerRoute(
        name: "Post",
        pattern: "post",
        defaults: new { controller = "Home", action = "Post" });

    // Map additional endpoints if needed
});

app.Run();
