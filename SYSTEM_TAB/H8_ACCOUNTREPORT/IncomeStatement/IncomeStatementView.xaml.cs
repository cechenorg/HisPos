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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.IncomeStatement
{
    /// <summary>
    /// IncomeStatementView.xaml 的互動邏輯
    /// </summary>
    public partial class IncomeStatementView : UserControl
    {
        public IncomeStatementView()
        {
            InitializeComponent();
            DataContext = new IncomeStatementViewModel();
        }
    }
}
