using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class CreatePicCodeVM
    {

        [Required]
        public string PicDesc { get; set; }
        [Required]
        public string Remark { get; set; }
    }
}
