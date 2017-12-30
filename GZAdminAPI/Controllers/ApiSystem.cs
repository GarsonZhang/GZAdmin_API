using System;
using vueAdminAPI.Common;

namespace vueAdminAPI.Controllers
{
    public class ApiSystem : ApiDbContext<DbContextSystem>
    {
        public ApiSystem()
        {
        }
    }
}