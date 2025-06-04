﻿using IdentityProject.Models;
using IdentityProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    public class AccountController:Controller
    {
        private UserManager<AppUser> _userManager; //UserManager bir yere tanımlı değil otomatik gelioyr sadece injection yapmak yeterli
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;
        private IEmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager; //Giriş yapıldığında yaptığımız işlem 
            _emailSender = emailSender; //Smtp işlemleri için 
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    if (!await _userManager.IsEmailConfirmedAsync(user))//Model Onayı kontrolü ypmayı unutma
                    {
                        ModelState.AddModelError("", "Hesabınızı Onaylayınız.");
                        return View(model); 
                    }
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user); //Hatalı giriş sayacı sıfırlanır
                        await _userManager.SetLockoutEndDateAsync(user, null);//Bitiş süresini sıfırlama işlemi kilitlenme işlemi 
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut) //Giriş başarısızsa hesap kilitlenmişse bu
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeleft = lockoutDate.Value - DateTime.UtcNow;
                        ModelState.AddModelError("", $"Hesabınız kilitledi, Lütfen {timeleft.Minutes} dakika sonra deneyiniz.");
                    }
                    else//Yanlış şifre girilmişse parola hatalı der.
                    {
                        ModelState.AddModelError("", "parolanız hatalı");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Böyle email bulunamadı.");
                }


            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
      
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,

                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token }, Request.Scheme);

                    string emailBody = $"Lütfen email onayınız için <a href='{url}'>tıklayın</a>";

                    await _emailSender.SendEmailAsync(user.Email, "Hesap Onayı", emailBody);


                    return RedirectToAction("Login","Account");

                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string Id,string token)
        {
            if(Id==null || token == null)
            {
                TempData["message"] = "Geçersiz token bilgisi";
                return View();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if(user!= null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData["message"] = "Hesabınız Onaylandı";
                    return RedirectToAction("Login", "Account");
                }

            }
            TempData["message"] = "Kullanıcı Bulunamadı";
            return View();



        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["message"] = "Eposta Adresinizi Giriniz.";
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                TempData["message"] = "Eşleşen Mail Yok";
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);//Şifre tokeni
            var url = Url.Action("ResetPassword", "Account", new { Id = user.Id, token }, Request.Scheme);
            string emailBody = $"Lütfen email onayınız <a href='{url}'>tıklayın</a>"; //url oluşturup resete yönlendirdim.


            await _emailSender.SendEmailAsync(Email, "Parola Sıfırlama", emailBody);

            TempData["message"] = "Sıfırlayın";
            return View();



        }
        public IActionResult ResetPassword(string id, string token)
        {
            if (id == null || token == null)

            {
                return RedirectToAction("Login");
            }
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["message"] = "Bu Mail Adresi ile eşleşen mail yok";
                    return RedirectToAction("Login");

                }
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    TempData["message"] = "Şifreniz Değiştirildi";
                    return RedirectToAction("Login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);

        }

    }
}
