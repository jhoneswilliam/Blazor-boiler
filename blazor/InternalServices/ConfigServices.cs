using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace blazor.InternalServices
{
    public class ConfigServices
    {
        private CultureInfo _culture;
        private String _baseApi;

        public CultureInfo Culture
        {
            get { return _culture; }
           
        }

        public String BaseApi
        {
            get { return _baseApi; }

        }

        public ConfigServices(IConfiguration configuration)
        {
            _baseApi = configuration.GetSection("BaseApi").Value;
            _culture = new CultureInfo(configuration.GetSection("BaseApi").Value);
        }
    }
}
