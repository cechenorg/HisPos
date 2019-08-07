using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace His_Pos.GeneralCustomControl
{
    /// <summary>
    /// ProductStatusIcon.xaml 的互動邏輯
    /// </summary>
    public partial class ProductStatusIcon : UserControl, INotifyPropertyChanged
    {
        #region ----- Define DependencyProperty -----
        public bool IsCommon
        {
            get { return (bool)GetValue(IsCommonProperty); }
            set { SetValue(IsCommonProperty, value); }
        }

        public int CommonSafeAmount
        {
            get { return (int)GetValue(CommonSafeAmountProperty); }
            set { SetValue(CommonSafeAmountProperty, value); }
        }

        public int ControlLevel
        {
            get { return (int)GetValue(ControlLevelProperty); }
            set { SetValue(ControlLevelProperty, value); }
        }

        public bool IsFrozen
        {
            get { return (bool)GetValue(IsFrozenProperty); }
            set { SetValue(IsFrozenProperty, value); }
        }

        public bool IsInventoryError
        {
            get { return (bool)GetValue(IsFrozenProperty); }
            set { SetValue(IsFrozenProperty, value); }
        }

        public static readonly DependencyProperty IsCommonProperty =
            DependencyProperty.Register("IsCommon", typeof(bool), typeof(ProductStatusIcon), new PropertyMetadata(false));

        public static readonly DependencyProperty CommonSafeAmountProperty =
            DependencyProperty.Register("CommonSafeAmount", typeof(int), typeof(ProductStatusIcon), new PropertyMetadata(0, OnVariableChanged));

        public static readonly DependencyProperty ControlLevelProperty =
            DependencyProperty.Register("ControlLevel", typeof(int), typeof(ProductStatusIcon), new PropertyMetadata(0, OnVariableChanged));

        public static readonly DependencyProperty IsFrozenProperty =
            DependencyProperty.Register("IsFrozen", typeof(bool), typeof(ProductStatusIcon), new PropertyMetadata(false));

        public static readonly DependencyProperty IsInventoryErrorProperty =
            DependencyProperty.Register("IsInventoryError", typeof(bool), typeof(ProductStatusIcon), new PropertyMetadata(false));

        private static void OnVariableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProductStatusIcon userControl = sender as ProductStatusIcon;

            userControl.OnPropertyChanged(nameof(IsControl));
            userControl.OnPropertyChanged(nameof(ShowSafeAmount));
        }
        #endregion

        #region ----- Define Variables -----
        public bool IsControl { get { return ControlLevel > 0; } }
        public bool ShowSafeAmount { get { return CommonSafeAmount > 0; } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public ProductStatusIcon()
        {
            InitializeComponent();
            StatusStack.DataContext = this;
        }
    }
}
