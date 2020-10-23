using log4net;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Resources.Localization;
using AndreasReitberger.Models;
using AndreasReitberger.Enums;

namespace PrintCostCalculator3d.Models.Syncfusion
{
    class ExcelHandler
    {
        #region Variables
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties

        #endregion

        public static bool ExportCaclulations(ObservableCollection<Calculation3d> calcs, string path, bool asPdf = false)
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2013;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                    worksheet.UsedRange.AutofitColumns();
                    // Create Header

                    worksheet.Range["A1"].Text = Strings.Printer;
                    //worksheet.Range["A1"].ColumnWidth = 80;
                   
                    worksheet.Range["B1"].Text = Strings.Material;
                    //worksheet.Range["B1"].ColumnWidth = 80;
                    
                    worksheet.Range["C1"].Text = Strings.Name;
                    //worksheet.Range["C1"].ColumnWidth = 80;

                    worksheet.Range["D1"].Text = "Volume";

                    worksheet.Range["E1"].Text = "Quantity";

                    worksheet.Range["F1"].Text = "Print Time";

                    worksheet.Range["G1"].Text = "Print Costs";

                    worksheet.Range["H1"].Text = "Material Costs";

                    worksheet.Range["I1"].Text = "Enregy Costs";

                    worksheet.Range["J1"].Text = "Worksteps";

                    worksheet.Range["K1"].Text = "Handling";

                    worksheet.Range["L1"].Text = "Margin";

                    worksheet.Range["M1"].Text = "Tax";

                    worksheet.Range["N1"].Text = "Total";

                    worksheet.Range["A1:N1"].CellStyle.Color = Color.LightGray;
                    worksheet.Range["A1:N1"].CellStyle.Font.Color = ExcelKnownColors.White;
                    worksheet.Range["A1:N1"].RowHeight = 25;
                    worksheet.Range["A1:N1"].CellStyle.Font.Bold = true;

