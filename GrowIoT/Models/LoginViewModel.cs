using System.ComponentModel.DataAnnotations;

namespace GrowIoT.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email не может быть пустым"), DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Пароль не может быть пустым"), DataType(DataType.Password)]
        public string Password { get; set; }
        
        public string Error { get; set; }
    }
}
