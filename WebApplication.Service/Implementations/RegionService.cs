using Microsoft.AspNetCore.Http;
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
using WebApplication.Domain.ViewModel.RegionViewModel;
using WebApplication.Domain.ViewModel.User;
using WebApplication.Service.Interfaces;

namespace WebApplication.Service.Implementations
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;

        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository=regionRepository;
        }
        public async Task<IBaseResponse<bool>> CreateRegion(RegionViewModel regionViewModel)
        {
            try
            {
                if (await _regionRepository.CheckRegionExistsAsync(regionViewModel.Name,regionViewModel.ParentId))
                {
                    return new BaseResponse<bool>()
                    {
                        Description="регион уже ранее был создан",
                        StatusCode=StatusCodeEnum.Found,
                        Data=false,
                    };
                }
                var region = new Region()
                {
                    Name= regionViewModel.Name,
                    ParentId=regionViewModel.ParentId,
                    Children = regionViewModel.Children.Select(c => new Region()
                    {
                        Name = c.Name,
                        ParentId = c.ParentId
                    }).ToList()
                };
                await _regionRepository.Create(region);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="регион создан",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[CreateRegion]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError,
                    Data = false
                };
            }
        }



        public async Task<IBaseResponse<bool>> DeleteRegion(int id)
        {
            try
            {
                var region = await _regionRepository.GetById(id);
                if (region==null)
                {
                    return new BaseResponse<bool>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=false,
                    };
                }
                await _regionRepository.Delete(region);
                return new BaseResponse<bool>()
                {
                    Data=true,
                    Description="регион найден и удален",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description= $"[DeleteRegion]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<RegionViewModel>> GetRegionById(int id)
        {
            try
            {
                var region = await _regionRepository.GetById(id);

                if (region==null)
                {
                    return new BaseResponse<RegionViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                var regionViewModel = MapToViewModel(region);
              
                return new BaseResponse<RegionViewModel>()
                {
                    Data=regionViewModel,
                    Description="Регион найден",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RegionViewModel>()
                {
                    Description= $"[GetRegionById]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError,
                    Data=null
                };
            }
        }

        public IBaseResponse<IEnumerable<RegionViewModel>> GetRegionWithChildren()
        {
            try
            {
                var regions = _regionRepository.GetAll();
                if(regions==null)
                {
                    return new BaseResponse<IEnumerable<RegionViewModel>>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                var regionViewModel = new List<RegionViewModel>();
                foreach (var region in regions)
                {
                    var viewModel = MapToViewModel(region);
                    regionViewModel.Add(viewModel);
                }
                return new BaseResponse<IEnumerable<RegionViewModel>>()
                {
                    Data= regionViewModel,
                    StatusCode=Domain.Enum.StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<RegionViewModel>>()
                {
                    Description= $"[GetRegionWithChildren]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError,
                    Data=null
                };
            }
            
        }
        private RegionViewModel MapToViewModel(Region region)
        {
            var viewModel = new RegionViewModel()
            {
                Name = region.Name,
                ParentId = region.ParentId,
                Children = new List<RegionViewModel>()
            };
            foreach (var child in region.Children)
            {
                var childViewModel = MapToViewModel(child);
                viewModel.Children.Add(childViewModel);
            }
            return viewModel;
        }

        public async Task<IBaseResponse<RegionViewModel>> UpdateRegion(int id,string? name,int? parentId)
        {
           
            try
            {
                var region = await _regionRepository.GetById(id);
                if (region==null)
                {
                    return new BaseResponse<RegionViewModel>()
                    {
                        Description="Найдено ноль элементов",
                        StatusCode=Domain.Enum.StatusCodeEnum.NotFound,
                        Data=null,
                    };
                }
                if(name!=null)
                {
                    region.Name= name;
                }
                if (parentId!=null)
                {
                    region.ParentId= parentId;
                }
                await _regionRepository.Update(region);
                var regionViewModel = MapToViewModel(region);
                return new BaseResponse<RegionViewModel>()
                {
                    Data=regionViewModel,
                    Description="регион найден и обновлен",
                    StatusCode=StatusCodeEnum.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RegionViewModel>()
                {
                    Description= $"[UpdateRegion]: {ex.Message}",
                    StatusCode = StatusCodeEnum.InternalServerError
                };
            }
        }

    }
}
