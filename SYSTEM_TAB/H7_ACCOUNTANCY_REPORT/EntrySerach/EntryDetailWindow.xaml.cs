using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using His_Pos.Class.Entry;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach {
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
                ///EntryDetailCollection = EntryDb.GetEntryDetailByDate(date);
            
        }

        private void CheckBoxPurchase_Checked(object sender, RoutedEventArgs e) {
            if (EntryDetailCollection is null) return;
            EntryDetail.Items.Filter = ((o) => {
                string name = ((Entry)o).EntryName;
                switch (name)
                {
                    case "進貨":
                        if ((bool)CheckBoxPurchase.IsChecked)
                            return true;
                        else
                            return false;
                    case "退貨":
                        if ((bool)CheckBoxReturn.IsChecked)
                            return true;
                        else
                            return false;
                    case "盤點單盤點":
                        if ((bool)CheckBoxCheck.IsChecked)
                            return true;
                        else
                            return false;
                    case "單品盤點":
                        if ((bool)CheckBoxCheck.IsChecked)
                            return true;
                        else
                            return false;
                    case "調劑耗用":
                        if ((bool)CheckBoxMedUse.IsChecked)
                            return true;
                        else
                            return false;

                    default:
                        return false;
                }
            });

        }

         
    }
}
