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
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl
{
    /// <summary>
    /// GetCustomerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class GetCustomerWindow : Window
    {
        #region ----- Define Variables -----
        public string SearchString { get; set; }
        #endregion

        public GetCustomerWindow(string searchString)
        {
            InitializeComponent();

            SearchString = searchString;
        }

        #region ----- Define Functions -----

        #endregion
    }
}