using His_Pos.Class;
using His_Pos.Class.Entry;
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

namespace His_Pos.H7_ACCOUNTANCY_REPORT.EntrySerach {
    /// <summary>
    /// EntryDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EntryDetailWindow : Window , INotifyPropertyChanged {
         
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<Entry> entryDetailCollection;
        public ObservableCollection<Entry> EntryDetailCollection
        {
            get => entryDetailCollection;
            set
            {
                entryDetailCollection = value;
                NotifyPropertyChanged("EntryDetailCollection");
            }
        }

        public EntryDetailWindow(string date) {
            InitializeComponent();
            DataContext = this;
            EntryDetailCollection = EntryDb.GetEntryDetailByDate(date);
        }
         
    }
}
