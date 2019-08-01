using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class UploadFilePlanVM
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public IFormFile Fileupload { get; set; }
    }
}
