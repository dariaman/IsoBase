using IsoBase.Models;
using System.Collections.Generic;

namespace IsoBase.ViewModels
{
    public class ClientDetailVM
    {
        public ClientModel ClientData { get; set; }
        public List<ClientPicModel> PicList { get; set; }
        public List<ClientAccBankModel> AccBankList { get; set; }
    }
}
