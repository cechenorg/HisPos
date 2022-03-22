using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ExportProductUsageTemplate;
using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.PresControl
{
    /// <summary>
    /// ExportPresHistoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ExportPresHistoryWindow : Window
    {
        private string productID;
        private int wareHouseID;

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public ExportPresHistoryWindow(string proID, int wareID)
        {
            InitializeComponent();
            productID = proID;
            wareHouseID = wareID;
            DataContext = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (StartDateTime is null || EndDateTime is null)
            {
                MessageWindow.ShowMessage("日期不可為空!", MessageType.ERROR);
                return;
            }

            if (DateTime.Compare((DateTime)StartDateTime, (DateTime)EndDateTime) > 0)
            {
                MessageWindow.ShowMessage("起始日期不可大於結束日期!", MessageType.ERROR);
                return;
            }

            Collection<object> tempCollection = new Collection<object>() { new List<object>() { productID, (DateTime)StartDateTime, (DateTime)EndDateTime, wareHouseID } };

            MainWindow.ServerConnection.OpenConnection();
            ExportExcelService service = new ExportExcelService(tempCollection, new ExportProductUsageTemplate());
            bool isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{productID} 商品用量歷程{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
                MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
            else
                MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);

            Close();
        }
    }
}