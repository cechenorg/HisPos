using System;
using System.ComponentModel;

namespace His_Pos.Class
{
    public enum InventoryDetailOverviewType
    {
        Error = 0,
        Treatment = 1,
        PurchaseReturn = 2,
        StockTaking = 3
    }
     

    public enum FeatureItem
    {
        處方登錄 = 0,
        處方查詢 = 1,
        匯出申報檔 = 2,
        商品查詢 = 3,
        進退貨管理 = 4,
        進退貨紀錄 = 5,
        新增盤點 = 6,
        庫存盤點紀錄 = 7,
        商品類別管理 = 8,
        櫃位管理 = 9,
        供應商管理 = 10,
        員工管理 = 11,
        上下班打卡 = 12,
        審核管理 = 13,
        排班管理 = 14,
        藥品頻率管理 = 15,
        藥局管理 = 17,
        顧客管理 = 19,
        每日作業 = 20,
        庫存現值報表 = 21,
        進退貨報表 = 22,
        合作診所藥品耗用 = 23,
        系統函式 = 25,
        管制藥品簿冊申報 = 26,
        會計總帳報表 = 27,
        額外收支 = 29,
        申報院所點數總表 = 30,
        盤點計畫 = 31,
        結帳作業 = 32,
        銷售紀錄 = 33,
        促銷管理 = 34,
        藥袋查詢 = 35,
        藥健康網頁 = 36,
        系統教學文件 = 37,
        損益報表 = 38,
        資產負債表 = 39,
        關班作業 = 40,
        立帳作業 = 41,
        舊每日總帳報表 = 42,
        關班帳務查詢 = 43,
        打卡記錄查詢 = 47, // add by SHANI
        沖帳作業 = 48,
        每日總帳報表 = 49,
    }

    public enum StoreOrderProductType
    {
        BASIC = 0,
        SAFE = 1
    }

    public enum ProcedureProcessType
    {
        INSERT = 0,
        UPDATE = 1,
        DELETE = 2
    }

    public enum OrderType
    {
        UNPROCESSING = 0,
        PROCESSING = 1,
        DONE = 2,
        ALL = 3,
        WAITING = 4,
        SCRAP = 5,
        ERROR = 100
    }

    public enum AddOrderType
    {
        RETURNBYMANUFACTORY = 1,
        RETURNBYORDER = 2,
        ADDALLBELOWSAFEAMOUNT = 4,
        ADDBYMANUFACTORY = 5,
        ADDALLTOBASICAMOUNT = 6,
        ADDALLGOODSALES = 8,
        ADDBYMANUFACTORYBELOWSAFEAMOUNT = 10,
        ADDBYMANUFACTORYTOBASICAMOUNT = 15,
        ADDBYMANUFACTORYGOODSALES = 20,
        ERROR = 100
    }

    public enum DataSource
    {
        GetOtc = 0,
        STOORDLIST = 1,
        GetMedicine = 2,
        MANUFACTORY = 3,
        PROMAN = 4,
        PRODUCTBASICORSAFE = 5,
        ITEMDIALOGPRODUCT = 6,
        GetStoreOrderDetail = 7,
        GetItemDialogProduct = 8,
        GetMedicalIcCard = 9,
        InitMedicalIcCard = 10,
        GetHospitalData = 11,
        InitHospitalData = 12,
    }

    public enum MessageType
    {
        ERROR = 0,
        SUCCESS = 1,
        WARNING = 2,
        ONLYMESSAGE = 3,
        TIPS = 4
    }

    public enum DeclareFileType
    {
        LOG_IN = 0,
        UPDATE = 1,
        DECLAREFILE_UPDATE = 2
    }

    public enum MedBagMode
    {
        SINGLE = 0,
        MULTI = 1
    }

    public enum SystemType
    {
        HIS = 0,
        POS = 1,
        ALL = 2
    }

    public enum SearchType
    {
        OTC = 0,
        MED = 1,
        ALL = 2
    }

    [Flags]
    public enum LoginAuth
    {
        None = 0,
        處方登錄 = 1,
        處方申報 = 2,
        盤點作業 = 4,
        採購作業 = 8,
        All = None | 處方登錄 | 處方申報 | 盤點作業 | 採購作業
    };

    public enum Alphabat
    {
        [Description("1")]
        ㄅ,

        [Description("q")]
        ㄆ,

        [Description("a")]
        ㄇ,

        [Description("z")]
        ㄈ,

        [Description("2")]
        ㄉ,

        [Description("w")]
        ㄊ,

        [Description("s")]
        ㄋ,

        [Description("x")]
        ㄌ,

        [Description("3")]
        ˇ,

