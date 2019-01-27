using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogowe_pkp
{
    class StationTuple
    {
        public StationTuple(String name, String variant)
        {
            Name = name;
            Variant = variant;
        }

        public String Name { get; set; }
        public String Variant { get; set; }
    }
}
