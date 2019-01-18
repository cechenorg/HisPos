using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.NewClass.Person; 
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage
{

    /// <summary>
    /// EmployeeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class EmployeeManageView : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        private bool isFirst = true;

        public Collection<string> positionCollection;
        public Collection<string> PositionCollection
        {
            get {
                return positionCollection;
            }
            set {
                positionCollection = value;
                NotifyPropertyChanged("PositionCollection");
            }
        }
       
         
        public Employee employee;
        public Employee Employee
        {
            get {
                return employee;
            }
            set {
                employee = value;
                NotifyPropertyChanged("Employee");
            }
        }
        public Employees employeeCollection;
        public Employees EmployeeCollection
        {
            get
            {
                return employeeCollection;
            }
            set
            {
                employeeCollection = value;
                NotifyPropertyChanged("EmployeeCollection");
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
        #endregion

        public EmployeeManageView()
        {
            InitializeComponent();
            GetEmployeeData();
            DataContext = this;
        }

        private void GetEmployeeData() {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.InitEmployeeManageView(this); 
        }

        private void DataGridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null) return;
          /// Employee = (Employee)((Employee)(sender as DataGrid).SelectedItem).Clone();
            richtextbox.Document.Blocks.Clear();
            richtextbox.AppendText("");/// Employee.Description);
            InitDataChanged();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Employee newemployee = EmployeeCollection.Where(emp => emp.Id == Employee.Id).ToList()[0];
           /// Employee = (Employee)newemployee.Clone();
            richtextbox.Document.Blocks.Clear();
            ///richtextbox.AppendText(Employee.Description);
            InitDataChanged();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EmployeeCollection.Count; i++) {
                if (EmployeeCollection[i].Id == Employee.Id) {
                    EmployeeCollection[i] = Employee;
                    ///EmployeeCollection[i].Description = new TextRange(richtextbox.Document.ContentStart, richtextbox.Document.ContentEnd).Text;
                    Employee.Save();
                    InitDataChanged();
                    break;
                }
            }
            ///ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels = PharmacyDb.GetPharmacyMedicalPersonData();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int count = EmployeeCollection.Count;
            Employee newEmployee = new Employee();
            newEmployee.Save();
           /// employee.Id = table.Rows[0][0].ToString();
            EmployeeCollection.Add(newEmployee);
            DataGridEmployee.SelectedIndex = count;
            ///ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels = PharmacyDb.GetPharmacyMedicalPersonData();
        }

        private void ButtonDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridEmployee.SelectedItem == null) return;
            Employee.Delete();  
            EmployeeCollection.Remove((Employee)DataGridEmployee.SelectedItem);
            DataGridEmployee.SelectedIndex = EmployeeCollection.Count-1;
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }
        private void DataChanged()
        {
            if (isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonCancel.IsEnabled = true;
            ButtonSubmit.IsEnabled = true;
        }

        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonCancel.IsEnabled = false;
            ButtonSubmit.IsEnabled = false;
        }
       
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            isFirst = false;
        }

        private void EmpId_TextChanged(object sender, TextChangedEventArgs e) {
            DataGridEmployee.Items.Filter = ((o) => {
                if (((Employee)o).Name.Contains(EmpId.Text) || ((Employee)o).Id.ToString().Contains(EmpId.Text) || ((Employee)o).Name.Contains(EmpId.Text))
                    return true;
                else
                    return false;
            }); 
        }

        private void ChangePassword_OnClick(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(Employee.Id);
        }
    }
}
