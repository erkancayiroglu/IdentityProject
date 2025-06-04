using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace IdentityProject.Models
{
    public class IdentitySeedData
    {
        private const string adminUser = "admin";// sabit değişkenler için

        private const string adminPassword = "Admin_123";

        public static async void IdentityTestUser(IApplicationBuilder app)//Veritabanını kontrol edip bir admin oluşturulup oluşmadığını 
        {//app yapılandırma aşaması
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();//veritabanı bağlanamk için sağlamış olduğumuz bağlantı dizesi ile veritabanına bağlanıyoruz.

            if (context.Database.GetAppliedMigrations().Any())//Migraition var i,se update database işlemi yap//mevcut migrationlarını alır.
            {
                context.Database.Migrate();
            }

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager=app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            //UserManager<AppUser>, ASP.NET Core Identity'nin bir parçası olan ve kullanıcı işlemlerini (kullanıcı oluşturma, silme, güncelleme vb.) yöneten bir hizmettir.
            if(!await roleManager.RoleExistsAsync("admin"))//Admin rolü yoksa
            {
                var role = new AppRole
                {
                    Name = "admin",
                    
                };
                await roleManager.CreateAsync(role);
            }


            var user = await userManager.FindByNameAsync(adminUser);

            if (user == null)
            {
                user = new AppUser
                {
                   
                    UserName = adminUser,
                    Email = "admin@erkancayiroglu.com",
                    PhoneNumber = "44444444"
                };

             var results=  await userManager.CreateAsync(user, adminPassword);//Parola hash leme burada       
                if(results.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }

            }
        }
    }
}
