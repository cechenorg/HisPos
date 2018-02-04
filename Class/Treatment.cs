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
        public PaymentCategory PaymentCategory { get; set; }//d5 給付類別
        public Copayment Copayment { get; set; }//d15 部分負擔代碼
        public AdjustCase AdjustCase { get; set; }//d1 案件分類
        public string TreatmentDate { get; set; }//d14 就醫(處方)日期
        public string AdjustDate { get; set; }//d23 調劑日期
        public string MedicineDays { get; set; }//d30  給藥日份

        public XmlDocument CreateToXml()
        {
            string pData, dData;
            var xml = new XmlDocument();
            dData = "<ddata><dhead>";
            dData += "<d1>" + AdjustCase.Name + "</d1>";
            dData += "<d2></d2>";
            dData += "<d3>" + D3 + "</d3>";
            //D4 : 補報原因 非補報免填
            if (D4 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d4>" + D4 + "</d4>";
            if (D5 != DBNull.Value.ToString(CultureInfo.CurrentCulture) && (D1.Equals("2") || D1.Equals("D")))
                dData += "<d5>" + D5 + "</d5>";
            dData += "<d6>" + D6 + "</d6>";
            dData += "<d7>" + D7 + "</d7>";
            /*
             D8 ~ D12 國際疾病代碼
             */
            if (D8 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d8>" + D8 + "</d8>";
            if (D9 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d9>" + D9 + "</d9>";
            if (D10 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d10>" + D10 + "</d10>";
            if (D11.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d11>" + D11 + "</d11>";
            if (D12 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d12>" + D12 + "</d12>";
            if (D13 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d13>" + D13 + "</d13>";
            if (D14 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d14>" + D14 + "</d14>";
            dData += "<d15>" + D15 + "</d15>";
            dData += "<d16>" + D16 + "</d16>";
            dData += "<d17>" + D17 + "</d17>";
            dData += "<d18>" + D18 + "</d18>";
            if (D19.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d19>" + D19 + "</d19>";
            dData += "<d20>" + D20 + "</d20>";
            dData += "<d21>" + D21 + "</d21>";
            if (D22 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d22>" + D22 + "</d22>";
            dData += "<d23>" + D23 + "</d23>";
            if (D24 != DBNull.Value.ToString())
                dData += "<d24>" + D24 + "</d24>";
            dData += "<d25>" + D25 + "</d25>";
            if (D26 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d26>" + D26 + "</d26>";
            if (D27 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d27>" + D27 + "</d27>";
            if (D28 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d28>" + D28 + "</d28>";
            if (D29 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d29>" + D29 + "</d29>";
            if (D30.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d30>" + D30 + "</d30>";
            if (D31.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d31>" + D31 + "</d31>";
            if (D32.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d32>" + D32 + "</d32>";
            if (D33.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d33>" + D33 + "</d33>";
            //免填 dData += "<d34>" + "" + "</d34>";
            if (_d1.Equals("2"))
            {
                dData += "<d35>" + D35 + "</d35>";
                dData += "<d36>" + D36 + "</d36>";
            }
            //待確認
            /*if (d37.ToString() != DBNull.Value.ToString())*/
            dData += "<d37>" + D37 + "</d37>";
            /*if (d38.ToString() != DBNull.Value.ToString())*/
            dData += "<d38>" + D38 + "</d38>";
            //D39~D42免填
            //dData += "<d39>" + "" + "</d39>";      
            //dData += "<d40>" + "" + "</d40>";         
            //dData += "<d43>" + "" + "</d43>";
            //dData += "<d41>" + "" + "</d41>";
            //dData += "<d42>" + "" + "</d42>";
            /*if (d44.ToString() != DBNull.Value.ToString())*/
            if (D1.Equals("2") && Convert.ToDecimal(D35) >= 2)
                dData += "<d43>" + D43 + "</d43>";
            //待確認 新生兒註記就醫
            dData += "<d44>" + "" + "</d44>";
            //待確認 矯正機關代號
            dData += "<d45>" + "" + "</d45>";
            //特定地區醫療服務 免填
            //dData += "<d46>" + "" + "</d46>";
            dData += "</dhead>";
            foreach (var detail in _details)
            {
                pData = "<pdata>";
                pData += "<p1>" + detail.P1 + "</p1>";
                pData += "<p2>" + detail.P2 + "</p2>";
                pData += "<p7>" + detail.P7 + "</p7>";
                pData += "<p8>" + detail.P8 + "</p8>";
                pData += "<p9>" + detail.P9 + "</p9>";
                if (detail.P3 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p3>" + detail.P3 + "</p3>";
                if (detail.P4 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p4>" + detail.P4 + "</p4>";
                if (detail.P5 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p5>" + detail.P5 + "</p5>";
                if (detail.P6 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p6>" + detail.P6 + "</p6>";
                pData += "<p10>" + detail.P10 + "</p10>";
                if (detail.P11.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                {
                    pData += "<p11>" + detail.P11 + "</p11>";
                    if (_d30 < detail.P11)
                        _d30 = detail.P11;
                }
                pData += "</pdata>";
                dData += pData;
            }

            dData += "</ddata>";
            xml.LoadXml(dData);
            return xml;
        }

    }
}
