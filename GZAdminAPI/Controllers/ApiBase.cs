using System;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using vueAdminAPI.Common;
using vueAdminAPI.Models;

namespace vueAdminAPI.Controllers
{
    public class ApiDbContext<T> : ApiBase where T : IGZAPIDBContext, new()
    {
        public T entities;

        public ApiDbContext()
        {
            _entities = entities = new T();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && entities != null)
                entities.Dispose();
            base.Dispose(disposing);


        }

    }

    [ApiExceptionHandlingAttribute]
    [UserAuthorizeAttribute]
    public class ApiBase : ApiController
    {
        private UserInfo _user;

        protected IGZAPIDBContext _entities;


        public UserInfo currentUser
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                if (_entities != null)
                    _entities._userInfo = value;
            }
        }

        public ApiBase()
        {
        }


        /// <summary>
        /// 验证模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected GZAPIResult validateModel(object model)
        {
            if (model == null)
            {
                return GZAPINULLRequest();
            }
            if (!ModelState.IsValid)
            {
                return GZAPIBadRequest(ModelState);
            }
            return null;
        }

        ResponseModel getResponseModel(System.Net.HttpStatusCode httpCode, EnumResponseCode errCode, string message, object data)
        {
            ResponseModel result = new ResponseModel(httpCode);
            result.request = this.Request;
            result.error = errCode;
            result.message = message;
            result.data = data;
            return result;
        }

        //[Obsolete("方法已经过时，请使用BadRequestModelState代替", true)]
        protected GZAPIResult GZAPIBadRequest(ModelStateDictionary modelState)
        {
            HttpError error = new HttpError(modelState, true);
            var result = getResponseModel(System.Net.HttpStatusCode.BadRequest, EnumResponseCode.errModelState, null, error);
            return result.getHttpActionResult();
        }

        protected GZAPIResult GZAPINULLRequest()
        {
            return GZAPIBadRequest("对象为空！");
        }
        protected GZAPIResult GZAPIBadRequest()
        {
            return GZAPIBadRequest("错误的请求");
        }

        protected GZAPIResult GZAPIBadRequest(string message)
        {

            var result = getResponseModel(System.Net.HttpStatusCode.BadRequest,
                EnumResponseCode.err, message, null);
            return result.getHttpActionResult();
        }
        protected GZAPIResult GZAPIBadRequest(string message, EnumResponseCode code)
        {

            var result = getResponseModel(System.Net.HttpStatusCode.BadRequest,
                code, message, null);
            return result.getHttpActionResult();
        }

        protected GZAPIResult GZAPIThrowException(Exception exception)
        {

            var result = getResponseModel(System.Net.HttpStatusCode.BadRequest,
                EnumResponseCode.errException, exception.Message, null);
            return result.getHttpActionResult();
        }
        protected GZAPIResult GZAPISuccess(object obj)
        {
            var result = getResponseModel(System.Net.HttpStatusCode.OK,
                EnumResponseCode.success, "请求成功", obj);
            return result.getHttpActionResult();
        }
        protected GZAPIResult GZAPISuccess()
        {
            return GZAPISuccess("请求成功");
        }

        protected GZAPIResult GZAPISuccess(string message)
        {
            var result = getResponseModel(System.Net.HttpStatusCode.OK,
                EnumResponseCode.success, message, null);
            return result.getHttpActionResult();
        }
        protected GZAPIResult GZAPISuccess(string message, object data)
        {
            var result = getResponseModel(System.Net.HttpStatusCode.OK,
                EnumResponseCode.success, message, data);
            return result.getHttpActionResult();
        }


    }
}