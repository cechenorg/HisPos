using GalaSoft.MvvmLight;
using His_Pos.SYSTEM_TAB.H9_SYSTEMTUTORIAL;
using System;
using System.Data;

namespace His_Pos.NewClass.Tutorial
{
    public class Tutorial : ObservableObject
    {
        public Tutorial(DataRow r)
        {
            Type = (TutorialEnum)Enum.Parse(typeof(TutorialEnum), r.Field<string>("Type"), false);
            Title = r.Field<string>("Name");
            FileName = r.Field<string>("FileName");
        }

        public TutorialEnum Type { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
    }
}