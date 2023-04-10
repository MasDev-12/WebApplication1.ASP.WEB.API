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
using WebApplication.Domain.ViewModel.OrderViewModel;
using WebApplication.Service.Interfaces;

namespace WebApplication.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRegionRepository _regionRepository;
        private readonly IBaseRepository<Item> _itemRepository;

        public OrderService(IOrderRepository orderRepository,IRegionRepository regionRepository,IBaseRepository<Item> itemRepository) 
        {
            _orderRepository=orderRepository;
            _regionRepository=regionRepository;
            _itemRepository=itemRepository;
        }
        public async Task<IBaseResponse<bool>> CreateOrder(OrderViewModel orderViewModel)
        {
            try
            {
                var order = new Order()
                {
                    OrderDate= DateTime.Now,
                    RegionId= orderViewModel.RegionId,
                    Region = _regionRepository.GetById(orderViewModel.RegionId).Result,
                    ItemId= orderViewModel.ItemId,
                    Item = _itemRepository.GetById(orderViewModel.ItemId).Result,
                    Amount= orderViewModel.Amount,
                };
                await _orderRepository.Create(order);
           
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="Заказ создан",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[CreateOrder]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }

        }

        public async Task<IBaseResponse<bool>> DeleteOrder(int id)
        {
            try
            {
                var order = await _orderRepository.GetById(id);
                if (order==null)
                {
                    return new BaseResponse<bool>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=false,
                    };
                }
                await _orderRepository.Delete(order);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="Заказ найден и удален",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[DeleteOrder]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<OrderViewModel>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepository.GetById(id);

                if (order==null)
                {
                    return new BaseResponse<OrderViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                var orderViewModel = new OrderViewModel()
                {
                     OrderDate= order.OrderDate,
                     ItemId= order.ItemId,
                     Item= _itemRepository.GetById(order.ItemId).Result,
                     RegionId= order.RegionId,
                     Region=_regionRepository.GetById(order.RegionId).Result,
                     Amount= order.Amount,
                };
                //orderViewModel.Region = _regionRepository.GetById(order.Id).Result;
                //orderViewModel.Item=_itemRepository.GetById(order.Id).Result;
                return new BaseResponse<OrderViewModel>()
                {
                    Data=orderViewModel,
                    Description="Заказ найден",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<OrderViewModel>()
                {
                    Description= $"[GetOrderById]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError,
                    Data=null
                };
            }
        }

        public IBaseResponse<IEnumerable<OrderViewModel>> GetOrders()
        {
            try
            {
                var orders = _orderRepository.GetAll()
                    .Select(order => new OrderViewModel()
                    {
                        OrderDate= order.OrderDate,
                        ItemId= order.ItemId,
                        Item= _itemRepository.GetById(order.ItemId).Result,
                        RegionId= order.RegionId,
                        Region=_regionRepository.GetById(order.RegionId).Result,
                        Amount= order.Amount,
                    });
                return new BaseResponse<IEnumerable<OrderViewModel>>()
                {
                    Data= orders,
                    StatusCode=Domain.Enum.StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<OrderViewModel>>()
                {

                    StatusCode=Domain.Enum.StatusCodeEnum.InternalServerError,
                    Description=$"Ошибка подключения + {ex.Message}"

                };
            }
        }

        public async Task<IBaseResponse<OrderViewModel>> UpdateOrder(int id, int? regionId, int? itemId, int? amount,DateTime? dateTime)
        {
            try
            {
                var order = await _orderRepository.GetById(id);
                if (order==null)
                {
                    return new BaseResponse<OrderViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                if (regionId!=null)
                {
                    order.RegionId= (int)regionId;
                }
                if (itemId!=null)
                {
                    order.ItemId = (int)itemId;
                }
                if(amount!=null)
                {
                    order.Amount=(int)amount;
                }
                if(dateTime!=null)
                {
                    order.OrderDate=(DateTime)dateTime;
                }
                await _orderRepository.Update(order);
                var orderViewModel = new OrderViewModel()
                {
                    OrderDate= order.OrderDate,
                    ItemId= order.ItemId,
                    Region = _regionRepository.GetById(order.RegionId).Result,
                    RegionId= order.RegionId,
                    Item = _itemRepository.GetById(order.ItemId).Result,
                    Amount= order.Amount,
                };
                //orderViewModel.Region = _regionRepository.GetById(order.RegionId).Result;
                //orderViewModel.Item = _itemRepository.GetById(order.ItemId).Result;
                return new BaseResponse<OrderViewModel>()
                {
                    Data=orderViewModel,
                    Description="товар найден и обновлен",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<OrderViewModel>()
                {
                    Description= $"[UpdateItem]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<OrderViewModel>>> GetOrdersByPagitation(int pageSize, int pageNumber, string searchTerm)
        {
            try
            {
                var orders = _orderRepository.GetOrdersByPagitation(pageSize, pageNumber, searchTerm)
                    .Result
                    .Select(order => new OrderViewModel()
                    {
                        OrderDate= order.OrderDate,
                        ItemId= order.ItemId,
                        Item= _itemRepository.GetById(order.ItemId).Result,
                        RegionId= order.RegionId,
                        Region=_regionRepository.GetById(order.RegionId).Result,
                        Amount= order.Amount,
                    });
                return new BaseResponse<IEnumerable<OrderViewModel>>()
                {
                    Data = orders,
                    StatusCode=Domain.Enum.StatusCodeEnum.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<OrderViewModel>>()
                {

                    StatusCode=Domain.Enum.StatusCodeEnum.InternalServerError,
                    Description=$"Ошибка подключения + {ex.Message}"

                };
            }
        }
    }
}
