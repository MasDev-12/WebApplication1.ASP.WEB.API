using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;

namespace WebApplication.DAL.Interfaces
{
    public interface IRegionRepository:IBaseRepository<Region>
    {
        public Task<bool> CheckRegionExistsAsync(string name, int? parentId);
    }
}
