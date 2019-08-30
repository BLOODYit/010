﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Models.ViewModels
{
    public class UserViewModel
    {
        public RegisterViewModel RegisterVM{ get; set; }
        public string UserGroup { get; set; }
        [Display(Name="مستخدمى مجموعة")]
        public IEnumerable<SelectListItem> UserGroups { get; set; }
    }
}