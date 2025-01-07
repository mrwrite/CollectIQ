using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Models
{
    public class Watch : Item
    {
       
        public string MovementType { get; set; }
        public string CaseMaterial { get; set; }
        public string CaseDiameter { get; set; }
        public string CaseThickness { get; set; }
        public string BandMaterial { get; set; }
        public string BandWidth { get; set; }
    }
}
