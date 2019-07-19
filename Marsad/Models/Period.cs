using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Period
    {

        public Period()
        {
            this.DataSources = new List<DataSource>();
        }

        [Key]
        public int ID { get; set; }
       
        [Required]
        [MaxLength(255)]        
        public string Name { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Day { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}