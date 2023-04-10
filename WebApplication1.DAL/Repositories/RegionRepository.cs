using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.Domain.Entity;
using System.Data.SqlClient;

namespace WebApplication.DAL.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly IDapperConnection _dapperConnection;

        public RegionRepository(IDapperConnection dapperConnection) 
        {
            _dapperConnection=dapperConnection;
        }
        public async Task<bool> Create(Region entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string SqlQuery = "INSERT INTO Regions (Name, ParentId) VALUES (@Name, @ParentId)";
                var result = await connection.ExecuteAsync(SqlQuery, entity);
                return result > 0 ? true : false;
            }
        }

        public async Task<bool> Delete(Region entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var region = await GetById(entity.Id);

                    if (region == null)
                    {
                        return false;
                    }

                    // Удаляем регион и все его дочерние регионы
                    var result = await DeleteRecursive(region.Id, connection, transaction);

                    if (result)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return result;
                }
            }
        }

        private async Task<bool> DeleteRecursive(int regionId, IDbConnection connection, IDbTransaction transaction)
        {
            // Удаляем дочерние регионы
            var children = await connection.QueryAsync<Region>("SELECT * FROM Regions WHERE ParentId = @RegionId", new { RegionId = regionId }, transaction);
            foreach (var child in children)
            {
                await DeleteRecursive(child.Id, connection, transaction);
            }

            // Удаляем родительский регион
            var result = await connection.ExecuteAsync("DELETE FROM Regions WHERE Id = @RegionId", new { RegionId = regionId }, transaction);
            return result > 0;
        }

        public IEnumerable<Region> GetAll()
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string query = "SELECT * FROM Regions";
                var regions = connection.Query<Region>(query);
                var parentRegions = regions.Where(r => r.ParentId == null).ToList();
                foreach (var parent in parentRegions)
                {
                    parent.Children = GetChildren(regions, parent.Id);
                }
                return regions.Where(r => r.ParentId == null).ToList();
            }
        }

        public async Task<Region> Update(Region entity)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var region = await GetById(entity.Id);

                    if (region == null)
                    {
                        return null;
                    }

                    // Обновляем поля региона
                    
                    region.Name = entity.Name;
                    region.ParentId = entity.ParentId;

                    var sql = @"
                UPDATE Regions SET
                    Name = @Name,
                    ParentId = @ParentId
                WHERE Id = @Id;
            ";

                    // Обновляем запись в БД
                    var result = await connection.ExecuteAsync(sql, region, transaction);

                    if (result > 0)
                    {
                        transaction.Commit();
                        return region;
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return null;
                }
            }
        }

        public async Task<Region> GetById(int id)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string query = "SELECT * FROM Regions WHERE Id = @Id";
                var region = await connection.QueryFirstOrDefaultAsync<Region>(query, new { Id = id });
                if (region == null)
                {
                    return null;
                }
                region.Children = GetChildren(GetAllForGetById(), region.Id);
                return region;
            }
        }

        private IEnumerable<Region> GetAllForGetById()
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string query = "SELECT * FROM Regions";
                var regions = connection.Query<Region>(query);
                return regions;
            }
        }

        private static List<Region> GetChildren(IEnumerable<Region> regions, int parentId)
        {
            var children = regions.Where(r => r.ParentId == parentId).ToList();
            foreach (var child in children)
            {
                child.Children = GetChildren(regions, child.Id);
            }
            return children;
        }

        public async Task<bool> CheckRegionExistsAsync(string name, int? parentId)
        {
            using (var connection = _dapperConnection.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Regions WHERE Name = @Name AND ParentId = @ParentId";
                var result = await connection.ExecuteScalarAsync<int>(query, new { Name = name, ParentId = parentId });
                return result > 0 ? true : false;
            }
        }
    }
}
