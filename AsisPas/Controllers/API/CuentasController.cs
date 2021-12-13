using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace AsisPas.Controllers.API
{
    /// <summary>
    /// para hacer operaciones de usuario
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {


        #region ctor

        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
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
            IMapper mapper
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        #endregion



        #region login
        /// <summary>
        /// para ingresar al sistema
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult<UserToken>> login(UserLogin userInfo)
        {

            try {

                var result = await signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var usuario = await userManager.FindByEmailAsync(userInfo.Email);
                    var roles = await userManager.GetRolesAsync(usuario);

                    if (roles.Count < 1 || roles == null)
                        roles = new List<string>();

                    return new UserToken(userInfo, roles, configuration);
                   
                }

                return BadRequest("Usuario o contraseña no validos");

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new UserToken()
                {
                    ok = false,
                    Expiration = DateTime.Now.AddDays(-1),
                    Token = null
                };

            }

        }

        #endregion

        #region prueba
        [HttpGet]
        public async System.Threading.Tasks.Task GetBorrar()
        {
            DateTime inicio = new(2021, 11, 28);
            DateTime fin = new(2022, 1, 8);




            for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
            {
                if (i.DayOfWeek != DayOfWeek.Monday)
                {
                    int y = i.Year;
                    int m = i.Month;
                    int d = i.Day;

                    var datei = new DateTime(y, m, d, 9, 0, 30);
                    var datehi = new DateTime(y, m, d, 11, 5, 22);
                    var datehf = new DateTime(y, m, d, 13, 0, 30);
                    var datef = new DateTime(y, m, d, 15, 57, 50);


                    Marca obji = new Marca()
                    {
                        Empleadoid = 8,
                        Gate = "No Aplica",
                        Horarioid = 5,
                        marca = datei,
                        Sedeid = 1,
                        TipoIngreso = 0,

                    };
                    obji.Complete();
                    context.Add(obji);

                    Marca objdi = new Marca()
                    {
                        Empleadoid = 8,
                        Gate = "No Aplica",
                        Horarioid = 5,
                        marca = datehi,
                        Sedeid = 1,
                        TipoIngreso = 1,

                    };
                    objdi.Complete();
                    context.Add(objdi);


                    Marca objdf = new Marca()
                    {
                        Empleadoid = 8,
                        Gate = "No Aplica",
                        Horarioid = 5,
                        marca = datehf,
                        Sedeid = 1,
                        TipoIngreso = 2,

                    };
                    objdf.Complete();
                    context.Add(objdf);


                    Marca objf = new Marca()
                    {
                        Empleadoid = 8,
                        Gate = "No Aplica",
                        Horarioid = 5,
                        marca = datef,
                        Sedeid = 1,
                        TipoIngreso = 3,

                    };
                    objf.Complete();
                    context.Add(objf);

                }

            }
            await context.SaveChangesAsync();
        }

        #endregion




    }
}
