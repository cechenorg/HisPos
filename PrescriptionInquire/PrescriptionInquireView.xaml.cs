using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Properties;
using His_Pos.Service;
using ComboBox = System.Windows.Controls.ComboBox;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireView.xaml 的互動邏輯
    /// </summary>
    /// 
    public class DeclareDataList : ObservableCollection<DeclareData>
    {

    }

    public partial class PrescriptionInquireView : UserControl
    {
        public DeclareDataList PrescriptionOutcome = new DeclareDataList();
        private readonly ObservableCollection<Institution> _institutionsList;
        private Function f = new Function();
        //private readonly ObservableCollection<DeclareDetail> _declareDetailList;
        public PrescriptionInquireView()
        {
            InitializeComponent();
            DataContext = this;
            LoadAdjustCases();
            LoadHospitalData();
            _institutionsList = new ObservableCollection<Institution>();
            //_declareDetailList = new ObservableCollection<DeclareDetail>();
            PrescriptionOutcome = (DeclareDataList) this.Resources["PrescriptionOutcome"];
            
        }
        /*
         *載入原處方案件類別
         */
        private void LoadAdjustCases()
        {
            foreach (var adjustCase in AdjustCaseDb.AdjustCaseList)
            {
                AdjustCaseCombo.Items.Add(adjustCase.Id + ". " + adjustCase.Name);
            }
        }

        /*
         *載入醫療院所資料
         */
        private void LoadHospitalData()
        {
            var institutions = new Institutions();
            institutions.GetData();
            ReleasePalace.ItemsSource = institutions.InstitutionsCollection;
        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            row.InputBindings.Add(new MouseBinding(ShowCustomDialogCommand,
                new MouseGesture() {MouseAction = MouseAction.LeftDoubleClick}));
        }

        private ICommand _showCustomDialogCommand;

        private ICommand ShowCustomDialogCommand
        {
            get
            {
                return _showCustomDialogCommand ?? (_showCustomDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()
                });
            }
        }

        private void RunCustomFromVm()
        {
            var selected = (PrescriptionSet.SelectedItem as DeclareData);
            PrescriptionInquireOutcome prescriptionInquireOutcome = new PrescriptionInquireOutcome(selected);
            prescriptionInquireOutcome.Show();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var date = sender as DatePicker;
            SetDecStatus(date.Name.Equals("start") ? "start" : "end");
        }

        private void SetDecStatus(string dateType)
        {
            if (dateType.Equals("start"))
                DeclareStatus.Content = "已選擇 " + start.Text + " ~ ";
            else
            {
                DeclareStatus.Content = "已選擇 " + start.Text + " ~ " + end.Text + " 之處方。";
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            PrescriptionOutcome.Clear();
            var d = new DateTimeExtensions();
            var sDate = d.ToSimpleTaiwanDate(Convert.ToDateTime(start.SelectedDate));
            var eDate = d.ToSimpleTaiwanDate(Convert.ToDateTime(end.SelectedDate));
            var institution = string.Empty;
            if (ReleasePalace.SelectedItem != null)
                institution = ReleasePalace.SelectedItem.ToString().Substring(0, 10);
            var parameters = SetInquireSqlParameters(sDate,eDate, PatientName.Text, PharmacistName.Text, institution);
            GetDataFromXml(parameters);
        }
        /*
         * 設定處方查詢條件Parameters
         */
        private List<SqlParameter> SetInquireSqlParameters(string sDate,string eDate,string cusName,string medicalPerson,string institution)
        {
            return new List<SqlParameter>
            {
                sDate.Equals(string.Empty) ? new SqlParameter("SDATE", DBNull.Value) : new SqlParameter("SDATE", sDate),
                eDate.Equals(string.Empty) ? new SqlParameter("EDATE", DBNull.Value) : new SqlParameter("EDATE", eDate),
                cusName.Equals(string.Empty)
                    ? new SqlParameter("CUSNAME", DBNull.Value)
                    : new SqlParameter("CUSNAME", cusName),
                medicalPerson.Equals(string.Empty)
                    ? new SqlParameter("DOCTOR", DBNull.Value)
                    : new SqlParameter("DOCTOR", medicalPerson),
                institution.Equals(string.Empty)
                    ? new SqlParameter("REALEASEPLACE", DBNull.Value)
                    : new SqlParameter("REALEASEPLACE", institution)
            };
        }

        private void GetDataFromXml(List<SqlParameter> param = null) //將data的XML還原成Declaredata
        {
            var listData = new ObservableCollection<DeclareData>();
            var table = new DataTable();
            if (param != null)
                table = f.GetDataFromProc("[HIS_POS_DB].[GET].[DECLAREDATA]",param);
            if (param == null)
                table = f.GetDataFromProc("[HIS_POS_DB].[GET].[DECLAREDDATATOXML]");
            var parameters = new List<SqlParameter>();
            var xml = new XmlDocument();
            foreach (DataRow row in table.Rows)
            {
                parameters.Clear();
                parameters.Add(new SqlParameter("ID", row["CUS_ID"].ToString()));
                var table1 = f.GetDataFromProc("[HIS_POS_DB].[GET].[CUSDATA]", parameters);
                xml.LoadXml(row["HISDECMAS_DETXML"].ToString());
                var prescription = new Prescription();
                GetTreatmentData(ref prescription,xml, GetCustomer(table1));
                var searchedDeclareData = SetDeclareData(prescription,xml,row);
                listData.Add(searchedDeclareData);
                foreach (var dec in listData)
                {
                    dec.Prescription.Treatment.MedicalInfo.Hospital.Name = GetInstitutionName(dec);
                    dec.Prescription.Treatment.MedicalInfo.TreatmentCase.Name = GetTreatmentCaseName(dec);
                    dec.Prescription.Treatment.Copayment.Name = GetCopaymentName(dec);
                    dec.Prescription.Treatment.MedicalInfo.Hospital.Division.Name = GetDivisionName(dec);
                    PrescriptionOutcome.Add(dec);
                }
            } //GetDataFromXml
        }

        private void GetTreatmentData(ref Prescription prescription,XmlDocument xml,Customer cus)
        {
            prescription.Treatment.Customer = cus;
            prescription.Treatment.AdjustCase.Id = f.GetXmlNodeData(xml, "ddata/dhead/d1");//調劑案件分類
            prescription.Treatment.PaymentCategory.Id = f.GetXmlNodeData(xml, "ddata/dhead/d5");//給付類別
            prescription.IcCard.MedicalNumber = f.GetXmlNodeData(xml, "ddata/dhead/d7");//就醫序號
            prescription.Treatment.TreatmentDate = Convert.ToDateTime(f.GetXmlNodeData(xml, "ddata/dhead/d14"));//就醫(處方)日期
            prescription.Treatment.Copayment.Id = f.GetXmlNodeData(xml, "ddata/dhead/d15");//部分負擔類別
            prescription.Treatment.AdjustDate = Convert.ToDateTime(f.GetXmlNodeData(xml, "ddata/dhead/d23"));//調劑日期
            prescription.Treatment.MedicalPersonId = f.GetXmlNodeData(xml, "ddata/dhead/d25");//醫事人員代號
            prescription.Treatment.MedicineDays = f.GetXmlNodeData(xml, "ddata/dhead/d30");//給藥日分
            SetDiseaseCode(ref prescription,xml);
            SetMedicalInfoData(ref prescription,xml);
        }
        /*
         * 設定診斷代碼
         */
        private void SetDiseaseCode(ref Prescription prescription,XmlDocument xml)
        {
            var main = new DiseaseCode();
            var second = new DiseaseCode();
            if (f.GetXmlNodeData(xml, "ddata/dhead/d8").Equals(string.Empty)) return;
            main.Id = f.GetXmlNodeData(xml, "ddata/dhead/d8");
            prescription.Treatment.MedicalInfo.DiseaseCodes.Add(main);
            if (f.GetXmlNodeData(xml, "ddata/dhead/d9").Equals(string.Empty)) return;
            second.Id = f.GetXmlNodeData(xml, "ddata/dhead/d9");
            prescription.Treatment.MedicalInfo.DiseaseCodes.Add(second);
        }
        /*
         * 設定MedicalInfo
         */
        private void SetMedicalInfoData(ref Prescription prescription,XmlDocument xml)
        {
            prescription.Treatment.MedicalInfo.Hospital.Division.Id = f.GetXmlNodeData(xml, "ddata/dhead/d13");//就醫科別
            prescription.Treatment.MedicalInfo.Hospital.Id = f.GetXmlNodeData(xml, "ddata/dhead/d21");//釋出院所
            prescription.Treatment.MedicalInfo.TreatmentCase.Id = f.GetXmlNodeData(xml, "ddata/dhead/d22");//處方案件分類
            prescription.Treatment.MedicalInfo.Hospital.Doctor.Id = f.GetXmlNodeData(xml, "ddata/dhead/d24"); //診治醫師代號
            prescription.Treatment.MedicalInfo.SpecialCode.Id = f.GetXmlNodeData(xml, "ddata/dhead/d26");//特定治療代碼
        }
        /*
         * 取得病人資料
         */
        private Customer GetCustomer(DataTable table)
        {
            return new Customer
            {
                Id = table.Rows[0]["CUS_ID"].ToString(),
                Name = table.Rows[0]["CUS_NAME"].ToString(),
                Qname = table.Rows[0]["CUS_QNAME"].ToString(),
                Birthday = table.Rows[0]["CUS_BIRTH"].ToString(),
                ContactInfo =
                {
                    Address = table.Rows[0]["CUS_ADDR"].ToString(),
                    Tel = table.Rows[0]["CUS_TEL"].ToString(),
                    Email = table.Rows[0]["CUS_EMAIL"].ToString()
                },
                IcNumber = table.Rows[0]["CUS_IDNUM"].ToString(),
                Gender = Convert.ToBoolean(table.Rows[0]["CUS_GENDER"].ToString())
            };
        }

        private DeclareData SetDeclareData(Prescription prescription,XmlDocument xml,DataRow row)
        {
            var d35 = f.GetXmlNodeData(xml, "ddata/dhead/d35");
            var d36 = f.GetXmlNodeData(xml, "ddata/dhead/d36");
            var searchedDeclareData = new DeclareData(prescription);
            var pelemList = xml.GetElementsByTagName("pdata");
            searchedDeclareData.Id = row["HISDECMAS_ID"].ToString();
            searchedDeclareData.StatusFlag = row["HISDECMAS_FLAG"].ToString();
            searchedDeclareData.Xml.LoadXml(row["HISDECMAS_DETXML"].ToString());
            searchedDeclareData.ChronicSequence = d35;
            searchedDeclareData.ChronicTotal = d36;
            for (var i = 0; i < pelemList.Count; i++)
            {
                var detail = GetDeclareDetail(xml);
                searchedDeclareData.DeclareDetails.Add(detail);
            }
            return searchedDeclareData;
        }

        /*
         * 從XmlDocument取得DeclareData資料並初始化
         */
        private DeclareDetail GetDeclareDetail(XmlDocument xml)
        {
            var sDate = f.GetXmlNodeData(xml, "ddata/pdata/p12");
            var eDate = f.GetXmlNodeData(xml, "ddata/pdata/p13");
            var medicalPersonId = f.GetXmlNodeData(xml, "ddata/pdata/p14");
            return new DeclareDetail
            {
                MedicalOrder = f.GetXmlNodeData(xml, "ddata/pdata/p1"),
                MedicalId = f.GetXmlNodeData(xml, "ddata/pdata/p2"),
                Dosage = Convert.ToDouble(xml.SelectSingleNode("ddata/pdata/p3")?.InnerText),
                Usage = f.GetXmlNodeData(xml, "ddata/pdata/p4"),
                Position = f.GetXmlNodeData(xml, "ddata/pdata/p5"),
                Percent = Convert.ToDouble(f.GetXmlNodeData(xml, "ddata/pdata/p6")),
                Total = Convert.ToDouble(f.GetXmlNodeData(xml, "ddata/pdata/p7") ?? throw new InvalidOperationException()),
                Price = Convert.ToDouble(f.GetXmlNodeData(xml, "ddata/pdata/p8") ?? throw new InvalidOperationException()),
                Point = Convert.ToDouble(f.GetXmlNodeData(xml, "ddata/pdata/p9") ?? throw new InvalidOperationException()),
                Sequence = int.Parse(f.GetXmlNodeData(xml, "ddata/pdata/p10") ?? throw new InvalidOperationException()),
                StartDate = sDate,
                EndDate = eDate,
                MedicalPersonnelId = medicalPersonId
            };
        }
        /*
         * 取得院所名稱
         */
        private string GetInstitutionName(DeclareData dec)
        {
            var paramfordata = new List<SqlParameter>();
            paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.MedicalInfo.Hospital.Id));
            var table = f.GetDataFromProc("[HIS_POS_DB].[GET].[INSNAME]", paramfordata);
            return table.Rows[0]["INS_NAME"].ToString();
        }
        /*
         * 取得原處方案件名稱
         */
        private string GetTreatmentCaseName(DeclareData dec)
        {
            var paramfordata = new List<SqlParameter>();
            paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.MedicalInfo.TreatmentCase.Id));
            var table = f.GetDataFromProc("[HIS_POS_DB].[GET].[CASNAME]", paramfordata);
            return table.Rows[0]["HISMEDCAS_NAME"].ToString();
        }
        /*
         * 取得部分負擔代碼對應名稱
         */
        private string GetCopaymentName(DeclareData dec)
        {
            var paramfordata = new List<SqlParameter>();
            paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.Copayment.Id));
            var table = f.GetDataFromProc("[HIS_POS_DB].[GET].[HISCOPNAME]", paramfordata);
            return table.Rows[0]["HISCOP_NAME"].ToString();
        }
        /*
         * 取得就醫科別名稱
         */
        private string GetDivisionName(DeclareData dec)
        {
            var paramfordata = new List<SqlParameter>();
            paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.MedicalInfo.Hospital.Division.Id));
            var table = f.GetDataFromProc("[HIS_POS_DB].[GET].[DIVNAME]", paramfordata);
            return table.Rows[0]["HISDIV_NAME"].ToString();
        }

        private void Combo_DropDownOpened(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            Debug.Assert(comboBox != null, nameof(comboBox) + " != null");
            comboBox.Background = Brushes.WhiteSmoke;
            comboBox.IsReadOnly = false;
        }
        private void Combo_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            Debug.Assert(comboBox != null, nameof(comboBox) + " != null");
            comboBox.Background = Brushes.Transparent;
            comboBox.Text = comboBox.SelectedItem?.ToString() ?? "";
            comboBox.IsReadOnly = true;
        }
    }
}
