﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace vueAdminAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class vueAdminSystem : DbContext
    {
        public vueAdminSystem()
            : base("name=vueAdminSystem")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<base_module> base_module { get; set; }
        public DbSet<base_moduleForm> base_moduleForm { get; set; }
        public DbSet<base_company> base_company { get; set; }
        public DbSet<base_dept> base_dept { get; set; }
        public DbSet<base_position> base_position { get; set; }
        public DbSet<base_user> base_user { get; set; }
        public DbSet<base_token> base_token { get; set; }
        public DbSet<base_Authorize> base_Authorize { get; set; }
        public DbSet<base_APIList> base_APIList { get; set; }
        public DbSet<base_moduleFormAuthorize> base_moduleFormAuthorize { get; set; }
        public DbSet<base_moduleFormAuthorizeAction> base_moduleFormAuthorizeAction { get; set; }
        public DbSet<base_moduleFormActions> base_moduleFormActions { get; set; }
        public DbSet<base_userRelation> base_userRelation { get; set; }
    }
}