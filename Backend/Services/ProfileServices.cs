using Portfolio.Models;

namespace Portfolio.Services
{
    public class ProfileServices
    {
        private readonly Database db;

        public ProfileServices(Database _db) => db = _db;

        public async Task Create(Profile profile)
        {
            using var conn = db.connect();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO Profile(name,role_title,photo_url,status,bio,updated_at) VALUES(@name,@role_title,@photo,@status,@bio,@update_at)";
            cmd.Parameters.AddWithValue("@name", profile.Name);
            cmd.Parameters.AddWithValue("@role_title", profile.Role_Title);
            cmd.Parameters.AddWithValue("@photo", profile.Photo_Url);
            cmd.Parameters.AddWithValue("@status", profile.Status);
            cmd.Parameters.AddWithValue("@bio", profile.Bio);
            cmd.Parameters.AddWithValue("@update_at", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<List<Profile>> GetAllProfile()
        {
            using var conn = db.connect();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM Profile";
            var profiles = new List<Profile>();

            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                profiles.Add(new Profile
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Role_Title = reader.GetString(2),
                    Photo_Url = reader.GetString(3),
                    Status = reader.GetString(4),
                    Bio = reader.GetString(5),
                    Created_At = reader.GetDateTime(6),
                });
            }
            return await Task.FromResult(profiles);

        }

    }
}
