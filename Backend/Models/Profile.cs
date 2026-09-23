using System;

namespace Backend.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role_Title { get; set; } = string.Empty;
        public string Photo_Url { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public DateTime Updated_At { get; set; }
    }
}
