﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class NormalPrescriptionService:IPrescriptionService
    {
        public void GetMasterData()
        {
            throw new NotImplementedException();
        }

        public void GetDetailData()
        {
            throw new NotImplementedException();
        }

        public bool Adjust()
        {
            throw new NotImplementedException();
        }

        public bool ErrorAdjust()
        {
            throw new NotImplementedException();
        }

        public bool NoCardAdjust()
        {
            throw new NotImplementedException();
        }

        public List<bool> AskPrint()
        {
            throw new NotImplementedException();
        }

        public bool Register()
        {
            MessageWindow.ShowMessage("一般箋不可登錄",MessageType.WARNING);
            return false;
        }
    }
}
