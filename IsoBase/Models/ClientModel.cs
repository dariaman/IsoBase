using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("Client")]
    public class ClientModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ClientCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ClientTypeID { get; set; }

        public string Building { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
