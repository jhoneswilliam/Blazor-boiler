using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace blazor.Models.Security
{
    public class CreateLoginRequest
    {
        public string Email{ get; set; }
        public string Password { get; set; }
    }
}
