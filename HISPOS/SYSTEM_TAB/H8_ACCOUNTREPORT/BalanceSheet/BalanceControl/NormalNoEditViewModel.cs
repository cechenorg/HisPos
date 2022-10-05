using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class NormalNoEditViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand InsertCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string transferValue;
        private string target;
        public string IDClone;
        public DateTime EndDate;
        public double MaxValue { get; set; } = 0;

        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                RaisePropertyChanged(nameof(Target));
            }
        }

        public string TransferValue
        {
            get { return transferValue; }
            set
            {
                transferValue = value;
                RaisePropertyChanged(nameof(TransferValue));
            }
        }

        private AccountsReport accData;

        public AccountsReport AccDataNoEdit
        {
            get => accData;
            set
            {
                Set(() => AccDataNoEdit, ref accData, value);
            }
        }

        private AccountsReports selected;

        public AccountsReports Selected
        {
            get => selected;
            set
            {
                Set(() => Selected, ref selected, value);
            }
        }

        #endregion ----- Define Variables -----

        public NormalNoEditViewModel(string ID,DateTime endDate)
        {
            AccDataNoEdit = new AccountsReport();
            IDClone = ID;
            EndDate = endDate;
            Init();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
        }

        public NormalNoEditViewModel()
        {
            AccDataNoEdit = new AccountsReport();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
        }

        public void Init()
        {
            AccDataNoEdit = new AccountsReport();
            DataTable Data = AccountsDb.GetAccountsDetail(IDClone, EndDate);
            foreach (DataRow r in Data.Rows)
            {
                AccDataNoEdit.Add(new AccountsReports(r));
            }
        }

        public void DeleteAction()
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("錯誤", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", Selected.ID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[AccountsDetailDelete]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
            Selected = null;
            Init();
        }

        public void InsertAction()
        {
            if (TransferValue == "" || TransferValue == null)
            {
                MessageWindow.ShowMessage("請輸入名稱", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            parameters.Add(new SqlParameter("NAME", TransferValue));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccounts]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("新增成功", MessageType.SUCCESS);
            TransferValue = "";
            Init();
        }
    }
}