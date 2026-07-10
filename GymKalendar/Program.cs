using Microsoft.EntityFrameworkCore;
using GymKalendar.Data;
using GymKalendar.Models;
using GymKalendar.Controllers;
using Microsoft.AspNetCore.DataProtection;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));


// 1. Подключаем сервис авторизации через Куки
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "GymKalendar.Auth"; // Имя куки-файла в браузере
        options.LoginPath = "/Home/Login";        // Куда слать юзера, если он не авторизован
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Кука будет валидна на сервере 30 дней
        options.SlidingExpiration = true; // Продлевать срок действия, если ты активно пользуешься сайтом
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

string keysFolder = Path.Combine(builder.Environment.ContentRootPath, ".auth-keys");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("GymKalendar");



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 2. Включаем проверку личности (кто ты такой?)
app.UseAuthentication();


app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Register}/{id?}")
    .WithStaticAssets();


app.Run();
