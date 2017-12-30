using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using vueAdminAPI.Common;
using vueAdminAPI.Models;

namespace vueAdminAPI.Controllers
{

    public class ModuleController : ApiSystem
    {
        /// <summary>
        /// 获得所有菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/menu/all")]
        [APIAuthorizeAttribute(APIAuthorizeAttribute.RequestType.login)]
        public GZAPIResult MenuAll()
        {
            if (base.currentUser.Account == "administrator")
                return all();
            if (base.currentUser.Account == "admin")
                return all();
            else
                return userAuthority();
        }
        GZAPIResult userAuthority()
        {
            //用户映射权限
            var userRelation = entities.base_userRelation.Where(w => w.userID == currentUser.UserID).Select(p => p.ObjectID);

            var myAuthrorize = entities.base_Authorize.Where(p => p.itemType == 2 && ((p.category == 1 && p.objectID == currentUser.UserID) || userRelation.Contains(p.objectID))).Select(p => p.itemID).Distinct().ToList();

            var formAuthorize = entities.base_moduleFormAuthorize.Where(p => myAuthrorize.Contains(p.rowID))
                .GroupBy(p => p.formID)
                .Select(p => new { formID = p.Key, authorizeValue = p.Sum(s => s.authorizeValue) });

            var moduleForm = (from form in entities.base_moduleForm
                              join authorize in formAuthorize on form.rowID equals authorize.formID
                              select new
                              {
                                  form.rowID,
                                  form.mainID,
                                  form.name,
                                  form.description,
                                  form.description_tw,
                                  form.description_en,
                                  form.description_other,
                                  form.routeName,
                                  form.icon,
                                  form.componentPath,
                                  form.componentName,
                                  form.sort,
                                  authorize.authorizeValue
                              }).ToList();

            var moduleIDs = from f in moduleForm
                            group f by f.mainID into g
                            select g.Key;



            var module = entities.base_module.Where(p => moduleIDs.Contains(p.rowID)).OrderBy(o => o.sort).ToList();

            var data = from _m in module
                       select new
                       {
                           name = _m.name,
                           text = _m.description,
                           text_tw = _m.description_tw,
                           text_en = _m.description_en,
                           text_other = _m.description_other,
                           icon = _m.icon,
                           componentPath = _m.componentPath,
                           componentName = _m.componentName,
                           type = 1,
                           items = from _f in moduleForm
                                   where _f.mainID == _m.rowID
                                   orderby _f.sort
                                   select new
                                   {
                                       id = _f.rowID,
                                       name = _m.name + "-" + _f.name,
                                       text = _f.description,
                                       text_tw = _f.description_tw,
                                       text_en = _f.description_en,
                                       text_other = _f.description_other,
                                       routeName = _f.routeName,
                                       icon = _f.icon,
                                       type = 2,
                                       componentPath = _f.componentPath,
                                       componentName = _f.componentName,
                                       authorize = _f.authorizeValue
                                   }
                       };



            //var data = entities.base_module.OrderBy(o => o.sort).Select(b => new
            //{
            //    name = b.name,
            //    text = b.description,
            //    icon = b.icon,
            //    componentPath = b.componentPath,
            //    componentName = b.componentName,
            //    type = 1,
            //    items = b.base_moduleForm.OrderBy(o => o.sort).Select(c => new
            //    {
            //        id = c.rowID,
            //        name = b.name + "-" + c.name,
            //        text = c.description,
            //        routeName = c.routeName,
            //        icon = c.icon,
            //        type = 2,
            //        componentPath = c.componentPath,
            //        componentName = c.componentName,
            //        authorize = c.base_moduleFormAuthorize.Where(p => myAuthrorize.Contains(p.rowID)).Sum(authorize => authorize.authorizeValue)
            //    })
            //}).ToList();



            return GZAPISuccess(data);
        }

        GZAPIResult all()
        {
            var data = entities.base_module.OrderBy(o => o.sort).Select(b => new
            {
                name = b.name,
                text = b.description,
                text_tw = b.description_tw,
                text_en = b.description_en,
                text_other = b.description_other,
                icon = b.icon,
                componentPath = b.componentPath,
                componentName = b.componentName,
                type = 1,
                items = b.base_moduleForm.OrderBy(o => o.sort).Select(c => new
                {
                    id = c.rowID,
                    name = b.name + "-" + c.name,
                    text = c.description,
                    text_tw = c.description_tw,
                    text_en = c.description_en,
                    text_other = c.description_other,
                    routeName = c.routeName,
                    icon = c.icon,
                    type = 2,
                    componentPath = c.componentPath,
                    componentName = c.componentName,
                    authorize = 65535
                })
            }).ToList();
            return GZAPISuccess(data);
        }



