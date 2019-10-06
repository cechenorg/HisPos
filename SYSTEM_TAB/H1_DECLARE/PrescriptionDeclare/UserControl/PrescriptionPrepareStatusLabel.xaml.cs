using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl
{
    /// <summary>
    /// PrescriptionPrepareStatusLabel.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionPrepareStatusLabel : System.Windows.Controls.UserControl,INotifyPropertyChanged
    {
        #region OrderStatus
        public static readonly DependencyProperty OrderStatusProperty =
            DependencyProperty.Register(
                "OrderStatus",
                typeof(String),
                typeof(PrescriptionPrepareStatusLabel),
                new PropertyMetadata(null));
        public String OrderStatus
        {
            get { return (String)GetValue(OrderStatusProperty); }
            set
            {
                switch (value)
                {
                    case "備藥狀態 : 未處理":
                    case "訂單狀態 : 無訂單":
                        MyBrush = new SolidColorBrush(Colors.Red);
                        LabelWidth = 173;
                        break;
                    case "備藥狀態 : 已備藥":
                    case "訂單狀態 : 已收貨":
                        MyBrush = new SolidColorBrush(Colors.Green);
                        LabelWidth = 173;
                        break;  
                    case "備藥狀態 : 不備藥":
                    case "訂單狀態 : 訂單作廢":
                        MyBrush = new SolidColorBrush(Colors.DimGray);
                        LabelWidth = 173;
                        break; 
                    case "訂單狀態 : 等待確認":
                        MyBrush = new SolidColorBrush(Colors.OrangeRed);
                        LabelWidth = 173;
                        break; 
                    case "訂單狀態 : 等待收貨":
                        MyBrush = new SolidColorBrush(Colors.RoyalBlue);
                        LabelWidth = 173;
                        break; 
                }
                SetValue(OrderStatusProperty, value);
                OnPropertyChanged(nameof(OrderStatus));
            }
        }
        #endregion

        private int labelWidth;
        public int LabelWidth
        {
            get => labelWidth;
            set
            {
                labelWidth = value;
                OnPropertyChanged(nameof(LabelWidth));
            }
        }

        private Brush myBrush;

        public Brush MyBrush
        {
            get => myBrush;
            set
            {
                myBrush = value;
                OnPropertyChanged(nameof(MyBrush));
            }
        }

        public PrescriptionPrepareStatusLabel()
        {
            InitializeComponent();
            Root.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
