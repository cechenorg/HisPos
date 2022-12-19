using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.ICCard;
using His_Pos.NewClass.Prescription.ICCard.Upload;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;

// ReSharper disable All

namespace His_Pos.HisApi
{
    public class HisApiFunction
    {
        public static List<string> WritePrescriptionData(Prescription p)
        {
            p.WriteCardSuccess = -1;
            var signList = new List<string>();
            var medList = p.Medicines.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf).ToList();
            var iWriteCount = medList.Count;
            var iBufferLength = 40 * iWriteCount;
            p.Card.Read();
            var treatDateTime = DateTimeExtensions.ToStringWithSecond(p.Card.MedicalNumberData.TreatDateTime);
            var pDataWriteStr = p.Medicines.CreateMedicalData(treatDateTime);
            byte[] pDateTime = ConvertData.StringToBytes(treatDateTime + "\0", (treatDateTime + "\0").Length);
            byte[] pPatientID = ConvertData.StringToBytes(p.Card.PatientBasicData.IDNumber + "\0", (p.Card.PatientBasicData.IDNumber + "\0").Length);
            byte[] pPatientBirthDay = ConvertData.StringToBytes(p.Card.PatientBasicData.BirthdayStr + "\0", (p.Card.PatientBasicData.BirthdayStr + "\0").Length);
            byte[] pDataWrite = ConvertData.StringToBytes(pDataWriteStr, 3660);
            byte[] pBuffer = new byte[iBufferLength];
            if (OpenCom())
            {
                var res = HisApiBase.hisWriteMultiPrescriptSign(pDateTime, pPatientID, pPatientBirthDay, pDataWrite, ref iWriteCount, pBuffer, ref iBufferLength);
                p.WriteCardSuccess = res;
                if (res == 0)
                {
                    var startIndex = 0;
                    for (int i = 0; i < iWriteCount; i++)
                    {
                        signList.Add(ConvertData.ByToString(pBuffer, startIndex, 40));
                        startIndex += 40;
                    }
                }
                CloseCom();
            }
            return signList;
        }

        //正常上傳
        public static DataTable CreatDailyUploadData(Prescription p, bool isMakeUp)
        {
            DataTable table;
            IcDataUploadService.Rec rec1 = new IcDataUploadService.Rec(p, isMakeUp);
            var uploadData1 = rec1.SerializeDailyUploadObject();
            MainWindow.ServerConnection.OpenConnection();
            table = InsertUploadData(p, uploadData1, p.Card.MedicalNumberData.TreatDateTime);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }

        //異常上傳
        public static DataTable CreatErrorDailyUploadData(Prescription p, bool isMakeUp, ErrorUploadWindowViewModel.IcErrorCode e = null)
        {
            DataTable table;
            IcDataUploadService.Rec rec1 = new IcDataUploadService.Rec(p, isMakeUp, e);
            var uploadData1 = rec1.SerializeDailyUploadObject();
            MainWindow.ServerConnection.OpenConnection();
            table = InsertUploadData(p, uploadData1, DateTime.Now);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }

        public static bool OpenCom()
        {
            SetCardReaderStatus(Resources.開啟讀卡機);
            var res = HisApiBase.csOpenCom(Convert.ToInt32(Settings.Default.ReaderComPort));
            SetStatus(res == 0, 1);
            MainWindow.Instance.SetCardReaderStatus(res == 0 ? Resources.連接成功 : Resources.連接失敗);
            return res == 0;
        }

        public static void CloseCom()
        {
            if (HisApiBase.csCloseCom() == 0)
                SetStatus(false, 1);
        }

        public static bool VerifySamDc()
        {
            bool status;
            MainWindow.Instance.SetSamDcStatus(Resources.健保局連線中);
            try
            {
                if (OpenCom())
                {
                    MainWindow.Instance.SetSamDcStatus(Resources.認證安全模組);
                    int res = HisApiBase.csVerifySAMDC();
                    CloseCom();
                    if (res == 0)
                    {
                        status = true;
                        MainWindow.Instance.SetSamDcStatus(Resources.認證成功);
                        SetStatus(status, 2);
                        return true;
                    }
                    else
                    {
                        status = false;
                        var description = Resources.認證失敗 + ":" + MainWindow.GetEnumDescription((ErrorCode)res);
                        MainWindow.Instance.SetSamDcStatus(description);
                        SetStatus(status, 2);
                        return false;
                    }
                }
                SetStatus(false, 2);
                return false;
            }
            catch (Exception)
            {
                ShowMessage(Resources.控制軟體異常);
                return false;
            }
        }

        public static void SetStatus(bool status, int type)
        {
            void Status()
            {
                switch (type)
                {
                    case 1:
                        ViewModelMainWindow.IsConnectionOpened = status;
                        break;

                    case 2:
                        ViewModelMainWindow.IsVerifySamDc = status;
                        break;

                    case 3:
                        ViewModelMainWindow.IsHpcValid = status;
                        break;

                    case 4:
                        status = ViewModelMainWindow.IsIcCardValid;
                        break;
                }
            }
            MainWindow.Instance.Dispatcher.BeginInvoke((Action)Status);
        }

