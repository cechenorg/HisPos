using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace His_Pos.FunctionWindow {
    public class ConfirmWindowViewModel : ViewModelBase {
        #region ----- Define Command -----
        public RelayCommand YesCommand { get; set; }
        public RelayCommand NoCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public string Title { get; set; }
        public string Message { get; set; }
        #endregion
        
        public ConfirmWindowViewModel(string message, string title) {
            Message = message;
            Title = title;
            YesCommand = new RelayCommand(YesAction);
            NoCommand = new RelayCommand(NoAction);
        }
        #region ----- Define Actions -----
        private void YesAction() { 
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("YesAction")); 
        }
        private void NoAction() {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("NoAction"));
        }
        #endregion

    }
}
