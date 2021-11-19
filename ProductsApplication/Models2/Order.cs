using System;
using System.Collections.Generic;

namespace ProductsApplication.Models2
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProducts>();
        }

        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<OrderProducts> OrderProducts { get; set; }
    }
}