        [Description("e")]
        ㄍ,

        [Description("d")]
        ㄎ,

        [Description("c")]
        ㄏ,

        [Description("4")]
        ˋ,

        [Description("r")]
        ㄐ,

        [Description("f")]
        ㄑ,

        [Description("v")]
        ㄒ,

        [Description("5")]
        ㄓ,

        [Description("t")]
        ㄔ,

        [Description("g")]
        ㄕ,

        [Description("b")]
        ㄖ,

        [Description("6")]
        ˊ,

        [Description("y")]
        ㄗ,

        [Description("h")]
        ㄘ,

        [Description("n")]
        ㄙ,

        [Description("u")]
        ㄧ,

        [Description("j")]
        ㄨ,

        [Description("m")]
        ㄩ,

        [Description("8")]
        ㄚ,

        [Description("i")]
        ㄛ,

        [Description("k")]
        ㄜ,

        [Description(",")]
        ㄝ,

        [Description("9")]
        ㄞ,

        [Description("o")]
        ㄟ,

        [Description("l")]
        ㄠ,

        [Description(".")]
        ㄡ,

        [Description("0")]
        ㄢ,

        [Description("p")]
        ㄣ,

        [Description(";")]
        ㄤ,

        [Description("/")]
        ㄥ,

        [Description("-")]
        ㄦ
    }

    public enum ErrorCode
    {
        [Description("讀卡機連接埠連線失敗")]
        ErrorCode1 = -1,

        [Description("讀卡機正常運作中")]
        ErrorCode0 = 0,

        [Description("讀卡機逾時")]
        ErrorCode4000 = 4000,

        [Description("未置入安全模組卡")]
        ErrorCode4012 = 4012,

        [Description("未置入健保卡")]
        ErrorCode4013 = 4013,

        [Description("未置入醫事人員卡")]
        ErrorCode4014 = 4014,

        [Description("健保卡權限不足")]
        ErrorCode4029 = 4029,

        [Description("所插入非安全模組卡")]
        ErrorCode4032 = 4032,

        [Description("所置入非健保卡")]
        ErrorCode4033 = 4033,

        [Description("所置入非醫事人員卡")]
        ErrorCode4034 = 4034,

        [Description("醫事人員卡PIN尚未認證成功")]
        ErrorCode4042 = 4042,

        [Description("安全模組尚未與IDC認證")]
        ErrorCode4050 = 4050,

        [Description("安全模組與IDC認證失敗")]
        ErrorCode4051 = 4051,

        [Description("網路不通")]
        ErrorCode4061 = 4061,

        [Description("健保IC卡與IDC認證失敗")]
        ErrorCode4071 = 4071,

        [Description("就醫可用次數不足")]
        ErrorCode5001 = 5001,

        [Description("卡片已註銷")]
        ErrorCode5002 = 5002,

        [Description("卡片已過有限期限")]
        ErrorCode5003 = 5003,

        [Description("非新生兒一個月內就診")]
        ErrorCode5004 = 5004,

        [Description("讀卡機的日期時間讀取失敗")]
        ErrorCode5005 = 5005,

        [Description("讀取安全模組內的「醫療院所代碼」失敗")]
        ErrorCode5006 = 5006,

        [Description("寫入一組新的「就醫資料登錄」失敗")]
        ErrorCode5007 = 5007,

        [Description("安全模組簽章失敗")]
        ErrorCode5008 = 5008,

        [Description("無寫入就醫相關紀錄之權限")]
        ErrorCode5009 = 5009,

        [Description("同一天看診兩科(含)以上")]
        ErrorCode5010 = 5010,

        [Description("此人未在保或欠費")]
        ErrorCode5012 = 5012,

        [Description("「門診處方箋」讀取失敗。")]
        ErrorCode5015 = 5015,

        [Description("「長期處方箋」讀取失敗。")]
        ErrorCode5016 = 5016,

        [Description("「重要醫令」讀取失敗。")]
        ErrorCode5017 = 5017,

        [Description("要寫入的資料和健保IC卡不是屬於同一人。")]
        ErrorCode5020 = 5020,

        [Description("找不到「就醫資料登錄」中的該組資料。")]
        ErrorCode5022 = 5022,

        [Description("「就醫資料登錄」寫入失敗。")]
        ErrorCode5023 = 5023,

        [Description("HC卡「就醫費用紀錄」寫入失敗。")]
        ErrorCode5028 = 5028,

        [Description("「門診處方箋」寫入失敗。")]
        ErrorCode5033 = 5033,

        [Description("新生兒註記寫入失敗")]
        ErrorCode5051 = 5051,

