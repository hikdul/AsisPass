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

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// para generar el reporte para domingos y feriados
    /// obteniendo solo el id de empleado y el rango
    /// </summary>
    public class ReporteDomingosYFeriados : CabeceraReportes
    {

        #region prop

        /// <summary>
        /// total domingos del periodo
        /// </summary>
        public int TotalDomingosPeriodo { get; set; }
        /// <summary>
        /// laborados
        /// </summary>
        public int TotalDomingos { get; set; }
        /// <summary>
        /// total feriados del periodo
        /// </summary>
        public int TotalFeriadosPeriodo { get; set; }
        /// <summary>
        /// laborados
        /// </summary>
        public int TotalFeriadosLaborados { get; set; }

        /// <summary>
        /// calculos totales por ano
        /// </summary>
        public List<RangoTotalesDomingo> totalesPorAno{ get; set; }

        /// <summary>
        /// lleva el registro mes a mes
        /// </summary>
        public List<MensualDomingos> RegitroMensual { get; set; }


        #endregion


        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        public ReporteDomingosYFeriados()
        {
            this.totalesPorAno = new();
            this.RegitroMensual = new();
        }

        #endregion

        #region llenado de los datos
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
                // ### LLENO LA CABECERA
                await UpHead(idEmpleado, inicio, fin, context);

                // ## COMPLETO EL ROPORTE
                int laboradosD = 0;
                int periodoD = 0;
                int anoD = 0;
                int mesD = 0;
                int totalmesD = 0;
                int totalanoD = 0;
                int periodoF = 0;
                int laboradosF = 0;
                int anoF = 0;
                int mesF = 0;
                int totalanoF = 0;
                int totalmesF = 0;

                DateTime anoI = new DateTime();
                DateTime mesI = new DateTime();
                int mesActual = 0;
                int anoActual = 0;
                List<DiaDomingos> diario = new();


                var feriados = await Feriado.listado(context);

                for (var i = inicio; i <= fin; i = i.AddDays(1))
                {
                    //obtengo horario
                    Horario horario = await Horario.HorarioPorIdEmpleado(idEmpleado, i, context);
                    //almacen y cambio de fechos en interaccion
                    if(i == inicio)
                    {
                        anoI = i;
                        mesI = i;
                        mesActual = i.Month;
                        anoActual = i.Year;
                    }

                    if(mesActual != i.Month)
                    {
                        var inincio = mesI;
                        var finalizo = i.AddDays(-1);
                        int mes = mesD;
                        int mess = mesF;
                        int Tmes = totalmesD;
                        int Tmess = totalmesF;
                        List<DiaDomingos> dias = new();
                        foreach (var d in diario)
                        {
                            dias.Add(d);
                        }
                        MensualDomingos bandera = new MensualDomingos()
                        {

                            Diario = dias,
                            inicio = inincio,
                            fin = finalizo,
                            totalDomingos = mes,
                            totalFeriados = mess,
                            totalDomingosenPeriodo = Tmes,
                            totalFeriadosenPeriodo = Tmess,
                        };
                        this.RegitroMensual.Add(bandera);
                        mesActual = i.Month;
                        mesD = 0;
                        mesF = 0;
                        totalmesD = 0;
                        totalmesF = 0;
                        mesI = i;
                        diario.Clear();
                    }

                    if (anoActual != i.Year)
                    {
                        var inincio = anoI;
                        var finalizo = i.AddDays(-1);
                        int ano = anoD;
                        int anoo = anoF;
                        int Tano = totalanoD;
                        int Tanoo = totalanoF;
                        this.totalesPorAno.Add(new() { 
                            inicio = inincio,
                            fin = finalizo,
                            totalDomingos = ano,
                            totalFeriados = anoo,
                            totalDomingosenPeriodo=Tano,
                            totalFeriadosenPeriodo=Tanoo
                        });

                        anoActual = i.Year;
                        anoD = 0;
                        anoF = 0;
                        totalanoD = 0;
                        totalanoF = 0;
                        anoI = i;
                    }

                    //Calculo y sumas para Domingos

                    if ((int) i.DayOfWeek == 0)
                    {
                        periodoD++;
                        totalanoD++;
                        totalmesD++;
                        
                        
                        var fecha = i.ToString("dd/MM/yyyy");
                        var mi = await context.Marcaciones.FirstOrDefaultAsync(x => 
                        x.Empleadoid == idEmpleado
                        && x.marca.Year == i.Year
                        && x.marca.Month == i.Month
                        && x.marca.Year == i.Year
                        );
                        var laboral = horario.DiaLaboral((int)i.DayOfWeek);
                        laboradosD++;
                        if(mi != null)
                        {
                          
                            anoD++;
                            mesD++;
                            diario.Add(new()
                            {
                                Asistio = "SI",
                                fecha = i,
                                Observacion = laboral == true ? "" : "Dia De Descansa" 
                            });
                        }

                        if(mi == null && laboral)
                        {
                            var permiso = await context.Permisos
                                .Where(x => x.Empleadoid == idEmpleado && x.inicio <= i && x.fin >= i)
                                .FirstOrDefaultAsync();
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = permiso == null || permiso.id < 1 ? "No Hay Justificacion Para La Falta" : permiso.Desc
                            });
                        }

                        if (mi == null && !laboral)
                        {
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = "Dia de Descaso, Segun Horario"
                            });
                        }

                    }

                    //Calculo y sumas para Feriados
                    var fechaFeriado = i.ToString("dd/MM");
                    if(feriados.Find(x => x.fecha.ToString("dd/MM") == fechaFeriado) != null)
                    {
                        periodoF++;
                        totalanoF++;
                        totalmesF++;
                        var laboral = horario.DiaLaboral(i.Day);
                        var mi = await context.Marcaciones
                            .FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado
                            && x.marca.Year == i.Year
                            && x.marca.Month == i.Month
                            && x.marca.Day == i.Day);
                            

                        if (mi != null)
                        {

                            laboradosF++;
                            anoF++;
                            mesF++;
                            diario.Add(new()
                            {
                                Asistio = "SI",
                                fecha = i,
                                Observacion = "",
                            });
                        }

                        if (mi == null && laboral)
                        {
                            var permiso = await context.Permisos
                                .Where(x => x.Empleadoid == idEmpleado && x.inicio <= i && x.fin >= i)
                                .FirstOrDefaultAsync();
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = permiso == null || permiso.id < 1 ? "Descanzo Por Feriado" : permiso.Desc
                            });
                        }


                        if (mi == null && !laboral)
                        {
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = "Descanzo Por Feriado, dia no laboral"
                            });
                        }

                    }

                    // finaliza el periodo -- creo que hay que agregar el cierre de mes y de ano
                    if(i == fin)
                    {
                        //total periodo
                        this.TotalDomingosPeriodo = periodoD;
                        this.TotalFeriadosPeriodo = periodoF;
                        this.TotalDomingos = laboradosD;
                        this.TotalFeriadosLaborados = laboradosF;
                        //total mes
                        this.RegitroMensual.Add(new()
                        {
                            Diario = diario,
                            inicio = mesI,
                            fin = i,
                            totalDomingos = mesD,
                            totalFeriados = mesF,
                            totalDomingosenPeriodo = totalmesD,
                            totalFeriadosenPeriodo = totalmesF,
                        });
                        //total ano
                        this.totalesPorAno.Add(new()
                        {
                            inicio = anoI,
                            fin = i,
                            totalDomingos = anoD,
                            totalFeriados = anoF,
                            totalDomingosenPeriodo = totalanoD,
                            totalFeriadosenPeriodo = totalanoF
                        });

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
        public static byte[] Excel(ReporteDomingosYFeriados reporte)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage ep = new ExcelPackage())
                    {
                        int fila = 11;
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

                        ew.Cells[8, 1].Value = "Domingos";
                        ew.Cells[8, 2].Value = "En Perioado";
                        ew.Cells[8, 3].Value = reporte.TotalDomingosPeriodo;
                        ew.Cells[8, 4].Value = "Laborados";
                        ew.Cells[8, 5].Value = reporte.TotalDomingos;


                        ew.Cells[9, 1].Value = "Feriados";
                        ew.Cells[9, 2].Value = "En Perioado";
                        ew.Cells[9, 3].Value = reporte.TotalFeriadosPeriodo;
                        ew.Cells[9, 4].Value = "Laborados";
                        ew.Cells[9, 5].Value = reporte.TotalFeriadosLaborados;


                        if (reporte.totalesPorAno != null && reporte.totalesPorAno.Count > 0)
                        {
                            ew.Cells[fila, 1].Value = "Datos Anuales";
                            fila++;

                            ew.Cells[fila, 1].Value = "Inicio";
                            ew.Cells[fila, 2].Value = "fin";
                            ew.Cells[fila, 3].Value = "Domingos en periodo";
                            ew.Cells[fila, 4].Value = "Domingos laborados en periodo";
                            ew.Cells[fila, 5].Value = "Feriados en periodo";
                            ew.Cells[fila, 6].Value = "Feriados laborados en periodo";
                            fila++;
                            fila++;

                            foreach (var anual in reporte.totalesPorAno)
                            {
                                ew.Cells[fila, 1].Value = anual.inicio.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = anual.fin.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 3].Value = anual.totalDomingosenPeriodo;
                                ew.Cells[fila, 4].Value = anual.totalDomingos;
                                ew.Cells[fila, 5].Value = anual.totalFeriadosenPeriodo;
                                ew.Cells[fila, 6].Value = anual.totalFeriados;
                                fila++;

                            }
                        }
                        fila += 3;
                        if (reporte.RegitroMensual != null && reporte.RegitroMensual.Count > 0)
                        {
                            ew.Cells[fila, 1].Value = "Datos Mensuales";
                            fila++;

                            ew.Cells[fila, 1].Value = "Inicio";
                            ew.Cells[fila, 2].Value = "fin";
                            ew.Cells[fila, 3].Value = "Domingos en periodo";
                            ew.Cells[fila, 4].Value = "Domingos laborados en periodo";
                            ew.Cells[fila, 5].Value = "Feriados en periodo";
                            ew.Cells[fila, 6].Value = "Feriados laborados en periodo";
                            fila += 2;
                            foreach (var mes in reporte.RegitroMensual)
                            {
                                ew.Cells[fila, 1].Value = mes.inicio.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 2].Value = mes.fin.ToString("dd/MM/yyyy");
                                ew.Cells[fila, 3].Value = mes.totalDomingosenPeriodo;
                                ew.Cells[fila, 4].Value = mes.totalDomingos;
                                ew.Cells[fila, 5].Value = mes.totalFeriadosenPeriodo;
                                ew.Cells[fila, 6].Value = mes.totalFeriados;
                                fila += 2;
                                ew.Cells[fila, 1].Value = "Fecha";
                                ew.Cells[fila, 2].Value = "Asistio";
                                ew.Cells[fila, 3].Value = "Observaciones";
                                fila++;

                                foreach (var dia in mes.Diario)
                                {
                                    ew.Cells[fila, 1].Value = dia.fecha.ToString("dd/MM/yyyy");
                                    ew.Cells[fila, 2].Value = dia.Asistio;
                                    ew.Cells[fila, 3].Value = dia.Observacion;

                                    fila++;
                                }
                                fila += 3;
                            }

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
        public static byte[] Pdf(ReporteDomingosYFeriados reporte)
        {

            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new(ms);
                    using (PdfDocument document = new(writer))
                    {
                        Document doc = new(document);
                        Paragraph titulo = new Paragraph($" REPORTE De Domingos Y Feriados \n Reporte Generado el ${DateTime.Now:dd/MM/yyyy}");
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


                        //datos generales del periodo

                        Cell ano = new();
                        ano.Add(new Paragraph($"Totales del periodo: \n Domingos: {reporte.TotalDomingosPeriodo}, Feriados: {reporte.TotalFeriadosPeriodo} \n Laborados en el periodo: Domingos: {reporte.TotalDomingos}, Feriados: {reporte.TotalFeriadosLaborados}"));
                        doc.Add(ano);

                        //datos anuales

                        foreach (var item in reporte.totalesPorAno)
                        {
                            Cell anual = new();
                            anual.Add(new Paragraph($"Totales por año: periodo de estudio del {item.inicio.ToString("dd/MM/yyyy")} hasta {item.fin.ToString("dd/MM/yyyy")} \n Domingos: { item.totalDomingosenPeriodo}, Feriados: { item.totalFeriadosenPeriodo} \n Laborados en el periodo: Domingos: {item.totalDomingos}, Feriados: {item.totalFeriados}"));
                            doc.Add(anual);
                            
                        }


                        //datos por mes

                        foreach (var item in reporte.RegitroMensual)
                        {
                            Cell anual = new();
                            anual.Add(new Paragraph($"Totales por Mes: periodo de estudio del {item.inicio.ToString("dd/MM/yyyy")} hasta {item.fin.ToString("dd/MM/yyyy")} \n Domingos: { item.totalDomingosenPeriodo}, Feriados: { item.totalFeriadosenPeriodo} \n Laborados en el periodo: Domingos: {item.totalDomingos}, Feriados: {item.totalFeriados}"));
                            doc.Add(anual);


                            Table Tabla;


                            Cell C01 = new();
                            Cell C02 = new();
                            Cell C03 = new();




                            C01.Add(new Paragraph(" Fecha"));
                            C02.Add(new Paragraph(" Asistio"));
                            C03.Add(new Paragraph(" Observacion"));







                            Tabla = new Table(10);

                            Tabla.AddHeaderCell(C01);
                            Tabla.AddHeaderCell(C02);
                            Tabla.AddHeaderCell(C03);

                                Cell C0 = new();
                                Cell C1 = new();
                                Cell C2 = new();

                            foreach (var dia in item.Diario)
                            {
                                C0.Add(new Paragraph(dia.fecha.ToString("dd/MM/yyyy")));
                                C1.Add(new Paragraph(dia.Asistio));
                                C2.Add(new Paragraph(dia.Observacion == "" ? " " : dia.Observacion));
                            }
                                Tabla.AddCell(C0);
                                Tabla.AddCell(C1);
                                Tabla.AddCell(C2);

                            doc.Add(white);
                            doc.Add(Tabla);
                        }

                      
                            
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
        public static byte[] Word(ReporteDomingosYFeriados reporte)
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



                    //periodo
                    IWParagraph periodo = section.AddParagraph();
                    periodo.ApplyStyle("Normal");
                    periodo.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange periodox = Horario.AppendText($"\n\nDatos del periodo. \n Domingos, total:{reporte.TotalDomingosPeriodo}, Laborados: {reporte.TotalDomingos} \n Feriados, Total: {reporte.TotalFeriadosPeriodo}, Laborados: {reporte.TotalFeriadosLaborados}") as WTextRange;


                    // anual

                    string ano = "Datos por año \n\n";

                    foreach (var item in reporte.totalesPorAno)
                    {
                        ano += $"\n año del {item.inicio.ToString("dd/MM/yyyy")} al {item.fin.ToString("dd/MM/yyyy")}  \n Domingos, total:{reporte.TotalDomingosPeriodo}, Laborados: {reporte.TotalDomingos} \n Feriados, Total: {reporte.TotalFeriadosPeriodo}, Laborados: {reporte.TotalFeriadosLaborados} \n\n";

                    }
                    IWParagraph anoxx = section.AddParagraph();
                    anoxx.ApplyStyle("Normal");
                    anoxx.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange anox = Horario.AppendText(ano) as WTextRange;




                    //mensual

                    string mesualidad = "Datos Mensuales \n\n";

                    foreach (var item in reporte.RegitroMensual)
                    {
                        mesualidad += $"\n mes del {item.inicio.ToString("dd/MM/yyyy")} al {item.fin.ToString("dd/MM/yyyy")}  \n Domingos, total:{reporte.TotalDomingosPeriodo}, Laborados: {reporte.TotalDomingos} \n Feriados, Total: {reporte.TotalFeriadosPeriodo}, Laborados: {reporte.TotalFeriadosLaborados} \n\n";

                        foreach (var dia in item.Diario)
                        {
                            mesualidad += $"fecha: {dia.fecha.ToString("dd/MM/yyyy")}, Asistio: {dia.Asistio}, {dia.Observacion} \n";
                        }
                       
                    }

                    IWParagraph mesualidadxx = section.AddParagraph();
                    mesualidadxx.ApplyStyle("Normal");
                    mesualidadxx.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    WTextRange mesualidadx = Horario.AppendText(mesualidad) as WTextRange;




                    

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
