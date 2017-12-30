using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vueAdminAPI.Models
{
    public interface ITree
    {
        string id { get; set; }
        string parentID { get; set; }
        List<ITree> children { get; set; }
    }
}