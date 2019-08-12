using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("PicCode")]
    public class PicCodeModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string PicDesc { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateCreate { get; set; }
        public string UserCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string UserUpdate { get; set; }
    }
}
