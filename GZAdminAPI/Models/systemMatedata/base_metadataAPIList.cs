using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataAPIList : Base_metadata_base
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<base_moduleFormAuthorizeAction> base_moduleFormAuthorizeAction { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<base_moduleFormActions> base_moduleFormActions { get; set; }

    }
}