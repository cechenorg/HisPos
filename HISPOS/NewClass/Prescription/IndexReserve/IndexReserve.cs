using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.ReserveMedicine;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.INDEX;
using His_Pos.SYSTEM_TAB.INDEX.ReserveSendConfirmWindow;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
    public class IndexReserve : ObservableObject
    {
        public IndexReserve(DataRow r)
        {
            IndexReserveDetailCollection = new IndexReserveDetails();
            Id = r.Field<int>("Id");
            CusId = r.Field<int>("Cus_ID");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            TreatDate = r.Field<DateTime>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            PhoneNote = r.Field<string>("Cus_UrgentNote");
            Profit = Convert.ToInt32(r.Field<double>("Profit"));
            ChronicSequence = r.Field<byte>("ResMas_ChronicSequence");
            ChronicTotal = r.Field<byte>("ResMas_ChronicTotal");
            IsExpensive = r.Field<bool>("IsExpensive");
            CusBirth = r.Field<DateTime>("Cus_Birthday");
            switch (r.Field<string>("MedPrepareStatus"))
            {
                case "N":
                    PrepareMedStatus = IndexPrepareMedType.Unprocess;
                    break;

                case "D":
                    PrepareMedStatus = IndexPrepareMedType.Prepare;
                    break;

                case "F":
                    PrepareMedStatus = IndexPrepareMedType.UnPrepare;
                    isNoSend = true;
                    break;
            }
            switch (r.Field<string>("CallStatus"))
            {
                case "N":
                    PhoneCallStatusName = "未處理";
                    break;

                case "D":
                    PhoneCallStatusName = "已聯絡";
                    break;

                case "F":
                    PhoneCallStatusName = "電話未接";
                    break;
            }
        }

        public int CusId { get; set; }
        public int Id { get; set; }
        public string StoOrdID { get; set; }
        public string CusName { get; set; }
        public DateTime CusBirth { get; set; }
        public string InsName { get; set; }
        public string DivName { get; set; }
        public int Profit { get; set; }
        public int ChronicSequence { get; set; }
        public int ChronicTotal { get; set; }
        public DateTime TreatDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public string PhoneNote { get; set; }
        private ReserveSendType prepareMedType;

        public ReserveSendType PrepareMedType
        {
            get => prepareMedType;
            set
            {
                Set(() => PrepareMedType, ref prepareMedType, value);
            }
        }

        private bool isNoSend;

        public bool IsNoSend
        {
            get => isNoSend;
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();
                if (value)
                {
                    if (PrepareMedStatus == IndexPrepareMedType.Prepare)
                    {
                        var confirmWindow = new ConfirmWindow("此預約處方已備藥 是否轉不備藥? (如訂單已出貨不會取消)", "預約處方通知");
                        if ((bool)confirmWindow.DialogResult)
                        {
                            PrepareMedStatus = IndexPrepareMedType.UnPrepare;
                            DeleteOrder();
                        }
                        else
                            value = false;
                    }
                    else
                        PrepareMedStatus = IndexPrepareMedType.UnPrepare;
                }
                else
                {
                    PrepareMedStatus = IndexPrepareMedType.Unprocess;
                }
                SaveStatus();
                MainWindow.ServerConnection.CloseConnection();
                MainWindow.SingdeConnection.CloseConnection();
                Set(() => IsNoSend, ref isNoSend, value);
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("ReloadIndexReserves"));
            }
        }

        private string phoneCallStatus;

        public string PhoneCallStatus
        {
            get => phoneCallStatus;
            set
            {
                Set(() => PhoneCallStatus, ref phoneCallStatus, value);
            }
        }

        private string phoneCallStatusName;

        public string PhoneCallStatusName
        {
            get => phoneCallStatusName;
            set
            {
                Set(() => PhoneCallStatusName, ref phoneCallStatusName, value);
                switch (PhoneCallStatusName)
                {
                    case "未處理":
                        PhoneCallStatus = "N";
                        break;

                    case "已聯絡":
                        PhoneCallStatus = "D";
                        break;

                    case "電話未接":
                        PhoneCallStatus = "F";
                        break;
                }
            }
        }

        private IndexReserveDetails indexReserveDetailCollection;

        public IndexReserveDetails IndexReserveDetailCollection
        {
            get => indexReserveDetailCollection;
            set
            {
                Set(() => IndexReserveDetailCollection, ref indexReserveDetailCollection, value);
            }
        }

        private IndexPrepareMedType prepareMedStatus;

        public IndexPrepareMedType PrepareMedStatus
        {
            get => prepareMedStatus;
            set
            {
                Set(() => PrepareMedStatus, ref prepareMedStatus, value);
            }
        }

        public bool IsExpensive { get; set; }

        public void SaveStatus()
        {
            IndexReserveDb.Save(Id, PhoneCallStatus, PrepareMedStatus, StoOrdID);
        }

        public void GetIndexDetail()
        {
            IndexReserveDetailCollection.GetDataById(Id);
        }

        public void GetIndexSendDetail()
        {
            IndexReserveDetailCollection.GetSendDataById(Id);
        }

        public bool StoreOrderToSingde()
        {
            int count = StoreOrderDB.GetStoOrdMasterCountByDate().Rows[0].Field<int>("Count");
            bool result = false;
            string newStoOrdID = "P" + DateTime.Today.ToString("yyyyMMdd") + "-" + count.ToString().PadLeft(2, '0');
            this.StoOrdID = newStoOrdID;
            string note = "調劑日:" + AdjustDate.AddYears(-1911).ToString("yyy-MM-dd") + "\r\n";
            for (int j = 0; j < this.IndexReserveDetailCollection.Count; j++)
            {
                IndexReserveDetailCollection[j].StoOrdID = newStoOrdID;
                note += $"{IndexReserveDetailCollection[j].ID} {IndexReserveDetailCollection[j].FullName.PadRight(20).Substring(0, 20)} 傳送 {IndexReserveDetailCollection[j].SendAmount}  自備 {IndexReserveDetailCollection[j].Amount - IndexReserveDetailCollection[j].SendAmount} \r\n";
            }
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            if (StoreOrderDB.InsertIndexReserveOrder(this).Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                if (StoreOrderDB.SendStoreOrderToSingde(this, note).Rows[0][0].ToString() == "SUCCESS")
                {
                    StoreOrderDB.StoreOrderToWaiting(StoOrdID);
                    PrepareMedStatus = IndexPrepareMedType.Prepare;
                    SaveStatus();
                    result = true;
                }
                else
                    MessageWindow.ShowMessage(StoOrdID + "傳送失敗", NewClass.MessageType.ERROR);
            }
            else
                MessageWindow.ShowMessage(CusName + "預約已傳送過 請重新查詢!", NewClass.MessageType.ERROR);
            MainWindow.ServerConnection.CloseConnection();
            MainWindow.SingdeConnection.CloseConnection();
            return result;
        }

        public void SetReserveMedicinesSheetReportViewer(ReportViewer rptViewer)
        {
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new ReserveMedicines(IndexReserveDetailCollection);
            var json = JsonConvert.SerializeObject(medBagMedicines);
            var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            List<ReportParameter> parameters;
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    rptViewer.LocalReport.ReportPath = @"RDLC\ReserveSheet_A5.rdlc";
                    parameters = CreateReserveMedicinesSheetParametersA5();
                    break;

                default:
                    rptViewer.LocalReport.ReportPath = @"RDLC\ReserveSheet.rdlc";
                    parameters = CreateReserveMedicinesSheetParameters();
                    break;
            }
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            var rd = new ReportDataSource("ReserveMedicinesDataSet", dataTable);
            rptViewer.LocalReport.DataSources.Add(rd);
            rptViewer.LocalReport.Refresh();
        }

        private List<ReportParameter> CreateReserveMedicinesSheetParameters()
        {
            var adjustEnd = AdjustDate.AddYears(-1911).AddDays(20);
            return new List<ReportParameter>
            {
                new ReportParameter("Type","預約"),
                new ReportParameter("PatientName",CusName),
                new ReportParameter("PatientBirthday",CusBirth.AddYears(-1911).ToString("yyy-MM-dd")),
                new ReportParameter("PatientTel",PhoneNote),
                new ReportParameter("Institution", InsName),
                new ReportParameter("Division", DivName),
                new ReportParameter("AdjustStart", $"{AdjustDate.AddYears(-1911):yyy-MM-dd}"),
                new ReportParameter("AdjustEnd", $"{adjustEnd:yyy-MM-dd}"),
                new ReportParameter("AdjustDay", AdjustDate.Day.ToString())
            };
        }

        private List<ReportParameter> CreateReserveMedicinesSheetParametersA5()
        {
            var adjustEnd = AdjustDate.AddYears(-1911).AddDays(20);
            return new List<ReportParameter>
            {
                new ReportParameter("Type","預約"),
                new ReportParameter("PatientName",CusName),
                new ReportParameter("PatientBirthday",CusBirth.AddYears(-1911).ToString("yyy-MM-dd")),
                new ReportParameter("PatientTel",PhoneNote),
                new ReportParameter("Institution", InsName),
                new ReportParameter("Division", DivName),
                new ReportParameter("AdjustStart", $"{AdjustDate.AddYears(-1911):yyy-MM-dd}"),
                new ReportParameter("AdjustEnd", $"{adjustEnd:yyy-MM-dd}"),
                new ReportParameter("AdjustDay", AdjustDate.Day.ToString())
            };
        }

        private void DeleteOrder()
        {
            var orderIDTable = IndexReserveDb.GetOrderIDByResMasID(Id);
            if (orderIDTable.Rows.Count > 0)
            {
                var orderID = orderIDTable.Rows[0].Field<string>("StoOrd_ID");
                if (!string.IsNullOrEmpty(orderID))
                {
                    var removeSingdeOrder = StoreOrderDB.RemoveSingdeStoreOrderByID(orderID).Rows[0].Field<string>("RESULT").Equals("SUCCESS");
                    if (!removeSingdeOrder)
                        MessageWindow.ShowMessage("處方訂單已出貨或網路異常，訂單刪除失敗", MessageType.ERROR);
                    else
                    {
                        var dataTable = StoreOrderDB.RemoveStoreOrderToSingdeByID(orderID);
                        var removeLocalOrder = dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
                        if (!removeLocalOrder)
                            MessageWindow.ShowMessage("處方訂單刪除失敗，請至進退貨管理確認。", MessageType.ERROR);
                        else
                            StoreOrderDB.UpdateProductOnTheWay();
                    }
                }
            }
        }
    }
}