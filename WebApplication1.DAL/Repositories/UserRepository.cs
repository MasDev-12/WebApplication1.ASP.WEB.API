using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.Domain.Entity;

namespace WebApplication.DAL.Repositories
{
    public class UserRepository : IBaseRepository<User>
    {
        private readonly IDapperConnection _dapperConnection;

        public UserRepository(IDapperConnection dapperConnection) 
        {
            _dapperConnection=dapperConnection;
        }
        public async Task<bool> Create(User entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "INSERT INTO Users (Login, Password, Email,Name,Role) VALUES (@Login, @Password,@Email,@Name,@Role)";
                var result = await connection.ExecuteAsync(sql, entity);
                return result > 0 ? true : false;
            }
        }

        public async Task<bool> Delete(User entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "DELETE FROM Users WHERE Id=@Id";
                var result = await connection.ExecuteAsync(sql, entity);
                return result > 0 ? true : false;
            }
        }

        public IEnumerable<User> GetAll()
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "SELECT * FROM Users";
                return connection.Query<User>(sql);
            }
        }

        public async Task<User> GetById(int id)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "SELECT * FROM Regions WHERE Id = @Id";
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<User> Update(User entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "UPDATE Users SET Password = @Password,Email = @Email, Name = @Name, WHERE Id = @Id";
                await connection.ExecuteAsync(sql, entity);
                sql = "SELECT * FROM Items WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<User>(sql,new {Id=entity.Id });
            }
        }
    }
}
