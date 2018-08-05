using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
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
using System.Xml.Serialization;
using His_Pos.Class.MedBag;
using His_Pos.Class.MedBagLocation;
using JetBrains.Annotations;
using CheckBox = System.Windows.Controls.CheckBox;
using UserControl = System.Windows.Controls.UserControl;
using His_Pos.RDLC;
using Microsoft.Reporting.WinForms;
using Rectangle = His_Pos.RDLC.Rectangle;
using Report = His_Pos.RDLC.Report;

namespace His_Pos.H1_DECLARE.MedBagManage
{
    /// <summary>
    /// MedBagManageView.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagManageView : UserControl, INotifyPropertyChanged
    {
        public static MedBagManageView Instance;
        private MedBag _selectedMedBag;
        private ObservableCollection<MedBagLocation> _medBagLocations;

        public MedBag SelectedMedBag
        {
            get => _selectedMedBag;
            set
            {
                _selectedMedBag = value;
                OnPropertyChanged(nameof(SelectedMedBag));
            }
        }

        private double _medBagImgWidth;

        public double MedBagImgWidth
        {
            get => _medBagImgWidth;
            set
            {
                _medBagImgWidth = value;
                OnPropertyChanged(nameof(MedBagImgWidth));
            }
        }

        private double _medBagImgHeight;

        public double MedBagImgHeight
        {
            get => _medBagImgHeight;
            set
            {
                _medBagImgHeight = value;
                OnPropertyChanged(nameof(MedBagImgHeight));
            }
        }

        private int _id;

        public MedBagManageView()
        {
            InitializeComponent();
            Instance = this;
            DataContext = this;
            _medBagLocations = new ObservableCollection<MedBagLocation>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ImageSelector_Click(object sender, RoutedEventArgs e)
        {
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
                ImgWrap.Width = bitmap.Width;
                MedBagImgWidth = (bitmap.Width / bitmap.Height) * 850;
                SelectedMedBag = new MedBag(bitmap);
                SetMedBagRange();
            }
        }

        private void SetMedBagRange()
        {
            MedRangeLocationControl.Template = (ControlTemplate)FindResource("MedBagRangeItemTemplate");
            MedRangeLocationControl.SetValue(Canvas.LeftProperty, 0.0);
            MedRangeLocationControl.SetValue(Canvas.TopProperty, 0.0);
            MedRangeLocationControl.SetValue(WidthProperty, MedBagImgWidth);
            MedRangeLocationControl.SetValue(HeightProperty, MedBagImg.Height);
            MedBagCanvas.Width = MedBagImgWidth;
            MedBagCanvas.Height = MedBagImg.Height;
        }

        private void NewLocationChecked(object sender, RoutedEventArgs e)
        {
            var locationName = (sender as CheckBox)?.Content.ToString();
            var controlName = (sender as CheckBox)?.Name;
            Instance.NewLocation(null, locationName, controlName);
        }

        public void NewLocation(string locid = null, string content = null, string controlName = null, double height = 0, double width = 0, double top = 0, double left = 0)
        {
            var contentControl = new ContentControl();
            if (string.IsNullOrEmpty(content))
                contentControl.Template = (ControlTemplate)FindResource("MedBagRangeItemTemplate");
            else
                contentControl.Template = (ControlTemplate)FindResource("MedBagDesignerItemTemplate");
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
                contentControl.Height = (height == 0) ? 150 : height;
                contentControl.Width = (width == 0) ? 150 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, top == 0 ? 360 : top);
                Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            }
            else
            {
                contentControl.Height = (height == 0) ? 15 : height;
                contentControl.Width = (width == 0) ? 60 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, top == 0 ? 360 : top);
                Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            }
        }

        public void SaveLocation()
        {
            foreach (var contentControl in MedBagCanvas.Children.OfType<ContentControl>().Where(r => r.Content is RdlLocationControl))
            {
                var rdlLocation = (RdlLocationControl) contentControl.Content;
                var medBagImage = MedBagCanvas.Children.OfType<RdlLocationControl>().Where(r => r.Content is Grid).Single(r =>
                        string.IsNullOrEmpty(r.LabelContent));
                var width = medBagImage.Width;
                var convert = SelectedMedBag.BagWidth / width;
                if (!string.IsNullOrEmpty(rdlLocation.LabelContent))
                {
                    var pathX = convert * (double)rdlLocation.Parent.GetValue(Canvas.LeftProperty);
                    var pathY = convert * (double)rdlLocation.Parent.GetValue(Canvas.TopProperty);
                    var locationWidth = (double)rdlLocation.Parent.GetValue(WidthProperty) * convert;
                    var locationHeight = (double)rdlLocation.Parent.GetValue(HeightProperty) * convert;
                    _medBagLocations.Add(new MedBagLocation(rdlLocation.id, rdlLocation.LabelName,pathX,pathY,locationWidth,locationHeight,rdlLocation.LabelContent));
                }
            }
        }

        private void DeleteLocation(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            MedBagCanvas.Children.Remove(MedBagCanvas.Children.OfType<ContentControl>().Where(r => r.Content is RdlLocationControl).Single(r => (r.Content as RdlLocationControl).LabelContent.Equals(checkBox.Content)));
        }

        private void MedBagSaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveLocation();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(Report));
            serializer.Serialize(Console.Out, CreatReportXml(), ns);
            File.WriteAllText(@"C:\HisPos\RDLC\MedBagReport.rdlc", string.Empty);
            Stream fs = new FileStream(@"C:\HisPos\RDLC\MedBagReport.rdlc", FileMode.OpenOrCreate,FileAccess.Write);
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            serializer.Serialize(writer, CreatReportXml());
            writer.Close();
            _reportViewer.LocalReport.ReportPath = @"C:\HisPos\RDLC\MedBagReport.rdlc";
            _reportViewer.RefreshReport();
            _reportViewer.PrinterSettings.PrintToFile = true;
        }

        private Report CreatReportXml()
        {
            Report medBagReport = new Report
            {
                Xmlns = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition",
                Rd = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner",
                Body = new Body
                {
                    ReportItems = new ReportItems(),
                    Height = SelectedMedBag.BagHeight + "cm",
                    Style = new RDLC.Style
                    {
                        BackgroundImage = new BackgroundImage
                        {
                            Source = "External",
                            Value = "=Parameters!ImagePath.Value"
                        }
                    }
                },
                Page = new RDLC.Page
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
            foreach (var m in _medBagLocations)
            {
                if (m.Name != "MedicineList")
                {
                    medBagReport.Body.ReportItems.Textbox.Add(CreatTextBoxField(m));
                }
            }
            return medBagReport;
        }

        private Textbox CreatTextBoxField(MedBagLocation m)
        {
            return new Textbox
            {
                Name = m.Name,
                DefaultName = m.Name,
                CanGrow = "true",
                KeepTogether = "true",
                Top = m.PathY.ToString(CultureInfo.InvariantCulture) + "cm",
                Left = m.PathX.ToString(CultureInfo.InvariantCulture) + "cm",
                Height = m.Height.ToString(CultureInfo.InvariantCulture) + "cm",
                Width = m.Width.ToString(CultureInfo.InvariantCulture) + "cm",
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
                Style = new RDLC.Style
                {
                    Border = new RDLC.Border {Style = "None"},
                    PaddingLeft = "2pt",
                    PaddingRight = "2pt",
                    PaddingTop = "2pt",
                    PaddingBottom = "2pt"
                }
            };
        }

    }
}