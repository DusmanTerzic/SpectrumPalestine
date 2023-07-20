using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Application;
using FrekvencijeProject.Models.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers.UploadFIles
{
    public class UploadSingleDoc
    {
        private readonly ApplicationDBContext _conApp;


        public UploadSingleDoc(ApplicationDBContext conApp)
        {
            _conApp  = conApp;
        }


        public void ExecuteUpload(List<DocumentsDb> SingleDocuments, int ApplicationTermsDBId, int ApplicationId, string document)
        {

            var ValuesDocuments = (from ww in _conApp.DocumentsDb
                                   select new
                                   {

                                       ww.DocumentsId,
                                       ww.Doc_number,
                                       ww.Title_of_doc,
                                       ww.Hyperlink,
                                       ww.Low_freq,
                                       ww.High_freq,
                                       ww.Application,
                                       ww.Type_of_doc

                                   }
                                                               ).ToList();

            if (SingleDocuments.Count == 0)
            {
                var root = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTermsDBId).FirstOrDefault();
                if (root.Layer == 1)
                {
                    var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == root.ApplicationTermsDBId).ToList();
                    var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == ApplicationId).SingleOrDefault();
                    var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                    var LowStr = "";
                    var HighStr = "";
                    long Low = 0;
                    long High = 0;
                    foreach (var tempRoot in valuesRoot)
                    {
                        var SingleDocumentsLayerOne = ValuesDocuments.Where(x => x.Doc_number.Contains(document) && x.Application.Equals(tempRoot.name)).ToList();

                        foreach (var tempValueLayerOne in SingleDocumentsLayerOne)
                        {

                            if (tempValueLayerOne.Low_freq.Contains("GHz"))
                            {
                                var tempLow = tempValueLayerOne.Low_freq.Split(" GHz");
                                LowStr = tempLow[0];

                                Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            }
                            else if (tempValueLayerOne.Low_freq.Contains("MHz"))
                            {
                                var tempLow = tempValueLayerOne.Low_freq.Split(" MHz");
                                LowStr = tempLow[0];

                                Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            }
                            else if (tempValueLayerOne.Low_freq.Contains("kHz"))
                            {
                                var tempLow = tempValueLayerOne.Low_freq.Split(" kHz");
                                LowStr = tempLow[0];

                                Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            }

                            else if (tempValueLayerOne.Low_freq.Contains("Hz"))
                            {
                                var tempLow = tempValueLayerOne.Low_freq.Split(" Hz");
                                LowStr = tempLow[0];
                                Low = long.Parse(LowStr);
                            }


                            if (tempValueLayerOne.High_freq.Contains("GHz"))
                            {
                                var tempHIgh = tempValueLayerOne.High_freq.Split(" GHz");
                                HighStr = tempHIgh[0];
                                
                                High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            }
                            else if (tempValueLayerOne.High_freq.Contains("MHz"))
                            {
                                var tempHIgh = tempValueLayerOne.High_freq.Split(" MHz");
                                HighStr = tempHIgh[0];
                                
                                High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            }
                            else if (tempValueLayerOne.High_freq.Contains("kHz"))
                            {
                                var tempHIgh = tempValueLayerOne.High_freq.Split(" kHz");
                                HighStr = tempHIgh[0];
                                
                                High = (long)Math.Round(1000 * double.Parse(HighStr));
                            }
                            else if (tempValueLayerOne.High_freq.Contains("Hz"))
                            {
                                var tempHIgh = tempValueLayerOne.High_freq.Split(" Hz");
                                HighStr = tempHIgh[0];
                                High = long.Parse(HighStr);
                            }

                            if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            {
                                if (tempValueLayerOne.Type_of_doc == "R")
                                {
                                    if (ApplicationTemp.documents_Id == null)
                                    {
                                        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerOne.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                        x.documents_Id == tempValueLayerOne.DocumentsId
                                        ).SingleOrDefault();
                                        if (isExistApp == null)
                                        {
                                            ApplicationDB newInsertApp = new ApplicationDB()
                                            {
                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                documents_Id = tempValueLayerOne.DocumentsId,
                                                comment = ApplicationTemp.comment,
                                            };
                                            _conApp.Application.Add(newInsertApp);
                                            _conApp.SaveChanges();

                                            ApplicationTemp.isDeletedApp = true;
                                            _conApp.Application.Update(ApplicationTemp);
                                            _conApp.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTemp.documents_Id).FirstOrDefault();
                                        if (!tempValueLayerOne.Doc_number.Equals(documentsTemp.Doc_number))
                                        {
                                            var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerOne.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                            var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                            x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                            x.documents_Id == tempValueLayerOne.DocumentsId
                                            ).SingleOrDefault();
                                            if (isExistApp == null)
                                            {
                                                ApplicationDB newInsertApp = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                    ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                    documents_Id = tempValueLayerOne.DocumentsId,
                                                    comment = ApplicationTemp.comment,
                                                };
                                                _conApp.Application.Add(newInsertApp);
                                                _conApp.SaveChanges();


                                            }
                                        }
                                    }
                                }

                                if (tempValueLayerOne.Type_of_doc == "I")
                                {

                                    if (ApplicationTemp.documents_Id == null)
                                    {
                                        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerOne.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                        x.documents_Id == tempValueLayerOne.DocumentsId
                                        ).SingleOrDefault();
                                        if (isExistApp == null)
                                        {
                                            ApplicationDB newInsertApp = new ApplicationDB()
                                            {
                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                documents_Id = tempValueLayerOne.DocumentsId,
                                                comment = ApplicationTemp.comment,
                                            };
                                            _conApp.Application.Add(newInsertApp);
                                            _conApp.SaveChanges();

                                            ApplicationTemp.isDeletedApp = true;
                                            _conApp.Application.Update(ApplicationTemp);
                                            _conApp.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTemp.documents_Id).FirstOrDefault();
                                        if (!tempValueLayerOne.Doc_number.Equals(documentsTemp.Doc_number))
                                        {
                                            var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerOne.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                            var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                            x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                            x.documents_Id == tempValueLayerOne.DocumentsId
                                            ).SingleOrDefault();
                                            if (isExistApp == null)
                                            {
                                                ApplicationDB newInsertApp = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                    ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                    documents_Id = tempValueLayerOne.DocumentsId,
                                                    comment = ApplicationTemp.comment,
                                                };
                                                _conApp.Application.Add(newInsertApp);
                                                _conApp.SaveChanges();


                                            }
                                        }
                                    }
                                }

                            }


                        }
                    }


                }
                else if (root.Layer == 2)
                {
                    var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == root.ApplicationTermsDBId).ToList();
                    var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == ApplicationId).SingleOrDefault();
                    var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                    var LowStr = "";
                    var HighStr = "";
                    long Low = 0;
                    long High = 0;
                    foreach (var tempRoot in valuesRoot)
                    {
                        var SingleDocumentsLayerTwo = ValuesDocuments.Where(x => x.Doc_number.Contains(document) && x.Application.Equals(tempRoot.name)).ToList();

                        foreach (var tempValueLayerTwo in SingleDocumentsLayerTwo)
                        {

                            if (tempValueLayerTwo.Low_freq.Contains("GHz"))
                            {
                                var tempLow = tempValueLayerTwo.Low_freq.Split(" GHz");
                                LowStr = tempLow[0];

                                Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            }
                            else if (tempValueLayerTwo.Low_freq.Contains("MHz"))
                            {
                                var tempLow = tempValueLayerTwo.Low_freq.Split(" MHz");
                                LowStr = tempLow[0];

                                Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            }
                            else if (tempValueLayerTwo.Low_freq.Contains("kHz"))
                            {
                                var tempLow = tempValueLayerTwo.Low_freq.Split(" kHz");
                                LowStr = tempLow[0];

                                Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            }

                            else if (tempValueLayerTwo.Low_freq.Contains("Hz"))
                            {
                                var tempLow = tempValueLayerTwo.Low_freq.Split(" Hz");
                                LowStr = tempLow[0];
                                Low = long.Parse(LowStr);
                            }


                            if (tempValueLayerTwo.High_freq.Contains("GHz"))
                            {
                                var tempHIgh = tempValueLayerTwo.High_freq.Split(" GHz");
                                HighStr = tempHIgh[0];
                                
                                High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            }
                            else if (tempValueLayerTwo.High_freq.Contains("MHz"))
                            {
                                var tempHIgh = tempValueLayerTwo.High_freq.Split(" MHz");
                                HighStr = tempHIgh[0];
                                
                                High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            }
                            else if (tempValueLayerTwo.High_freq.Contains("kHz"))
                            {
                                var tempHIgh = tempValueLayerTwo.High_freq.Split(" kHz");
                                HighStr = tempHIgh[0];
                                
                                High = (long)Math.Round(1000 * double.Parse(HighStr));
                            }
                            else if (tempValueLayerTwo.High_freq.Contains("Hz"))
                            {
                                var tempHIgh = tempValueLayerTwo.High_freq.Split(" Hz");
                                HighStr = tempHIgh[0];
                                High = long.Parse(HighStr);
                            }

                            if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            {
                                if (tempValueLayerTwo.Type_of_doc == "R")
                                {
                                    if(ApplicationTemp.documents_Id == null)
                                    {
                                        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                        x.documents_Id == tempValueLayerTwo.DocumentsId
                                        ).SingleOrDefault();
                                        if (isExistApp == null)
                                        {
                                            ApplicationDB newInsertApp = new ApplicationDB()
                                            {
                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                documents_Id = tempValueLayerTwo.DocumentsId,
                                                comment = ApplicationTemp.comment,
                                            };
                                            _conApp.Application.Add(newInsertApp);
                                            _conApp.SaveChanges();

                                            ApplicationTemp.isDeletedApp = true;
                                            _conApp.Application.Update(ApplicationTemp);
                                            _conApp.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTemp.documents_Id).FirstOrDefault();
                                        if (!tempValueLayerTwo.Doc_number.Equals(documentsTemp.Doc_number))
                                        {
                                            var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                            var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                            x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                            x.documents_Id == tempValueLayerTwo.DocumentsId
                                            ).SingleOrDefault();
                                            if (isExistApp == null)
                                            {
                                                ApplicationDB newInsertApp = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                    ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                    documents_Id = tempValueLayerTwo.DocumentsId,
                                                    comment = ApplicationTemp.comment,
                                                };
                                                _conApp.Application.Add(newInsertApp);
                                                _conApp.SaveChanges();

                                                
                                            }
                                        }
                                    }
                                    
                                }


                                if (tempValueLayerTwo.Type_of_doc == "I")
                                {
                                    if (ApplicationTemp.documents_Id == null)
                                    {
                                        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                        x.documents_Id == tempValueLayerTwo.DocumentsId
                                        ).SingleOrDefault();
                                        if (isExistApp == null)
                                        {
                                            ApplicationDB newInsertApp = new ApplicationDB()
                                            {
                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                documents_Id = tempValueLayerTwo.DocumentsId,
                                                comment = ApplicationTemp.comment,
                                            };
                                            _conApp.Application.Add(newInsertApp);
                                            _conApp.SaveChanges();

                                            ApplicationTemp.isDeletedApp = true;
                                            _conApp.Application.Update(ApplicationTemp);
                                            _conApp.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTemp.documents_Id).FirstOrDefault();
                                        if (!tempValueLayerTwo.Doc_number.Equals(documentsTemp.Doc_number))
                                        {
                                            var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.ApplicationTermsDBId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                            var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                            x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                            x.documents_Id == tempValueLayerTwo.DocumentsId
                                            ).SingleOrDefault();
                                            if (isExistApp == null)
                                            {
                                                ApplicationDB newInsertApp = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                    ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                    documents_Id = tempValueLayerTwo.DocumentsId,
                                                    comment = ApplicationTemp.comment,
                                                };
                                                _conApp.Application.Add(newInsertApp);
                                                _conApp.SaveChanges();

                                            }
                                        }
                                    }

                                }

                                //    if (tempValueLayerTwo.Type_of_doc == "R")
                                //    {
                                //    var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerTwo.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //    && x.ApplicationTermId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                //    if (ApplicationTempNew != null)
                                //    {
                                //        if (ApplicationTempNew.documents_Id == null)
                                //        {
                                //            ApplicationTempNew.documents_Id = tempValueLayerTwo.DocumentsId;
                                //            _conApp.Application.Update(ApplicationTempNew);
                                //            _conApp.SaveChanges();
                                //        }
                                //        else if (ApplicationTempNew.documents_Id != tempValueLayerTwo.DocumentsId)
                                //        {

                                //            ApplicationDB app = new ApplicationDB()
                                //            {
                                //                ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                comment = null,
                                //                documents_Id = tempValueLayerTwo.DocumentsId

                                //            };

                                //            _conApp.Application.Add(app);
                                //            _conApp.SaveChanges();

                                //        }
                                //    }
                                //    else
                                //    {
                                //        ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                                //        _conApp.Application.Update(ApplicationTemp);
                                //        _conApp.SaveChanges();
                                //    }
                                //}

                                //if (tempValueLayerTwo.Type_of_doc == "I")
                                //{

                                //    var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerTwo.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //  && x.ApplicationTermId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                //    if (ApplicationTempNew != null)
                                //    {
                                //        if (ApplicationTempNew.documents_Id == null)
                                //        {
                                //            ApplicationTempNew.documents_Id = tempValueLayerTwo.DocumentsId;
                                //            _conApp.Application.Update(ApplicationTempNew);
                                //            _conApp.SaveChanges();
                                //        }
                                //        else if (ApplicationTempNew.documents_Id != tempValueLayerTwo.DocumentsId)
                                //        {


                                //            ApplicationDB app = new ApplicationDB()
                                //            {
                                //                ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                comment = null,
                                //                documents_Id = tempValueLayerTwo.DocumentsId

                                //            };

                                //            _conApp.Application.Add(app);
                                //            _conApp.SaveChanges();


                                //        }
                                //    }
                                //    else
                                //    {
                                //        ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                                //        _conApp.Application.Update(ApplicationTemp);
                                //        _conApp.SaveChanges();
                                //    }
                                //}

                            }


                        }
                    }
                }

            }
            else if (SingleDocuments.Count > 0)
            {

                foreach (var tempValue in SingleDocuments)
                {

                    var root = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTermsDBId).FirstOrDefault();

                    if (root.Layer == 1)
                    {

                        var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == root.ApplicationTermsDBId).ToList();
                        var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == ApplicationId).SingleOrDefault();
                        var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                        var LowStr = "";
                        var HighStr = "";
                        long Low = 0;
                        long High = 0;

                        if (tempValue.Low_freq.Contains("GHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" GHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                        }
                        else if (tempValue.Low_freq.Contains("MHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" MHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                        }
                        else if (tempValue.Low_freq.Contains("kHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" kHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000 * double.Parse(LowStr));
                        }

                        else if (tempValue.Low_freq.Contains("Hz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" Hz");
                            LowStr = tempLow[0];
                            Low = long.Parse(LowStr);
                        }


                        if (tempValue.High_freq.Contains("GHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" GHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000000000;
                            High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                        }
                        else if (tempValue.High_freq.Contains("MHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" MHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000000;
                            High = (long)Math.Round(1000000 * double.Parse(HighStr));
                        }
                        else if (tempValue.High_freq.Contains("kHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" kHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000;
                            High = (long)Math.Round(1000 * double.Parse(HighStr));
                        }
                        else if (tempValue.High_freq.Contains("Hz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" Hz");
                            HighStr = tempHIgh[0];
                            High = long.Parse(HighStr);
                        }

                        if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                        {
                            if (valuesRoot.Count == 0)
                            {
                                var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                   && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                                if (tempValue.Type_of_doc == "R")
                                {
                                    if (ApplicationTempNew != null)
                                    {
                                        if (ApplicationTempNew.documents_Id == null)
                                        {
                                            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                            _conApp.Application.Update(ApplicationTempNew);
                                            _conApp.SaveChanges();
                                        }
                                        else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                        {
                                            var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                            if (!tempValue.Doc_number.Equals(documentsTemp.Doc_number))
                                            {
                                                ApplicationDB app = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                    comment = ApplicationTempNew.comment,
                                                    documents_Id = tempValue.DocumentsId

                                                };

                                                _conApp.Application.Add(app);
                                                _conApp.SaveChanges();
                                            }

                                        }
                                    }
                                    else
                                    {
                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                        _conApp.Application.Update(ApplicationTemp);
                                        _conApp.SaveChanges();
                                    }
                                }

                                if (tempValue.Type_of_doc == "I")
                                {

                                    if (ApplicationTempNew != null)
                                    {

                                        if (ApplicationTempNew.documents_Id == null)
                                        {
                                            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                            _conApp.Application.Update(ApplicationTempNew);
                                            _conApp.SaveChanges();
                                        }
                                        else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                        {

                                            var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                            if (!tempValue.Doc_number.Equals(documentsTemp.Doc_number))
                                            {
                                                ApplicationDB app = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                    comment = ApplicationTempNew.comment,
                                                    documents_Id = tempValue.DocumentsId

                                                };

                                                _conApp.Application.Add(app);
                                                _conApp.SaveChanges();
                                            }


                                        }
                                    }
                                    else
                                    {
                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                        _conApp.Application.Update(ApplicationTemp);
                                        _conApp.SaveChanges();
                                    }
                                }
                            }


                            //if (tempValue.Type_of_doc == "R")
                            //{
                            //    if (ApplicationTemp.documents_Id == null)
                            //    {
                            //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //        _conApp.Application.Update(ApplicationTemp);
                            //        _conApp.SaveChanges();
                            //    }
                            //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //    {
                            //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //        if (ApplicationVal != null)
                            //        {
                            //            if (ApplicationVal.documents_Id == null)
                            //            {
                            //                ApplicationDB app = new ApplicationDB()
                            //                {
                            //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                    comment = null,
                            //                    documents_Id = tempValue.DocumentsId

                            //                };

                            //                _conApp.Application.Add(app);
                            //                _conApp.SaveChanges();
                            //            }
                            //        }
                            //    }
                            //}


                            //if (tempValue.Type_of_doc == "I")
                            //{
                            //    if (ApplicationTemp.documents_Id == null)
                            //    {
                            //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //        _conApp.Application.Update(ApplicationTemp);
                            //        _conApp.SaveChanges();
                            //    }
                            //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //    {
                            //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //        if (ApplicationVal != null)
                            //        {
                            //            if (ApplicationVal.documents_Id == null)
                            //            {
                            //                ApplicationDB app = new ApplicationDB()
                            //                {
                            //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                    comment = null,
                            //                    documents_Id = tempValue.DocumentsId

                            //                };

                            //                _conApp.Application.Add(app);
                            //                _conApp.SaveChanges();
                            //            }
                            //        }
                            //    }
                            //}




                        }

                        foreach (var temp in valuesRoot)
                        {

                            // Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                            var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                            ).ToList();

                            foreach (var ttDoc in docTemp)
                            {
                                //Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                                var LowStrDoc = "";
                                var HighStrDoc = "";
                                long LowDoc = 0;
                                long HighDoc = 0;
                                if (ttDoc != null)
                                {
                                    if (ttDoc.Low_freq.Contains("GHz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" GHz");
                                        LowStrDoc = tempLow[0];

                                        LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                                    }
                                    else if (ttDoc.Low_freq.Contains("MHz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" MHz");
                                        LowStrDoc = tempLow[0];

                                        LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                                    }
                                    else if (ttDoc.Low_freq.Contains("kHz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" kHz");
                                        LowStrDoc = tempLow[0];

                                        LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                                    }

                                    else if (ttDoc.Low_freq.Contains("Hz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" Hz");
                                        LowStrDoc = tempLow[0];
                                        LowDoc = long.Parse(LowStrDoc);
                                    }


                                    if (ttDoc.High_freq.Contains("GHz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" GHz");
                                        HighStrDoc = tempHIgh[0];
                                        //High = (long)double.Parse(HighStr) * 1000000000;
                                        HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                                    }
                                    else if (ttDoc.High_freq.Contains("MHz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" MHz");
                                        HighStrDoc = tempHIgh[0];
                                        //High = (long)double.Parse(HighStr) * 1000000;
                                        HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                                    }
                                    else if (ttDoc.High_freq.Contains("kHz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" kHz");
                                        HighStrDoc = tempHIgh[0];

                                        //High = (long)double.Parse(HighStr) * 1000;
                                        HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                                    }
                                    else if (ttDoc.High_freq.Contains("Hz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" Hz");
                                        HighStrDoc = tempHIgh[0];
                                        HighDoc = long.Parse(HighStrDoc);
                                    }

                                    if ((ApplicationRange.low >= LowDoc && ApplicationRange.low <= HighDoc) || (ApplicationRange.high >= LowDoc && ApplicationRange.high <= HighDoc))
                                    {

                                        //if (ttDoc.Type_of_doc == "R")
                                        //{
                                        //    if (ApplicationTemp.documents_Id == null)
                                        //    {
                                        //        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.second_up_lvl_id == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                                        //        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                        //        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                        //        x.documents_Id == tempValueLayerTwo.DocumentsId
                                        //        ).SingleOrDefault();
                                        //        if (isExistApp == null)
                                        //        {
                                        //            ApplicationDB newInsertApp = new ApplicationDB()
                                        //            {
                                        //                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                        //                ApplicationTermId = appTerm.ApplicationTermsDBId,
                                        //                documents_Id = tempValueLayerTwo.DocumentsId,
                                        //                comment = ApplicationTemp.comment,
                                        //            };
                                        //            _conApp.Application.Add(newInsertApp);
                                        //            _conApp.SaveChanges();

                                        //            ApplicationTemp.isDeletedApp = true;
                                        //            _conApp.Application.Update(ApplicationTemp);
                                        //            _conApp.SaveChanges();
                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTemp.documents_Id).FirstOrDefault();
                                        //        if (!tempValueLayerTwo.Doc_number.Equals(documentsTemp.Doc_number))
                                        //        {
                                        //            var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.second_up_lvl_id == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                                        //            var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                        //            x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                        //            x.documents_Id == tempValueLayerTwo.DocumentsId
                                        //            ).SingleOrDefault();
                                        //            if (isExistApp == null)
                                        //            {
                                        //                ApplicationDB newInsertApp = new ApplicationDB()
                                        //                {
                                        //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                        //                    ApplicationTermId = appTerm.ApplicationTermsDBId,
                                        //                    documents_Id = tempValueLayerTwo.DocumentsId,
                                        //                    comment = ApplicationTemp.comment,
                                        //                };
                                        //                _conApp.Application.Add(newInsertApp);
                                        //                _conApp.SaveChanges();


                                        //            }
                                        //        }
                                        //    }

                                        //}

                                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == ttDoc.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                            && x.ApplicationTermId == temp.ApplicationTermsDBId).SingleOrDefault();
                                        if (ttDoc.Type_of_doc == "R")
                                        {
                                            if (ApplicationTempNew != null)
                                            {

                                                if (ApplicationTempNew.documents_Id == null)
                                                {

                                                    ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                                    _conApp.Application.Update(ApplicationTempNew);
                                                    _conApp.SaveChanges();
                                                }
                                                else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                                {
                                                    var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                                    if (!ttDoc.Doc_number.Equals(documentsTemp.Doc_number))
                                                    {
                                                        ApplicationDB app = new ApplicationDB()
                                                        {
                                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                            comment = ApplicationTempNew.comment,
                                                            documents_Id = ttDoc.DocumentsId

                                                        };

                                                        _conApp.Application.Add(app);
                                                        _conApp.SaveChanges();
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(ttDoc.Application) && x.ApplicationTermsDBId == temp.ApplicationTermsDBId).SingleOrDefault();
                                                var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                                        x.documents_Id == ttDoc.DocumentsId
                                                        ).SingleOrDefault();
                                                        if (isExistApp == null)
                                                        {
                                                            ApplicationDB newInsertApp = new ApplicationDB()
                                                            {
                                                                ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                                documents_Id = ttDoc.DocumentsId,
                                                                comment = ApplicationTemp.comment
                                                            };
                                                        
                                                            _conApp.Application.Add(newInsertApp);
                                                            _conApp.SaveChanges();
                                                        }
                                            }

                                        }


                                        if (ttDoc.Type_of_doc == "I")
                                        {
                                            if (ApplicationTempNew != null)
                                            {
                                                if (ApplicationTempNew.documents_Id == null)
                                                {
                                                    ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                                    _conApp.Application.Update(ApplicationTempNew);
                                                    _conApp.SaveChanges();
                                                }
                                                else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                                {

                                                    var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                                    if (!ttDoc.Doc_number.Equals(documentsTemp.Doc_number))
                                                    {
                                                        ApplicationDB app = new ApplicationDB()
                                                        {
                                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                            comment = ApplicationTempNew.comment,
                                                            documents_Id = ttDoc.DocumentsId

                                                        };

                                                        _conApp.Application.Add(app);
                                                        _conApp.SaveChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(ttDoc.Application) && x.ApplicationTermsDBId == temp.ApplicationTermsDBId).SingleOrDefault();
                                                var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                                        x.documents_Id == ttDoc.DocumentsId
                                                        ).SingleOrDefault();
                                                if (isExistApp == null)
                                                {
                                                    ApplicationDB newInsertApp = new ApplicationDB()
                                                    {
                                                        ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                        documents_Id = ttDoc.DocumentsId,
                                                        comment = ApplicationTemp.comment
                                                    };

                                                    _conApp.Application.Add(newInsertApp);
                                                    _conApp.SaveChanges();
                                                }


                                            }

                                        }

                                    }
                                }
                            }

                        }
                    }
                    else if (root.Layer == 2)
                    {

                        //Debug.WriteLine("layer 2:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString());
                        var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == root.ApplicationTermsDBId).ToList();
                        var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == ApplicationId).SingleOrDefault();
                        var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                        var LowStr = "";
                        var HighStr = "";
                        long Low = 0;
                        long High = 0;
                        if (tempValue.Low_freq.Contains("GHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" GHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                        }
                        else if (tempValue.Low_freq.Contains("MHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" MHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                        }
                        else if (tempValue.Low_freq.Contains("kHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" kHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000 * double.Parse(LowStr));
                        }

                        else if (tempValue.Low_freq.Contains("Hz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" Hz");
                            LowStr = tempLow[0];
                            Low = long.Parse(LowStr);
                        }


                        if (tempValue.High_freq.Contains("GHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" GHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000000000;
                            High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                        }
                        else if (tempValue.High_freq.Contains("MHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" MHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000000;
                            High = (long)Math.Round(1000000 * double.Parse(HighStr));
                        }
                        else if (tempValue.High_freq.Contains("kHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" kHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000;
                            High = (long)Math.Round(1000 * double.Parse(HighStr));
                        }
                        else if (tempValue.High_freq.Contains("Hz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" Hz");
                            HighStr = tempHIgh[0];
                            High = long.Parse(HighStr);
                        }

                        if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                        {
                            if (valuesRoot.Count == 0)
                            {

                                var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                   && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                                if (tempValue.Type_of_doc == "R")
                                {
                                    if (ApplicationTempNew != null)
                                    {
                                        if (ApplicationTempNew.documents_Id == null)
                                        {
                                            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                            _conApp.Application.Update(ApplicationTempNew);
                                            _conApp.SaveChanges();
                                        }
                                        else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                        {
                                            var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                            if (!tempValue.Doc_number.Equals(documentsTemp.Doc_number))
                                            {
                                                ApplicationDB app = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                    comment = ApplicationTempNew.comment,
                                                    documents_Id = tempValue.DocumentsId

                                                };

                                                _conApp.Application.Add(app);
                                                _conApp.SaveChanges();
                                            }

                                        }
                                    }
                                    else
                                    {
                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                        _conApp.Application.Update(ApplicationTemp);
                                        _conApp.SaveChanges();
                                    }
                                }

                                if (tempValue.Type_of_doc == "I")
                                {

                                    if (ApplicationTempNew != null)
                                    {

                                        if (ApplicationTempNew.documents_Id == null)
                                        {
                                            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                            _conApp.Application.Update(ApplicationTempNew);
                                            _conApp.SaveChanges();
                                        }
                                        else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                        {

                                            var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                            if (!tempValue.Doc_number.Equals(documentsTemp.Doc_number))
                                            {
                                                ApplicationDB app = new ApplicationDB()
                                                {
                                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                    comment = ApplicationTempNew.comment,
                                                    documents_Id = tempValue.DocumentsId

                                                };

                                                _conApp.Application.Add(app);
                                                _conApp.SaveChanges();
                                            }


                                        }
                                    }
                                    else
                                    {
                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                        _conApp.Application.Update(ApplicationTemp);
                                        _conApp.SaveChanges();
                                    }
                                }



                                //var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //       && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                                //if (tempValue.Type_of_doc == "R")
                                //{
                                //    if (ApplicationTempNew != null)
                                //    {

                                //        if (ApplicationTempNew.documents_Id == null)
                                //        {
                                //            //if (root.ApplicationTermsDBId == 152)
                                //            //{
                                //            //    Debug.WriteLine(" it is null:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //            //}
                                //            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //            _conApp.Application.Update(ApplicationTempNew);
                                //            _conApp.SaveChanges();
                                //        }
                                //        else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //        {
                                //            //if (root.ApplicationTermsDBId == 152)
                                //            //{
                                //            //    Debug.WriteLine(" it is diffrent:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //            //}
                                //            ApplicationDB app = new ApplicationDB()
                                //            {
                                //                ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                comment = null,
                                //                documents_Id = tempValue.DocumentsId

                                //            };

                                //            _conApp.Application.Add(app);
                                //            _conApp.SaveChanges();

                                //        }
                                //    }
                                //    else
                                //    {
                                //        //if (root.ApplicationTermsDBId == 152)
                                //        //{
                                //        //    Debug.WriteLine(" it is simple update:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //        //}
                                //        var queryDocument = (from all in _conApp.Application
                                //                             join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                                //                             where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                             && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                                //                             select new
                                //                             {
                                //                                 app_id = all.ApplicationId,
                                //                                 doc_id = all.documents_Id
                                //                             }
                                //                             ).ToList();
                                //        if (queryDocument.Count == 0)
                                //        {
                                //            //Debug.WriteLine("pp:");                                                                                 
                                //            if (ApplicationTemp.documents_Id == null)
                                //            {
                                //                //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                                //                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                _conApp.Application.Update(ApplicationTemp);
                                //                _conApp.SaveChanges();
                                //            }
                                //            else
                                //            {
                                //                // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                                //                //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                                //                //    + "===second doc:" + tempValue.Doc_number);
                                //                ApplicationTemp.ApplicationId = 0;
                                //                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                _conApp.Application.Add(ApplicationTemp);
                                //                _conApp.SaveChanges();
                                //            }

                                //        }
                                //        else
                                //        {
                                //            //Debug.WriteLine(":yes:"+ ApplicationRange.LowView);
                                //        }

                                //    }


                                //}

                                //if (tempValue.Type_of_doc == "I")
                                //{
                                //    if (ApplicationTempNew != null)
                                //    {
                                //        if (ApplicationTempNew.documents_Id == null)
                                //        {
                                //            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //            _conApp.Application.Update(ApplicationTempNew);
                                //            _conApp.SaveChanges();
                                //        }
                                //        else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //        {

                                //            ApplicationDB app = new ApplicationDB()
                                //            {
                                //                ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                comment = null,
                                //                documents_Id = tempValue.DocumentsId

                                //            };

                                //            _conApp.Application.Add(app);
                                //            _conApp.SaveChanges();
                                //        }
                                //    }
                                //    else
                                //    {
                                //        //ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //        //_conApp.Application.Update(ApplicationTemp);
                                //        //_conApp.SaveChanges();

                                //        var queryDocument = (from all in _conApp.Application
                                //                             join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                                //                             where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                             && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                                //                             select new
                                //                             {
                                //                                 app_id = all.ApplicationId,
                                //                                 doc_id = all.documents_Id
                                //                             }
                                //                            ).ToList();
                                //        if (queryDocument.Count == 0)
                                //        {
                                //            //Debug.WriteLine("pp:");                                                                                 
                                //            if (ApplicationTemp.documents_Id == null)
                                //            {
                                //                //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                                //                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                _conApp.Application.Update(ApplicationTemp);
                                //                _conApp.SaveChanges();
                                //            }
                                //            else
                                //            {
                                //                // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                                //                //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                                //                //    + "===second doc:" + tempValue.Doc_number);
                                //                ApplicationTemp.ApplicationId = 0;
                                //                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                _conApp.Application.Add(ApplicationTemp);
                                //                _conApp.SaveChanges();
                                //            }

                                //        }
                                //        else
                                //        {
                                //            //Debug.WriteLine(":yes:"+ ApplicationRange.LowView);
                                //        }
                                //    }

                                //}

                            }

                        }



                        foreach (var temp in valuesRoot)
                        {

                            //Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                            var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                            ).ToList();

                            foreach (var ttDoc in docTemp)
                            {
                                if (ttDoc != null)
                                {
                                    //Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                                    var LowStrDoc = "";
                                    var HighStrDoc = "";
                                    long LowDoc = 0;
                                    long HighDoc = 0;
                                    if (ttDoc.Low_freq.Contains("GHz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" GHz");
                                        LowStrDoc = tempLow[0];

                                        LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                                    }
                                    else if (ttDoc.Low_freq.Contains("MHz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" MHz");
                                        LowStrDoc = tempLow[0];

                                        LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                                    }
                                    else if (ttDoc.Low_freq.Contains("kHz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" kHz");
                                        LowStrDoc = tempLow[0];

                                        LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                                    }

                                    else if (ttDoc.Low_freq.Contains("Hz"))
                                    {
                                        var tempLow = ttDoc.Low_freq.Split(" Hz");
                                        LowStrDoc = tempLow[0];
                                        LowDoc = long.Parse(LowStrDoc);
                                    }


                                    if (ttDoc.High_freq.Contains("GHz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" GHz");
                                        HighStrDoc = tempHIgh[0];
                                        //High = (long)double.Parse(HighStr) * 1000000000;
                                        HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                                    }
                                    else if (ttDoc.High_freq.Contains("MHz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" MHz");
                                        HighStrDoc = tempHIgh[0];
                                        //High = (long)double.Parse(HighStr) * 1000000;
                                        HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                                    }
                                    else if (ttDoc.High_freq.Contains("kHz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" kHz");
                                        HighStrDoc = tempHIgh[0];

                                        //High = (long)double.Parse(HighStr) * 1000;
                                        HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                                    }
                                    else if (ttDoc.High_freq.Contains("Hz"))
                                    {
                                        var tempHIgh = ttDoc.High_freq.Split(" Hz");
                                        HighStrDoc = tempHIgh[0];
                                        HighDoc = long.Parse(HighStrDoc);
                                    }

                                    if ((ApplicationRange.low >= LowDoc && ApplicationRange.low <= HighDoc) || (ApplicationRange.high >= LowDoc && ApplicationRange.high <= HighDoc))
                                    {

                                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == ttDoc.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                            && x.ApplicationTermId == temp.ApplicationTermsDBId).SingleOrDefault();
                                        if (ttDoc.Type_of_doc == "R")
                                        {
                                            if (ApplicationTempNew != null)
                                            {

                                                if (ApplicationTempNew.documents_Id == null)
                                                {

                                                    ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                                    _conApp.Application.Update(ApplicationTempNew);
                                                    _conApp.SaveChanges();
                                                }
                                                else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                                {
                                                    var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                                    if (!ttDoc.Doc_number.Equals(documentsTemp.Doc_number))
                                                    {
                                                        ApplicationDB app = new ApplicationDB()
                                                        {
                                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                            comment = ApplicationTempNew.comment,
                                                            documents_Id = ttDoc.DocumentsId

                                                        };

                                                        _conApp.Application.Add(app);
                                                        _conApp.SaveChanges();
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(ttDoc.Application) && x.ApplicationTermsDBId == temp.ApplicationTermsDBId).SingleOrDefault();
                                                var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                                        x.documents_Id == ttDoc.DocumentsId
                                                        ).SingleOrDefault();
                                                if (isExistApp == null)
                                                {
                                                    ApplicationDB newInsertApp = new ApplicationDB()
                                                    {
                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                        documents_Id = ttDoc.DocumentsId,
                                                        comment = ApplicationTemp.comment
                                                    };

                                                    _conApp.Application.Add(newInsertApp);
                                                    _conApp.SaveChanges();
                                                }
                                            }

                                        }


                                        if (ttDoc.Type_of_doc == "I")
                                        {
                                            if (ApplicationTempNew != null)
                                            {
                                                if (ApplicationTempNew.documents_Id == null)
                                                {
                                                    ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                                    _conApp.Application.Update(ApplicationTempNew);
                                                    _conApp.SaveChanges();
                                                }
                                                else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                                {

                                                    var documentsTemp = _conApp.DocumentsDb.Where(x => x.DocumentsId == ApplicationTempNew.documents_Id).FirstOrDefault();
                                                    if (!ttDoc.Doc_number.Equals(documentsTemp.Doc_number))
                                                    {
                                                        ApplicationDB app = new ApplicationDB()
                                                        {
                                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                                            comment = ApplicationTempNew.comment,
                                                            documents_Id = ttDoc.DocumentsId

                                                        };

                                                        _conApp.Application.Add(app);
                                                        _conApp.SaveChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(ttDoc.Application) && x.ApplicationTermsDBId == temp.ApplicationTermsDBId).SingleOrDefault();
                                                var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                                                        x.documents_Id == ttDoc.DocumentsId
                                                        ).SingleOrDefault();
                                                if (isExistApp == null)
                                                {
                                                    ApplicationDB newInsertApp = new ApplicationDB()
                                                    {
                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                                                        documents_Id = ttDoc.DocumentsId,
                                                        comment = ApplicationTemp.comment
                                                    };

                                                    _conApp.Application.Add(newInsertApp);
                                                    _conApp.SaveChanges();
                                                }


                                            }

                                        }
                                        // Debug.WriteLine("hello im entered");

                                        //if (root.ApplicationTermsDBId == 152)
                                        //{
                                        //    Debug.WriteLine("low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId + "===" + ttDoc.DocumentsId);
                                        //}
                                        //var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == ttDoc.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                        //&& x.ApplicationTermId == temp.ApplicationTermsDBId).SingleOrDefault();
                                        //if (ttDoc.Type_of_doc == "R")
                                        //{
                                        //    if (ApplicationTempNew != null)
                                        //    {
                                        //        if (ApplicationTempNew.documents_Id == null)
                                        //        {
                                        //            ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                        //            _conApp.Application.Update(ApplicationTempNew);
                                        //            _conApp.SaveChanges();
                                        //        }
                                        //        else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                        //        {

                                        //            ApplicationDB app = new ApplicationDB()
                                        //            {
                                        //                ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                        //                ApplicationTermId = temp.ApplicationTermsDBId,
                                        //                comment = null,
                                        //                documents_Id = ttDoc.DocumentsId

                                        //            };

                                        //            _conApp.Application.Add(app);
                                        //            _conApp.SaveChanges();

                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                                        //        _conApp.Application.Update(ApplicationTemp);
                                        //        _conApp.SaveChanges();
                                        //    }



                                        //}

                                        //if (ttDoc.Type_of_doc == "I")
                                        //{
                                        //    if (ApplicationTempNew != null)
                                        //    {
                                        //        if (ApplicationTempNew.documents_Id == null)
                                        //        {
                                        //            ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                        //            _conApp.Application.Update(ApplicationTempNew);
                                        //            _conApp.SaveChanges();
                                        //        }
                                        //        else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                        //        {
                                        //            //Debug.WriteLine("second enter:");

                                        //            ApplicationDB app = new ApplicationDB()
                                        //            {
                                        //                ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                        //                ApplicationTermId = temp.ApplicationTermsDBId,
                                        //                comment = null,
                                        //                documents_Id = ttDoc.DocumentsId

                                        //            };

                                        //            _conApp.Application.Add(app);
                                        //            _conApp.SaveChanges();

                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                                        //        _conApp.Application.Update(ApplicationTemp);
                                        //        _conApp.SaveChanges();
                                        //    }
                                        //}

                                    }


                                }

                            }

                        }

                    }
                    else if (root.Layer == 3)
                    {
                        //Debug.WriteLine("layer 3:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString());
                        var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == ApplicationId).SingleOrDefault();
                        var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                        var LowStr = "";
                        var HighStr = "";
                        long Low = 0;
                        long High = 0;
                        if (tempValue.Low_freq.Contains("GHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" GHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                        }
                        else if (tempValue.Low_freq.Contains("MHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" MHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                        }
                        else if (tempValue.Low_freq.Contains("kHz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" kHz");
                            LowStr = tempLow[0];

                            Low = (long)Math.Round(1000 * double.Parse(LowStr));
                        }

                        else if (tempValue.Low_freq.Contains("Hz"))
                        {
                            var tempLow = tempValue.Low_freq.Split(" Hz");
                            LowStr = tempLow[0];
                            Low = long.Parse(LowStr);
                        }

                        //Debug.WriteLine("high:" + tempValue.High_freq + ":low:" + tempValue.Low_freq);

                        if (tempValue.High_freq.Contains("GHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" GHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000000000;
                            High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                        }
                        else if (tempValue.High_freq.Contains("MHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" MHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000000;
                            High = (long)Math.Round(1000000 * double.Parse(HighStr));
                        }
                        else if (tempValue.High_freq.Contains("kHz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" kHz");
                            HighStr = tempHIgh[0];
                            //High = (long)double.Parse(HighStr) * 1000;
                            High = (long)Math.Round(1000 * double.Parse(HighStr));
                        }
                        else if (tempValue.High_freq.Contains("Hz"))
                        {
                            var tempHIgh = tempValue.High_freq.Split(" Hz");
                            HighStr = tempHIgh[0];
                            High = long.Parse(HighStr);
                        }

                        //Debug.WriteLine("low from doc:" + Low + "===" + ApplicationRange.low);
                        //Debug.WriteLine("high from doc:" + High + "===" + ApplicationRange.high+"::"+ApplicationTemp.ApplicationId+"::range::"+ApplicationTemp.ApplicationRangeId);

                        if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                        {
                            var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId
                                   && x.ApplicationTermId == ApplicationTemp.ApplicationTermId).SingleOrDefault();

                            if (tempValue.Type_of_doc == "R")
                            {

                                //Debug.WriteLine("low is correct:" + Low);
                                if (ApplicationTemp.documents_Id == null)
                                {
                                    ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                    _conApp.Application.Update(ApplicationTemp);
                                    _conApp.SaveChanges();
                                }


                                if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                                {

                                    if (ApplicationVal != null)
                                    {
                                        if (ApplicationVal.documents_Id == null)
                                        {
                                            ApplicationDB app = new ApplicationDB()
                                            {
                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                ApplicationTermId = ApplicationTemp.ApplicationTermId,
                                                comment = ApplicationTemp.comment,
                                                documents_Id = tempValue.DocumentsId

                                            };

                                            _conApp.Application.Add(app);
                                            _conApp.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                        _conApp.Application.Update(ApplicationTemp);
                                        _conApp.SaveChanges();
                                    }

                                }
                            }

                            if (tempValue.Type_of_doc == "I")
                            {

                                //Debug.WriteLine("low is correct:" + Low);
                                if (ApplicationTemp.documents_Id == null)
                                {
                                    ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                    _conApp.Application.Update(ApplicationTemp);
                                    _conApp.SaveChanges();
                                }


                                if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                                {


                                    if (ApplicationVal != null)
                                    {
                                        if (ApplicationVal.documents_Id == null)
                                        {
                                            ApplicationDB app = new ApplicationDB()
                                            {
                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                                ApplicationTermId = ApplicationTemp.ApplicationTermId,
                                                comment = ApplicationTemp.comment,
                                                documents_Id = tempValue.DocumentsId

                                            };

                                            _conApp.Application.Add(app);
                                            _conApp.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                        _conApp.Application.Update(ApplicationTemp);
                                        _conApp.SaveChanges();
                                    }

                                }
                            }

                        }
                    }

                }
            }
        }
    }
}
