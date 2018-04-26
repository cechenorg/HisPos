using System.Text;

namespace His_Pos.HisApi
{
    public class HisApiFunction
    {
        public string VerifySamdc() {   //安全模組認證
            string msg = string.Empty;
            int comConnection =  HisApiBase.csOpenCom(0);
            if(comConnection == -1) msg = "開啟Com失敗";
            if (comConnection == 0) {
                int samdcStatus = HisApiBase.csVerifySAMDC();
                switch (samdcStatus)
                {
                    case 0:
                        msg = "Success";
                        break;
                    case 4012:
                        msg = "未置入安全模組卡";
                        break;
                    case 4032:
                        msg = "所插入非安全模組卡";
                        break;
                    case 4051:
                        msg = "安全模組與IDC認證失敗";
                        break;
                    case 4052:
                        msg = "Bnhihost.ini 檔案無法開啟";
                        break;
                    case 4053:
                        msg = "Bnhihost.ini 檔案內容有誤";
                        break;
                    case 4061:
                        msg = "網路連線失敗";
                        break;
                    case 6005:
                        msg = "安全模組卡的外部認證失敗";
                        break;
                    case 6006:
                        msg = "IDC的外部認證失敗";
                        break;
                    case 6007:
                        msg = "安全模組卡的內部認證失敗";
                        break;
                    case 6008:
                        msg = "寫入讀卡機日期時間失敗";
                        break;
                }
            }
            HisApiBase.csCloseCom();
            return msg;
        }//VerifySAMDC()


        public string VerifyHpcpin() {    //檢查醫事人員PIN值
            string msg = string.Empty;
            int comConnection = HisApiBase.csOpenCom(0);
            if (comConnection == -1) msg = "開啟Com失敗";
            if (comConnection == 0) {
                int hpcStatus = HisApiBase.hpcVerifyHPCPIN();
                switch (hpcStatus)
                {
                    case 0:
                        msg = "Success";
                        break;
                    case 4000:
                        msg = "讀卡機timeout";
                        break;
                    case 4014:
                        msg = "未置入醫事人員卡";
                        break;
                    case 4029:
                        msg = "IC卡權限不足";
                        break;
                    case 4034:
                        msg = "所置入非醫事人員卡";
                        break;
                    case 4050:
                        msg = "安全模組尚未與IDC認證";
                        break;
                }
            }
            HisApiBase.csCloseCom();
            return msg;
        } //VerifyHPCPIN()



        public string GetBasicData() {
            string data = string.Empty;
            int comConnection = HisApiBase.csOpenCom(0);
            HisApiBase.csCloseCom();
            return data;
        }//GetBasicData()
        private string ByteSubStr(string strInput, int startIndex, int byteLength)
        {
            var lEncoding = Encoding.GetEncoding("big5", new EncoderExceptionFallback(), new DecoderReplacementFallback(""));
            var lByte = lEncoding.GetBytes(strInput);
            if (byteLength <= 0)
                return "";
            if (startIndex + 1 > lByte.Length)
                return "";
            if (startIndex + byteLength > lByte.Length)
                byteLength = lByte.Length - startIndex;
            return lEncoding.GetString(lByte, startIndex, byteLength);
        }

        public string GetIcData(StringBuilder data, int startIndex, int length)
        {
            return ByteSubStr(data.ToString(), startIndex, length);
        }

        public void ErrorDetect(int _res)
        {
            var _message = new StringBuilder(100);
            var errorDescription = new Function();
            _message.Clear();
            if (_res == 0)
                return;
            var errorCodrDescription = errorDescription.GetEnumDescription("ErrorCode", _res.ToString());
            _message.Append(errorCodrDescription);
        }
    }
}
