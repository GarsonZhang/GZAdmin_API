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
    
    public partial class base_moduleFormActions
    {
        public int isid { get; set; }
        public string rowID { get; set; }
        public string formID { get; set; }
        public string apiID { get; set; }
    
        public virtual base_APIList base_APIList { get; set; }
        public virtual base_moduleForm base_moduleForm { get; set; }
    }
}