using IdentityProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);//T�m serviceleri ayrlamaaya yarar

// Swagger servisi ekle 1.olan
builder.Services.AddEndpointsApiExplorer();//Swagger, API'yi test etmek ve belgelemek i�in kullan�lan bir ara�t�r.
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailSender, SmtpGridEmailSender>(i =>
    new SmtpGridEmailSender(
        builder.Configuration["EmailSender:Host"],
        builder.Configuration.GetValue<int>("EmailSender:Port"),
        builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
        builder.Configuration["EmailSender:Username"],
        builder.Configuration["EmailSender:Password"]

        ));

// Add services to the container.
builder.Services.AddControllersWithViews();//MVC yap�s�n� kullanmak i�in gerekli olan servisleri ekler



// Ba�lant� dizesini al
var connectionString = builder.Configuration.GetConnectionString("database");//appsettings.json dosyas�ndaki "ConnectionStrings" b�l�m�nden "database" anahtar�na ait veritaban� ba�lant� dizesi al�n�r.

// DbContext'i ayarlay�n
builder.Services.AddDbContext<IdentityContext>(options =>           //IdentityContext, DbContext s�n�f�ndan t�retilmi�tir ve veritaban� i�lemlerini yapar.
    options.UseSqlServer(connectionString));

// Identity yap�land�rmas�
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<IdentityContext>()// Kimlik verileri veritaban�nda tutulacak.
    .AddDefaultTokenProviders();//E-posta onay�, �ifre s�f�rlama gibi i�lemler i�in token �reten yap�d�r.


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6; //Karakter Uzunluk
    options.Password.RequireNonAlphanumeric = false;// numric de�er istiyor muyuz.
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;//sadece say�sal de�erler olailir

    options.User.RequireUniqueEmail = true; //emailin uniq olmas� laz�m
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";//Giri� yapmak istedi�mizde art�k bura
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true; //kullana�c� giri� yapt���nda her defas�nda 30 olur.
    options.ExpireTimeSpan = TimeSpan.FromDays(30);//Giri� yap�ld�ktan sonra 30 g�n s�re var 

});


var app = builder.Build(); //app, uygulaman�n kendisini temsil eder ve middleware'leri eklemek i�in kullan�l�r.

if (app.Environment.IsDevelopment()) //2.olan //Geli�tirme modundaysa aktif edilir
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//Middleware, uygulaman�n istekleri nas�l i�ledi�ini belirleyen bile�enlerdir.
app.UseHttpsRedirection();
app.UseStaticFiles();//Static dosyalar i�in gerekli

app.UseRouting();//Controller ve Action 'lar� bulmak i�in gerekli

app.UseAuthentication();//Giri� i�in �nemli

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityTestUser(app);
app.Run();
