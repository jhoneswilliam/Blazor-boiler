using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazor.Models
{
    public class Result : Result<Object>
    {
        public Result() : base()
        {
            this.Value = null;
        }
    }
    public class Result<T>
    {
        public T Value { get; set; }
        public bool ValidOperation { get; set; }
        public string Message { get; set; }

     
    }
}