        /// <summary>
        /// 获得模块列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/module/list")]
        public GZAPIResult ModuleList()
        {
            //sort是我手动加的，先加数据库字段，然后再加对象属性
            var data = entities.base_module.OrderBy(o => o.sort).Select(b => new
            {
                b.rowID,
                b.name,
                b.description,
                b.description_tw,
                b.description_en,
                b.description_other,
                b.icon,
                b.componentPath,
                b.componentName,
                b.sort
            }).ToList();
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 根据模块ID获得模块
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/module/get")]
        public GZAPIResult ModuleGet(string rowID)
        {
            var data = entities.base_module.Where(w => w.rowID == rowID).Select(b => new
            {
                b.rowID,
                b.name,
                b.description,
                b.description_tw,
                b.description_en,
                b.description_other,
                b.icon,
                b.componentPath,
                b.componentName,
                b.sort
            }).FirstOrDefault();
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 新增模块
        /// </summary>
        [HttpPost]
        [Route("api/module/create")]
        public GZAPIResult ModuleCreate(base_module model)
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
                entities.base_module.Add(model);
                entities.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return GZAPIBadRequest($"模块编码【{model.name}】已经存在，请修改");
            }
            //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
            var data = new
            {
                model.rowID,
                model.name,
                model.description,
                model.description_tw,
                model.description_en,
                model.description_other,
                model.icon,
                model.componentPath,
                model.componentName,
                model.sort
            };
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 更新模块
        /// </summary>
        [HttpPost]
        [Route("api/module/update")]
        public GZAPIResult ModuleUpdate(base_module model)
        {
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            entities.Entry(model).State = System.Data.EntityState.Modified;
            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //这个是判断存在的一个方法  得你自己写 哦
                return GZAPIThrowException(e);
            }

            return GZAPISuccess();
        }

        /// <summary>
        /// 批量更新模块，排序字段用到
        /// </summary>
        /// <param name="modules"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/module/updatebatch")]
        public GZAPIResult ModuleUpdateBatch(List<base_module> modules)
        {
            if (modules == null || modules.Count == 0)
                return base.GZAPINULLRequest();
            if (!ModelState.IsValid)
                return GZAPIBadRequest(ModelState);

            foreach (base_module m in modules)
            {
                entities.Entry(m).State = System.Data.EntityState.Modified;
            }
            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //这个是判断存在的一个方法  得你自己写 哦
                return GZAPIThrowException(e);
            }

            return GZAPISuccess();
        }

        /// <summary>
        /// 模块删除，会同时删除当前模块下所有功能
        /// </summary>
        /// <param name="rowID"></param>
        [HttpGet]
        [Route("api/module/delete")]
        public GZAPIResult ModuleDelete(string rowID)
        {
            #region 官方推荐写法
            /*
            var a = entities.base_module.FirstOrDefault(m => m.rowID == id);
            if (a != null)
            {
                entities.base_module.Remove(a);
            }
            var i = entities.SaveChanges();
            */
            #endregion

            #region 第二种写法，缺点，只能根据主键来删除，因为只能对指定主键的对象进行操作
            /*
            base_module b = new base_module() { rowID = id };
            entities.base_module.Attach(b);
            entities.base_module.Remove(b);
            var i = entities.SaveChanges();
            */
            #endregion

            #region 第三种写法，和第二种写法一样
            base_module c = new base_module() { rowID = rowID };
            entities.Entry(c).State = System.Data.EntityState.Deleted;
            entities.SaveChanges();
            return GZAPISuccess();
            #endregion
        }

