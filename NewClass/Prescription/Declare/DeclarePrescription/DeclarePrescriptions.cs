using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Xml;
using His_Pos.ChromeTabViewModel;
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

        public void GetSearchPrescriptions(DateTime decStart, DateTime decEnd)
        {
            var table = DeclarePrescriptionDb.GetDeclarePrescriptionsByMonthRange(decStart, decEnd);
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
                //for (var i = 1; i <= p.FileContent.Dbody.Pdata.Count; i++)
                //{
                //    p.FileContent.Dbody.Pdata[i - 1].P10 = (i.ToString()).PadLeft(3, '0');
                //}
                //p.FileContent.Dbody.D31 = p.FileContent.Dbody.Pdata.Where(pd => !pd.PaySelf && pd.P1.Equals("3")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(7, '0');
                //p.FileContent.Dbody.D33 = p.FileContent.Dbody.Pdata.Where(pd => !pd.PaySelf && pd.P1.Equals("1")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                //p.FileContent.Dbody.D38 = p.FileContent.Dbody.Pdata.Where(pd => pd.P1.Equals("9")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                //p.FileContent.Dhead.D18 = (int.Parse(p.FileContent.Dbody.D31) + int.Parse(p.FileContent.Dbody.D32) + int.Parse(p.FileContent.Dbody.D33) + int.Parse(p.FileContent.Dbody.D38)).ToString().PadLeft(8, '0');
                //p.FileContent.Dhead.D16 = (int.Parse(p.FileContent.Dhead.D18) - int.Parse(p.FileContent.Dhead.D17)).ToString().PadLeft(8, '0');
            }
        }

        public void AdjustPharmacist(List<PharmacistScheduleItem> pharmacistList)
        {
            foreach (var g in this.GroupBy(decPres => decPres.AdjustDate).Select(group => group.ToList()))
            {
                var phCount = pharmacistList.Count(p => p.Date.Equals(g[0].AdjustDate));
                if (phCount > 0)
                {
                    var partitionList = Partition(g.Where(p => p.IsDeclare).OrderByDescending(p => p.MedicalServicePoint).ThenBy(p => p.InsertTime).ToList(), phCount).ToList();
                    if (partitionList.Count > phCount)
                    {
                        for (var i = phCount; i < partitionList.Count; i++)
                        {
                            foreach (var p in partitionList[i])
                            {
                                partitionList[phCount - 1].Add(p);
                            }
                        }
                    }
                    for (var i = 0; i < phCount; i++)
                    {
                        for (var j = 1; j <= partitionList[i].Count; j++)
                        {
                            var k = j - 1;
                            var pre = partitionList[i][k];
                            pre.ApplyPoint -= pre.MedicalServicePoint;
                            pre.TotalPoint -= pre.MedicalServicePoint;
                            pre.Pharmacist = ViewModelMainWindow.GetMedicalPersonByID(pharmacistList[i].MedicalPersonnel.ID);
                            pre.FileContent.Dhead.D25 = pre.Pharmacist.IDNumber;
                            int days = pre.MedicineDays;
                            if (j <= 80)
                            {
                                if (days >= 28)
                                {
                                    pre.MedicalServicePoint = 69;
                                    pre.MedicalServiceID = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                                }
                                else if (days > 7 && days < 14)
                                {
                                    pre.MedicalServicePoint = 48;
                                    pre.MedicalServiceID = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                                }
                                else if (days >= 14 && days < 28)
                                {
                                    pre.MedicalServicePoint = 59;
                                    pre.MedicalServiceID = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                                }
                                else
                                {
                                    pre.MedicalServicePoint = 48;
                                    pre.MedicalServiceID = "05202B";//一般處方給付(7天以內)
                                }
                                pre.FileContent.Dbody.D38 = partitionList[i][k].MedicalServicePoint.ToString().PadLeft(8, '0');
                            }
                            else if (j > 80 && j <= 100)
                            {
                                pre.MedicalServicePoint = 18;
                                pre.MedicalServiceID = "05234D";//門診藥事服務費－每人每日81-100件內
                            }
                            else
                            {
                                pre.MedicalServicePoint = 0;
                                if (days >= 28)
                                    pre.MedicalServiceID = "05210B";
                                else if (days > 7 && days < 14)
                                    pre.MedicalServiceID = "05223B";
                                else if (days >= 14 && days < 28)
                                    pre.MedicalServiceID = "05206B";
                                else
                                    pre.MedicalServiceID = "05202B";
                                pre.FileContent.Dbody.D38 = partitionList[i][k].MedicalServicePoint.ToString().PadLeft(8, '0');
                            }
                            pre.FileContent.Dbody.D37 = partitionList[i][k].MedicalServiceID;
                            pre.FileContent.Dbody.Pdata.Single(p => p.P1.Equals("9")).P2 =
                                pre.FileContent.Dbody.D37;
                            pre.FileContent.Dbody.Pdata.Single(p => p.P1.Equals("9")).P9 =
                                pre.FileContent.Dbody.D38;
                            pre.ApplyPoint += partitionList[i][k].MedicalServicePoint;
                            pre.TotalPoint += partitionList[i][k].MedicalServicePoint;
                            pre.DeclareContent = new SqlXml(new XmlTextReader(
                                XmlService.ToXmlDocument(partitionList[i][k].FileContent.SerializeObjectToXDocument()).InnerXml,
                                XmlNodeType.Document, null));
                            Console.WriteLine(partitionList[i][k].MedicalServiceID + " " + partitionList[i][k].FileContent.Dbody.D37);
                        }
                    }
                }
            }
            PrescriptionDb.UpdatePrescriptionFromDeclareAdjust(this);
        }

        public IEnumerable<List<T>> Partition<T>(IList<T> source,int pharmacistCount)
        {
            var size = source.Count / pharmacistCount;
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}
