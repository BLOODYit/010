using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Entity
    {
        public Entity()
        {
            this.DataSources = new List<DataSource>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "يجب إدخال الجهة")]
        [MaxLength(255, ErrorMessage = "إسم الجهة يجب الا يتعدى 255 حرف")]
        [Display(Name = "الجهة")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}