        /// <summary>
        /// 获得模块功能列表
        /// </summary>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/moduleForm/list")]
        public GZAPIResult FormList(string moduleID)
        {
            var data = entities.base_moduleForm.Where(p => p.mainID == moduleID).OrderBy(o => o.sort).Select(s => new
            {
                s.rowID,
                s.name,
                s.mainID,
                s.description,
                s.description_tw,
                s.description_en,
                s.description_other,
                s.routeName,
                s.componentPath,
                s.componentName,
                s.sort
            });
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 获得功能详情
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/moduleForm/get")]
        public GZAPIResult FormGet(string rowID)
        {
            var data = entities.base_moduleForm.Where(p => p.rowID == rowID).Select(s => new
            {
                s.rowID,
                s.name,
                s.mainID,
                s.description,
                s.description_tw,
                s.description_en,
                s.description_other,
                s.routeName,
                s.componentPath,
                s.componentName,
                s.sort,
                actions = s.base_moduleFormActions.Select<base_moduleFormActions, string>(p => p.apiID)
            }).FirstOrDefault();
            return GZAPISuccess(data);
        }

        /// <summary>
        /// 新增模块功能
        /// </summary>
        [HttpPost]
        //[ResponseType(typeof(base_moduleForm))]
        [Route("api/moduleForm/create")]
        public GZAPIResult FormCreate(base_moduleForm model)
        {
            //修改 好像是直接赋值  就可以了 ，这个id不用？
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            try
            {
                model.rowID = Tools.GUID;
                entities.base_moduleForm.Add(model);
                foreach (string apiID in model.actions)
                {
                    base_moduleFormActions action = new base_moduleFormActions();
                    action.rowID = Tools.GUID;
                    action.formID = model.rowID;
                    action.apiID = apiID;
                    entities.base_moduleFormActions.Add(action);
                }
                //添加一个访问权限
                base_moduleFormAuthorize authorize = new base_moduleFormAuthorize();
                authorize.authorizeName = "访问";
                authorize.authorizeValue = 0;
                authorize.formID = model.rowID;
                authorize.rowID = Tools.GUID;
                entities.base_moduleFormAuthorize.Add(authorize);


                entities.SaveChanges();
                //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
                sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_moduleFormActions | sysAuthorizeBiz.CacheFlag.base_moduleFormAuthorize);
                return GZAPISuccess(model);
            }
            catch (DbUpdateException)
            {
                return GZAPIBadRequest($"模块编码【{model.name}】已经存在，请修改");
            }
            //这个就是标准的 新增并返回 新增后的 实体
        }


        /// <summary>
        /// 更新模块功能
        /// </summary>
        [HttpPost]
        [Route("api/moduleForm/update")]
        public GZAPIResult FormUpdate(base_moduleForm model)
        {
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            entities.Entry(model).State = System.Data.EntityState.Modified;

            var actions = entities.base_moduleFormActions.Where(p => p.formID == model.rowID);
            foreach (var item in actions)
            {
                entities.Entry(item).State = System.Data.EntityState.Deleted;
            }

            foreach (string apiID in model.actions)
            {
                base_moduleFormActions action = new base_moduleFormActions();
                action.rowID = Tools.GUID;
                action.formID = model.rowID;
                action.apiID = apiID;
                entities.base_moduleFormActions.Add(action);
            }
            bool isExists = entities.base_moduleFormAuthorize.Any(p => p.formID == model.rowID && p.authorizeValue == 0);
            if (isExists == false)
            {
                base_moduleFormAuthorize authorize = new base_moduleFormAuthorize();
                authorize.authorizeName = "访问";
                authorize.authorizeValue = 0;
                authorize.formID = model.rowID;
                authorize.rowID = Tools.GUID;
                entities.base_moduleFormAuthorize.Add(authorize);
            }
            try
            {
                entities.SaveChanges();
                sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_moduleFormActions);
            }
            catch (DbUpdateConcurrencyException e)
            {
                //这个是判断存在的一个方法  得你自己写 哦
                return GZAPIThrowException(e);
            }

