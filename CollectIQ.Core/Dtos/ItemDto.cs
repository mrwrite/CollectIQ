using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Dtos
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public DateTime DateAcquired { get; set; }
        public decimal Price { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ItemTypeName { get; set; }
        public Guid ItemTypeId { get; set; }
        public Guid UserId { get; set; }

        // Cologne-specific properties
        public string Type { get; set; } // Renamed from FragranceType
        public string Concentration { get; set; } // Existing property
        public string FragranceNotes { get; set; } // New property
        public string Size { get; set; } // New property

        //Watch-specific properties
        public string MovementType { get; set; }
        public string CaseMaterial { get; set; }
        public string CaseDiameter { get; set; }
        public string CaseThickness { get; set; }
        public string BandMaterial { get; set; }
        public string BandWidth { get; set; }

        //Sneaker-specific properties
        public string Colorway { get; set; }
    }
}
