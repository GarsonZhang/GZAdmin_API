using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vueAdminAPI.Models
{
    public class Base_metadataModuleFormActions
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual base_APIList base_APIList { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual base_moduleForm base_moduleForm { get; set; }
    }
}