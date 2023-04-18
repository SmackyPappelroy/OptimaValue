using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OptimaValue
{
    public class ExcelConverter
    {
        public static void CsvToXlsx(string csvFileName, string xlsxFileName)
        {
            // Create an instance of Excel, and make it visible
            var excelApp = new Application
            {
                Visible = false
            };

            // Open the CSV file in Excel
            var csvWorkbook = excelApp.Workbooks.Open(csvFileName);

            // Save the CSV file as an XLSX file
            csvWorkbook.SaveAs(xlsxFileName, XlFileFormat.xlOpenXMLWorkbook);

            // Close the workbook and the Excel application
            csvWorkbook.Close();
            excelApp.Quit();
        }
    }
}
