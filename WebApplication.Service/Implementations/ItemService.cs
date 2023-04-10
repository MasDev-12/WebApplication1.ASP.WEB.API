using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.DAL.Repositories;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Enum;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.ItemViewModel;
using WebApplication.Domain.ViewModel.RegionViewModel;
using WebApplication.Domain.ViewModel.User;
using WebApplication.Service.Interfaces;

namespace WebApplication.Service.Implementations
{
    public class ItemService : IItemService
    {
        private readonly IBaseRepository<Item> _itemRepository;

        public ItemService(IBaseRepository<Item> itemRepository)
        {
            this._itemRepository=itemRepository;
        }
        public async Task<IBaseResponse<bool>> CreateItem(ItemViewModel itemViewModel)
        {
            try
            {
                var item = new Item()
                {
                    Name= itemViewModel.Name,
                    Price= itemViewModel.Price,
                };
                await _itemRepository.Create(item);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="Товар создан",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[CreateItem]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteItem(int id)
        {
            try
            {
                var item = await _itemRepository.GetById(id);
                if (item==null)
                {
                    return new BaseResponse<bool>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=false,
                    };
                }
                await _itemRepository.Delete(item);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="Товар найден и удален",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[DeleteItem]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<ItemViewModel>> GetItemById(int id)
        {
            try
            {
                var item = await _itemRepository.GetById(id);

                if (item==null)
                {
                    return new BaseResponse<ItemViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                var itemViewModel = new ItemViewModel()
                {
                    Name=item.Name,
                    Price=item.Price,
                };
                return new BaseResponse<ItemViewModel>()
                {
                    Data=itemViewModel,
                    Description="Товар найден",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ItemViewModel>()
                {
                    Description= $"[GetItemById]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError,
                    Data=null
                };
            }
        }

        public IBaseResponse<IEnumerable<ItemViewModel>> GetItems()
        {
            try
            {
                var item = _itemRepository.GetAll()
                    .Select(x => new ItemViewModel()
                    {
                        Price=x.Price,
                        Name=x.Name,
                    });

                return new BaseResponse<IEnumerable<ItemViewModel>>()
                {
                    Data= item,
                    StatusCode=Domain.Enum.StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<ItemViewModel>>()
                {

                    StatusCode=Domain.Enum.StatusCodeEnum.InternalServerError,
                    Description=$"Ошибка подключения + {ex.Message}"

                };
            }
        }

        public async Task<IBaseResponse<ItemViewModel>> UpdateItem(int id, string? name, decimal? price)
        {
            try
            {
                var item = await _itemRepository.GetById(id);
                if (item==null)
                {
                    return new BaseResponse<ItemViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                if (name!=null)
                {
                    item.Name= name;
                }
                if (price!=null)
                {
                    item.Price = (decimal)price;
                }
                await _itemRepository.Update(item);
                var itemViewModel = new ItemViewModel()
                {
                    Name=item.Name,
                    Price=item.Price,
                };
                return new BaseResponse<ItemViewModel>()
                {
                    Data=itemViewModel,
                    Description="товар найден и обновлен",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ItemViewModel>()
                {
                    Description= $"[UpdateItem]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }
    }
}

