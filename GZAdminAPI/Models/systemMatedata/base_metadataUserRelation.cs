using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace vueAdminAPI.Models
{
    public class Base_metadataUserRelation : Base_metadata_base
    {

        [Newtonsoft.Json.JsonIgnore]
        public virtual base_user base_user { get; set; }


    }
}