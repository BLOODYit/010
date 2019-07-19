using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Element
    {
        public Element()
        {
            this.Equations = new List<Equation>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string MeasureUnit { get; set; }

        [Required]
        public int DataSourceID { get; set; }
        public DataSource DataSource { get; set; }

        public List<Equation> Equations { get; set; }
    }
}