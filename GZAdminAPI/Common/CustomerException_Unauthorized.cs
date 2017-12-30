using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace vueAdminAPI.Common
{
    public class CustomerException_Unauthorized : HttpResponseException
    {
        public CustomerException_Unauthorized(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public CustomerException_Unauthorized(HttpResponseMessage response) : base(response)
        {
        }
    }
}