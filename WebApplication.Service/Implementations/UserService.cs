using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Enum;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.User;
using WebApplication.Service.Interfaces;

namespace WebApplication.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;

        public UserService(IBaseRepository<User> userRepository)
        {
            _userRepository=userRepository;
        }

        public async Task<IBaseResponse<bool>> CreateUser(User user)
        {
            try
            {
                await _userRepository.Create(user);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="Пользователь создан",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[CreateUser]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.GetById(id);
                if (user==null)
                {
                    return new BaseResponse<bool>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=false,
                    };
                }
                await _userRepository.Delete(user);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="Пользователь найден и удален",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[DeleteUser]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
            
        }

        public async Task<IBaseResponse<UserViewModel>> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetById(id);
             
                if (user==null)
                {
                    return new BaseResponse<UserViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                var userViewModel = new UserViewModel()
                {
                    Email=user.Email,
                    Login=user.Login,
                    Name=user.Name,
                    Password=user.Password,
                    Role=user.Role.ToString(),
                };
                return new BaseResponse<UserViewModel>()
                {
                    Data=userViewModel,
                    Description="Пользователь найден",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description= $"[DeleteUser]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError,
                    Data=null
                };
            }
        }

        public IBaseResponse<IEnumerable<UserViewModel>> GetUsers()
        {
            try
            {
                var users = _userRepository.GetAll()
                    .Select(x => new UserViewModel()
                    {

                        Name = x.Name,
                        Login =x.Login,
                        Email=x.Email,
                        Password=x.Password,
                        Role=x.Role.ToString(),
                    });

                return new BaseResponse<IEnumerable<UserViewModel>>()
                {
                    Data= users,
                    StatusCode=Domain.Enum.StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<UserViewModel>>()
                {

                    StatusCode=Domain.Enum.StatusCodeEnum.InternalServerError,
                    Description=$"Ошибка подключения + {ex.Message}"

                };
            }
        }
    }
}
