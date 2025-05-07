using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class EditViewModel
    {


        public string? Id { get; set;}

        public string? UserName { get; set;}

        [EmailAddress]
        public string? Email { get; set; }


        [DataType(DataType.Password)]
        public string? Password { get; set; }


        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parola eşleşmiyor.")]
        public string? ConfirmPassword { get; set; } = string.Empty;

        public IList<string>? SelectedRoles { get; set; }

    }
}
