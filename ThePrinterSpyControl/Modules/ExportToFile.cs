using System;
using System.Collections.ObjectModel;

using Microsoft.Office.Interop.Excel;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.Modules
{
    class ExportToFile
    {
        public void Excel(string filename, ObservableCollection<PrintDataGrid> reportGrid)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Application.Workbooks.Add(Type.Missing);
            excelApp.Cells[1, 1] = "Документ";
            excelApp.Cells[1, 2] = "Принтер";
            excelApp.Cells[1, 3] = "Страницы, шт.";
            excelApp.Cells[1, 4] = "Пользователь";
            excelApp.Cells[1, 5] = "Дата и время";
            (excelApp.Cells[1, 1] as Range).EntireRow.Font.Bold = true;
            for (int i = 0; i < reportGrid.Count; i++)
            {
                var row = reportGrid[i];
                excelApp.Cells[i + 2, 1] = row.DocName;
                excelApp.Cells[i + 2, 2] = row.PrinterName;
                excelApp.Cells[i + 2, 3] = row.Pages.ToString();
                excelApp.Cells[i + 2, 4] = row.UserName;
                excelApp.Cells[i + 2, 5] = row.TimeStamp.ToString("yyyy.MM.dd HH:mm:ss");
            }
            excelApp.Columns.EntireColumn.AutoFit();

            excelApp.ActiveWorkbook.SaveAs(filename);
            excelApp.Quit();
        }
    }
}
