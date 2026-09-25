namespace Backend.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Photo_Url { get; set; }
        public string Achieved_At { get; set; }
        public int Sort_Order { get; set; }
    }
}
