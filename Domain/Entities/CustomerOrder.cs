using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CustomerOrder
    {
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
    }
}
