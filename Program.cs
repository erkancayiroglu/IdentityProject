using IdentityProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);//Tüm serviceleri ayrlamaaya yarar

// Swagger servisi ekle 1.olan
builder.Services.AddEndpointsApiExplorer();//Swagger, API'yi test etmek ve belgelemek için kullanýlan bir araçtýr.
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
builder.Services.AddControllersWithViews();//MVC yapýsýný kullanmak için gerekli olan servisleri ekler



// Baðlantý dizesini al
var connectionString = builder.Configuration.GetConnectionString("database");//appsettings.json dosyasýndaki "ConnectionStrings" bölümünden "database" anahtarýna ait veritabaný baðlantý dizesi alýnýr.

// DbContext'i ayarlayýn
builder.Services.AddDbContext<IdentityContext>(options =>           //IdentityContext, DbContext sýnýfýndan türetilmiþtir ve veritabaný iþlemlerini yapar.
    options.UseSqlServer(connectionString));

// Identity yapýlandýrmasý
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<IdentityContext>()// Kimlik verileri veritabanýnda tutulacak.
    .AddDefaultTokenProviders();//E-posta onayý, þifre sýfýrlama gibi iþlemler için token üreten yapýdýr.


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6; //Karakter Uzunluk
    options.Password.RequireNonAlphanumeric = false;// numric deðer istiyor muyuz.
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;//sadece sayýsal deðerler olailir

    options.User.RequireUniqueEmail = true; //emailin uniq olmasý lazým
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";//Giriþ yapmak istediðmizde artýk bura
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true; //kullanaýcý giriþ yaptýðýnda her defasýnda 30 olur.
    options.ExpireTimeSpan = TimeSpan.FromDays(30);//Giriþ yapýldýktan sonra 30 gün süre var 

});


var app = builder.Build(); //app, uygulamanýn kendisini temsil eder ve middleware'leri eklemek için kullanýlýr.

if (app.Environment.IsDevelopment()) //2.olan //Geliþtirme modundaysa aktif edilir
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
//Middleware, uygulamanýn istekleri nasýl iþlediðini belirleyen bileþenlerdir.
app.UseHttpsRedirection();
app.UseStaticFiles();//Static dosyalar için gerekli

app.UseRouting();//Controller ve Action 'larý bulmak için gerekli

app.UseAuthentication();//Giriþ için önemli

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityTestUser(app);
app.Run();
