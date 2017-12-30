using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadata_base
    {
        [Newtonsoft.Json.JsonIgnore]
        public int isid { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 1)]
        //[Required(ErrorMessage = "主键不能为空")]
        public string rowID { get; set; }
    }

}