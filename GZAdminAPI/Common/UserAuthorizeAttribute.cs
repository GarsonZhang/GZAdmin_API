using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using vueAdminAPI.Models;

namespace vueAdminAPI.Common
{
    public class UserAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        // baseDbContext DataBase;
        public UserAuthorizeAttribute()
        {
            // DataBase = new baseDbContext();
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            ResponseModel model = new ResponseModel(System.Net.HttpStatusCode.Unauthorized);
            model.error = EnumResponseCode.Unauthorized;
            model.message = "没有权限";
            var challengemessage = model.getResponse();
            challengemessage.Headers.Add("www-authenticate", "basic");
            throw new CustomerException_Unauthorized(challengemessage);
        }

        void HandleBadRequest(HttpActionContext actionContext, string message)
        {
            ResponseModel model = new ResponseModel(System.Net.HttpStatusCode.BadRequest);
            model.error = EnumResponseCode.err;
            model.message = message;
            var challengemessage = model.getResponse();
            challengemessage.Headers.Add("www-authenticate", "basic");
            throw new CustomerException_Unauthorized(challengemessage);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //double _d = Math.Pow(2, 10000);
            ////long.MaxValue
            //int 二的n次方 = Math.Pow(2, 50);
            var controller = actionContext.ControllerContext.Controller as Controllers.ApiBase;
   
            string fid = actionContext.Request.Headers.Contains("fid") ? actionContext.Request.Headers.GetValues("fid").FirstOrDefault() : "";

            UserInfo user = null;
            if (actionContext.Request.Properties.ContainsKey("user"))
            {
                user = actionContext.Request.Properties["user"] as UserInfo;
            }

            controller.currentUser = user;

            string localPath = actionContext.Request.RequestUri.LocalPath;
            var result = sysAuthorizeHandle.Intance.validate(fid, localPath, user);
            switch (result)
            {
                case sysAuthorizeHandle.AuthorizeErr.NoReg:
                    HandleBadRequest(actionContext, "接口未注册");
                    break;
                case sysAuthorizeHandle.AuthorizeErr.NoMap:
                    HandleBadRequest(actionContext, "接口在功能中未登记：" + localPath);
                    break;
                case sysAuthorizeHandle.AuthorizeErr.NoAuthorize:
                    HandleUnauthorizedRequest(actionContext);
                    break;
                case sysAuthorizeHandle.AuthorizeErr.accross:
                    IsAuthorized(actionContext);
                    break;
            }
            #region 旧的实现

            ////判断接口验证类型
            //var _api = from apilist in DataBase.base_APIList
            //           join actions in DataBase.base_moduleFormActions on apilist.rowID equals actions.apiID into temp
            //           from tt in temp.DefaultIfEmpty()
            //           where apilist.url == localPath
            //           select new
            //           {
            //               apilist.rowID,
            //               apilist.checkType,
            //               isid = tt == null ? 0 : tt.isid
            //           };
            //var api = _api.FirstOrDefault();

            //if (api == null)
            //{
            //    HandleBadRequest(actionContext, "接口未注册");
            //}
            //if (api.checkType == 0)
            //{
            //    IsAuthorized(actionContext);
            //    return;
            //}
            //if (api.isid == 0)
            //{
            //    HandleBadRequest(actionContext, "接口在功能中未登记");
            //}
            //if (user?.Account == "admin")//内置管理员拥有所有权限
            //{
            //    IsAuthorized(actionContext);
            //    return;
            //}
            //bool isAllowed = false;
            ////判断用户是否有权限
            //if (user != null)
            //{
            //    var cache = DataBase.base_moduleFormAuthorizeAction.Where(p => p.formID == fid && p.apiID == p.rowID);

            //    var r1 = from a in DataBase.base_moduleFormAuthorize
            //             join b in cache on a.rowID equals b.authorizeID into temp
            //             from tt in temp.DefaultIfEmpty()
            //             where (a.formID == fid && a.authorizeValue == 0)
            //             select a.authorizeValue;
            //    var s1 = r1.Sum();

            //    //用户映射权限
            //    var _userRelation = DataBase.base_userRelation.Where(w => w.userID == user.UserID).Select(p => new { p.ObjectID, p.Category }).ToList();

            //    var _position = (from relation in _userRelation
            //                     where relation.Category == 1
            //                     select relation.ObjectID).ToList();

            //    var positionData = DataBase.base_position.ToList();

            //    var query = from c in positionData
            //                where _position.Contains(c.rowID)
            //                select c;
            //    var _IuserRelation = query.Concat(query.SelectMany(t => getPositionData(positionData, new String[] { t.rowID })));
            //    var userRelation = _IuserRelation.Select(p => p.rowID).ToList();


            //    //ID ，用户权限
            //    var r2 = from c in DataBase.base_moduleFormAuthorize
            //             join d in DataBase.base_Authorize on c.rowID equals d.itemID
            //             where c.formID == fid && d.itemType == 2
            //             && ((d.objectID == user.UserID && d.category == 1) || userRelation.Contains(d.objectID))
            //             select c.authorizeValue;


            //    var s2 = r2.Sum();

            //    var av = new
            //    {
            //        v1 = r1.Sum(),
            //        v2 = r2.Sum()
            //    };

            //    //0,0
            //    var i1 = av.v1 & av.v2;
            //    if (!i1.HasValue)
            //        isAllowed = false;
            //    else
            //    {
            //        if (i1 == 0)
            //        {
            //            isAllowed = av.v1 == 0;
            //        }
            //        else
            //        {
            //            isAllowed = true;
            //        }
            //    }


            //    //var value = r.Sum();
            //    //if (value.HasValue) //有权限
            //    //{

            //    //}

            //}
            //if (!isAllowed)
            //{
            //    HandleUnauthorizedRequest(actionContext);
            //}

            //IsAuthorized(actionContext);
            #endregion
        }

    }
}