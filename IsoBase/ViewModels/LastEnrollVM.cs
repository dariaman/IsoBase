using IsoBase.Models;
using System;

namespace IsoBase.ViewModels
{
    public class LastEnrollVM
    {
        public LastEnrollVM()
        {
            _enrollmentHdrVM = new EnrollmentHdrVM();
            _clientModel = new ClientModel();
        }
        public EnrollmentHdrVM _enrollmentHdrVM { get; set; }
        public ClientModel _clientModel { get; set; }
    }
}
