
namespace Portfolio.Models{
    public class Achievement{
      public int Id {get;set}
      public string Title {get;set} = String.Empty;
      public string Description {get;set} = String.Empty;
      public string Date_Label {get;set} = String.Empty;
      public string Context {get;set} = String.Empty;
      public DateTime Created_At {get;set;} 
    }

    public class Tags{
      public int Id {get;set;}
      public int Achievement_Id {get;set}
      public string Tag {get;set;} = String.Empty;
    }
 
    public class Photos{
      public int Id {get;set;}
      public int Achievement_Id {get;set}
      public string Url {get;set;} = String.Empty;
      public int Sort_Order {get;set;}
    }
}
