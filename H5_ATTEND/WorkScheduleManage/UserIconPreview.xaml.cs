﻿using His_Pos.Class.WorkSchedule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace His_Pos.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// UserIconPreview.xaml 的互動邏輯
    /// </summary>
    public partial class UserIconPreview : UserControl, INotifyPropertyChanged
    {
        private bool isMed;
        public bool IsMed
        {
            get
            {
                return isMed;
            }
            set
            {
                isMed = value;
                NotifyPropertyChanged("IsMed");
            }
        }
        public UserIconPreview(UserIconData userIconData)
        {
            InitializeComponent();
            DataContext = this;
            UserName.Text = userIconData.Name;
            UserColor.Fill = userIconData.BackBrush;
            IsMed = userIconData.IsMed;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
