using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass;
using System.Collections.ObjectModel;

namespace His_Pos.FunctionWindow.ErrorUploadWindow
{
    public class ErrorUploadWindowViewModel : ViewModelBase
    {
        public class IcErrorCode
        {
            public string ID { get; set; }
            public string Content { get; set; }

            public IcErrorCode(string id, string content)
            {
                ID = id;
                Content = content;
            }

            public IcErrorCode()
            {
            }
        }

        public class IcErrorCodes : Collection<IcErrorCode>
        {
            public IcErrorCodes(bool isMedicalNumberGet)
            {
                Init(isMedicalNumberGet);
            }

            private void Init(bool isMedicalNumberGet)
            {
                if (isMedicalNumberGet)
                {
                    Add(new IcErrorCode("A001", "讀卡設備故障"));
                    Add(new IcErrorCode("A011", "讀卡機故障"));
                    Add(new IcErrorCode("A021", "網路故障造成讀卡機無法使用"));
                    Add(new IcErrorCode("A031", "安全模組故障造成讀卡機無法使用"));
                    Add(new IcErrorCode("B001", "卡片不良 (表面正常, 晶片異常)"));
                    Add(new IcErrorCode("D001", "醫療資訊系統(HIS)當機"));
                    Add(new IcErrorCode("D011", "醫療院所電腦故障"));
                }
                else
                {
                    Add(new IcErrorCode("A001", "讀卡設備故障"));
                    Add(new IcErrorCode("A010", "讀卡機故障"));
                    Add(new IcErrorCode("A020", "網路故障造成讀卡機無法使用"));
                    Add(new IcErrorCode("A030", "安全模組故障造成讀卡機無法使用"));
                    Add(new IcErrorCode("B000", "卡片不良 (表面正常, 晶片異常)"));
                    Add(new IcErrorCode("C000", "停電"));
                    Add(new IcErrorCode("C001", "例外就醫者（首次加保 1 個月內、換補發卡 14 日內)"));
                    Add(new IcErrorCode("C002", "20 歲以下兒少例外就醫"));
                    Add(new IcErrorCode("C003", "懷孕婦女例外就醫"));
                    Add(new IcErrorCode("D000", "醫療資訊系統(HIS)當機"));
                    Add(new IcErrorCode("D010", "醫療院所電腦故障"));
                    Add(new IcErrorCode("E000", "健保署資訊系統當機"));
                    Add(new IcErrorCode("E001", "控卡名單已簽切結書"));
                    Add(new IcErrorCode("F000", "醫事機構赴偏遠地區因無電話撥接"));
                    Add(new IcErrorCode("G000", "新特約使用"));
                }
            }
        }

        public IcErrorCodes IcErrorCodeList { get; set; }
        private IcErrorCode selectedIcErrorCode;

        public IcErrorCode SelectedIcErrorCode
        {
            get => selectedIcErrorCode;
            set
            {
                Set(() => SelectedIcErrorCode, ref selectedIcErrorCode, value);
            }
        }

        public RelayCommand ConfirmClick { get; set; }
        public RelayCommand CancelClick { get; set; }

        public ErrorUploadWindowViewModel(bool isGetMedicalNumber)
        {
            IcErrorCodeList = new IcErrorCodes(isGetMedicalNumber);
            ConfirmClick = new RelayCommand(ConfirmClickAction);
            CancelClick = new RelayCommand(CancelClickAction);
        }

        private void ConfirmClickAction()
        {
            if (SelectedIcErrorCode is null)
            {
                MessageWindow.ShowMessage("尚未選擇異常代碼", MessageType.ERROR);
                return;
            }
            Messenger.Default.Send(SelectedIcErrorCode, "SelectedIcErrorCode");
            Messenger.Default.Send(new NotificationMessage("CloseErrorUploadWindow"));
        }

        private void CancelClickAction()
        {
            SelectedIcErrorCode = null;
            Messenger.Default.Send(new NotificationMessage("CloseErrorUploadWindow"));
        }
    }
}