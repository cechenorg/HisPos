﻿using System;
using System.Collections.Generic;
using System.Data;
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
using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// EditTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EditTypeWindow : Window
    {
        #region ----- Define Variables -----
        private ProductTypeManageMaster SelectedType { get; set; }
        public string BigTypeName { get; set; }
        public string SmallTypeName { get; set; } = "";
        #endregion

        public EditTypeWindow(ProductTypeManageMaster currentType)
        {
            InitializeComponent();

            SelectedType = currentType;

            BigTypeName = currentType.Name;

            if (currentType.CurrentDetailType != null)
            {
                SmallTypeName = currentType.CurrentDetailType.Name;
                SmallTypeTextBox.IsEnabled = true;
            }

            DataContext = this;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if(BigTypeName.Equals(String.Empty))
            {
                MessageWindow.ShowMessage("大類別名稱不可為空", Class.MessageType.ERROR);
                return;
            }

            if (SelectedType.CurrentDetailType != null && SmallTypeName.Equals(String.Empty))
            {
                MessageWindow.ShowMessage("小類別名稱不可為空", Class.MessageType.ERROR);
                return;
            }
            
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductTypeDB.UpdateType(SelectedType.ID, BigTypeName, (SelectedType.CurrentDetailType is null)? -1 : SelectedType.CurrentDetailType.ID, SmallTypeName);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable is null || dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("新增失敗 請稍後再試", Class.MessageType.ERROR);
                return;
            }

            Close();
        }
    }
}
