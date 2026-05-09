

namespace Portfolio.Models{

      public class Contact{
        public int Id {get;set;}
        public int Profile_Id {get;set;}
        public string Platform {get;set;} = String.Empty;
        public string Value {get;set;} = String.Empty;
        public int Sort_Order {get;set;}
      } 

      public class Skill{
        public int Id {get;set;}
        public string Name {get;set;} = String.Empty;
        public string Category {get;set;} = String.Empty;
        public int Profiency {get;set;}
        public int Sort_Order {get;set;}
      }


      public class Project{
        public int Id {get;set;}
        public string Title {get;set;} = String.Empty;
        public string Description {get;set;} = String.Empty;
        public string Repo_Url {get;set;} = String.Empty;
        public int Sort_Order {get;set;}
        public DateTime Created_At {get;set;}
      }

}


