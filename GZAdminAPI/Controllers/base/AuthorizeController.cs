using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vueAdminAPI.Common;
using vueAdminAPI.Models;

namespace vueAdminAPI.Controllers
{
    public class AuthorizeController : ApiSystem
    {
        /// <summary>
        /// 获得功能列表(根据权限ID自动勾选)
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult formTree(string objectID)
        {
            var authorizeForm = entities.base_Authorize.Where(p => p.objectID == objectID && p.itemType == 2).Select(p => p.itemID).ToList();//按钮
            var data = entities.base_module.OrderBy(o => o.sort).Select(module => new treeDataParent()
            {
                rowID = module.rowID,
                title = module.description,//标题
                icon = module.icon,
                expand = true,//展开
                type = 1,
                children = module.base_moduleForm.OrderBy(o => o.sort).Select(form => new treeDataParent()
                {
                    rowID = form.rowID,
                    title = form.description,
                    icon = "",
                    expand = true,//展开
                    type = 2,
                    children = form.base_moduleFormAuthorize.OrderBy(p => p.authorizeValue).Select(authorize => new treeDataBase()
                    {
                        rowID = authorize.rowID,
                        title = authorize.authorizeName,
                        icon = "",
                        type = 3,
                        isChecked = authorizeForm.Contains(authorize.rowID)
                    })
                })
            }).ToList();
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 更新权限映射
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public GZAPIResult update(udateModel model)
        {
            //删除旧的映射
            var pars = new DbParameter[]{
            new SqlParameter{ParameterName ="@objectID",Value = model.ObjectID} };
            entities.Database.ExecuteSqlCommand("delete from base_authorize where objectID=@objectID", pars);

            if (model.data != null)
            {
                foreach (var item in model.data)
                {
                    if (item.isChecked)
                    {
                        base_Authorize authorize = new base_Authorize();
                        authorize.rowID = Tools.GUID;
                        authorize.category = model.Category;
                        authorize.objectID = model.ObjectID;
                        authorize.itemID = item.rowID;
                        authorize.itemType = 2;
                        entities.base_Authorize.Add(authorize);
                    }
                }
            }

            entities.SaveChanges();
            refreshCache();

            return GZAPISuccess();
        }
        void refreshCache()
        {
            sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_Authorize);
        }

        public class udateModel
        {
            public string ObjectID { get; set; }

            public int Category { get; set; }

            public IEnumerable<treeDataBase> data { get; set; }
        }



        public class treeDataBase
        {
            public int type { get; set; }
            public string rowID { get; set; }
            public string title { get; set; }
            public string icon { get; set; }
            public bool expand { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "checked")]
            public bool isChecked { get; set; }
        }
        public class treeDataParent : treeDataBase
        {
            public IEnumerable<treeDataBase> children { get; set; }
        }
    }
}
