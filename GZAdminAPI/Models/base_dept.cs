//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class base_dept
    {
        public int isid { get; set; }
        public string rowID { get; set; }
        public string deptCode { get; set; }
        public string deptName { get; set; }
        public string dtptShortName { get; set; }
        public string companyID { get; set; }
        public string principalName { get; set; }
        public string principalPhone { get; set; }
        public string principalEmail { get; set; }
        public string remark { get; set; }
        public string parentID { get; set; }
        public string parentFullID { get; set; }
        public string parentFullName { get; set; }
        public int levelID { get; set; }
    
        public virtual base_company base_company { get; set; }
    }
}