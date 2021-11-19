using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication
{
    public class AppUserAuth
    {
        public AppUserAuth() : base()
        {
            UserName = "Not authorized";
            BearerToken = string.Empty;
        }
        public string UserName { get; set; }
        public string BearerToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserId { set; get; }

    }
}
