using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models
{
    public class JwtIssuerOptions
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
