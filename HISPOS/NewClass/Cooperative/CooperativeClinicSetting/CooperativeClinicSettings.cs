using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicSetting
{
    public class CooperativeClinicSettings : ObservableCollection<CooperativeClinicSetting>
    {
        public CooperativeClinicSettings()
        {
        }

        public void Init()
        {
            Clear();
            var table = CooperativeClinicSettingDb.Init();
            foreach (DataRow r in table.Rows)
            {
                Add(new CooperativeClinicSetting(r));
            }
        }

        public void Update()
        {
            CooperativeClinicSettingDb.Update(this);
        }

        public WareHouse.WareHouse GetWareHouseByPrescription(Institution ins, string adjcaseID)
        {
            if (ins is null) return ChromeTabViewModel.ViewModelMainWindow.GetWareHouse("0");
            var temp = Items.SingleOrDefault(w => w.CooperavieClinic.ID == ins.ID);
            if (temp is null)
                return ChromeTabViewModel.ViewModelMainWindow.GetWareHouse("0");

            switch (adjcaseID)
            {
                case "2":
                    return temp.ChronicIsBuckle ? temp.ChronicWareHouse : null;

                default:
                    return temp.NormalIsBuckle ? temp.NormalWareHouse : null;
            }
        }

        /// <summary>
        /// 壓縮合作XML，今日的不壓縮
        /// </summary>
        public void FilePurge()
        {
            foreach (var c in this)
            {
                if (string.IsNullOrEmpty(c.FilePath)) continue;
                try
                {
                    if (!Directory.Exists($"{c.FilePath}\\PurgeFile"))
                        Directory.CreateDirectory($"{c.FilePath}\\PurgeFile");

                    if (!Directory.Exists($"{c.FilePath}\\TxtFile"))
                        Directory.CreateDirectory($"{c.FilePath}\\TxtFile");

                    List<string> fileList = new List<string>();
                    DirectoryInfo di = new DirectoryInfo(c.FilePath);
                    TaiwanCalendar tc = new TaiwanCalendar();
                    DateTime now = DateTime.Now;
                    string date = string.Format("{0}{1}{2}", tc.GetYear(now), tc.GetMonth(now).ToString().PadLeft(2,'0'), tc.GetDayOfMonth(now).ToString().PadLeft(2, '0'));
                    foreach (FileInfo f in di.GetFiles("*.xml", SearchOption.AllDirectories))
                    {
                        string[] file = f.FullName.Split('_');
                        if(file != null && file.Length > 2)
                        {
                            if(date != file[1])//如果不是今日的處方
                            {
                                fileList.Add(f.FullName);
                            }
                        }
                    }
                    Function.ZipFiles(fileList, $"{c.FilePath}\\PurgeFile\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip");
                    foreach (string fs in fileList)
                    {
                        File.Delete(fs);
                    }

                    List<string> fileListTxt = new List<string>();
                    DirectoryInfo ditxt = new DirectoryInfo(c.FilePath);
                    foreach (FileInfo f in ditxt.GetFiles("*.txt", SearchOption.AllDirectories))
                    {
                        string[] file = f.FullName.Split('_');
                        if (file != null && file.Length > 2)
                        {
                            if (date != file[1])//如果不是今日的處方
                            {
                                fileListTxt.Add(f.FullName);
                            }
                        }
                    }
                    Function.ZipFiles(fileListTxt, $"{c.FilePath}\\TxtFile\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip");
                    foreach (string fs in fileListTxt)
                    {
                        File.Delete(fs);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}