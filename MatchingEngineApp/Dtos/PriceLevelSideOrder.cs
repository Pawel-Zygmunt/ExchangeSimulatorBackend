using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp.Dtos
{
    public class PriceLevelSideOrder
    {
        public Guid UserId { get; set; }
        public uint Quantity { get; set; }
    }
}
