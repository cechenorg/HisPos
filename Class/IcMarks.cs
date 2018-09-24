﻿namespace His_Pos.Class
{
    public class IcMarks
    {
        public IcMarks()
        {
            NewbornsData = new NewbornsData();
        }

        public IcMarks(string logOutMark,NewbornsData newbornsData)
        {
            LogOutMark = logOutMark;
            NewbornsData = newbornsData;
        }

        public string LogOutMark { get; set; }//卡片註銷註記
        public NewbornsData NewbornsData { get; set; } = new NewbornsData();//新生兒出生日期.新生兒胞胎註記.新生兒就醫註記
    }
}