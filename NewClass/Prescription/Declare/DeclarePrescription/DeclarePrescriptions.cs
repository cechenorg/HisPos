﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Xml;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescriptions:ObservableCollection<DeclarePrescription>
    {
        public DeclarePrescriptions()
        {

        }

        public void GetSearchPrescriptions(DateTime decStart, DateTime decEnd, string pharmacyID)
        {
            var table = DeclarePrescriptionDb.GetDeclarePrescriptionsByMonthRange(decStart, decEnd, pharmacyID);
            foreach (DataRow r in table.Rows)
            {
                Add(new DeclarePrescription(r));
            }
        }
        public void AddPrescriptions(List<DeclarePrescription> pres)
        {
            foreach (var p in pres.OrderBy(p => p.IsDeclare))
            {
                Add(p);
            }
        }

        public void SerializeFileContent()
        {
            foreach (var p in Items.Where(pre => pre.IsDeclare))
            {
                //var pre = new Prescription(PrescriptionDb.GetPrescriptionByID(p.ID).Rows[0],
                //    PrescriptionSource.Normal);
                //MainWindow.ServerConnection.OpenConnection();
                //pre.Patient = pre.Patient.GetCustomerByCusId(pre.Patient.ID);
                //pre.AdjustMedicinesType();
                //MainWindow.ServerConnection.CloseConnection();
                //if (pre.Treatment.Division != null)
                //    pre.Treatment.Division = ViewModelMainWindow.GetDivision(pre.Treatment.Division?.ID);
                //pre.Treatment.Pharmacist =
                //    ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(pr => pr.IDNumber.Equals(pre.Treatment.Pharmacist.IDNumber));
                //pre.Treatment.AdjustCase = ViewModelMainWindow.GetAdjustCase(pre.Treatment.AdjustCase.ID);
                //pre.Treatment.Copayment = ViewModelMainWindow.GetCopayment(pre.Treatment.Copayment?.Id);
                //if (pre.Treatment.PrescriptionCase != null)
                //    pre.Treatment.PrescriptionCase = ViewModelMainWindow.GetPrescriptionCases(pre.Treatment.PrescriptionCase?.ID);
                //if (pre.Treatment.SpecialTreat != null)
                //    pre.Treatment.SpecialTreat = ViewModelMainWindow.GetSpecialTreat(pre.Treatment.SpecialTreat?.ID);
                //pre.RemakeDeclareFile();
                //pre.UpdateDeclareContent();
                //p.FileContent = XmlService.Deserialize<Ddata>(pre.DeclareContent.ToString());

                //p.Patient = p.Patient.GetCustomerByCusId(p.Patient.ID);
                //p.FileContent = XmlService.Deserialize<Ddata>(p.FileContentStr);
                for (var i = 1; i <= p.FileContent.Dbody.Pdata.Count; i++)
                {
                    p.FileContent.Dbody.Pdata[i - 1].P10 = (i.ToString()).PadLeft(3, '0');
                }
                p.FileContent.Dbody.D31 = p.FileContent.Dbody.Pdata.Where(pd => !pd.PaySelf && pd.P1.Equals("3")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(7, '0');
                p.FileContent.Dbody.D32 = p.FileContent.Dbody.Pdata.Where(pd => !pd.PaySelf && pd.P1.Equals("2")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                p.FileContent.Dbody.D33 = p.FileContent.Dbody.Pdata.Where(pd => !pd.PaySelf && pd.P1.Equals("1")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                p.FileContent.Dbody.D38 = p.FileContent.Dbody.Pdata.Where(pd => pd.P1.Equals("9")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                p.FileContent.Dhead.D18 = (int.Parse(p.FileContent.Dbody.D31) + int.Parse(p.FileContent.Dbody.D32) + int.Parse(p.FileContent.Dbody.D33) + int.Parse(p.FileContent.Dbody.D38)).ToString().PadLeft(8, '0');
                p.FileContent.Dhead.D16 = (int.Parse(p.FileContent.Dhead.D18) - int.Parse(p.FileContent.Dhead.D17)).ToString().PadLeft(8, '0');
            }
        }

        public void AdjustPharmacist(List<PharmacistScheduleItem> pharmacistList)
        {
            foreach (var g in this.Where(p => p.IsDeclare).GroupBy(decPres => decPres.AdjustDate).Select(group => group.ToList()))
            {
                var tempPharmacistList = pharmacistList.Where(p => p.Date.Equals(g[0].AdjustDate)).OrderBy(p => p.RegisterTime).ToList();
                var phCount = tempPharmacistList.Count;
                if (phCount > 0)
                {
                    var partitionList = Partition(g.Where(p => p.IsDeclare).OrderByDescending(p => p.MedicalServicePoint).ThenBy(p => p.InsertTime).ToList(), phCount).ToList();
                    if (partitionList.Count > phCount)
                    {
                        var partitionListCount = partitionList.Count;
                        for (var i = partitionListCount - 1; i >= phCount; i--)
                        {
                            foreach (var p in partitionList[i])
                            {
                                partitionList[phCount - 1].Add(p);
                            }
                            partitionList.RemoveAt(i);
                        }
                    }
                    for (var i = 0; i < phCount; i++)
                    {
                        for (var j = 1; j <= partitionList[i].Count; j++)
                        {
                            var k = j - 1;
                            var pre = partitionList[i][k];
                            pre.Pharmacist = ViewModelMainWindow.GetMedicalPersonByID(tempPharmacistList[i].MedicalPersonnel.ID);
                            pre.FileContent.Dhead.D25 = pre.Pharmacist.IDNumber;
                            //pre.ApplyPoint -= pre.MedicalServicePoint;
                            //pre.TotalPoint -= pre.MedicalServicePoint;
                            //pre.Pharmacist = ViewModelMainWindow.GetMedicalPersonByID(tempPharmacistList[i].MedicalPersonnel.ID);
                            //pre.FileContent.Dhead.D25 = pre.Pharmacist.IDNumber;
                            //int days = pre.MedicineDays;
                            //if (j <= 80)
                            //{
                            //    if (days >= 28)
                            //    {
                            //        pre.MedicalServicePoint = 69;
                            //        pre.MedicalServiceID = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                            //    }
                            //    else if (days > 7 && days < 14)
                            //    {
                            //        pre.MedicalServicePoint = 48;
                            //        pre.MedicalServiceID = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                            //    }
                            //    else if (days >= 14 && days < 28)
                            //    {
                            //        pre.MedicalServicePoint = 59;
                            //        pre.MedicalServiceID = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                            //    }
                            //    else
                            //    {
                            //        pre.MedicalServicePoint = 48;
                            //        pre.MedicalServiceID = "05202B";//一般處方給付(7天以內)
                            //    }
                            //}
                            //else if (j > 80 && j <= 100)
                            //{
                            //    pre.MedicalServicePoint = 18;
                            //    pre.MedicalServiceID = "05234D";//門診藥事服務費－每人每日81-100件內
                            //}
                            //else
                            //{
                            //    pre.MedicalServicePoint = 0;
                            //    if (days >= 28)
                            //        pre.MedicalServiceID = "05210B";
                            //    else if (days > 7 && days < 14)
                            //        pre.MedicalServiceID = "05223B";
                            //    else if (days >= 14 && days < 28)
                            //        pre.MedicalServiceID = "05206B";
                            //    else
                            //        pre.MedicalServiceID = "05202B";
                            //}
                            //pre.FileContent.Dbody.D38 = pre.MedicalServicePoint.ToString().PadLeft(8, '0');
                            //pre.FileContent.Dbody.D37 = partitionList[i][k].MedicalServiceID;
                            //var medicalService = pre.FileContent.Dbody.Pdata.Single(p => p.P1.Equals("9"));
                            //medicalService.P2 = pre.FileContent.Dbody.D37;
                            //medicalService.P8 = pre.FileContent.Dbody.D38;
                            //medicalService.P9 = pre.FileContent.Dbody.D38;
                            //pre.ApplyPoint += pre.MedicalServicePoint;
                            //pre.TotalPoint += pre.MedicalServicePoint;
                            //pre.DeclareContent = new SqlXml(new XmlTextReader(
                            //    XmlService.ToXmlDocument(partitionList[i][k].FileContent.SerializeObjectToXDocument()).InnerXml,
                            //    XmlNodeType.Document, null));
                        }
                    }
                }
                else
                {
                    MessageWindow.ShowMessage(g[0].AdjustDate.Month + "/" + g[0].AdjustDate.Day + " 超過合理調劑量但並未設定欲調整藥師，按ok繼續",MessageType.WARNING);
                }
            }
            AdjustMedicalService();
            PrescriptionDb.UpdatePrescriptionFromDeclareAdjust(this);
        }

        public IEnumerable<List<T>> Partition<T>(IList<T> source,int pharmacistCount)
        {
            var size = source.Count / pharmacistCount;
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }

        public void AdjustMedicalServiceAndSerialNumber()
        {
            AdjustMedicalService();
            AdjustSerialNumber();
            PrescriptionDb.UpdatePrescriptionFromDeclareAdjust(this);
        }

        private void AdjustMedicalService()
        {
            foreach (var g in this.Where(p => p.IsDeclare).GroupBy(decPres => decPres.AdjustDate).Select(group => group.ToList()))
            {
                foreach (var pres in g.GroupBy(pres => pres.Pharmacist.IDNumber))
                {
                    var pharmacist = ViewModelMainWindow.GetMedicalPersonByIDNumber(pres.Key);
                    var pList = pres.OrderByDescending(pre => (int)Math.Round(Convert.ToDouble((pre.MedicalServicePoint * double.Parse(pre.FileContent.Dbody.Pdata.Single(p => p.P1.Equals("9")).P6) / 100).ToString()), MidpointRounding.AwayFromZero)).ThenBy(p => p.InsertTime).ToList();
                    for (var j = 1; j <= pList.Count; j++)
                    {
                        var k = j - 1;
                        var pre = pList[k];
                        pre.ApplyPoint -= pre.MedicalServicePoint;
                        pre.TotalPoint -= pre.MedicalServicePoint;
                        pre.Pharmacist = pharmacist;
                        pre.FileContent.Dhead.D25 = pre.Pharmacist.IDNumber;
                        int days = pre.MedicineDays;
                        var medicalService = pre.FileContent.Dbody.Pdata.Single(p => p.P1.Equals("9"));
                        var servicePoint = 0;
                        if (j <= 80)
                        {
                            if (days >= 28)
                            {
                                servicePoint = 69;
                                pre.MedicalServiceID = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                            }
                            else if (days > 7 && days < 14)
                            {
                                servicePoint = 48;
                                pre.MedicalServiceID = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                            }
                            else if (days >= 14 && days < 28)
                            {
                                servicePoint = 59;
                                pre.MedicalServiceID = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                            }
                            else
                            {
                                servicePoint = 48;
                                pre.MedicalServiceID = "05202B";//一般處方給付(7天以內)
                            }
                        }
                        else if (j > 80 && j <= 100)
                        {
                            servicePoint = 18;
                            pre.MedicalServiceID = "05234D";//門診藥事服務費－每人每日81-100件內
                        }
                        else
                        {
                            servicePoint = 0;
                            if (days >= 28)
                                pre.MedicalServiceID = "05210B";
                            else if (days > 7 && days < 14)
                                pre.MedicalServiceID = "05223B";
                            else if (days >= 14 && days < 28)
                                pre.MedicalServiceID = "05206B";
                            else
                                pre.MedicalServiceID = "05202B";
                        }
                        pre.MedicalServicePoint  = (int)Math.Round(Convert.ToDouble((servicePoint * double.Parse(medicalService.P6) / 100).ToString()), MidpointRounding.AwayFromZero);
                        pre.FileContent.Dbody.D38 = pre.MedicalServicePoint.ToString().PadLeft(8, '0');
                        pre.FileContent.Dbody.D37 = pre.MedicalServiceID;
                        medicalService.P2 = pre.FileContent.Dbody.D37;
                        medicalService.P8 = $"{servicePoint:0000000.00}";
                        medicalService.P9 = pre.FileContent.Dbody.D38;
                        pre.ApplyPoint += pre.MedicalServicePoint;
                        pre.TotalPoint += pre.MedicalServicePoint;
                        pre.DeclareContent = new SqlXml(new XmlTextReader(
                            XmlService.ToXmlDocument(pre.FileContent.SerializeObjectToXDocument()).InnerXml,
                            XmlNodeType.Document, null));
                    }
                }
            }
        }
        private void AdjustSerialNumber()
        {
            foreach (var g in this.Where(p => p.IsDeclare).OrderBy(d => int.Parse(d.AdjustCase.ID)).GroupBy(d => d.AdjustCase.ID).Select(group => group.ToList()).ToList())
            {
                var serial = 1;
                foreach (var ddata in g)
                {
                    ddata.SerialNumber = serial;
                    ddata.FileContent.Dhead.D2 = serial.ToString().PadLeft(6, '0');
                    serial++;
                }
            }
        }
    }
}
