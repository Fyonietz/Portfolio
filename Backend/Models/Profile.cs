namespace Backend.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role_Title { get; set; }
        public string Description { get; set; }
        public string Photo_Url { get; set; }
        public string Status { get; set; }
        public string Bio { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
