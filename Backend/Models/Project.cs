namespace Backend.Models
{
    public class Project
    {
        public int    Id                { get; set; }
        public string Title             { get; set; } = string.Empty;
        public string Slug              { get; set; } = string.Empty;
        public string Short_Description { get; set; } = string.Empty;
        public string Full_Description  { get; set; } = string.Empty;
        public string Photos            { get; set; } = string.Empty;
        public string Repo_Url          { get; set; } = string.Empty;
        public string Demo_Url          { get; set; } = string.Empty;
        public int    Sort_Order        { get; set; } = 0;
        public bool   Published         { get; set; } = false;
        public string Created_At        { get; set; } = string.Empty;
        public string Updated_At        { get; set; } = string.Empty;

        // relasi — tidak disimpan di DB, diisi saat query
        public List<TechStack> TechStacks { get; set; } = new();
    }

    public class TechStack
    {
        public int    Id         { get; set; }
        public string Name       { get; set; } = string.Empty;
        public string Icon_Url   { get; set; } = string.Empty;
        public int    Sort_Order { get; set; } = 0;
    }

    // untuk terima input dari frontend
    public class ProjectRequest
    {
        public string Title             { get; set; } = string.Empty;
        public string Slug              { get; set; } = string.Empty;
        public string Short_Description { get; set; } = string.Empty;
        public string Full_Description  { get; set; } = string.Empty;
        public string Photos            { get; set; } = string.Empty;
        public string Repo_Url          { get; set; } = string.Empty;
        public string Demo_Url          { get; set; } = string.Empty;
        public int    Sort_Order        { get; set; } = 0;
        public bool   Published         { get; set; } = false;
        public List<int> TechStack_Ids  { get; set; } = new(); // array id dari frontend
    }
}
