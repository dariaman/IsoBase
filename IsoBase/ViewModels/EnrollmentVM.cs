using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.ViewModels
{
    public class EnrollmentVM
    {
        [Key]
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
    }
}
