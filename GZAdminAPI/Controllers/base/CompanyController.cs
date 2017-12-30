using System;
using System.Collections;
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
    public class CompanyController : ApiSystem
    {
        /// <summary>
        /// 公司明细
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get")]
        public GZAPIResult get(string rowID)
        {
            return GZAPISuccess(entities.base_company.Where(p => p.rowID == rowID).FirstOrDefault());
        }


        /// <summary>
        /// 公司检索
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("search")]
        public GZAPIResult search(string code)
        {
            var lst = entities.base_company.Where(p => p.companyName_chs.Contains(code) || p.companyName_en.Contains(code)).OrderBy(o => o.levelID);
            return GZAPISuccess(convertToTree(lst));
        }

        /// <summary>
        /// 获得公司列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult list()
        {
            // System.Web.HttpContext.Current.Request.Headers

            //请求日志
            //var request = System.Web.HttpContext.Current.Request;
            //request.InputStream.Position = 0;
            //byte[] byts = new byte[request.InputStream.Length];
            //request.InputStream.Read(byts, 0, byts.Length);
            //string msg = string.Format("请求Content:{0}", System.Text.Encoding.Default.GetString(byts));
            //LoggerHelper.Info(msg);

            //string where = "rowID=\"875dd8eb63214cf4a41f7006680da73f\"";
            var lst = entities.base_company
                //.Where(where)
                .OrderBy(o => o.levelID).ThenBy(o => o.companyName_chs);
            return GZAPISuccess(convertToTree(lst));
        }
        ///// <summary>
        ///// 获得公司列表 IviewTree使用
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public GZAPIResult treeData()
        //{
        //    var lst = entities.base_company.OrderBy(o => o.levelID).ThenBy(o => o.companyName_chs);

        //    var array = Tools.convert2Tree<TreeDataModel, base_company>(lst, p =>
        //     {
        //         return new TreeDataModel()
        //         {
        //             id = p.rowID,
        //             parentID = p.parentID,
        //             title = p.companyName_chs
        //         };
        //     });

        //    return GZAPISuccess(array);

        //}



        /// <summary>
        /// 新增模块
        /// </summary>
        [HttpPost]
        [ResponseType(typeof(base_company))]
        public GZAPIResult Create(base_company model)
        {
            if (model == null)
            {
                return base.GZAPINULLRequest();
            }
            if (!ModelState.IsValid)
            {
                return GZAPIBadRequest(ModelState);
            }

            //try
            //{

                model.rowID = Guid.NewGuid().ToString().Replace("-", "");
                entities.base_company.Add(model);
                entities.SaveChanges();
            //}
            //catch (DbUpdateException)
            //{
            //    return GZAPIBadRequest("公司编号不允许重复");
            //}
            //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
            // return Created("create", model);
            return GZAPISuccess(model);
        }

        /// <summary>
        /// 更新模块
        /// </summary>
        [HttpPost]
        public GZAPIResult Update(base_company model)
        {
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            entities.Entry(model).State = System.Data.EntityState.Modified;

            var ChildrenLst = entities.base_company.Where(w => w.parentFullID.Contains("/" + model.rowID));

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
                item.parentFullName = model.parentFullName + "/" + model.companyName_chs + "/" + String.Join("/", pname);
                item.levelID = model.levelID + pid.Count + 1;
                entities.Entry(item).State = System.Data.EntityState.Modified;
            }

            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                //这个是判断存在的一个方法  得你自己写 哦
                //return GZAPIThrowException(ex);
                return GZAPIBadRequest("公司编号不允许重复");
            }

            //return StatusCode(HttpStatusCode.NoContent);
            return GZAPISuccess(null);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="rowid"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult Delete(string rowid)
        {
            base_company c = new base_company() { rowID = rowid };
            entities.Entry(c).State = System.Data.EntityState.Deleted;
            entities.SaveChanges();
            return GZAPISuccess(null);
        }


        List<base_company> convertToTree(IEnumerable<base_company> lst)
        {
            List<base_company> array = new List<base_company>();
            
            foreach (var item in lst)
            {
                if (string.IsNullOrEmpty(item.parentID)) {
                    item.deep = 1;
                    array.Add(item);
                }
                else
                {
                    var v = findParent(array, item.parentID);
                    if (v != null)
                    {
                        item.deep = v.deep + 1;
                        if (v.children == null) v.children = new List<base_company>();
                        v.children.Add(item);
                    }
                    else
                    {
                        array.Add(item);
                    }
                }
            }
            return array;
        }

        base_company findParent(ICollection<base_company> lst, string rowID)
        {
            foreach (var item in lst)
            {
                if (item.rowID == rowID)
                    return item;
                else
                {
                    if (item.children != null && item.children.Count > 0)
                    {
                        var v = findParent(item.children, rowID);
                        if (v != null)
                            return v;
                    }
                }
            }
            return null;
        }

    }
}
