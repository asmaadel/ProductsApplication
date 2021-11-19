using System;
using System.Collections.Generic;

namespace ProductsApplication.Models2
{
    public partial class OrderProducts
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int? Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
