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
    public class DeptController : ApiSystem
    {
        /// <summary>
        /// 根据公司ID获得部门列表
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult list(string companyID)
        {
            //var lst = entities.base_dept.Where(w => w.companyID == companyID)
            //    .OrderBy(o => o.levelID);
            //.ThenBy(o => o.deptName).Skip(2).Take(4);//.Select(p=>p);



            var data = from dept in entities.base_dept
                       join company in entities.base_company on dept.companyID equals company.rowID
                       orderby dept.levelID
                       where dept.companyID == companyID
                       select new
                       {
                           dept,
                           company
                       };

            List<base_dept> result = new List<base_dept>();
            foreach (var i in data)
            {
                i.dept.companyName = i.company.companyName_chs;
                result.Add(i.dept);
            }

            var v = convertToTree(result);
            return GZAPISuccess(v);
            //return GZAPISuccess(convertToTree(data));
        }

        /// <summary>
        /// 部门检索
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("search")]
        public GZAPIResult search(string companyID, string code)
        {
            var lst = entities.base_dept
                //.Where(p => p.companyID == companyID && (p.deptCode.Contains(code) || p.deptName.Contains(code)))
                .Where(p => p.companyID == companyID)
                .Where(p => p.deptCode.Contains(code) || p.deptName.Contains(code))
                .OrderBy(o => o.levelID);
            return GZAPISuccess(convertToTree(lst));
        }

        /// <summary>
        /// 根据部门ID获得部门详细信息
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult Get(string rowID)
        {
            var v = entities.base_dept.Where(w => w.rowID == rowID).FirstOrDefault();
            return GZAPISuccess(v);
        }

        /// <summary>
        /// 获得部门列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult treeData()
        {
            var lst = entities.base_dept.OrderBy(o => o.levelID).ThenBy(o => o.deptName);

            var array = Tools.convert2Tree<TreeDataModel, base_dept>(lst, p =>
            {
                return new TreeDataModel()
                {
                    id = p.rowID,
                    parentID = p.parentID,
                    title = p.deptName
                };
            });

            return GZAPISuccess(array);

        }



        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public GZAPIResult Create(base_dept model)
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
                entities.base_dept.Add(model);
                entities.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return GZAPIBadRequest(e.Message);
            }
            //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
            //return Created("create", model);
            return GZAPISuccess(model);
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        [HttpPost]
        public GZAPIResult Update(base_dept model)
        {
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            entities.Entry(model).State = System.Data.EntityState.Modified;

            var ChildrenLst = entities.base_dept.Where(w => w.parentFullID.Contains("/" + model.rowID));

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
                item.parentFullName = model.parentFullName + "/" + model.deptName + "/" + String.Join("/", pname);
                item.levelID = model.levelID + pid.Count + 1;
                entities.Entry(item).State = System.Data.EntityState.Modified;
            }

            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //这个是判断存在的一个方法  得你自己写 哦
                return GZAPIThrowException(ex);
            }

            //return StatusCode(HttpStatusCode.NoContent);
            return GZAPISuccess(null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="rowid"></param>
        [HttpGet]
        public GZAPIResult Delete(string rowid)
        {
            base_dept c = new base_dept() { rowID = rowid };
            entities.Entry(c).State = System.Data.EntityState.Deleted;
            entities.SaveChanges();
            return GZAPISuccess(null);
        }


        private List<base_dept> convertToTree(IEnumerable<base_dept> lst)
        {
            List<base_dept> array = new List<base_dept>();
            Dictionary<string, base_dept> cacheData = new Dictionary<string, base_dept>();

            foreach (var item in lst)
            {
                cacheData.Add(item.rowID, item);
                if (!string.IsNullOrEmpty(item.parentID) && cacheData.ContainsKey(item.parentID))
                {
                    var parent = cacheData[item.parentID];
                    if (parent.children == null) parent.children = new List<base_dept>();
                    item.deep = parent.deep + 1;
                    parent.children.Add(item);
                }
                else
                {
                    item.deep = 1;
                    array.Add(item);
                }
            }
            return array;
        }

    }
}