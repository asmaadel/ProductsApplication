using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? orderDate { set; get; }
        public List<ProductDto> Products { set; get; }
    }
}
