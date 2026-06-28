using Portfolio.Models;

namespace Portfolio.Services
{
    public class ProfileServices{
      private readonly Database db;
      
      public ProfileServices(Database _db)=>db = _db;
      
      public async Task Create(Profile profile){
        using var conn = db.connect();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "INSERT INTO Profile(name,role_title,photo_url,status,bio,updated_at) VALUES(@name,@role_title,@photo,@status,@bio,@update_at)";
        cmd.Parameters.AddWithValue("@name",profile.Name);
        cmd.Parameters.AddWithValue("@role_title",profile.Role_Title);
        cmd.Parameters.AddWithValue("@photo",profile.Photo_Url);
        cmd.Parameters.AddWithValue("@status",profile.Status);
        cmd.Parameters.AddWithValue("@bio",profile.Bio);
        cmd.Parameters.AddWithValue("@update_at",DateTime.Now);

        await cmd.ExecuteNonQueryAsync();
      }

    }
}
