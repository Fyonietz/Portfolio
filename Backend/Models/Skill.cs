namespace Backend.Models
{
    public class Skill
    {
        public int    Id          { get; set; }
        public string Name        { get; set; } = string.Empty;
        public string Category    { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon_Url    { get; set; } = string.Empty;
        public int    Level       { get; set; } = 0;
        public int    Sort_Order  { get; set; } = 0;
        public bool   Published   { get; set; } = true;
        public string Created_At  { get; set; } = string.Empty;
        public string Updated_At  { get; set; } = string.Empty;
    }
}
