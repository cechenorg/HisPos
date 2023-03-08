using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.InvalidWindow
{
    public class InvalidViewModel : ViewModelBase
    {
        public InvalidViewModel()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
        }
        public string VoidReason
        {
            get => voidReason;
            set
            {
                Set(() => VoidReason, ref voidReason, value);
            }
        }
        private string voidReason;
        
        public RelayCommand SubmitCommand { get; set; }

        private void SubmitAction()
        {
            Messenger.Default.Send(new NotificationMessage("YesAction"));
        }
    }
}
