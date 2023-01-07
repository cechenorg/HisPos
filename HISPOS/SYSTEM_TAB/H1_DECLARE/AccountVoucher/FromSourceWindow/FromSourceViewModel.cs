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
            CheckAllCommand = new RelayCommand(OnCheckAll);
            CheckCommand = new RelayCommand(OnCheck);
            SubmitCommand = new RelayCommand(SubmitAction);
            GetData();
        }
        public List<JournalType> Types { get; set; }
        public JournalType Type { get; set; }
        private DataTable soureTable;
        public DataTable SoureTable
        {
            get { return soureTable; }
            set { Set(() => SoureTable, ref soureTable, value); }
        }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand CheckAllCommand { get; set; }
        public RelayCommand CheckCommand { get; set; }
        private void SubmitAction()
        {
            Messenger.Default.Send(new NotificationMessage("YesAction"));
        }
        /// <summary>
        /// 全選
        /// </summary>
        private void OnCheckAll()
        {
            DataTable table = SoureTable;
            if (SoureTable != null)
            {
                foreach (DataRow item in table.Rows)
                {
                    item["IsCheck"] = true;
                }
            }
        }
        /// <summary>
        /// 明細判斷全選
        /// </summary>
        private void OnCheck()
        {

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
        }
    }
}
