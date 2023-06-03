using CooksCornerAPP.Data;
using CooksCornerAPP.Repositories.Abstract;
using CooksCornerAPP.Repositories.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using EmailService;
using CooksCornerAPP.Services;
using CooksCornerAPP.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();



builder.Services.ConfigureApplicationCookie(options =>

{
    options.LoginPath = "/UserAuth/Login";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.LogoutPath = "/UserAuth/Logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.AccessDeniedPath = "/UserAuth/Login";

});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSignalR();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.AddTransient(typeof(GoogleCaptchaService));
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
    options.AddPolicy("ModeratorPolicy", policy => policy.RequireRole("moderator"));
});

var MyAllowedSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowedSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7058", "http://localhost:5000", "http://localhost:6009", "http://192.168.56.102").AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.Configure<GoogleCaptchaConfig>(builder.Configuration.GetSection("GoogleReCaptcha"));
//Add Email Configs
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

//    await next();

//    if (!context.Response.Headers.ContainsKey("Content-Type"))
//    {
//        context.Response.Headers.Add("Content-Type", "application/octet-stream");
//    }

//    if (context.Response.Headers["Content-Type"].ToString().StartsWith("text") ||
//        context.Response.Headers["Content-Type"].ToString().EndsWith("+xml") ||
//        context.Response.Headers["Content-Type"].ToString() == "application/xml")
//    {
//        if (!context.Response.Headers.ContainsKey("Content-Encoding"))
//        {
//            context.Response.Headers.Add("Content-Encoding", "UTF-8");
//        }
//    }
//});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

// SignalR hub imlementeren
app.MapHub<RecipeHub>("/recipehub");
app.UseSession();

app.Run();
