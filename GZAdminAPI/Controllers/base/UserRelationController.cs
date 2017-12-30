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
    public class UserRelationController : ApiSystem
    {
        IEnumerable<userModel> queryable(IQueryable<base_user> userData, string objectID)
        {
            var tmpRelation = entities.base_userRelation.Where(w => w.Category == 1 && w.ObjectID == objectID);

            var data = from user in userData
                       join company in entities.base_company on user.companyID equals company.rowID
                       join dept in entities.base_dept on user.deptID equals dept.rowID
                       join relation in tmpRelation on user.rowID equals relation.userID into temp
                       from tt in temp.DefaultIfEmpty()
                       orderby user.userName
                       select new
                       {
                           user,
                           company.companyName_chs,
                           dept.deptName,
                           isSelect = tt == null ? false : true
                       };
            List<userModel> result = new List<userModel>();
            foreach (var i in data)
            {
                result.Add(new userModel()
                {
                    userID = i.user.rowID,
                    account = i.user.account,
                    userName = i.user.userName,
                    companyID = i.user.companyID,
                    companyName = i.companyName_chs,
                    deptID = i.user.deptID,
                    deptName = i.deptName,
                    isSelect = i.isSelect

                });
            }
            return result;
        }

        public class userModel
        {
            public string userID { get; set; }
            public string account { get; set; }
            public string userName { get; set; }
            public string companyID { get; set; }
            public string companyName { get; set; }
            public string deptID { get; set; }
            public string deptName { get; set; }
            public bool isSelect { get; set; }


        }

        /// <summary>
        /// 获得员工数据
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult list(string objectID)
        {
            var userData = entities.base_user.OrderBy(o => o.userName);
            var result = queryable(userData, objectID);
            return GZAPISuccess(result);
        }

        /// <summary>
        /// 查找部门内员工
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="deptID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult listByDept(string objectID, string deptID)
        {
            var userData = entities.base_user.Where(p => p.deptID == deptID).OrderBy(o => o.userName);
            var result = queryable(userData, objectID);
            return GZAPISuccess(result);
        }
        /// <summary>
        /// 查找公司内员工
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult listByCompanyID(string objectID, string companyID)
        {
            var userData = entities.base_user.Where(p => p.companyID == companyID).OrderBy(o => o.userName);
            var result = queryable(userData, objectID);
            return GZAPISuccess(result);
        }
        /// <summary>
        /// 根据条件查找员工
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="companyID"></param>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult listByQuery(string objectID, string companyID, string queryStr)
        {
            var userData = entities.base_user.Where(p => p.companyID == companyID && (p.account.Contains(queryStr) || p.userName.Contains(queryStr))).OrderBy(o => o.userName);
            var result = queryable(userData, objectID);
            return GZAPISuccess(result);
        }
        /// <summary>
        /// 更新岗位权限
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="category"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public GZAPIResult update(string objectID, int category, List<userModel> data)
        {


            //删除用户映射记录

            //第一种删除，一条sql语句
            //string sql = "delete from base_userRelation where objectID=@objectID and Category=1";
            //var pars = new DbParameter[]{
            //new SqlParameter{ParameterName ="@objectID",Value = objectID} };
            //entities.Database.ExecuteSqlCommand(sql, pars);

            //这里跟随大神的脚步用第二种  //开始你的表演

            //第二种删除
            var od = entities.base_userRelation.Where(w => w.ObjectID == objectID && w.Category == category);
            foreach (var item in od)
            {
                entities.Entry(item).State = System.Data.EntityState.Deleted;//还有这里，state和remove用一中就可以了吧
            }
            entities.SaveChanges();

            foreach (var user in data)
            {
                base_userRelation relation = new base_userRelation();
                relation.rowID = Tools.GUID;
                relation.userID = user.userID;
                relation.Category = category;
                relation.ObjectID = objectID;
                entities.base_userRelation.Add(relation);
            }

            //你这是 先删除所有 再新增所有？对
            entities.SaveChanges();
            refreshCache();
            return GZAPISuccess();

        }

        void refreshCache()
        {
            sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_userRelation);
        }
    }
}