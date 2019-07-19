using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class ClientListVM
    {
        [Key]
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientTypeName { get; set; }
        public string IsActive { get; set; }
        public string UserCreate { get; set; }
        public string DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public string DateUpdate { get; set; }
    }
}
