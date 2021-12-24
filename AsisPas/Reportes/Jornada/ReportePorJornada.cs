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
    /// para generar el reporte por jornada
    /// </summary>
    public class ReportePorJornada : CabeceraReportes
    {
        #region props
        /// <summary>
        /// sobre los dias totales de estudio
        /// </summary>
        public List<DiaJornada> Dias { get; set; }
        /// <summary>
        /// para almacenar los totales por semana
        /// </summary>
        public List<TotalesJornada> TotalesSemanales { get; set; }
        /// <summary>
        /// verifica el total del periodo
        /// </summary>
        public string TotalPeriodo { get; set; }

        #endregion

        #region constructor
        /// <summary>
        /// constructor
        /// </summary>
        public ReportePorJornada()
        {
            this.Dias = new List<DiaJornada>();
            this.TotalesSemanales = new List<TotalesJornada>();
        }


        #endregion

        #region llenarData
        /// <summary>
        /// para llenar los datos del reporte
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
                var first = await context.Marcaciones.FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado);
                await UpHead(idEmpleado, inicio, fin,first, context);
                if(first.marca > inicio)
                    inicio = first.marca;


                var ah = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado && x.fin >= inicio && x.inicio <= fin)
                    .ToListAsync();

                DateTime inicioSem = new();
                double contSem = 0;
                double contPer = 0;

                if (first == null || first.id < 1)
                    this.Mensaje = "El Empleado aun no posee marcas en el periodo indicado, por tanto no se puede hocer el estudio.";
                else
                for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
                {
                    if (i == inicio)
                        inicioSem = i;
                    
                    var Haux = ah.Where(x => x.inicio<= i && x.fin >= i).ToList();
                    var Horario = Haux == null || Haux.Count < 1 ? null : Haux[0].Horario;

                    if(Horario != null && Horario.id > 0)
                    {
                            if (Horario.DiaLaboral((int)i.DayOfWeek))
                            {
                                var marcas = await Marca.MarcasDiaYUsuario(idEmpleado, i, context);

                                if (marcas == null || marcas.Count < 4)
                                {
                                    Dias.Add(new()
                                    {
                                        fecha = i,
                                        horario = new()
                                        {
                                            FinDescazo = Horario.hbf,
                                            FinJornada = Horario.hf,
                                            InicioDescanzo = Horario.hbi,
                                            InicioJornada = Horario.hi,
                                        },
                                        marcas = null,
                                        tiempoLaborado = new RTime(0).toStr()
                                    });
                                }
                                else
                                {



                                    DateTime inicioTur;
                                    DateTime finTur ;
                                    DateTime inicioDes ;
                                    DateTime findes ;

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

                                 

                                    if (inicioTur.ToString("dd/MM/yyyy") == "01/01/0001" || finTur.ToString("dd/MM/yyyy") == "01/01/0001" || inicioDes.ToString("dd/MM/yyyy") == "01/01/0001" || findes.ToString("dd/MM/yyyy") == "01/01/0001")
                                    {
                                        //esta parte la voy a ignorar pero la dejare aqui por si la necesito
                                        //Dias.Add(new()
                                        //{
                                        //    fecha = i,
                                        //    horario = new()
                                        //    {
                                        //        FinDescazo = Horario.hbf,
                                        //        FinJornada = Horario.hf,
                                        //        InicioDescanzo = Horario.hbi,
                                        //        InicioJornada = Horario.hi,
                                        //    },
                                        //    marcas = null,
                                        //    tiempoLaborado = new RTime(0).toStr()
                                        //});
                                    }
                                    else
                                    {

                                        double tiempoHoy = RTime.ObtenerTotalSg(inicioTur, finTur, inicioDes, findes);

                                        contSem += tiempoHoy;
                                        contPer += tiempoHoy;

                                        Dias.Add(new()
                                        {
                                            fecha = i,
                                            horario = new()
                                            {
                                                FinDescazo = Horario.hbf,
                                                FinJornada = Horario.hf,
                                                InicioDescanzo = Horario.hbi,
                                                InicioJornada = Horario.hi,
                                            },
                                            marcas = new()
                                            {
                                                InicioJornada = inicioTur.ToString("HH:mm:ss"),
                                                FinJornada = finTur.ToString("HH:mm:ss"),
                                                InicioDescanzo = inicioDes.ToString("HH:mm:ss"),
                                                FinDescazo = findes.ToString("HH:mm:ss"),
                                            },
                                            tiempoLaborado = new RTime(tiempoHoy).toStr()
                                        });


                                   

                                    }
                                }
                            }
                    }

                    //cierres
                        if ((int)i.DayOfWeek == 6 && i != fin)
                        {
                            var band = contSem;
                            //this.TotalesSemanales.Add(new(inicioSem, i, band));
                            contSem = 0;
                            inicioSem = i.AddDays(1);
                        }
                        if (i == fin)
                        {
                            //this.TotalesSemanales.Add(new(inicioSem, i, contSem));
                            RTime flag = new(contPer);
                            this.TotalPeriodo = flag.toStr();
                        }

                    }
                if (Dias.Count < 1)
                    this.Mensaje = "No Hay Horario Asignado, o el Empleado no se encontraba laborando en la empresa papa este periodo";
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
        public static byte[] Excel(ReportePorJornada reporte)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage ep = new ExcelPackage())
                    {
                        int fila = 10;
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
                        ew.Cells[7, 6].Value = reporte.TotalPeriodo;

                        ew.Cells[8, 2].Value = "Totales Semanales";
                        ew.Cells[9, 1].Value = "Inicio";
                        ew.Cells[9, 2].Value = "fin";
                        ew.Cells[9, 3].Value = "Tiempo Cumplido";


                        if (reporte.TotalesSemanales != null && reporte.TotalesSemanales.Count > 0)
                            foreach (var item in reporte.TotalesSemanales)
                            {
                                ew.Cells[fila, 1].Value = item.inicio.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = item.fin.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 3].Value = item.tiempo.toStr();
                              
                                fila++;
                            }


                        fila+=2;

                        ew.Cells[fila, 2].Value = "Estudio por dias";
                        fila++;
                        ew.Cells[fila, 2].Value = "Horario";
                        ew.Cells[fila, 6].Value = "Marcas";
                        fila++;
                        ew.Cells[fila, 1].Value = "fecha";
                        ew.Cells[fila, 2].Value = "Hora Ingreso turno";
                        ew.Cells[fila, 3].Value = "Hora Fin turno";
                        ew.Cells[fila, 4].Value = "Hora Fin Descanso";
                        ew.Cells[fila, 5].Value = "Hora Ingreso Descanso";
                        ew.Cells[fila, 6].Value = "Hora Ingreso turno";
                        ew.Cells[fila, 7].Value = "Hora Fin turno";
                        ew.Cells[fila, 8].Value = "Hora Fin Descanso";
                        ew.Cells[fila, 9].Value = "Hora Ingreso Descanso";
                        ew.Cells[fila, 10].Value = "Tiempo Laborado";

                        if (reporte.Dias != null && reporte.Dias.Count > 0)
                            foreach (var item in reporte.Dias)
                            {
                                ew.Cells[fila, 1].Value = item.fecha.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = item.horario.InicioJornada;
                                ew.Cells[fila, 3].Value = item.horario.FinJornada;
                                ew.Cells[fila, 4].Value = item.horario.InicioDescanzo;
                                ew.Cells[fila, 5].Value = item.horario.FinDescazo;
                                ew.Cells[fila, 6].Value = item.marcas.InicioJornada;
                                ew.Cells[fila, 7].Value = item.marcas.FinJornada;
                                ew.Cells[fila, 8].Value = item.marcas.InicioDescanzo;
                                ew.Cells[fila, 9].Value = item.marcas.FinDescazo;
                                ew.Cells[fila, 10].Value = item.tiempoLaborado;

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
        public static byte[] Pdf(ReportePorJornada reporte)
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




                        Cell semanales = new();
                        semanales.Add(new Paragraph("Totales semanales"));
                        doc.Add(semanales);


                        Tabla = new Table(10);

                        Tabla.AddHeaderCell(C01);
                        Tabla.AddHeaderCell(C02);
                        Tabla.AddHeaderCell(C03);


                        Cell C0 = new();
                        Cell C1 = new();
                        Cell C2 = new();



                        if (reporte.TotalesSemanales != null && reporte.TotalesSemanales.Count > 0)
                            foreach (var item in reporte.TotalesSemanales)
                            {
                                C0.Add(new Paragraph(item.inicio.ToString("dd/MM/yyyy")));
                                C1.Add(new Paragraph(item.fin.ToString("dd/MM/yyyy")));
                                C2.Add(new Paragraph(item.tiempo.toStr()));
                            }
                                Tabla.AddCell(C0);
                                Tabla.AddCell(C1);
                                Tabla.AddCell(C2);
                        doc.Add(white);
                        doc.Add(Tabla);

                        doc.Add(white);
                        doc.Add(white);


                        //diarios

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

                        Cell Diarios = new();
                        Diarios.Add(new Paragraph("Estudio Diario"));
                        doc.Add(Diarios);


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




                        if (reporte.Dias != null && reporte.Dias.Count > 0)
                            foreach (var d in reporte.Dias)
                            {
                                C0x.Add(new Paragraph(d.fecha.ToString("dd/MM/yyyy")));
                                C1x.Add(new Paragraph(d.horario.InicioJornada));
                                C2x.Add(new Paragraph(d.horario.FinJornada));
                                C3x.Add(new Paragraph(d.horario.InicioDescanzo));
                                C4x.Add(new Paragraph(d.horario.FinDescazo));
                                C5x.Add(new Paragraph(d.marcas.InicioJornada));
                                C6x.Add(new Paragraph(d.marcas.FinJornada));
                                C7x.Add(new Paragraph(d.marcas.InicioDescanzo));
                                C8x.Add(new Paragraph(d.marcas.FinDescazo));
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
        public static byte[] Word(ReportePorJornada reporte)
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

                    if (reporte.TotalesSemanales != null && reporte.TotalesSemanales.Count > 0)
                    {


                        IWTable tablas;
                        tablas = section.AddTable();
                        tablas.ResetCells(reporte.TotalesSemanales.Count + 2, 3);


                        tablas[0, 0].AddParagraph().AppendText("inicio");
                        tablas[0, 1].AddParagraph().AppendText("fin");
                        tablas[0, 2].AddParagraph().AppendText("Total tiempo laborado");


                        int i = 2;

                        foreach (var item in reporte.TotalesSemanales)
                        {

                            tablas[i, 0].AddParagraph().AppendText(item.inicio.ToString("dd/MM/yyyy"));
                            tablas[i, 1].AddParagraph().AppendText(item.fin.ToString("dd/MM/yyyy"));
                            tablas[i, 2].AddParagraph().AppendText(item.tiempo.toStr());

                            i++;
                        }
                    }




                    //diario


                    if (reporte.Dias != null && reporte.Dias.Count > 0)
                    {


                        IWTable tablas;
                        tablas = section.AddTable();
                        tablas.ResetCells(reporte.Dias.Count + 2, 6);


                        tablas[0, 0].AddParagraph().AppendText("fecha");
                        tablas[0, 1].AddParagraph().AppendText("Ingreso Pautado");
                        tablas[0, 2].AddParagraph().AppendText("Descanso Pautado");
                        tablas[0, 3].AddParagraph().AppendText("Ingresos marcados");
                        tablas[0, 4].AddParagraph().AppendText("Descansos marcados");
                        tablas[0, 5].AddParagraph().AppendText("Tiempo Laborado");


                        int i = 2;

                        foreach (var item in reporte.Dias)
                        {

                            tablas[i, 0].AddParagraph().AppendText(item.fecha.ToString("dd/MM/yyyy"));
                            tablas[i, 1].AddParagraph().AppendText($"{item.horario.InicioJornada} - { item.horario.FinJornada}");
                            tablas[i, 2].AddParagraph().AppendText($"{item.horario.InicioDescanzo} - { item.horario.FinDescazo}");
                            tablas[i, 3].AddParagraph().AppendText($"{item.marcas.InicioJornada} - { item.marcas.FinJornada}");
                            tablas[i, 4].AddParagraph().AppendText($"{item.marcas.InicioDescanzo} - { item.marcas.FinDescazo}");
                            tablas[i, 5].AddParagraph().AppendText($"{item.tiempoLaborado}");

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

