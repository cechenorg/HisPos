using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement
{
    internal class NewIncomeStatementViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private BackgroundWorker bgWorker = new BackgroundWorker();
        
        private int _inputYear = DateTime.Today.Year;

        public int InputYear
        {
            get { return _inputYear; }
            set { Set(() => InputYear, ref _inputYear, value); }
        }

        private string _busyContent;

        public string BusyContent
        {
            get { return _busyContent; }
            set { Set(() => BusyContent, ref _busyContent, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(() => IsBusy, ref _isBusy, value); }
        }

        private DataView _countDataView;

        public DataView CountDataView
        {
            get { return _countDataView; }
            set { Set(() => CountDataView, ref _countDataView, value); }
        }

        private DataView _incomeDataView;

        public DataView IncomeDataView
        {
            get { return _incomeDataView; }
            set { Set(() => IncomeDataView, ref _incomeDataView, value); }
        }

        private DataView _expanseDataView;

        public DataView ExpanseDataView
        {
            get { return _expanseDataView; }
            set { Set(() => ExpanseDataView, ref _expanseDataView, value); }
        }

        private DataView _totalDataView;

        public DataView TotalDataView
        {
            get { return _totalDataView; }
            set { Set(() => TotalDataView, ref _totalDataView, value); }
        }


        private DataView _closedDataView;

        public DataView ClosedDataView
        {
            get { return _closedDataView; }
            set { Set(() => ClosedDataView, ref _closedDataView, value); }
        }
        public RelayCommand btn_LeftCommand { get; set; }
        public RelayCommand btn_RightCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }

        public RelayCommand ExportCsvCommand { get; set; }

        public NewIncomeStatementViewModel()
        {
            InitBackgroundWorker();
            SearchData();
            SearchCommand = new RelayCommand(SearchAction);
            ExportCsvCommand = new RelayCommand(ExportCsvAction);
            btn_LeftCommand = new RelayCommand(btnLeftClick);
            btn_RightCommand = new RelayCommand(btnRightClick);
        }
        private void btnLeftClick()
        {
            InputYear -= 1;
            GetData(InputYear);
        }
        private void btnRightClick()
        {
            InputYear += 1;
            GetData(InputYear);
        }
        private void ExportCsvAction()
        {
            System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog();
            diag.FileName = InputYear + "損益表.csv";
            diag.Filter = "csv (*.csv)|*.csv";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string filePath = diag.FileName;

                string result = "";
                result += GetDataViewData(TotalDataView);
                result += GetDataViewData(CountDataView);
                result += GetDataViewData(IncomeDataView);
                result += GetDataViewData(ExpanseDataView);
                result += GetDataViewData(ClosedDataView);

                File.WriteAllText(filePath, result, Encoding.Unicode);
            }
        }

        private void SearchAction()
        {
            SearchData();
        }

        private void InitBackgroundWorker()
        {
            bgWorker.DoWork += (sender, args) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    BusyContent = "取得損益資料...";
                });
                GetData(InputYear);
            };

            bgWorker.RunWorkerCompleted += (sender, args) =>
            {
                 IsBusy = false;
            };
        }

        private void GetData(int year)
        {
            MainWindow.ServerConnection.OpenConnection();

            List<SqlParameter> paraCount = new List<SqlParameter>();
            paraCount.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraIncome = new List<SqlParameter>();
            paraIncome.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraExpanse = new List<SqlParameter>();
            paraExpanse.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraClosed = new List<SqlParameter>();
            paraClosed.Add(new SqlParameter("YEAR", year));

            DataTable count = MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCountByYear]", paraCount);
            DataTable income = MainWindow.ServerConnection.ExecuteProc("[Get].[IncomeByYear]", paraIncome);
            DataTable expanse = MainWindow.ServerConnection.ExecuteProc("[Get].[ExpanseByYear]", paraExpanse);
            DataTable closed = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsClosedByYear]", paraClosed);

            foreach (DataRow dr in closed.Rows)
            {
                bool isZero = true;
                foreach (DataColumn dc in closed.Columns)
                {
                    bool isInt = int.TryParse(dr[dc].ToString(), out int value);
                    if (isInt && value != 0)
                    {
                        isZero = false;
                    }
                }
                if (isZero)
                {
                    dr.Delete();
                }
            }
            closed.AcceptChanges();

            Dispatcher.CurrentDispatcher.Invoke( () =>
            {  
                CountDataView = count.DefaultView;
                IncomeDataView = income.DefaultView;
                ExpanseDataView = expanse.DefaultView;
                ClosedDataView = closed.DefaultView;
            });

            MainWindow.ServerConnection.CloseConnection();

            DataTable total = income.Clone();

            if ((income.Rows.Count > 0) && (expanse.Rows.Count > 0) && (closed.Rows.Count > 0))
            {
                total.Rows.Add(income.Rows[income.Rows.Count - 1].ItemArray);
                total.Rows[0][0] = "營業毛利";
                total.Rows.Add(expanse.Rows[expanse.Rows.Count - 1].ItemArray);
                total.Rows[1][0] = "費用";
                total.Rows.Add(closed.Rows[closed.Rows.Count - 1].ItemArray);
                total.Rows[2][0] = "結案差額";

                DataRow totalRow = total.NewRow();
                totalRow["MONTH"] = "總計";
                totalRow["JAN"] = total.Compute("Sum(JAN)", string.Empty);
                totalRow["FEB"] = total.Compute("Sum(FEB)", string.Empty);
                totalRow["MAR"] = total.Compute("Sum(MAR)", string.Empty);
                totalRow["APR"] = total.Compute("Sum(APR)", string.Empty);
                totalRow["MAY"] = total.Compute("Sum(MAY)", string.Empty);
                totalRow["JUN"] = total.Compute("Sum(JUN)", string.Empty);
                totalRow["JUL"] = total.Compute("Sum(JUL)", string.Empty);
                totalRow["AUG"] = total.Compute("Sum(AUG)", string.Empty);
                totalRow["SEP"] = total.Compute("Sum(SEP)", string.Empty);
                totalRow["OCT"] = total.Compute("Sum(OCT)", string.Empty);
                totalRow["NOV"] = total.Compute("Sum(NOV)", string.Empty);
                totalRow["DEC"] = total.Compute("Sum(DEC)", string.Empty);
                totalRow["TOTAL"] = total.Compute("Sum(TOTAL)", string.Empty);
                total.Rows.Add(totalRow);
            }

            Dispatcher.CurrentDispatcher.Invoke(() =>
            { 
                TotalDataView = total.DefaultView; 
            });
        }

        private void SearchData()
        {
             

            if (InputYear > 2000 && InputYear < 2100)
            {
                 IsBusy = true;
                if (!bgWorker.IsBusy)
                {
                    bgWorker.RunWorkerAsync();
                }
            }
            else
            {
                MessageWindow.ShowMessage("查詢範圍有誤", MessageType.ERROR);
            }
        }

        

        private string GetDataViewData(DataView grid)
        {
            string result = "\t1月\t2月\t3月\t4月\t5月\t6月\t7月\t8月\t9月\t10月\t11月\t12月\t總計\r\n";
            foreach (DataRow row in (grid as DataView).ToTable().Rows)
            {

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    result += row.ItemArray[i];

                    if (i < row.ItemArray.Length - 1)
                        result += "\t";
                }
                result += "\r\n";
            }

            result += "\r\n";
            return result;
        }
    }
}