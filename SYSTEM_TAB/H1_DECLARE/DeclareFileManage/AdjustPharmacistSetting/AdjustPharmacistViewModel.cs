using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.Properties;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AdjustPharmacistViewModel : ViewModelBase
    {
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }
        private string busyContent;
        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
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
        public string SelectedDateStr => ((DateTime)MySelectedDate).Month + "/" + ((DateTime)MySelectedDate).Day;
        public static DateTime CurrentDate { get; set; }
        private static DateTime first { get; set; }
        private static DateTime last { get; set; }
        private DateTime? _selectedDate;
        public DateTime? MySelectedDate
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
        public RelayCommand AddAllPharmacists { get; set; }
        public RelayCommand DeletePharmacistScheduleItem { get; set; }
        public RelayCommand AddPharmacistScheduleItem { get; set; }
        public RelayCommand SavePharmacistScheduleItem { get; set; }
        public RelayCommand Close { get; set; }

        #endregion

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
            MonthViewCalendar = new MonthViewCalendar(MyDisplayDate);
            InitItemsSource();
        }

        private void InitItemsSource()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "取得調整設定...";
                MainWindow.ServerConnection.OpenConnection();
                PharmacistSchedule.GetPharmacistSchedule(first, last);
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void InitialCommands()
        {
            AddAllPharmacists = new RelayCommand(AddAllPharmacistsAction);
            DeletePharmacistScheduleItem = new RelayCommand(DeletePharmacistScheduleItemAction);
            AddPharmacistScheduleItem = new RelayCommand(AddPharmacistScheduleItemAction);
            SavePharmacistScheduleItem = new RelayCommand(SavePharmacistScheduleItemAction);
            Close = new RelayCommand(CloseAction);
        }

        private void AddAllPharmacistsAction()
        {
            if (MySelectedDate == null)
            {
                MessageWindow.ShowMessage("請選擇欲新增日期",MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var tempPharmacistList = new Employees();
            tempPharmacistList.GetEnablePharmacist((DateTime)MySelectedDate);
            MainWindow.ServerConnection.CloseConnection();
            foreach (var pharmacist in tempPharmacistList)
            {
                var item = new PharmacistScheduleItem
                {
                    Date = (DateTime)MySelectedDate,
                    MedicalPersonnel = new DeclareMedicalPersonnel(pharmacist),
                    RegisterTime = DateTime.Now
                };
                PharmacistSchedule.Add(item);
            }
            RaisePropertyChanged(nameof(PharmacistSchedule));
            IsEdit = true;
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
                RaisePropertyChanged(nameof(PharmacistSchedule));
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
                        return;
                    }

                    PharmacistSchedule.Add(pharmacistScheduleItem);
                    RaisePropertyChanged(nameof(PharmacistSchedule));
                    IsEdit = true;
                }, (DateTime)MySelectedDate
            );
            addPharmacistScheduleItemWindow.Show();
        }

        private void SavePharmacistScheduleItemAction()
        {
            PharmacistSchedule.SaveSchedule(first,last);
            InitItemsSource();
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
