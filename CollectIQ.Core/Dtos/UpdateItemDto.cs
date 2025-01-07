using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Dtos
{
    public class UpdateItemDto
    {
        // Common properties for all items
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public DateTime DateAcquired { get; set; }
        public decimal Price { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public Guid ItemTypeId { get; set; } // Link to ItemType

        // Watch-specific properties
        public string? MovementType { get; set; }
        public string? CaseMaterial { get; set; }
        public string? CaseDiameter { get; set; }
        public string? CaseThickness { get; set; }
        public string? BandMaterial { get; set; }
        public string? BandWidth { get; set; }

        // Cologne-specific properties
        public string? Type { get; set; } // Fragrance type (e.g., "Woody")
        public string? Concentration { get; set; } // e.g., "Eau de Parfum"
        public string? FragranceNotes { get; set; } // e.g., "Bergamot, Ambergris, Lavender"

        // Sneaker-specific properties
        public string? Size { get; set; } // e.g., "10"
        public string? Material { get; set; } // e.g., "Leather"
        public string? Colorway { get; set; } // e.g., "Red/Black/White"
    }
}
