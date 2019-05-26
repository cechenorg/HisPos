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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerSearchWindow
{
    /// <summary>
    /// CustomerSearchWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSearchWindow : Window
    {
        public CustomerSearchWindow(string search,CustomerSearchCondition condition)
        {
            InitializeComponent();
            DataContext = new CustomerSearchViewModel(search,condition);
        }

        public CustomerSearchWindow(DateTime birth)
        {
            InitializeComponent();
            DataContext = new CustomerSearchViewModel(birth);
        }
    }
}
