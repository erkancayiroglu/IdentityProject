using IdentityProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityProject.Controllers
{
    [Authorize(Roles ="admin,editör")]
    public class ContentsController : Controller
    {
        private readonly IdentityContext _context;

        public ContentsController(IdentityContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppContent model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                model.UserId = userId;

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Klasöre kaydetme! Sadece adını al
                    var fileName = Path.GetFileName(imageFile.FileName);
                    model.Image = fileName; // varsayalım img klasöründe zaten bu dosya var
                }

                _context.AppContents.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_context.AppContents);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var content = await _context.AppContents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppContent model, IFormFile? imageFile)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
           
            

            if (ModelState.IsValid)
            {
                var content = await _context.AppContents.FindAsync(id);
                if (content == null)
                {
                    return NotFound();
                }

                content.Title = model.Title;
                content.Body = model.Body;

                // Yeni resim seçildiyse sadece adını veritabanına yaz
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Sadece dosya adını alıyoruz
                    content.Image = imageFile.FileName;
                }

                // Seçilmemişse eski resim yolu zaten korunmuş olur
                _context.Update(content);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var content = await _context.AppContents.FindAsync(id);

            if(content!= null)
            {
                _context.AppContents.Remove(content);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return NotFound();

            


        }





    }
}
