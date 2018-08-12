using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.Class;
using His_Pos.Class.MedBag;
using His_Pos.Class.MedBagLocation;
using His_Pos.RDLC;
using His_Pos.Service;
using JetBrains.Annotations;
using Microsoft.Reporting.WinForms;
using Border = His_Pos.RDLC.Border;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using DataGrid = System.Windows.Controls.DataGrid;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Page = His_Pos.RDLC.Page;
using RadioButton = System.Windows.Controls.RadioButton;
using Report = His_Pos.RDLC.Report;
using Style = His_Pos.RDLC.Style;

namespace His_Pos.H1_DECLARE.MedBagManage
{
    /// <summary>
    ///     MedBagManageView.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagManageView : INotifyPropertyChanged
    {
        private const string ReportPath = @"..\..\RDLC\MedBagReport.rdlc";
        public static MedBagManageView Instance;
        private bool _checkMulti;
        private bool _checkSingle;

        private int _id;
        private ObservableCollection<MedBag> _medBagCollection;

        private double _medBagImgHeight;

        private double _medBagImgWidth;
        private MedBagMode _mode;
        private MedBag _selectedMedBag;

        public MedBagManageView()
        {
            InitializeComponent();
            Instance = this;
            DataContext = this;
            InitializeMedManageViewControl();
        }

        public bool CheckSingle
        {
            get => _checkSingle;
            set
            {
                _checkSingle = value;
                OnPropertyChanged(nameof(CheckSingle));
            }
        }

        public bool CheckMulti
        {
            get => _checkMulti;
            set
            {
                _checkMulti = value;
                OnPropertyChanged(nameof(CheckMulti));
            }
        }

        public MedBagMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                if (_mode == MedBagMode.SINGLE)
                {
                    CheckSingle = true;
                    CheckMulti = false;
                }
                else
                {
                    CheckSingle = false;
                    CheckMulti = true;
                }
            }
        }

        public ObservableCollection<MedBag> MedBagCollection
        {
            get => _medBagCollection;
            set
            {
                _medBagCollection = value;
                OnPropertyChanged(nameof(MedBagCollection));
            }
        }

        public MedBag SelectedMedBag
        {
            get => _selectedMedBag;
            set
            {
                _selectedMedBag = value;
                OnPropertyChanged(nameof(SelectedMedBag));
            }
        }

        public double MedBagImgWidth
        {
            get => _medBagImgWidth;
            set
            {
                _medBagImgWidth = value;
                OnPropertyChanged(nameof(MedBagImgWidth));
            }
        }

        public double MedBagImgHeight
        {
            get => _medBagImgHeight;
            set
            {
                _medBagImgHeight = value;
                OnPropertyChanged(nameof(MedBagImgHeight));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeMedManageViewControl()
        {
            Mode = MedBagMode.SINGLE;
            SelectedMedBag = new MedBag(Mode);
            MedBagCollection = MedBagDb.ObservableGetMedBagData();
            MedBags.ItemsSource = MedBagCollection;
            if (MedBags.Items.Count != 0)
                MedBags.SelectedIndex = 0;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ImageSelector_Click(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            var dlg = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = @"Image files (*.jpg;*.png)|*.jpg|All Files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var selectedFileName = dlg.FileName;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                SelectedMedBag.MedBagImage = bitmap;
                SetMedBagRange();
            }
        }

        private void SetMedBagRange()
        {
            if (SelectedMedBag.MedBagImage.Width >= SelectedMedBag.MedBagImage.Height)
            {
                MedBagImgWidth = 935;
                MedBagImgHeight = 935 * (SelectedMedBag.MedBagImage.Height / SelectedMedBag.MedBagImage.Width);
                while (MedBagImgHeight > MedBagImg.MaxHeight)
                {
                    MedBagImgWidth *= 0.9;
                    MedBagImgHeight *= 0.9;
                }
            }
            else
            {
                MedBagImgHeight = 850;
                MedBagImgWidth = 850 * (SelectedMedBag.MedBagImage.Width / SelectedMedBag.MedBagImage.Height);
                while (MedBagImgWidth > MedBagImg.MaxWidth)
                {
                    MedBagImgWidth *= 0.9;
                    MedBagImgHeight *= 0.9;
                }
            }

            ImgWrap.Width = MedBagImgWidth;
            ImgWrap.Height = MedBagImgHeight;

            MedBagCanvas.Width = MedBagImgWidth;
            MedBagCanvas.Height = MedBagImgHeight;

            MedRangeLocationControl.Template = (ControlTemplate) FindResource("MedBagRangeItemTemplate");
            MedRangeLocationControl.SetValue(Canvas.LeftProperty, 0.0);
            MedRangeLocationControl.SetValue(Canvas.TopProperty, 0.0);
            MedRangeLocationControl.SetValue(WidthProperty, MedBagImgWidth);
            MedRangeLocationControl.SetValue(HeightProperty, MedBagImgHeight);
        }

        private void NewLocationClick(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            var c = sender as CheckBox;
            if (c.IsChecked == true)
            {
                var locationName = (sender as CheckBox)?.Content.ToString();
                var controlName = (sender as CheckBox)?.Name;
                Instance.NewLocation(null, locationName, controlName);
            }
            else
            {
                MedBagCanvas.Children.Remove(MedBagCanvas.Children.OfType<ContentControl>()
                    .Where(r => r.Content is RdlLocationControl)
                    .Single(r => (r.Content as RdlLocationControl).LabelContent.Equals(c.Content)));
            }
        }

        public void NewLocation(string locid = null, string content = null, string controlName = null,
            double height = 0, double width = 0, double top = 0, double left = 0)
        {
            var contentControl = new ContentControl();
            RdlLocationControl newLocation;
            if (locid != null)
            {
                newLocation = new RdlLocationControl(Convert.ToInt32(locid), content, controlName);
                _id = Convert.ToInt32(locid);
                _id++;
            }
            else
            {
                newLocation = new RdlLocationControl(_id, content, controlName);
                _id++;
            }

            if (string.IsNullOrEmpty(content))
            {
                contentControl.Template = (ControlTemplate) FindResource("MedBagRangeItemTemplate");
                contentControl.Height = height == 0 ? 150 : height;
                contentControl.Width = width == 0 ? 150 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, top == 0 ? 360 : top);
                Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            }
            else
            {
                contentControl.Template = (ControlTemplate) FindResource("MedBagDesignerItemTemplate");
                contentControl.Height = height == 0 ? 15 : height;
                contentControl.Width = width == 0 ? 60 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, top == 0 ? 360 : top);
                Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            }
        }

        public void SetLocation(MedBagLocation m)
        {
            if (!string.IsNullOrEmpty(m.Content))
                NewFunction.FindChild<CheckBox>(CheckBoxStack, m.Name).IsChecked = true;
            var contentControl =
                new ContentControl {Template = (ControlTemplate) FindResource("MedBagDesignerItemTemplate")};
            var newLocation = new RdlLocationControl(m.Id, m.Content, m.Name);
            contentControl.Height = m.Height;
            contentControl.Width = m.Width;
            contentControl.Content = newLocation;
            MedBagCanvas.Children.Add(contentControl);
            Canvas.SetTop(contentControl, m.CanvasTop);
            Canvas.SetLeft(contentControl, m.CanvasLeft);
        }

        private void SaveMedBagData()
        {
            if (CheckMedBagCollectionEmpty())
                return;
            SelectedMedBag.SetLocationCollection(MedBagCanvas.Children);
            MedBagDb.SaveMedBagData(SelectedMedBag);
        }

        private void MedBagSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (MedBagCollection[MedBags.SelectedIndex] == null)
                return;
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            SaveMedBagData();
            File.WriteAllText(ReportPath, string.Empty);
            File.AppendAllText(ReportPath, SerializeObject<Report>(CreatReport()));
            CreatePdf();
            var m = new MessageWindow("藥袋儲存成功", MessageType.SUCCESS);
            m.Show();
        }

        //新增/刪除藥袋
        private void NewMedBagButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Name.Contains("Add"))
            {
                MedBagCollection.Add(new MedBag(_mode));
            }
            else
            {
                if (CheckMedBagCollectionEmpty())
                    return;
                var i = MedBags.SelectedIndex;
                if (i > 0)
                    MedBags.SelectedItem = MedBagCollection[i - 1];
                MedBagCollection.Remove(MedBagCollection[i]);
            }
        }

        //設定預設藥袋
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            SelectedMedBag.Default = true;
        }

        //取消預設藥袋
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            SelectedMedBag.Default = false;
        }

        private void MedBags_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SelectedMedBag = ((DataGrid) sender).SelectedItem as MedBag;
            if (SelectedMedBag != null && SelectedMedBag.MedLocations.Count > 0)
            {
                if (SelectedMedBag.MedBagImage != null)
                    SetMedBagRange();
                foreach (var location in SelectedMedBag.MedLocations)
                    SetLocation(location);
            }
        }

        private void CleanButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedMedBag = new MedBag(_mode);
            MedBagCollection[MedBags.SelectedIndex] = SelectedMedBag;
        }

        private bool CheckMedBagCollectionEmpty()
        {
            var m = new MessageWindow("未新增藥袋", MessageType.WARNING);
            m.SetLabelFontSize(16);
            m.SetLabelContentAlignment(HorizontalAlignment.Center);
            if (MedBagCollection.Count == 0)
            {
                m.Show();
                return true;
            }

            return false;
        }

        private void SetReportItem(Report medBagReport, ObservableCollection<MedBagLocation> locations)
        {
            foreach (var m in locations)
                if (m.Name != "MedicineList")
                    medBagReport.Body.ReportItems.Textbox.Add(CreatTextBoxField(m));
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox c)) return;
            if (c.Name.Equals("SingleMed"))
                Mode = MedBagMode.SINGLE;
            else
                Mode = MedBagMode.MULTI;
            SelectedMedBag.Mode = Mode;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
                Mode = (MedBagMode) short.Parse(radioButton.Tag.ToString());

            if (MedBags is null) return;
            MedBags.Items.Filter = ModeFilter;
        }

        private bool ModeFilter(object item)
        {
            if (Mode == MedBagMode.SINGLE)
                return true;
            return ((MedBag) item).Mode == Mode;
        }

        private Report CreatReport()
        {
            var medBagReport = new Report
            {
                Xmlns = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition",
                Rd = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner",
                Body = new Body
                {
                    ReportItems = new ReportItems(),
                    Height = SelectedMedBag.BagHeight + "cm",
                    Style = new Style()
                },
                Page = new Page
                {
                    PageHeight = SelectedMedBag.BagHeight.ToString(CultureInfo.InvariantCulture) + "cm",
                    PageWidth = SelectedMedBag.BagWidth.ToString(CultureInfo.InvariantCulture) + "cm",
                    Style = string.Empty,
                    LeftMargin = "0cm",
                    RightMargin = "0cm",
                    TopMargin = "0cm",
                    BottomMargin = "0cm",
                    ColumnSpacing = "0cm"
                },
                Width = SelectedMedBag.BagWidth.ToString(CultureInfo.InvariantCulture) + "cm",
                AutoRefresh = "0",
                ReportUnitType = "cm",
                ReportID = "cdd7925b-803a-4208-8788-8e2ae4bd14b8"
            };
            SetReportItem(medBagReport, SelectedMedBag.MedLocations);
            return medBagReport;
        }

        private static Textbox CreatTextBoxField(MedBagLocation m)
        {
            return new Textbox
            {
                Name = m.Name,
                DefaultName = m.Name,
                CanGrow = "true",
                KeepTogether = "true",
                Top = m.PathY.ToString(CultureInfo.InvariantCulture) + "cm",
                Left = m.PathX.ToString(CultureInfo.InvariantCulture) + "cm",
                Height = m.RealHeight.ToString(CultureInfo.InvariantCulture) + "cm",
                Width = m.RealWidth.ToString(CultureInfo.InvariantCulture) + "cm",
                Paragraphs = new Paragraphs
                {
                    Paragraph = new Paragraph
                    {
                        Style = string.Empty,
                        TextRuns = new TextRuns
                        {
                            TextRun = new TextRun
                            {
                                Value = m.Name,
                                Style = string.Empty
                            }
                        }
                    }
                },
                Style = new Style
                {
                    Border = new Border {Style = "None"},
                    PaddingLeft = "2pt",
                    PaddingRight = "2pt",
                    PaddingTop = "2pt",
                    PaddingBottom = "2pt"
                }
            };
        }

        public string SerializeObject<T>(Report report)
        {
            var xmlSerializer = new XmlSerializer(report.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, report);
                return PrettyXml(textWriter);
            }
        }

        private static string PrettyXml(StringWriter writer)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(writer.ToString());

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        private void CreatePdf()
        {
            var mimeType = string.Empty;
            var encoding = string.Empty;
            var extension = string.Empty;
            var deviceInfo = "<DeviceInfo>" +
                             "  <OutputFormat>PDF</OutputFormat>" +
                             "  <PageWidth>" + SelectedMedBag.BagWidth + "cm</PageWidth>" +
                             "  <PageHeight>" + SelectedMedBag.BagHeight + "cm</PageHeight>" +
                             "  <MarginTop>0cm</MarginTop>" +
                             "  <MarginLeft>0cm</MarginLeft>" +
                             "  <MarginRight>0cm</MarginRight>" +
                             "  <MarginBottom>0cm</MarginBottom>" +
                             "</DeviceInfo>";
            deviceInfo = string.Format(deviceInfo, SelectedMedBag.BagWidth, SelectedMedBag.BagHeight);
            var viewer = new ReportViewer {ProcessingMode = ProcessingMode.Local};
            viewer.LocalReport.ReportPath = ReportPath;
            var bytes = viewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension,
                out var streamIds, out var warnings);

            using (var fs = new FileStream("output.pdf", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}