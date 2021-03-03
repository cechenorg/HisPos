using System.Windows.Media.Imaging;

namespace His_Pos.GeneralCustomControl
{
    /// <summary>
    /// IconLabel.xaml 的互動邏輯
    /// </summary>
    public partial class IconLabel
    {
        public IconLabel()
        {
            InitializeComponent();
        }

        public void SetIconLabel(double labelWidth, double labelHeight, string labelContent)
        {
            Width = labelWidth;
            Height = labelHeight;
            Label.Text = labelContent;
            UpdateLayout();
        }

        public void SetIconSource(BitmapImage iconsource)
        {
            Icon.Source = iconsource;
        }

        public void SetLabelContent(string content)
        {
            Label.Text = content;
        }

        public void SetLabelSize(double labelWidth, double labelHeight)
        {
            Width = labelWidth;
            Height = labelHeight;
            UpdateLayout();
        }
    }
}