using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vueAdminAPI.Models;

namespace vueAdminAPI.IviewModel
{
    public class TreeDataModel : ITree
    {
        public string title { get; set; }
        public string id { get; set; }
        public string parentID { get; set; }
        public List<ITree> children { get; set; }
    }
}