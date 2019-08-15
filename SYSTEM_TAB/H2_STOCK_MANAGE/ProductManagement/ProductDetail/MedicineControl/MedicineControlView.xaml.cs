﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    /// <summary>
    /// MedicineControlView.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineControlView : UserControl
    {
        public MedicineControlView()
        {
            InitializeComponent();
        }


        private void IsCommon_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if(checkBox is null) return;

            if ((bool) checkBox.IsChecked)
            {
                MessageWindow.ShowMessage("架上安全量: 架上量小於安全量時，會在常備採購時轉出採購\r\n\r\n基準量: 架上量小於安全量時，會採購至基準量\r\n\r\n包裝量: 常備採購時，會採購包裝量的倍數", MessageType.WARNING);
            }
        }
    }
}
