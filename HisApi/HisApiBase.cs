using System.Runtime.InteropServices;

namespace His_Pos.HisApi
{
    public class HisApiBase
    {
        // 1.1 讀取不需個人PIN碼資料
        [DllImport("CSHIS.dll", EntryPoint = "hisGetBasicData")]
        public static extern int hisGetBasicData(byte[] pBuffer, ref int iBufferLen);

        // 1.2 掛號或報到時讀取基本資料
        [DllImport("CSHIS.dll", EntryPoint = "hisGetRegisterBasic")]
        public static extern int hisGetRegisterBasic(byte[] pBuffer, ref int iBufferLen);

        // 1.3 預防保健掛號作業
        [DllImport("CSHIS.dll", EntryPoint = "hisGetRegisterPrevent")]
        public static extern int hisGetRegisterPrevent(byte[] pBuffer, ref int intpBufferLen);

        // 1.4 孕婦產前檢查掛號作業
        [DllImport("CSHIS.dll", EntryPoint = "hisGetRegisterPregnant")]
        public static extern int hisGetRegisterPregnant(byte[] pBuffer, ref int iBufferLen);

        // 1.5 讀取就醫資料不需HPC卡的部份
        [DllImport("CSHIS.dll", EntryPoint = "hisGetTreatmentNoNeedHPC")]
        public static extern int hisGetTreatmentNoNeedHPC(byte[] strpBuffer, ref int intpBufferLen);

        // 1.6 讀取就醫累計資料
        [DllImport("CSHIS.dll", EntryPoint = "hisGetCumulativeData")]
        public static extern int hisGetCumulativeData(byte[] pBuffer, ref int iBufferLen);

        // 1.7 讀取醫療費用線累計
        [DllImport("CSHIS.dll", EntryPoint = "hisGetCumulativeFee")]
        public static extern int hisGetCumulativeFee(byte[] strpBuffer, ref int intpBufferLen);

        // 1.8 讀取就醫資料需HPC卡的部份
        [DllImport("CSHIS.dll", EntryPoint = "hisGetTreatmentNeedHPC")]
        public static extern int hisGetTreatmentNeedHPC(byte[] pBuffer, ref int iBufferLen);

        // 1.9 取得就醫序號  strpBuffer欄位宣告原本VB為As Any  C#要改成string and long
        [DllImport("CSHIS.dll", EntryPoint = "hisGetSeqNumber")]
        public static extern int hisGetSeqNumber(byte[] cTreatItem, byte[] cBabyTreat, byte[] strpBuffer, ref int intpBufferLen);

        // 1.10 讀取處方箋作業
        [DllImport("CSHIS.dll", EntryPoint = "hisReadPrescription")]
        public static extern int hisReadPrescription(byte[] pOutpatientPrescription, ref int iBufferLenOutpatient, byte[] pLongTermPrescription, ref int iBufferLenLongTerm, byte[] pImportantTreatmentCode, ref int iBufferLenImportant, byte[] pIrritationDrug, ref int iBufferLenIrritation);

        // 1.11 讀取預防接種資料
        [DllImport("CSHIS.dll", EntryPoint = "hisGetInoculateData")]
        public static extern int hisGetInoculateData(byte[] strpBuffer, ref int intpBufferLen);

        // 1.12 讀取器官捐贈資料
        [DllImport("CSHIS.dll", EntryPoint = "hisGetOrganDonate")]
        public static extern int hisGetOrganDonate(byte[] pBuffer, ref int iBufferLen);

        // 1.13 讀取緊急聯絡電話資料
        [DllImport("CSHIS.dll", EntryPoint = "HisGetEmergentTel")]
        public static extern int HisGetEmergentTel(byte[] pBuffer, ref int iBufferLen);

        // 1.14 讀取最近一次就醫序號
        [DllImport("CSHIS.dll", EntryPoint = "hisGetLastSeqNum")]
        public static extern int hisGetLastSeqNum(byte[] pBuffer, ref int iBufferLen);

