using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp.Dtos
{
    public class OrderBookDto
    {
        public Dictionary<double, IEnumerable<PriceLevelSideOrder>> bidSide { get; set; }
        public Dictionary<double, IEnumerable<PriceLevelSideOrder>> askSide { get; set; }
    }
}
