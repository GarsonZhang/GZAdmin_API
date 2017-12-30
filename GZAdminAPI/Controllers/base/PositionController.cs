using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using vueAdminAPI.Common;
using vueAdminAPI.IviewModel;
using vueAdminAPI.Models;

namespace vueAdminAPI.Controllers
{
    public class PositionController : ApiSystem
    {
        /// <summary>
        /// 获得岗位列表
        /// </summary>
        /// <param name="companyID">公司ID</param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult list(string companyID)
        {
            var lst = entities.base_position.Where(w => w.companyID == companyID).OrderBy(o => o.levelID).ThenBy(o => o.posName);
            return GZAPISuccess(convertToTree(lst));
        }

        /// <summary>
        /// 检索岗位
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("search")]
        public GZAPIResult search(string companyID, string code)
        {
            var lst = entities.base_position
                .Where(p => p.companyID == companyID)
                .Where(p => p.posCode.Contains(code) || p.posName.Contains(code))
                .OrderBy(o => o.levelID);
            return GZAPISuccess(convertToTree(lst));
        }

        /// <summary>
        /// 获得岗位详细信息
        /// </summary>
        /// <param name="rowID">岗位ID</param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult Get(string rowID)
        {
            var data = entities.base_position.Where(w => w.rowID == rowID).FirstOrDefault();
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 获得岗位Tree列表
        /// </summary>
        /// <param name="companyID">公司ID</param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult treeData(string companyID)
        {
            var lst = entities.base_position.OrderBy(o => o.levelID).ThenBy(o => o.posName);

            var array = Tools.convert2Tree<TreeDataModel, base_position>(lst, p =>
            {
                return new TreeDataModel()
                {
                    id = p.rowID,
                    parentID = p.parentID,
                    title = p.posName
                };
            });

            return GZAPISuccess(array);

        }



        /// <summary>
        /// 创建岗位
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [ResponseType(typeof(base_position))]
        public GZAPIResult Create(base_position model)
        {
            if (model == null)
            {
                return base.GZAPINULLRequest();
            }
            if (!ModelState.IsValid)
            {
                return GZAPIBadRequest(ModelState);
            }

            try
            {

                model.rowID = Guid.NewGuid().ToString().Replace("-", "");
                entities.base_position.Add(model);
                entities.SaveChanges();
                refreshCache();
            }
            catch (DbUpdateException e)
            {
                return GZAPIThrowException(e);
            }
            //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
            //return Created("create", model);
            return GZAPISuccess(model);
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        [HttpPost]
        public GZAPIResult Update(base_position model)
        {
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            entities.Entry(model).State = System.Data.EntityState.Modified;

            var ChildrenLst = entities.base_position.Where(w => w.parentFullID.Contains("/" + model.rowID));

            List<string> pid = null;
            List<string> pname = null;
            int index = -1;
            foreach (var item in ChildrenLst)
            {
                pid = item.parentFullID.Split('/').ToList();
                pname = item.parentFullName.Split('/').ToList();
                index = pid.IndexOf(model.rowID);
                pid.RemoveRange(0, index + 1);
                pname.RemoveRange(0, index + 1);

                item.parentFullID = model.parentFullID + "/" + model.rowID + "/" + String.Join("/", pid);
                item.parentFullName = model.parentFullName + "/" + model.posName + "/" + String.Join("/", pname);
                item.levelID = model.levelID + pid.Count + 1;
                entities.Entry(item).State = System.Data.EntityState.Modified;
            }

            try
            {
                entities.SaveChanges();
                refreshCache();

            }
            catch (DbUpdateConcurrencyException ex)
            {
                return GZAPIThrowException(ex);
            }

            return GZAPISuccess();
        }
        void refreshCache() {
            sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_position);
        }
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="rowid"></param>
        [HttpGet]
        public GZAPIResult Delete(string rowid)
        {
            base_position c = new base_position() { rowID = rowid };
            entities.Entry(c).State = System.Data.EntityState.Deleted;
            entities.SaveChanges();
            refreshCache();
            return GZAPISuccess();
        }


        private List<base_position> convertToTree(IEnumerable<base_position> lst)
        {
            List<base_position> array = new List<base_position>();
            Dictionary<string, base_position> cacheData = new Dictionary<string, base_position>();

            foreach (var item in lst)
            {
                cacheData.Add(item.rowID, item);
                if (!string.IsNullOrEmpty(item.parentID) && cacheData.ContainsKey(item.parentID))
                {
                    var parent = cacheData[item.parentID];
                    if (parent.children == null) parent.children = new List<base_position>();
                    parent.children.Add(item);
                }
                else
                {
                    array.Add(item);
                }
            }
            return array;
        }

    }
}