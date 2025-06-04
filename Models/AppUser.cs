using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Models
{
    public class AppUser:IdentityUser
    {
        public List<AppContent> AppContents { get; set; } = new List<AppContent>();
    }
}
