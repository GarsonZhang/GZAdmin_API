using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vueAdminAPI.Models;
using static vueAdminAPI.Common.sysAuthorizeBiz;

namespace vueAdminAPI.Common
{
    public class sysAuthorizeCache
    {
        static sysAuthorizeCache _intance;
        public static sysAuthorizeCache Intance
        {
            get
            {
                if (_intance == null)
                    _intance = new sysAuthorizeCache();
                return _intance;
            }
        }
        DbContextSystem dbContext;
        sysAuthorizeCache()
        {
            dbContext = new DbContextSystem();
        }
        List<base_APIList> _apiList;
        public List<base_APIList> base_APIList
        {
            get
            {
                if (_apiList == null)
                {
                    lock (this)
                        _apiList = dbContext.base_APIList.ToList();
                }
                return _apiList;
            }
        }

        List<base_moduleFormActions> _formActions;
        public List<base_moduleFormActions> base_moduleFormActions
        {
            get
            {
                if (_formActions == null)
                {
                    lock (this)
                        _formActions = dbContext.base_moduleFormActions.ToList();
                }
                return _formActions;
            }
        }

        List<base_userRelation> _userRelation;
        public List<base_userRelation> base_userRelation
        {
            get
            {
                if (_userRelation == null)
                {
                    lock (this)
                        _userRelation = dbContext.base_userRelation.ToList();
                }
                return _userRelation;
            }
        }
        List<base_position> _position;
        public List<base_position> base_position
        {
            get
            {
                if (_position == null)
                {
                    lock (this)
                        _position = dbContext.base_position.ToList();
                }
                return _position;
            }
        }
        List<base_Authorize> _authorize;

        public List<base_Authorize> base_Authorize
        {
            get
            {
                if (_authorize == null)
                {
                    lock (this)
                        _authorize = dbContext.base_Authorize.ToList();
                }
                return _authorize;
            }
        }

        List<base_moduleFormAuthorizeAction> _formAuthorizeAction;
        public List<base_moduleFormAuthorizeAction> base_moduleFormAuthorizeAction
        {
            get
            {
                if (_formAuthorizeAction == null)
                {
                    lock (this)
                        _formAuthorizeAction = dbContext.base_moduleFormAuthorizeAction.ToList();
                }
                return _formAuthorizeAction;
            }
        }

        List<base_moduleFormAuthorize> _formAuthorize;
        public List<base_moduleFormAuthorize> base_moduleFormAuthorize
        {
            get
            {
                if (_formAuthorize == null)
                {
                    lock (this)
                        _formAuthorize = dbContext.base_moduleFormAuthorize.ToList();
                }
                return _formAuthorize;
            }
        }

        public void refreshCache(CacheFlag flag)
        {
            lock (this)
            {
                //var v = _apiList.Where(p => p.rowID == "22ace96b-03b9-4fe0-a312-86e7de8deb8e").ToList();
                if ((flag & CacheFlag.base_APIList) == CacheFlag.base_APIList)
                    _apiList = dbContext.base_APIList.AsNoTracking().ToList();
                if ((flag & CacheFlag.base_moduleFormActions) == CacheFlag.base_moduleFormActions)
                    _formActions = dbContext.base_moduleFormActions.AsNoTracking().ToList();
                if ((flag & CacheFlag.base_userRelation) == CacheFlag.base_userRelation)
                    _userRelation = dbContext.base_userRelation.AsNoTracking().ToList();
                if ((flag & CacheFlag.base_position) == CacheFlag.base_position)
                    _position = dbContext.base_position.AsNoTracking().ToList();
                if ((flag & CacheFlag.base_Authorize) == CacheFlag.base_Authorize)
                    _authorize = dbContext.base_Authorize.AsNoTracking().ToList();
                if ((flag & CacheFlag.base_moduleFormAuthorizeAction) == CacheFlag.base_moduleFormAuthorizeAction)
                    _formAuthorizeAction = dbContext.base_moduleFormAuthorizeAction.AsNoTracking().ToList();
                if ((flag & CacheFlag.base_moduleFormAuthorize) == CacheFlag.base_moduleFormAuthorize)
                    _formAuthorize = dbContext.base_moduleFormAuthorize.AsNoTracking().ToList();
            }

        }

    }


    public class sysAuthorizeBiz
    {
        //public static void addAPIListRange(IEnumerable<base_APIList> data)
        //{
        //    lock (sysAuthorizeCache.Intance.base_APIList)
        //    {
        //        foreach (var v in data)
        //            sysAuthorizeCache.Intance.base_APIList.Add(v);
        //    }
        //}

        //public static void addAPIList(base_APIList data)
        //{
        //    lock (sysAuthorizeCache.Intance.base_APIList)
        //        sysAuthorizeCache.Intance.base_APIList.Add(data);
        //}

        //public static void removeAPIList(string rowID)
        //{
        //    lock (sysAuthorizeCache.Intance.base_APIList)
        //    {
        //        var v = sysAuthorizeCache.Intance.base_APIList.FirstOrDefault(p => p.rowID == rowID);
        //        if (v != null)
        //            sysAuthorizeCache.Intance.base_APIList.Remove(v);
        //    }
        //}

        //public static void replaceAPIList(base_APIList data)
        //{
        //    lock (sysAuthorizeCache.Intance.base_APIList)
        //    {
        //        var v = sysAuthorizeCache.Intance.base_APIList.FirstOrDefault(p => p.rowID == data.rowID);
        //        if (v != null)
        //            sysAuthorizeCache.Intance.base_APIList.Remove(v);
        //        sysAuthorizeCache.Intance.base_APIList.Add(data);
        //    }
        //}

