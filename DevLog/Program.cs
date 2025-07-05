using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Services.EmailSender;
using DevLog.Services.FileHandler;
using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Extensions;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment webHostEnvironment = builder.Environment;

//Configure file handler options
builder.Services.PostConfigure<FileHandlerOptions>(options =>
{
    options.WebRootPath = webHostEnvironment.WebRootPath;
    options.BaseFilesPath = FilePathsConstant.BaseFilesPath;
    options.PostFilesPath = FilePathsConstant.PostFilesPath;
    options.ProfileFilesPath = FilePathsConstant.ProfileFilesPath;
    options.CertificateFilesPath = FilePathsConstant.CertificateFilesPath;
});

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IFileHandler, FileHandler>();

//Add breadcrumbs support
builder.Services.AddBreadcrumbs(typeof(Program).Assembly, options =>
{
    options.TagName = "nav";
    options.TagClasses = "";
    options.OlClasses = "breadcrumb";
    options.LiClasses = "breadcrumb-item";
    options.ActiveLiClasses = "breadcrumb-item active";
    options.SeparatorElement = "<li class=\"separator\">/</li>";
});

//Add Elmah logger
builder.Services.AddElmah<XmlFileErrorLog>(options =>
{
    options.Path = "/admin/logs";
    options.LogPath = "~/logs";
});

builder.Services.Configure<EmailSenderOptions>(options =>
    configuration.GetSection("EmailService").Bind(options));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson()
    .AddRazorRuntimeCompilation();

builder.Services.ConfigureApplicationCookie(options =>
    options.LoginPath = "/Panel/Users/Login");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseElmah();

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
        areaName: "Panel",
        name: "panel",
        pattern: "/panel/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );

//Ensure Database Creation
using var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();
if (serviceScope != null)
{
    var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
    var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.Migrate();

    var roles = new List<string>()
        {
            UserRolesConstant.SuperAdmin,
            UserRolesConstant.Admin,
            UserRolesConstant.Writer
        };

    var user = new User()
    {
        FirstName = "FirstName",
        LastName = "LastName",
        UserName = "A@dmin123",
        Email = "example@gmail.com",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true,
        IsActive = true,
        CreateDate = DateTime.Now,
        LastEditDate = DateTime.Now
    };

    ApplicationDbInitializer.SeedData(
        context,
        userManager!,
        roleManager!,
        roles,
        UserRolesConstant.SuperAdmin,
        user,
        "P@ss123");
}

app.Run();
