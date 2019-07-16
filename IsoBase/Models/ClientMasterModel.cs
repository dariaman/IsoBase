using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("ClientMaster")]
    public class ClientMasterModel
    {
        [Key]
        public int ClientID { get; set; }
        [Required]
        public string ClientCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ClientTypeID { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
