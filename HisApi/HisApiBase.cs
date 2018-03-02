using System.Runtime.InteropServices;
using System.Text;

namespace His_Pos.HisApi
{
    public class HisApiBase
    {
        // 1.1 讀取不需個人PIN碼資料
        [DllImport("csHis.dll")]
        public static extern int hisGetBasicData(StringBuilder pBuffer, ref int iBufferLen);
        // 1.2 掛號或報到時讀取基本資料
        [DllImport("csHis.dll")]
        public static extern int hisGetRegisterBasic(StringBuilder pBuffer, ref int iBufferLen);
        // 1.3 預防保健掛號作業
        [DllImport("csHis.dll")]
        public static extern int hisGetRegisterPrevent(StringBuilder pBuffer, ref int intpBufferLen);
        // 1.4 孕婦產前檢查掛號作業
        [DllImport("csHis.dll")]
        public static extern int hisGetRegisterPregnant(StringBuilder pBuffer, ref int iBufferLen);
        // 1.5 讀取就醫資料不需HPC卡的部份
        [DllImport("csHis.dll")]
        public static extern int hisGetTreatmentNoNeedHPC(StringBuilder strpBuffer, ref int intpBufferLen);
        // 1.6 讀取就醫累計資料
        [DllImport("csHis.dll")]
        public static extern int hisGetCumulativeData(StringBuilder pBuffer, ref int iBufferLen);
        // 1.7 讀取醫療費用線累計
        [DllImport("csHis.dll")]
        public static extern int hisGetCumulativeFee(StringBuilder strpBuffer, ref int intpBufferLen);
        // 1.8 讀取就醫資料需HPC卡的部份
        [DllImport("csHis.dll")]
        public static extern int hisGetTreatmentNeedHPC(StringBuilder pBuffer, ref int iBufferLen);
        // 1.9 取得就醫序號  strpBuffer欄位宣告原本VB為As Any  C#要改成string and long
        [DllImport("csHis.dll")]
        public static extern int hisGetSeqNumber(StringBuilder cTreatItem, StringBuilder cBabyTreat, StringBuilder strpBuffer, ref int intpBufferLen);
        // 1.10 讀取處方箋作業
        [DllImport("csHis.dll")]
        public static extern int hisReadPrescription(StringBuilder pOutpatientPrescription, ref int iBufferLenOutpatient, StringBuilder pLongTermPrescription, ref int iBufferLenLongTerm, StringBuilder pImportantTreatmentCode, ref int iBufferLenImportant, StringBuilder pIrritationDrug, ref int iBufferLenIrritation);
        // 1.11 讀取預防接種資料
        [DllImport("csHis.dll")]
        public static extern int hisGetInoculateData(StringBuilder strpBuffer, ref int intpBufferLen);
        // 1.12 讀取器官捐贈資料
        [DllImport("csHis.dll")]
        public static extern int hisGetOrganDonate(StringBuilder pBuffer, ref int iBufferLen);
        // 1.13 讀取緊急聯絡電話資料
        [DllImport("csHis.dll")]
        public static extern int HisGetEmergentTel(StringBuilder pBuffer, ref int iBufferLen);
        // 1.14 讀取最近一次就醫序號
        [DllImport("csHis.dll")]
        public static extern int hisGetLastSeqNum(StringBuilder pBuffer, ref int iBufferLen);
        // 1.15 讀取卡片狀態
        [DllImport("csHis.dll")]
        public static extern int hisGetCardStatus(int CardType);
        // 1.16 就醫診療資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteTreatmentCode(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, StringBuilder pBufferDocID);
        // 1.17 就醫費用資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteTreatmentFee(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite);
        // 1.18 處方箋寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWritePrescription(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite);
        // 1.19 新生兒註記寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteNewBorn(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pNewBornDate, StringBuilder pNoOfDelivered);
        // 1.20 過敏藥物寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteAllergicMedicines(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, StringBuilder pBufferDocID);
        // 1.21 同意器官捐贈及安寧緩和醫療註記寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteOrganDonate(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pOrganDonate);
        // 1.22 預防保健資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteHealthInsurance(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pServiceItem, StringBuilder pServiceItemCode);
        // 1.23 緊急聯絡電話資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteEmergentTel(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pEmergentTel);
        // 1.24 寫入產前檢查資料
        [DllImport("csHis.dll")]
        public static extern int hisWritePredeliveryCheckup(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pCheckupCode);
        // 1.25 清除產前檢查資料
        [DllImport("csHis.dll")]
        public static extern int hisDeletePredeliveryData(StringBuilder pPatientID, StringBuilder pPatientBirthDate);
        // 1.26 預防接種資料寫入作業
        [DllImport("csHis.dll")]
        public static extern int hisWriteInoculateData(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pItem, StringBuilder pPackageNumber);
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
        public static extern int hisGetCriticalIllness(StringBuilder pBuffer, ref int iBufferLen);
        // 1.34 讀取讀卡機日期時間
        [DllImport("csHis.dll")]
        public static extern int csGetDateTime(StringBuilder pBuffer, ref int iBufferLen);
        //1.35 讀取卡片號碼
        [DllImport("csHis.dll")]
        public static extern int csGetCardNo(int CardType, StringBuilder pBuffer, ref int iBufferLen);
        //1.36 檢查健保IC卡是否設定密碼
        [DllImport("csHis.dll")]
        public static extern int csISSetPIN();
        //1.37 取得就醫序號新版
        [DllImport("csHis.dll")]
        public static extern int hisGetSeqNumber256(StringBuilder cTreatItem, StringBuilder cBabyTreat, StringBuilder cTreatAfterCheck, StringBuilder pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //1.38 掛號或報到時讀取有效卡期限及就醫次數資料
        public static extern int hisGetRegisterBasic2(StringBuilder pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //1.39 回復就醫資料累計值---退掛
        public static extern int csUnGetSeqNumber(StringBuilder pUnTreatDate);
        [DllImport("csHis.dll")]
        //1.40 健保IC卡卡片內容更新作業
        public static extern int csUpdateHCNoReset();
        [DllImport("csHis.dll")]
        //1.41 讀取就醫資料-門診處方箋
        public static extern int hisReadPrescriptMain(StringBuilder pOutpatientPrescription, ref int iBufferLenOutpatient, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.42 讀取就醫資料-長期處方箋
        public static extern int hisReadPrescriptLongTerm(StringBuilder pLongTermPrescription, ref int iBufferLenLongTerm, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.43 讀取就醫資料-重要醫令
        public static extern int hisReadPrescriptHVE(StringBuilder pImportantTreatmentCode, ref int iBufferLenImportant, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.44 讀取就醫資料-過敏藥物
        public static extern int hisReadPrescriptAllergic(StringBuilder pIrritationDrug, ref int iBufferLenIrritation, int iStartPos, int iEndPos, ref int iRecCount);
        [DllImport("csHis.dll")]
        //1.45 多筆處方箋寫入作業 
        public static extern int hisWriteMultiPrescript(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, ref int iWriteCount);
        [DllImport("csHis.dll")]
        //1.46 過敏藥物寫入指定欄位作業
        public static extern int hisWriteAllergicNum(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, StringBuilder pBufferDocID, int iNum);
        [DllImport("csHis.dll")]
        //1.47 就醫診療資料及費用寫入作業
        public static extern int hisWriteTreatmentData(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, StringBuilder pBufferDocID);
        [DllImport("csHis.dll")]
        //1.48 處方箋寫入作業-回傳簽章
        public static extern int hisWritePrescriptionSign(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, StringBuilder pBuffer, ref int iLen);
        [DllImport("csHis.dll")]
        //1.49 多筆處方箋寫入作業-回傳簽章
        public static extern int hisWriteMultiPrescriptSign(StringBuilder pDateTime, StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pDataWrite, ref int iWriteCount, StringBuilder pBuffer, ref int iLen);
        [DllImport("csHis.dll")]
        //1.50 讀取重大傷病註記資料身分比對
        public static extern int hisGetCriticalIllnessID(StringBuilder pPatientID, StringBuilder pPatientBirthDate, StringBuilder pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //1.51 取得控制軟體版本
        public static extern StringBuilder csGetVersionEx(StringBuilder pPath);
        [DllImport("csHis.dll")]
        //1.52 提供His重置讀卡機或卡片的API
        public static extern int csSoftwareReset(long iType);
        // 2.1 安全模組認證
        [DllImport("csHis.dll")]
        public static extern int csVerifySAMDC();
        [DllImport("csHis.dll")]
        //2.2 讀取SAM院所代碼
        public static extern int csGetHospID(StringBuilder pBuffer, ref int iBufferLen);
        // 3.1 資料上傳
        [DllImport("csHis.dll")]
        public static extern int csUploadData(StringBuilder pUploadFileName, StringBuilder fFileSize, StringBuilder pNumber, StringBuilder pBuffer, ref int iBufferLen);
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
        public static extern int hpcGetHPCSN(StringBuilder pBuffer, ref int iBufferLen);
        // 4.6 取得醫事人員卡身份證字號
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCSSN(StringBuilder pBuffer, ref int iBufferLen);
        // 4.7 取得醫事人員卡中文姓名
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCCNAME(StringBuilder pBuffer, ref int iBufferLen);
        // 4.8 取得醫事人員卡英文姓名
        [DllImport("csHis.dll")]
        public static extern int hpcGetHPCENAME(StringBuilder pBuffer, ref int iBufferLen);
        [DllImport("csHis.dll")]
        //5.1 進行疾病診斷碼押碼
        public static extern int hisGetICD10EnC(StringBuilder pIN, StringBuilder pOUT);
        [DllImport("csHis.dll")]
        //5.2 進行疾病診斷碼解押碼
        public static extern int hisGetICD10DeC(StringBuilder pIN, StringBuilder pOUT);
    }
}
