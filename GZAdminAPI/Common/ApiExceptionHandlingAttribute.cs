using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using vueAdminAPI.Models;

namespace vueAdminAPI.Common
{
    /// <summary>
    /// API自定义错误过滤器属性
    /// </summary>
    public class ApiExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 统一对调用异常信息进行处理，返回自定义的异常信息
        /// </summary>
        /// <param name="context">HTTP上下文对象</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is CustomerException_Unauthorized)
                return;
            if (context.Exception is System.Data.Entity.Validation.DbEntityValidationException)
            {
                var EntityValidationErrors = (context.Exception as System.Data.Entity.Validation.DbEntityValidationException).EntityValidationErrors;
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                foreach (var item in EntityValidationErrors)
                {
                    foreach (var e in item.ValidationErrors)
                    {
                        stringBuilder.AppendLine($"{e.PropertyName}：{e.ErrorMessage}");
                    }
                }
                ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
                err.error = EnumResponseCode.errValidation;
                err.request = context.Request;
                err.message = stringBuilder.ToString();
                //return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse()); ;

                throw new HttpResponseException(err.getResponse());
            }
            if (context.Exception is DbUpdateException)
            {
                ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
                err.error = EnumResponseCode.errException;
                err.request = context.Request;
                err.message = context.Exception.ToString();
                //return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse()); ;

                throw new HttpResponseException(err.getResponse());
            }
            if (context.Exception is Exception)
            {
                ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
                err.error = EnumResponseCode.errException;
                err.request = context.Request;
                err.message = context.Exception.ToString();
                //return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse()); ;

                throw new HttpResponseException(err.getResponse());
            }

            ////自定义异常的处理
            //if (context.Exception is NotImplementedException)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented)
            //    {
            //        //封装处理异常信息，返回指定JSON对象
            //        Content = new StringContent(JsonHelper.ToJson(new ErrorModel((int)HttpStatusCode.NotImplemented, 0, ex.Message)), Encoding.UTF8, "application/json"),
            //        ReasonPhrase = "NotImplementedException"
            //    });
            //}
            //else if (context.Exception is TimeoutException)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
            //    {
            //        //封装处理异常信息，返回指定JSON对象
            //        Content = new StringContent(JsonHelper.ToJson(new ErrorModel((int)HttpStatusCode.RequestTimeout, 0, ex.Message)), Encoding.UTF8, "application/json"),
            //        ReasonPhrase = "TimeoutException"
            //    });
            //}
            ////.....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
            //else
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            //    {
            //        //封装处理异常信息，返回指定JSON对象
            //        Content = new StringContent(JsonHelper.ToJson(new ErrorModel((int)HttpStatusCode.InternalServerError, 0, ex.Message)), Encoding.UTF8, "application/json"),
            //        ReasonPhrase = "InternalServerErrorException"
            //    });
            //}

            //base.OnException(context);

            //记录关键的异常信息
            //Debug.WriteLine(context.Exception);
        }
    }
}