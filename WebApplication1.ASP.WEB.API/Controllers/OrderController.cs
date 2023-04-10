using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Enum;
using WebApplication.Domain.ViewModel.OrderViewModel;
using WebApplication.Domain.ViewModel.RegionViewModel;
using WebApplication.Service.Implementations;
using WebApplication.Service.Interfaces;

namespace WebApplication1.ASP.WEB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController:Controller
    {
        private readonly IOrderService _orderService;
        private readonly IExcelImportHelper _excelImportHelper;

        public OrderController(IOrderService orderService, IExcelImportHelper excelImportHelper) 
        {
            _orderService=orderService;
            _excelImportHelper=excelImportHelper;
        }

        [HttpPost]
        [Route("AddOrder")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddOrder([FromBody] OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _orderService.CreateOrder(orderViewModel);
            if (response.StatusCode==StatusCodeEnum.Found)
            {
                ModelState.AddModelError("", response.Description);
                return StatusCode(422, ModelState);
            }
            if (response.StatusCode==StatusCodeEnum.InternalServerError)
            {
                ModelState.AddModelError("", response.Description);
                return StatusCode(500, ModelState);
            }
            return Ok(response.Description);
        }

        [HttpGet]
        [Route("GetOrders")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetOrders()
        {
            var response = _orderService.GetOrders();
            if (response.StatusCode==StatusCodeEnum.NotFound)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Data);
        }

        [HttpGet]
        [Route("GetById/{OrderId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var response = await _orderService.GetOrderById(orderId);
            if (response.StatusCode==StatusCodeEnum.NotFound)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Data);
        }

        [HttpPost]
        [Route("DeleteOrderById/{OrderId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteOrderById(int orderId)
        {
            var item = await _orderService.GetOrderById(orderId);
            if (item==null)
            {
                return NotFound();
            }
            var response = await _orderService.DeleteOrder(orderId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Description);
        }

        [HttpPut]
        [Route("UpdateOrderById/{OrderId}/{regionId?}/{itemId?}/{amount?}/{dateTime?}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateOrderById(int orderId, int? regionId, int? itemId, int? amount, DateTime? dateTime)
        {
            if (regionId==null && itemId==null && amount==null && dateTime==null)
            {
                return BadRequest(ModelState);
            }
            var item = await _orderService.GetOrderById(orderId);
            if (item==null)
            {
                return NotFound();
            }
            var response = await _orderService.UpdateOrder(orderId, regionId, itemId, amount, dateTime);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Description);
        }

        [HttpGet]
        [Route("GetOrdersByPagitation/{pageSize}/{pageNumber}/{searchTerm}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetOrdersByPagitation(int pageSize, int pageNumber, string searchTerm)
        {
            var orders = await _orderService.GetOrdersByPagitation(pageSize, pageNumber, searchTerm);
            if(orders==null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (orders.StatusCode==StatusCodeEnum.InternalServerError)
            {
                return BadRequest(ModelState);
            }
            return Ok(orders.Data);
        }

        [HttpGet]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            var response = _orderService.GetOrders();
            if(response.Data==null)
            {
                return NotFound();
            }
            var filePath = _excelImportHelper.ExportToExcel(response.Data);

            return Ok(filePath);
        }
    }
}
