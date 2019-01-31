using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription.IcData;
using His_Pos.NewClass.Prescription.IcData.Upload;
using His_Pos.Service;
using Application = System.Windows.Application;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription
{
    public class IcCard : ObservableObject
    {
        public IcCard() { }
        public string CardNumber { get; set; }//卡片號碼
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string IDNumber { get; set; }//身分證字號
        public DateTime CardReleaseDate { get; set; }//發卡日期
        public DateTime ValidityPeriod { get; set; }//卡片有效期限
        public int? AvailableTimes { get; set; }//就醫可用次數
        public DateTime? NewBornBirthday { get; }//卡片有效期限
        public BasicData PatientBasicData { get; set; }
        public SeqNumber MedicalNumberData { get; set; }
        public TreatRecords TreatRecords { get; set; }
        public bool IsGetMedicalNumber { get; set; }
        public bool GetBasicData()
        {
            var strLength = 72;
            var icData = new byte[72];
            if (HisApiBase.OpenCom())
            {
                MainWindow.Instance.SetCardReaderStatus(StringRes.讀取健保卡);
                Thread.Sleep(800);
                var res = HisApiBase.hisGetBasicData(icData, ref strLength);
                if (res == 0)
                {
                    byte[] BasicDataArr = new byte[72];
                    MainWindow.Instance.SetCardReaderStatus(StringRes.讀取成功);
                    icData.CopyTo(BasicDataArr, 0);
                    PatientBasicData = new BasicData(icData);
                    CardNumber = PatientBasicData.CardNumber;
                    Name = PatientBasicData.Name;
                    Birthday = PatientBasicData.Birthday;
                    Gender = PatientBasicData.Gender;
                    IDNumber = PatientBasicData.IDNumber;
                    CardReleaseDate = PatientBasicData.CardReleaseDate;
                    HisApiBase.CloseCom();
                    return true;
                }
                var description = MainWindow.GetEnumDescription((ErrorCode)res);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MessageWindow.ShowMessage("取得健保卡基本資料異常 " + res + ":" + description, MessageType.WARNING);
                });
                HisApiBase.CloseCom();
            }
            return false;
        }
        public void GetMedicalNumber(byte makeUp)
        {
            byte[] cTreatItem = ConvertData.StringToBytes("AF\0", 3);//就醫類別長度3個char;
            byte[] cBabyTreat = ConvertData.StringToBytes(" ", 2);//新生兒就醫註記,長度2個char
            byte[] cTreatAfterCheck = { makeUp };//補卡註記
            int iBufferLen = 296;
            byte[] pBuffer = new byte[296];
            if (HisApiBase.OpenCom())
            {
                var res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, cTreatAfterCheck, pBuffer, ref iBufferLen);
                if (res == 0)
                {
                    MedicalNumberData = new SeqNumber(pBuffer);
                    IsGetMedicalNumber = true;
                }
                else
                {
                    var description = MainWindow.GetEnumDescription((ErrorCode)res);
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MessageWindow.ShowMessage("取得就醫序號異常" + res + ":" + description, MessageType.WARNING);
                    });
                }
                HisApiBase.CloseCom();
            }
        }

        public void GetTreatDataNoNeedHPC()
        {
            int iBufferLen = 498;
            byte[] pBuffer = new byte[498];
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (HisApiBase.OpenCom())
                {
                    var res = HisApiBase.hisGetTreatmentNoNeedHPC(pBuffer, ref iBufferLen);
                    if (res == 0)
                    {
                        TreatRecords = new TreatRecords(pBuffer);
                    }
                    HisApiBase.CloseCom();
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {

            };
            worker.RunWorkerAsync();
        }

        public void GetRegisterBasic()
        {
            byte[] pBuffer = new byte[9];
            var strLength = 9;
            if (HisApiBase.OpenCom())
            {
                var res = HisApiBase.hisGetRegisterBasic2(pBuffer, ref strLength);
                if (res == 0)
                {
                    ValidityPeriod = DateTimeExtensions.TWDateStringToDateOnly(Function.ByteArrayToString(7, pBuffer, 0));
                    AvailableTimes = int.Parse(Function.ByteArrayToString(2, pBuffer, 7));
                }
                else
                {
                    var registerBasicErr = MainWindow.GetEnumDescription((ErrorCode)res);
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MessageWindow.ShowMessage("取得就醫可用次數異常 " + res + ":" + registerBasicErr, MessageType.WARNING);
                    });
                }
                HisApiBase.CloseCom();
            }
        }

        public void UpdateCard()
        {
            int res = -1;
            if (AvailableTimes != 0) return;
            if (HisApiBase.OpenCom())
            {
                res = HisApiBase.csUpdateHCContents();
                if (res != 0)
                {
                    var registerBasicErr = MainWindow.GetEnumDescription((ErrorCode)res);
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MessageWindow.ShowMessage("更新卡片異常 " + res + ":" + registerBasicErr, MessageType.WARNING);
                    });
                }
                HisApiBase.CloseCom();
            }
        }
    }
}
