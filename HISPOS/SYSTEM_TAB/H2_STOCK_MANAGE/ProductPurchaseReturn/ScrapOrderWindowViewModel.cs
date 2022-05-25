using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ScrapOrderWindowViewModel: ViewModelBase
    {
        private string other;
        public string Other {
            get => other;
            set { Set(() => Other, ref other, value); }
        }
        private bool isWrite;
        public bool IsWrite
        {
            get => isWrite;
            set { Set(() => IsWrite, ref isWrite, value); }
        }
        public RelayCommand YesCommand { get; set; }
        public RelayCommand NoCommand { get; set; }
        public ScrapOrderWindowViewModel()
        {
            YesCommand = new RelayCommand(YesAction);
            NoCommand = new RelayCommand(NoAction);
        }
        private void YesAction()
        {
            Messenger.Default.Send(new NotificationMessage("YesAction"));
        }

        private void NoAction()
        {
            Messenger.Default.Send(new NotificationMessage("NoAction"));
        }
    }
}
