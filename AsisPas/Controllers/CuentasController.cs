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
using System.Threading.Tasks;

namespace AsisPas.Controllers
{
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
        public async Task<ActionResult> login()
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

        public async Task<ActionResult> Singin(UserLogin userInfo)
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
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete("Token");
            await signInManager.SignOutAsync();
            Response.Redirect("Login");
            return View("Login");
        }

        #endregion


        #region Crear usuarios

        /// <summary>
        /// para crear un nuevo usuario Administrador de empresas
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AdmoEmpresa()
        {

            try
            {
                ViewBag.Empresas = Empresa.toSelect(await context.Empresas.Where(x => x.act == true).ToListAsync());
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para Crear Usuarios";
                return View();
            }

        }


        #endregion

    }
}
