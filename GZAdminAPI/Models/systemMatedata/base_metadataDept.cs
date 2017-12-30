using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataDept : Base_metadata_base
    {
        [Newtonsoft.Json.JsonProperty(Order = 2)]
        [Required(ErrorMessage = "部门编号不能为空")]
        public string deptCode { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 3)]
        [Required(ErrorMessage = "部门名称不能为空")]
        public string deptName { get; set; }

        public string dtptShortName { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 3)]
        [Required(ErrorMessage = "所属公司不能为空")]
        public string companyID { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 4)]
        public string parentID { get; set; }
        [Newtonsoft.Json.JsonProperty(Order = 5)]
        public string parentFullID { get; set; }
        [Newtonsoft.Json.JsonProperty(Order = 6)]
        public string parentFullName { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 7)]
        public string principalName { get; set; }
        [Newtonsoft.Json.JsonProperty(Order = 8)]
        public string principalPhone { get; set; }
        [Newtonsoft.Json.JsonProperty(Order = 9)]
        public string principalEmail { get; set; }
        [Newtonsoft.Json.JsonProperty(Order = 10)]
        public string remark { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 11)]
        public int levelID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual base_company base_company { get; set; }
        
    }
}