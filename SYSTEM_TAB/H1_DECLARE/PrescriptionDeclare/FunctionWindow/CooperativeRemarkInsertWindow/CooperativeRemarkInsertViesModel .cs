using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow
{
    public class CooperativeRemarkInsertViesModel : ViewModelBase
    {
        private string remark;

        public string Remark
        {
            get => remark;
            set { Set(() => Remark, ref remark, value); }
        }

        public RelayCommand SubmitCommand { get; set; }

        public CooperativeRemarkInsertViesModel()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
        }

        private void SubmitAction()
        {
            if (Remark.Trim().Length != 16)
                MessageWindow.ShowMessage("單號須為16碼 請重新確認 ^皿^", NewClass.MessageType.ERROR);
            else
            {
                MessageWindow.ShowMessage("叮咚叮咚 輸入完成~", NewClass.MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CooperativeRemarkInsert"));
            }
        }
    }
}