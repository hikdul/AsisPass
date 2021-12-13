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

namespace AsisPas.Reportes
{
    /// <summary>
    /// para generar un reporte por asistencia
    /// </summary>
    public class ReporteAsistencia : CabeceraReportes
    {
        #region prop


        /// <summary>
        /// lista del recorrido por los dias habiles
        /// </summary>
        public List<DiasAsistencia> Recorrido { get; set; }
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

                
                var ahs = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado)
                    .ToListAsync();

                AdmoHorario ah = ahs
                    .Where(x => x.inicio <= inicio && x.fin >= fin)
                    .FirstOrDefault();


                List<Feriado> feriados = await Feriado.listado(context);
                List<DiasAsistencia> dias = new();

                for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
                {
                    var istr = i.ToString("dd/MM/yyyy");
                    var fer = feriados.FirstOrDefault(x => x.fecha.ToString("dd/MM/yyyy") == istr);
                    var laboral = ah.Horario.DiaLaboral((int)i.DayOfWeek);

                    if (laboral && (fer == null || fer.id < 1))
                    {
                        if (ah == null)
                            dias.Add(new(i, false, new()
                            {
                                id = -1,
                                Desc = "El Usuario no estaba contratado o no hay registros para este dia"
                            }));
                        else
                        {
                            if (ah.fin < i)
                            {
                                    ah = await context.AdmoHorarios
                                .Include(x => x.Horario)
                                .FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado && x.inicio >= i && x.fin <= i);

                            }

                            var ri = await context.Marcaciones.Where(x =>
                            x.Empleadoid == idEmpleado 
                            && x.TipoIngreso == 0
                            && x.marca.Year == i.Year
                            && x.marca.Month == i.Month
                            && x.marca.Day == i.Day
                            )
                                .FirstOrDefaultAsync();
                            var per = await context.Permisos.Where(x =>
                            x.Empleadoid == idEmpleado
                            && x.inicio <= i && x.fin >= i).FirstOrDefaultAsync();

                            dias.Add(new(i, (ri != null || ri.id < 1), per));

                        }


                    }
                }

                this.Recorrido = dias;

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
        public static byte[] Excel(ReporteAsistencia reporte)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage ep = new ExcelPackage())
                    {
                        int fila = 9;
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

                        ew.Cells[8, 1].Value = "Fecha";
                        ew.Cells[8, 2].Value = "Asistio";
                        ew.Cells[8, 3].Value = "Observaciones";


                        if(reporte.Recorrido != null && reporte.Recorrido.Count > 0)
                        foreach (var item in reporte.Recorrido)
                        {
                            ew.Cells[fila, 1].Value = item.fecha;
                            ew.Cells[fila, 2].Value = item.Asistio;
                            ew.Cells[fila, 3].Value = item.Observacion;
                            if(item.permiso != null && item.permiso.id > 0 && item.permiso.act == true)
                            {
                                ew.Cells[fila, 4].Value = $"Periodo del permiso, del {item.permiso.inicio.ToString("dd/MM/yyyy")} al {item.permiso.fin.ToString("dd/MM/yyyy")} ";

                            }
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
        public static byte[] Pdf(ReporteAsistencia reporte)
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

                        Cell perioda= new();
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

                        nombre.Add(new Paragraph($"Empleado: {reporte.NombreEmpleado} - RUT: {reporte.RutEmpleado}"));
                        empresa.Add(new Paragraph($"Empresa: {reporte.NombreEmpresa} - RUT: {reporte.RutEmpresa}"));
                        sede.Add(new Paragraph($"Sede: {reporte.NombreSede} - Direccion: {reporte.DireccionDelaSede}"));
                        horario.Add(new Paragraph($"Turno: {reporte.NombreHorario} "));

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
                       


                        C01.Add(new Paragraph(" Fecha"));
                        C02.Add(new Paragraph(" Asistio"));
                        C03.Add(new Paragraph(" Observacion"));
                        



                        Cell C0 = new();
                        Cell C1 = new();
                        Cell C2 = new();
                        




                        Tabla = new Table(10);

                        Tabla.AddHeaderCell(C01);
                        Tabla.AddHeaderCell(C02);
                        Tabla.AddHeaderCell(C03);
                       



                        if(reporte.Recorrido != null && reporte.Recorrido.Count > 0)
                        foreach (var item in reporte.Recorrido)
                        {
                            C0.Add(new Paragraph(item.fecha));
                            C1.Add(new Paragraph(item.Asistio));
                            C2.Add(new Paragraph(item.Observacion));
                            Tabla.AddCell(C0);
                            Tabla.AddCell(C1);
                            Tabla.AddCell(C2);
                            }
                        doc.Add(white);
                        doc.Add(Tabla);



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
        public static byte[] Word(ReporteAsistencia reporte)
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

                    if (reporte.Recorrido != null && reporte.Recorrido.Count > 0)
                    {


                        IWTable tablas;
                        tablas = section.AddTable();
                        tablas.ResetCells(reporte.Recorrido.Count + 2, 3);


                        tablas[0, 0].AddParagraph().AppendText("fecha");
                        tablas[0, 1].AddParagraph().AppendText("Asistio");
                        tablas[0, 2].AddParagraph().AppendText("Observacion");


                        int i = 2;

                        foreach (var item in reporte.Recorrido)
                        {

                            tablas[i, 0].AddParagraph().AppendText(item.fecha);
                            tablas[i, 1].AddParagraph().AppendText(item.Asistio);
                            tablas[i, 2].AddParagraph().AppendText(item.Observacion);

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
