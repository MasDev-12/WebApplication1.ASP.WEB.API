using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.Domain.Entity;

namespace WebApplication.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDapperConnection _dapperConnection;

        public OrderRepository(IDapperConnection dapperConnection)
        {
            _dapperConnection=dapperConnection;
        }
        public async Task<bool> Create(Order entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                var query = "INSERT INTO Orders (OrderDate, RegionId, ItemId, Amount) VALUES (@OrderDate, @RegionId, @ItemId, @Amount);";
                var result = await connection.ExecuteAsync(query, entity);
                return result>0 ? true : false;
            }
        }

        public async Task<bool> Delete(Order entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "DELETE FROM Orders WHERE id = @id";
                        var result = await connection.ExecuteAsync(query, new { id = entity.Id }, transaction);
                        transaction.Commit();
                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                       
                        return false;
                    }
                }
            }
        }

        public IEnumerable<Order> GetAll()
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                var query = "SELECT * FROM Orders";
                var orders = connection.Query<Order>(query);
                return orders;
            }
        }

        public async Task<Order> GetById(int id)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                var query = "SELECT * FROM Orders WHERE id = @id";
                var order = await connection.QueryFirstOrDefaultAsync<Order>(query, new { id });
                return order;
            }
        }

        public async Task<Order> Update(Order entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                var query = "UPDATE Orders SET OrderDate = @OrderDate, RegionId = @RegionId, ItemId = @ItemId, amount = @Amount WHERE id = @Id";
                var result = await connection.ExecuteAsync(query, entity);

                if (result > 0)
                {
                    return entity;
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<IEnumerable<Order>> GetOrdersByPagitation(int pageSize, int pageNumber, string searchTerm)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@SearchTerm", searchTerm);

                var result = await connection.QueryAsync<Order>("GetOrders", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
