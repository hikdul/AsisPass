using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsisPas.Controllers.API
{
    /// <summary>
    /// para ingresar marcaciones, 
    /// este controlador es de libre acceso
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MarcajeController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        /// <summary>
        /// controlador
        /// </summary>
        /// <param name="context"></param>
        public MarcajeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// para insertar una nueva marca
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult<MarcaDTO>> Post(MarcaDTO_in ins)
        {
            try
            {
                // validamos puerta de ingreso
                var gate = await context.Gates
                    .Where(x => x.code == ins.GateCode)
                    .FirstOrDefaultAsync();
                if(gate == null || gate.act == false)
                    return BadRequest("El Codigo de ingreso no es valido");
                // validamos usuario
                var usuario = await context.Empleados
                    .Include(x => x.user)
                    .Where(x => x.user.Rut == ins.Rut)
                    .FirstOrDefaultAsync();
                if(usuario == null || usuario.act == false)
                    return BadRequest("El Usuario no tiene Acceso");
                // ahora validamos nuestro tipo de ingreso
                if(ins.TipoIngreso != 5)
                {
                    var ti = await context.Marcaciones
                        .Where(x => x.Empleadoid == usuario.id && x.TipoIngreso == ins.TipoIngreso)
                        .FirstOrDefaultAsync();
                    if(ti == null)
                        return BadRequest("El Usuario Ya Tiene una marca para esta marca");
                }

                // ** => ahora construimos nuestra entidad

                var entidad = new Marca();
                var AHorario = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == usuario.id && x.inicio <= ins.marca && x.fin >= ins.marca)
                    .FirstOrDefaultAsync();

                entidad.marca = ins.marca;
                entidad.Gate = gate.code;
                entidad.Sedeid = gate.Sedeid;
                entidad.Empleadoid = usuario.id;
                entidad.Horarioid = AHorario.Horarioid;
                entidad.TipoIngreso = ins.TipoIngreso != 5 ? ins.TipoIngreso : await CalcularTipoIngreso(ins.marca, ins.TipoIngreso, usuario, AHorario.Horario);
                entidad.Complete();
                // ** guardo en base de datos 
                context.Add(entidad);
                await context.SaveChangesAsync();

                //construllo mi retordo
                MarcaDTO resp = new();
                resp.Sede = gate.Sede.Direccion;
                resp.Gate = gate.code;
                resp.Hash = entidad.Hash;
                resp.HorarioNombre = AHorario.Horario.Nombre;
                resp.key = entidad.key;
                resp.EmpleadoApellido = usuario.user.Nombres;
                resp.EmpleadoNombre =usuario.user.Apellidos;
                resp.EmpleadoRut = usuario.user.Rut;
                resp.marca = entidad.marca;
                resp.TipoIngreso = entidad.tipoIngreso();

                // envio la info
                return Ok(resp);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("No se reconoce el usuario");
            }
        }


        /// <summary>
        /// verifica el ingreso que viene acontinuacion y lo agrega
        /// </summary>
        /// <param name="marca"></param>
        /// <param name="tipoIngreso"></param>
        /// <param name="usuario"></param>
        /// <param name="horario"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task<int> CalcularTipoIngreso(DateTime marca, int tipoIngreso, Empleado usuario,Horario horario)
        {
            // verificar esta funcion luego

            var marcas = await context.Marcaciones
                  .Where(x => x.marca.ToString("dd/MM/YYY") == DateTime.Now.ToString("dd/MM/yyyy"))
                  .ToListAsync();


            if(marcas == null || marcas.Count == 0)
                return 0;
            
            if(horario.sinDescanzo)
                    return 4;

            if (horario.diaSiguiente == false)
                return marcas.Count;

           

            if (marcas.Count < 2)
                return marcas.Count + 2;

            return -1;

        }
    }

}
