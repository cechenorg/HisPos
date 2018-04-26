using His_Pos.Class;
using System;
using System.Collections.Generic;
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

namespace His_Pos.StockTaking
{
    /// <summary>
    /// StockTakingView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingView : UserControl
    {
        private StockTakingStatus stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
        public StockTakingView()
        {
            InitializeComponent();
        }

        private void UpdateUi()
        {
            switch(stockTakingStatus)
            {
                case StockTakingStatus.ADDPRODUCTS:
                    ViewGrid.RowDefinitions[2].Height = new GridLength(15);
                    ViewGrid.RowDefinitions[3].Height = new GridLength(150);
                    Print.IsEnabled = false;
                    AddProductsEllipse.Fill = Brushes.GreenYellow;
                    PrintLine.Stroke = Brushes.LightSlateGray;
                    PrintLine.StrokeDashArray = new DoubleCollection() { 4 };
                    PrintEllipse.Fill = Brushes.LightSlateGray;
                    InputLine.Stroke = Brushes.LightSlateGray;
                    InputLine.StrokeDashArray = new DoubleCollection() { 4 };
                    InputEllipse.Fill = Brushes.LightSlateGray;
                    CompleteLine.Stroke = Brushes.LightSlateGray;
                    CompleteLine.StrokeDashArray = new DoubleCollection() { 4 };
                    CompleteEllipse.Fill = Brushes.LightSlateGray;
                    break;
                case StockTakingStatus.PRINT:
                    ViewGrid.RowDefinitions[2].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[3].Height = new GridLength(0);
                    Print.IsEnabled = true;
                    AddProductsEllipse.Fill = Brushes.DeepSkyBlue;
                    PrintLine.Stroke = Brushes.DeepSkyBlue;
                    PrintLine.StrokeDashArray = new DoubleCollection() { 300 };
                    PrintEllipse.Fill = Brushes.GreenYellow;
                    break;
                case StockTakingStatus.INPUTRESULT:
                    Print.IsEnabled = false;
                    PrintEllipse.Fill = Brushes.DeepSkyBlue;
                    InputLine.Stroke = Brushes.DeepSkyBlue;
                    InputLine.StrokeDashArray = new DoubleCollection() { 300 };
                    InputEllipse.Fill = Brushes.GreenYellow;
                    break;
                case StockTakingStatus.COMPLETE:
                    InputEllipse.Fill = Brushes.DeepSkyBlue;
                    CompleteLine.Stroke = Brushes.DeepSkyBlue;
                    CompleteLine.StrokeDashArray = new DoubleCollection() { 300 };
                    CompleteEllipse.Fill = Brushes.GreenYellow;
                    break;
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (stockTakingStatus == StockTakingStatus.COMPLETE)
                stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
            else
                stockTakingStatus++;

            UpdateUi();
        }
    }
}
