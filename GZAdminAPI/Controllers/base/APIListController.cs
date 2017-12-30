using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vueAdminAPI.Common;
using vueAdminAPI.Models;

namespace vueAdminAPI.Controllers
{
    public class APIListController : ApiSystem
    {
        private void refreshCatch()
        {
            sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_APIList);
        }
        void saveChage()
        {
            entities.SaveChanges();
            refreshCatch();
        }
        /// <summary>
        /// 导入后台API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult import()
        {
            var v = Configuration.Services.GetApiExplorer().ApiDescriptions;
            List<apiModel> data = new List<apiModel>();
            foreach (var item in v)
            {
                apiModel model = new apiModel();
                model.title = item.Documentation;
                if (item.ActionDescriptor is System.Web.Http.Controllers.ReflectedHttpActionDescriptor)
                {
                    var _t = (APIAuthorizeAttribute)(item.ActionDescriptor as System.Web.Http.Controllers.ReflectedHttpActionDescriptor).MethodInfo.GetCustomAttributes(typeof(APIAuthorizeAttribute), false).FirstOrDefault();
                    if (_t != null)
                        model.authorize = (int)_t.RType;
                }

                //dictionary.Add("ID", item.ID);
                int i = item.RelativePath.IndexOf("?");
                if (i > 0)
                    model.RelativePath = "/" + item.RelativePath.Substring(0, i);

                else
                    model.RelativePath = "/" + item.RelativePath;
                //dictionary.Add("routeTemplate", item.Route.RouteTemplate);
                data.Add(model);
            }
            int count = 0;
            {
                var dbList = entities.base_APIList.ToList();
                var n = from api in data
                        where !dbList.Any(p => p.url == api.RelativePath)
                        select api;
                foreach (var item in n)
                {
                    base_APIList list = new base_APIList();
                    list.rowID = Guid.NewGuid().ToString();
                    list.APIName = item.title;
                    list.url = item.RelativePath;
                    list.checkType = item.authorize;
                    entities.Entry(list).State = System.Data.EntityState.Added;
                    count++;
                }
                saveChage();
            }

            string message = "导入成功,新增API数量：" + count;
            //Dictionary<string, object> dictionary = new Dictionary<string, object>();
            return GZAPISuccess(message, count);
        }

        /// <summary>
        /// 获取API接口列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult list(int pageSize, int pageIndex, string filter = "")
        {
            IQueryable<base_APIList> queryable = entities.base_APIList;
            if (!String.IsNullOrEmpty(filter))
                queryable = queryable.Where(p => p.url.Contains(filter) || p.APIName.Contains(filter));
            var count = queryable.Count();

            var data = queryable.OrderBy(o => o.url).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            

            foreach (var item in data)
            {
                item.checkTypeName = item.checkType == 0 ? "匿名访问" : item.checkType == 1 ? "登陆用户" : "权限用户";
            }

            //var count = Math.Ceiling(entities.base_APIList.Count() / (pageSize * 1.0));
           
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("totalPage", count);
            dic.Add("data", data);
            return GZAPISuccess(dic);
        }

        /// <summary>
        /// 获得API接口明细
        /// </summary>
        /// <param name="rowid"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult get(string rowid)
        {
            var data = entities.base_APIList.Where(p => p.rowID == rowid).First();
            return GZAPISuccess(data);
        }


        /// <summary>
        /// 更新API接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public GZAPIResult update(base_APIList data)
        {
            var validate = base.validateModel(data);
            if (validate != null) return validate;
            entities.Entry(data).State = System.Data.EntityState.Modified;
            //entities.SaveChanges();
            //sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_APIList);
            saveChage();
            return GZAPISuccess();
        }
        /// <summary>
        /// 删除API接口
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult delete(string rowID)
        {
            var data = entities.base_APIList.Where(p => p.rowID == rowID).FirstOrDefault();
            if (data != null)
            {
                entities.Entry(data).State = System.Data.EntityState.Deleted;
                //entities.SaveChanges();
                //sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_APIList);
                saveChage();
                return base.GZAPISuccess();
            }
            else
            {
                return base.GZAPIBadRequest("删除失败，没有找到数据");
            }

        }
    }

    public class apiModel
    {
        public apiModel()
        {
            authorize = 2;
        }
        public string title { get; set; }
        public string RelativePath { get; set; }
        public int authorize { get; set; }
    }
}
