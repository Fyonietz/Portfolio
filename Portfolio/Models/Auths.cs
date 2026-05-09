
namespace Portfolio.Models{
    
    public class Admin{
      public int Id{get;set;}
      public string Username{get;set;} = String.Empty;
:     public string Password_Hash{get;set;} = String.Empty;
      public string Email{get;set;} = String.Empty;
      public DateTime Created_At{get;set;}
  }
    public class Profile{
      public int Id {get;set;}
      public string Name {get;set;} = String.Empty;
      public string Role_Title {get;set;} = String.Empty;
      public string Photo_Url {get;set;} = String.Empty;
      public string Status {get;set;} = String.Empty;
      public string Bio {get;set;} = String.Empty;
      public DateTime Created_At{get;set;}

    }
}
