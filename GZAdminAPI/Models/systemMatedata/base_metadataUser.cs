using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataUser : Base_metadata_base
    {

        [Newtonsoft.Json.JsonProperty(Order = 2)]
        [Required(ErrorMessage = "账号号不能为空")]
        public string account { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 3)]
        //[Required(ErrorMessage = "登录密码不能为空")]
        public string password { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 4)]
        [Required(ErrorMessage = "用户名不能为空")]
        public string userName { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 5)]
        public string sex { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 6)]
        public string companyID { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 7)]
        public string deptID { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 8)]
        public string phone { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 9)]
        public int status { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 10)]
        public string remark { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 11)]
        public string createUser { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 12)]
        public Nullable<System.DateTime> createTime { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 13)]
        public string updateUser { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 14)]
        public Nullable<System.DateTime> updateTime { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<base_userRelation> base_userRelation { get; set; }

    }
}