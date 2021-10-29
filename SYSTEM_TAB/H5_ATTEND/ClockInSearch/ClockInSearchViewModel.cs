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

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockInSearch
{
    public class ClockInSearchViewModel : TabBase
    {

        #region ----- Define Variables -----
        public override TabBase getTab()
        {
            return this;

        }
        public ClockInSearchViewModel()
        {
            RegisterCommands();
            GetDate();
        }

        public string account = "";
        public string Account
        {
            get { return account; }
            set { Set(() => Account, ref account, value); }
        }

        public string searchMonth = System.DateTime.Now.Month.ToString();
        public string SearchMonth
        {
            get { return searchMonth; }
            set { Set(() => SearchMonth, ref searchMonth, value);

                GetDate();
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

        public System.Collections.Generic.List<CommonBox> checkLines;
        public System.Collections.Generic.List<CommonBox> CheckLines
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

            SingInEmployee = new Employee();
            SingInEmployee.Account = Account;
            SingInEmployee.Password = (sender as System.Windows.Controls.PasswordBox)?.Password;

            if (string.IsNullOrEmpty(SingInEmployee.Password) && !string.IsNullOrEmpty(SingInEmployee.Account))
            {
                return;
            }
            if (string.IsNullOrEmpty(SingInEmployee.Password) && string.IsNullOrEmpty(SingInEmployee.Account))
            {
                MessageWindow.ShowMessage("請輸入帳號密碼!", Class.MessageType.ERROR);
                return;
            }
            else
                if (!CheckPassWord()) return;  //檢查帳密

            SetStore();

            (sender as System.Windows.Controls.PasswordBox)?.Clear();
            this.Account = "";


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

            //1.如果全部都沒有,查無帳號,請確認帳號
            if (SingInEmployee.CheckEmployeeAccountSame())
            {
                MessageWindow.ShowMessage("此帳號不存在!", Class.MessageType.ERROR);
                return false;
            }

            MainWindow.ServerConnection.OpenConnection();
            SingInEmployee = Employee.Login(SingInEmployee.Account, SingInEmployee.Password);
            MainWindow.ServerConnection.CloseConnection();

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
                ClockInLogs = new ClockInLog(ClockInDb.ClockInLogByDate(System.DateTime.Now.Year.ToString(), SearchMonth, Employee.ID.ToString()));
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
            this.CheckLines = null;
            this.CheckLines = new System.Collections.Generic.List<CommonBox>();
            DataTable dt;
            MainWindow.ServerConnection.OpenConnection();
            dt = MainWindow.ServerConnection.ExecuteProc("[Get].[Pharmacy]");
            MainWindow.ServerConnection.CloseConnection();



            if (SingInEmployee.ID == 1)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    this.CheckLines.Add(new CommonBox() { Namepath = dr["Namepath"].ToString(), Value = dr["Value"].ToString() });
                }

                #region OLD
                //this.CheckLines.Add(new CommonBox() { Namepath = "測試", Value = "Develop" });
                //this.CheckLines.Add(new CommonBox() { Namepath = "杏昌", Value = "XingChang" });
                //this.CheckLines.Add(new CommonBox() { Namepath = "明昌", Value = "MingChang" });
                //this.CheckLines.Add(new CommonBox() { Namepath = "宏昌", Value = "HongChang" });
                //this.CheckLines.Add(new CommonBox() { Namepath = "佑昌", Value = "YoChang" });
                //this.CheckLines.Add(new CommonBox() { Namepath = "佑東", Value = "YoDong" });
                //this.CheckLines.Add(new CommonBox() { Namepath = "和昌", Value = "HeChang" });
                #endregion

                this.CheckLine = this.checkLines[0];


            }
            else if ( SingInEmployee.ID == 97 || SingInEmployee.ID == 45 || SingInEmployee.ID == 114) //需要看見各店的人
            {

                foreach (DataRow dr in dt.Rows)
                {
                    if(dr["Value"].ToString() != "Develop")
                        this.CheckLines.Add(new CommonBox() { Namepath = dr["Namepath"].ToString(), Value = dr["Value"].ToString() });
                }

                this.CheckLine = this.checkLines[0];

            }
            else if ((SingInEmployee.ID == 40 || SingInEmployee.ID == 48 || SingInEmployee.ID == 72 || SingInEmployee.ID == 47) && ViewModelMainWindow.CurrentPharmacy.Name != "和安藥局") //店長能看見其他員工SingInEmployee.WorkPosition.WorkPositionId == 2 || 
            {
                //不能選擇店
                if (SingInEmployee.ID == 72)
                    this.CheckLines.Add(new CommonBox() { Namepath = "和昌", Value = "HeChang" });
                if (SingInEmployee.ID == 40)
                    this.CheckLines.Add(new CommonBox() { Namepath = "明昌", Value = "MingChang" });
                if (SingInEmployee.ID == 47)
                    this.CheckLines.Add(new CommonBox() { Namepath = "佑昌", Value = "YoChang" });
                if (SingInEmployee.ID == 48)
                    this.CheckLines.Add(new CommonBox() { Namepath = "杏昌", Value = "XingChang" });

                this.CheckLine = this.checkLines[0];

            }
            else //一般員工
            {
                //不能選擇店
                this.CheckLines.Add(new CommonBox() { Namepath = "無選項", Value = "" });
                this.CheckLine = this.checkLines[0];

            }

            SetEmployeeCollection();


        }
        public void SetEmployeeCollection()
        {

            if (SingInEmployee.ID == 1)
            {
              

                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(), CheckLine.Value, "", 1);
                MainWindow.ServerConnection.CloseConnection();

            }
            else if (SingInEmployee.ID == 97 || SingInEmployee.ID == 45 || SingInEmployee.ID == 114) //需要看見各店的人
            {

                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(), CheckLine.Value, "", 2);
                MainWindow.ServerConnection.CloseConnection();

            }
            else if (SingInEmployee.ID == 40 || SingInEmployee.ID == 48 || SingInEmployee.ID == 72 || SingInEmployee.ID == 47) //店長能看見其他員工SingInEmployee.WorkPosition.WorkPositionId == 2 || 
            {

                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(), CheckLine.Value, "", 2);
                MainWindow.ServerConnection.CloseConnection();

            }
            else //一般員工
            {

                //只能看自己資料
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockInEmp(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(), null, SingInEmployee.ID.ToString(), 3);
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
                ClockInLogsRpt = new ClockInLog(ClockInDb.ClockInLogFotReport(System.DateTime.Now.Year.ToString(), SearchMonth, CheckLine.Value));
                MainWindow.ServerConnection.CloseConnection();

                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.CreateNew))
                {
                    
                    StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                    var c = 0;int? cMin=0;
                    sw.WriteLine("日期" + "\t" + "店別" + "\t" + "姓名" + "\t" + "上班" + "\t" + "下班" + "\t" + "時數" + "\t" + "小計");
                    
                    foreach (var row in ClockInLogsRpt)
                    {
                        string _str = row.Date.Substring(row.Date.Length - 3, 3);
                        string days = "/"+System.DateTime.DaysInMonth(System.DateTime.Now.Year, int.Parse(SearchMonth)).ToString();

                        sw.WriteLine(row.Date + "\t" + row.EmpAccount + "\t" + row.EmpName + "\t" + row.Time + "\t" + row.Time2 + "\t" + row.WMin/60 + "\t"+ row.Type);
                        cMin += row.WMin/60;

                        if (_str == days)
                        {

                            sw.WriteLine("----------\t----------\t---------\t---------\t 總計:  \t   " + cMin + " 小時 ");

                            cMin = 0;
                        }
                        c++;
                    }
                    sw.Close();
                }
            }


        }


        #endregion ----- Define Functions -----


    }
}
