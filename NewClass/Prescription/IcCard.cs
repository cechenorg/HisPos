using System;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.HisApi;
using His_Pos.Struct.IcData;
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
        public DateTime ValidityPeriod { get;}//卡片有效期限
        public int AvailableTimes { get;}//就醫可用次數
        public DateTime? NewBornBirthday { get; }//卡片有效期限
        public bool GetBasicData()
        {
            if (ViewModelMainWindow.IsVerifySamDc)
            {
                var strLength = 72;
                var icData = new byte[72];
                if (HisApiBase.OpenCom())
                {
                    MainWindow.Instance.SetCardReaderStatus(StringRes.讀取健保卡);
                    var res = HisApiBase.hisGetBasicData(icData, ref strLength);
                    if (res == 0)
                    {
                        byte[] BasicDataArr = new byte[72];
                        MainWindow.Instance.SetCardReaderStatus(StringRes.讀取成功);
                        icData.CopyTo(BasicDataArr, 0);
                        BasicData basic = new BasicData(icData);
                        HisApiBase.CloseCom();
                        CardNumber = basic.CardNumber;
                        Name = basic.Name;
                        Birthday = basic.Birthday;
                        Gender = basic.Gender;
                        IDNumber = basic.IDNumber;
                        CardReleaseDate = basic.CardReleaseDate;
                        return true;
                    }
                    HisApiBase.CloseCom();
                }
                return false;
            }
            else
            {
                MainWindow.Instance.SetCardReaderStatus("安全模組未認證");
                return false;
            }
        }
    }
}
