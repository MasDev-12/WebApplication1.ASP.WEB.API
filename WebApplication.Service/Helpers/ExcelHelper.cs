using ClosedXML.Excel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.ViewModel.OrderViewModel;
using WebApplication.Service.Interfaces;


namespace WebApplication.Service.Helpers
{
    public class ExcelHelper : IExcelImportHelper
    {
        public string ExportToExcel(IEnumerable<OrderViewModel> ordersViewModel)
        {
            string fileName = "Orders_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "excels", fileName);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");

                // Заполняем заголовки столбцов
                worksheet.Cell(1, 1).Value = "Order ID";
                worksheet.Cell(1, 2).Value = "Order Date";
                worksheet.Cell(1, 3).Value = "Region";
                worksheet.Cell(1, 4).Value = "Item";
                worksheet.Cell(1, 5).Value = "Amount";

                // Заполняем данные заказов
                int rowNumber = 1;
                int row = 2;
                foreach (var order in ordersViewModel)
                {
                    worksheet.Cell(row, 1).Value = rowNumber++;
                    worksheet.Cell(row, 2).Value = order.OrderDate.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 3).Value = order.Region.Name;
                    worksheet.Cell(row, 4).Value = order.Item.Name;
                    worksheet.Cell(row, 5).Value = order.Amount;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }

            return filePath;
        }
    }
}



