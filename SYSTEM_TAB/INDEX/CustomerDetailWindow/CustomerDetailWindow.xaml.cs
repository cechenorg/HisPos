using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.Customer;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow {
    /// <summary>
    /// CustomerDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerDetailWindow : Window {
     
        public CustomerDetailWindow(int cusID) {
            InitializeComponent();
            CustomerDetailWindowViewModel customerDetailWindowViewModel = new CustomerDetailWindowViewModel(cusID);
            DataContext = customerDetailWindowViewModel;
            ShowDialog();
        }
    }
}
