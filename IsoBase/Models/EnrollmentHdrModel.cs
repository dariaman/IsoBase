using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("EnrollmentHdr")]
    public class EnrollmentHdrModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ClientID { get; set; }
        [Required]
        public string FileUploadName { get; set; }
        public bool Status { get; set; }
        public int EnrollType { get; set; }
        public int CountPlan { get; set; }
        public int CountCoverage { get; set; }
        public int CountBenefit { get; set; }
        public int CountMember { get; set; }
        public DateTime EnrollDate { get; set; }
        public string EnrollByUser { get; set; }

    }
}
