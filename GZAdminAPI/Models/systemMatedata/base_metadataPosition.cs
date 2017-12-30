using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataPosition : Base_metadata_base
    {
        [Newtonsoft.Json.JsonProperty(Order = 2)]
        [Required(ErrorMessage = "公司ID不能为空")]
        public string companyID { get; set; }
        

        [Newtonsoft.Json.JsonProperty(Order = 4)]
        [Required(ErrorMessage = "岗位编号不能为空")]
        public string posCode { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 5)]
        [Required(ErrorMessage = "岗位名称不能为空")]
        public string posName { get; set; }

        public string parentID { get; set; }
        public string parentFullID { get; set; }
        public string parentFullName { get; set; }

        public int levelID { get; set; }

        public string remark { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual base_company base_company { get; set; }
    }
}