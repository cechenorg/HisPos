﻿using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionWindow
{
    /// <summary>
    /// CooperativeSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeSelectionWindow : Window
    {
        public CooperativeSelectionWindow()
        {
            InitializeComponent();
            this.DataContext = new CooperativeSelectionViewModel();
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
