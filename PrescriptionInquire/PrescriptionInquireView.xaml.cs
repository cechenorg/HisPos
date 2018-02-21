using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireView.xaml 的互動邏輯
    /// </summary>
    /// 
    
    public partial class PrescriptionInquireView : UserControl
    {
        public PrescriptionInquireView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            row.InputBindings.Add(new MouseBinding(ShowCustomDialogCommand,
                new MouseGesture() { MouseAction = MouseAction.LeftDoubleClick }));
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
    }
}
