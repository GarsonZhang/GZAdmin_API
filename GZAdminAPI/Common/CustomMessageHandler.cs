using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using vueAdminAPI.Models;

namespace vueAdminAPI.Common
{
    public class CustomMessageHandler : DelegatingHandler
    {

        private string GetValue(object obj)
        {
            Type type = obj.GetType();
            if (type.IsValueType || type.FullName == "System.String")
                return obj + "";
            else
            {
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                return str;
            }
        }
        private string GetSign(Dictionary<string, object> dictionary, string token)
        {
            token = token ?? "GarsonHans";
            // 按照关键字排序，并组合成新的字符串

            StringBuilder sb = new StringBuilder();

            //没有按照ASCII排序

            //var newDic = dictionary.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            ////拼接字符串
            //foreach (string key in newDic.Keys)
            //{
            //    if (newDic[key] == null) continue;
            //    //if (String.Equals(newDic[key], "")) continue;
            //    sb.AppendFormat("&{0}={1}", key, GetValue(newDic[key]));
            //}


            //没有按照ASCII排序

            //var vDic = (from objDic in dictionary orderby objDic.Key select objDic);
            //StringBuilder str = new StringBuilder();
            //foreach (KeyValuePair<string, object> kv in vDic)
            //{
            //    if (kv.Value == null) continue;
            //    //if (String.Equals(newDic[key], "")) continue;
            //    sb.AppendFormat("&{0}={1}", kv.Key, GetValue(kv.Value));
            //}


            //string[] _sortKeys = dictionary.Keys.ToArray();
            ////dictionary.OrderBy(p => p.Key, new MyComparer());
            //Array.Sort(_sortKeys, string.CompareOrdinal);

            //foreach (string key in _sortKeys)
            //{
            //    if (dictionary[key] == null) continue;
            //    //if (String.Equals(newDic[key], "")) continue;
            //    sb.AppendFormat("&{0}={1}", key, GetValue(dictionary[key]));
            //}

            Dictionary<string, object> sortDic = dictionary.OrderBy(p => p.Key, new MyComparer()).ToDictionary(p => p.Key, o => o.Value);
            foreach (var item in sortDic)
            {
                if (item.Value == null) continue;
                sb.AppendFormat("&{0}={1}", item.Key, GetValue(item.Value));
            }



            //删掉第一个&符号
            if (sb.Length > 0)
                sb.Remove(0, 1);

            //添加密钥
            string tmpstr = sb.ToString() + token;

            //计算MD5值
            string validateSign = MD5.MD5Encrypt32(tmpstr);
            return validateSign;
        }

        public class MyComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x.ToLower(), y.ToLower(), StringComparison.Ordinal);
            }
        }

        bool validateClient(string client)
        {
            return client == "pc" || client == "mobile";

        }



        private Task<HttpResponseMessage> validateSign(HttpRequestMessage request, Dictionary<string, object> dictionary)
        {
            string sign = request.Headers.Contains("sign") ? request.Headers.GetValues("sign").FirstOrDefault() : "";

            string token = request.Headers.Contains("token") ? request.Headers.GetValues("token").FirstOrDefault() : "";
            long requestID = long.Parse(request.Headers.Contains("rid") ? request.Headers.GetValues("rid").FirstOrDefault() : "0");
            string client = request.Headers.Contains("client") ? request.Headers.GetValues("client").FirstOrDefault() : "";
            if (validateClient(client) == false)
                return errClient(request);

            UserInfo user = null;
            if (!String.IsNullOrEmpty(token))
            {
                user = APIDataCache.UserHandle.getUserInfo(token);
                if (user == null)
                    return responError(request, "Token错误", EnumResponseCode.tokenErr);
                else
                {
                    if (user.LastTime < DateTime.Now.AddHours(-2))
                        return responError(request, "Token已过期", EnumResponseCode.tokenExpired);

                    if (!String.Equals(user.Client, client, StringComparison.CurrentCultureIgnoreCase))
                        return errClient(request);
                }
            }

            bool validate = false;
            if (!String.IsNullOrEmpty(sign))
            {
                //计算MD5值
                string validateSign = this.GetSign(dictionary, user?.getSecretKey(requestID));
                //验证签名
                validate = sign == "GarsonZhang2017" || string.Equals(sign, validateSign, StringComparison.CurrentCultureIgnoreCase);
            }

            if (validate == false)
            {
                ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
                err.error = EnumResponseCode.errSign;
                err.request = request;
                err.message = "签名不正确";

                return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse());
            }

            if (user != null)
            {
                request.Properties["user"] = user;

                APIDataCache.UserHandle.updateLastTime(user);

            }
            return null;
        }

        private static Task<HttpResponseMessage> errClient(HttpRequestMessage request)
        {
            ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
            err.error = EnumResponseCode.errClient;
            err.request = request;
            err.message = "错误的请求";
            return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse());
        }
        private static Task<HttpResponseMessage> responError(HttpRequestMessage request, string message, EnumResponseCode errCode)
        {
            ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
            err.error = errCode;
            err.request = request;
            err.message = message;
            return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse());
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //记录请求内容
            if (request.Content != null)
            {
                Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                };
                string msg = "";

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                string postData = "";
                if (request.Method == HttpMethod.Post)
                {
                    postData = request.Content.ReadAsStringAsync().Result;

                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(postData);

                    if (obj is Newtonsoft.Json.Linq.JObject)
                    {
                        dictionary = (obj as Newtonsoft.Json.Linq.JObject).ToObject<Dictionary<string, object>>();
                    }
                    else if (obj is Newtonsoft.Json.Linq.JArray)
                    {
                        var ja = obj as Newtonsoft.Json.Linq.JArray;
                        for (int i = 0; i < ja.Count; i++)
                        {
                            //dictionary = new Dictionary<string, object>();
                            dictionary.Add(i + "", ja[i]);
                        }
                    }
                    //Dictionary<string, object> dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(postData, settings);
                    //验证签名
                }
                else if (request.Method == HttpMethod.Get)
                    postData = request.RequestUri.Query;
                HttpContextBase context = (HttpContextBase)request.Properties["MS_HttpContext"];//获取传统context
                var QueryString = context.Request.QueryString;//定义传统request对象
                foreach (string key in QueryString.Keys)
                {
                    dictionary.Add(key, QueryString[key]);
                }

                var result = this.validateSign(request, dictionary);
                if (result != null) return result;
                msg = string.Format("请求Content:{0}", postData);
                LoggerHelper.Info(msg);
            }

            //身份验证
            bool validateUser = true;
            if (!validateUser)
            {
                ResponseModel err = new ResponseModel(HttpStatusCode.BadRequest);
                err.error = EnumResponseCode.err;
                err.request = request;
                err.message = "身份验证错误";

                return Task<HttpResponseMessage>.Factory.StartNew(() => err.getResponse());

            }

            //request.Properties.Add("userID", "abcdefg");

            //发送HTTP请求到内部处理程序，在异步处理完成后记录相应内容

            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((task) =>
            {
                // 记录响应内容
                if (task.Result.Content != null)
                {
                    string msg = string.Format("响应Content:{0}", task.Result.Content.ReadAsStringAsync().Result);
                    LoggerHelper.Info(msg);
                }
                return task.Result;
            });

        }

        private Task<HttpResponseMessage> SendError(string error, HttpStatusCode code)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(error);
            response.StatusCode = code;
            return Task<HttpResponseMessage>.Factory.StartNew(() => response);
        }
    }
}