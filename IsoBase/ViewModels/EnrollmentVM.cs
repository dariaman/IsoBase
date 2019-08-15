using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class EnrollmentVM
    {
        [Key]
        public int ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientTypeName { get; set; }
        public string Building { get; set; }
        public string Address { get; set; }

    }
}
