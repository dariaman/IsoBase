using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("ClientAccBank")]
    public class ClientAccBankModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ClientID { get; set; }
        [Required]
        public string BankCode { get; set; }
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string AccountNo { get; set; }
        public string Remark { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
