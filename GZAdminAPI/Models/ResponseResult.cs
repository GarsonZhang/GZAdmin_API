using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace vueAdminAPI.Models
{
    public class ResponseModel
    {
        private System.Net.HttpStatusCode httpStatusCode { get; set; }
        public ResponseModel(System.Net.HttpStatusCode httpCode)
        {
            httpStatusCode = httpCode;

            // new ExceptionResult.ApiControllerDependencyProvider(controller)
        }
        [Newtonsoft.Json.JsonIgnore]
        public HttpRequestMessage request { get; set; }
        /// <summary>
        /// 相应代码，0代表成功
        /// </summary>
        public EnumResponseCode error { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 返回内容
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// token过期时间
        /// </summary>
        public double servertime { get; set; }

        /// <summary>
        /// 新的私钥，为了安全，服务器每5分钟更新一次token
        /// </summary>
        public string tokenSecret { get; set; }
        public long rid { get; set; }

        /// <summary>
        /// 设置httpStatusCode
        /// </summary>
        /// <param name="httpCode"></param>
        public void setHttpStatusCode(System.Net.HttpStatusCode httpCode)
        {
            httpStatusCode = httpCode;
        }

        /// <summary>
        /// 获得response消息对象
        /// </summary>
        /// <returns></returns>
        public System.Net.Http.HttpResponseMessage getResponse()
        {

            HttpResponseMessage response = new HttpResponseMessage(httpStatusCode);

            try
            {
                //ArraySegment<byte> segment = Serialize();
                response.Content = new System.Net.Http.StringContent(getContent());
                System.Net.Http.Headers.MediaTypeHeaderValue contentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                contentType.CharSet = "utf-8";
                response.Content.Headers.ContentType = contentType;
                response.RequestMessage = request;
            }
            catch
            {
                response.Dispose();
                throw;
            }

            return response;


            //var response = new System.Net.Http.HttpResponseMessage();
            //response.Content = new System.Net.Http.StringContent(getContent());
            //response.StatusCode = httpStatusCode;
            //return response;
        }



        private string getContent()
        {
            if (request != null)
            {
                if (request.Properties.ContainsKey("user"))
                {
                    UserInfo user = request.Properties["user"] as UserInfo;

                    this.servertime = (user.LastTime.Value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    if (user.isSecreKeyUpdate)
                    {
                        this.tokenSecret = user.newSecretKey;
                        this.rid = user.newID;
                        user.isSecreKeyUpdate = false;
                    }
                }
            }
            //Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            //var v = new GZDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

            //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            //settings.Converters.Add(timeConverter);

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(this, System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
            return content;
        }

        public GZAPIResult getHttpActionResult()
        {
            return new GZAPIResult(this);
        }

    }

    public class GZAPIResult : IHttpActionResult
    {
        ResponseModel _model;
        public GZAPIResult(ResponseModel model)
        {
            _model = model;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_model.getResponse());
        }
    }

}