using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataModule
    {
        [Newtonsoft.Json.JsonIgnore]
        public int isid { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 1)]
        public string rowID { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 2)]
        [Required(ErrorMessage = "编码不能为空")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 3)]
        [Required(ErrorMessage = "描述不能为空")]
        public string description { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 4)]
        public string icon { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 5)]
        [Required(ErrorMessage = "组件路径不能为空")]
        public string componentPath { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 6)]

        [Required(ErrorMessage = "组件名称不能为空")]
        public string componentName { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 7)]
        public int sort { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 8)]

        public virtual ICollection<base_moduleForm> base_moduleForm { get; set; }
    }
}