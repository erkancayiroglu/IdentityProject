using IdentityProject.DTO;
using IdentityProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//var contents = await _context.AppContents
//    .Include(x => x.User) İçerik içinde kullanıcı bilgilerini almak için
//    .ToListAsync();

namespace IdentityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsApiController : ControllerBase
    {
        private readonly IdentityContext _context;

        public ContentsApiController(IdentityContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetContents()
        {
            var contents = await _context.AppContents.Select(p=> new ContentDTO {                                                                       


                Id=p.Id,
                Title = p.Title,
                Body=p.Body,
                Image = p.Image,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = _context.Users.FirstOrDefault(u => u.Id == p.UserId).UserName,

            }).ToListAsync();
            return Ok(contents);
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> GetContent(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null");
            }
            var content = await _context.AppContents
                .Where(c => c.Id == id)
                .Select(p => new ContentDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Body=p.Body,
                    Image = p.Image,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId,
                    UserName = _context.Users.FirstOrDefault(u => u.Id == p.UserId).UserName,
                })
                .FirstOrDefaultAsync();
            if (content == null)
            {
                return NotFound();
            }
            return Ok(content);
        }
        [HttpPost]
        public async Task<IActionResult> ContentCreate([FromForm] ContentCreateDTO model, IFormFile imageFile)
        {
            ModelState.Remove(nameof(model.Image));
            if (imageFile != null && imageFile.Length > 0)
            {
                // Klasöre kaydetme! Sadece adını al
                var fileName = Path.GetFileName(imageFile.FileName);
                model.Image = fileName; // Sadece ismini alıyoruz
            }

            var appContent = new AppContent
            {
                Title = model.Title,
                CreatedAt = DateTime.Now,
                Body=model.Body,
                Image = model.Image,
                
            };

            _context.AppContents.Add(appContent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContent), new { id = appContent.Id }, appContent);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContent(int id, [FromForm] ContentCreateDTO entity, IFormFile imageFile)
        {
            if (id != entity.Id)
            {
                return NotFound("Content not found");
            }

            var content = await _context.AppContents.FirstOrDefaultAsync(p => p.Id == id);


            if (content == null)
            {
                return NotFound();
            
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                // Klasöre kaydetme! Sadece adını al
                var fileName = Path.GetFileName(imageFile.FileName);
                entity.Image = fileName; // Sadece ismini alıyoruz
            }

            content.Title = entity.Title;
            content.Body = entity.Body;
            content.Image = entity.Image;
            content.CreatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }
            return NoContent(); //Bittikten sonra mesaj bilgisi için 



        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int? id)
        {
            if (id == null)
            {
                return NotFound("user not found");
            }

            var content = await _context.AppContents.FirstOrDefaultAsync(p => p.Id == id);

            if (content == null)
            {
                return NotFound("Content not found");
            }
            _context.AppContents.Remove(content);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return NoContent();

        }












    }
}
