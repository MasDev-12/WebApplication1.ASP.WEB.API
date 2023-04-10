using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.ItemViewModel;
using WebApplication.Domain.ViewModel.User;

namespace WebApplication.Service.Interfaces
{
    public interface IItemService
    {
        IBaseResponse<IEnumerable<ItemViewModel>> GetItems();
        Task<IBaseResponse<ItemViewModel>> GetItemById(int id);
        Task<IBaseResponse<bool>> DeleteItem(int id);

        Task<IBaseResponse<bool>> CreateItem(ItemViewModel itemViewModel);

        Task<IBaseResponse<ItemViewModel>> UpdateItem(int id, string? name, decimal? price);
    }
}