        [Description("有新生兒出生日期，但無新生兒胞胎註記資料")]
        ErrorCode5052 = 5052,

        [Description("讀取醫事人員ID失敗")]
        ErrorCode5056 = 5056,

        [Description("過敏藥物寫入失敗。")]
        ErrorCode5057 = 5057,

        [Description("同意器官捐贈及安寧緩和醫療註記寫入失敗寫入失敗")]
        ErrorCode5061 = 5061,

        [Description("放棄同意器官捐贈及安寧緩和醫療註記輸入")]
        ErrorCode5062 = 5062,

        [Description("安全模組卡「醫療院所代碼」讀取失敗")]
        ErrorCode5067 = 5067,

        [Description("預防保健資料寫入失敗")]
        ErrorCode5068 = 5068,

        [Description("緊急聯絡電話寫失敗。")]
        ErrorCode5071 = 5071,

        [Description("產前檢查資料寫入失敗")]
        ErrorCode5078 = 5078,

        [Description("性別不符，健保IC卡記載為男性")]
        ErrorCode5079 = 5079,

        [Description("最近24小時內同院所未曾就醫，故不可取消就醫")]
        ErrorCode5081 = 5081,

        [Description("最近24小時內同院所未曾執行產檢服務紀錄，故不可取消產檢")]
        ErrorCode5082 = 5082,

        [Description("最近6次就醫不含就醫類別AC，不可單獨寫入預防保健或產檢紀錄")]
        ErrorCode5083 = 5083,

        [Description("最近24小時內同院所未曾執行保健服務項目紀錄，故不可取消保健服務")]
        ErrorCode5084 = 5084,

        [Description("刪除「孕婦產前檢查(限女性)」全部11組的資料失敗。")]
        ErrorCode5087 = 5087,

        [Description("預防接種資料寫入失敗")]
        ErrorCode5093 = 5093,

        [Description("使用者所輸入之pin值，與卡上之pin值不合")]
        ErrorCode5102 = 5102,

        [Description("原PIN碼尚未通過認證")]
        ErrorCode5105 = 5105,

        [Description("使用者輸入兩次新PIN值，兩次PIN值不合")]
        ErrorCode5107 = 5107,

        [Description("密碼變更失敗")]
        ErrorCode5108 = 5108,

        [Description("密碼輸入過程按『取消』鍵")]
        ErrorCode5109 = 5109,

        [Description("變更健保IC卡密碼時,請移除醫事人員卡")]
        ErrorCode5110 = 5110,

        [Description("停用失敗，且健保IC卡之Pin碼輸入功能仍啟用")]
        ErrorCode5111 = 5111,

        [Description("被鎖住的醫事人員卡仍未解開")]
        ErrorCode5122 = 5122,

        [Description("更新健保IC卡內容失敗。")]
        ErrorCode5130 = 5130,

        [Description("未置入醫事人員卡,僅能讀取重大傷病有效起訖日期")]
        ErrorCode5141 = 5141,

        [Description("卡片中無此筆就醫記錄")]
        ErrorCode5150 = 5150,

        [Description("就醫類別為數值才可退掛")]
        ErrorCode5151 = 5151,

        [Description("醫療院所不同，不可退掛")]
        ErrorCode5152 = 5152,

        [Description("本筆就醫記錄已經退掛過，不可重覆退掛")]
        ErrorCode5153 = 5153,

        [Description("退掛日期不符合規定")]
        ErrorCode5154 = 5154,

        [Description("就醫可用次數不合理")]
        ErrorCode5160 = 5160,

        [Description("最近一次就醫年不合理")]
        ErrorCode5161 = 5161,

        [Description("最近一次就醫序號不合理")]
        ErrorCode5162 = 5162,

        [Description("住診費用總累計不合理")]
        ErrorCode5163 = 5163,

        [Description("門診費用總累計不合理")]
        ErrorCode5164 = 5164,

        [Description("就醫累計資料年不合理")]
        ErrorCode5165 = 5165,

        [Description("門住診就醫累計次數不合理")]
        ErrorCode5166 = 5166,

        [Description("門診部分負擔費用累計不合理")]
        ErrorCode5167 = 5167,

        [Description("住診急性30天、慢性180天以下部分負擔費用累計不合理")]
        ErrorCode5168 = 5168,

        [Description("住診急性31天、慢性181天以上部分負擔費用累計不合理")]
        ErrorCode5169 = 5169,

        [Description("門診+住診部分負擔費用累計不合理")]
        ErrorCode5170 = 5170,

        [Description("[門診+住診(急性30天、慢性180天以下)]部分負擔費用累計不合理")]
        ErrorCode5171 = 5171,

