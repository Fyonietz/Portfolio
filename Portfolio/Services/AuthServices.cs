using Portfolio.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
namespace Portfolio.Services{

  public class JwtServices{
    private readonly Envs _env;

    public JwtServices(IOptions<Envs> options){
      _env = options.Value;
    }

    public string GenerateToken(Admin user){
        var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Roles),
        new Claim("userId", user.Id.ToString())
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_env.JWT_KEY!));

    var creds = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        _env.JWT_ISSUER,
        _env.JWT_AUDIENCE,
        claims,
        expires: DateTime.UtcNow.AddHours(6),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler()
        .WriteToken(token); 
    }
  }




  public class AuthServices{
    private readonly Database db;

    public AuthServices(Database _db)=>db=_db;

    
    public async Task<bool> Create(Admin admin){
      using var conn = db.connect();
      using var cmd = conn.CreateCommand();
      cmd.CommandText = "INSERT INTO admin(username,password_hash,email,created_at) VALUES(@name,@password,@email,@created_at)";
      cmd.Parameters.AddWithValue("@name",admin.Username);
      cmd.Parameters.AddWithValue("@password",admin.Password);
      cmd.Parameters.AddWithValue("@email",admin.Email);
      cmd.Parameters.AddWithValue("@created_at",admin.Created_At);
      
      var results = await cmd.ExecuteNonQueryAsync() > 0;
      return await Task.FromResult(results);
    }
  

    public async Task<Admin?> Login(LoginRequest request){
        using var conn = db.connect();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, username, password_hash, email 
            FROM admin 
            WHERE username = @name";

        cmd.Parameters.AddWithValue("@name", request.Name);

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Admin
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            Password = reader.GetString(2), // hashed password
            Email = reader.GetString(3)
        };
    }


  }//AuthServices

    



}//Namespace
