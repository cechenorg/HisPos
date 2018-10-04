using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using His_Pos.Service;
using JetBrains.Annotations;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// IcErrorCodeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class IcErrorCodeWindow : Window,INotifyPropertyChanged
    {
        public class IcErrorCode : INotifyPropertyChanged
        {
            private bool _selected;
            public bool Selected
            {
                get => _selected;
                set
                {
                    _selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
            private string _id;
            public string Id
            {
                get => _id;
                set
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }

            private string _cause;
            public string Cause
            {
                get => _cause;
                set
                {
                    _cause = value;
                    OnPropertyChanged(nameof(Cause));
                }
            }

            public IcErrorCode(string id,string cause)
            {
                Id = id;
                Cause = cause;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private IcErrorCode _selectedItem;
        public IcErrorCode SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        private ObservableCollection<IcErrorCode> _icErrorCodes;

        public ObservableCollection<IcErrorCode> IcErrorCodes
        {
            get => _icErrorCodes;
            set
            {
                _icErrorCodes = value;
                OnPropertyChanged(nameof(IcErrorCodes));
            }
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool AbNormalUpload;
        public IcErrorCodeWindow(bool isMedicalNumberGet,string errMsg)
        {
            InitializeComponent();
            DataContext = this;
            ErrorMessage = "錯誤回報 : " + errMsg;
            SetErrorCodes(isMedicalNumberGet);
        }

        private void SetErrorCodes(bool isMedicalNumberGet)
        {
            IcErrorCodes = new ObservableCollection<IcErrorCode>();
            if (isMedicalNumberGet)
            {
                IcErrorCodes.Add(new IcErrorCode("A001", "讀卡設備故障"));
                IcErrorCodes.Add(new IcErrorCode("A011", "讀卡機故障"));
                IcErrorCodes.Add(new IcErrorCode("A021", "網路故障造成讀卡機無法使用"));
                IcErrorCodes.Add(new IcErrorCode("A031", "安全模組故障造成讀卡機無法使用"));
                IcErrorCodes.Add(new IcErrorCode("B001", "卡片不良 (表面正常, 晶片異常)"));
                IcErrorCodes.Add(new IcErrorCode("D001", "醫療資訊系統(HIS)當機"));
                IcErrorCodes.Add(new IcErrorCode("D011", "醫療院所電腦故障"));
            }
            else
            {
                IcErrorCodes.Add(new IcErrorCode("A001", "讀卡設備故障"));
                IcErrorCodes.Add(new IcErrorCode("A010", "讀卡機故障"));
                IcErrorCodes.Add(new IcErrorCode("A020", "網路故障造成讀卡機無法使用"));
                IcErrorCodes.Add(new IcErrorCode("A030", "安全模組故障造成讀卡機無法使用"));
                IcErrorCodes.Add(new IcErrorCode("B000", "卡片不良 (表面正常, 晶片異常)"));
                IcErrorCodes.Add(new IcErrorCode("C000", "停電"));
                IcErrorCodes.Add(new IcErrorCode("C001", "例外就醫者（首次加保 1 個月內、換補發卡 14 日內"));
                IcErrorCodes.Add(new IcErrorCode("C002", "20 歲以下兒少例外就醫"));
                IcErrorCodes.Add(new IcErrorCode("C003", "懷孕婦女例外就醫"));
                IcErrorCodes.Add(new IcErrorCode("D000", "醫療資訊系統(HIS)當機"));
                IcErrorCodes.Add(new IcErrorCode("D010", "醫療院所電腦故障"));
                IcErrorCodes.Add(new IcErrorCode("E000", "健保署資訊系統當機"));
                IcErrorCodes.Add(new IcErrorCode("E001", "控卡名單已簽切結書"));
                IcErrorCodes.Add(new IcErrorCode("F000", "醫事機構赴偏遠地區因無電話撥接"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SelectedItem = IcErrorCodes[GetCurrentRowIndex(sender)];
            foreach (var icError in IcErrorCodes)
            {
                if (icError.Selected && !icError.Id.Equals(SelectedItem.Id))
                    icError.Selected = false;
            }
        }
        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case Label lable:
                {
                    var temp = new List<TextBox>();
                    NewFunction.FindChildGroup(lv, lable.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(lable))
                            return x;
                    }
                    break;
                }
                case CheckBox checkBox:
                {
                    var temp = new List<CheckBox>();
                    NewFunction.FindChildGroup(lv, checkBox.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(checkBox))
                            return x;
                    }
                    break;
                }
            }
            return -1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedItem.Id))
            {
                MessageBox.Show("請選擇異常代碼");
                return;
            }
            AbNormalUpload = true;
            Close();
        }
    }
}
