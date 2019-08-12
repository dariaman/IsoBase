using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class ClientCreateVM
    {
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
    }
}
