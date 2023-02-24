using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.ClockIn;
using System.Data;
using System.Windows.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text;
using DomainModel.Enum;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DomainModel;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockInSearch
{
    public class ClockInSearchViewModel : TabBase
    {

        #region ----- Define Variables -----
        public override TabBase getTab()
        {
            return this;

        }

        private IEmployeeService _employeeService;
        public ClockInSearchViewModel(IEmployeeService employeeService)
        {
            RegisterCommands();
            GetComYear();
            GetDate();
            _employeeService = employeeService;
        }

        public string account = "";
        public string Account
        {
            get { return account; }
            set { Set(() => Account, ref account, value); }
        }
        public List<string> comYears;
        public List<string> ComYears
        {
            get { return comYears; }
            set
            {
                Set(() => ComYears, ref comYears, value);
            }
        }
        public string searchYear = DateTime.Now.Year.ToString();
        public string SearchYear
        {
            get { return searchYear; }
            set
            {
                Set(() => SearchYear, ref searchYear, value);
                GetDate();
            }
        }
        public string searchMonth = DateTime.Now.Month.ToString();
        public string SearchMonth
        {
            get { return searchMonth; }
            set { Set(() => SearchMonth, ref searchMonth, value);
                GetDate();
            }            
        }

        private void GetComYear()
        {
            ComYears = new List<string>();
            int nowYear = DateTime.Now.Year;

            for (int i = 2021; i <= nowYear; i++)
            {
                ComYears.Add(i.ToString());
            }
        }

        public int hourCount;
        public int HourCount
        {
            get { return hourCount; }
            set
            {
                Set(() => HourCount, ref hourCount, value);
            }
        }

        public int minCount;
        public int MinCount
        {
            get { return minCount; }
            set
            {
                Set(() => MinCount, ref minCount, value);
            }
        }

        public Employee singinemployee;
        public Employee SingInEmployee
        {
            get { return singinemployee; }
            set
            {
                Set(() => SingInEmployee, ref singinemployee, value);
            }
        }

        public Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set
            {
                Set(() => Employee, ref employee, value);
                GetDate();
        }
    }

        public Employees employeeCollection;
        public Employees EmployeeCollection
        {
            get { return employeeCollection; }
            set
            {
                Set(() => EmployeeCollection, ref employeeCollection, value);
            }
        }

        public ClockInLog clockInLogs;
        public ClockInLog ClockInLogs
        {
            get { return clockInLogs; }
            set
            {
                Set(() => ClockInLogs, ref clockInLogs, value);
            }
        }


        public ClockInLog clockInLogsRpt;
        public ClockInLog ClockInLogsRpt
        {
            get { return clockInLogsRpt; }
            set
            {
                Set(() => ClockInLogsRpt, ref clockInLogsRpt, value);
            }
        }
        public class CommonBox
        {
            public string Namepath { get; set; }
            public string Value { get; set; }
        }

        public CommonBox checkLine;
        public CommonBox CheckLine
        {
            get { return checkLine; }
            set
            {
                Set(() => CheckLine, ref checkLine, value);
                if(CheckLine!=null)
                    SetEmployeeCollection();
            }
        }
        public ObservableCollection<CommonBox> checkLines;
        public ObservableCollection<CommonBox> CheckLines
        {
            get { return checkLines; }
            set
            {
                Set(() => CheckLines, ref checkLines, value);
            }
        }
        


        #endregion ----- Define Variables -----

        #region ----- Define Commands -----

        public RelayCommand<object> ConfirmEmpCommand { get; set; }
        public RelayCommand<object> SearchCommand { get; set; }
        public RelayCommand<object> DataChangeCommand { get; set; }
        public RelayCommand ExportCsvCommand { get; set; }


        #endregion ----- Define Commands -----

        #region ----- Define Actions -----

        private void ConfirmEmpAction(object sender)
        {
            SingInEmployee = new Employee
            {
                Account = Account,
                Password = (sender as PasswordBox)?.Password
            };

            if (string.IsNullOrEmpty(SingInEmployee.Password) && !string.IsNullOrEmpty(SingInEmployee.Account))
            {
                return;
            }
            if (string.IsNullOrEmpty(SingInEmployee.Password) && string.IsNullOrEmpty(SingInEmployee.Account))
            {
                MessageWindow.ShowMessage("請輸入帳號密碼!", MessageType.ERROR);
                return;
            }
            else
            {
                if (!CheckPassWord())
                {
                    return;  //檢查帳密
                } 
            }

            SetStore();

            (sender as PasswordBox)?.Clear();
            Account = "";
        }
        private void SearchAction(object sender)
        {
        }

        private void DataChangeAction(object sender)
        {
            MessageWindow.ShowMessage(SearchMonth, Class.MessageType.ERROR);
        }


        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            ConfirmEmpCommand = new RelayCommand<object>(ConfirmEmpAction);
            SearchCommand = new RelayCommand<object>(SearchAction);
            DataChangeCommand = new RelayCommand<object>(DataChangeAction);
            ExportCsvCommand = new RelayCommand(ExportCsv);
        }
        private bool CheckPassWord()
        {
            SingInEmployee = _employeeService.Login(SingInEmployee.Account, SingInEmployee.Password);
            
            //檢查帳密 密碼錯誤
            if (SingInEmployee == null)
            {
                MessageWindow.ShowMessage("密碼錯誤!", Class.MessageType.ERROR);
                return false;
            }
            return true;
        }
        public void GetDate()
        {
            ClockInLogs = null;
            if (Employee != null)
            {
                MainWindow.ServerConnection.OpenConnection();
                ClockInLogs = new ClockInLog(ClockInDb.ClockInLogByDate(SearchYear, SearchMonth, Employee.ID.ToString()));
                MainWindow.ServerConnection.CloseConnection();

                if (ClockInLogs is null) return;
                int iMin = 0;
                foreach (var s in ClockInLogs)
                {
                    iMin += (int)s.WMin;
                }
                HourCount = iMin / 60;
                MinCount = iMin % 60;
            }
        }
        public void SetStore()
        {
            CheckLines = new ObservableCollection<CommonBox>();
            MainWindow.ServerConnection.OpenConnection();
            DataTable dt = MainWindow.ServerConnection.ExecuteProc("[Get].[Pharmacy]");
            MainWindow.ServerConnection.CloseConnection();

            if (SingInEmployee.Authority == Authority.Admin || SingInEmployee.Authority == Authority.PharmacyManager || SingInEmployee.Authority == Authority.AccountingStaff) //需要看見各店的人(系統管理員、藥局經理、會計人員)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    if(dr["Value"].ToString() != "Develop")
                    {
                        CheckLines.Add(new CommonBox() { Namepath = dr["Namepath"].ToString(), Value = dr["Value"].ToString() });
                    }
                    else
                    {
                        if (SingInEmployee.Authority == Authority.Admin)
                        {
                            CheckLines.Add(new CommonBox() { Namepath = dr["Namepath"].ToString(), Value = dr["Value"].ToString() });
                        }
                    }
                }
                if (CheckLines.Count > 0)
                {
                    CheckLine = CheckLines[0];
                }
            }
            else if (SingInEmployee.Authority == Authority.StoreManager) //(4.店長)店長能看見其他員工 
            {
                //不能選擇店
                if (SingInEmployee.IsLocal)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        string Ename = Convert.ToString(dr["Value"]);
                        string Cname = Convert.ToString(dr["Namepath"]);
                        if (Properties.Settings.Default.SystemSerialNumber.Contains(Ename))
                        {
                            CheckLines.Add(new CommonBox() { Namepath = Cname, Value = Ename });
                        }
                    }
                }
                if (CheckLines.Count > 0)
                {
                    CheckLine = CheckLines[0];
                }
            }
            else //(5.店員 6.負責藥師 7.執業藥師 8.支援藥師)只能看自己的打卡紀錄
            {
                if(dt != null && dt.Rows.Count > 0)
                {
                    string namepath = Convert.ToString(dt.Rows[0]["Namepath"]);
                    string value = Convert.ToString(dt.Rows[0]["Value"]);
                    CheckLines.Add(new CommonBox() { Namepath = namepath, Value = value });
                    CheckLine = CheckLines[0];
                }
            }
            SetEmployeeCollection();
        }
        public void SetEmployeeCollection()
        {
            if (SingInEmployee.Authority == Authority.Admin)
            {
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), CheckLine.Value, "", 1);
                MainWindow.ServerConnection.CloseConnection();
            }
            else if (SingInEmployee.Authority == Authority.Admin || SingInEmployee.Authority == Authority.PharmacyManager || SingInEmployee.Authority == Authority.AccountingStaff) //需要看見各店的人
            {
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), CheckLine.Value, "", 2);
                MainWindow.ServerConnection.CloseConnection();
            }
            else if (SingInEmployee.Authority == Authority.StoreManager) //店長能看見其他員工
            {
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), CheckLine.Value, "", 2);
                MainWindow.ServerConnection.CloseConnection();
            }
            else //一般員工
            {
                //只能看自己資料
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), null, SingInEmployee.ID.ToString(), 3);
                MainWindow.ServerConnection.CloseConnection();
            }

            //選第一位
            if (EmployeeCollection.Count > 0)
                Employee = EmployeeCollection[0];
        }
        private void ExportCsv()
        {
            if (SingInEmployee == null)
            {
                MessageWindow.ShowMessage("請先登入查詢!!!", Class.MessageType.ERROR);
                return ;
            }

            //ClockInLogFotReport
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv|*.csv ";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MainWindow.ServerConnection.OpenConnection();
                ClockInLogsRpt = new ClockInLog(ClockInDb.ClockInLogFotReport(SearchYear, SearchMonth, CheckLine.Value));
                MainWindow.ServerConnection.CloseConnection();

                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.CreateNew))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                    var c = 0;int? cMin=0;
                    sw.WriteLine("日期" + "\t" + "店別" + "\t" + "員工編號" + "\t" + "姓名" + "\t" + "上班" + "\t" + "下班" + "\t" + "時數" + "\t" + "小計");
                    
                    foreach (var row in ClockInLogsRpt)
                    {
                        string _str = row.Date.Substring(row.Date.Length - 3, 3);
                        string days = "/" + DateTime.DaysInMonth(DateTime.Now.Year, int.Parse(SearchMonth)).ToString();

                        sw.WriteLine(row.Date + "\t" + row.CurPha_Name + "\t" + row.EmpAccount + "\t" + row.EmpName + "\t" + row.Time + "\t" + row.Time2 + "\t" + row.WMin/60 + "\t"+ row.Type);
                        cMin += row.WMin/60;

                        if (_str == days)
                        {
                            sw.WriteLine("----------\t----------\t---------\t---------\t 總計:  \t   " + cMin + " 小時 ");
                            cMin = 0;
                        }
                        c++;
                    }
                    sw.Close();
                    try
                    {
                        ConfirmWindow cw = new ConfirmWindow("是否開啟檔案", "確認");
                        if ((bool)cw.DialogResult)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageWindow.ShowMessage(e.Message, MessageType.WARNING);
                    }
                }
            }
        }


        #endregion ----- Define Functions -----


    }
}
