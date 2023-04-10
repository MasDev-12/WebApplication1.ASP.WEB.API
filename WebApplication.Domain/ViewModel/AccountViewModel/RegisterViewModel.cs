using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Domain.ViewModel.AccountViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(50, ErrorMessage = "Имя должно иметь длину меньше 50 символов")]
        [MinLength(2, ErrorMessage = "Имя должно иметь длину больше 2 символов")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [MaxLength(50, ErrorMessage = "Имя должно иметь длину меньше 50 символов")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
