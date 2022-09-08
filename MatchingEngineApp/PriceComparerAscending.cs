using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp
{
    internal class PriceComparerAscending : IComparer<double>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(double x, double y)
        {
            if (x < y)
            {
                return -1;
            }
            else if (x > y)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
