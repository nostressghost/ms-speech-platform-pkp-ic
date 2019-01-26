using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogowe_pkp
{
    public class Order
    {
        public String From;
        public String To;

        public Order()
        {

        }

        public Order(String from, String to)
        {
            From = from;
            To = to;
        }
    }
}
