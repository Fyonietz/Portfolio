
namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string ImageUrl { get; set; } = String.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;

        public IFormFile Image { get; set; }
    }
    public class LoginRequest
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string Token { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string ImageUrl { get; set; } = String.Empty;
    }
}
