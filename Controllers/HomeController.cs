using ContpaqiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContpaqiApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel a)
        {
            if (!ModelState.IsValid)
            {
                 return View(a);
            }
            else
            {
                AdminReference.AdminServiceClient ar = new AdminReference.AdminServiceClient();
                if (ar.LoginAdministradores(a.Usuario, a.Password) != null)
                {
                    if (ar.LoginAdministradores(a.Usuario, a.Password).Rol == "SuperAdministrador")
                    {
                        Session["UsuarioLogeado"] = ar.LoginAdministradores(a.Usuario, a.Password).Usuario.ToString();
                        return RedirectToAction("SuperLogeado");
                    }
                    else if (ar.LoginAdministradores(a.Usuario, a.Password).Rol == "Administrador")
                    {
                        Session["UsuarioLogeado"] = ar.LoginAdministradores(a.Usuario, a.Password).Usuario.ToString();
                        return RedirectToAction("AdminLogeado");
                    }
                }
            }
            ModelState.AddModelError("", "No existe el usuario");
            return View(a);
        }


        public ActionResult SuperLogeado()
        {
            if (Session["UsuarioLogeado"] != null)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                return View(sr.MostrarAdministradores());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AdminLogeado()
        {
            if (Session["UsuarioLogeado"] !=  null)
            {
                
                return View("Logeado");
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult LogOut()
        {
            Session["UsuarioLogeado"] = null;
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public ActionResult AgregarNuevo()
        {
            if (Session["UsuarioLogeado"] != null)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                IEnumerable<SelectListItem> items = sr.MostrarDeptos().Select
                    (
                        c => new SelectListItem
                        {
                            Text = c.Nombre
                        }
                    );
                ViewBag.Depos = items;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult AgregarNuevo(Administradores a)
        {
            if (!ModelState.IsValid)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                IEnumerable<SelectListItem> cambio = sr.MostrarDeptos().Select(c => new SelectListItem { Text = c.Nombre });
                ViewBag.Depos = cambio;
                return View(a);
            }
            try
            { 
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                AdminReference.Administradores ad = new AdminReference.Administradores();
                ad.AdminID = a.AdminID;
                ad.Nombre = a.Nombre;
                ad.Apellido = a.Apellido;
                ad.Departamento = a.Departamento.ToString();
                ad.Email = a.Email;
                ad.Usuario = a.Usuario;
                ad.Password = a.Password;
                sr.AgregarNuevo(ad);

                return RedirectToAction("SuperLogeado","Home");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Verifique los campos incorrectos");
                return View(a);
            }
        }

        [HttpPost]
        public ActionResult BuscarAdministrador(string valor)
        {
            AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
            IEnumerable<AdminReference.Administradores> ad = sr.BuscarAdministrador(valor).Select
                (
                    p => new AdminReference.Administradores 
                    { 
                        Nombre = p.Nombre, Usuario = p.Usuario,Departamento=p.Departamento
                    }
                );
            return View("SuperLogeado", ad);
        }
    }
}
