namespace IdentityProject.Models
{
    public class RoleWithUsersViewModel
    {
        public AppRole Role { get; set; }
        public List<AppUser> Users { get; set; }
    }
}
