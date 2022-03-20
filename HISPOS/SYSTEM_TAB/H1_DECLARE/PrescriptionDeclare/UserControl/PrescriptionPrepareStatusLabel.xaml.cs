using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl
{
    /// <summary>
    /// PrescriptionPrepareStatusLabel.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionPrepareStatusLabel : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        #region OrderStatus

        public static readonly DependencyProperty OrderStatusProperty =
            DependencyProperty.Register(
                "OrderStatus",
                typeof(string),
                typeof(PrescriptionPrepareStatusLabel),
                new PropertyMetadata(null));

        public string OrderStatus
        {
            get { return (string)GetValue(OrderStatusProperty); }
            set
            {
                SetValue(OrderStatusProperty, value);
            }
        }

        #endregion OrderStatus

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}