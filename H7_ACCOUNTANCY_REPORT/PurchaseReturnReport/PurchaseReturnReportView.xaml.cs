﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace His_Pos.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    /// <summary>
    /// PurchaseReturnReportView.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseReturnReportView : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----

        private bool IsFirstStack { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public PurchaseReturnReportView()
        {
            InitializeComponent();


        }


    }
}