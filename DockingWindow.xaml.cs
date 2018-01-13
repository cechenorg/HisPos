using System.Windows;
using System.Windows.Media.Animation;

namespace His_Pos
{
    /// <summary>
    /// DockingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DockingWindow
    {
        public DockingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["FadeInContentAnim"] as Storyboard;
            sb.Begin();
        }
    }
}
