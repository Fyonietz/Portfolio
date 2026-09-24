namespace Backend.Models
{
    public class Project
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Short_Description { get; set; } = string.Empty;
        public string Full_Description { get; set; } = string.Empty;
        public string Photos { get; set; } = string.Empty;
        public string Tech_Stack { get; set; } = string.Empty;
        public string Repo_Url { get; set; } = string.Empty;
        public string Demo_Url { get; set; } = string.Empty;
        public int Sort_Order { get; set; } = 0;
        public bool Published { get; set; } = false;
    }
}
