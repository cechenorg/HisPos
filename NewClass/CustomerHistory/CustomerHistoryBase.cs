using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using His_Pos.Class;
using JetBrains.Annotations;

namespace His_Pos.NewClass.CustomerHistory
{
    public class CustomerHistoryBase:INotifyPropertyChanged
    {
        public SystemType Type { get; }
        private DateTime date;//日期
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        private string title;//標題
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private string subtitle;//副標題
        public string SubTitle
        {
            get => subtitle;
            set
            {
                subtitle = value;
                OnPropertyChanged(nameof(SubTitle));
            }
        }
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
