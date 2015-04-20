using ContpaqiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ContpaqiApi.Controllers
{
    public class SuperAdministradorController : Controller
    {
        public string EncriptarContraseña(string pass)
        {
            SHA1CryptoServiceProvider sh = new SHA1CryptoServiceProvider();
            sh.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pass));
            byte[] re = sh.Hash;
            StringBuilder sb = new StringBuilder();
            foreach(byte b in re)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        // GET: SuperAdministrador
        public ActionResult Index()
        {
            return RedirectToAction("GestionAdministradores");
        }

        public ActionResult GestionAdministradores()
        {
            if (Session["UsuarioLogeado"] != null)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                return View(sr.MostrarAdministradores());
            }
            else
            {
                return RedirectToAction("Index","Acceso");
            }
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
                return RedirectToAction("Index", "Acceso");
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

                if (sr.VerificarAdministrador(a.Usuario) == "Existe")
                {
                    ModelState.AddModelError("", "El usuario ya ha sido tomado");
                    IEnumerable<SelectListItem> cambio = sr.MostrarDeptos().Select(c => new SelectListItem { Text = c.Nombre });
                    ViewBag.Depos = cambio;
                    return View(a);
                }
                else
                {
                    ad.AdminID = a.AdminID;
                    ad.Nombre = a.Nombre;
                    ad.Apellido = a.Apellido;
                    ad.Departamento = a.Departamento.ToString();
                    ad.Email = a.Email;
                    ad.Usuario = a.Usuario;
                    ad.Password = EncriptarContraseña(a.Password);
                    ad.ConfirmarPassword = EncriptarContraseña(a.ConfirmPassword);
                    ad.Bloqueado = a.Bloqueado;
                    ad.CargarReportes = a.CargarReportes;
                    ad.CrearAdmin = a.CrearAdmin;
                    ad.EnviarNotificaciones = a.EnviarNotificaciones;
                    ad.Permisos = a.Permisos;
                    if (a.Permisos || a.CrearAdmin)
                    {
                        ad.Rol = "SuperAdministrador";
                    }
                    else
                    {
                        ad.Rol = "Administrador";
                    }

                    if(sr.AgregarNuevo(ad) == "Agregado")
                    {
                        return RedirectToAction("GestionAdministradores");
                    }
                    else
                    {
                        IEnumerable<SelectListItem> cambio = sr.MostrarDeptos().Select(c => new SelectListItem { Text = c.Nombre });
                        ViewBag.Depos = cambio;
                        ModelState.AddModelError("", "Un error ha ocurrido, no se pudo guardar el nuevo administrador");
                        return View(a);
                    }
                }
            }
            catch (Exception e)
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
                        Nombre = p.Nombre,
                        Usuario = p.Usuario,
                        Departamento = p.Departamento
                    }
                );
            return View("GestionAdministradores", ad);
        }                   

        [HttpGet]
        public ActionResult EditarAdministrador(string id)
        {
            //if (Session["UsuarioLogeado"] != null)
            //{
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();

                IEnumerable<SelectListItem> items = sr.MostrarDeptos().Select
                    (
                        c => new SelectListItem
                        {
                            Text = c.Nombre
                        }
                    );
                ViewBag.Depos = items;

                Administradores ad = new Administradores();
                foreach (var item in sr.BuscarAdministrador(id))
                {
                    ad.AdminID = item.AdminID;
                    ad.Nombre = item.Nombre;
                    ad.Apellido = item.Apellido;
                    ad.Departamento = item.Departamento;
                    ad.Email = item.Email;
                    ad.Usuario = item.Usuario;
                    ad.Password = item.Password;
                    ad.CrearAdmin = item.CrearAdmin;
                    ad.Permisos = item.Permisos;
                    ad.CargarReportes = (bool)item.CargarReportes;
                    ad.EnviarNotificaciones = item.EnviarNotificaciones;
                    ad.Rol = item.Rol;
                    ad.Bloqueado = (bool)item.Bloqueado;
                }                
                return View(ad);
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Acceso");
            //}
        }

        [HttpPost]
        public ActionResult EditarAdministrador(Administradores admin)
        {
            if (!ModelState.IsValid)
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
                ModelState.AddModelError("", "Verifique los campos incorrectos");
                return View(admin);
            }
            try
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                AdminReference.Administradores toEdit = new AdminReference.Administradores();
                toEdit.AdminID = admin.AdminID;
                toEdit.Nombre = admin.Nombre;
                toEdit.Apellido = admin.Apellido;
                toEdit.Departamento = admin.Departamento.ToString();
                toEdit.Email = admin.Email;
                toEdit.Usuario = admin.Usuario;
                toEdit.Password = EncriptarContraseña(admin.Password);
                toEdit.ConfirmarPassword = EncriptarContraseña(admin.ConfirmPassword);
                toEdit.CargarReportes = admin.CargarReportes;
                toEdit.EnviarNotificaciones = admin.EnviarNotificaciones;
                toEdit.Permisos = admin.Permisos;
                toEdit.CrearAdmin = admin.CrearAdmin;
                toEdit.Bloqueado = admin.Bloqueado;
                if (admin.Permisos || admin.CrearAdmin)
                {
                   toEdit.Rol = "SuperAdministrador";
                }
                else
                {
                    toEdit.Rol = "Administrador";
                }
                sr.EditarAdministrador(toEdit);
                return RedirectToAction("GestionAdministradores");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Verifique los campos incorrectos");
                return View(admin);
            }
        }

        [HttpGet]
        public ActionResult Eliminar(string id,string d)
        {
            if (Session["UsuarioLogeado"] != null)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                Administradores ad = new Administradores();
                foreach (var item in sr.BuscarAdministrador(id))
                {
                    ad.AdminID = item.AdminID;
                    ad.Nombre = item.Nombre;
                    ad.Apellido = item.Apellido;
                    ad.Departamento = item.Departamento;
                    ad.Email = item.Email;
                    ad.Usuario = item.Usuario;
                    ad.Password = item.Password;
                    ad.CrearAdmin = item.CrearAdmin;
                    ad.Permisos = item.Permisos;
                    ad.Rol = item.Rol;
                    ad.CargarReportes = (bool)item.CargarReportes;
                    ad.EnviarNotificaciones = item.EnviarNotificaciones;
                    ad.Bloqueado = (bool)item.Bloqueado;
                }
                return PartialView("EliminarAdmin",ad);
            }
            else
            {
                return RedirectToAction("Index", "Acceso");
            }
        }

        [HttpPost]
        public ActionResult Eliminar(string id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                sr.EliminarAdministrador(id);
                return RedirectToAction("GestionAdministradores");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Verifique los campos incorrectos");
                return View(id);
            }
        }

        [HttpGet]
        public ActionResult Bloquear(string id)
        {
            if (Session["UsuarioLogeado"] != null)
            {
                AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
                Administradores ad = new Administradores();
                foreach (var item in sr.BuscarAdministrador(id))
                {
                    ad.AdminID = item.AdminID;
                    ad.Nombre = item.Nombre;
                    ad.Apellido = item.Apellido;
                    ad.Departamento = item.Departamento;
                    ad.Email = item.Email;
                    ad.Usuario = item.Usuario;
                    ad.Password = item.Password;
                    ad.CrearAdmin = item.CrearAdmin;
                    ad.Permisos = item.Permisos;
                    ad.Rol = item.Rol;
                    ad.CargarReportes = (bool)item.CargarReportes;
                    ad.EnviarNotificaciones = item.EnviarNotificaciones;
                    ad.Bloqueado = (bool)item.Bloqueado;
                }
                return PartialView(ad);
            }
            else
            {
                return RedirectToAction("Index", "Acceso");
            }
        }

        [HttpPost]
        public ActionResult Bloquear(string id,string usuario)
        {
            AdminReference.AdminServiceClient sr = new AdminReference.AdminServiceClient();
            if (sr.BloquearAdministrador(id) == "Bloqueado")
            {
                return RedirectToAction("GestionAdministradores");
            }
            else
            {
                ModelState.AddModelError("","Ha ocurrido un problema");
                return PartialView("Bloquear");
            }
        }
    }
}