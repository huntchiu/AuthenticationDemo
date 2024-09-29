using AuthenticationDemo.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 向容器中添加服务。
builder.Services.AddControllers(); // 添加控制器服务。
// 了解更多关于 Swagger/OpenAPI 配置的信息，请访问 https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen();  

// 获取数据库连接字符串
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 配置数据库上下文，使用 SQLite 数据库。
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 配置 Identity 服务，使用 IdentityUser 和 IdentityRole。
// 使用 Entity Framework 存储应用程序用户和角色，并添加默认令牌提供程序。
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 配置应用程序的 Cookie 验证选项。
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // 仅允许 HTTP 访问 Cookie，防止 JavaScript 读取，提高安全性。
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // 设置 Cookie 过期时间为 30 分钟。
    options.LoginPath = "/api/account/login"; // 设置未认证用户的跳转登录路径。
    options.AccessDeniedPath = "/api/account/access-denied"; // 设置访问被拒绝时的路径。
    options.SlidingExpiration = true; // 开启滑动过期，即在每次访问时重新计算过期时间。
});
// LoginPath：适用于未认证用户，需要登录。
// AccessDeniedPath：适用于已认证用户，但没有访问权限。

// 添加并配置 Cookie 认证。
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();


var app = builder.Build();

// 配置 HTTP 请求管道。
// 如果应用程序在开发环境中，启用 Swagger 和 Swagger UI。
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseAuthentication();
app.UseAuthorization();  

app.MapControllers();  

app.Run(); 