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

namespace AsisPas.Reportes.Jornada
{
    /// <summary>
    /// reporte de jornada segun complemento enviado
    /// </summary>
    public class ReportePorJornadaActual : CabeceraReportes
    {

        #region props
        /// <summary>
        /// para almacenar los totales por semana
        /// </summary>
        public List<TotalesJornada> semanas { get; set; }
        /// <summary>
        /// verifica el total del periodo
        /// </summary>
        public RTime TotalPeriodo { get; set; }

        #endregion


        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        public ReportePorJornadaActual()
        {
            this.semanas = new();
            this.TotalPeriodo = new(0);
        }

        #endregion


        #region up


        public async System.Threading.Tasks.Task Up(int idEmpleado, DateTime inicio, DateTime fin, ApplicationDbContext context)
        {
            try
            {
                // lleno mis fechas
                var first = await context.Marcaciones.FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado);
                await UpHead(idEmpleado, inicio, fin, first, context);
                if (first.id > 0 && first.marca > inicio)
                {
                    inicio = first.marca;
                    this.Mensaje = $"El Empleado no posee marcas antes de {first.marca.ToString("dd/MM/yyyy")}, Es Posible que no estuviera contratado";
                }


                var ah = await context.AdmoHorarios
                  .Include(x => x.Horario)
                  .Where(x => x.Empleadoid == idEmpleado && x.fin >= inicio && x.inicio <= fin)
                  .ToListAsync();

                DateTime inicioSem = inicio;
                double contSemanal = 0;
                double contPeriodo = 0;
                List<DiaJornada> fdias = new();
               

                for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
                {
                    var Haux = ah.Where(x => x.inicio <= i && x.fin >= i).ToList();
                    var Horario = Haux == null || Haux.Count < 1 ? null : Haux[0].Horario;
                    DiaJornada day = null;
                    var marcas = await Marca.MarcasDiaYUsuario(idEmpleado, i, context);
                    DateTime inicioTur;
                    DateTime finTur;
                    DateTime inicioDes;
                    DateTime findes;

                    try
                    {
                        inicioTur = marcas.FirstOrDefault(x => x.TipoIngreso == 0).marca;
                        finTur = marcas.FirstOrDefault(x => x.TipoIngreso == 3).marca;
                        inicioDes = marcas.FirstOrDefault(x => x.TipoIngreso == 1).marca;
                        findes = marcas.FirstOrDefault(x => x.TipoIngreso == 2).marca;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        inicioTur = new();
                        finTur = new();
                        inicioDes = new();
                        findes = new();
                    }
                    // verificamos que no tenemos un horario asignado
                    if (Horario == null || Horario.id < 0)
                    {
                        if(marcas != null && marcas.Count > 0
                            && inicioTur.ToString("dd/MM/yyyy") != "01/01/0001"
                            && finTur.ToString("dd/MM/yyyy") != "01/01/0001"
                            && inicioDes.ToString("dd/MM/yyyy") != "01/01/0001"
                            && findes.ToString("dd/MM/yyyy") != "01/01/0001")
                        {
                            var contHoy = RTime.CalcularTiempos(inicioTur, finTur, inicioDes, findes);
                            day = new DiaJornada() {
                                fecha = i,
                                horario = null,
                                marcas = new(inicioTur, finTur, inicioDes, findes),
                                tiempoLaborado =contHoy.toStr()
                            };

                            contSemanal += contHoy.TiempoEnSegundos;
                            contPeriodo += contHoy.TiempoEnSegundos;

                        }


                    }// en le caso de que el horario si exista
                    else
                    {
                        if (marcas != null && marcas.Count > 0
                            && inicioTur.ToString("dd/MM/yyyy") != "01/01/0001"
                            && finTur.ToString("dd/MM/yyyy") != "01/01/0001"
                            && inicioDes.ToString("dd/MM/yyyy") != "01/01/0001"
                            && findes.ToString("dd/MM/yyyy") != "01/01/0001")
                        {
                            var contHoy = RTime.CalcularTiempos(inicioTur, finTur, inicioDes, findes);
                            day = new DiaJornada()
                            {
                                fecha = i,
                                horario = new(Horario),
                                marcas = new(inicioTur, finTur, inicioDes, findes),
                                tiempoLaborado = RTime.CalcularTiemposToStr(inicioTur, finTur, inicioDes, findes)
                            };
                            contSemanal += contHoy.TiempoEnSegundos;
                            contPeriodo += contHoy.TiempoEnSegundos;
                        }
                        else
                            day = null;

                    }
                    //acumulo mis dias
                    if(day != null)
                        fdias.Add(day);
                    //cierro semanas

                    if(i.DayOfWeek == DayOfWeek.Saturday && i != fin)
                    {

                        this.semanas.Add(new(inicioSem, i, contSemanal, fdias));

                        inicioSem = i.AddDays(1);
                        fdias.Clear();
                        contSemanal = 0;



                    }



                    //cierro periodo
                    if(i == fin)
                    {
                        this.semanas.Add(new(inicioSem, i, contSemanal, fdias));
                        this.TotalPeriodo = new(contPeriodo);
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
        public static byte[] Excel(ReportePorJornadaActual reporte)
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

                        ew.Cells[7, 5].Value = "Tiempo total laborado en periodo";
                        ew.Cells[7, 6].Value = reporte.TotalPeriodo.toStr();

                        ew.Cells[8, 2].Value = "Totales Semanales";
                        ew.Cells[9, 1].Value = "Inicio";
                        ew.Cells[9, 2].Value = "fin";
                        ew.Cells[9, 3].Value = "Tiempo Cumplido";


                        if (reporte.semanas != null && reporte.semanas.Count > 0)
                            foreach (var item in reporte.semanas)
                            {
                                ew.Cells[fila, 1].Value = "Inicio";
                                ew.Cells[fila, 2].Value = "fin";
                                ew.Cells[fila, 3].Value = "Tiempo Cumplido";
                                fila++;
                                ew.Cells[fila, 1].Value = item.inicio.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = item.fin.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 3].Value = item.tiempo.toStr();
                                fila++;
                                ew.Cells[fila, 2].Value = "jornada pactada";
                                ew.Cells[fila, 6].Value = "jornada Laborada";
                                fila++;
                                ew.Cells[fila, 1].Value = "fecha";
                                ew.Cells[fila, 2].Value = "Hora inicio turno";
                                ew.Cells[fila, 3].Value = "Hora Fin turno";
                                ew.Cells[fila, 5].Value = "Hora Ingreso Descanso";
                                ew.Cells[fila, 4].Value = "Hora fin Descanso";
                                ew.Cells[fila, 6].Value = "Hora inicio turno";
                                ew.Cells[fila, 7].Value = "Hora fin turno";
                                ew.Cells[fila, 8].Value = "Hora inicio Descanso";
                                ew.Cells[fila, 9].Value = "Hora fin Descanso";
                                ew.Cells[fila, 10].Value = "Tiempo Laborado";
                                fila++;

                                foreach (var dia in item.Dario)
                                {
                                    ew.Cells[fila, 1].Value = dia.fecha.ToString("dd/MM/yyyy");
                                    if(dia.horario == null || dia.horario.InicioJornada == dia.horario.FinDescazo)
                                    {
                                        ew.Cells[fila, 2].Value = "No Hay Horario Asignado para este dia";
                                    }
                                    else
                                    {
                                        ew.Cells[fila, 2].Value = dia.horario.InicioJornada;
                                        ew.Cells[fila, 3].Value = dia.horario.FinJornada;
                                        ew.Cells[fila, 4].Value = dia.horario.InicioDescanzo;
                                        ew.Cells[fila, 5].Value = dia.horario.FinDescazo;
                                    }

                                    if(dia.marcas == null || dia.marcas.InicioJornada == dia.marcas.FinDescazo)
                                    {
                                        ew.Cells[fila, 2].Value = "No Hay Marcas Validas para este dia";
                                    }
                                    else
                                    {

                                    ew.Cells[fila, 6].Value = dia.marcas.InicioJornada;
                                    ew.Cells[fila, 7].Value = dia.marcas.FinJornada;
                                    ew.Cells[fila, 8].Value = dia.marcas.InicioDescanzo;
                                    ew.Cells[fila, 9].Value = dia.marcas.FinDescazo;
                                    }
                                    ew.Cells[fila, 10].Value = dia.tiempoLaborado;
                                    fila++;
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
        public static byte[] Pdf(ReportePorJornadaActual reporte)
        {

            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new(ms);
                    using (PdfDocument document = new(writer))
                    {
                        Document doc = new(document);
                        Paragraph titulo = new Paragraph($" REPORTE POR ASISTENCIAS \n Reporte Generado el {DateTime.Now:dd/MM/yyyy}");
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



                        if (reporte.semanas != null && reporte.semanas.Count > 0)
                            foreach (var item in reporte.semanas)
                            {
                                //agrego resumen semanal
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



                                C01.Add(new Paragraph(" inicio"));
                                C02.Add(new Paragraph(" fin"));
                                C03.Add(new Paragraph(" tiempo Cumplido"));




                             

                                Tabla = new Table(10);

                                Tabla.AddHeaderCell(C01);
                                Tabla.AddHeaderCell(C02);
                                Tabla.AddHeaderCell(C03);


                                Cell C0 = new();
                                Cell C1 = new();
                                Cell C2 = new();
                                C0.Add(new Paragraph(item.inicio.ToString("dd/MM/yyyy")));
                                C1.Add(new Paragraph(item.fin.ToString("dd/MM/yyyy")));
                                C2.Add(new Paragraph(item.tiempo.toStr()));
                                Tabla.AddCell(C0);
                                Tabla.AddCell(C1);
                                Tabla.AddCell(C2);
                                doc.Add(white);
                                doc.Add(Tabla);
                                //agrego resumen diario

                                Table Tabla2;


                                Cell C012 = new();
                                Cell C022 = new();
                                Cell C032 = new();
                                Cell C042 = new();
                                Cell C052 = new();
                                Cell C062 = new();
                                Cell C072 = new();
                                Cell C082 = new();
                                Cell C092 = new();
                                Cell C010 = new();



                                C012.Add(new Paragraph(" fecha"));
                                C022.Add(new Paragraph(" Inicio Pautado"));
                                C032.Add(new Paragraph(" fin Pautado"));
                                C042.Add(new Paragraph(" inicia Descanso Pautado"));
                                C052.Add(new Paragraph(" fin Descanso Pautado"));
                                C062.Add(new Paragraph(" Inicio Cumplido"));
                                C072.Add(new Paragraph(" fin Cumplido"));
                                C082.Add(new Paragraph(" inicia Descanso Cumplido"));
                                C092.Add(new Paragraph(" fin Descanso Cumplido"));
                                C010.Add(new Paragraph(" Tiempo Cumplido"));




                                Cell C0x = new();
                                Cell C1x = new();
                                Cell C2x = new();
                                Cell C3x = new();
                                Cell C4x = new();
                                Cell C5x = new();
                                Cell C6x = new();
                                Cell C7x = new();
                                Cell C8x = new();
                                Cell C9x = new();

                                Tabla2 = new Table(10);
                                Tabla2.AddHeaderCell(C012);
                                Tabla2.AddHeaderCell(C022);
                                Tabla2.AddHeaderCell(C032);
                                Tabla2.AddHeaderCell(C042);
                                Tabla2.AddHeaderCell(C052);
                                Tabla2.AddHeaderCell(C062);
                                Tabla2.AddHeaderCell(C072);
                                Tabla2.AddHeaderCell(C082);
                                Tabla2.AddHeaderCell(C092);
                                Tabla2.AddHeaderCell(C010);

                                foreach (var d in item.Dario)
                                {
                                    C0x.Add(new Paragraph(d.fecha.ToString("dd/MM/yyyy")));
                                    if(d.horario == null || d.horario.InicioJornada == d.horario.FinDescazo)
                                    {
                                        C1x.Add(new Paragraph("Sin Horario"));
                                        C2x.Add(new Paragraph("Sin Horario"));
                                        C3x.Add(new Paragraph("Sin Horario"));
                                        C4x.Add(new Paragraph("Sin Horario"));

                                    }
                                    else
                                    {
                                        C1x.Add(new Paragraph(d.horario.InicioJornada));
                                        C2x.Add(new Paragraph(d.horario.FinJornada));
                                        C3x.Add(new Paragraph(d.horario.InicioDescanzo));
                                        C4x.Add(new Paragraph(d.horario.FinDescazo));

                                    }
                                    if(d.marcas == null || d.marcas.InicioJornada == d.marcas.FinDescazo)
                                    {
                                       C5x.Add(new Paragraph("No Registro"));
                                       C6x.Add(new Paragraph("No Registro"));
                                       C7x.Add(new Paragraph("No Registro"));
                                       C8x.Add(new Paragraph("No Registro"));
                                   }
                                    else
                                    {
                                       C5x.Add(new Paragraph(d.marcas.InicioJornada));
                                       C6x.Add(new Paragraph(d.marcas.FinJornada));
                                       C7x.Add(new Paragraph(d.marcas.InicioDescanzo));
                                       C8x.Add(new Paragraph(d.marcas.FinDescazo));
                                    }
                                    
                                    C9x.Add(new Paragraph(d.tiempoLaborado));



                                }
                                    Tabla2.AddCell(C0x);
                                    Tabla2.AddCell(C1x);
                                    Tabla2.AddCell(C2x);
                                    Tabla2.AddCell(C3x);
                                    Tabla2.AddCell(C4x);
                                    Tabla2.AddCell(C5x);
                                    Tabla2.AddCell(C6x);
                                    Tabla2.AddCell(C7x);
                                    Tabla2.AddCell(C8x);
                                    Tabla2.AddCell(C9x);


                                    doc.Add(white);
                                    doc.Add(Tabla2);
                            }


                        doc.Add(white);
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
        public static byte[] Word(ReportePorJornadaActual reporte)
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


                    IWParagraph totalPer = section.AddParagraph();
                    totalPer.ApplyStyle("Normal");
                    totalPer.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange tp = Horario.AppendText($"Tiempo Total del periodo: {reporte.TotalPeriodo} \n") as WTextRange;



                    //semanal

                    if (reporte.semanas != null && reporte.semanas.Count > 0)
                    {


                        foreach (var item in reporte.semanas)
                        {


                            IWParagraph rsem = section.AddParagraph();
                            rsem.ApplyStyle("Normal");
                            rsem.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                            WTextRange rzeno = Horario.AppendText($"Semana del {item.inicio.ToString("dd/MM/yyyy")} al {item.fin.ToString("dd/MM/yyyy")}\n Total de tiempo Laborado {item.tiempo.toStr()}") as WTextRange;



                            IWTable tday;
                            tday = section.AddTable();
                            tday.ResetCells(item.Dario.Count + 3, 6);


                            tday[0, 0].AddParagraph().AppendText("fecha");
                            tday[0, 1].AddParagraph().AppendText("Ingreso Pautado");
                            tday[0, 2].AddParagraph().AppendText("Descanso Pautado");
                            tday[0, 3].AddParagraph().AppendText("Ingresos marcados");
                            tday[0, 4].AddParagraph().AppendText("Descansos marcados");
                            tday[0, 5].AddParagraph().AppendText("Tiempo Laborado");


                            int i = 2;

                            foreach (var d in item.Dario)
                            {
                                tday[i, 0].AddParagraph().AppendText(d.fecha.ToString("dd/MM/yyyy"));
                                if(d.horario == null || d.horario.InicioJornada == d.horario.FinDescazo)
                                {
                                    tday[i, 1].AddParagraph().AppendText("No Horario");
                                    tday[i, 2].AddParagraph().AppendText("Asignado");
                                }
                                else
                                {
                                    tday[i, 1].AddParagraph().AppendText($"{d.horario.InicioJornada} - { d.horario.FinJornada}");
                                    tday[i, 2].AddParagraph().AppendText($"{d.horario.InicioDescanzo} - { d.horario.FinDescazo}");
                                }

                                if(d.marcas == null || d.marcas.InicioJornada == d.marcas.FinDescazo)
                                {
                                    tday[i, 3].AddParagraph().AppendText("No marcas");
                                    tday[i, 4].AddParagraph().AppendText("validas");
                                }
                                else
                                {

                                    tday[i, 3].AddParagraph().AppendText($"{d.marcas.InicioJornada} - { d.marcas.FinJornada}");
                                    tday[i, 4].AddParagraph().AppendText($"{d.marcas.InicioDescanzo} - { d.marcas.FinDescazo}");
                                }
                                tday[i, 5].AddParagraph().AppendText($"{d.tiempoLaborado}");
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
