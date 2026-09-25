using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class ContactServices
    {
        private readonly Database db;

        public ContactServices(Database _db)
        {
            db = _db;
        }

        public async Task<IEnumerable<Contact>> GetAll()
        {
            using var conn = db.connect();
            return await conn.QueryAsync<Contact>(
                "SELECT * FROM Contact ORDER BY Sort_Order ASC"
            );
        }

        public async Task<Contact?> GetById(int id)
        {
            using var conn = db.connect();
            return await conn.QueryFirstOrDefaultAsync<Contact>(
                "SELECT * FROM Contact WHERE Id = @Id", new { Id = id }
            );
        }

        public async Task<bool> Create(Contact data)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                "INSERT INTO Contact(Platform, Value, Sort_Order) VALUES(@Platform, @Value, @Sort_Order)",
                new {
                    Platform   = data.Platform,
                    Value      = data.Value,
                    Sort_Order = data.Sort_Order,
                }
            );
            return res > 0;
        }

        public async Task<bool> Update(int id, Contact data)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                @"UPDATE Contact SET
                    Platform   = @Platform,
                    Value      = @Value,
                    Sort_Order = @SOrder
                WHERE Id = @Id",
                new {
                    Id       = id,
                    Platform = data.Platform,
                    Value    = data.Value,
                    SOrder   = data.Sort_Order,
                }
            );
            return res > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                "DELETE FROM Contact WHERE Id = @Id", new { Id = id }
            );
            return res > 0;
        }

    }//Class
}//Namespace
