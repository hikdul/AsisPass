using AsisPas.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsisPas.Reportes.CambiosTurno
{
    /// <summary>
    /// para generar el reporte por turnos
    /// </summary>
    public class ReporteCombioDeTurno: CabeceraReportes
    {

        #region propiedades
        /// <summary>
        /// listado de cambios
        /// </summary>
        public List<DiaCambio> cambios { get; set; }

        #endregion


        #region llenar data
        public async System.Threading.Tasks.Task Up(int idEmpleado, DateTime inicio, DateTime fin, ApplicationDbContext context)
        {
            try {
                await UpHead(idEmpleado, inicio, fin, context);
                
                var admos =  await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado
                    && x.inicio >= inicio && x.fin <= fin)
                    .ToListAsync();

                var primer = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado && x.fin <= inicio)
                    .LastOrDefaultAsync();


                for (int i = 0; i < admos.Count; i++)
                {
                    modificaciones anterion = new();
                    modificaciones actual = new();
                    DiaCambio diaI = new();
                    //primero el anterior
                    if (i == 0)
                    {
                        if (primer == null || primer.id < 1)
                            anterion = null;
                        else
                        {
                            anterion.inicio = primer.inicio;
                            anterion.fin = primer.fin;
                            anterion.Horario = primer.Horario;
                            anterion.Modificacion = primer.fechaAsignacion;
                        }
                        actual.inicio = admos[i].inicio;
                        actual.fin = admos[i].fin;
                        actual.Horario = admos[i].Horario;
                        actual.Modificacion = admos[i].fechaAsignacion;
                    }
                    else
                    {
                        anterion.inicio = admos[i-1].inicio;
                        anterion.fin = admos[i-1].fin;
                        anterion.Horario = admos[i-1].Horario;
                        anterion.Modificacion = admos[i-1].fechaAsignacion;
                        actual.inicio = admos[i].inicio;
                        actual.fin = admos[i].fin;
                        actual.Horario = admos[i].Horario;
                        actual.Modificacion = admos[i].fechaAsignacion;
                    }

                        diaI.Anterior = anterion;
                        diaI.nuevo = actual;
                        diaI.Desc = admos[i].Razon;
                        diaI.SolicitanteCambio = "Empleador";
                }
            }catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            } 
            


        }
            #endregion

    }
}