        //public static void updateModuleFormActionsRange(string formID, IEnumerable<base_moduleFormActions> data)
        //{
        //    lock (sysAuthorizeCache.Intance.base_moduleFormActions)
        //    {
        //        var lst = sysAuthorizeCache.Intance.base_moduleFormActions.Where(p => p.formID == formID).ToList();
        //        foreach (var d in lst)
        //            sysAuthorizeCache.Intance.base_moduleFormActions.Remove(d);
        //        foreach (var v in data)
        //            sysAuthorizeCache.Intance.base_moduleFormActions.Add(v);
        //    }
        //}

        public static void refreshCache(CacheFlag flag)
        {
            sysAuthorizeCache.Intance.refreshCache(flag);
        }
        public enum CacheFlag
        {
            base_APIList = 1,
            base_userRelation = 2,
            base_position = 4,
            base_Authorize = 8,
            base_moduleFormActions = 16,
            base_moduleFormAuthorize = 32,
            base_moduleFormAuthorizeAction = 64
        }

    }

    public class sysAuthorizeHandle
    {
        private static sysAuthorizeHandle _intance;
        public static sysAuthorizeHandle Intance
        {
            get
            {
                if (_intance == null)
                    _intance = new sysAuthorizeHandle();
                return _intance;
            }
        }
        sysAuthorizeCache cache = sysAuthorizeCache.Intance;
        public AuthorizeErr validate(string formID, string LocalPath, UserInfo user)
        {
            if (user?.Account == "administrator")//内置管理员拥有所有权限
                return AuthorizeErr.accross;
            // return AuthorizeErr.accross;
            //判断接口验证类型
            var _api = (from apilist in cache.base_APIList
                        join actions in cache.base_moduleFormActions.Where(p => p.formID == formID) on apilist.rowID equals actions.apiID into temp
                        from tt in temp.DefaultIfEmpty()
                        where apilist.url.ToLower() == LocalPath.ToLower()
                        select new
                        {
                            apilist.rowID,
                            apilist.checkType,
                            isid = tt == null ? 0 : tt.isid
                        });
            var api = _api.FirstOrDefault();
            if (api == null)//接口未注册
                return AuthorizeErr.NoReg;
            if (api.checkType == 0)
                return AuthorizeErr.accross;
            if (api.checkType == 1)
            {
                if (user != null)
                    return AuthorizeErr.accross;
                else
                    return AuthorizeErr.NoAuthorize;
            }
            if (api.isid == 0)
                return AuthorizeErr.NoMap;
            if (user?.Account == "admin")//内置管理员拥有所有权限
                return AuthorizeErr.accross;
            if (user == null)
                return AuthorizeErr.NoAuthorize;
            //判断用户是否有权限
            // 获得API对应的权限值
            var authorizeAction = cache.base_moduleFormAuthorizeAction.Where(p => p.formID == formID && p.apiID == api.rowID).Select(p => p.authorizeID);

            var r1 = from a in cache.base_moduleFormAuthorize
                         //join b in authorizeAction on a.rowID equals b.authorizeID into temp
                         //from tt in temp.DefaultIfEmpty()
                     where a.formID == formID && (a.authorizeValue == 0 || authorizeAction.Contains(a.rowID))
                     select a.authorizeValue;
            var authorizeValue = r1.Sum();
            if (r1.Count() == 0)
            {
                authorizeValue = null;
                return AuthorizeErr.NoMap;
            }
            //获得用户权限

            //用户映射权限
            var _userRelation = cache.base_userRelation.Where(w => w.userID == user.UserID).Select(p => new { p.ObjectID, p.Category }).ToList();

            //获得用户所属的职位
            var _position = (from relation in _userRelation
                             where relation.Category == 1
                             select relation.ObjectID).ToList();

            //获得职位权限，父级岗位拥有子级岗位的所有权限，即下级岗位分配了权限以后，上级岗位自然就包含了
            var query = from c in cache.base_position
                        where _position.Contains(c.rowID)
                        select c;
            var _IuserRelation = query.Concat(query.SelectMany(t => getPositionData(cache.base_position, new String[] { t.rowID })));
            var userAllpositionID = _IuserRelation.Select(p => p.rowID).ToList();


            //取得用户在改功能下的权限值
            var r2 = from c in cache.base_moduleFormAuthorize
                     join d in cache.base_Authorize on c.rowID equals d.itemID
                     where c.formID == formID && d.itemType == 2
                     && ((d.objectID == user.UserID && d.category == 1) || userAllpositionID.Contains(d.objectID))
                     select c.authorizeValue;
            int? userAuthorizeValue = r2.Sum();
            if (r2.Count() == 0)
                userAuthorizeValue = null;

            if (!userAuthorizeValue.HasValue)//如果没有找到用户的权限值信息，代表没有任何权限
                return AuthorizeErr.NoAuthorize;
            if (authorizeValue > 0)//如果API需要权限访问,需要验证用户是否有这个权限
            {
                if ((authorizeValue & userAuthorizeValue) != authorizeValue)
                    return AuthorizeErr.NoAuthorize;
            }
            return AuthorizeErr.accross;
        }

        private IEnumerable<base_position> getPositionData(IEnumerable<base_position> Data, IEnumerable<string> rowIDs)
        {
            var query = from c in Data
                        where rowIDs.Contains(c.parentID)
                        select c;
            return query.Concat(query.SelectMany(t => getPositionData(Data, new String[] { t.rowID })));
        }

        public enum AuthorizeErr
        {
            accross,
            NoReg,
            NoMap,
            NoAuthorize
        }
    }
}