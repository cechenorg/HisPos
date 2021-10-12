using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class StrikeHistoryViewModel : ViewModelBase
    {
        private DateTime startDate = DateTime.Today;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime endDate = DateTime.Today;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private string type = "";

        public string Type
        {
            get => type;
            set
            {
                Set(() => Type, ref type, value);
            }
        }

        private string sujectString = "";

        public string SujectString
        {
            get => sujectString;
            set
            {
                Set(() => SujectString, ref sujectString, value);
            }
        }

        private string accountString = "";

        public string AccountString
        {
            get => accountString;
            set
            {
                Set(() => AccountString, ref accountString, value);
            }
        }

        private string emp = "";

        public string Emp
        {
            get => emp;
            set
            {
                Set(() => Emp, ref emp, value);
            }
        }

        private DataTable typeTable;

        public DataTable TypeTable
        {
            get => typeTable;
            set
            {
                Set(() => TypeTable, ref typeTable, value);
            }
        }

        private DataTable sujectTable;

        public DataTable SujectTable
        {
            get => sujectTable;
            set
            {
                Set(() => SujectTable, ref sujectTable, value);
            }
        }

        private DataTable accountTable;

        public DataTable AccountTable
        {
            get => accountTable;
            set
            {
                Set(() => AccountTable, ref accountTable, value);
            }
        }

        private DataTable empTable;

        public DataTable EmpTable
        {
            get => empTable;
            set
            {
                Set(() => EmpTable, ref empTable, value);
            }
        }

        private StrikeHistory selectedHistory;

        public StrikeHistory SelectedHistory
        {
            get => selectedHistory;
            set
            {
                if (selectedHistory != null)
                    selectedHistory.IsSelected = false;

                selectedHistory = value;
                RaisePropertyChanged(nameof(SelectedHistory));

                if (selectedHistory != null)
                    selectedHistory.IsSelected = true;
            }
        }

        private StrikeHistories strikeHistories;

        public StrikeHistories StrikeHistories
        {
            get => strikeHistories;
            set
            {
                strikeHistories = value;
                RaisePropertyChanged(nameof(StrikeHistories));
            }
        }

        public RelayCommand DeleteStrikeHistory { get; set; }
        public RelayCommand SearchStrikeHistory { get; set; }

        public StrikeHistoryViewModel()
        {
            DeleteStrikeHistory = new RelayCommand(DeleteStrikeHistoryAction);
            SearchStrikeHistory = new RelayCommand(SearchStrikeHistoryAction);
            StrikeHistories = new StrikeHistories();
            Init();
        }

        private void Init()
        {
            GetData(true);
        }

        private void SearchStrikeHistoryAction()
        {
            GetData(false);
        }

        private void DeleteStrikeHistoryAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行刪除", "確認");
            if (!(bool)cw.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            CashReportDb.DeleteStrikeHistory(SelectedHistory);
            Init();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void GetData(bool isInit)
        {
            StrikeHistories = new StrikeHistories();
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("type", Type));
            parameters.Add(new SqlParameter("sdate", StartDate));
            parameters.Add(new SqlParameter("edate", EndDate));
            parameters.Add(new SqlParameter("SujectString", SujectString));
            parameters.Add(new SqlParameter("AccountString", AccountString));
            parameters.Add(new SqlParameter("Category", DBNull.Value));
            parameters.Add(new SqlParameter("Emp", Emp));
            DataSet result = MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[StrikeHistoriesByCondition]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Tables.Count == 5)
            {
                StrikeHistories.GetSelectData(result.Tables[0]);

                if (isInit)
                {
                    DataRow dr1 = result.Tables[1].NewRow();
                    dr1[0] = "";
                    result.Tables[1].Rows.InsertAt(dr1, 0);
                    TypeTable = result.Tables[1];

                    DataRow dr2 = result.Tables[2].NewRow();
                    dr2[0] = "";
                    result.Tables[2].Rows.InsertAt(dr2, 0);
                    SujectTable = result.Tables[2];

                    DataRow dr3 = result.Tables[3].NewRow();
                    dr3[0] = "";
                    result.Tables[3].Rows.InsertAt(dr3, 0);
                    AccountTable = result.Tables[3];

                    DataRow dr4 = result.Tables[4].NewRow();
                    dr4[0] = "";
                    result.Tables[4].Rows.InsertAt(dr4, 0);
                    EmpTable = result.Tables[4];
                }
            }
        }
    }
}