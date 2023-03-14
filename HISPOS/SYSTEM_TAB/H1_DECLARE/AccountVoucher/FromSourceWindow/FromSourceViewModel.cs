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
            FilterCommand = new RelayCommand(FilterAction);
            IsSubmit = false;
            FilterAction();
        }
        private bool isSubmit;
        public bool IsSubmit
        {
            get { return isSubmit; }
            set { Set(() => IsSubmit, ref isSubmit, value); }
        }
        private bool isAllSelected;
        public bool IsAllSelected
        {
            get { return isAllSelected; }
            set { Set(() => IsAllSelected, ref isAllSelected, value); }
        }

        private DateTime? beginDate;
        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { Set(() => BeginDate, ref beginDate, value); }
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { Set(() => EndDate, ref endDate, value); }
        }
        private int selectCount;
        public int SelectCount
        {
            get { return selectCount; }
            set { Set(() => SelectCount, ref selectCount, value); }
        }
        private int unSelectCount;
        public int UnSelectCount
        {
            get { return unSelectCount; }
            set { Set(() => UnSelectCount, ref unSelectCount, value); }
        }
        private int unCheckCash;
        public int UnCheckCash
        {
            get { return unCheckCash; }
            set { Set(() => UnCheckCash, ref unCheckCash, value); }
        }

        private int checkCash;
        public int CheckCash
        {
            get { return checkCash; }
            set { Set(() => CheckCash, ref checkCash, value); }
        }

        private DataTable soureTable;
        public DataTable SoureTable
        {
            get { return soureTable; }
            set { Set(() => SoureTable, ref soureTable, value); }
        }
        private DataTable selectTable;
        public DataTable SelectTable
        {
            get { return selectTable; }
            set { Set(() => SelectTable, ref selectTable, value); }
        }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        public RelayCommand CheckAllCommand { get; set; }
        public RelayCommand CheckCommand { get; set; }
        private void SubmitAction()
        {
            IsSubmit = true;
            Messenger.Default.Send(new NotificationMessage("YesAction"));
        }
        /// <summary>
        /// 全選
        /// </summary>
        private void OnCheckAll()
        {
            DataTable table = SelectTable;
            if (SelectTable != null)
            {
                foreach (DataRow item in table.Rows)
                {
                    item["IsChecked"] = IsAllSelected;
                }
                SetTotalCash();
            }
        }
        /// <summary>
        /// 明細判斷全選
        /// </summary>
        private void OnCheck()
        {
            DataTable table = SelectTable;
            if (SelectTable != null)
            {
                DataRow[] drs = table.Select("IsChecked = false");
                IsAllSelected = (drs != null && drs.Count() > 0) ? false : true;
            }
            SetTotalCash();
        }
        private void SetTotalCash()
        {
            if (SelectTable != null)
            {
                SelectCount = Convert.ToInt32(SelectTable.Compute("Count(JouDet_Amount)", "IsChecked = true"));
                UnSelectCount = Convert.ToInt32(SelectTable.Compute("Count(JouDet_Amount)", "IsChecked = false"));
                CheckCash = Convert.ToInt32(SelectCount > 0 ? SelectTable.Compute("Sum(JouDet_Amount)", "IsChecked = true") : 0);
                UnCheckCash = Convert.ToInt32(UnSelectCount > 0 ? SelectTable.Compute("Sum(JouDet_Amount)", "IsChecked = false") : 0);
            }
        }
        public void FilterAction()
        {
            if (SoureTable != null && SoureTable.Rows.Count > 0)
            {
                SelectTable = new DataTable();
                SelectTable = SoureTable.Clone();

                foreach (DataRow dr in SoureTable.Rows)
                {
                    DateTime date = Convert.ToDateTime(dr["JouMas_Date"]);
                    if (beginDate == null || endDate == null)
                    {
                        if (beginDate == null && endDate != null)
                        {
                            SelectTable.ImportRow(dr);
                            continue;
                        }
                        else if (beginDate != null && endDate == null)
                        {
                            if (DateTime.Compare((DateTime)beginDate, date) <= 0)
                            {
                                SelectTable.ImportRow(dr);
                                continue;
                            }
                        }
                        else
                        {
                            SelectTable.ImportRow(dr);
                            continue;
                        }
                    }

                    if (DateTime.Compare((DateTime)beginDate, date) <= 0 && DateTime.Compare((DateTime)endDate, date) >= 0)
                    {
                        SelectTable.ImportRow(dr);
                    }
                }
            }
            SetTotalCash();
        }
    }
}
