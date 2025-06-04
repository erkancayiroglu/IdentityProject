namespace IdentityProject.DTO
{
    public class ContentDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
       public string Body { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = null!; // Kullanıcı ID'si
        public string UserName { get; set; } = null!; // Kullanıcı adı
    }
}
