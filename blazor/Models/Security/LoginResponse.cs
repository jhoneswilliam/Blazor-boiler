using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazor.Models.Security
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
