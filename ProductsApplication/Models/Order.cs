using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

    }
}
