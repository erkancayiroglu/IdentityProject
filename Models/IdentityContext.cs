using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityProject.Models

{
    public class IdentityContext : IdentityDbContext<AppUser,AppRole,string> //IdentityDbContext sınıfını kullanarak kullanıcı ve rol bilgilerini tutan bir veritabanı bağlamı oluşturuyoruz.
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }//base ile dışarıdan gelen options parametresini alıyoruz. Veri tabanı bağlantı dizesini alır ve DbContext sınıfının yapıcısına iletir.

        public DbSet<AppContent> AppContents { get; set; }
    }
}

