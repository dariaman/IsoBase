using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("ClientType")]
    public class ClientTypeModel
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
