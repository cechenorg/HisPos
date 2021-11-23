using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.Product;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.SYSTEM_TAB.INDEX.ReserveSendConfirmWindow
{
    public class ReserveSendConfirmViewModel : ViewModelBase
    {
        private bool isAllSend;

        public bool IsAllSend
        {
            get => isAllSend;
            set
            {
                Set(() => IsAllSend, ref isAllSend, value);
                CaculateReserveSendAmount();
            }
        }

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
                if (IndexReserveSelectedItem is null) return;
                CaculateReserveSendAmount();
            }
        }

        private IndexReserveDetail indexReserveMedicineSelectedItem;

        public IndexReserveDetail IndexReserveMedicineSelectedItem
        {
            get => indexReserveMedicineSelectedItem;
            set
            {
                Set(() => IndexReserveMedicineSelectedItem, ref indexReserveMedicineSelectedItem, value);
            }
        }

        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand SendAmountChangeCommand { get; set; }
        public RelayCommand ShowMedicineDetailCommand { get; set; }

        public ReserveSendConfirmViewModel(IndexReserves indexReserves)
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            SendAmountChangeCommand = new RelayCommand(SendAmountChangeAction);
            ShowMedicineDetailCommand = new RelayCommand(ShowMedicineDetailAction);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "UpdateUsableAmountMessage")
                    CaculateReserveSendAmount();
            });
            IndexReserveCollection = indexReserves;
            if (IndexReserveCollection.Count > 0)
            {
                IndexReserveSelectedItem = IndexReserveCollection[0];
                CaculateReserveSendAmount();
            }
        }

        #region Action

        private void SubmitAction()
        {
            if (IndexReserveSelectedItem is null) return;
            ConfirmWindow confirmWindow;
            ConfirmWindow printWindow;
            var confirm = false;
            var print = false;
            switch (IndexReserveSelectedItem.PrepareMedType)
            {
                case ReserveSendType.AllPrepare:
                    printWindow = new ConfirmWindow("是否列印封包明細?", "預約慢箋採購");
                    print = (bool)printWindow.DialogResult;
                    SendReserveStoOrder(print);
                    break;

                default:
                    confirmWindow = new ConfirmWindow("是否傳送藥健康?", "預約慢箋採購");
                    confirm = (bool)confirmWindow.DialogResult;
                    if (!confirm) return;
                    printWindow = new ConfirmWindow("是否列印封包明細?", "預約慢箋採購");
                    print = (bool)printWindow.DialogResult;
                    SendReserveStoOrder(print);
                    break;
            }
            if (IndexReserveCollection.Count == 0)
            {
                MessageWindow.ShowMessage("未有備藥傳送處方", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CloseReserveSendConfirmWindow"));
            }
        }

        private void SendAmountChangeAction()
        {
            CheckSendStatus();
        }

        private void SendReserveStoOrder(bool print)
        {
            MainWindow.ServerConnection.OpenConnection();
            switch (IndexReserveSelectedItem.PrepareMedType)
            {
                case ReserveSendType.AllPrepare:
                    IndexReserveSelectedItem.PrepareMedStatus = IndexPrepareMedType.Prepare;
                    IndexReserveSelectedItem.SaveStatus();
                    if (print)
                        PrintPackage();
                    IndexReserveCollection.Remove(IndexReserveSelectedItem);
                    break;

                case ReserveSendType.AllSend:
                case ReserveSendType.CoPrepare:
                    if (IndexReserveSelectedItem.StoreOrderToSingde())
                    {
                        if (print)
                            PrintPackage();
                        IndexReserveCollection.Remove(IndexReserveSelectedItem);
                    }

                    break;
            }

            if (IndexReserveCollection.Count > 0)
                IndexReserveSelectedItem = IndexReserveCollection[0];
            MainWindow.ServerConnection.CloseConnection();
        }

        private void SavePrepareMedMessage()
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "封包架上量統計表";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "封包架上量統計表";
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

                        foreach (var med in IndexReserveSelectedItem.IndexReserveDetailCollection)
                        {
                            if (med.Amount > med.SendAmount)
                            {
                                file.WriteLine($"{med.ID},{med.FullName},{med.Amount},{med.SendAmount},{IndexReserveSelectedItem.CusName},需從架上拿{med.Amount - med.SendAmount}個單位至封包");
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

        private void CheckSendStatus()
        {
            int sameCount = 0;
            int zeroSendCount = 0;
            foreach (var s in IndexReserveSelectedItem.IndexReserveDetailCollection)
            {
                if (s.SendAmount == s.Amount)
                    sameCount++;
                else if (s.SendAmount == 0)
                    zeroSendCount++;
            }
            if (sameCount == 0 && zeroSendCount == IndexReserveSelectedItem.IndexReserveDetailCollection.Count)
                IndexReserveSelectedItem.PrepareMedType = ReserveSendType.AllPrepare;
            else if (sameCount == IndexReserveSelectedItem.IndexReserveDetailCollection.Count)
                IndexReserveSelectedItem.PrepareMedType = ReserveSendType.AllSend;
            else
                IndexReserveSelectedItem.PrepareMedType = ReserveSendType.CoPrepare;
        }

        private void CaculateReserveSendAmount()
        {
            if (IndexReserveSelectedItem is null) return;
            MainWindow.ServerConnection.OpenConnection();

            IndexReserveSelectedItem.GetIndexSendDetail();
            List<string> MedicineIds = new List<string>();
            foreach (var med in IndexReserveSelectedItem.IndexReserveDetailCollection)
            {
                MedicineIds.Add(med.ID);
            }
            Inventorys InventoryCollection = Inventorys.GetAllInventoryByProIDs(MedicineIds);

            for (int j = 0; j < IndexReserveSelectedItem.IndexReserveDetailCollection.Count; j++)
            {
                var pro = IndexReserveSelectedItem.IndexReserveDetailCollection[j];
                if (InventoryCollection.Count(inv => inv.InvID.ToString() == pro.InvID) == 0)
                    pro.SendAmount = Convert.ToInt32(pro.Amount);
                else
                {
                    var target = InventoryCollection.Single(inv => inv.InvID.ToString() == pro.InvID);
                    pro.SendAmount = IsAllSend ? pro.Amount : target.OnTheFrame - pro.Amount > 0 ? 0 : pro.Amount - target.OnTheFrame;
                    pro.FrameAmount = target.OnTheFrame;
                }
            }
            MainWindow.ServerConnection.CloseConnection();
            CheckSendStatus();
        }

        private void PrintPackage()
        {
            ReportViewer rptViewer = new ReportViewer();
            IndexReserveSelectedItem.SetReserveMedicinesSheetReportViewer(rptViewer);
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                ((VM)MainWindow.Instance.DataContext).StartPrintReserve(rptViewer);
            });
        }

        private void ShowMedicineDetailAction()
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { IndexReserveMedicineSelectedItem.ID, "0" }, "ShowProductDetail"));
        }

        #endregion Action
    }
}