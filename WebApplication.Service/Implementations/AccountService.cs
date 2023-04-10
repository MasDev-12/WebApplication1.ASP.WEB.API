using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.DAL.Repositories;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Enum;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.AccountViewModel;
using WebApplication.Domain.ViewModel.User;
using WebApplication.Service.Interfaces;

namespace WebApplication.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<User> _userRepository;

        public AccountService(IBaseRepository<User> userRepository)
        {
            this._userRepository=userRepository;
        }
        public async Task<BaseResponse<UserViewModel>> LoginUser(LoginViewModel loginViewModel)
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email==loginViewModel.Email);
            if (user==null)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description="Пользователь найден",
                    StatusCode=StatusCodeEnum.Found,
                    Data=null
                };
            }
            if (user.Password!=loginViewModel.Password)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description="Неверный пароль",
                    StatusCode=StatusCodeEnum.PasswordWrong,
                    Data=null
                };
            }
            var userViewModel = new UserViewModel()
            {
                Email=user.Email,
                Password=user.Password,
                Login = user.Login,
                Name = user.Name,
                Role = user.Role.ToString(),
            };
            return new BaseResponse<UserViewModel>()
            {
                Data= userViewModel,
                StatusCode=StatusCodeEnum.OK
            };
        }

        public async Task<BaseResponse<User>> RegisterUser(RegisterViewModel registerViewModel)
        {
            try
            {
                var user = _userRepository.GetAll().FirstOrDefault(u => u.Email==registerViewModel.Email);
                if (user!=null)
                {
                    return new BaseResponse<User>()
                    {
                        Description="Пользователь с таким email уже есть",
                        Data=null,
                        StatusCode=StatusCodeEnum.Found
                    };
                }

                user = new User()
                {
                     Login=registerViewModel.Login,
                     Email=registerViewModel.Email,
                     Name=registerViewModel.Name,
                     Password = registerViewModel.Password,
                     Role = Role.Customer
                };

                await _userRepository.Create(user);
               

                return new BaseResponse<User>()
                {
                    Data = user,
                    Description = "Пользователь добавился",
                    StatusCode = StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {

                return new BaseResponse<User>()
                {
                    Description= $"[RegisterUser]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        
    }
}
