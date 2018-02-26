using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Properties;
using His_Pos.Service;
using Label = System.Reflection.Emit.Label;
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
        private readonly ObservableCollection<DeclareDetail> _declareDetailList;

        public PrescriptionInquireView()
        {
            InitializeComponent();
            _institutionsList = new ObservableCollection<Institution>();
            _declareDetailList = new ObservableCollection<DeclareDetail>();
            PrescriptionOutcome = (DeclareDataList) this.Resources["PrescriptionOutcome"];
            DataContext = this;
            DataInitialize();
        }

        private void DataInitialize()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[INSTITUTION]");
            foreach (DataRow d in table.Rows)
            {
                var institution = new Institution(d["INS_ID"].ToString(), d["INS_NAME"].ToString());
                _institutionsList.Add(institution);
            }
            ReleasePalace.ItemsSource = _institutionsList;
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

        private void DeclareFileImportButtonClick(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var folderPath = fbd.SelectedPath;
                    foreach (var file in Directory.EnumerateFiles(folderPath, "*.xml"))
                    {
                        var xmlReader = new XmlTextReader(file);
                        while (xmlReader.Read())
                        {
                            xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                            if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name.Equals("pharmacy"))
                                break;
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {

                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            PrescriptionOutcome.Clear();
            var function = new Function();
            var d = new DateTimeExtensions();
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy/mm/dd";
            Thread.CurrentThread.CurrentCulture = ci;
            var sDate = d.ToSimpleTaiwanDate(Convert.ToDateTime(start.SelectedDate));
            var eDate = d.ToSimpleTaiwanDate(Convert.ToDateTime(end.SelectedDate));
            var cusName = PatientName.Text;
            var pharmacist = PharmacistName.Text;
            var releasePalace = string.Empty;
            if (ReleasePalace.SelectedItem != null)
                releasePalace = ReleasePalace.SelectedItem.ToString().Substring(0, 10);
            var parameters = new List<SqlParameter>
            {
                sDate.Equals(string.Empty) ? new SqlParameter("SDATE", DBNull.Value) : new SqlParameter("SDATE", sDate),
                eDate.Equals(string.Empty) ? new SqlParameter("EDATE", DBNull.Value) : new SqlParameter("EDATE", eDate),
                cusName.Equals(string.Empty)
                    ? new SqlParameter("CUSNAME", DBNull.Value)
                    : new SqlParameter("CUSNAME", cusName),
                pharmacist.Equals(string.Empty)
                    ? new SqlParameter("DOCTOR", DBNull.Value)
                    : new SqlParameter("DOCTOR", pharmacist),
                releasePalace.Equals(string.Empty)
                    ? new SqlParameter("REALEASEPLACE", DBNull.Value)
                    : new SqlParameter("REALEASEPLACE", releasePalace)
            };
            GetDataFromXml(parameters);
        }

        public void GetDataFromXml(List<SqlParameter> param = null) //將data的XML還原成Declaredata
        {
            var listData = new ObservableCollection<DeclareData>();
            var conn = new DbConnection(Settings.Default.SQL_local);
            var paramfordata = new List<SqlParameter>();
            var table = new DataTable();
            if (param != null)
                table = conn.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREDATA]", param);
            if (param == null)
                table = conn.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREDDATATOXML]");
            var parameters = new List<SqlParameter>();
            var xml = new XmlDocument();
            string _p12, _p13, _p14;
            foreach (DataRow row in table.Rows)
            {
                parameters.Clear();
                parameters.Add(new SqlParameter("ID", row["CUS_ID"].ToString()));
                var table1 = conn.ExecuteProc("[HIS_POS_DB].[GET].[CUSDATA]", parameters);
                var cus = GetCustomer(table1);
                xml.LoadXml(row["HISDECMAS_DETXML"].ToString());
                var elemList = xml.GetElementsByTagName("dhead");
                var d5 = !elemList[0].InnerXml.Contains("d5") ? "" : xml.SelectSingleNode("ddata/dhead/d5")?.InnerText;
                var d35 = !elemList[0].InnerXml.Contains("d35")? "0": xml.SelectSingleNode("ddata/dhead/d35")?.InnerText;
                var d36 = !elemList[0].InnerXml.Contains("d36")? "0": xml.SelectSingleNode("ddata/dhead/d36").InnerText;
                var mainDiseaseCode = new DiseaseCode();
                var secontDiseaseCode = new DiseaseCode();

                var prescription = new Prescription();
                prescription.Treatment.AdjustCase.Id = xml.SelectSingleNode("ddata/dhead/d1").InnerText; //調劑案件分類
                prescription.Treatment.PaymentCategory.Id = d5; //給付類別
                prescription.Treatment.MedicalInfo.Hospital.Id =
                    xml.SelectSingleNode("ddata/dhead/d21").InnerText; //釋出院所
                prescription.Treatment.MedicalInfo.TreatmentCase.Id =
                    xml.SelectSingleNode("ddata/dhead/d22").InnerText; //處方案件分類
                prescription.Treatment.AdjustDate = xml.SelectSingleNode("ddata/dhead/d23").InnerText; //調劑日期
                prescription.Treatment.Customer = cus;
                prescription.IcCard.MedicalNumber = xml.SelectSingleNode("ddata/dhead/d7").InnerText; //就醫序號
                prescription.Treatment.Copayment.Id = xml.SelectSingleNode("ddata/dhead/d15").InnerText; //部分負擔類別
                prescription.Treatment.MedicalPersonId = xml.SelectSingleNode("ddata/dhead/d25").InnerText; //醫事人員代號
                prescription.Treatment.MedicalInfo.Hospital.Division.Id =
                    xml.SelectSingleNode("ddata/dhead/d13").InnerText; //就醫科別
                prescription.Treatment.TreatmentDate = xml.SelectSingleNode("ddata/dhead/d14").InnerText; //就醫(處方)日期
                prescription.Treatment.MedicineDays = xml.SelectSingleNode("ddata/dhead/d30").InnerText; //給藥日分
                if (xml.SelectSingleNode("ddata/dhead/d8")?.InnerText.Equals(string.Empty) == false)
                {
                    mainDiseaseCode.Id = xml.SelectSingleNode("ddata/dhead/d8")?.InnerText;
                    prescription.Treatment.MedicalInfo.DiseaseCodes.Add(mainDiseaseCode);
                    if (xml.SelectSingleNode("ddata/dhead/d9")?.InnerText.Equals(string.Empty) == false)
                    {
                        secontDiseaseCode.Id = xml.SelectSingleNode("ddata/dhead/d9")?.InnerText;
                        prescription.Treatment.MedicalInfo.DiseaseCodes.Add(secontDiseaseCode);
                    }
                }
                prescription.Treatment.MedicalInfo.SpecialCode.Id = xml.SelectSingleNode("ddata/dhead/d26")?.InnerText;
                prescription.Treatment.MedicalInfo.Hospital.Doctor.Id =
                    xml.SelectSingleNode("ddata/dhead/d24")?.InnerText; //診治醫師代號;

                var searchedDeclareData = new DeclareData(prescription);
                searchedDeclareData.Id = row["HISDECMAS_ID"].ToString();
                searchedDeclareData.StatusFlag = row["HISDECMAS_FLAG"].ToString();
                searchedDeclareData.Xml.LoadXml(row["HISDECMAS_DETXML"].ToString());
                searchedDeclareData.ChronicSequence = d35;
                searchedDeclareData.ChronicTotal = d36;
                var pelemList = xml.GetElementsByTagName("pdata");
                for (var i = 0; i < pelemList.Count; i++)
                {
                    if (pelemList[i].InnerXml.Contains("p12")) _p12 = "";
                    else _p12 = xml.SelectSingleNode("ddata/pdata/p12")?.InnerText;
                    if (pelemList[i].InnerXml.Contains("p13")) _p13 = "";
                    else _p13 = xml.SelectSingleNode("ddata/pdata/p13")?.InnerText;
                    if (pelemList[i].InnerXml.Contains("p14")) _p14 = "";
                    else _p14 = xml.SelectSingleNode("ddata/pdata/p14")?.InnerText;
                    var detail = new DeclareDetail();
                    detail.MedicalOrder = xml.SelectSingleNode("ddata/pdata/p1")?.InnerText;
                    detail.MedicalId = xml.SelectSingleNode("ddata/pdata/p2")?.InnerText;
                    detail.Dosage = Convert.ToDouble(xml.SelectSingleNode("ddata/pdata/p3")?.InnerText);
                    detail.Usage = xml.SelectSingleNode("ddata/pdata/p4")?.InnerText;
                    detail.Position = xml.SelectSingleNode("ddata/pdata/p5")?.InnerText;
                    detail.Percent = Convert.ToDouble(xml.SelectSingleNode("ddata/pdata/p6")?.InnerText);
                    detail.Total = Convert.ToDouble(xml.SelectSingleNode("ddata/pdata/p7")?.InnerText ??
                                                    throw new InvalidOperationException());
                    detail.Price = Convert.ToDouble(xml.SelectSingleNode("ddata/pdata/p8")?.InnerText ??
                                                    throw new InvalidOperationException());
                    detail.Point = Convert.ToDouble(xml.SelectSingleNode("ddata/pdata/p9")?.InnerText ??
                                                    throw new InvalidOperationException());
                    detail.Sequence = int.Parse(xml.SelectSingleNode("ddata/pdata/p10")?.InnerText ??
                                                throw new InvalidOperationException());
                    detail.StartDate = _p12;
                    detail.EndDate = _p13;
                    detail.MedicalPersonnelId = _p14;
                    listData.Add(searchedDeclareData);
                }
                foreach (var dec in listData)
                {
                    paramfordata.Clear();
                    paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.MedicalInfo.Hospital.Id));
                    table = conn.ExecuteProc("[HIS_POS_DB].[GET].[INSNAME]", paramfordata);
                    dec.Prescription.Treatment.MedicalInfo.Hospital.Name = table.Rows[0]["INS_NAME"].ToString();

                    paramfordata.Clear();
                    paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.MedicalInfo.TreatmentCase.Id));
                    table.Clear();
                    table = conn.ExecuteProc("[HIS_POS_DB].[GET].[CASNAME]", paramfordata);
                    dec.Prescription.Treatment.MedicalInfo.TreatmentCase.Name =
                        table.Rows[0]["HISMEDCAS_NAME"].ToString();

                    paramfordata.Clear();
                    paramfordata.Add(new SqlParameter("ID", dec.Prescription.Treatment.Copayment.Id));
                    table.Clear();
                    table = conn.ExecuteProc("[HIS_POS_DB].[GET].[HISCOPNAME]", paramfordata);
                    dec.Prescription.Treatment.Copayment.Name = table.Rows[0]["HISCOP_NAME"].ToString();

                    paramfordata.Clear();
                    paramfordata.Add(
                        new SqlParameter("ID", dec.Prescription.Treatment.MedicalInfo.Hospital.Division.Id));
                    table.Clear();
                    table = conn.ExecuteProc("[HIS_POS_DB].[GET].[DIVNAME]", paramfordata);
                    dec.Prescription.Treatment.MedicalInfo.Hospital.Division.Name =
                        table.Rows[0]["HISDIV_NAME"].ToString();
                    PrescriptionOutcome.Add(dec);
                }
            } //GetDataFromXml
        }

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
    }
}
