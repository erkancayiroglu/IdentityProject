using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityProject.Models
{
    public class AppContent
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Body { get; set; }

        [Display(Name = "İçerik Resim")]
        public string? Image { get; set; }


        public string? UserId { get; set; } // Kullanıcı ID'si
        
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }// AppUser ile ilişki

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}