        [Description("門診醫療費用累計不合理")]
        ErrorCode5172 = 5172,

        [Description("住診醫療費用累計不合理")]
        ErrorCode5173 = 5173,

        [Description("安全模組卡的外部認證失敗")]
        ErrorCode6005 = 6005,

        [Description("IDC的外部認證失敗")]
        ErrorCode6006 = 6006,

        [Description("安全模組卡的內部認證失敗")]
        ErrorCode6007 = 6007,

        [Description("寫入讀卡機日期時間失敗")]
        ErrorCode6008 = 6008,

        [Description("IDC驗證簽章失敗")]
        ErrorCode6014 = 6014,

        [Description("檔案大小不合或檔案傳輸失敗")]
        ErrorCode6015 = 6015,

        [Description("記憶體空間不足")]
        ErrorCode6016 = 6016,

        [Description("權限不足無法開啟檔案或找不到檔案")]
        ErrorCode6017 = 6017,

        [Description("傳入參數錯誤")]
        ErrorCode6018 = 6018,

        [Description("送至IDCMeageHeader檢核不符")]
        ErrorCode9001 = 9001,

        [Description("送至健保卡資料中心語法不符")]
        ErrorCode9002 = 9002,

        [Description("與健保卡資料中心作業逾時")]
        ErrorCode9003 = 9003,

        [Description("健保卡資料中心異常無法提供服務")]
        ErrorCode9004 = 9004,

        [Description("健保卡資料中心無法驗證該卡片")]
        ErrorCode9010 = 9010,

        [Description("健保卡資料中心驗證健保卡失敗")]
        ErrorCode9011 = 9011,

        [Description("健保卡資料中心無該卡片資料")]
        ErrorCode9012 = 9012,

        [Description("無效的安全模組卡")]
        ErrorCode9013 = 9013,

        [Description("健保卡資料中心對安全模組卡認證失敗")]
        ErrorCode9014 = 9014,

        [Description("安全模組卡對健保卡資料中心認證失敗")]
        ErrorCode9015 = 9015,

        [Description("健保卡資料中心驗章錯誤")]
        ErrorCode9020 = 9020,

        [Description("無法執行卡片管理系統的認證")]
        ErrorCode9030 = 9030,

        [Description("無法執行健保IC卡AppletPero認證")]
        ErrorCode9040 = 9040,

        [Description("健保卡AppletPero認證失敗")]
        ErrorCode9041 = 9041,

        [Description("無法執行安全模組卡世代碼更新認證")]
        ErrorCode9050 = 9050,

        [Description("安全模組卡世代碼更新認證失敗")]
        ErrorCode9051 = 9051,

        [Description("安全模組卡遭停約處罰")]
        ErrorCode9060 = 9060,

        [Description("安全模組卡不在有效期內")]
        ErrorCode9061 = 9061,

        [Description("安全模組卡合約逾期或尚未生效")]
        ErrorCode9062 = 9062,

        [Description("上傳資料大小不符無法接收檔案")]
        ErrorCode9070 = 9070,

        [Description("上傳日期與資料中心不一致")]
        ErrorCode9071 = 9071,

        [Description("卡片可用次數大於3次,未達可更新標準")]
        ErrorCode9081 = 9081,

        [Description("此卡已被註銷,無法進行卡片更新作業")]
        ErrorCode9082 = 9082,

        [Description("不在保")]
        ErrorCode9083 = 9083,

        [Description("停保中")]
        ErrorCode9084 = 9084,

        [Description("已退保")]
        ErrorCode9085 = 9085,

        [Description("個人欠費")]
        ErrorCode9086 = 9086,

        [Description("負責人欠費")]
        ErrorCode9087 = 9087,

        [Description("投保單位欠費")]
        ErrorCode9088 = 9088,

        [Description("個人及單位均欠費")]
        ErrorCode9089 = 9089,

        [Description("欠費且未在保")]
        ErrorCode9090 = 9090,

        [Description("聲明不實")]
        ErrorCode9091 = 9091,

        [Description("其他")]
        ErrorCode9092 = 9092,

        [Description("藥師藥局無權限")]
        ErrorCode9100 = 9100,

        [Description("保留項目")]
        ErrorCode9101 = 9101,

        [Description("保留項目")]
        ErrorCode9102 = 9102,

        [Description("保留項目")]
        ErrorCode9103 = 9103,

        [Description("保留項目")]
        ErrorCode9104 = 9104,

        [Description("保留項目")]
        ErrorCode9105 = 9105,

        [Description("保留項目")]
        ErrorCode9106 = 9106,

        [Description("保留項目")]
        ErrorCode9107 = 9107,

