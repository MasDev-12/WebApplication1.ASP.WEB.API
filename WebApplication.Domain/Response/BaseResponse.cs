using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Enum;

namespace WebApplication.Domain.Response
{
    public class BaseResponse<T> : IBaseResponse<T>//запрос в БД,
    {
        public string Description { get; set; }//название ошибки если случиться во время запроса в бд
        public StatusCodeEnum StatusCode { get; set; }
        public T Data { get; set; }//Данные, мы записываем результат запроса, манипуляция с данными
    }

    public interface IBaseResponse<T>
    {
        T Data { get; }
        StatusCodeEnum StatusCode { get; }
        public string Description { get; }

    }
}
