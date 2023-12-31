﻿using System;
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

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement2
{
    /// <summary>
    /// NewIncomeStatement2.xaml 的互動邏輯
    /// </summary>
    public partial class NewIncomeStatement2 : UserControl
    {
        public NewIncomeStatement2()
        {
            InitializeComponent();
        }

        private void dg_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock box = (TextBlock)sender;
            Grid grid = (Grid)box.Parent;
            int index = grid.Children.IndexOf(box);
            ((NewIncomeStatement2ViewModel)DataContext).CurrentMonth = index;
        }
    }
}
