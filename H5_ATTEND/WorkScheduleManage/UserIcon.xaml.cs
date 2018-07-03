﻿using System;
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
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// UserIcon.xaml 的互動邏輯
    /// </summary>
    public partial class UserIcon : UserControl
    {
        public string Id { get; }

        public UserIcon(UserIconData userIconData)
        {
            InitializeComponent();
            Id = userIconData.Id;
            UserName.Text = userIconData.Name.Substring(0,1);
            Back.Background = userIconData.BackBrush;
        }
    }
}
