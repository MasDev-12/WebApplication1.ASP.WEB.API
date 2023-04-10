using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Domain.ViewModel.RegionViewModel
{
    public class RegionViewModel
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<RegionViewModel> Children { get; set; } = new List<RegionViewModel>();
    }
}
