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

namespace His_Pos.H7_ACCOUNTANCY_REPORT.CooperativeAdjustReport
{
    /// <summary>
    /// CooperativeAdjustReportView.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeAdjustReportView : UserControl
    {
        public CooperativeAdjustReportView()
        {
            InitializeComponent();
        }

        #region ----- Date Control -----
        private void Date_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
        }

        private void Date_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;

                if (textBox is null) return;

                switch (textBox.Name)
                {
                    case "StartDate":
                        EndDate.Focus();
                        EndDate.SelectAll();
                        break;
                    case "EndDate":
                        SearchButton.Focus();
                        break;
                }
            }
        }
        #endregion
    }
}
