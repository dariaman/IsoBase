using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("ClientPic")]
    public class ClientPicModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ClientID { get; set; }
        [Required]
        public int PicCodeID { get; set; }
        [Required]
        public string PicName { get; set; }
        [Required]
        public string PicTitle { get; set; }
        public string AddressBuilding { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string HP { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
