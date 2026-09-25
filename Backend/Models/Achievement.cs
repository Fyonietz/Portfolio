namespace Backend.Models
{
    public class Achievement
    {
        public int    Id          { get; set; }
        public string Title       { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Achieved_At { get; set; } = string.Empty;
        public int    Sort_Order  { get; set; } = 0;
    }
}
