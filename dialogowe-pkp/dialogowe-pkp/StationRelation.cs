using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogowe_pkp
{
    class StationRelation
    {
        public StationRelation(String fromA, String fromB, String toA, String toB)
        {
            FromA = fromA;
            FromB = fromB;
            ToA = toA;
            ToB = toB;
        }

        public StationRelation(StationTuple stationTupleA, StationTuple stationTupleB) : this(stationTupleA.Name, stationTupleA.Variant, stationTupleB.Name, stationTupleB.Variant)
        {
        }
        public String FromA { get; set; }
        public String ToA { get; set; }

        public String FromB { get; set; }
        public String ToB { get; set; }
    }
}
