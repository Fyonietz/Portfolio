
namespace Portfolio.Models{
    
    public class Admin{
      public int Id{get;set;}
      public string Username{get;set;} = String.Empty;
      public string Password{get;set;} = String.Empty;
      public string Email{get;set;} = String.Empty;
      public string Roles {get;set;} = "Admin";
      public DateTime Created_At{get;set;} = DateTime.UtcNow;
  }
    public class Profile{
      public int Id {get;set;}
      public string Name {get;set;} = String.Empty;
      public string Role_Title {get;set;} = String.Empty;
      public string Photo_Url {get;set;} = String.Empty;
      public string Status {get;set;} = String.Empty;
      public string Bio {get;set;} = String.Empty;
      public DateTime Created_At{get;set;} = DateTime.UtcNow;

    }
    public class Envs
    {
        public string? JWT_KEY { get; set; }
        public string? JWT_ISSUER { get; set; }
        public string? JWT_AUDIENCE { get; set; }
    } 
    public class LoginRequest{ 
        public required string Name {get;set;}
        public required string Password {get;set;}
    }
}
