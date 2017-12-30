using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using vueAdminAPI.Models;

namespace vueAdminAPI.Common
{
    public interface IGZAPIDBContext : IDisposable
    {
        UserInfo _userInfo { get; set; }
    }
    public class DbContextSystem : vueAdminSystem, IGZAPIDBContext
    {
        public UserInfo _userInfo { get; set; }

        public DbContextSystem()
        {

        }
        public override int SaveChanges()
        {
            var entries = from e in this.ChangeTracker.Entries()
                          where e.State != EntityState.Unchanged
                          select e;   //过滤所有修改了的实体，包括：增加 / 修改 / 删除
            foreach (var entry in entries)
            {
                updateCommonValue(entry);
            }
            return base.SaveChanges();  //返回普通的上下文SaveChanges方法

        }

        void updateCommonValue(DbEntityEntry entry)
        {
            var state = entry.State;
            var AddOrModified = EntityState.Added | EntityState.Modified;
            if ((state & AddOrModified) == state)
            {
                if (entry.CurrentValues.PropertyNames.Contains("updateTime"))
                    entry.CurrentValues["updateTime"] = DateTime.Now;
                if (_userInfo != null && entry.CurrentValues.PropertyNames.Contains("updateUser"))
                    entry.CurrentValues["updateUser"] = _userInfo.UserID;
            }
            if (state == EntityState.Added)
            {
                if (entry.CurrentValues.PropertyNames.Contains("createTime"))
                    entry.CurrentValues["createTime"] = DateTime.Now;
                if (_userInfo != null && entry.CurrentValues.PropertyNames.Contains("createUser"))
                    entry.CurrentValues["createUser"] = _userInfo.UserID;
            }
        }
    }
    


}