using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Helpers
{
    public class RoleHelper
    {
        public static string GetRoleName(string roleName)
        {
            if (roleName.Equals("Admin"))
                return "مدير نظام";
            else if (roleName.Equals("Officer"))
                return "ضابط إتصال";
            else if (roleName.Equals("Visitor"))
                return "زائر";
            return roleName;
        }
    }
}