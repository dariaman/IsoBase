using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class UploadFilePlanVM
    {
        [Required]
        public int ClientID { get; set; } // ClientID
        [Required]
        public IFormFile Fileupload { get; set; }
    }
}
