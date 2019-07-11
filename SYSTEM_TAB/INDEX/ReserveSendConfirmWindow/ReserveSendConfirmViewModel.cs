using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.INDEX.ReserveSendConfirmWindow
{
    public class ReserveSendConfirmViewModel : ViewModelBase
    {
        private IndexReserves indexReserveCollection;
        public IndexReserves IndexReserveCollection
        {
            get => indexReserveCollection;
            set
            {
                Set(() => IndexReserveCollection, ref indexReserveCollection, value);
            }
        }
        private IndexReserve indexReserveSelectedItem;
        public IndexReserve IndexReserveSelectedItem
        {
            get => indexReserveSelectedItem;
            set
            {
                Set(() => IndexReserveSelectedItem, ref indexReserveSelectedItem, value);
            }
        }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public ReserveSendConfirmViewModel(IndexReserves indexReserves) {
            SubmitCommand = new RelayCommand(SubmitAction);
            CancelCommand = new RelayCommand(CancelAction);
            IndexReserveCollection = indexReserves;
            if (IndexReserveCollection.Count > 0)
                IndexReserveSelectedItem = IndexReserveCollection[0];
        }
        #region Action
        private void SubmitAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否傳送藥健康?","預約慢箋採購");
            if ((bool)confirmWindow.DialogResult) {
                SavePrepareMedMessage();

            }
        }
        private void CancelAction()
        {
        }
        private void SavePrepareMedMessage() {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "封包架上量統計表";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName =  DateTime.Today.ToString("yyyyMMdd") +  ViewModelMainWindow.CurrentPharmacy.Name + "封包架上量統計表";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("藥品代碼,藥品名稱,調劑量,傳送量,病患,備藥訊息");
                        foreach (var i in IndexReserveCollection)
                        {
                            foreach (var med in i.IndexReserveDetailCollection)
                            {
                                if (med.Amount > med.SendAmount)
                                {
                                    file.WriteLine($"{med.ID},{med.FullName},{med.Amount},{med.SendAmount},{i.CusName},需從架上拿{med.Amount - med.SendAmount}個單位至封包");
                                }
                            }
                        }
                       
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel成功 開始列印封包明細", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }  
        }
        private void PrintPackage() {

        }
        private void StartOrder() {
             
        }
        #endregion
    }
}
