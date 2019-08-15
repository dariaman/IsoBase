using System;
using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class ClientListVM
    {
        [Key]
        public int ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientTypeName { get; set; }

        public string Building { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public bool IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime? DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
