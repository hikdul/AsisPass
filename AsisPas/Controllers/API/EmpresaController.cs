using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsisPas.Controllers.API
{
    /// <summary>
    /// controlador api para los empresas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmpresaController : CustomBaseController
    {
        #region constructor

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public EmpresaController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        #endregion


        #region POST

        /// <summary>
        /// para agregar una nueva empresa
        /// </summary>
        /// <param name="insert"></param>
        /// <returns></returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult<EmpresaDTO>> Post([FromForm] empresaDTO_in insert)
        {
            try
            {
                return Ok(await Post<empresaDTO_in, Empresa,EmpresaDTO>(insert));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex);
                return BadRequest("no es posible almacenar la empreso");
            }
        }

        #endregion

        #region put
/// <summary>
/// para actualizar los datos de una empresa
/// </summary>
/// <param name="id"></param>
/// <param name="ins"></param>
/// <returns></returns>
        [HttpPut("{id:int}")]
        public async System.Threading.Tasks.Task<ActionResult> Put(int id, [FromForm] empresaDTO_in ins)
        {
            try
            {
                return await Put<empresaDTO_in, Empresa>(id,ins);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex);
                return BadRequest("no es posible actualizar la empreso");
            }

        }

        #endregion

        #region DELETE
        /// <summary>
        /// para eliminar un elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task<ActionResult> Delete(int id) 
        {
            try
            {
                return await Delete<Empresa>(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex);
                return BadRequest("no es posible borrar la empreso");
            }

        }
        #endregion

        #region Getter's
        /// <summary>
        /// para obtener una lista de elementos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult<List<EmpresaDTO>>> GetElement()
        {
            try
            {
                List<EmpresaDTO> Resp= new();
                if (User.IsInRole("Admin"))
                    return await GetAll<Empresa,EmpresaDTO>();
                
                return Ok(Resp);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex);
                return BadRequest("Algo no esta bien");
            }
        }

        /// <summary>
        /// para obtener una empresa mediante su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async System.Threading.Tasks.Task<ActionResult<EmpresaDTO>> Get(int id)
        {
            try
            {
                return Ok( await Get<Empresa,EmpresaDTO>(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex);
                return BadRequest("Algo no esta bien");
            }
        }

        #endregion

    }
}
