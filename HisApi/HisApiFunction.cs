using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Prescription.IcData.Upload;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Properties;
using His_Pos.Service;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
// ReSharper disable All

namespace His_Pos.HisApi
{
    public class HisApiFunction
    {
        public static List<string> WritePrescriptionData(Prescription p)
        {
            p.WriteCardSuccess = -1;
            var signList = new List<string>();
            var medList = p.Medicines.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf).ToList();
            var iWriteCount = medList.Count;
            var iBufferLength = 40 * iWriteCount;
            var treatDateTime = DateTimeExtensions.ToStringWithSecond(p.Card.MedicalNumberData.TreatDateTime);
            var pDataWriteStr = p.Medicines.CreateMedicalData(treatDateTime);
            byte[] pDateTime = ConvertData.StringToBytes(treatDateTime+"\0",14);
            byte[] pPatientID = ConvertData.StringToBytes(p.Card.PatientBasicData.IDNumber + "\0", 11);
            byte[] pPatientBirthDay = ConvertData.StringToBytes(p.Card.PatientBasicData.BirthdayStr + "\0", 8);
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
                        signList.Add(ConvertData.ByToString(pBuffer, startIndex,40));
                        startIndex += 40;
                    }
                }
                CloseCom();
            }
            return signList;
        }
        //正常上傳
        public static void CreatDailyUploadData(Prescription p, bool isMakeUp)
        {
            Rec rec = new Rec(p, isMakeUp);
            var uploadData = rec.SerializeDailyUploadObject();
            MainWindow.ServerConnection.OpenConnection();
            IcDataUploadDb.InsertDailyUploadData(p.Id, uploadData, p.Card.MedicalNumberData.TreatDateTime);
            MainWindow.ServerConnection.CloseConnection();
        }

        //異常上傳
        public static void CreatErrorDailyUploadData(Prescription p, bool isMakeUp ,ErrorUploadWindowViewModel.IcErrorCode e = null)
        {
            Rec rec = new Rec(p, isMakeUp, e);
            var uploadData = rec.SerializeDailyUploadObject();
            MainWindow.ServerConnection.OpenConnection();
            IcDataUploadDb.InsertDailyUploadData(p.Id, uploadData, DateTime.Now);
            MainWindow.ServerConnection.CloseConnection();
            Console.WriteLine(uploadData);
        }

        public static bool OpenCom()
        {
            SetCardReaderStatus(Resources.開啟讀卡機);
            var res = HisApiBase.csOpenCom(ViewModelMainWindow.CurrentPharmacy.ReaderCom);
            SetStatus(res == 0, 1);
            MainWindow.Instance.SetCardReaderStatus(res == 0 ? Resources.連接成功 : Resources.連接失敗);
            return res == 0;
        }

        public static void CloseCom()
        {
            if (HisApiBase.csCloseCom() == 0)
                SetStatus(false, 1);
        }

        public static void CheckCardStatus(int type)
        {
            int res = 4000;
            SetCardReaderStatus(Resources.讀卡機異常);
            OpenCom();
            string status;
            switch (type)
            {
                case 1:
                    status = Resources.檢查安全模組;
                    break;
                case 2:
                    status = Resources.檢查健保卡;
                    break;
                case 3:
                    status = Resources.檢查醫事人員卡;
                    break;
                default:
                    status = Resources.檢查中;
                    break;
            }
            MainWindow.Instance.SetCardReaderStatus(status);
            try
            {
                res = HisApiBase.hisGetCardStatus(type);
            }
            catch (Exception e)
            {
                ShowMessage(Resources.控制軟體異常);
            }
            switch (type)
            {
                case 1:
                    switch (res)
                    {
                        case 4000:
                            MainWindow.Instance.SetSamDcStatus(Resources.讀卡機逾時);
                            SetStatus(false, 2);
                            break;
                        case 1:
                            MainWindow.Instance.SetHpcCardStatus(Resources.未認證);
                            SetStatus(false, 2);
                            break;
                        case 2:
                            MainWindow.Instance.SetHpcCardStatus(Resources.認證成功);
                            SetStatus(true, 2);
                            break;
                        default:
                            MainWindow.Instance.SetHpcCardStatus(Resources.所置入非安全模組);
                            SetStatus(false, 2);
                            break;
                    }
                    break;
                case 2:
                    SetStatus(res == 2, 4);
                    MainWindow.Instance.SetCardReaderStatus(Resources.讀取失敗);
                    break;
                case 3:
                    switch (res)
                    {
                        case 2:
                            MainWindow.Instance.SetHpcCardStatus(Resources.認證成功未驗);
                            ViewModelMainWindow.IsHpcValid = true;
                            break;
                        case 3:
                            MainWindow.Instance.SetHpcCardStatus(Resources.認證成功已驗);
                            ViewModelMainWindow.IsHpcValid = true;
                            break;
                        default:
                            MainWindow.Instance.SetHpcCardStatus(Resources.未認證);
                            ViewModelMainWindow.IsHpcValid = true;
                            break;
                    }
                    break;
            }
            CloseCom();
        }

        public static void VerifySamDc()
        {
            SetCardReaderStatus(Resources.讀卡機異常);
            bool status;
            int res = 0;
            MainWindow.Instance.SetSamDcStatus(Resources.健保局連線中);
            if (!OpenCom())
            {
                SetStatus(false, 2);
            }
            try
            {
                res = HisApiBase.csVerifySAMDC();
            }
            catch (Exception e)
            {
                ShowMessage(Resources.控制軟體異常);
            }
            if (res == 0)
            {
                status = true;
                MainWindow.Instance.SetSamDcStatus(Resources.認證成功);
                SetStatus(status, 2);
            }
            else
            {
                status = false;
                MainWindow.Instance.SetSamDcStatus(Resources.未認證);
                SetStatus(status, 2);
            }
            CloseCom();
        }

        public static void ResetCardReader()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                ViewModelMainWindow.HisApiException = false;
            });
            SetStatus(false, 1);
            SetStatus(false, 2);
            SetStatus(false, 3);
            bool isPassed = false;
            Application.Current.Dispatcher.Invoke(delegate { isPassed = OpenCom(); });
            if (!isPassed)
                return;
            HisApiBase.csSoftwareReset(3);
            CloseCom();
            VerifySamDc();
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
            catch (Exception e)
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
            Application.Current.Dispatcher.Invoke(delegate {
                ViewModelMainWindow.HisApiException = true;
                MessageWindow.ShowMessage(message, MessageType.ERROR);
            });
        }
    }
}
