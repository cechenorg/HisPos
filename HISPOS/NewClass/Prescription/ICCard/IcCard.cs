using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription.ICCard.Upload;
using His_Pos.Service;
using System;
using System.ComponentModel;
using System.Threading;
using ZeroFormatter;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription.ICCard
{
    [ZeroFormattable]
    public class IcCard : ObservableObject, ICloneable
    {
        public IcCard()
        {
        }

        [Index(0)]
        public virtual string CardNumber { get; set; }//卡片號碼

        [Index(1)]
        public virtual string Name { get; set; }

        [Index(2)]
        public virtual DateTime Birthday { get; set; }

        [Index(3)]
        public virtual string Gender { get; set; }

        [Index(4)]
        public virtual string IDNumber { get; set; }//身分證字號

        [Index(5)]
        public virtual DateTime CardReleaseDate { get; set; }//發卡日期

        [Index(6)]
        public virtual DateTime ValidityPeriod { get; set; }//卡片有效期限

        [Index(7)]
        public virtual int? AvailableTimes { get; set; }//就醫可用次數

        [Index(8)]
        public virtual string NewBornBirthday { get; set; }//卡片有效期限

        [Index(9)]
        public virtual BasicData PatientBasicData { get; set; }

        [Index(10)]
        public virtual SeqNumber MedicalNumberData { get; set; }

        [IgnoreFormat]
        public virtual TreatRecords TreatRecords { get; set; }

        [Index(11)]
        public virtual bool IsGetMedicalNumber { get; set; }

        [Index(12)]
        public virtual string TreatDateTime { get; set; } = string.Empty;

        [Index(13)]
        public virtual string Tel { get; set; }

        [Index(14)]
        public virtual bool IsRead { get; set; }

        public bool Read()
        {
            if (HisApiFunction.OpenCom())
            {
                MainWindow.Instance.SetCardReaderStatus(StringRes.讀取健保卡);
                CheckCardStatus();
                IsRead = GetBasicData();
                return true;
            }
            return false;
        }

        private bool GetBasicData()
        {
            var strLength = 72;
            var icData = new byte[72];
            var res = HisApiBase.hisGetBasicData(icData, ref strLength);
            if (res == 0)
            {
                var basicDataArr = new byte[72];
                MainWindow.Instance.SetCardReaderStatus(StringRes.讀取成功);
                icData.CopyTo(basicDataArr, 0);
                SetBasicData(icData);
                HisApiFunction.CloseCom();
                return true;
            }
            ShowHISAPIErrorMessage(res, "取得健保卡基本資料異常 ");
            HisApiFunction.CloseCom();
            return false;
        }

        private void CheckCardStatus()
        {
            var cardStatus = HisApiBase.hisGetCardStatus(2);
            if (cardStatus != 2)
                Thread.Sleep(1500);
        }

        private void SetBasicData(byte[] icData)
        {
            PatientBasicData = new BasicData(icData);
            CardNumber = PatientBasicData.CardNumber;
            Name = PatientBasicData.Name;
            Birthday = PatientBasicData.Birthday;
            Gender = PatientBasicData.Gender;
            IDNumber = PatientBasicData.IDNumber;
            CardReleaseDate = PatientBasicData.CardReleaseDate;
            Tel = PatientBasicData.Tel;
        }

        public int GetMedicalNumber(byte makeUp)
        {
            byte[] cTreatItem = ConvertData.StringToBytes("AF\0", 3);//就醫類別長度3個char;
            byte[] cBabyTreat = ConvertData.StringToBytes(" ", 2);//新生兒就醫註記,長度2個char
            byte[] cTreatAfterCheck = { makeUp };//補卡註記
            int iBufferLen = 296;
            byte[] pBuffer = new byte[296];
            if (HisApiFunction.OpenCom())
            {
                var res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, cTreatAfterCheck, pBuffer, ref iBufferLen);
                switch (res)
                {
                    case 0:
                        MedicalNumberData = new SeqNumber(pBuffer);
                        IsGetMedicalNumber = true;
                        TreatDateTime = DateTimeExtensions.ToStringWithSecond(MedicalNumberData.TreatDateTime);
                        break;

                    case 5003:
                        UpdateCard();
                        GetMedicalNumber(makeUp);
                        break;

                    default:
                        ShowHISAPIErrorMessage(res, "取得就醫序號異常 ");
                        break;
                }
                HisApiFunction.CloseCom();
                return res;
            }
            MessageWindow.ShowMessage("讀卡機連接開啟失敗", MessageType.ERROR);
            return -1;
        }

        public void GetTreatDataNoNeedHPC()
        {
            var iBufferLen = 498;
            var pBuffer = new byte[498];
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (HisApiFunction.OpenCom())
                {
                    var res = HisApiBase.hisGetTreatmentNoNeedHPC(pBuffer, ref iBufferLen);
                    if (res == 0)
                    {
                        TreatRecords = new TreatRecords(pBuffer);
                    }
                    HisApiFunction.CloseCom();
                }
            };
            worker.RunWorkerAsync();
        }

        public void GetRegisterBasic()
        {
            var pBuffer = new byte[9];
            var strLength = 9;
            if (HisApiFunction.OpenCom())
            {
                var res = HisApiBase.hisGetRegisterBasic2(pBuffer, ref strLength);
                if (res == 0)
                {
                    ValidityPeriod = (DateTime)DateTimeExtensions.TWDateStringToDateOnly(Function.ByteArrayToString(7, pBuffer, 0));
                    AvailableTimes = int.Parse(Function.ByteArrayToString(2, pBuffer, 7));
                }
                else
                    ShowHISAPIErrorMessage(res, "取得就醫可用次數異常 ");
                HisApiFunction.CloseCom();
            }
        }

        public void UpdateCard()
        {
            if (HisApiFunction.OpenCom())
            {
                var res = HisApiBase.csUpdateHCContents();
                if (res != 0)
                    ShowHISAPIErrorMessage(res, "更新卡片異常 ");
                HisApiFunction.CloseCom();
            }
        }

        private void ShowHISAPIErrorMessage(int res, string title)
        {
            var description = MainWindow.GetEnumDescription((ErrorCode)res);
            NewFunction.ShowMessageFromDispatcher($"{title + res}:{description}", MessageType.WARNING);
        }

        public object Clone()
        {
            var c = new IcCard();
            c.CardNumber = CardNumber;
            c.Name = Name;
            c.Birthday = Birthday;
            c.Gender = Gender;
            c.IDNumber = IDNumber;
            c.CardReleaseDate = CardReleaseDate;
            c.ValidityPeriod = ValidityPeriod;
            c.AvailableTimes = AvailableTimes;
            c.NewBornBirthday = NewBornBirthday;
            c.PatientBasicData = PatientBasicData.DeepCloneViaJson();
            c.MedicalNumberData = MedicalNumberData.DeepCloneViaJson();
            c.TreatRecords = TreatRecords?.DeepCloneViaJson();
            c.IsGetMedicalNumber = IsGetMedicalNumber;
            c.TreatDateTime = TreatDateTime;
            c.Tel = Tel;
            return c;
        }

        public bool CheckNeedUpdate()
        {
            if (AvailableTimes is null) return false;
            var availableTimesUseUp = AvailableTimes == 0;
            var expired = new TimeSpan(ValidityPeriod.Ticks - DateTime.Today.Ticks).Days < 30;
            return availableTimesUseUp || expired;
        }

        public string GetLastMedicalNumber()
        {
            var tempMedicalNumber = string.Empty;
            if (HisApiFunction.OpenCom())
            {
                int iBufferLen = 7;
                byte[] pBuffer = new byte[7];
                var res = HisApiBase.hisGetLastSeqNum(pBuffer, ref iBufferLen);
                if (res == 0)
                    tempMedicalNumber = Function.ByteArrayToString(4, pBuffer, 3);
                HisApiFunction.CloseCom();
            }
            return tempMedicalNumber;
        }
    }
}