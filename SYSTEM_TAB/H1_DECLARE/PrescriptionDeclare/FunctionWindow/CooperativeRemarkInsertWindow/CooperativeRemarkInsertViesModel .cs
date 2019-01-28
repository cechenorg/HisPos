using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow {
    public class CooperativeRemarkInsertViesModel :ViewModelBase{
        private string remark;
        public string Remark {
            get { return remark; }
            set { Set(() => Remark, ref remark, value);  }
        }
        public RelayCommand SubmitCommand { get; set; }
        public CooperativeRemarkInsertViesModel() {
            SubmitCommand = new RelayCommand(SubmitAction);
        }
        public void SubmitAction() {
            if (Remark.Trim().Length != 16)
                MessageWindow.ShowMessage("單號須為16碼 請重新確認 ^皿^", Class.MessageType.ERROR);
            else {
                MessageWindow.ShowMessage("叮咚叮咚 輸入完成~", Class.MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CooperativeRemarkInsert"));
            }

        }
    }
}
