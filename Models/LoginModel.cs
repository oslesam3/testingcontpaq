using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContpaqiApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage="Ingrese un nombre de usuario", AllowEmptyStrings = false)]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Ingrese la contraseña del usuario", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Mensaje { get; set; }
    }
}