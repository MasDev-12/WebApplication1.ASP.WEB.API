using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Domain.Enum
{
    public enum StatusCodeEnum
    {
        
        //общее
        NotFound = 0,
        Found= 1,
        PasswordWrong=2,
        OK = 200,
        InternalServerError = 500//Ошибка на стороне нашего сервеса
    }
}
