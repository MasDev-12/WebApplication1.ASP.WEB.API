using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<bool> Create(T entity);
        IEnumerable<T> GetAll();
        Task<bool> Delete(T entity);
        Task<T> Update(T entity);
        Task<T> GetById(int id);
    }
}
