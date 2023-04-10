using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Response;
using WebApplication.Domain.ViewModel.RegionViewModel;
using WebApplication.Domain.ViewModel.User;

namespace WebApplication.Service.Interfaces
{
    public interface IRegionService
    {
        IBaseResponse<IEnumerable<RegionViewModel>> GetRegionWithChildren();
        Task<IBaseResponse<RegionViewModel>> GetRegionById(int id);
        Task<IBaseResponse<bool>> DeleteRegion(int id);

        Task<IBaseResponse<bool>> CreateRegion(RegionViewModel regionViewModel);

        Task<IBaseResponse<RegionViewModel>> UpdateRegion(int id,string? name,int? parentId);
    }
}
