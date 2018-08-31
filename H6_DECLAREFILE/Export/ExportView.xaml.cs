using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class.Declare;

namespace His_Pos.H6_DECLAREFILE.Export
{
    /// <summary>
    /// ExportView.xaml 的互動邏輯
    /// </summary>
    public partial class ExportView : UserControl
    {
        public ObservableCollection<DeclareFile> DeclareFiles { get; set; }
        public ExportView()
        {
            InitializeComponent();
            InitializeDeclareFiles();
        }

        private void InitializeDeclareFiles()
        {
            var load = new LoadingWindow();
            //load.GetMedBagData(this);
            load.Show();
        }
    }
}
