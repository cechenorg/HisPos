﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.CooperativeInstitution;

namespace His_Pos.NewClass.PrescriptionRefactoring.Cooperative
{
    public class OrthopedicsPreview : CooperativePreBase
    {
        public OrthopedicsPreview(OrthopedicsPrescription c) :base(c)
        {
            Content = c;
        }
        public OrthopedicsPrescription Content { get;}
        public override void Print()
        {
            var printPre = CreatePrescription();
            printPre.PrintMedBagAndReceipt();
        }
        public override Prescription CreatePrescription()
        {
            var pre = new Prescription(Content);
            pre.UpdateCooperativePrescriptionIsRead();
            pre.CountPrescriptionPoint(true);
            return pre;
        }
    }
}
