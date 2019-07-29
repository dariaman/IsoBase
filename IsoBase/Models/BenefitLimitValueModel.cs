using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("BenefitLimitValue")]
    public class BenefitLimitValueModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int BenefitID { get; set; }
        public int FrequencyCodeID { get; set; }
        public string LimitCodeID { get; set; }
        public decimal MaxLimitValue { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
