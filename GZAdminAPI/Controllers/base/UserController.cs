using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vueAdminAPI.Common;
using vueAdminAPI.Models;

namespace vueAdminAPI.Controllers
{

    public class UserController : ApiSystem
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult list(int pageSize, int pageIndex)
        {
            IQueryable<base_user> userData;
            if (pageSize > 0)
                userData = entities.base_user
              .OrderBy(o => o.account).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            else
                userData = entities.base_user
                    .OrderBy(o => o.account);


            var result = queryable(userData);

            var count = Math.Ceiling(entities.base_user.Count() / (pageSize * 1.0));
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("totalPage", count);
            dic.Add("data", result);
            return GZAPISuccess(dic);
        }

        IEnumerable<base_user> queryable(IQueryable<base_user> userData)
        {
            var data = from user in userData
                       join company in entities.base_company on user.companyID equals company.rowID
                       join dept in entities.base_dept on user.deptID equals dept.rowID
                       orderby user.account
                       select new
                       {
                           user,
                           company.companyName_chs,
                           dept.deptName
                       };
            List<base_user> result = new List<base_user>();
            foreach (var i in data)
            {
                i.user.companyName = i.companyName_chs;
                i.user.deptName = i.deptName;
                result.Add(i.user);
            }
            return result;
        }

        /// <summary>
        /// 查找部门内员工
        /// </summary>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public GZAPIResult listByDept(string deptID)
        {
            var userData = entities.base_user.Where(p => p.deptID == deptID).OrderBy(o => o.userName);
            var result = queryable(userData);
            return GZAPISuccess(result);
        }
        /// <summary>
        /// 根据条件查找员工
        /// </summary>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public GZAPIResult listByQuery(string queryStr)
        {
            var userData = entities.base_user.Where(p => p.account.Contains(queryStr) || p.userName.Contains(queryStr)).OrderBy(o => o.userName);
            var result = queryable(userData);
            return GZAPISuccess(result);
        }

        /// <summary>
        /// 用户明细
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult get(string rowID)
        {
            var data = entities.base_user.Where(w => w.rowID == rowID);
            var result = queryable(data).FirstOrDefault();
            return GZAPISuccess(result);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorizeAttribute(APIAuthorizeAttribute.RequestType.everyOne)]
        public GZAPIResult login(string account, string pwd)
        {
            //获取用户
            var user = entities.base_user.Where(p => p.account == account && p.password == pwd).FirstOrDefault();
            if (user != null)
            {
                //request.Headers.Contains("sign") ? request.Headers.GetValues("sign").FirstOrDefault() : "";
                string client = Request.Headers.Contains("client") ? Request.Headers.GetValues("client").FirstOrDefault() : "";
                if (String.IsNullOrEmpty(client))
                {
                    return base.GZAPIBadRequest("缺少必要信息：Client not find", EnumResponseCode.errClient);
                }
                //刷新token
                var data = APIDataCache.UserHandle.updateToken(user, client, Request.GetClientIpAddress());

                return base.GZAPISuccess(new
                {
                    data.Token,
                    TokenSecret = data.newSecretKey,
                    rid = data.newID,
                    data.Account,
                    data.UserName,
                    data.Status
                });
            }
            else
            {
                return base.GZAPIBadRequest("用户名或密码错误！", EnumResponseCode.errPwd);
            }
        }

        /// <summary>
        /// 改变用户状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult changeStatus(string userID, int status)
        {
            var v = entities.base_user.Where(p => p.rowID == userID).FirstOrDefault();
            if (v != null)
            {
                v.status = status;
                entities.Entry(v).State = System.Data.EntityState.Modified;
                entities.SaveChanges();
                if (status != 0)
                {
                    APIDataCache.UserHandle.deleteUser(userID);
                }
                return GZAPISuccess();
            }
            else
            {
                return GZAPIBadRequest("用户不能存在");
            }
        }

        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult logout()
        {
            //base.currentUser
            APIDataCache.UserHandle.deleteToken(base.currentUser.Token);
            return GZAPISuccess();
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public GZAPIResult create(base_user model)
        {
            var result = base.validateModel(model);
            if (result != null) return result;

            try
            {
                model.rowID = Guid.NewGuid().ToString().Replace("-", "");
                model.password = "123456";
                entities.base_user.Add(model);
                entities.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return GZAPIBadRequest($"【用户账号】已经存在，请修改");
            }

            return GZAPISuccess(model);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        public GZAPIResult delete(string rowID)
        {
            base_user c = new base_user() { rowID = rowID };
            entities.Entry(c).State = System.Data.EntityState.Deleted;
            entities.SaveChanges();
            return GZAPISuccess();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public GZAPIResult update(base_user model)
        {
            var result = base.validateModel(model);
            if (result != null) return result;

            entities.Entry(model).State = System.Data.EntityState.Modified;
            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return GZAPIBadRequest($"【用户账号】已经存在，请修改");
            }

            return GZAPISuccess();
        }

    }
}
