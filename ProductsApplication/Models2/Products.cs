using System;
using System.Collections.Generic;

namespace ProductsApplication.Models2
{
    public partial class Products
    {
        public Products()
        {
            OrderProducts = new HashSet<OrderProducts>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Valid { get; set; }

        public virtual ICollection<OrderProducts> OrderProducts { get; set; }
    }
}
