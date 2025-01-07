using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Models
{
    public abstract class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public DateTime DateAcquired { get; set; }
        public decimal Price { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }

        public Guid ItemTypeId { get; set; }
        public ItemType ItemType { get; set; }

        public Guid UserId { get; set; }
    }
}