            return GZAPISuccess();

        }

        /// <summary>
        /// 批量更新模块功能，排序
        /// </summary>
        [HttpPost]
        [Route("api/moduleForm/updatebatch")]
        public GZAPIResult FormUpdateBatch(List<base_moduleForm> models)
        {
            if (models == null || models.Count == 0)
            {
                return GZAPINULLRequest();
            }

            if (!ModelState.IsValid)
            {
                return GZAPIBadRequest(ModelState);
            }
            foreach (var v in models)
                entities.Entry(v).State = System.Data.EntityState.Modified;
            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //这个是判断存在的一个方法  得你自己写 哦
                return GZAPIThrowException(e);
            }

            return GZAPISuccess();

        }


        /// <summary>
        /// 功能删除
        /// </summary>
        /// <param name="rowID"></param>
        [HttpGet]
        [Route("api/moduleForm/delete")]
        public GZAPIResult FormDelete(string rowID)
        {
            //删除actions
            var lst = entities.base_moduleFormActions.Where(p => p.formID == rowID);
            foreach (var action in lst)
            {
                entities.Entry(action).State = System.Data.EntityState.Deleted;
            }

            base_moduleForm c = new base_moduleForm() { rowID = rowID };
            entities.Entry(c).State = System.Data.EntityState.Deleted;

            entities.SaveChanges();
            sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_moduleFormActions | sysAuthorizeBiz.CacheFlag.base_moduleFormAuthorize | sysAuthorizeBiz.CacheFlag.base_moduleFormAuthorizeAction);
            return GZAPISuccess();
        }
        /// <summary>
        /// 功能Actions
        /// </summary>
        /// <param name="rowID"></param>
        [HttpGet]
        [Route("api/moduleForm/actions")]
        public GZAPIResult FormActions(string rowID)
        {
            var lst = entities.base_moduleFormActions.Where(p => p.formID == rowID).Join(entities.base_APIList, fa => fa.apiID, api => api.rowID, (fa, api) => api).OrderBy(p => p.url).Select(p => p);

            return GZAPISuccess(lst);
        }
        /// <summary>
        /// 权限列表
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/moduleFormAuthorize/list")]
        public GZAPIResult AuthorizeList(string formID)
        {
            var data = entities.base_moduleFormAuthorize.Where(p => p.formID == formID).OrderBy(p => p.authorizeValue).Select(p => new
            {
                p.rowID,
                p.formID,
                p.authorizeName,
                p.authorizeValue,
                actions = p.base_moduleFormAuthorizeAction.Select(s => s.apiID),
                actionsUrl = p.base_moduleFormAuthorizeAction.Join(entities.base_APIList, authorize => authorize.apiID, api => api.rowID, (a, b) => b.url)
            });
            return GZAPISuccess(data);
        }
        /// <summary>
        /// 权限详情
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/moduleFormAuthorize/get")]
        public GZAPIResult AuthorizeGet(string rowID)
        {
            var data = entities.base_moduleFormAuthorize.Where(p => p.rowID == rowID).Select(p => new
            {
                p.rowID,
                p.formID,
                p.authorizeName,
                p.authorizeValue,
                actions = p.base_moduleFormAuthorizeAction.Select(s => s.apiID)
            }).FirstOrDefault();
            return GZAPISuccess(data);
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="rowID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/moduleFormAuthorize/delete")]
        public GZAPIResult AuthorizeDelete(string rowID)
        {
            var lst = entities.base_moduleFormAuthorizeAction.Where(p => p.authorizeID == rowID);
            foreach (var action in lst)
            {
                entities.Entry(action).State = System.Data.EntityState.Deleted;
            }
            base_moduleFormAuthorize authorize = new base_moduleFormAuthorize();
            authorize.rowID = rowID;
            entities.Entry(authorize).State = System.Data.EntityState.Deleted;
            entities.SaveChanges();
            refreshCacheAuthorize();
            return GZAPISuccess();
        }

        void refreshCacheAuthorize()
        {
            sysAuthorizeBiz.refreshCache(sysAuthorizeBiz.CacheFlag.base_moduleFormAuthorize | sysAuthorizeBiz.CacheFlag.base_moduleFormAuthorizeAction);
        }

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/moduleFormAuthorize/create")]
        public GZAPIResult AuthorizeCreate(base_moduleFormAuthorize model)
        {
            //修改 好像是直接赋值  就可以了 ，这个id不用？
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            try
            {
                model.rowID = Tools.GUID;
                entities.base_moduleFormAuthorize.Add(model);
                foreach (string apiID in model.actions)
                {
                    base_moduleFormAuthorizeAction action = new base_moduleFormAuthorizeAction();
                    action.rowID = Tools.GUID;
                    action.formID = model.formID;
                    action.authorizeID = model.rowID;
                    action.apiID = apiID;
                    entities.base_moduleFormAuthorizeAction.Add(action);
                }

                entities.SaveChanges();
                refreshCacheAuthorize();
                //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
                return GZAPISuccess(model);
            }
            catch (DbUpdateException)
            {
                return GZAPIBadRequest($"模块编码【{model.authorizeName}】已经存在，请修改");
            }
            //这个就是标准的 新增并返回 新增后的 实体
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/moduleFormAuthorize/update")]
        public GZAPIResult AuthorizeUpdate(base_moduleFormAuthorize model)
        {
            //修改 好像是直接赋值  就可以了 ，这个id不用？
            if (!ModelState.IsValid && model != null)
            {
                return GZAPIBadRequest(ModelState);
            }
            try
            {
                entities.Entry(model).State = System.Data.EntityState.Modified;

                var actions = entities.base_moduleFormAuthorizeAction.Where(p => p.authorizeID == model.rowID);
                foreach (var item in actions)
                {
                    entities.Entry(item).State = System.Data.EntityState.Deleted;
                }

                foreach (string apiID in model.actions)
                {
                    base_moduleFormAuthorizeAction action = new base_moduleFormAuthorizeAction();
                    action.rowID = Tools.GUID;
                    action.formID = model.formID;
                    action.authorizeID = model.rowID;
                    action.apiID = apiID;
                    entities.base_moduleFormAuthorizeAction.Add(action);
                }

                entities.SaveChanges();
                refreshCacheAuthorize();
                //return CreatedAtRoute("DefaultApi", new { id = model.isid }, model);
                return GZAPISuccess(model);
            }
            catch (DbUpdateException)
            {
                return GZAPIBadRequest($"模块编码【{model.authorizeName}】已经存在，请修改");
            }
            //这个就是标准的 新增并返回 新增后的 实体
        }

    }
}