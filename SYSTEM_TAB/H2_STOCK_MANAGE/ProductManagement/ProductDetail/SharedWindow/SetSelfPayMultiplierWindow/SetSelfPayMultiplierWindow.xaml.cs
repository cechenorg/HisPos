using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.SetSelfPayMultiplierWindow
{
    /// <summary>
    /// SetSelfPayMultiplierWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SetSelfPayMultiplierWindow : Window, INotifyPropertyChanged
    {
        #region ----- Define Variables -----

        private double selfPayMultiplier;

        public double SelfPayMultiplier
        {
            get { return selfPayMultiplier; }
            set
            {
                selfPayMultiplier = value;
                NotifyPropertyChanged(nameof(SelfPayMultiplier));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion ----- Define Variables -----

        public SetSelfPayMultiplierWindow(double multiplier)
        {
            InitializeComponent();
            DataContext = this;

            SelfPayMultiplier = multiplier;
        }

        #region ----- Define Functions -----

        private void TextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.Focus();
        }

        private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = ProductDetailDB.SetSelfPayMultiplier(SelfPayMultiplier);

            if (dataTable?.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                DialogResult = true;
                MessageWindow.ShowMessage("更新成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("更新倍率失敗 請稍後再試", MessageType.ERROR);
            }
            Close();
        }

        #endregion ----- Define Functions -----
    }
}