        // 1.15 讀取卡片狀態
        [DllImport("CSHIS.dll", EntryPoint = "hisGetCardStatus")]
        public static extern int hisGetCardStatus(int CardType);

        // 1.16 就醫診療資料寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteTreatmentCode")]
        public static extern int hisWriteTreatmentCode(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID);

        // 1.17 就醫費用資料寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteTreatmentFee")]
        public static extern int hisWriteTreatmentFee(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite);

        // 1.18 處方箋寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWritePrescription")]
        public static extern int hisWritePrescription(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite);

        // 1.19 新生兒註記寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteNewBorn")]
        public static extern int hisWriteNewBorn(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pNewBornDate, byte[] pNoOfDelivered);

        // 1.20 過敏藥物寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteAllergicMedicines")]
        public static extern int hisWriteAllergicMedicines(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID);

        // 1.21 同意器官捐贈及安寧緩和醫療註記寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteOrganDonate")]
        public static extern int hisWriteOrganDonate(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pOrganDonate);

        // 1.22 預防保健資料寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteHealthInsurance")]
        public static extern int hisWriteHealthInsurance(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pServiceItem, byte[] pServiceItemCode);

        // 1.23 緊急聯絡電話資料寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteEmergentTel")]
        public static extern int hisWriteEmergentTel(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pEmergentTel);

        // 1.24 寫入產前檢查資料
        [DllImport("CSHIS.dll", EntryPoint = "hisWritePredeliveryCheckup")]
        public static extern int hisWritePredeliveryCheckup(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pCheckupCode);

        // 1.25 清除產前檢查資料
        [DllImport("CSHIS.dll", EntryPoint = "hisDeletePredeliveryData")]
        public static extern int hisDeletePredeliveryData(byte[] pPatientID, byte[] pPatientBirthDate);

        // 1.26 預防接種資料寫入作業
        [DllImport("CSHIS.dll", EntryPoint = "hisWriteInoculateData")]
        public static extern int hisWriteInoculateData(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pItem, byte[] pPackageNumber);

        // 1.27 驗證健保IC卡之PIN值
        [DllImport("CSHIS.dll", EntryPoint = "csVerifyHCPIN")]
        public static extern int csVerifyHCPIN();

        // 1.28 輸入新的健保IC卡PIN值
        [DllImport("CSHIS.dll", EntryPoint = "csInputHCPIN")]
        public static extern int csInputHCPIN();

        // 1.29 停用健保IC卡之PIN碼輸入功能
        [DllImport("CSHIS.dll", EntryPoint = "csDisableHCPIN")]
        public static extern int csDisableHCPIN();

        // 1.30 健保IC卡卡片內容更新作業
        [DllImport("CSHIS.dll", EntryPoint = "csUpdateHCContents")]
        public static extern int csUpdateHCContents();

        // 1.31 開啟讀卡機連結埠
        [DllImport("CSHIS.dll", EntryPoint = "csOpenCom")]
        public static extern int csOpenCom(int pcom);

        // 1.32 關閉讀卡機連結埠
        [DllImport("CSHIS.dll", EntryPoint = "csCloseCom")]
        public static extern int csCloseCom();

        // 1.33 讀取重大傷病註記資料
        [DllImport("CSHIS.dll", EntryPoint = "hisGetCriticalIllness")]
        public static extern int hisGetCriticalIllness(byte[] pBuffer, ref int iBufferLen);

        // 1.34 讀取讀卡機日期時間
        [DllImport("CSHIS.dll", EntryPoint = "csGetDateTime")]
        public static extern int csGetDateTime(byte[] pBuffer, ref int iBufferLen);

        //1.35 讀取卡片號碼
        [DllImport("CSHIS.dll", EntryPoint = "csGetCardNo")]
        public static extern int csGetCardNo(int CardType, byte[] pBuffer, ref int iBufferLen);

        //1.36 檢查健保IC卡是否設定密碼
        [DllImport("CSHIS.dll", EntryPoint = "csISSetPIN")]
        public static extern int csISSetPIN();

