﻿using His_Pos.Class;
using His_Pos.Class.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace His_Pos.H4_BASIC_MANAGE.ProductTypeManage
{
    /// <summary>
    /// ChangeTypeResultWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChangeTypeResultWindow : Window,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private ObservableCollection<ItemChangeTypeWindow.ChangeItem> changeItems = new ObservableCollection<ItemChangeTypeWindow.ChangeItem>();

        public ObservableCollection<ItemChangeTypeWindow.ChangeItem> ChangeItems
        {
            get
            {
                return changeItems;
            }
            set
            {
                changeItems = value;
                NotifyPropertyChanged("ChangeItems");
            }
        }
        public ChangeTypeResultWindow(ObservableCollection<ItemChangeTypeWindow.ChangeItem> tempchangeItems)
        {
            InitializeComponent();
            DataContext = this;
            ChangeItems = tempchangeItems;
        }

   
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in changeItems)
            {
            ProductDb.ChangeProductType(item.id, item.newvalue);
            }
            MessageWindow messageWindow = new MessageWindow("更新成功!", MessageType.SUCCESS, true);
            messageWindow.ShowDialog();
            Close();
        }
    }
}
