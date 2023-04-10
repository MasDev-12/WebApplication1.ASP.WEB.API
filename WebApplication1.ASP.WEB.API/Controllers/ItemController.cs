using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.Domain.Enum;
using WebApplication.Domain.ViewModel.ItemViewModel;
using WebApplication.Domain.ViewModel.RegionViewModel;
using WebApplication.Service.Implementations;
using WebApplication.Service.Interfaces;

namespace WebApplication1.ASP.WEB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController:Controller
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService=itemService;
        }

        [HttpPost]
        [Route("AddItem")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddItem([FromBody] ItemViewModel itemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _itemService.CreateItem(itemViewModel);
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
        [Route("GetItems")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetItems()
        {
            var item = _itemService.GetItems();
            if (item.StatusCode==StatusCodeEnum.NotFound)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(item.Data);
        }

        [HttpGet]
        [Route("GetById/{ItemId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetById(int itemId)
        {
            var item = await _itemService.GetItemById(itemId);
            if (item.StatusCode==StatusCodeEnum.NotFound)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(item.Data);
        }

        [HttpPost]
        [Route("DeleteItemById/{ItemId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteItemById(int itemId)
        {
            var item = await _itemService.GetItemById(itemId);
            if (item==null)
            {
                return NotFound();
            }
            var response = await _itemService.DeleteItem(itemId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Description);
        }

        [HttpPut]
        [Route("UpdateItemById/{itemId}/{name?}/{price?}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateItemById(int itemId, string? name, decimal? price)
        {
            if (name==null && price==null)
            {
                return BadRequest(ModelState);
            }
            var item = await _itemService.GetItemById(itemId);
            if (item==null)
            {
                return NotFound();
            }
            var response = await _itemService.UpdateItem(itemId, name, price);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Description);
        }
    }
}