        //1.37 取得就醫序號新版
        [DllImport("CSHIS.dll", EntryPoint = "hisGetSeqNumber256")]
        public static extern int hisGetSeqNumber256(byte[] cTreatItem, byte[] cBabyTreat, byte[] cTreatAfterCheck, byte[] pBuffer, ref int iBufferLen);

        [DllImport("CSHIS.dll", EntryPoint = "hisGetRegisterBasic2")]
        //1.38 掛號或報到時讀取有效卡期限及就醫次數資料
        public static extern int hisGetRegisterBasic2(byte[] pBuffer, ref int iBufferLen);

        [DllImport("CSHIS.dll", EntryPoint = "csUnGetSeqNumber")]
        //1.39 回復就醫資料累計值---退掛
        public static extern int csUnGetSeqNumber(byte[] pUnTreatDate);

        [DllImport("CSHIS.dll", EntryPoint = "csUpdateHCNoReset")]
        //1.40 健保IC卡卡片內容更新作業
        public static extern int csUpdateHCNoReset();

        [DllImport("CSHIS.dll", EntryPoint = "hisReadPrescriptMain")]
        //1.41 讀取就醫資料-門診處方箋
        public static extern int hisReadPrescriptMain(byte[] pOutpatientPrescription, ref int iBufferLenOutpatient, int iStartPos, int iEndPos, ref int iRecCount);

        [DllImport("CSHIS.dll", EntryPoint = "hisReadPrescriptLongTerm")]
        //1.42 讀取就醫資料-長期處方箋
        public static extern int hisReadPrescriptLongTerm(byte[] pLongTermPrescription, ref int iBufferLenLongTerm, int iStartPos, int iEndPos, ref int iRecCount);

        [DllImport("CSHIS.dll", EntryPoint = "hisReadPrescriptHVE")]
        //1.43 讀取就醫資料-重要醫令
        public static extern int hisReadPrescriptHVE(byte[] pImportantTreatmentCode, ref int iBufferLenImportant, int iStartPos, int iEndPos, ref int iRecCount);

        [DllImport("CSHIS.dll", EntryPoint = "hisReadPrescriptAllergic")]
        //1.44 讀取就醫資料-過敏藥物
        public static extern int hisReadPrescriptAllergic(byte[] pIrritationDrug, ref int iBufferLenIrritation, int iStartPos, int iEndPos, ref int iRecCount);

        [DllImport("CSHIS.dll", EntryPoint = "hisWriteMultiPrescript")]
        //1.45 多筆處方箋寫入作業
        public static extern int hisWriteMultiPrescript(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, ref int iWriteCount);

        [DllImport("CSHIS.dll", EntryPoint = "hisWriteAllergicNum")]
        //1.46 過敏藥物寫入指定欄位作業
        public static extern int hisWriteAllergicNum(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID, int iNum);

        [DllImport("CSHIS.dll", EntryPoint = "hisWriteTreatmentData")]
        //1.47 就醫診療資料及費用寫入作業
        public static extern int hisWriteTreatmentData(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBufferDocID);

        [DllImport("CSHIS.dll", EntryPoint = "hisWritePrescriptionSign")]
        //1.48 處方箋寫入作業-回傳簽章
        public static extern int hisWritePrescriptionSign(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, byte[] pBuffer, ref int iLen);

        [DllImport("CSHIS.dll", EntryPoint = "hisWriteMultiPrescriptSign")]
        //1.49 多筆處方箋寫入作業-回傳簽章
        public static extern int hisWriteMultiPrescriptSign(byte[] pDateTime, byte[] pPatientID, byte[] pPatientBirthDate, byte[] pDataWrite, ref int iWriteCount, byte[] pBuffer, ref int iLen);

        [DllImport("CSHIS.dll", EntryPoint = "hisGetCriticalIllnessID")]
        //1.50 讀取重大傷病註記資料身分比對
        public static extern int hisGetCriticalIllnessID(byte[] pPatientID, byte[] pPatientBirthDate, byte[] pBuffer, ref int iBufferLen);

