using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Enum;
using WebApplication.Domain.ViewModel.RegionViewModel;
using WebApplication.Service.Interfaces;

namespace WebApplication1.ASP.WEB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService=regionService;
        }

        [HttpPost]
        [Route("AddRegion")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddRegion([FromBody] RegionViewModel regionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _regionService.CreateRegion(regionViewModel);
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
        [Route("GetRegions")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAllRegions()
        {
            var regions = _regionService.GetRegionWithChildren();
            if (regions.StatusCode==StatusCodeEnum.NotFound)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(regions.Data);
        }

        [HttpGet]
        [Route("GetById/{RegionId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetById(int RegionId)
        {
            var region = await _regionService.GetRegionById(RegionId);
            if (region.StatusCode==StatusCodeEnum.NotFound)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(region.Data);
        }

        [HttpPost]
        [Route("DeleteRegionById/{RegionId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteRegionById(int RegionId)
        {
            var region = await _regionService.GetRegionById(RegionId);
            if (region==null)
            {
                return NotFound();
            }
            var response = await _regionService.DeleteRegion(RegionId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Description);
        }

        [HttpPut]
        [Route("UpdateRegionById/{RegionId}/{name?}/{parentId?}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateRegionById(int RegionId,string? name,int? parentId)
        {
            if(name==null && parentId==null)
            {
                return BadRequest(ModelState);
            }
            var region = await _regionService.GetRegionById(RegionId);
            if (region==null)
            {
                return NotFound();
            }
            var response = await _regionService.UpdateRegion(RegionId,name,parentId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(response.Description);
        }

    }
}
