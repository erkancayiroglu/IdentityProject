using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Email Değiştirmek için required ile kesin tanımlı hata mesajı içim



        [Required]

        public string Token { get; set; } = string.Empty; //token bilgisi gelecek


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;    // Password bilgisi girilecek güncel

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Parola eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;


    }
}