        [DllImport("CSHIS.dll", EntryPoint = "csGetVersionEx")]
        //1.51 取得控制軟體版本
        public static extern byte[] csGetVersionEx(byte[] pPath);

        [DllImport("CSHIS.dll", EntryPoint = "csSoftwareReset")]
        //1.52 提供His重置讀卡機或卡片的API
        public static extern int csSoftwareReset(int iType);

        // 2.1 安全模組認證
        [DllImport("CSHIS.dll", EntryPoint = "csVerifySAMDC")]
        public static extern int csVerifySAMDC();

        [DllImport("CSHIS.dll", EntryPoint = "csGetHospID")]
        //2.2 讀取SAM院所代碼
        public static extern int csGetHospID(byte[] pBuffer, ref int iBufferLen);

        // 3.1 資料上傳
        [DllImport("CSHIS.dll", EntryPoint = "csUploadData")]
        public static extern int csUploadData(byte[] pUploadFileName, byte[] fFileSize, byte[] pNumber, byte[] pBuffer, ref int iBufferLen);

        // 3.2 資料上傳 增加「處方筆數」參數
        [DllImport("CSHIS.dll", EntryPoint = "csUploadDataPrec")]
        public static extern int csUploadDataPrec(byte[] pUploadFileName, byte[] pFileSize, byte[] pNumber, byte[] pPrecNumber, byte[] pBuffer, ref int iBufferLen);

        // 3.2 資料上傳 增加「處方筆數」參數
        [DllImport("CSHIS.dll", EntryPoint = "csDownloadData")]
        public static extern int csDownloadData(byte[] pSAMID, byte[] pHospID, byte[] pNumber, byte[] pSendDate, byte[] pRecvDate, byte[] pServerRandom, byte[] pDownloadFileName);

        // 4.1 取得醫事人員卡狀態
        [DllImport("CSHIS.dll", EntryPoint = "hpcGetHPCStatus")]
        public static extern int hpcGetHPCStatus(int Req, ref int Status);

        // 4.2 檢查醫事人員卡之PIN值
        [DllImport("CSHIS.dll", EntryPoint = "hpcVerifyHPCPIN")]
        public static extern int hpcVerifyHPCPIN();

        // 4.3 輸入新的醫事人員卡之PIN值
        [DllImport("CSHIS.dll", EntryPoint = "hpcInputHPCPIN")]
        public static extern int hpcInputHPCPIN();

        // 4.4 解開鎖住的醫事人員卡
        [DllImport("CSHIS.dll", EntryPoint = "hpcUnlockHPC")]
        public static extern int hpcUnlockHPC();

        // 4.5 取得醫事人員卡序號
        [DllImport("CSHIS.dll", EntryPoint = "hpcGetHPCSN")]
        public static extern int hpcGetHPCSN(byte[] pBuffer, ref int iBufferLen);

        // 4.6 取得醫事人員卡身份證字號
        [DllImport("CSHIS.dll", EntryPoint = "hpcGetHPCSSN")]
        public static extern int hpcGetHPCSSN(byte[] pBuffer, ref int iBufferLen);

        // 4.7 取得醫事人員卡中文姓名
        [DllImport("CSHIS.dll", EntryPoint = "hpcGetHPCCNAME")]
        public static extern int hpcGetHPCCNAME(byte[] pBuffer, ref int iBufferLen);

        // 4.8 取得醫事人員卡英文姓名
        [DllImport("CSHIS.dll", EntryPoint = "hpcGetHPCENAME")]
        public static extern int hpcGetHPCENAME(byte[] pBuffer, ref int iBufferLen);

        [DllImport("CSHIS.dll", EntryPoint = "hisGetICD10EnC")]
        //5.1 進行疾病診斷碼押碼
        public static extern int hisGetICD10EnC(byte[] pIN, byte[] pOUT);

        [DllImport("CSHIS.dll", EntryPoint = "hisGetICD10DeC")]
        //5.2 進行疾病診斷碼解押碼
        public static extern int hisGetICD10DeC(byte[] pIN, byte[] pOUT);
    }
}