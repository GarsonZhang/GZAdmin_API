using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataCompany
    {
        [Newtonsoft.Json.JsonIgnore]
        public int isid { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 1)]
        public string rowID { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 2)]
        [Required(ErrorMessage = "公司编码不能为空")]
        public string companyCode { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 2)]
        [Required(ErrorMessage = "公司中文名不能为空")]
        public string companyName_chs { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 3)]
        public string companyName_en { get; set; }

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
        public string address { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 11)]
        [DefaultValue(0)]
        public int levelID { get; set; }
    }
}