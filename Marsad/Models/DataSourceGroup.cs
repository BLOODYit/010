using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class DataSourceGroup
    {
        public DataSourceGroup()
        {
            this.DataSources = new List<DataSource>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}