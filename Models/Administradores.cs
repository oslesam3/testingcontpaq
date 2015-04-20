using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ContpaqiApi.Models
{
    public class Administradores
    {
        public int AdminID { get; set; }
        [Required(ErrorMessage = "Ingrese nombre", AllowEmptyStrings = false)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Ingrese apellido", AllowEmptyStrings = false)]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Seleccione departamento", AllowEmptyStrings = false)]
        public string Departamento { get; set; }
        //public  IEnumerable<SelectListItem> Departamento { get; set; }

        [Required(ErrorMessage="Ingrese una dirección de correo válida",AllowEmptyStrings = false)]
        [EmailAddress]
        [Display(Name= "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage="Ingrese un nombre de usuario",AllowEmptyStrings =false)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(100,ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "La contraseña no coincide")]
        public string ConfirmPassword { get; set; }

        public bool CrearAdmin { get; set; }
        public bool Permisos { get; set; }
        public bool EnviarNotificaciones { get; set; }
        public bool CargarReportes { get; set; }
        public string Rol { get; set; }
        public bool Bloqueado { get; set; }
        public string Error { get; set; }
    }
}