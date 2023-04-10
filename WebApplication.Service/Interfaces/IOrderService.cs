using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.ItemViewModel;
using WebApplication.Domain.ViewModel.OrderViewModel;

namespace WebApplication.Service.Interfaces
{
    public interface IOrderService
    {
        IBaseResponse<IEnumerable<OrderViewModel>> GetOrders();
        Task<IBaseResponse<OrderViewModel>> GetOrderById(int id);
        Task<IBaseResponse<bool>> DeleteOrder(int id);

        Task<IBaseResponse<bool>> CreateOrder(OrderViewModel orderViewModel);

        Task<IBaseResponse<OrderViewModel>> UpdateOrder(int id, int? regionId, int? itemId, int? amount,DateTime? dateTime);

        Task<BaseResponse<IEnumerable<OrderViewModel>>> GetOrdersByPagitation(int pageSize, int pageNumber, string searchTerm);
    }
}
