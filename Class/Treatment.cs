﻿using His_Pos.Class.Person;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class
{
    public class Treatment
    {
        //d8 d9 國際疾病分類碼 d13就醫科別  d21原處方服務機構代號 d22原處方服務機構之案件分類 d24診治醫師代號 d26原處方服務機構之特定治療項目代號
        public MedicalInfo MedicalInfo { get; set; }
        public PaymentCategory.PaymentCategory PaymentCategory { get; set; }//d5 給付類別
        public Copayment.Copayment Copayment { get; set; }//d15 部分負擔代碼
        public AdjustCase.AdjustCase AdjustCase { get; set; }//d1 案件分類
        public string TreatmentDate { get; set; }//d14 就醫(處方)日期
        public string AdjustDate { get; set; }//d23 調劑日期
        public string MedicineDays { get; set; }//d30  給藥日份
        public string MedicalPersonId { get; set; }//d25 醫事人員代號
        public Customer Customer { get; set; } //病患資料
       

    }
}
