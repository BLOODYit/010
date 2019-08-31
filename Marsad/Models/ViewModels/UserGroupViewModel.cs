using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.ViewModels
{
    public class UserGroupViewModel
    {
        public List<string> SelectedClaims { get; set; }
        public UserGroup UserGroup { get; set; }
    }
}