        public static bool GetStatus(int type)
        {
            bool status = false;
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                switch (type)
                {
                    case 1:
                        status = ViewModelMainWindow.IsConnectionOpened;
                        break;

                    case 2:
                        status = ViewModelMainWindow.IsVerifySamDc;
                        break;

                    case 3:
                        status = ViewModelMainWindow.IsHpcValid;
                        break;

                    case 4:
                        status = ViewModelMainWindow.IsIcCardValid;
                        break;
                }
            });
            return status;
        }

        public static void VerifyHpcPin()
        {
            int res = 0;
            MainWindow.Instance.SetCardReaderStatus(Resources.驗證醫事人員卡);
            if (!OpenCom())
                return;
            try
            {
                res = HisApiBase.hpcVerifyHPCPIN();
            }
            catch (Exception)
            {
                ShowMessage(Resources.控制軟體異常);
            }

            if (res == 0)
            {
                MainWindow.Instance.SetHpcCardStatus(Resources.認證成功);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ViewModelMainWindow.IsHpcValid = true;
                });
            }
            else
            {
                MainWindow.Instance.SetHpcCardStatus(Resources.認證失敗);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ViewModelMainWindow.IsHpcValid = false;
                });
            }
            CloseCom();
        }

        private static void SetCardReaderStatus(string message)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (ViewModelMainWindow.HisApiException)
                {
                    MainWindow.Instance.SetCardReaderStatus(message);
                }
            });
        }

        private static void ShowMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ViewModelMainWindow.HisApiException = true;
                MessageWindow.ShowMessage(message, MessageType.ERROR);
            });
        }

        public static void CheckDailyUpload()
        {
            if (ViewModelMainWindow.CurrentPharmacy.NewInstitution) return;
            var uploadTable = UploadFunctions.CheckUpload();
            if (uploadTable.Rows.Count > 0 && ViewModelMainWindow.IsVerifySamDc)
            {
                var dailyUploadConfirm = new ConfirmWindow("尚有" + uploadTable.Rows.Count + "筆健保資料未上傳，是否執行上傳作業", "每日上傳確認", true);
                bool upload = (bool)dailyUploadConfirm.DialogResult;
                if (upload)
                    UploadFunctions.StartDailyUpload(uploadTable);
            }
        }

        public static void CheckDailyUpload100()
        {
            if (ViewModelMainWindow.CurrentPharmacy.NewInstitution) return;
            var uploadTable = UploadFunctions.CheckUpload();
            if (uploadTable.Rows.Count >= 100 && ViewModelMainWindow.IsVerifySamDc)
            {
                UploadFunctions.StartDailyUpload100(uploadTable);
            }
        }

        private static DataTable InsertUploadData(Prescription p, string uploadData, DateTime treat)
        {
            var table = IcDataUploadDb.InsertDailyUploadData(p.ID, uploadData, treat);
            while (NewFunction.CheckTransaction(table))
            {
                MessageWindow.ShowMessage("寫卡資料存檔異常，按下OK重試", MessageType.WARNING);
                table = IcDataUploadDb.InsertDailyUploadData(p.ID, uploadData, treat);
            }
            return table;
        }

        public static string GetIcData2(Prescription p, bool isMakeUp)
        {
            var uploadData2 = string.Empty;
            try
            {
                IcDataUploadService2.Recs recs2 = new IcDataUploadService2.Recs();
                IcDataUploadService2.Rec rec2 = new IcDataUploadService2.Rec(p, isMakeUp);
                recs2.Rec = new List<IcDataUploadService2.Rec> { rec2 };
                int i = 1;
                foreach (IcDataUploadService2.Rec rc in recs2.Rec)
                {
                    foreach (IcDataUploadService2.MedicalData md in rc.MainMessage.MedicalMessageList)
                    {
                        md.MedicalRow = i;
                        i++;
                    }
                }
                uploadData2 = recs2.SerializeDailyUploadObject();
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage(e.Message, MessageType.ERROR);
            }
            return uploadData2;
        }

        public static SeqNumber GetSeqNumber256N1()
        {
            byte[] cTreatItem = ConvertData.StringToBytes("AF\0", 3);
            byte[] cBabyTreat = ConvertData.StringToBytes(" ", 2);
            byte[] cTreatAfterCheck = { new byte() };
            int iBufferLen = 316;
            byte[] pBuffer = new byte[316];
            var res = HisApiBase.hisGetSeqNumber256N1(cTreatItem, cBabyTreat, cTreatAfterCheck, pBuffer, ref iBufferLen);
            IcCard card = new IcCard();
            card.MedicalNumberData = new SeqNumber(pBuffer);
            return card.MedicalNumberData;
        }
    }
}