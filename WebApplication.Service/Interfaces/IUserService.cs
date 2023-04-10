using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.User;

namespace WebApplication.Service.Interfaces
{
    public interface IUserService
    {
        IBaseResponse<IEnumerable<UserViewModel>> GetUsers();
        Task<IBaseResponse<UserViewModel>> GetUserById(int id);
        Task<IBaseResponse<bool>> DeleteUser(int id);

        Task<IBaseResponse<bool>> CreateUser(User user);
    }
}