                    string currencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
                    //Go to list
                    int currentRow = 2;
                    for (int i = 0; i < calcs.Count; i++)
                    {       
                        foreach (Printer3d printer in calcs[i].Printers)
                        { 
                            foreach (Material3d material in calcs[i].Materials)
                            {
                                //Printer Name
                                worksheet.Range[string.Format("A{0}", currentRow)].Text = printer.Name;
                                //Material name
                                worksheet.Range[string.Format("B{0}", currentRow)].Text = material.Name;
                                //Name
                                worksheet.Range[string.Format("C{0}", currentRow)].Text = calcs[i].Name;
                                //Volume
                                worksheet.Range[string.Format("D{0}", currentRow)].Number = Convert.ToDouble(calcs[i].TotalVolume);
                                worksheet.Range[string.Format("D{0}", currentRow)].NumberFormat = "0.00";
                                //Quantity
                                worksheet.Range[string.Format("E{0}", currentRow)].Number = Convert.ToDouble(calcs[i].Quantity);
                                worksheet.Range[string.Format("E{0}", currentRow)].NumberFormat = "0";
                                //Print time
                                worksheet.Range[string.Format("F{0}", currentRow)].Number = Convert.ToDouble(calcs[i].TotalPrintTime);
                                worksheet.Range[string.Format("F{0}", currentRow)].NumberFormat = "0.000";
                                //Print Costs
                                calcs[i].Material = material;
                                calcs[i].Printer = printer;
                                var printCosts = calcs[i].MachineCosts;
                                worksheet.Range[string.Format("G{0}", currentRow)].Number = Convert.ToDouble(printCosts);
                                worksheet.Range[string.Format("G{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                //Material costs
                                var materialCosts = calcs[i].MaterialCosts;
                                worksheet.Range[string.Format("H{0}", currentRow)].Number = Convert.ToDouble(materialCosts);
                                worksheet.Range[string.Format("H{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                //Energy costs
                                var energyCosts = calcs[i].EnergyCosts;
                                worksheet.Range[string.Format("I{0}", currentRow)].Number = Convert.ToDouble(energyCosts);
                                worksheet.Range[string.Format("I{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                // Worksteps
                                var workstepCosts = calcs[i].WorkstepCosts;
                                worksheet.Range[string.Format("J{0}", currentRow)].Number = Convert.ToDouble(workstepCosts);
                                worksheet.Range[string.Format("J{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                // Handling fee
                                worksheet.Range[string.Format("K{0}", currentRow)].Number = Convert.ToDouble(calcs[i].getTotalCosts(CalculationAttributeType.FixCost));
                                worksheet.Range[string.Format("K{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                // Margin
                                worksheet.Range[string.Format("L{0}", currentRow)].Number = Convert.ToDouble(calcs[i].CalculatedMargin);
                                worksheet.Range[string.Format("L{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                //Tax
                                worksheet.Range[string.Format("M{0}", currentRow)].Number = Convert.ToDouble(calcs[i].CalculatedTax);
                                worksheet.Range[string.Format("M{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";
                                //Total
                                worksheet.Range[string.Format("N{0}", currentRow)].Number = Convert.ToDouble(calcs[i].TotalCosts);
                                worksheet.Range[string.Format("N{0}", currentRow)].NumberFormat = currencySymbol + "#,##0.00";

                                currentRow++;
                            }
                        }
                    }
                    if (asPdf)
                    {
                        //Open the Excel document to Convert
                        ExcelToPdfConverter converter = new ExcelToPdfConverter(workbook);
                        ExcelToPdfConverterSettings settings = new ExcelToPdfConverterSettings();
                        settings.LayoutOptions = LayoutOptions.Automatic;

                        //Initialize PDF document
                        PdfDocument pdfDocument = new PdfDocument();

                        //Convert Excel document into PDF document
                        pdfDocument = converter.Convert(settings);

                        //Save the PDF file
                        pdfDocument.Save(path);
                    }
                    else
                        workbook.SaveAs(path);

                    workbook.Close();
                    return true;
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }
        public static bool WriteCalculationsToTemplate(ObservableCollection<Calculation3d> calcs, 
            string template, string path, ExcelWriteTemplateSettings Settings, bool asPdf = false
            )
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2013;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Open(template, ExcelOpenType.Automatic);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    //Accessing first table in the sheet
                    IListObject table = worksheet.ListObjects[0];
                    string currencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

                    int insertRow = Convert.ToInt32(Settings.StartRow);
                    IStyle rowStyle = worksheet.Rows.ElementAt(insertRow).CellStyle;
                    worksheet.InsertRow(insertRow, calcs.Count-1, ExcelInsertOptions.FormatAsAfter);
                    //Go to list
                    for (int i = 0; i < calcs.Count; i++)
                    {
                        int Row = Convert.ToInt32(Settings.StartRow) + i;
                        //worksheet.Rows.ElementAt(Row).CellStyle = rowStyle;

                        string Col = Settings.StartColumn;
                        // Pos
                        worksheet.Range[string.Format("{0}{1}", Col ,Row)].Number = i +1 ;
                        // Description

                        StringBuilder sb = new StringBuilder();
                        sb.Append(calcs[i].Name);
                        /*
                        sb.AppendLine(string.Format("Volume: {0} | Time: {1}", 
                            Convert.ToDouble(calcs[i].Volume), 
                            Convert.ToDouble(calcs[i].CalculatedPrintTime))
                            );
                            */
                        Col = GetNextColumn(Col, 1);
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].Text = sb.ToString();

                        // Quantity
                        Col = GetNextColumn(Col, 1);
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = Convert.ToDouble(calcs[i].Quantity);
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].NumberFormat = "0";
                        
                        // Single
                        Col = GetNextColumn(Col, 1);
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = (Convert.ToDouble(calcs[i].TotalCosts) / (Convert.ToDouble(calcs[i].Quantity)));
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].NumberFormat = currencySymbol + "#,##0.00";

                        // Total
                        Col = GetNextColumn(Col, 1);
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = Convert.ToDouble(calcs[i].TotalCosts);
                        worksheet.Range[string.Format("{0}{1}", Col, Row)].NumberFormat = currencySymbol + "#,##0.00";

                        if (i < calcs.Count - 1)
                        {
                            var loc = table.Location;
                            //worksheet.InsertRow(Row +1, 1, ExcelInsertOptions.FormatAsBefore);
                        }
                    }

                    // remove the empty rows
                    worksheet.DeleteRow(insertRow + calcs.Count - 1, calcs.Count - 1);
                    if (asPdf)
                    {
                        //Open the Excel document to Convert
                        ExcelToPdfConverter converter = new ExcelToPdfConverter(workbook);

                        //Initialize PDF document
                        PdfDocument pdfDocument = new PdfDocument();

                        //Convert Excel document into PDF document
                        pdfDocument = converter.Convert();

                        //Save the PDF file
                        pdfDocument.Save(path);
                    }
                    else 
                        workbook.SaveAs(path);
                    return true;
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }
        
        public static bool WriteCalculationsToExporterTemplate(ObservableCollection<Calculation3d> calcs, 
            string path, ExporterTemplate template, bool asPdf = false
            )
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    bool targetIsFolder = false;
                    application.DefaultVersion = ExcelVersion.Excel2013;

                    string ext = Path.GetExtension(path);
                    targetIsFolder = string.IsNullOrEmpty(ext);

                    string root = Path.GetDirectoryName(path);

                    string filename = string.Format(@"{0}_{1}{2}",
                        Regex.Replace(Strings.MyCalculation, "[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled),
                        Regex.Replace("1", "[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled),
                        string.IsNullOrEmpty(ext) ? asPdf ? ".pdf" : ".xlsx" : ext
                        );
                    filename = getDuplicatedFileName(targetIsFolder ? path : root, filename);
                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Open(template.TemplatePath, ExcelOpenType.Automatic);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    string currencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

                    foreach (ExporterSettings setting in template.Settings)
                    {
                        worksheet = workbook.Worksheets[setting.WorkSheetName];
                        switch (setting.Attribute.Property)
                        {
                            #region List
                            case ExporterProperty.CalculationList:

                                int insertRow = Convert.ToInt32(setting.Coordinates.Row);
                                IStyle rowStyle = worksheet.Rows.ElementAt(insertRow).CellStyle;
                                if(calcs.Count != 1)
                                    worksheet.InsertRow(insertRow, calcs.Count - 1, ExcelInsertOptions.FormatAsAfter);
                                //Go to list
                                for (int i = 0; i < calcs.Count; i++)
                                {
                                    foreach (Printer3d printer in calcs[i].Printers)
                                    {
                                        foreach (Material3d material in calcs[i].Materials)
                                        {
                                            
                                            int Row = Convert.ToInt32(insertRow) + i;
                                            string Col = setting.Coordinates.Column;
                                            // Pos
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = i + 1;

                                            calcs[i].Material = material;
                                            calcs[i].Printer = printer;

                                            // Description
                                            StringBuilder sb = new StringBuilder();
                                            sb.Append(calcs[i].Name);
                                            /*
                                            sb.AppendLine(string.Format("Volume: {0} | Time: {1}", 
                                                Convert.ToDouble(calcs[i].Volume), 
                                                Convert.ToDouble(calcs[i].CalculatedPrintTime))
                                                );
                                                */
                                            Col = GetNextColumn(Col, 1);
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].Text = sb.ToString();

                                            // Quantity
                                            Col = GetNextColumn(Col, 1);
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = Convert.ToDouble(calcs[i].Quantity);
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].NumberFormat = "0";

                                            // Single
                                            Col = GetNextColumn(Col, 1);
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = (Convert.ToDouble(calcs[i].TotalCosts) / (Convert.ToDouble(calcs[i].Quantity)));
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].NumberFormat = currencySymbol + "#,##0.00";

                                            // Total
                                            Col = GetNextColumn(Col, 1);
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].Number = Convert.ToDouble(calcs[i].TotalCosts);
                                            worksheet.Range[string.Format("{0}{1}", Col, Row)].NumberFormat = currencySymbol + "#,##0.00";

                                            if (i < calcs.Count - 1)
                                            {
                                            }
                                        }
                                    }
                                }

                                // remove the empty rows
                                if(calcs.Count != 1)
                                    worksheet.DeleteRow(insertRow + calcs.Count, calcs.Count - 1);

                                break;

                            #endregion
                        }
                    }

                    if (asPdf)
                    {
                        //Open the Excel document to Convert
                        ExcelToPdfConverter converter = new ExcelToPdfConverter(workbook);

                        //Initialize PDF document
                        PdfDocument pdfDocument = new PdfDocument();

                        //Convert Excel document into PDF document
                        pdfDocument = converter.Convert();

                        //Save the PDF file
                        string savePath = targetIsFolder ? Path.Combine(path, filename) : Path.Combine(root, filename);
                        pdfDocument.Save(savePath);
                    }
                    else
                    {
                        string savePath = targetIsFolder ? Path.Combine(path, filename) : Path.Combine(root, filename);
                        workbook.SaveAs(savePath);
                    }

                    workbook.Close();
                    return true;
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }

        
        public static bool WriteCalculationToExporterTemplate(Calculation3d calculation, 
            string path, ExporterTemplate template, bool asPdf = false
            )
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    bool targetIsFolder = false;
                    application.DefaultVersion = ExcelVersion.Excel2013;
                    string ext = Path.GetExtension(path);
                    targetIsFolder = string.IsNullOrEmpty(ext);
                    
                    string root = Path.GetDirectoryName(path);

                    // Foreach printer...
                    foreach (Printer3d printer in calculation.Printers)
                    {
                        //... and material
                        foreach (Material3d material in calculation.Materials)
                        {
                            string filename = string.Format(@"{0}_{1}{2}",
                                Regex.Replace(printer.ToString(), "[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled), 
                                Regex.Replace(material.ToString(), "[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled), 
                                string.IsNullOrEmpty(ext) ? asPdf ? ".pdf" : ".xlsx" : ext
                                );
                            //Create a workbook
                            IWorkbook workbook = application.Workbooks.Open(template.TemplatePath, ExcelOpenType.Automatic);
                            IWorksheet worksheet = workbook.Worksheets[0];

                            string currencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

                            foreach (ExporterSettings setting in template.Settings)
                            {
                                worksheet = workbook.Worksheets[setting.WorkSheetName];
                                switch (setting.Attribute.Property)
                                {
                                    #region Prices
                                    case ExporterProperty.CalculationMargin:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.CalculatedMargin);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPriceMaterial:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.MaterialCosts);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPriceEnergy:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.EnergyCosts);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPriceHandling:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.getTotalCosts(CalculationAttributeType.FixCost));
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPricePrinter:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.MachineCosts);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPriceTax:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.CalculatedTax);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPriceWorksteps:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.WorkstepCosts);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;

                                    case ExporterProperty.CalculationPriceTotal:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.TotalCosts);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = currencySymbol + "#,##0.00";
                                        break;
                                    #endregion

                                    #region Material
                                    case ExporterProperty.CalculationMaterial:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Text
                                            = material.ToString();
                                        break;
                                    #endregion

                                    #region Printer
                                    case ExporterProperty.CalculationPrinter:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Text
                                            = printer.ToString();
                                        break;
                                    #endregion

                                    #region Misc

                                    case ExporterProperty.CalculationFailrate:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.FailRate);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = "#0 %";
                                        break;

                                    case ExporterProperty.CalculationPrintTime:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.TotalPrintTime);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = "#0 %";
                                        break;

                                    case ExporterProperty.CalculationQuantity:
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].Number
                                            = Convert.ToDouble(calculation.Quantity);
                                        worksheet.Range[string.Format("{0}{1}", setting.Coordinates.Column, setting.Coordinates.Row)].NumberFormat = "0";
                                        break;
                                    #endregion

                                }
                            }

