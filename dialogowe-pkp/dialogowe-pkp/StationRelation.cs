using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogowe_pkp
{
    class StationRelation
    {
        public StationRelation(String from, String to)
        {
            From = from;
            To = to;
        }

        public String From { get; set; }
        public String To { get; set; }
    }
}
