using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Accounts;
using His_Pos.NewClass.Report.Accounts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.FromSourceWindow
{
    public class FromSourceViewModel : ViewModelBase
    {
        public FromSourceViewModel()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            GetData();
        }
        public List<JournalType> Types { get; set; }
        public JournalType Type { get; set; }
        public DataTable SoureTable { get; set; }
        public RelayCommand SubmitCommand { get; set; }

        private void SubmitAction()
        {
            Messenger.Default.Send(new NotificationMessage("YesAction"));
        }
        private void GetData()
        {
            JournalType journalType0 = new JournalType(0, "ALL", "1:全部");
            JournalType journalType1 = new JournalType(1, "傳票作業", "2:傳票作業");
            JournalType journalType2 = new JournalType(2, "關班轉入", "3:關班轉入");
            JournalType journalType3 = new JournalType(3, "進退貨轉入", "4:進退貨轉入");
            List<JournalType> types = new List<JournalType>() { journalType0, journalType1, journalType2, journalType3 };
            Types = types;
            Type = Types[0];
            SoureTable = AccountsDb.GetSourceData("");
        }
    }
}
