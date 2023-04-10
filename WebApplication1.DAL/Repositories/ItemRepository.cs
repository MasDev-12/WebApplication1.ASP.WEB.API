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
    public class ItemRepository : IBaseRepository<Item>
    {
        private readonly IDapperConnection _dapperConnection;

        public ItemRepository(IDapperConnection dapperConnection) 
        {
            _dapperConnection=dapperConnection;
        }
        public async Task<bool> Create(Item entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "INSERT INTO Items (Name, Price) VALUES (@Name, @Price)";
                var result = await connection.ExecuteAsync(sql, entity);
                return result > 0 ? true : false;
            }
        }

        public async Task<bool> Delete(Item entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "DELETE FROM Items WHERE Id=@Id";
                var result = await connection.ExecuteAsync(sql,entity);
                return result>0?true:false;
            }
        }

        public IEnumerable<Item> GetAll()
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "SELECT * FROM Items";
                return connection.Query<Item>(sql);
            }
        }

        public async Task<Item> GetById(int id)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "SELECT * FROM Items WHERE Id = @Id";
                var result = await connection.QueryFirstOrDefaultAsync<Item>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<Item> Update(Item entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string sql = "UPDATE Items SET Name = @Name, Price = @Price WHERE Id = @Id";
                await connection.ExecuteAsync(sql, entity);
                sql = "SELECT * FROM Items WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<Item>(sql,new {Id=entity.Id });
            }
        }
    }
}
