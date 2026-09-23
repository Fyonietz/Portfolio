namespace Backend.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Sort_Order { get; set; }
    }
}