                            if (asPdf)
                            {
                                //Open the Excel document to Convert
                                ExcelToPdfConverter converter = new ExcelToPdfConverter(workbook);

                                //Initialize PDF document
                                PdfDocument pdfDocument = new PdfDocument();

                                //Convert Excel document into PDF document
                                pdfDocument = converter.Convert();

                                //Save the PDF file
                                string savePath = targetIsFolder ? Path.Combine(path, filename) : Path.Combine(root, filename);
                                pdfDocument.Save(savePath);
                            }
                            else
                            {
                                string savePath = targetIsFolder ? Path.Combine(path, filename) : Path.Combine(root, filename);
                                workbook.SaveAs(savePath);
                            }

                            workbook.Close();
                        }
                    }
                    return true;
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }

        public static string getDuplicatedFileName(string path, string targetFileName)
        {
            string name = string.Empty;
            var parts = targetFileName.Split('_');
            StringBuilder fileName = new StringBuilder();
            for(int i = 0; i < parts.Count() - 1; i++)
            {
                fileName.Append(parts[i]);
            }

            var duplicates = Directory.GetFiles(path).Where(file => file.StartsWith(fileName.ToString()));


            name = string.Format("{0}_{1}{2}", fileName.ToString(), duplicates.Count(), parts[parts.Count() -1]);

            return name;
        }
        public static List<string> GetWorksheetsFromFile(string path)
        {
            var sheets = new List<string>();
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2013;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Open(path, ExcelOpenType.Automatic);
                    sheets = workbook.Worksheets.Select(sheet => sheet.Name).ToList();
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            return sheets;
        }

        private static string GetNextColumn(string column, int steps = 1)
        {
            StringBuilder sb = new StringBuilder();
            column = column.ToUpper();

            int chars = column.Length;
            for(int i = 0; i < chars; i++)
            {
                if (column[i] == 'Z')
                {
                    sb.Append(string.Format("A{0}", ((char)((int)'A' + (steps -1))).ToString()));
                }
                else
                    sb.Append(((char)((int)column[i] + steps)));
            }

            return sb.ToString();
        }
    }

    public struct ExcelWriteTemplateSettings
    {
        #region Attributes
        public string StartRow;
        public string StartColumn;
        public int MaxRowsPerPage;
        #endregion
    }
}
