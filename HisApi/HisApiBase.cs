using System;
using System.Runtime.InteropServices;
using System.Windows;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.HisApi
{
    public class HisApiBase
    {
        // 1.1 讀取不需個人PIN碼資料
        [DllImport("csHis.dll")]
        public static extern int hisGetBasicData(byte[] pBuffer, ref int iBufferLen);
        // 1.2 掛號或報到時讀取基本資料
        [DllImport("csHis.dll")]
        public static extern int hisGetRegisterBasic(byte[] pBuffer, ref int iBufferLen);
        // 1.3 預防保健掛號作業
        [DllImport("csHis.dll")]
        public static extern int hisGetRegisterPrevent(byte[] pBuffer, ref int intpBufferLen);
        // 1.4 孕婦產前檢查掛號作業
        [DllImport("csHis.dll")]
        public static extern int hisGetRegisterPregnant(byte[] pBuffer, ref int iBufferLen);
        // 1.5 讀取就醫資料不需HPC卡的部份
        [DllImport("csHis.dll")]
        public static extern int hisGetTreatmentNoNeedHPC(byte[] strpBuffer, ref int intpBufferLen);
        // 1.6 讀取就醫累計資料
        [DllImport("csHis.dll")]
        public static extern int hisGetCumulativeData(byte[] pBuffer, ref int iBufferLen);
        // 1.7 讀取醫療費用線累計
        [DllImport("csHis.dll")]
        public static extern int hisGetCumulativeFee(byte[] strpBuffer, ref int intpBufferLen);
        // 1.8 讀取就醫資料需HPC卡的部份
        [DllImport("csHis.dll")]
        public static extern int hisGetTreatmentNeedHPC(byte[] pBuffer, ref int iBufferLen);
        // 1.9 取得就醫序號  strpBuffer欄位宣告原本VB為As Any  C#要改成string and long
        [DllImport("csHis.dll")]
        public static extern int hisGetSeqNumber(byte[] cTreatItem, byte[] cBabyTreat, byte[] strpBuffer, ref int intpBufferLen);
        // 1.10 讀取處方箋作業
        [DllImport("csHis.dll")]
        public static extern int hisReadPrescription(byte[] pOutpatientPrescription, ref int iBufferLenOutpatient, byte[] pLongTermPrescription, ref int iBufferLenLongTerm, byte[] pImportantTreatmentCode, ref int iBufferLenImportant, byte[] pIrritationDrug, ref int iBufferLenIrritation);
        // 1.11 讀取預防接種資料
        [DllImport("csHis.dll")]
        public static extern int hisGetInoculateData(byte[] strpBuffer, ref int intpBufferLen);
        // 1.12 讀取器官捐贈資料
        [DllImport("csHis.dll")]
        public static extern int hisGetOrganDonate(byte[] pBuffer, ref int iBufferLen);
        // 1.13 讀取緊急聯絡電話資料
        [DllImport("csHis.dll")]
        public static extern int HisGetEmergentTel(byte[] pBuffer, ref int iBufferLen);
        // 1.14 讀取最近一次就醫序號
        [DllImport("csHis.dll")]
        public static extern int hisGetLastSeqNum(byte[] pBuffer, ref int iBufferLen);
        // 1.15 讀取卡片狀態
        [DllImport("csHis.dll")]
        public static extern int hisGetCardStatus(int CardType);
        // 1.16 就醫診療資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteTreatmentCode(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID);
        // 1.17 就醫費用資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteTreatmentFee(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite);
        // 1.18 處方箋寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWritePrescription(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite);
        // 1.19 新生兒註記寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteNewBorn(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pNewBornDate, byte[] pNoOfDelivered);
        // 1.20 過敏藥物寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteAllergicMedicines(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID);
        // 1.21 同意器官捐贈及安寧緩和醫療註記寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteOrganDonate(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pOrganDonate);
        // 1.22 預防保健資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteHealthInsurance(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pServiceItem, byte[] pServiceItemCode);
        // 1.23 緊急聯絡電話資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteEmergentTel(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pEmergentTel);
        // 1.24 寫入產前檢查資料
        [DllImport("csHis.dll")]
        public static extern int hisWritePredeliveryCheckup(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pCheckupCode);
        // 1.25 清除產前檢查資料
        [DllImport("csHis.dll")]
        public static extern int hisDeletePredeliveryData(byte[] pPatientID, byte[] pPatientBirthDate);
        // 1.26 預防接種資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteInoculateData(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pItem, byte[] pPackageNumber);
        // 1.27 驗證健保IC卡之PIN值
        [DllImport("csHis.dll")]
        public static extern int csVerifyHCPIN();
        // 1.28 輸入新的健保IC卡PIN值
        [DllImport("csHis.dll")]
        public static extern int csInputHCPIN();
        // 1.29 停用健保IC卡之PIN碼輸入功能
        [DllImport("csHis.dll")]
        public static extern int csDisableHCPIN();
        // 1.30 健保IC卡卡片內容更新作業
        [DllImport("csHis.dll")]
        public static extern int csUpdateHCContents();
        // 1.31 開啟讀卡機連結埠
        [DllImport("csHis.dll")]
        public static extern int csOpenCom(int pcom);
        // 1.32 關閉讀卡機連結埠
        [DllImport("csHis.dll")]
        public static extern int csCloseCom();
        // 1.33 讀取重大傷病註記資料
        [DllImport("csHis.dll")]
        public static extern int hisGetCriticalIllness(byte[] pBuffer, ref int iBufferLen);
        // 1.34 讀取讀卡機日期時間
        [DllImport("csHis.dll")]
        public static extern int csGetDateTime(byte[] pBuffer, ref int iBufferLen);
        //1.35 讀取卡片號碼
        [DllImport("csHis.dll")]
        public static extern int csGetCardNo(int CardType, byte[] pBuffer, ref int iBufferLen);
        //1.36 檢查健保IC卡是否設定密碼
        [DllImport("csHis.dll")]
        public static extern int csISSetPIN();
        //1.37 取得就醫序號新版
        [DllImport("csHis.dll")]
        public static extern int hisGetSeqNumber256(byte[] cTreatItem, byte[] cBabyTreat, byte[] cTreatAfterCheck, byte[] pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //1.38 掛號或報到時讀取有效卡期限及就醫次數資料
        public static extern int hisGetRegisterBasic2(byte[] pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //1.39 回復就醫資料累計值---退掛
        public static extern int csUnGetSeqNumber(byte[] pUnTreatDate);
        [DllImport("csHis.dll")]
        //1.40 健保IC卡卡片內容更新作業
        public static extern int csUpdateHCNoReset();
        [DllImport("csHis.dll")]
        //1.41 讀取就醫資料-門診處方箋
        public static extern int hisReadPrescriptMain(byte[] pOutpatientPrescription, ref int iBufferLenOutpatient, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.42 讀取就醫資料-長期處方箋
        public static extern int hisReadPrescriptLongTerm(byte[] pLongTermPrescription, ref int iBufferLenLongTerm, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.43 讀取就醫資料-重要醫令
        public static extern int hisReadPrescriptHVE(byte[] pImportantTreatmentCode, ref int iBufferLenImportant, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.44 讀取就醫資料-過敏藥物
        public static extern int hisReadPrescriptAllergic(byte[] pIrritationDrug, ref int iBufferLenIrritation, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.45 多筆處方箋寫入作業 
        public static extern int hisWriteMultiPrescript(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, ref int iWriteCount);
        [DllImport("csHis.dll")]
        //1.46 過敏藥物寫入指定欄位作業
        public static extern int hisWriteAllergicNum(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID, int iNum);
        [DllImport("csHis.dll")]
        //1.47 就醫診療資料及費用寫入作業
        public static extern int hisWriteTreatmentData(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID);
        [DllImport("csHis.dll")]
        //1.48 處方箋寫入作業-回傳簽章
        public static extern int hisWritePrescriptionSign(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBuffer, ref int iLen);
        [DllImport("csHis.dll")]
        //1.49 多筆處方箋寫入作業-回傳簽章
        public static extern int hisWriteMultiPrescriptSign(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, ref int iWriteCount, byte[] pBuffer, ref int iLen);
        [DllImport("csHis.dll")]
        //1.50 讀取重大傷病註記資料身分比對
        public static extern int hisGetCriticalIllnessID(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //1.51 取得控制軟體版本
        public static extern byte[] csGetVersionEx(byte[] pPath);
        [DllImport("csHis.dll")]
        //1.52 提供His重置讀卡機或卡片的API
        public static extern int csSoftwareReset(int iType);
        // 2.1 安全模組認證
        [DllImport("csHis.dll")]
        public static extern int csVerifySAMDC();
        [DllImport("csHis.dll")]
        //2.2 讀取SAM院所代碼
        public static extern int csGetHospID(byte[] pBuffer, ref int iBufferLen);
        // 3.1 資料上傳
        [DllImport("csHis.dll")]
        public static extern int csUploadData(byte[] pUploadFileName, byte[] fFileSize, byte[] pNumber, byte[] pBuffer, ref int iBufferLen);
        // 4.1 取得醫事人員卡狀態
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCStatus(int Req, ref int Status);
        // 4.2 檢查醫事人員卡之PIN值
        [DllImport("csHis.dll")]
        public static extern int hpcVerifyHPCPIN();
        // 4.3 輸入新的醫事人員卡之PIN值
        [DllImport("csHis.dll")]
        public static extern int hpcInputHPCPIN();
        // 4.4 解開鎖住的醫事人員卡
        [DllImport("csHis.dll")]
        public static extern int hpcUnlockHPC();
        // 4.5 取得醫事人員卡序號
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCSN(byte[] pBuffer, ref int iBufferLen);
        // 4.6 取得醫事人員卡身份證字號
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCSSN(byte[] pBuffer, ref int iBufferLen);
        // 4.7 取得醫事人員卡中文姓名
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCCNAME(byte[] pBuffer, ref int iBufferLen);
        // 4.8 取得醫事人員卡英文姓名
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCENAME(byte[] pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //5.1 進行疾病診斷碼押碼
        public static extern int hisGetICD10EnC(byte[] pIN, byte[] pOUT);
        [DllImport("csHis.dll")]
        //5.2 進行疾病診斷碼解押碼
        public static extern int hisGetICD10DeC(byte[] pIN, byte[] pOUT);

        public static bool OpenCom()
        {
            SetCardReaderStatus(StringRes.讀卡機異常);
            SetCardReaderStatus(StringRes.開啟讀卡機);
            var res = csOpenCom(ViewModelMainWindow.CurrentPharmacy.ReaderCom);
            SetStatus(res == 0,1);
            MainWindow.Instance.SetCardReaderStatus(res == 0 ? StringRes.連接成功 : StringRes.連接失敗);
            return res == 0;
        }

        public static void CloseCom()
        {
            SetCardReaderStatus(StringRes.讀卡機異常);
            if (csCloseCom() == 0)
                SetStatus(false,1);
        }

        public static void CheckCardStatus(int type)
        {
            int res = 4000;
            SetCardReaderStatus(StringRes.讀卡機異常);
            OpenCom();
            string status;
            switch (type)
            {
                case 1:
                    status = StringRes.檢查安全模組;
                    break;
                case 2:
                    status = StringRes.檢查健保卡;
                    break;
                case 3:
                    status = StringRes.檢查醫事人員卡;
                    break;
                default:
                    status = StringRes.檢查中;
                    break;
            }
            MainWindow.Instance.SetCardReaderStatus(status);
            try
            {
                res = hisGetCardStatus(type);
            }
            catch (Exception e)
            {
                ShowMessage(StringRes.控制軟體異常);
            }
            switch (type)
            {
                case 1:
                    switch (res)
                    {
                        case 4000:
                            MainWindow.Instance.SetSamDcStatus(StringRes.讀卡機逾時);
                            SetStatus(false, 2);
                            break;
                        case 1:
                            MainWindow.Instance.SetHpcCardStatus(StringRes.未認證);
                            SetStatus(false, 2);
                            break;
                        case 2:
                            MainWindow.Instance.SetHpcCardStatus(StringRes.認證成功);
                            SetStatus(true, 2);
                            break;
                        default:
                            MainWindow.Instance.SetHpcCardStatus(StringRes.所置入非安全模組);
                            SetStatus(false, 2);
                            break;
                    }
                    break;
                case 2:
                    SetStatus(res == 2,4);
                    MainWindow.Instance.SetCardReaderStatus(StringRes.讀取失敗);
                    break;
                case 3:
                    switch (res)
                    {
                        case 2:
                            MainWindow.Instance.SetHpcCardStatus(StringRes.認證成功未驗);
                            ViewModelMainWindow.IsHpcValid = true;
                            break;
                        case 3:
                            MainWindow.Instance.SetHpcCardStatus(StringRes.認證成功已驗);
                            ViewModelMainWindow.IsHpcValid = true;
                            break;
                        default:
                            MainWindow.Instance.SetHpcCardStatus(StringRes.未認證);
                            ViewModelMainWindow.IsHpcValid = true;
                            break;
                    }
                    break;
            }
            CloseCom();
        }

        public static void VerifySamDc()
        {
            SetCardReaderStatus(StringRes.讀卡機異常);
            bool status;
            int res = 0;
            MainWindow.Instance.SetSamDcStatus(StringRes.健保局連線中);
            if (!OpenCom())
            {
                SetStatus(false, 2);
                MainWindow.Instance.SetSamDcStatus(StringRes.認證失敗);
            }
            try
            {
                res = csVerifySAMDC();
            }
            catch (Exception e)
            {
                ShowMessage(StringRes.控制軟體異常);
            }
            if (res == 0)
            {
                status = true;
                MainWindow.Instance.SetSamDcStatus(StringRes.認證成功);
            }
            else
            {
                status = false;
                MainWindow.Instance.SetSamDcStatus(StringRes.未認證);
            }
            SetStatus(status,2);
            CloseCom();
        }

        public static void ResetCardReader()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                ViewModelMainWindow.HisApiException = false;
            });
            SetStatus(false,1);
            SetStatus(false, 2);
            SetStatus(false, 3);
            bool isPassed = false;
            Application.Current.Dispatcher.Invoke(delegate { isPassed = OpenCom(); });
            if (!isPassed)
                return;
            csSoftwareReset(3);
            CloseCom();
            VerifySamDc();
        }

        public static void SetStatus(bool status,int type)
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
            MainWindow.Instance.Dispatcher.BeginInvoke((Action) Status);
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
            MainWindow.Instance.SetCardReaderStatus(StringRes.驗證醫事人員卡);
            if (!OpenCom())
                return;
            try
            {
                res = hpcVerifyHPCPIN();
            }
            catch (Exception e)
            {
                ShowMessage(StringRes.控制軟體異常);
            }

            if (res == 0)
            {
                MainWindow.Instance.SetHpcCardStatus(StringRes.認證成功);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ViewModelMainWindow.IsHpcValid = true;
                });
            }
            else
            {
                MainWindow.Instance.SetHpcCardStatus(StringRes.認證失敗);
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
