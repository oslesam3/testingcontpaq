using ContpaqiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ContpaqiApi.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        
        public ActionResult Index()
        {
            AdminReference.AdminServiceClient ar = new AdminReference.AdminServiceClient();
            if(Session["UsuarioLogueado"] != null)
            {
                return RedirectToAction("Index", "SuperAdministrador");
            }
            else
            {
                return View();
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel a)
        {
            
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            else
            {
                AdminReference.AdminServiceClient ar = new AdminReference.AdminServiceClient();
                string pass = EncriptarContraseña(a.Password);
                if (ar.LoginAdministradores(a.Usuario, pass) != null)
                {
                    if (ar.LoginAdministradores(a.Usuario, pass).Bloqueado != true)
                    {
                        if (ar.LoginAdministradores(a.Usuario, pass).Rol == "SuperAdministrador")
                        {
                            Session["UsuarioLogeado"] = ar.LoginAdministradores(a.Usuario, pass).Usuario.ToString();
                            Session["Rol"] = ar.LoginAdministradores(a.Usuario, pass).Rol.ToString();
                            return RedirectToAction("Index", "SuperAdministrador");
                        }
                        else if (ar.LoginAdministradores(a.Usuario, pass).Rol == "Administrador")
                        {
                            Session["UsuarioLogeado"] = ar.LoginAdministradores(a.Usuario, pass).Usuario.ToString();
                            Session["Rol"] = ar.LoginAdministradores(a.Usuario, pass).Rol.ToString();
                            return RedirectToAction("Administrador");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Este usuario ha sido bloqueado por el administrador");
                        return View("Index");
                    }
                }
            }
            ModelState.AddModelError("", "No existe el usuario seleccionado.");
            return View("Index");
        }

        public ActionResult SuperAdministrador()
        {
            if (Session["UsuarioLogeado"] != null)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                return View(sr.MostrarAdministradores());
            }
            else
            {
                return RedirectToAction("Index", "SuperAdministrador");
            }
        }

        public ActionResult Administrador()
        {
            if (Session["UsuarioLogeado"] != null)
            {

                return View("Logeado");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult Logout()
        {
            Session["UsuarioLogeado"] = null;
            return RedirectToAction("Index", "Acceso");
        }

        public string EncriptarContraseña(string pass)
        {
            SHA1CryptoServiceProvider sh = new SHA1CryptoServiceProvider();
            sh.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pass));
            byte[] re = sh.Hash;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in re)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

    }
}