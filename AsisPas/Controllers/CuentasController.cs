using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AsisPas.Controllers
{
    /// <summary>
    /// controlador para la manipulacion de cuentas a nivel visual
    /// </summary>
    public class CuentasController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;


        #region constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public CuentasController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }


        #endregion

        #region mi cuenta

        /// <summary>
        /// aqui devuelve los datos de usuario en base a mi correo electronico
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            return View();
        }

        #endregion

        #region logIn
        /// <summary>
        /// vista para ingresar al sistema
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<ActionResult> login()
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete("Token");
            if (User.Identity.IsAuthenticated)
                await signInManager.SignOutAsync();
            return View();
        }
        /// <summary>
        /// verifica si el usuario existe y le permite el paso a la app
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>

        public async System.Threading.Tasks.Task<ActionResult> Singin(UserLogin userInfo)
        {
            HttpClient client = new();
            try
            {
                UserToken tk;
                var result = await signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    var usuario = await userManager.FindByEmailAsync(userInfo.Email);
                    var roles = await userManager.GetRolesAsync(usuario);

                    if (roles.Count < 1 || roles == null)
                        roles = new List<string>();

                    tk = new UserToken(userInfo, roles, configuration);
                    HttpContext.Response.Cookies.Append("Token", tk.Token, new Microsoft.AspNetCore.Http.CookieOptions()
                    {
                        Expires = tk.Expiration
                    });

                    await signInManager.PasswordSignInAsync(usuario, userInfo.Password, false, false);
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Err = "El correo o la contraseña no son correctos.";
                return View("login");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\n HTTP  Exception Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                ViewBag.Err = "El correo o la contraseña no son correctos.";
                return View("login");
            }
            finally
            {
                client.Dispose();

            }

        }
        /// <summary>
        /// para salir del sistema
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete("Token");
            await signInManager.SignOutAsync();
            Response.Redirect("Login");
            return View("Login");
        }

        #endregion

        #region Crear usuarios Administradores de empresas

        /// <summary>
        /// para ver un listado de los administradores de empresas
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> IndexEmpresas()
        {
            try
            {
                var ent = await context.AdmoEmpresas
                    .Include(x => x.Empresa)
                    .Include(x => x.user)
                    .Where(x => x.act == true).ToListAsync();

                
                return View(ent);  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        /// <summary>
        /// para crear un nuevo usuario Administrador de empresas
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> AdmoEmpresa()
        {

            try
            {
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context,User));
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para Crear Usuarios";
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context,User));
                return View();
            }

        }
        /// <summary>
        /// para guarda al nuevo usuario administrador de empresa
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> GuardarAdmoEmpresa(AdmoEmpresaDTO_in ins)
        {
            try
            {
                var usuario = await userManager.FindByEmailAsync(ins.Email);
                if (usuario == null)
                {
                    var UsEnt = mapper.Map<Usuario>(ins);
                    UsEnt.Complete();
                    var user = new IdentityUser { UserName = ins.Email, Email = ins.Email };
                    var result = await userManager.CreateAsync(user, ins.Password);
                    if (result.Succeeded)
                    {
                        usuario = await userManager.FindByEmailAsync(ins.Email);
                        UsEnt.userid = usuario.Id;

                        context.Add(UsEnt);
                        await context.SaveChangesAsync();

                        AdmoEmpresas last = new()
                        {
                            Empresaid = ins.Empresaid,
                            act = true,
                            userid = UsEnt.id,
                        };

                        context.Add(last);
                        await context.SaveChangesAsync();
                        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Empresa"));

                        return RedirectToAction("IndexEmpresas");
                    }

                }
                ViewBag.Empresas = Empresa.toSelect(await context.Empresas.Where(x => x.act == true).ToListAsync());
                ViewBag.Err = "Este Usuario Ya Existe";
                return View("AdmoEmpresa", ins);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Empresas = Empresa.toSelect(await context.Empresas.Where(x => x.act == true).ToListAsync());
                ViewBag.Err = "Parece que algo no esta bien.. intente mas tarde";
                return View("AdmoEmpresa", ins);
            }
        }

        #endregion

        #region fiscalizador

        /// <summary>
        /// para ver un listado de los Fiscalizadores
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Fiscalizadores()
        {
            try
            {
                var ent = await context.Fiscales
                    .Include(x => x.Empresa)
                    .Include(x => x.user)
                    .Where(x => x.act == true).ToListAsync();


                return View(ent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        /// <summary>
        /// para crear un nuevo usuario Fiscalizadores
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> CrearFiscalizador()
        {

            try
            {
                ViewBag.Empresas = ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para Crear Usuarios";
                return View();
            }

        }
        /// <summary>
        /// para guarda al nuevo usuario Fiscalizadores
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> GuardarFiscalizador(AdmoEmpresaDTO_in ins)
        {
            try
            {
                var usuario = await userManager.FindByEmailAsync(ins.Email);
                if (usuario == null)
                {
                    var UsEnt = mapper.Map<Usuario>(ins);
                    UsEnt.Complete();
                    var user = new IdentityUser { UserName = ins.Email, Email = ins.Email };
                    var result = await userManager.CreateAsync(user, ins.Password);
                    if (result.Succeeded)
                    {
                        usuario = await userManager.FindByEmailAsync(ins.Email);
                        UsEnt.userid = usuario.Id;

                        context.Add(UsEnt);
                        await context.SaveChangesAsync();

                        Fiscal last = new()
                        {
                            Empresaid = ins.Empresaid,
                            act = true,
                            userid = UsEnt.id,
                        };

                        context.Add(last);
                        await context.SaveChangesAsync();
                        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Fiscal"));

                        return RedirectToAction("Fiscalizadores");
                    }

                }
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                ViewBag.Err = "Este Usuario Ya Existe";
                return View("CrearFiscalizador", ins);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                ViewBag.Err = "Parece que algo no esta bien.. intente mas tarde";
                return View("CrearFiscalizador", ins);
            }
        }

        #endregion

        #region Empleado
        /// <summary>
        /// para ver un listado de los Fiscalizadores
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Empleados()
        {
            try
            {
                var ent = await Empleado.EmpleadosXUsuario(context, User);  
                return View(ent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        /// <summary>
        /// para crear un nuevo usuario Fiscalizadores
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> CrearEmpleado()
        {

            try
            {
                ViewBag.Empresas = ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para Crear Usuarios";
                return View();
            }

        }
        /// <summary>
        /// para guarda al nuevo usuario Fiscalizadores
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> GuardarEmpleado(AdmoEmpresaDTO_in ins)
        {
            try
            {
                var usuario = await userManager.FindByEmailAsync(ins.Email);
                if (usuario == null)
                {
                    var UsEnt = mapper.Map<Usuario>(ins);
                    UsEnt.Complete();
                    var user = new IdentityUser { UserName = ins.Email, Email = ins.Email };
                    var result = await userManager.CreateAsync(user, ins.Password);
                    if (result.Succeeded)
                    {
                        usuario = await userManager.FindByEmailAsync(ins.Email);
                        UsEnt.userid = usuario.Id;

                        context.Add(UsEnt);
                        await context.SaveChangesAsync();

                        Empleado last = new()
                        {
                            Empresaid = ins.Empresaid,
                            act = true,
                            userid = UsEnt.id,
                        };

                        context.Add(last);
                        await context.SaveChangesAsync();
                        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Empleado"));

                        return RedirectToAction("Empleados");
                    }

                }
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                ViewBag.Err = "Este Usuario Ya Existe";
                return View("CrearEmpleado", ins);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                ViewBag.Err = "Parece que algo no esta bien.. intente mas tarde";
                return View("CrearEmpleado", ins);
            }
        }


        #endregion

        #region admo sistema

        /// <summary>
        /// para ver un listado de los Fiscalizadores
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> AdmoSistema()
        {
            if (User.IsInRole("SuperAdmin"))
            {
                try
                {
                    var ent = await context.AdmoSistema.Include(x => x.user).Where(x => x.act == true).ToListAsync();
                    return View(ent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return View();
                }
            }
            return RedirectToAction("Logout");
        }


        /// <summary>
        /// para crear un nuevo usuario Fiscalizadores
        /// </summary>
        /// <returns></returns>
        public IActionResult CrearAdmoSistema()
        {
            if (User.IsInRole("SuperAdmin"))
            {
                   try
               {
                   return View();
               }
               catch (Exception ex)
               {
                   Console.WriteLine(ex.Message);
                   ViewBag.Err = "upss, parece que no tienes permisos para Crear Usuarios";
                   return View();
               }
            }
            return RedirectToAction("Logout");
        }
        /// <summary>
        /// para guarda al nuevo usuario Fiscalizadores
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> GuardarAdmoSistema(UsuarioDTO_in ins)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                try
            {

                var usuario = await userManager.FindByEmailAsync(ins.Email);
                if (usuario == null)
                {
                    var UsEnt = mapper.Map<Usuario>(ins);
                    UsEnt.Complete();
                    var user = new IdentityUser { UserName = ins.Email, Email = ins.Email };
                    var result = await userManager.CreateAsync(user, ins.Password);
                    if (result.Succeeded)
                    {
                        usuario = await userManager.FindByEmailAsync(ins.Email);
                        UsEnt.userid = usuario.Id;

                        context.Add(UsEnt);
                        await context.SaveChangesAsync();

                        AdmoSistema last = new()
                        {
                            act = true,
                            userid = UsEnt.id,
                        };

                        context.Add(last);
                        await context.SaveChangesAsync();
                        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Admin"));

                        return RedirectToAction("AdmoSistema");
                    }



                }
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                ViewBag.Err = "Este Usuario Ya Existe";
                return View("CrearAdmoSistema", ins);
           
        }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Empresas = Empresa.toSelect(await Empresa.FiltrarEmpresas(context, User));
                ViewBag.Err = "Parece que algo no esta bien.. intente mas tarde";
                return View("CrearAdmoSistema", ins);
            }
            }
            return RedirectToAction("Logout");
        }


        #endregion

    }
}
