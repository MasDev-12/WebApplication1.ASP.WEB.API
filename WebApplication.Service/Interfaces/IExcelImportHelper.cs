using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.ViewModel.OrderViewModel;

namespace WebApplication.Service.Interfaces
{
    public interface IExcelImportHelper
    {
        string ExportToExcel(IEnumerable<OrderViewModel> ordersViewModel);
    }
}
