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
using His_Pos.Class.DiseaseCode;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// DiseaseCodeSelectDialog.xaml 的互動邏輯
    /// </summary>
    public partial class DiseaseCodeSelectDialog : Window
    {
        public DiseaseCodeSelectDialog(string id)
        {
            InitializeComponent();
            DiseaseCodeDb.GetDiseaseCodeById(id);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is null) return;

            ItemGrid.RowDefinitions[1].Height = new GridLength((sender as DiseaseCodeSelectDialog).Height - 140);
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
