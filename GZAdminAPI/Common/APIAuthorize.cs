using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vueAdminAPI.Common
{
    public class APIAuthorizeAttribute : Attribute
    {
        public RequestType RType = RequestType.everyOne;

        public APIAuthorizeAttribute(RequestType type)
        {
            RType = type;
        }
        public enum RequestType
        {
            //所有人都可以请求，开放接口
            everyOne = 0,
            //只要登陆用户即可访问，无需特殊权限
            login = 1,
            //只有通过权限验证的用户才能请求
            authorize = 2

        }
    }
}