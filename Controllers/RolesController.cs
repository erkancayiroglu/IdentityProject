using IdentityProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    [Authorize(Roles ="admin")]
    public class RolesController:Controller
    {
        private readonly RoleManager<AppRole> _roleManager; //Role Managerlarına bağlanamak için injection olayı gerçekleşiyor.

        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            // Tüm rolleri al
            var roles = _roleManager.Roles.ToList();
            var roleWithUsers = new List<RoleWithUsersViewModel>();

            foreach (var role in roles)
            {
                // Rolün içindeki kullanıcıları al
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);//Birden fazla kullanıcı varsa liste döner

                // Role ve kullanıcıları birleştir Liste olduğndan Add metodu kullanıyoruz
                roleWithUsers.Add(new RoleWithUsersViewModel
                {
                    Role = role,
                    Users = usersInRole.ToList()//Burada kullanıcıları listeye çeviriyoruz
                });
            }

            return View(roleWithUsers);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppRole model)
        {
            if (ModelState.IsValid)
            {
         

               var result= await _roleManager.CreateAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }


            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role != null && role.Name!=null)
            {
                ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
                return View(role);
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model)
        {
            if (ModelState.IsValid)
            {
               var role = await _roleManager.FindByIdAsync(model.Id);
                if (role != null)
                {
                    role.Name = model.Name;

                    var result = await _roleManager.UpdateAsync(role);

                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    if(role.Name!=null)
                    ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
                }

            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id); // 
            if (role != null)
            {
                //// Bu role atanmış tüm kullanıcıları alıyoruz
                //var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

                // Her kullanıcıyı sırasıyla siliyoruz
                //foreach (var user in usersInRole)
                //{
                //    await _userManager.RemoveFromRoleAsync(user, role.Name); // Kullanıcıyı rolünden çıkarıyoruz
                //    await _userManager.DeleteAsync(user); // Kullanıcıyı siliyoruz
                //}

                // Son olarak rolü siliyoruz
                await _roleManager.DeleteAsync(role); // Rolü siliyoruz
            }

            return RedirectToAction("Index");
        }


    }
}
