using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ProductsApplication
{
    public class User : IdentityUser
    {
        public User()
        {
            Order = new HashSet<Order>();
        }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Order> Order { get; set; }

    }
}