        [Description("保留項目")]
        ErrorCode9108 = 9108,

        [Description("保留項目")]
        ErrorCode9109 = 9109,

        [Description("保留項目")]
        ErrorCode9110 = 9110,

        [Description("保留項目")]
        ErrorCode9111 = 9111,

        [Description("保留項目")]
        ErrorCode9112 = 9112,

        [Description("保留項目")]
        ErrorCode9113 = 9113,

        [Description("保留項目")]
        ErrorCode9114 = 9114,

        [Description("保留項目")]
        ErrorCode9115 = 9115,

        [Description("保留項目")]
        ErrorCode9116 = 9116,

        [Description("保留項目")]
        ErrorCode9117 = 9117,

        [Description("保留項目")]
        ErrorCode9118 = 9118,

        [Description("保留項目")]
        ErrorCode9119 = 9119,

        [Description("保留項目")]
        ErrorCode9120 = 9120,

        [Description("保留項目")]
        ErrorCode9121 = 9121,

        [Description("保留項目")]
        ErrorCode9122 = 9122,

        [Description("保留項目")]
        ErrorCode9123 = 9123,

        [Description("保留項目")]
        ErrorCode9124 = 9124,

        [Description("保留項目")]
        ErrorCode9125 = 9125,

        [Description("保留項目")]
        ErrorCode = 9126,

        [Description("保留項目")]
        ErrorCode9127 = 9127,

        [Description("保留項目")]
        ErrorCode9128 = 9128,

        [Description("持卡人於非限制院所就診")]
        ErrorCode9129 = 9129,

        [Description("醫事卡失效")]
        ErrorCode9130 = 9130,

        [Description("醫事卡逾效期")]
        ErrorCode9140 = 9140,

        [Description("安全模組檔目錄錯誤或不存在或數量超過一個以上")]
        ErrorCode9200,

        [Description("初始安全模組檔讀取異常，請在 C:\\NHI\\SAM\\COMX1 目錄下放置健保署正確安全模組檔。")]
        ErrorCode9201,

        [Description("安全模組檔讀取異常，已在其它電腦使用過，請在 C:\\NHI\\SAM\\COMX1 目錄下放置健保署正確安全模組檔。")]
        ErrorCode9202,

        [Description("卡片配對錯誤，正式卡與測試卡不能混用 ")]
        ErrorCode9203,

        [Description("找不到讀卡機，或 PCSC 環境異常")]
        ErrorCode9204,

        [Description("開啟讀卡機連結埠失敗")]
        ErrorCode9205,

        [Description("健保 IC 卡內部認證失敗 ")]
        ErrorCode9210,

        [Description("雲端安全模組(IDC)對健保 IC 卡認證失敗")]
        ErrorCode9211,

        [Description("健保 IC 卡對雲端安全模組認證失敗 ")]
        ErrorCode9212,

        [Description("雲端安全模組卡片更新逾時")]
        ErrorCode9213,

        [Description("醫事人員卡內部認證失敗")]
        ErrorCode9220,

        [Description("雲端安全模組(IDC)驗證醫事人員卡失敗")]
        ErrorCode9221,

        [Description("安全模組檔「醫療院所名稱」讀取失敗")]
        ErrorCode9230,

        [Description("安全模組檔「醫療院所簡稱」讀取失敗")]
        ErrorCode9231,

        [Description("雲端安全模組主控台沒啓動")]
        ErrorCode9240
    }

    public enum CardStatusReturnCode
    {
        [Description("卡片未置入")]
        NoCard = 0,

        [Description("健保卡尚未與安全模組認證")]
        SamNonVerify = 1,

        [Description("健保卡與安全模組認證成功 ")]
        SamVerifySuccess = 2,

        [Description("健保卡與醫事人員卡認證成功")]
        HpcVerifySuccess = 3,

        [Description("健保卡PIN認證成功 ")]
        IcPinVerifySuccess = 4,

        [Description("與健保局資料中心認證成功 ")]
        IdcVerifySuccess = 5,

        [Description("所置入非健保卡 ")]
        NotIcCard = 9
    }

    public enum SqlConnectionType
    {
        SqlServer = 0,
        NySql = 1
    }

    public enum HistoryType
    {
        AdjustRecord = 0,
        RegisterRecord = 1,
        ReservedPrescription = 2
    }

    public enum CashFlowType
    {
        Expenses = 0,
        Income = 1
    }

    public enum ServicePoint
    {
        CODE_05202B = 54,
        CODE_05234D = 20,
        CODE_05206B = 65,
        CODE_05223B = 54,
        CODE_05210B = 75
    }
}