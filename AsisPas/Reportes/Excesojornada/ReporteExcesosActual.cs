using AsisPas.Data;
using AsisPas.Entitys;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// este es el reporte por excesos segun el formato indicado
    /// </summary>
    public class ReporteExcesosActual: CabeceraReportes
    {
        #region props
        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public RTime ExcesoPeriodo { get; set; }
        /// <summary>
        /// tiempo de atrazos
        /// </summary>
        public RTime AtrazosPeriodo { get; set; }
        /// <summary>
        /// diferencial
        /// </summary>
        public RTime DiferencialPeriodo { get; set; }
        /// <summary>
        /// true si es a favor de la empresa 
        /// false si es a favor del empleado
        /// </summary>
        public bool AfavorEmpresa { get; set; }
        /// <summary>
        /// totales por semanas
        /// </summary>
        public List<TotalesExcesoJornadaSemanal> Semanales { get; set; }

        #endregion

        #region construtor
        /// <summary>
        /// empty ctor
        /// </summary>
        public ReporteExcesosActual()
        {
            this.AfavorEmpresa = false;
            this.AtrazosPeriodo = new(0);
            this.DiferencialPeriodo = new(0);
            this.ExcesoPeriodo = new(0);
            this.FechaGenerado = DateTime.Now;
            this.Mensaje = "";
            this.Semanales = new();
        }

        #endregion


        #region up
        /// <summary>
        /// para llenor mi reporte por exceso de jornada
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Up(int idEmpleado, DateTime inicio, DateTime fin, ApplicationDbContext context)
        {
            var first = await context.Marcaciones.FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado);
            await UpHead(idEmpleado, inicio, fin, first, context);


            if (first != null && first.id > 0)
            {
                if (first.marca > inicio)
                {
                    inicio = first.marca;
                    this.Mensaje = $"No se poseen registros anteriores a {inicio.ToString("dd/MM/yyyy")}, es posible que el empleado no se encontrara contratado para esta fecha.";
                }
            }

            List<DiaExcesoJornada> days = new();
            string msg = "";
            double diaX = 0;
            double semanaX = 0;
            double periodoX = 0;
            double diaZ = 0;
            double semanaZ = 0;
            double periodoZ = 0;
            DateTime Isemana = inicio;

            var ahs = await context.AdmoHorarios
                  .Include(x => x.Horario)
                  .Where(x => x.Empleadoid == idEmpleado)
                  .ToListAsync();


            for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
            {
                DiaExcesoJornada band = new DiaExcesoJornada();
                //calcular los excesos y retrazos y darle los valores a cada acumulador
                var Haux = ahs.Where(x => x.inicio <= i && x.fin >= i).ToList();
                var Horario = Haux == null || Haux.Count < 1 ? null : Haux[0].Horario;

                var marcas = await Marca.MarcasDiaYUsuario(idEmpleado, i, context);

                //verifico en caso de una jornada donde el empleado trabajo sin horario asignado
                if (Horario == null || Horario.id < 1)
                {
                    if (marcas != null)
                    {
                        if (marcas.Count > 3 && marcas[0] != null && marcas[1] != null && marcas[2] != null && marcas[3] != null)
                        {
                            var inicioTur = marcas.First(x => x.TipoIngreso == 0).marca;
                            var finTur = marcas.First(x => x.TipoIngreso == 3).marca;
                            var inicioDes = marcas.First(x => x.TipoIngreso == 1).marca;
                            var findes = marcas.First(x => x.TipoIngreso == 2).marca;
                            double tiempoHoy = RTime.ObtenerTotalSg(inicioTur, finTur, inicioDes, findes);
                            diaX += tiempoHoy;
                            semanaX += tiempoHoy;
                            periodoX += tiempoHoy;
                        }

                    }//ahora con horario asignado
                }
                else
                {
                    if (Horario.DiaLaboral((int)i.DayOfWeek))// si mi dia es laboral calculo normal
                    {
                        if (marcas != null)//si hay marcas
                        {
                            if (marcas.Count > 3 && marcas[0] != null && marcas[1] != null && marcas[2] != null && marcas[3] != null)
                            {// marcas normal
                                var inicioTur = marcas.First(x => x.TipoIngreso == 0).marca;
                                var finTur = marcas.First(x => x.TipoIngreso == 3).marca;
                                var inicioDes = marcas.First(x => x.TipoIngreso == 1).marca;
                                var findes = marcas.First(x => x.TipoIngreso == 2).marca;

                                double tiempoHoy = Horario.tiempoEnSegundos(Horario) - RTime.ObtenerTotalSg(inicioTur, finTur, inicioDes, findes);

                                bool FlagAfavorEmpresa = tiempoHoy >= 0;
                                tiempoHoy = tiempoHoy >= 0 ? tiempoHoy : -tiempoHoy;

                                //aqui hay que calcular las retrazos y excesos por cada marca
                                var ing = Marca.diferencialConHora(inicioTur, Horario.hi);
                                var aca = Marca.diferencialConHora(finTur, Horario.hf);
                                var iniD = Marca.diferencialConHora(inicioDes, Horario.hbi);
                                var acaD = Marca.diferencialConHora(findes, Horario.hbf);

                                if (ing >= 0)
                                {
                                    diaZ += ing;
                                    semanaZ += ing;
                                    periodoZ += ing;
                                }
                                else
                                {
                                    diaX += -ing;
                                    semanaX += -ing;
                                    periodoX += -ing; ;
                                }

                                if (aca >= 0)
                                {
                                    diaX += aca;
                                    semanaX += aca;
                                    periodoX += aca;
                                }
                                else
                                {
                                    diaZ += -aca;
                                    semanaZ += -aca;
                                    periodoZ += -aca;
                                }

                                if (iniD >= 0)
                                {
                                    diaX += iniD;
                                    semanaX += iniD;
                                    periodoX += iniD;
                                }
                                else
                                {
                                    diaZ += -iniD;
                                    semanaZ += -iniD;
                                    periodoZ += -iniD;
                                }


                                if (acaD >= 0)
                                {
                                    diaZ += acaD;
                                    semanaZ += acaD;
                                    periodoZ += acaD;
                                }
                                else
                                {
                                    diaX += -acaD;
                                    semanaX += -acaD;
                                    periodoX += -acaD;
                                }

                            }
                            else if (marcas.Count <= 3 && marcas.Count > 0)// si hay marcas incompletas... se toma como dia no laborado... y en deuda para la empresa
                            {
                                double tiempoHoy = Horario.tiempoEnSegundos(Horario);

                                diaZ += tiempoHoy;
                                semanaZ += tiempoHoy;
                                periodoZ = +tiempoHoy;
                                msg = "Marcas Incompletas, Todo el tiempo se toma como retrazo...";
                            }

                        }
                    }
                    else // se verifica que no sea un dia laboral, por tanto es tiempo extra a favor del empleado
                    {
                        if (marcas != null && marcas.Count > 3 && marcas[0] != null && marcas[1] != null && marcas[2] != null && marcas[3] != null)
                        {
                            var inicioTur = marcas.First(x => x.TipoIngreso == 0).marca;
                            var finTur = marcas.First(x => x.TipoIngreso == 3).marca;
                            var inicioDes = marcas.First(x => x.TipoIngreso == 1).marca;
                            var findes = marcas.First(x => x.TipoIngreso == 2).marca;
                            double tiempoHoy = RTime.ObtenerTotalSg(inicioTur, finTur, inicioDes, findes);
                            diaX += tiempoHoy;
                            semanaX += tiempoHoy;
                            periodoX += +tiempoHoy;
                        }
                    }
                }

                
             


                //aqui lleno mi dia y agrego sumatorias a las semanas
                DiaExcesoJornada diaFlag;
                //cierro la tienda
                //lleno un dia y limpio sus contadores
                try
                {
                    diaFlag = new()
                    {
                        Atrazos = new(diaZ),
                        Exceso = new(diaX),
                        DiaLaboral = Horario.DiaLaboral((int)i.DayOfWeek),
                        fecha = i,
                        Horario = new(Horario),
                        Marcas = marcas != null && marcas.Count >= 4
                        ?
                        new(marcas.First(x => x.TipoIngreso == 0).marca, marcas.First(x => x.TipoIngreso == 3).marca, marcas.First(x => x.TipoIngreso == 1).marca, marcas.First(x => x.TipoIngreso == 2).marca)
                        : null,
                        mensaje = msg,
                    };

                    diaX = diaZ = 0;

                }
                catch
                {
                    diaFlag = new()
                    {
                        Atrazos = new(0),
                        Exceso = new(0),
                        DiaLaboral = Horario == null || Horario.id < 1 ? false : Horario.DiaLaboral((int)i.DayOfWeek),
                        fecha = i,
                        Horario = null,
                        Marcas = null,
                        mensaje = msg,
                    };
                    diaX = diaZ = 0;
                }

                days.Add(diaFlag);

                //cierra de la semana
                if(i.DayOfWeek == DayOfWeek.Saturday && i != fin)
                {
                    try
                    {
                        this.Semanales.Add(new(Isemana, i, semanaX, semanaZ, days));
                        Isemana = i.AddDays(1);
                        days.Clear();
                        semanaX = 0;
                        semanaZ = 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                //en caso de fin del perioda se cierro todo el periodo
                if(i == fin)
                {
                    try
                    {
                        this.Semanales.Add(new(Isemana, i, semanaX, semanaZ, days));
                        this.ExcesoPeriodo = new(periodoX);
                        this.AtrazosPeriodo = new(periodoZ);
                        this.DiferencialPeriodo = TotalesExcesoJornada.VerCalculoDiferencia(periodoX, periodoZ);
                        this.AfavorEmpresa = periodoZ > periodoX;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }

            


            }
        #endregion


        #region exportar en excel
        /// <summary>
        /// para exportar un reporte en excel
        /// </summary>
        /// <param name="reporte"></param>
        /// <returns></returns>
        public static byte[] Excel(ReporteExcesosActual reporte)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage ep = new ExcelPackage())
                    {
                        int fila = 12;
                        ep.Workbook.Worksheets.Add("Reporte Por Asistencia");
                        ExcelWorksheet ew = ep.Workbook.Worksheets[0];

                        ew.Cells[1, 1].Value = "Reporte Generado el ";
                        ew.Cells[1, 2].Value = DateTime.Now.ToString("dd/MM/yyyy");

                        ew.Cells[3, 1].Value = "Trabajador";
                        ew.Cells[3, 2].Value = reporte.NombreEmpleado;
                        ew.Cells[3, 3].Value = "Rut";
                        ew.Cells[3, 4].Value = reporte.RutEmpleado;

                        ew.Cells[4, 1].Value = "Empresa";
                        ew.Cells[4, 2].Value = reporte.NombreEmpresa;
                        ew.Cells[4, 3].Value = "Rut";
                        ew.Cells[4, 4].Value = reporte.RutEmpresa;


                        ew.Cells[5, 1].Value = "Periodo de estudio";
                        ew.Cells[5, 2].Value = "Inicia";
                        ew.Cells[5, 3].Value = reporte.inicio.ToString("dd/MM/yyyy");
                        ew.Cells[5, 4].Value = "Termina";
                        ew.Cells[5, 5].Value = reporte.fin.ToString("dd/MM/yyyy");

                        ew.Cells[6, 1].Value = "Lugar de prestacion de servicio Actual";
                        ew.Cells[6, 2].Value = reporte.NombreSede;
                        ew.Cells[6, 3].Value = reporte.DireccionDelaSede;
                        ew.Cells[6, 5].Value = "Turno";
                        ew.Cells[6, 6].Value = reporte.NombreHorario;


                        ew.Cells[6, 3].Value = "Tiempos Totales Del Periodo";
                        ew.Cells[7, 1].Value = "Excesos";
                        ew.Cells[7, 1].Value = reporte.ExcesoPeriodo.toStr();
                        ew.Cells[8, 1].Value = "Atrazos";
                        ew.Cells[8, 2].Value = reporte.AtrazosPeriodo.toStr();
                        ew.Cells[9, 1].Value = "Diferencial";
                        ew.Cells[9, 2].Value = reporte.AfavorEmpresa ? "A Favor del empleador" : "A favor del empleado";
                        ew.Cells[9, 3].Value = reporte.DiferencialPeriodo.toStr();




                        if (reporte.Semanales != null && reporte.Semanales.Count > 0)
                            foreach (var item in reporte.Semanales)
                            {

                                ew.Cells[fila,1].Value = "Periodo";
                                ew.Cells[fila,2].Value = "inicia";
                                ew.Cells[fila,3].Value = item.inicio.ToString("dd/MM/yyyy");
                                ew.Cells[fila,4].Value = "Culmina";
                                ew.Cells[fila, 5].Value = item.fin.ToString("dd/MM/yyyy"); 

                                fila++;


                                ew.Cells[fila, 1].Value = "fecha";
                                ew.Cells[fila, 2].Value = "Jornada Pactada";
                                ew.Cells[fila, 4].Value = "Jornada Laborada";
                                ew.Cells[fila, 6].Value = "Atrasos y Excesos";
                                fila++;
                                ew.Cells[fila, 2].Value = "jornada";
                                ew.Cells[fila, 3].Value = "descanso";
                                ew.Cells[fila, 4].Value = "jornada";
                                ew.Cells[fila, 5].Value = "descansa";
                                ew.Cells[fila, 6].Value = "atrasos ";
                                ew.Cells[fila, 7].Value = "excesos";
                                fila++;

                                foreach (var dia in item.Diarios)
                                {
                                    ew.Cells[fila, 1].Value = dia.fecha.ToString("dd/MM/yyyy");
                                    if (dia.Horario != null && dia.Horario.InicioJornada != dia.Horario.FinDescazo)
                                    {
                                        ew.Cells[fila, 2].Value = $"{dia.Horario.InicioJornada} - {dia.Horario.FinJornada}";
                                        ew.Cells[fila, 3].Value = $"{dia.Horario.InicioDescanzo} - {dia.Horario.FinDescazo}";
                                    }
                                    else
                                    {
                                        ew.Cells[fila, 2].Value = "No Hay Horario Para esta fecha";
                                    }
                                 
                                    if (dia.Marcas != null && dia.Marcas.InicioJornada != dia.Marcas.FinDescazo) 
                                    { 
                                        ew.Cells[fila, 4].Value = $"{dia.Marcas.InicioJornada} - {dia.Marcas.FinJornada}";
                                        ew.Cells[fila, 5].Value = $"{dia.Marcas.InicioDescanzo} - {dia.Marcas.FinDescazo}";
                                    }
                                    else
                                    {
                                        ew.Cells[fila, 4].Value = "No Hay Marcas Validas Para esta fecha";

                                    }
                                    ew.Cells[fila, 6].Value = dia.Atrazos.toStr();
                                    ew.Cells[fila, 7].Value = dia.Exceso.toStr();
                                    fila++;

                                }
                                ew.Cells[fila, 1].Value = "Exceso Semanal";
                                ew.Cells[fila, 2].Value = item.Exceso.toStr();

                                ew.Cells[fila, 3].Value = "Atrasos Semanal";
                                ew.Cells[fila, 4].Value = item.Atrazos.toStr();
                                ew.Cells[fila, 5].Value = item.AfavorEmpresa ? "Tiempo que debe reponer el empleado" : "tiempo que debe reponer el empleador";
                                ew.Cells[fila, 6].Value = item.Compensacion.toStr();


                                fila+=2;
                            }
                     


                        ep.SaveAs(ms);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine($"Message: {0}", ex.Message);
                return new Byte[0];
            }

        }

        #endregion


        #region exportar en PDF
        /// <summary>
        /// para exportar un reporte en pdf
        /// </summary>
        /// <param name="reporte"></param>
        /// <returns></returns>
        public static byte[] Pdf(ReporteExcesosActual reporte)
        {

            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new(ms);
                    using (PdfDocument document = new(writer))
                    {
                        Document doc = new(document);
                        Paragraph titulo = new Paragraph($" REPORTE POR ASISTENCIAS \n Reporte Generado el ${DateTime.Now:dd/MM/yyyy}");
                        //TITULO
                        var white = new Paragraph(" ");

                        Cell perioda = new();
                        perioda.Add(new Paragraph($"Periodo del reporte, inicia {reporte.inicio.ToString("dd/MM/yyyy")} y culmina {reporte.fin.ToString("dd/MM/yyyy")}"));



                        titulo.SetFontSize(20);
                        titulo.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        doc.Add(titulo);
                        doc.Add(white);
                        doc.Add(perioda);

                        doc.Add(white);
                        doc.Add(white);
                        doc.Add(white);

                        Cell nombre = new();
                        Cell empresa = new();
                        Cell sede = new();
                        Cell horario = new();
                        Cell totales = new();

                        nombre.Add(new Paragraph($"Empleado: {reporte.NombreEmpleado} - RUT: {reporte.RutEmpleado}"));
                        empresa.Add(new Paragraph($"Empresa: {reporte.NombreEmpresa} - RUT: {reporte.RutEmpresa}"));
                        sede.Add(new Paragraph($"Sede: {reporte.NombreSede} - Direccion: {reporte.DireccionDelaSede}"));
                        horario.Add(new Paragraph($"Turno: {reporte.NombreHorario} "));
                        string flag = reporte.AfavorEmpresa ? "empresa" : "Empleador";
                        totales.Add(new Paragraph($"Totales Del Periodo \n Atrazos: {reporte.AtrazosPeriodo.toStr()} \n Excesos: {reporte.ExcesoPeriodo.toStr()} \n Compesacion: {reporte.DiferencialPeriodo.toStr()} A Favor de {flag}"));



                        doc.Add(nombre);
                        doc.Add(empresa);
                        doc.Add(sede);
                        doc.Add(horario);
                        doc.Add(white);
                        doc.Add(white);
                        doc.Add(white);


                        //TABLA DE DATOS DE LOS REPORTES DIARIOS

                        Table Tabla;


                        Cell C01 = new();
                        Cell C02 = new();
                        Cell C03 = new();
                        Cell C04 = new();
                        Cell C05 = new();
                        Cell C06 = new();
                        Cell C07 = new();
                        Cell C08 = new();
                        Cell C09 = new();


                        C01.Add(new Paragraph(" Inicia"));
                        C02.Add(new Paragraph(" Culmina"));
                        C03.Add(new Paragraph(" Exceso"));
                        C04.Add(new Paragraph(" Atrazo"));
                        C05.Add(new Paragraph(" diferencial"));
                        C06.Add(new Paragraph(" A Favor de"));




                        Cell C0 = new();
                        Cell C1 = new();
                        Cell C2 = new();
                        Cell C3 = new();
                        Cell C4 = new();
                        Cell C5 = new();





                        Tabla = new Table(6);

                        Tabla.AddHeaderCell(C01);
                        Tabla.AddHeaderCell(C02);
                        Tabla.AddHeaderCell(C03);
                        Tabla.AddHeaderCell(C04);
                        Tabla.AddHeaderCell(C05);
                        Tabla.AddHeaderCell(C06);


                        Table Tdias = new(7);


                        Cell Cd01 = new();
                        Cell Cd02 = new();
                        Cell Cd03 = new();
                        Cell Cd04 = new();
                        Cell Cd05 = new();
                        Cell Cd06 = new();
                        Cell Cd07 = new();


                                Cd01.Add(new Paragraph("Fecha"));
                                Cd02.Add(new Paragraph("Jornada Pautada"));
                                Cd03.Add(new Paragraph("Descanso Pautado"));
                                Cd04.Add(new Paragraph("Jornada Laborada"));
                                Cd05.Add(new Paragraph("Descanso Laborado"));
                                Cd06.Add(new Paragraph("Exceso"));
                                Cd07.Add(new Paragraph("Retraso"));
                        if (reporte.Semanales != null && reporte.Semanales.Count > 0)
                            foreach (var item in reporte.Semanales)
                            {
                                C0.Add(new Paragraph(item.inicio.ToString("dd/MM/yyyy")));
                                C1.Add(new Paragraph(item.fin.ToString("dd/MM/yyyy")));
                                C2.Add(new Paragraph(item.Exceso.toStr()));
                                C3.Add(new Paragraph(item.Atrazos.toStr()));
                                C4.Add(new Paragraph(item.Compensacion.toStr()));
                                C5.Add(new Paragraph(item.AfavorEmpresa ? "Empleador" : "Empleado"));




                                foreach (var dia in item.Diarios)
                                {
                                    Cell Cdd1 = new();
                                    Cell Cdd2 = new();
                                    Cell Cdd3 = new();
                                    Cell Cdd4 = new();
                                    Cell Cdd5 = new();
                                    Cell Cdd6 = new();
                                    Cell Cdd7 = new();
                                    Cdd1.Add(new Paragraph(dia.fecha.ToString("dd/MM/yyyy")));
                                    if (dia.Horario != null && dia.Horario.InicioJornada != dia.Horario.FinDescazo)
                                    {
                                        Cdd2.Add(new Paragraph($"{dia.Horario.InicioJornada} - {dia.Horario.FinJornada}"));
                                        Cdd3.Add(new Paragraph($"{dia.Horario.InicioDescanzo} - {dia.Horario.FinDescazo}"));
                                    }
                                    else
                                    {
                                        Cdd2.Add(new Paragraph("No Hay Horario Asignado para este dia"));
                                    }

                                    if (dia.Marcas != null && dia.Marcas.InicioJornada != dia.Marcas.FinDescazo)
                                    {
                                        Cdd4.Add(new Paragraph($"{dia.Marcas.InicioJornada} - {dia.Marcas.FinJornada}"));
                                        Cdd5.Add(new Paragraph($"{dia.Marcas.InicioDescanzo} - {dia.Marcas.FinDescazo}"));
                                    }
                                    else
                                    {
                                        Cdd4.Add(new Paragraph("No Hay Marcas Validas para este dia"));

                                    }
                                    Cdd6.Add(new Paragraph(dia.Exceso.toStr()));
                                    Cdd7.Add(new Paragraph(dia.Atrazos.toStr()));

                                    Tdias.AddCell(Cdd1);
                                    Tdias.AddCell(Cdd2);
                                    Tdias.AddCell(Cdd3);
                                    Tdias.AddCell(Cdd4);
                                    Tdias.AddCell(Cdd5);
                                    Tdias.AddCell(Cdd6);
                                    Tdias.AddCell(Cdd7);
                                }


                            }
                        Tabla.AddCell(C0);
                        Tabla.AddCell(C1);
                        Tabla.AddCell(C2);
                        Tabla.AddCell(C3);
                        Tabla.AddCell(C4);
                        Tabla.AddCell(C5);
                        doc.Add(white);
                        doc.Add(Tabla);
                        doc.Add(white);
                        doc.Add(white);




                      

                        Tdias.AddHeaderCell(Cd01);
                        Tdias.AddHeaderCell(Cd02);
                        Tdias.AddHeaderCell(Cd03);
                        Tdias.AddHeaderCell(Cd04);
                        Tdias.AddHeaderCell(Cd05);
                        Tdias.AddHeaderCell(Cd06);
                        Tdias.AddHeaderCell(Cd07);


                     


                        doc.Add(white);
                        doc.Add(Tdias);
                        doc.Add(white);

                        doc.Close();
                        writer.Close();
                    }

                    return ms.ToArray();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine($"Message: {0}", ex.Message);
                return new Byte[0];
            }

        }

        #endregion

        #region word

        /// <summary>
        /// para exportar un documento en word
        /// </summary>
        /// <param name="reporte"></param>
        /// <returns></returns>
        public static byte[] Word(ReporteExcesosActual reporte)
        {
            try
            {


                using (MemoryStream ms = new MemoryStream())
                {
                    //declaraciones necesarias para que el reporte termine ordenado
                    WordDocument document = new WordDocument();
                    WSection section = document.AddSection() as WSection;
                    section.PageSetup.Margins.All = 72;
                    section.PageSetup.PageSize = new Syncfusion.Drawing.SizeF(612, 792);


                    IWParagraph titulo = section.AddParagraph();
                    titulo.ApplyStyle("Normal");
                    titulo.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    WTextRange textRange = titulo.AppendText("Reporte Por Asistencias") as WTextRange;
                    textRange.CharacterFormat.FontSize = 20f;
                    textRange.CharacterFormat.FontName = "Calibri";


                    // cuerpo del reporte

                    IWParagraph DatosUsurio = section.AddParagraph();
                    DatosUsurio.ApplyStyle("Normal");
                    DatosUsurio.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange UserData = DatosUsurio.AppendText("Fecha Generado \n") as WTextRange;
                    DatosUsurio.AppendText($"{DateTime.Now.ToString("dd/MM/yyyy")}\n");

                    IWParagraph Empresa = section.AddParagraph();
                    Empresa.ApplyStyle("Normal");
                    Empresa.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange emp = Empresa.AppendText($"Empresa: {reporte.NombreEmpresa} \n") as WTextRange;
                    Empresa.AppendText($"Rut: {reporte.RutEmpresa}\n");


                    IWParagraph Empleado = section.AddParagraph();
                    Empleado.ApplyStyle("Normal");
                    Empleado.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange empl = Empleado.AppendText($"Empleado: {reporte.NombreEmpleado} \n") as WTextRange;
                    Empleado.AppendText($"Rut: {reporte.RutEmpleado}\n");


                    IWParagraph Sede = section.AddParagraph();
                    Sede.ApplyStyle("Normal");
                    Sede.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange sed = Sede.AppendText($"Sede: {reporte.NombreSede} \n") as WTextRange;
                    Sede.AppendText($"Direccion: {reporte.DireccionDelaSede}\n");

                    IWParagraph Horario = section.AddParagraph();
                    Horario.ApplyStyle("Normal");
                    Horario.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange hor = Horario.AppendText($"Horario: {reporte.NombreHorario} \n") as WTextRange;





                    //ingreso  la tabla donde se añadira cada dia
                    if (reporte.Semanales != null && reporte.Semanales.Count > 0)
                    {


                        IWTable tablas;
                        tablas = section.AddTable();
                        tablas.ResetCells(reporte.Semanales.Count + 2, 6);


                        tablas[0, 0].AddParagraph().AppendText("inicia");
                        tablas[0, 1].AddParagraph().AppendText("finaliza");
                        tablas[0, 2].AddParagraph().AppendText("Atraso");
                        tablas[0, 3].AddParagraph().AppendText("Exceso");
                        tablas[0, 4].AddParagraph().AppendText("Compensacion");
                        tablas[0, 5].AddParagraph().AppendText("A Favor de");


                        int i = 2;

                        foreach (var item in reporte.Semanales)
                        {

                            tablas[i, 0].AddParagraph().AppendText(item.inicio.ToString("dd/MM/yyyy"));
                            tablas[i, 1].AddParagraph().AppendText(item.fin.ToString("dd/MM/yyyy"));
                            tablas[i, 2].AddParagraph().AppendText(item.Atrazos.toStr());
                            tablas[i, 3].AddParagraph().AppendText(item.Exceso.toStr());
                            tablas[i, 4].AddParagraph().AppendText(item.Compensacion.toStr());
                            string winner = item.AfavorEmpresa ? "Empleador" : "Empleado";
                            tablas[i, 5].AddParagraph().AppendText(winner);
                            i++;

                            if (item.Diarios != null && item.Diarios.Count > 0)
                            {
                                IWTable tablas2;
                                tablas2 = section.AddTable();
                                tablas2.ResetCells(item.Diarios.Count + 2, 7);


                                tablas2[0, 0].AddParagraph().AppendText("Fecha");
                                tablas2[0, 1].AddParagraph().AppendText("Jornada Pautada");
                                tablas2[0, 2].AddParagraph().AppendText("Descanso Pautado ");
                                tablas2[0, 3].AddParagraph().AppendText("Jornada Laborada");
                                tablas2[0, 4].AddParagraph().AppendText("Descanso Laborado");
                                tablas2[0, 5].AddParagraph().AppendText("Atrasos");
                                tablas2[0, 6].AddParagraph().AppendText("excesos");


                                int tt = 2;

                                foreach (var dia in item.Diarios)
                                {
                                    tablas2[tt, 0].AddParagraph().AppendText(dia.fecha.ToString("dd/MM/yyyy"));

                                    if (dia.Horario != null && dia.Horario.InicioJornada != dia.Horario.FinDescazo) 
                                    { 
                                    tablas2[tt, 1].AddParagraph().AppendText($"{dia.Horario.InicioJornada} - {dia.Horario.FinJornada}");
                                    tablas2[tt, 2].AddParagraph().AppendText($"{dia.Horario.InicioDescanzo} - {dia.Horario.FinDescazo}");
                                    }
                                    else
                                    {
                                        tablas2[tt, 1].AddParagraph().AppendText("No Hay Horario Asignado Para este dia");
                                    }


                                    if (dia.Marcas != null && dia.Marcas.InicioJornada != dia.Marcas.FinDescazo)
                                    {
                                        tablas2[tt, 3].AddParagraph().AppendText($"{dia.Marcas.InicioJornada} - {dia.Marcas.FinJornada}");
                                        tablas2[tt, 4].AddParagraph().AppendText($"{dia.Marcas.InicioDescanzo} - {dia.Marcas.FinDescazo}");
                                    }
                                    else
                                    {
                                        tablas2[tt, 3].AddParagraph().AppendText("No Hay Macas Validas Para este dia");
                                    }
                                    tablas2[tt, 5].AddParagraph().AppendText(dia.Atrazos.toStr());
                                    tablas2[tt, 6].AddParagraph().AppendText(dia.Exceso.toStr());
                                    tt++;
                                }


                            }
                        }


                    }


                  

                    // fin del reporte
                    document.Save(ms, Syncfusion.DocIO.FormatType.Docx);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine($"Message: {0}", ex.Message);
                return new Byte[0];
            }
        }

        #endregion



    }
}
