using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcademiaWeb.Identity.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email é inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatório")]       
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
