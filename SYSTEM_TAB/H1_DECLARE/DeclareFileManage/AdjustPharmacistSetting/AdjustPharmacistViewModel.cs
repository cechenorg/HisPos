using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AdjustPharmacistViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private MonthViewCalendar monthViewCalendar;
        public MonthViewCalendar MonthViewCalendar
        {
            get => monthViewCalendar;
            set
            {
                Set(() => MonthViewCalendar, ref monthViewCalendar, value);
            }
        }

        private PharmacistSchedule pharmacistSchedule;
        public PharmacistSchedule PharmacistSchedule
        {
            get => pharmacistSchedule;
            set
            {
                Set(() => PharmacistSchedule, ref pharmacistSchedule, value);
            }
        }

        private PharmacistScheduleItem selectedPharmacistScheduleItem;
        public PharmacistScheduleItem SelectedPharmacistScheduleItem
        {
            get => selectedPharmacistScheduleItem;
            set
            {
                Set(() => SelectedPharmacistScheduleItem, ref selectedPharmacistScheduleItem, value);
            }
        }
        public string DeclareMonth => (CurrentDate.Year - 1911) + "年" + CurrentDate.Month + "月";
        public string SelectedDateStr => MySelectedDate.Month + "/" + MySelectedDate.Day;
        public static DateTime CurrentDate { get; set; }
        private static DateTime first { get; set; }
        private static DateTime last { get; set; }
        private DateTime _selectedDate;
        public DateTime MySelectedDate
        {
            get => _selectedDate;
            set
            {
                var temp = _selectedDate;
                if (value >= first && value <= last)
                {
                    _selectedDate = value;
                    if (PharmacistSchedule.Count(p => p.Date.Equals(MySelectedDate)) == 1)
                        SelectedPharmacistScheduleItem = PharmacistSchedule.Single(p => p.Date.Equals(MySelectedDate));
                }
                else
                {
                    _selectedDate = temp;
                }
            }
        }
        private DateTime _displayDate;
        public DateTime MyDisplayDate
        {
            get => _displayDate;
            set
            {
                if (value >= first && value <= last)
                {
                    _displayDate = value;
                }
                else
                {
                    _displayDate = CurrentDate;
                }
            }
        }
        private bool IsEdit { get; set; }
        #region Commands
        public RelayCommand DeletePharmacistScheduleItem { get; set; }
        public RelayCommand AddPharmacistScheduleItem { get; set; }
        public RelayCommand SavePharmacistScheduleItem { get; set; }
        public RelayCommand Close { get; set; }

        #endregion

        public static MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustPharmacistViewModel(DateTime declare)
        {
            InitialVariables(declare);
            InitialCommands();
        }

        private void InitialVariables(DateTime declare)
        {
            PharmacistSchedule = new PharmacistSchedule();
            CurrentDate = declare;
            MyDisplayDate = declare;
            first = new DateTime(declare.AddMonths(1).Year, declare.Month, 1);
            last = new DateTime(declare.AddMonths(1).Year, declare.AddMonths(1).Month, 1).AddDays(-1);
            MedicalPersonnels = new MedicalPersonnels(false);
            MainWindow.ServerConnection.OpenConnection();
            MedicalPersonnels.GetEnablePharmacist(first, last);
            MainWindow.ServerConnection.CloseConnection();
            MonthViewCalendar = new MonthViewCalendar(declare);
        }

        private void InitialCommands()
        {
            DeletePharmacistScheduleItem = new RelayCommand(DeletePharmacistScheduleItemAction);
            AddPharmacistScheduleItem = new RelayCommand(AddPharmacistScheduleItemAction);
            SavePharmacistScheduleItem = new RelayCommand(SavePharmacistScheduleItemAction);
            Close = new RelayCommand(CloseAction);
        }

        private void DeletePharmacistScheduleItemAction()
        {
            if (SelectedPharmacistScheduleItem is null)
            {
                MessageWindow.ShowMessage("請選擇欲刪除之藥師", MessageType.WARNING);
                return;
            }

            var deleteMsg = "確定刪除 : " + SelectedDateStr + "(" +
                            SelectedPharmacistScheduleItem.MedicalPersonnel.Name + ")?";
            var delete = new ConfirmWindow(deleteMsg, "刪除確認");
            if ((bool) delete.DialogResult)
            {
                PharmacistSchedule.Remove(SelectedPharmacistScheduleItem);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PharmacistSchedule"));
                IsEdit = true;
            }
        }

        private void AddPharmacistScheduleItemAction()
        {
            var addPharmacistScheduleItemWindow = new AddPharmacistScheduleItemWindow
            (
                pharmacistScheduleItem =>
                {
                    if (PharmacistSchedule.Count(p => p.Date.Equals(MySelectedDate) && p.MedicalPersonnel.ID.Equals(pharmacistScheduleItem.MedicalPersonnel.ID)) > 0)
                    {
                        MessageWindow.ShowMessage("日期 : "+ SelectedDateStr + " 藥師已存在",MessageType.ERROR);
                    }
                    else
                    {
                        PharmacistSchedule.Add(pharmacistScheduleItem);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PharmacistSchedule"));
                        IsEdit = true;
                    }
                }, MySelectedDate
            );
            addPharmacistScheduleItemWindow.Show();
        }

        private void SavePharmacistScheduleItemAction()
        {
            
        }

        private void CloseAction()
        {
            if (IsEdit)
            {
                var close = new ConfirmWindow("有變更尚未完成，仍要關閉?", "");
                if ((bool) close.DialogResult)
                    Messenger.Default.Send(new NotificationMessage("CloseAdjustPharmacistWindow"));
            }
            else
                Messenger.Default.Send(new NotificationMessage("CloseAdjustPharmacistWindow"));
        }
    }
}
