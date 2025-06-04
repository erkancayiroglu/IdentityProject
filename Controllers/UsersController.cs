using IdentityProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace IdentityProject.Controllers
{
   
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task< IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var nonAdminUsers = new List<AppUser>();//Kullanıcıları listeleyecek nesne oluşturdum

            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            return View(nonAdminUsers);
        }

       
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();//tüm roller sıralanır
                return View(new EditViewModel
                {

                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    SelectedRoles = await _userManager.GetRolesAsync(user) //Sadece usera ait Rolleri bana geliyor.
                });
            }


            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {

                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    var result = await _userManager.UpdateAsync(user); //Doğrudan şifre güncellenmez
                    if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user); //eğer password alanı boş değilse ilk önce kullanıcın şifreyi sil
                        await _userManager.AddPasswordAsync(user, model.Password);//Sonra modelden gelen passwordu ekle
                    }



                    if (result.Succeeded)
                    {
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user)); //ilk kullanıcıya ait roller silinir
                        if (model.SelectedRoles != null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles); // eğer sonra seçimiş bir rol var ise bunuda ekle

                        }
                        return RedirectToAction("Index");
                    }
                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }

            }


            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult>Delete(string id)
        {
            if (id == null)
            {
                return NotFound();  
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
         
            }
            return RedirectToAction("Index");
        }
    }       
}
