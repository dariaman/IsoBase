using IsoBase.Models;
using System;

namespace IsoBase.ViewModels
{
    public class LastEnrollVM
    {
        public LastEnrollVM()
        {
            EnrollmentHdrModel = new EnrollmentHdrModel();
            _clientModel = new ClientModel();
        }
        public EnrollmentHdrModel EnrollmentHdrModel { get; set; }
        public ClientModel _clientModel { get; set; }
    }
}
