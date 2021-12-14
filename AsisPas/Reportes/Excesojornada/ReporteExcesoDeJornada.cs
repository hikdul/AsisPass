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
    /// para obtener el reporte por exceso de jornada
    /// </summary>
    public class ReporteExcesoDeJornada: CabeceraReportes
    {
        #region props
        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public RTime Exceso { get; set; }
        /// <summary>
        /// tiempo de atrazos
        /// </summary>
        public RTime Atrazos { get; set; }
        /// <summary>
        /// diferencial
        /// </summary>
        public RTime Diferencial { get; set; }
        /// <summary>
        /// true si es a favor de la empresa 
        /// false si es a favor del empleado
        /// </summary>
        public bool AfavorEmpresa { get; set; }
        /// <summary>
        /// totales por semanas
        /// </summary>
        public List<TotalesExcesoJornada> Semanales { get; set; }
        /// <summary>
        /// calculos diarios
        /// </summary>
        public List<DiaExcesoJornada> Diarios { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// empty ctor
        /// </summary>
        public ReporteExcesoDeJornada()
        {
            this.Semanales = new();
            this.Diarios = new();
            this.AfavorEmpresa = false;
            this.Exceso = new(0);
            this.Atrazos = new(0);
            this.Diferencial = new(0);
        }

        #endregion


        #region llenar datos 
        /// <summary>
        /// para llenar los datos de mi reporte por asistencia
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Up(int idEmpleado, DateTime inicio, DateTime fin, ApplicationDbContext context)
        {
            try
            {
                // lleno mis fechas
                await UpHead(idEmpleado, inicio, fin, context);
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


                    if (Horario == null || Horario.id < 1)
                    {
                        if(marcas != null)
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
                                periodoX += +tiempoHoy;
                            }

                            if(marcas.Count < 3 && marcas.Count > 0)
                            {
                                msg = "Marcas Incompletas.";
                            }
                        }

                    }
                    else
                    {
                        if (Horario.DiaLaboral((int)i.DayOfWeek))
                        {
                            if (marcas != null)
                            {
                                if (marcas.Count > 3 && marcas[0] != null && marcas[1] != null && marcas[2] != null && marcas[3] != null)
                                {
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
                                else if (marcas.Count <= 3 && marcas.Count > 0)
                                {


                                    double tiempoHoy = Horario.tiempoEnSegundos(Horario);

                                    diaZ += tiempoHoy;
                                    semanaZ += tiempoHoy;
                                    periodoZ = +tiempoHoy;
                                    msg = "Marcas Incompletas, Todo el tiempo se toma como retrazo...";
                                }
                                else
                                {
                                    msg = "Este Dia no Hay Asistencia";
                                }
                            }
                        }
                        else
                        {
                            if(marcas != null && marcas.Count > 3 && marcas[0] != null && marcas[1] != null && marcas[2] != null && marcas[3] !=null )
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
                    }
                    catch
                    {
                        diaFlag = new();
                    }
                    if(diaFlag.fecha.Year > 1900)
                        this.Diarios.Add(diaFlag);

                    diaX = diaZ = 0;


                    // lleno la semana y limpio sus contadores
                    if ((int)i.DayOfWeek == 6 && i != fin)
                    {
                        this.Semanales.Add(new(Isemana, i, semanaX, semanaZ));
                        Isemana = i.AddDays(1);
                        semanaX = 0;
                        semanaZ = 0;

                    }
                    // termina el ciclo asi que cierro todo
                    if(i == fin)
                    {
                        this.Semanales.Add(new(Isemana, i, semanaX, semanaZ));
                        this.Exceso = new(periodoX);
                        this.Atrazos = new(periodoZ);
                        this.AfavorEmpresa = periodoZ > periodoX;
                        this.Diferencial = TotalesExcesoJornada.VerCalculoDiferencia(periodoX, periodoZ);

                    }
                
                
                }





            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #endregion


        #region exportar en excel
        /// <summary>
        /// para exportar un reporte en excel
        /// </summary>
        /// <param name="reporte"></param>
        /// <returns></returns>
        public static byte[] Excel(ReporteExcesoDeJornada reporte)
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

                        ew.Cells[6, 1].Value = "Lugar de prestacion de servicio";
                        ew.Cells[6, 2].Value = reporte.NombreSede;
                        ew.Cells[6, 3].Value = reporte.DireccionDelaSede;
                        ew.Cells[6, 5].Value = "Turno";
                        ew.Cells[6, 6].Value = reporte.NombreHorario;


                        ew.Cells[6, 3].Value = "Tiempos Totales Del Periodo";
                        ew.Cells[7, 1].Value = "Excesos";
                        ew.Cells[7, 1].Value = reporte.Exceso.toStr();
                        ew.Cells[8, 1].Value = "Atrazos";
                        ew.Cells[8, 2].Value = reporte.Atrazos.toStr();
                        ew.Cells[9, 1].Value = "Diferencial";
                        ew.Cells[9, 2].Value =  reporte.AfavorEmpresa ? "A Favor del empleador" : "A favor del empleado";
                        ew.Cells[9, 3].Value =  reporte.Diferencial.toStr();


                        ew.Cells[11, 1].Value = "inicia";
                        ew.Cells[11, 2].Value = "Culmina";
                        ew.Cells[11, 3].Value = "Exceso";
                        ew.Cells[11, 4].Value = "Atrazo";
                        ew.Cells[11, 5].Value = "diferencial";
                        ew.Cells[11, 6].Value = "A favor de";


                        if (reporte.Semanales != null && reporte.Semanales.Count > 0)
                            foreach (var item in reporte.Semanales)
                            {
                                ew.Cells[fila, 1].Value = item.inicio.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = item.fin.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 3].Value = item.Exceso.toStr();
                                ew.Cells[fila, 4].Value = item.Atrazos.toStr();
                                ew.Cells[fila, 5].Value = item.Diferencial.toStr();
                                ew.Cells[fila, 6].Value = item.AfavorEmpresa ? "Empleador" : "Empleado";
                               
                                fila++;
                            }
                        fila+=2;

                        ew.Cells[fila, 1].Value = "fecha";
                        ew.Cells[fila, 2].Value = "Jornada Pautada";
                        ew.Cells[fila, 3].Value = "Descanzo Pautado";
                        ew.Cells[fila, 4].Value = "Jornada Laborada";
                        ew.Cells[fila, 5].Value = "Descanzo Laborado";
                        fila++;

                        if (reporte.Diarios != null && reporte.Diarios.Count > 0)
                            foreach (var item in reporte.Diarios)
                            {
                                ew.Cells[fila, 1].Value = item.fecha.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = $"{item.Horario.InicioJornada} - {item.Horario.FinJornada}";
                                ew.Cells[fila, 3].Value = $"{item.Horario.InicioDescanzo} - {item.Horario.FinDescazo}";
                                ew.Cells[fila, 4].Value = $"{item.Marcas.InicioJornada} - {item.Marcas.FinJornada}";
                                ew.Cells[fila, 5].Value = $"{item.Marcas.InicioDescanzo} - {item.Marcas.FinDescazo}";

                                fila++;
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
        public static byte[] Pdf(ReporteExcesoDeJornada reporte)
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
                        totales.Add(new Paragraph($"Totales Del Periodo \n Atrazos: {reporte.Atrazos.toStr()} \n Excesos: {reporte.Exceso.toStr()} \n Compesacion: {reporte.Diferencial.toStr()} A Favor de {flag}"));

                     

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




                        if (reporte.Semanales != null && reporte.Semanales.Count > 0)
                            foreach (var item in reporte.Semanales)
                            {
                                C0.Add(new Paragraph(item.inicio.ToString("dd/MM/yyyy")));
                                C1.Add(new Paragraph(item.fin.ToString("dd/MM/yyyy")));
                                C2.Add(new Paragraph(item.Exceso.toStr()));
                                C3.Add(new Paragraph(item.Atrazos.toStr()));
                                C4.Add(new Paragraph(item.Diferencial.toStr()));
                                C5.Add(new Paragraph(item.AfavorEmpresa ? "Empleador" : "Empleado"));
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

                        Tdias.AddHeaderCell(Cd01);
                        Tdias.AddHeaderCell(Cd02);
                        Tdias.AddHeaderCell(Cd03);
                        Tdias.AddHeaderCell(Cd04);
                        Tdias.AddHeaderCell(Cd05);
                        Tdias.AddHeaderCell(Cd06);
                        Tdias.AddHeaderCell(Cd07);


                        if (reporte.Diarios != null && reporte.Diarios.Count > 0)
                            foreach (var item in reporte.Diarios)
                            {
                        Cell Cdd1 = new();
                        Cell Cdd2 = new();
                        Cell Cdd3 = new();
                        Cell Cdd4 = new();
                        Cell Cdd5 = new();
                        Cell Cdd6 = new();
                        Cell Cdd7 = new();
                                Cdd1.Add(new Paragraph(item.fecha.ToString("dd/MM/yyyy")));
                                Cdd2.Add(new Paragraph($"{item.Horario.InicioJornada} - {item.Horario.FinJornada}"));
                                Cdd3.Add(new Paragraph($"{item.Horario.InicioDescanzo} - {item.Horario.FinDescazo}"));
                                Cdd4.Add(new Paragraph($"{item.Marcas.InicioJornada} - {item.Marcas.FinJornada}"));
                                Cdd5.Add(new Paragraph($"{item.Marcas.InicioDescanzo} - {item.Marcas.FinDescazo}"));
                                Cdd6.Add(new Paragraph(item.Exceso.toStr()));
                                Cdd7.Add(new Paragraph(item.Atrazos.toStr()));

                        Tdias.AddCell(Cdd1);
                        Tdias.AddCell(Cdd2);
                        Tdias.AddCell(Cdd3);
                        Tdias.AddCell(Cdd4);
                        Tdias.AddCell(Cdd5);
                        Tdias.AddCell(Cdd6);
                        Tdias.AddCell(Cdd7);
                            }


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
        public static byte[] Word(ReporteExcesoDeJornada reporte)
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
                            tablas[i, 4].AddParagraph().AppendText(item.Diferencial.toStr());
                            string winner = item.AfavorEmpresa ? "Empleador" : "Empleado";
                            tablas[i, 5].AddParagraph().AppendText(winner);
                            i++;
                        }
                    }


                    if (reporte.Diarios != null && reporte.Diarios.Count > 0)
                    {


                        IWTable tablas;
                        tablas = section.AddTable();
                        tablas.ResetCells(reporte.Diarios.Count + 2, 7);


                        tablas[0, 0].AddParagraph().AppendText("Fecha");
                        tablas[0, 1].AddParagraph().AppendText("Jornada Pautada");
                        tablas[0, 2].AddParagraph().AppendText("Descanso Pautado ");
                        tablas[0, 3].AddParagraph().AppendText("Jornada Laborada");
                        tablas[0, 4].AddParagraph().AppendText("Descanso Laborado");
                        tablas[0, 5].AddParagraph().AppendText("Atrasos");
                        tablas[0, 6].AddParagraph().AppendText("excesos");


                        int i = 2;

                        foreach (var item in reporte.Diarios)
                        {

                            tablas[i, 0].AddParagraph().AppendText(item.fecha.ToString("dd/MM/yyyy"));
                            tablas[i, 1].AddParagraph().AppendText($"{item.Horario.InicioJornada} - {item.Horario.FinJornada}");
                            tablas[i, 2].AddParagraph().AppendText($"{item.Horario.InicioDescanzo} - {item.Horario.FinDescazo}");
                            tablas[i, 3].AddParagraph().AppendText($"{item.Marcas.InicioJornada} - {item.Marcas.FinJornada}");
                            tablas[i, 4].AddParagraph().AppendText($"{item.Marcas.InicioDescanzo} - {item.Marcas.FinDescazo}");
                            tablas[i, 5].AddParagraph().AppendText(item.Atrazos.toStr());
                            tablas[i, 6].AddParagraph().AppendText(item.Exceso.toStr());
                            i++;
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
