using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Domain.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int Amount { get; set; }
    }
}
