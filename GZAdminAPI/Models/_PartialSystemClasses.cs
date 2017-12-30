using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vueAdminAPI.Models
{
    [MetadataType(typeof(Base_metadataModule))]
    public partial class base_module
    {
    }
    [MetadataType(typeof(Base_metadataModuleForm))]
    public partial class base_moduleForm
    {
        public List<string> actions { get; set; }
    }
    [MetadataType(typeof(Base_metadataModuleFormActions))]
    public partial class base_moduleFormActions
    {
    }
    public partial class base_moduleFormAuthorize
    {
        public List<string> actions { get; set; }
    }
    [MetadataType(typeof(Base_metadataCompany))]
    public partial class base_company
    {
        public int deep { get; set; }
        public ICollection<base_company> children { get; set; }
    }
    [MetadataType(typeof(Base_metadataDept))]
    public partial class base_dept
    {
        public base_dept initCompany(base_company company)
        {
            this.companyName = company.companyName_chs;
            return this;
        }
        public int deep { get; set; }
        public string companyName { get; set; }
        public ICollection<base_dept> children { get; set; }
    }

    [MetadataType(typeof(Base_metadataPosition))]
    public partial class base_position
    {
        public ICollection<base_position> children { get; set; }
    }

    [MetadataType(typeof(Base_metadataUser))]
    public partial class base_user
    {
        public string companyName { get; set; }
        public string deptName { get; set; }
    }
    [MetadataType(typeof(Base_metadataAPIList))]
    public partial class base_APIList
    {
        public string checkTypeName { get; set; }
    }
    [MetadataType(typeof(Base_metadataUserRelation))]
    public partial class base_userRelation
    {

    }
}