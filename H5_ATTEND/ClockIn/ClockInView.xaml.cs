using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using His_Pos.Class;
using His_Pos.Class.Employee;
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.ClockIn
{
    /// <summary>
    /// ClockInView.xaml 的互動邏輯
    /// </summary>
    public partial class ClockInView : UserControl, INotifyPropertyChanged
    {
        private int typeFilterCondition = 0;
        public class EmpClockIn {
            public EmpClockIn(string name,string type,string date,string time) {
                empName = name;
                clocckType = type;
                typeIcon = type == "上班" ? new BitmapImage(new Uri(@"..\..\Images\DarkerHisDot.png", UriKind.Relative)) : new BitmapImage(new Uri(@"..\..\Images\PosDot.png", UriKind.Relative));
                clocckDate = date;
                clockTime = time;
            }
            public BitmapImage typeIcon { get; set; }
            public string empName { get; set;}
            public string clocckType { get; set; }
            public string clocckDate { get; set; }
            public string clockTime { get; set; }
        }
        private ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> Employees
        {
            get
            {
                return employees;
            }
            set
            {
                employees = value;
                NotifyPropertyChanged("Employees");
            }
        }
        private ObservableCollection<EmpClockIn> empClockIns = new ObservableCollection<EmpClockIn>();

        public ObservableCollection<EmpClockIn> EmpClockIns {
            get
            {
                return empClockIns;
            }
            set
            {
                empClockIns = value;
                NotifyPropertyChanged("EmpClockIns");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public ObservableCollection<UserIconData> UserIconDatas { get; set; }

        public ClockInView()
        {
            InitializeComponent();
            DataContext = this;
            EmpClockIns = EmployeeDb.GetEmpClockIn();
            StratClock();
            InitTodayUser();
            InitEmployee();
        }

        private void InitTodayUser()
        {
            UserIconDatas = WorkScheduleDb.GetTodayUsers();

            foreach (var userIconData in UserIconDatas)
            {
                ClockInUserIcon clockInUserIcon = new ClockInUserIcon(userIconData);
                clockInUserIcon.MouseLeftButtonDown += UserIcon_Click;

                UserIconStack.Children.Add(clockInUserIcon);
            }
        }
        private void InitEmployee() {
            Employee employee = new Employee();
            employee.Name = "";
            Employees.Add(employee);
            foreach (DataRow row in EmployeeDb.GetEmployeeData().Rows) {
                Employees.Add(new Employee(row));
            }
        }
        private void UserIcon_Click(object sender, RoutedEventArgs e)
        {
            UserId.Text = (sender as ClockInUserIcon).Id;

            UserPassword.Focus();
        }

        private void UserId_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                UserPassword.Focus();
        }

        private void UserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UserClockIn();
        }

        private void ClockIn_Click(object sender, RoutedEventArgs e)
        {
            UserClockIn();
        }

        private void UserClockIn()
        {
            if (IsDataEmpty())
            {
                MessageWindow messageWindow = new MessageWindow("帳號或密碼未填!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }

            string inout = InOutStack.Children.OfType<RadioButton>().Single(r => (bool) r.IsChecked).Tag.ToString();

            switch (WorkScheduleDb.UserClockIn(UserId.Text, UserPassword.Password, inout)) {
                case "上班打卡成功":
                    MessageWindow messageWindow = new MessageWindow("上班打卡成功!", MessageType.SUCCESS);
                    messageWindow.ShowDialog();
                    ClearUi();
                    break;
                case "您已經打過上班卡":
                    messageWindow = new MessageWindow("您已經打過上班卡", MessageType.ERROR);
                    messageWindow.ShowDialog();
                    UserId.Text = string.Empty;
                    UserPassword.Password = string.Empty;
                    break;
                case "您已經下班 無法打卡上班 已通知主管":
                    messageWindow = new MessageWindow("您已經下班 無法打卡上班 已通知主管", MessageType.ERROR);
                    messageWindow.ShowDialog();
                    UserId.Text = string.Empty;
                    UserPassword.Password = string.Empty;
                    break;
                case "下班打卡成功":
                    messageWindow = new MessageWindow("下班打卡成功!", MessageType.SUCCESS);
                    messageWindow.ShowDialog();
                    ClearUi();
                    break;
                case "打卡失敗":
                    messageWindow = new MessageWindow("帳號或密碼錯誤!", MessageType.ERROR);
                    messageWindow.ShowDialog();

                    UserPassword.Password = "";
                    UserPassword.Focus();
                    break;
            }
            EmpClockIns = EmployeeDb.GetEmpClockIn();
        }

        private void ClearUi()
        {
            UserId.Text = "";
            UserPassword.Password = "";
        }

        private bool IsDataEmpty()
        {
            return UserId.Text.Equals("") || UserPassword.Password.Equals("");
        }

        private void TickEvent(Object sender, EventArgs e)
        {
            SystemTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }
       
        private void StratClock()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += TickEvent;
            timer.Start();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            typeFilterCondition = Int16.Parse(radioButton.Tag.ToString());

            if (DataGridClockIn is null) return;
            DataGridClockIn.Items.Filter = ClockInTypeFilter;
            DataGridClockIn.SelectedIndex = 0;
        }
        private bool ClockInTypeFilter(object item)
        {
            string employee = comboboxEmployee.SelectedItem == null ? "" : ((Employee)comboboxEmployee.SelectedItem).Name;
            string date = start.Text == "" ? "" : Convert.ToDateTime(start.Text).ToString("yyyy/MM/dd");
            switch (typeFilterCondition) {
                case 0:
                    if ( (((EmpClockIn)item).clocckType == "上班")
                        && (((EmpClockIn)item).empName.Contains(employee) || employee == "")
                        && (((EmpClockIn)item).clocckDate.Equals(date) || start.Text == "")
                        )
                        return true;
                    else
                        return false;
                case 1:
                    if ((((EmpClockIn)item).clocckType == "下班")
                        && (((EmpClockIn)item).empName.Contains(employee) || employee == "")
                        && (((EmpClockIn)item).clocckDate.Equals(date) || start.Text == "")
                        )
                        return true;
                    else
                        return false;
                case 2:
                    if ((((EmpClockIn)item).empName.Contains(employee) || employee == "")
                        && (((EmpClockIn)item).clocckDate.Equals(date) || start.Text == "")
                        )
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }
        
        private void start_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridClockIn.Items.Filter = ClockInTypeFilter;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridClockIn.Items.Filter = ClockInTypeFilter;
        }
    }
}
