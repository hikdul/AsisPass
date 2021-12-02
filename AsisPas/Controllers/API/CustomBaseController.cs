using AsisPas.Data;
using AsisPas.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsisPas.Controllers.API
{
    /// <summary>
    /// clase para disminuir la escrituro de elementos en controller base 
    /// </summary>
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        /// <summary>
        /// contructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #region GET
        /// <summary>
        /// obtiene todo lo existente en base de datos
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <returns></returns>
        protected async Task<List<TDTO>> GetAll<TEntidad, TDTO>() where TEntidad : class
        {
            var ent = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
            return mapper.Map<List<TDTO>>(ent);
        }
      
        /// <summary>
        /// obtiene los datos activos en base de datos
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <returns></returns>
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class, IAct
        {
            var ent = await context.Set<TEntidad>().Where(i => i.act == true).AsNoTracking().ToListAsync();
            return mapper.Map<List<TDTO>>(ent);
        }

        /// <summary>
        /// obtiene un dato por medio de su id
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<TDTO> Get<TEntidad, TDTO>(int id) where TEntidad : class, Iid
        {
            var ent = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.id == id);
            return mapper.Map<TDTO>(ent);
        }
      


        #endregion

        #region POST
        /// <summary>
        /// new
        /// </summary>
        /// <typeparam name="T_DTO_in"></typeparam>
        /// <typeparam name="T_entidad"></typeparam>
        /// <typeparam name="T_DTO"></typeparam>
        /// <param name="_In"></param>
        /// <param name="NombreRuta"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Post<T_DTO_in, T_entidad, T_DTO>(T_DTO_in _In, string NombreRuta) where T_entidad : class, Iid, IAct
        {
            var ent = mapper.Map<T_entidad>(_In);
            ent.act = true;


            context.Add(ent);
            await context.SaveChangesAsync();
            var ret = mapper.Map<T_DTO>(ent);

            return new CreatedAtRouteResult(NombreRuta, new { id = ent.id }, ret);
        }
        /// <summary>
        /// new
        /// </summary>
        /// <typeparam name="T_DTO_in"></typeparam>
        /// <typeparam name="T_entidad"></typeparam>
        /// <typeparam name="T_DTO"></typeparam>
        /// <param name="_In"></param>
        /// <returns></returns>
        protected async Task<T_DTO> Post<T_DTO_in, T_entidad, T_DTO>(T_DTO_in _In) where T_entidad : class, IAct
        {

            var ent = mapper.Map<T_entidad>(_In);
            ent.act = true;
            context.Add(ent);
            await context.SaveChangesAsync();
            return mapper.Map<T_DTO>(ent);


        }
        /// <summary>
        /// cuando no tienen ninguna validacion o metodo
        /// </summary>
        /// <typeparam name="T_DTO_in"></typeparam>
        /// <typeparam name="T_entidad"></typeparam>
        /// <param name="_In"></param>
        /// <returns></returns>
        protected async Task Post<T_DTO_in, T_entidad>(T_DTO_in _In) where T_entidad : class
        {
            var ent = mapper.Map<T_entidad>(_In);
            context.Add(ent);
            await context.SaveChangesAsync();

        }

        #endregion

        #region PUT
        /// <summary>
        /// update
        /// </summary>
        /// <typeparam name="TDTO_in"></typeparam>
        /// <typeparam name="TEntidad"></typeparam>
        /// <param name="id"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Put<TDTO_in, TEntidad>(int id, TDTO_in insert) where TEntidad : class, Iid
        {
            var original = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.id == id);
            if (original == null)
                return NotFound();

            original = mapper.Map(insert, original);
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// update
        /// </summary>
        /// <typeparam name="TDTO_in"></typeparam>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="id"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        protected async Task<TDTO> Put<TDTO_in, TEntidad, TDTO>(int id, TDTO_in insert) where TEntidad : class, Iid
        {
            var original = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.id == id);

            original = mapper.Map(insert, original);
            await context.SaveChangesAsync();
            return mapper.Map<TDTO>(original);
        }

        #endregion

        #region DELETE
        /// <summary>
        /// delete
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, Iid, IAct
        {
            var ent = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.id == id);
            ent.act = false;
            await context.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// Remove
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Remove<TEntidad>(int id) where TEntidad : class, Iid, new()
        {
            if (!await context.Set<TEntidad>().AnyAsync(i => i.id == id))
                return NotFound();

            context.Remove(new TEntidad() { id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

        #endregion
    }
}
