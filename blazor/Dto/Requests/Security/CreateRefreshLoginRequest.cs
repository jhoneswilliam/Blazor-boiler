using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using blazor.Enums.Security;

namespace blazor.Models.Security
{
    public class CreateRefreshLoginRequest
    {
        public string AuthToken { get; set; }
        public string Email { get; set; }
        public string AuthRefreshToken { get; set; }
        public EnumSignInType SignInType { get; set; }
    }
}
