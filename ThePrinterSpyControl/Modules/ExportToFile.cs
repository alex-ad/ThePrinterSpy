using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Properties;

namespace ThePrinterSpyControl.Modules
{
    class ExportToFile
    {
        public void Excel(string filename, ObservableCollection<PrintDataGrid> reportGrid)
        {
            try
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Application.Workbooks.Add(Type.Missing);
                excelApp.Cells[1, 1] = Resources.ReportUser;
                excelApp.Cells[1, 2] = Resources.ReportPrinter;
                excelApp.Cells[1, 3] = Resources.ReportComputer;
                excelApp.Cells[1, 4] = Resources.ReportDocument;
                excelApp.Cells[1, 5] = Resources.ReportPages;
                excelApp.Cells[1, 6] = Resources.ReportDateTime;
                (excelApp.Cells[1, 1] as Range).EntireRow.Font.Bold = true;
                for (int i = 0; i < reportGrid.Count; i++)
                {
                    var row = reportGrid[i];
                    excelApp.Cells[i + 2, 1] = row.UserName;
                    excelApp.Cells[i + 2, 2] = row.PrinterName;
                    excelApp.Cells[i + 2, 3] = row.ComputerName;
                    excelApp.Cells[i + 2, 4] = row.DocName;
                    excelApp.Cells[i + 2, 5] = row.Pages.ToString();
                    excelApp.Cells[i + 2, 6] = row.TimeStamp.ToString("yyyy.MM.dd HH:mm:ss");
                }

                excelApp.Columns.EntireColumn.AutoFit();

                excelApp.ActiveWorkbook.SaveAs(filename);
                excelApp.Quit();
            }
            catch
            {
                MessageBox.Show("Excel error: make sure the Excel is installed on your computer", "Excel error");
            }
        }
    }
}
