using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.Models
{
    [Table("Benefit")]
    public class BenefitModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string BenefitCode { get; set; }
        public int CoverageLimitID { get; set; }
        public string ClientBenefitCode { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
