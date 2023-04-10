using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.AccountViewModel;
using WebApplication.Domain.ViewModel.User;

namespace WebApplication.Service.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<User>> RegisterUser(RegisterViewModel registerViewModel);
        Task<BaseResponse<UserViewModel>> LoginUser(LoginViewModel loginViewModel);
    }
}
