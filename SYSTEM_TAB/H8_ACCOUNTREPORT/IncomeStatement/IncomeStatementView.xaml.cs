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
using His_Pos.Class;
using His_Pos.FunctionWindow;

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
        }

        private void Search_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var text = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                if (text.Text.Length != 4)
                {
                    MessageWindow.ShowMessage("年份格式錯誤", MessageType.ERROR);
                    return;
                }
                (DataContext as IncomeStatementViewModel)?.Search.Execute(null);
            }
        }

        private void TextBox_SelectAll(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectAll();
        }
    }
}
