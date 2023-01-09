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
        }
        private bool isAllSelected;
        public bool IsAllSelected
        {
            get { return isAllSelected; }
            set { Set(() => IsAllSelected, ref isAllSelected, value); }
        }
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
                    item["IsChecked"] = IsAllSelected;
                }
            }
        }
        /// <summary>
        /// 明細判斷全選
        /// </summary>
        private void OnCheck()
        {
            DataTable table = SoureTable;
            if (SoureTable != null)
            {
                DataRow[] drs = table.Select("IsChecked = false");
                IsAllSelected = (drs != null && drs.Count() > 0) ? false : true;
            }
        }
    }
}
