using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.INDEX
{
    /// <summary>
    /// IndexView.xaml 的互動邏輯
    /// </summary>
    public partial class IndexView : UserControl { 
        public static IndexView Instance;
        public IndexView()
        {
            InitializeComponent(); 
        }
    }
}
