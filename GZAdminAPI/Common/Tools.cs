using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vueAdminAPI.Models;

namespace vueAdminAPI.Common
{
    public class Tools
    {
        public static long getTS()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static string GUID
        {
            get
            {
                return Guid.NewGuid().ToString().Replace("-", "");
            }
        }
        public static List<T> convert2Tree<T, K>(IEnumerable<K> lst, Func<K, T> func) where T : ITree, new()
        {
            List<T> array = new List<T>();
            Dictionary<string, T> cacheData = new Dictionary<string, T>();
            foreach (var item in lst)
            {
                var d = func(item);

                bool isChildren = d.parentID != null && cacheData.ContainsKey(d.parentID);
                if (isChildren)
                {
                    var parent = cacheData[d.parentID];
                    if (parent.children == null) parent.children = new List<ITree>();
                    parent.children.Add(d);
                }
                else
                {
                    cacheData.Add(d.id, d);
                    array.Add(d);
                }
            }
            return array;
        }
    }
}