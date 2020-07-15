using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace blazor.ApiServices
{
    public abstract class BaseAPIService
    {
        public string UnknownFailureMessage { get; } = "An unexpected error happened";
        public string UnAuthorizedMessage { get; } = "Not authorized to accomplish this task";
    }
}
