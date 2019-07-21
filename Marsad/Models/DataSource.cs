using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class DataSource
    {
        public DataSource()
        {
            this.DataSourceGroups = new List<DataSourceGroup>();
            this.Elements = new List<Element>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        public string PublishNumber { get; set; }

        public string PublisherName { get; set; }

        public string AuthorName { get; set; }

        public string OtherDataSourceType { get; set; }

        [Required]
        public bool IsPeriodic { get; set; }

        public int? PeriodID { get; set; }
        public Period Period { get; set; }

        public int? DataSourceID { get; set; }
        public DataSource ParentDataSource { get; set; }

        [Required]
        public bool IsPart { get; set; }

        [Required]
        public bool IsHijri { get; set; }

        [Required]
        public int DataSourceTypeID { get; set; }
        public DataSourceType DataSourceType { get; set; }

        [NotMapped]
        public string PeriodicString
        {
            get
            {
                if (!IsPeriodic)                
                    return "غير دوري";
                if (Period != null)
                    return Period.Name;
                return "";
            }
        }
        public List<DataSourceGroup> DataSourceGroups { get; set; }
        public List<Element> Elements { get; set; }
    }
}