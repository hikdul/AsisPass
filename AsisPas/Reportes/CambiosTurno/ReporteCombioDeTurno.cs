using AsisPas.Data;
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

        /// <summary>
        /// constructor
        /// </summary>
        public ReporteCombioDeTurno()
        {
            this.cambios = new List<DiaCambio>();   
        }

        #region llenar data
        public async System.Threading.Tasks.Task Up(int idEmpleado, DateTime inicio, DateTime fin, ApplicationDbContext context)
        {
            try {
                await UpHead(idEmpleado, inicio, fin, context);

                var admos = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x =>
                    ( (x.inicio >= inicio || x.fin <= inicio)
                    || (x.inicio >= fin || x.inicio <= fin))
                    && x.Empleadoid == idEmpleado
                    )
                    .ToListAsync();



                var primeros = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado && x.fin <= inicio)
                    .ToListAsync();

                var primer = primeros == null || primeros.Count < 1 ? null : primeros[primeros.Count - 1];


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
                    this.cambios.Add(diaI);
                }
            }catch (Exception ex) 
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
        public static byte[] Excel(ReporteCombioDeTurno reporte)
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

                        ew.Cells[8, 1].Value = "Fecha Asignacion turno anterior";
                        ew.Cells[8, 2].Value = "Nombre Horario";
                        ew.Cells[8, 3].Value = "Fecha Asignacion turno Nuevo";
                        ew.Cells[8, 4].Value = "Nombre Horario";
                        ew.Cells[8, 5].Value = "Solicitante de cambio";
                        ew.Cells[8, 6].Value = "Observaciones";


                        if (reporte.cambios != null && reporte.cambios.Count > 0)
                            foreach (var item in reporte.cambios)
                            {
                                if (item.Anterior == null)
                                {
                                    ew.Cells[fila, 2].Value = "Primer horario asignado";
                                }
                                else
                                {
                                    ew.Cells[fila, 1].Value = item.Anterior.Horario.Nombre;
                                    ew.Cells[fila, 2].Value = item.Anterior.Modificacion.ToString("dd/MM/yyyy");

                                }

                                ew.Cells[fila, 3].Value = item.nuevo.Horario.Nombre;
                                ew.Cells[fila, 4].Value = item.nuevo.Modificacion.ToString("dd/MM/yyyy");

                                ew.Cells[fila, 5].Value = item.SolicitanteCambio;
                                ew.Cells[fila, 5].Value = item.Desc;

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
        public static byte[] Pdf(ReporteCombioDeTurno reporte)
        {

            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new(ms);
                    using (PdfDocument document = new(writer))
                    {
                        Document doc = new(document);
                        Paragraph titulo = new Paragraph($" REPORTE POR CAMBIOS DE TURNO \n Reporte Generado el ${DateTime.Now:dd/MM/yyyy}");
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



                        C01.Add(new Paragraph(" Fecha turno anterior"));
                        C02.Add(new Paragraph(" nombre turno anterior"));
                        C03.Add(new Paragraph(" Fecha turno nuevo"));
                        C04.Add(new Paragraph(" nombre turno nuevo"));
                        C05.Add(new Paragraph(" solicitante de cambio"));
                        C06.Add(new Paragraph(" Observacion"));




                        Cell C0 = new();
                        Cell C1 = new();
                        Cell C2 = new();
                        Cell C3 = new();
                        Cell C4 = new();
                        Cell C5 = new();





                        Tabla = new Table(10);

                        Tabla.AddHeaderCell(C01);
                        Tabla.AddHeaderCell(C02);
                        Tabla.AddHeaderCell(C03);
                        Tabla.AddHeaderCell(C04);
                        Tabla.AddHeaderCell(C05);
                        Tabla.AddHeaderCell(C06);




                        if (reporte.cambios != null && reporte.cambios.Count > 0)
                            foreach (var item in reporte.cambios)
                            {
                                if(item.Anterior == null)
                                {
                                    C0.Add(new Paragraph("Primer Turno Asignado"));
                                    C1.Add(new Paragraph("Primer Turno Asignado"));
                                }
                                else
                                {
                                C0.Add(new Paragraph(item.Anterior.Modificacion.ToString("dd/MM/yyyy")));
                                C1.Add(new Paragraph(item.Anterior.Horario.Nombre));

                                }
                                C2.Add(new Paragraph(item.nuevo.Modificacion.ToString("dd/MM/yyyy")));
                                C3.Add(new Paragraph(item.nuevo.Horario.Nombre));

                                C4.Add(new Paragraph(item.SolicitanteCambio));
                                C5.Add(new Paragraph(item.Desc));
                            }
                                Tabla.AddCell(C0);
                                Tabla.AddCell(C1);
                                Tabla.AddCell(C2);
                                Tabla.AddCell(C3);
                                Tabla.AddCell(C4);
                                Tabla.AddCell(C5);
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
        public static byte[] Word(ReporteCombioDeTurno reporte)
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

                    if (reporte.cambios != null && reporte.cambios.Count > 0)
                    {


                        IWTable tablas;
                        tablas = section.AddTable();
                        tablas.ResetCells(reporte.cambios.Count + 3, 6);


                        tablas[0, 0].AddParagraph().AppendText("fecha Asignacion anterior");
                        tablas[0, 1].AddParagraph().AppendText("nombre Asignacion anterior");
                        tablas[0, 2].AddParagraph().AppendText("fecha Asignacion anterior");
                        tablas[0, 3].AddParagraph().AppendText("nombre Asignacion anterior");
                        tablas[0, 4].AddParagraph().AppendText("solicitante cambio");
                        tablas[0, 5].AddParagraph().AppendText("descripcion");


                        int i = 2;

                        foreach (var item in reporte.cambios)
                        {
                            if (item.Anterior == null)
                            {
                                tablas[i, 0].AddParagraph().AppendText("Primer Horario Asignado");
                                tablas[i, 1].AddParagraph().AppendText("--");
                            }
                            else
                            {
                                tablas[i, 0].AddParagraph().AppendText(item.Anterior.Modificacion.ToString("dd/MM/yyyy"));
                                tablas[i, 1].AddParagraph().AppendText(item.Anterior.Horario.Nombre);
                            }
                            tablas[i, 2].AddParagraph().AppendText(item.nuevo.Modificacion.ToString("dd/MM/yyyy"));
                            tablas[i, 3].AddParagraph().AppendText(item.nuevo.Horario.Nombre);
                            tablas[i, 4].AddParagraph().AppendText(item.SolicitanteCambio);
                            tablas[i, 5].AddParagraph().AppendText(item.Desc);

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
