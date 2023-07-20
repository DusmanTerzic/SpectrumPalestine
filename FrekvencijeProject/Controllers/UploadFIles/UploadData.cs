using FrekvencijeProject.JSON.Allocations;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.AllocationTerms;
using FrekvencijeProject.Models.Application;
using FrekvencijeProject.Models.Document;
using FrekvencijeProject.Models.Queries;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers.UploadFIles
{
    public class UploadData
    {
        private readonly ImportTempTableContext _conImport;
        private readonly AllocationDBContext _conAll;
        private readonly ApplicationDBContext _conApp;
        private readonly Tracking_tracing_data_acqDBContext _conTrack;
        private IConfiguration _conf;

        public UploadData(ImportTempTableContext conImport, AllocationDBContext conAll, ApplicationDBContext conApp, Tracking_tracing_data_acqDBContext conTrack, IConfiguration conf)
        {
            _conImport = conImport;
            _conAll = conAll;
            _conApp = conApp;
            _conTrack = conTrack;
            _conf = conf;
        }

        public void ReadDataItu()
        {
            var entryPoint = (from ep in _conImport.ImportTempTable

                              select new
                              {
                                  ep.ntfa_id,
                                  ep.lower_freq,
                                  ep.higher_freq,
                                  ep.itu_reg,
                                  ep.itu_reg_freq
                              }

                            ).ToList();
            List<string> lowerList = new List<string>();
            List<string> higherList = new List<string>();
            var RootAllocation = (from ep in _conAll.RootAllocationDB
                                  where ep.regionCode == "ITU"
                                  select new
                                  {
                                      ep.RootAllocationDBId
                                  }

                           );

            foreach (var val in entryPoint)
            {
                //parsing the lower freq and higher freq. into bytes
                string splitLower = "";
                long resultLower = 0;

                string splitHigher = "";
                long resultHigher = 0;
                if (!lowerList.Contains(val.lower_freq))
                {
                    if (val.lower_freq.EndsWith("GHz"))
                    {
                        var result = val.lower_freq.Split(" GHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000000);
                        //Debug.WriteLine("GHz radi:" + splitLower+"=="+resultLower);
                    }
                    else if (val.lower_freq.EndsWith("MHz"))
                    {
                        var result = val.lower_freq.Split(" MHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000);
                        //Debug.WriteLine("MHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("kHz"))
                    {
                        var result = val.lower_freq.Split(" kHz");
                        splitLower = result[0];
                        //Debug.WriteLine("test:" + splitLower+":");
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }

                        resultLower = (long)(tempLower * 1000);
                        //Debug.WriteLine("kHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("Hz"))
                    {
                        var result = val.lower_freq.Split(" Hz");
                        splitLower = result[0];
                        long tempLower = long.Parse(splitLower);
                        resultLower = tempLower;
                        //Debug.WriteLine("radi:" + splitLower + "==" + resultLower);
                    }
                    lowerList.Add(val.lower_freq);
                }

                if (!higherList.Contains(val.higher_freq))
                {
                    if (val.higher_freq.EndsWith("GHz"))
                    {
                        var result = val.higher_freq.Split(" GHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000000);
                        //Debug.WriteLine("GHz works:" + splitHigher + "=="+ resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("MHz"))
                    {
                        var result = val.higher_freq.Split(" MHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000);
                        //Debug.WriteLine("MHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("kHz"))
                    {
                        var result = val.higher_freq.Split(" kHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }

                        resultHigher = (long)(tempHigher * 1000);
                        //Debug.WriteLine("kHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("Hz"))
                    {
                        var result = val.higher_freq.Split(" Hz");
                        splitHigher = result[0];
                        long tempHigher = long.Parse(splitHigher);
                        resultHigher = tempHigher;
                        //Debug.WriteLine("works:" + splitHigher + "==" + resultHigher);
                    }
                    higherList.Add(val.higher_freq);

                    var AllocationRangeDb = (from allVal in _conAll.AllocationRangeDb
                                             where allVal.low == resultLower && allVal.high == resultHigher
                                             && allVal.RootAllocationDBId == RootAllocation.FirstOrDefault().RootAllocationDBId
                                             select new
                                             {
                                                 allVal.AllocationRangeId
                                             }
                          ).FirstOrDefault();
                    int value = 0;
                    if (AllocationRangeDb == null)
                    {
                        AllocationRangeDb tempAllRange = new AllocationRangeDb();
                        tempAllRange.low = resultLower;
                        tempAllRange.high = resultHigher;
                        tempAllRange.RootAllocationDBId = RootAllocation.FirstOrDefault().RootAllocationDBId;
                        tempAllRange.LowView = val.lower_freq;
                        tempAllRange.HighView = val.higher_freq;

                        _conAll.AllocationRangeDb.Add(tempAllRange);
                        _conAll.SaveChanges();
                        var BestproductSale = _conAll.AllocationRangeDb.ToList()
                        .GroupBy(x => x.AllocationRangeId)
                        .Select(grp => new
                        {
                            Id = grp.ToList().FirstOrDefault().AllocationRangeId
                        })
                        .OrderByDescending(x => x.Id).ToList();
                        value = BestproductSale[0].Id;
                    }
                    else
                    {
                        value = AllocationRangeDb.AllocationRangeId;
                    }

                    //int value = int.Parse(_conAll.AllocationRangeDb.Select(p => p.AllocationRangeId).LastOrDefault().ToString());

                    //read allocation exclude values what is included in allocation and they are not footnotes
                    bool isprimary = false;
                    if (val.itu_reg.Contains("("))
                    {
                        //Debug.WriteLine("var:" + val.itu_reg); 
                        if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (20 KHZ)"))
                        {
                            string standard = val.itu_reg;
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();

                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }



                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (20 kHz)"))
                        {
                            string standard = val.itu_reg;
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }

                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (2 500 KHZ)"))
                        {
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (2 500 kHz)"))
                        {
                            string standard = val.itu_reg;
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }


                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (5 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq 5 000 KHZ:");

                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (5 000 kHz)"))
                        {
                            string standard = val.itu_reg;
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }


                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (10 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (10 000 KHZ):");
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;
                            //var queryTerm
                            //        = (from all in _conAll.AllocationTermDb
                            //           where all.name == standard
                            //           select new
                            //           {
                            //               all.AllocationTermId

                            //           }
                            //     ).FirstOrDefault();


                            ////inserting records in database
                            //AllocationDb tempAll = new AllocationDb();
                            //tempAll.AllocationTermId = queryTerm.AllocationTermId;
                            //tempAll.primary = isprimary;
                            //tempAll.AllocationRangeId = value;
                            //_conAll.AllocationDb.Add(tempAll);
                            //_conAll.SaveChanges();
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (10 000 KHZ)"))
                        {
                            string standard = val.itu_reg;
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (15 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (10 000 KHZ):");
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();



                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (15 000 kHz)"))
                        {
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }



                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (20 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (20 000 KHZ):");
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;


                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (20 000 kHz)"))
                        {
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }

                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (25 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (25 000 KHZ):");
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;


                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                }
                                //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal (25 000 kHz)"))
                        {
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;


                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                }
                                //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                            }
                        }
                        else if (val.itu_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL-SATELLITE (400.1 MHZ)"))
                        {
                            //Debug.WriteLine("qqqq (400.1 MHZ):");
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }



                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.itu_reg.Equals("Standard frequency and time signal-satellite (400.1 MHz)"))
                        {
                            string standard = val.itu_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.itu_reg_freq != null)
                            {
                                if (val.itu_reg_freq != "")
                                {
                                    if (val.itu_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }

                        else
                        {
                            //read allocation thay are included footnotes in  allocation


                            //split current string using ,
                            var ArrayOfAllocations = val.itu_reg.Split(", ");
                            string NewValueAll = "";

                            //interate over the array of strings.
                            foreach (string tempAll in ArrayOfAllocations)
                            {


                                //this regex used for take out the footnotes.
                                if (Regex.IsMatch(tempAll, @"^\d.*"))
                                {
                                    if (tempAll[tempAll.Length - 1] == ')')
                                    {
                                        string cutAll = tempAll.Substring(0, tempAll.Length - 1);
                                        //Debug.WriteLine("entered cut:" + cutAll+"==="+val.itu_reg+"___"+ NewValueAll);
                                        InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                        iAE.InsertNewOne(NewValueAll, value);

                                        iAE.InsertFootnoteAllocation2(NewValueAll, value, cutAll);


                                        if (val.itu_reg_freq != null)
                                        {
                                            if (val.itu_reg_freq != "")
                                            {
                                                if (val.itu_reg_freq.Contains(","))
                                                {
                                                    var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                                    foreach (var tempItuReg in itu_reg_freqArray)
                                                    {
                                                        InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                        iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewValueAll);
                                                    }
                                                }

                                            }
                                        }

                                    }
                                    else
                                    {
                                        //Debug.WriteLine("entered:" + tempAll+"==="+val.itu_reg+"////"+ NewValueAll);
                                        InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                        iAE.InsertNewOne(NewValueAll, value);

                                        iAE.InsertFootnoteAllocation2(NewValueAll, value, tempAll);


                                        if (val.itu_reg_freq != null)
                                        {
                                            if (val.itu_reg_freq != "")
                                            {
                                                if (val.itu_reg_freq.Contains(","))
                                                {
                                                    var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                                    foreach (var tempItuReg in itu_reg_freqArray)
                                                    {
                                                        InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                        iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewValueAll);
                                                    }
                                                }

                                            }
                                        }
                                    }

                                    continue;
                                }
                                else
                                {
                                    if (Regex.IsMatch(tempAll, @"(?<=\()[0-9].+"))
                                    {
                                        var match = Regex.Match(tempAll, @"(?<=\()[0-9].+");
                                        string OriginalTempAll = tempAll;
                                        //Debug.WriteLine("match:" + tempAll+"=="+match.Index);
                                        string All = "";
                                        if (char.IsWhiteSpace(tempAll[match.Index - 1]))
                                        {
                                            All = tempAll.Substring(0, match.Index - 2);
                                        }
                                        else
                                        {
                                            All = tempAll.Substring(0, match.Index - 1);
                                        }
                                        NewValueAll = All;
                                        //Debug.WriteLine("match:" + All+"=="+ OriginalTempAll);
                                        InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                        iAE.InsertNewOne(All, value);

                                        if (val.itu_reg_freq != null)
                                        {
                                            if (val.itu_reg_freq != "")
                                            {
                                                if (val.itu_reg_freq.Contains(","))
                                                {
                                                    var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                                    foreach (var tempItuReg in itu_reg_freqArray)
                                                    {
                                                        //ovdje baca gresku
                                                        InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                        iAe.InsertFootnoteBandAllocation(value, tempItuReg, All);
                                                    }
                                                }

                                            }
                                        }

                                        var matchEnd = Regex.Match(OriginalTempAll, @"\)");
                                        //Debug.WriteLine("value:" + matchEnd.Index);
                                        string Footnotes = "";
                                        if (matchEnd.Index == 0)
                                        {
                                            int startIndex = match.Index;
                                            int endIndex = OriginalTempAll.Length - 1;
                                            //Debug.WriteLine("lenght:" + startIndex + "==" + endIndex);
                                            //Debug.WriteLine("ffff" + OriginalTempAll);
                                            Footnotes = OriginalTempAll.Substring(startIndex);
                                            iAE.InsertFootnoteAllocation(All, value, Footnotes);
                                            // Debug.WriteLine("footnote without parentheses:" + Footnotes);
                                        }
                                        else
                                        {

                                            string FootnotesParatheses = OriginalTempAll.Substring(match.Index);
                                            //Debug.WriteLine("ppppp:" + FootnotesParatheses);
                                            if (FootnotesParatheses[FootnotesParatheses.Length - 1] == ')')
                                            {
                                                string Footnote = FootnotesParatheses.Substring(0, FootnotesParatheses.Length - 1);
                                                if (char.IsWhiteSpace(Footnote[Footnote.Length - 1]))
                                                {
                                                    Footnote = Footnote.Substring(0, Footnote.Length - 1);
                                                    iAE.InsertFootnoteAllocation(All, value, Footnote);
                                                    //Debug.WriteLine("footnote cut:" + Footnote);
                                                }
                                                else
                                                {
                                                    //Debug.WriteLine("footnote:" + Footnote);
                                                    iAE.InsertFootnoteAllocation(All, value, Footnote);
                                                }
                                            }
                                            else
                                            {
                                                if (FootnotesParatheses.Contains(") "))
                                                {
                                                    string newFoot = FootnotesParatheses.Substring(0, FootnotesParatheses.Length - 2);
                                                    //Debug.WriteLine("footnote empty:" + newFoot);
                                                    iAE.InsertFootnoteAllocation(All, value, newFoot);
                                                }
                                                else
                                                {
                                                    //Debug.WriteLine("footnote ww:" + FootnotesParatheses);
                                                    iAE.InsertFootnoteAllocation(All, value, FootnotesParatheses);
                                                }

                                            }

                                        }

                                    }
                                    else
                                    {
                                        //Debug.WriteLine("RRRR:" + tempAll);
                                        if (tempAll != "")
                                        {
                                            InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                            iAE.InsertNewOne(tempAll, value);

                                            if (val.itu_reg_freq != null)
                                            {
                                                if (val.itu_reg_freq != "")
                                                {
                                                    if (val.itu_reg_freq.Contains(","))
                                                    {
                                                        var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                                        foreach (var tempItuReg in itu_reg_freqArray)
                                                        {
                                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, tempAll);
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
                    else
                    {
                        //this is consider for allocation which does not contains Parentheses.
                        string valueITu = val.itu_reg;
                        var arrayOfValues = valueITu.Split(",");
                        foreach (var temp in arrayOfValues)
                        {
                            if (temp != "")
                            {

                                string tempVa = "";
                                //Debug.WriteLine("www:" + temp);
                                if (char.IsWhiteSpace(temp[0]))
                                {
                                    //if(temp == " MARITIME MOBILE")
                                    //     Debug.WriteLine("wwww:");
                                    int length = temp.Length;
                                    //Debug.WriteLine("RRRR:" + length);
                                    string IsAnotherWhite = temp.Substring(1, length - 1);
                                    if (char.IsWhiteSpace(IsAnotherWhite[0]))
                                    {
                                        length = IsAnotherWhite.Length;
                                        tempVa = IsAnotherWhite.Substring(1, length - 1);
                                    }
                                    else
                                    {
                                        tempVa = IsAnotherWhite;
                                    }


                                    //Debug.WriteLine("www ttt:" + tempVa);
                                }
                                else
                                {
                                    tempVa = temp;
                                }
                                if (tempVa.StartsWith("MOBILE"))
                                {
                                    isprimary = true;
                                }
                                else
                                {
                                    if (IsAllUpper(tempVa))
                                    {
                                        isprimary = true;
                                    }
                                    else
                                    {
                                        isprimary = false;
                                    }
                                }

                                //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                                var queryTerm
                                    = (from all in _conAll.AllocationTermDb

                                       select new TempAllocationTerm
                                       {
                                           AllocationTermId = all.AllocationTermId,
                                           name = all.name,
                                           _PRIMARY = all._PRIMARY

                                       }
                                 ).ToList();
                                TempAllocationTerm NewQueryTerm = null;
                                if (isprimary)
                                {
                                    NewQueryTerm = queryTerm.Where(a => a.name == tempVa && a._PRIMARY == true).SingleOrDefault();
                                }
                                else
                                {
                                    NewQueryTerm = queryTerm.Where(a => a.name == tempVa && a._PRIMARY == false).SingleOrDefault();
                                }

                                var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                                if (AllocationDb == null)
                                {


                                    //inserting records in database
                                    AllocationDb tempAll = new AllocationDb();
                                    tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                    tempAll.primary = NewQueryTerm._PRIMARY;
                                    tempAll.AllocationRangeId = value;
                                    _conAll.AllocationDb.Add(tempAll);
                                    _conAll.SaveChanges();
                                }
                                //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                //this line code is used to add band for allocations
                                if (val.itu_reg_freq != null)
                                {
                                    if (val.itu_reg_freq != "")
                                    {
                                        if (val.itu_reg_freq.Contains(","))
                                        {
                                            var itu_reg_freqArray = val.itu_reg_freq.Split(",");

                                            foreach (var tempItuReg in itu_reg_freqArray)
                                            {
                                                InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
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

        public void ReadDataPalestine()
        {
            var entryPoint = (from ep in _conImport.ImportTempTable

                              select new
                              {
                                  ep.ntfa_id,
                                  ep.lower_freq,
                                  ep.higher_freq,
                                  ep.main_reg,
                                  ep.main_reg_freq
                              }

                            ).ToList();
            List<string> lowerList = new List<string>();
            List<string> higherList = new List<string>();
            var RootAllocation = (from ep in _conAll.RootAllocationDB
                                  where ep.regionCode == "PSE"
                                  select new
                                  {
                                      ep.RootAllocationDBId
                                  }

                           );

            foreach (var val in entryPoint)
            {
                //parsing the lower freq and higher freq. into bytes
                string splitLower = "";
                long resultLower = 0;

                string splitHigher = "";
                long resultHigher = 0;
                if (!lowerList.Contains(val.lower_freq))
                {
                    if (val.lower_freq.EndsWith("GHz"))
                    {
                        var result = val.lower_freq.Split(" GHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000000);
                        //Debug.WriteLine("GHz radi:" + splitLower+"=="+resultLower);
                    }
                    else if (val.lower_freq.EndsWith("MHz"))
                    {
                        var result = val.lower_freq.Split(" MHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000);
                        //Debug.WriteLine("MHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("kHz"))
                    {
                        var result = val.lower_freq.Split(" kHz");
                        splitLower = result[0];
                        //Debug.WriteLine("test:" + splitLower+":");
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }

                        resultLower = (long)(tempLower * 1000);
                        //Debug.WriteLine("kHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("Hz"))
                    {
                        var result = val.lower_freq.Split(" Hz");
                        splitLower = result[0];
                        long tempLower = long.Parse(splitLower);
                        resultLower = tempLower;
                        //Debug.WriteLine("radi:" + splitLower + "==" + resultLower);
                    }
                    lowerList.Add(val.lower_freq);
                }

                if (!higherList.Contains(val.higher_freq))
                {
                    if (val.higher_freq.EndsWith("GHz"))
                    {
                        var result = val.higher_freq.Split(" GHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000000);
                        //Debug.WriteLine("GHz works:" + splitHigher + "=="+ resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("MHz"))
                    {
                        var result = val.higher_freq.Split(" MHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000);
                        //Debug.WriteLine("MHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("kHz"))
                    {
                        var result = val.higher_freq.Split(" kHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }

                        resultHigher = (long)(tempHigher * 1000);
                        //Debug.WriteLine("kHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("Hz"))
                    {
                        var result = val.higher_freq.Split(" Hz");
                        splitHigher = result[0];
                        long tempHigher = long.Parse(splitHigher);
                        resultHigher = tempHigher;
                        //Debug.WriteLine("works:" + splitHigher + "==" + resultHigher);
                    }
                    higherList.Add(val.higher_freq);
                    var AllocationRangeDb = (from allVal in _conAll.AllocationRangeDb
                                             where allVal.low == resultLower && allVal.high == resultHigher
                                             && allVal.RootAllocationDBId == RootAllocation.FirstOrDefault().RootAllocationDBId
                                             select new
                                             {
                                                 allVal.AllocationRangeId
                                             }
                          ).FirstOrDefault();
                    if (AllocationRangeDb == null)
                    {
                        AllocationRangeDb tempAllRange = new AllocationRangeDb();
                        tempAllRange.low = resultLower;
                        tempAllRange.high = resultHigher;
                        tempAllRange.RootAllocationDBId = RootAllocation.FirstOrDefault().RootAllocationDBId;
                        tempAllRange.LowView = val.lower_freq;
                        tempAllRange.HighView = val.higher_freq;

                        _conAll.AllocationRangeDb.Add(tempAllRange);
                        _conAll.SaveChanges();
                    }

                    //int value = int.Parse(_conAll.AllocationRangeDb.Select(p => p.AllocationRangeId).LastOrDefault().ToString());

                    var BestproductSale = _conAll.AllocationRangeDb.ToList()
          .GroupBy(x => x.AllocationRangeId)
          .Select(grp => new
          {
              Id = grp.ToList().FirstOrDefault().AllocationRangeId
          })
          .OrderByDescending(x => x.Id).ToList();
                    int value = BestproductSale[0].Id;
                    //read allocation exclude values what is included in allocation and they are not footnotes
                    bool isprimary = false;
                    if (val.main_reg.Contains("("))
                    {
                        //Debug.WriteLine("var:" + val.main_reg); 
                        if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (20 KHZ)"))
                        {
                            string standard = val.main_reg;
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }



                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (20 kHz)"))
                        {
                            string standard = val.main_reg;
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }

                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (2 500 KHZ)"))
                        {
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (2 500 kHz)"))
                        {
                            string standard = val.main_reg;
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }


                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (5 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq 5 000 KHZ:");

                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (5 000 kHz)"))
                        {
                            string standard = val.main_reg;
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }


                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (10 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (10 000 KHZ):");
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (10 000 KHZ)"))
                        {
                            string standard = val.main_reg;
                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }


                            //Debug.WriteLine("qqqq:"+standard);
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (15 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (10 000 KHZ):");
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (15 000 kHz)"))
                        {
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }



                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (20 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (20 000 KHZ):");
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;


                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (20 000 kHz)"))
                        {
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();

                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            //handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }
                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }


                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL (25 000 KHZ)"))
                        {
                            //Debug.WriteLine("qqqq (25 000 KHZ):");
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;


                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                }
                                //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal (25 000 kHz)"))
                        {
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;

                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;


                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                }
                                //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                            }
                        }
                        else if (val.main_reg.Equals("STANDARD FREQUENCY AND TIME SIGNAL-SATELLITE (400.1 MHZ)"))
                        {
                            //Debug.WriteLine("qqqq (400.1 MHZ):");
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }
                        else if (val.main_reg.Equals("Standard frequency and time signal-satellite (400.1 MHz)"))
                        {
                            string standard = val.main_reg;
                            //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                            isprimary = true;


                            if (IsAllUpper(standard))
                            {
                                isprimary = true;
                            }
                            else
                            {
                                isprimary = false;
                            }

                            //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                            var queryTerm
                                = (from all in _conAll.AllocationTermDb

                                   select new TempAllocationTerm
                                   {
                                       AllocationTermId = all.AllocationTermId,
                                       name = all.name,
                                       _PRIMARY = all._PRIMARY

                                   }
                             ).ToList();
                            TempAllocationTerm NewQueryTerm = null;

                            NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == false).SingleOrDefault();


                            var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                   select new
                                   {
                                       all.AllocationId
                                   }
                             ).SingleOrDefault();
                            // handling double records for allocation.
                            if (AllocationDb == null)
                            {
                                //inserting records in database
                                AllocationDb tempAll = new AllocationDb();
                                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                tempAll.primary = NewQueryTerm._PRIMARY;
                                tempAll.AllocationRangeId = value;
                                _conAll.AllocationDb.Add(tempAll);
                                _conAll.SaveChanges();
                            }

                            //this line code is used to add band for allocations
                            if (val.main_reg_freq != null)
                            {
                                if (val.main_reg_freq != "")
                                {
                                    if (val.main_reg_freq.Contains(","))
                                    {
                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                        foreach (var tempItuReg in itu_reg_freqArray)
                                        {
                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                        }
                                    }
                                    //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                }
                            }
                        }

                        else
                        {
                            //read allocation thay are included footnotes in  allocation
                            //Debug.WriteLine("testing length:" + val.main_reg.Length);
                            // Debug.WriteLine("testing:" + val.main_reg);
                            //split current string using ,
                            var ArrayOfAllocations = val.main_reg.Split(", ");

                            string NewValueAll = "";

                            //interate over the array of strings.
                            foreach (string tempAll in ArrayOfAllocations)
                            {
                                //if allocation contains this value excute this condition.
                                if (tempAll.Equals("STANDARD FREQUENCY AND TIME SIGNAL-SATELLITE (400.1 MHZ)"))
                                {
                                    string standard = tempAll;
                                    //Debug.WriteLine("qqqq 2 500 KHZ:" + standard);
                                    isprimary = true;

                                    if (IsAllUpper(standard))
                                    {
                                        isprimary = true;
                                    }
                                    else
                                    {
                                        isprimary = false;
                                    }

                                    //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                                    var queryTerm
                                        = (from all in _conAll.AllocationTermDb

                                           select new TempAllocationTerm
                                           {
                                               AllocationTermId = all.AllocationTermId,
                                               name = all.name,
                                               _PRIMARY = all._PRIMARY

                                           }
                                     ).ToList();
                                    TempAllocationTerm NewQueryTerm = null;

                                    NewQueryTerm = queryTerm.Where(a => a.name == standard && a._PRIMARY == true).SingleOrDefault();


                                    var AllocationDb
                                        = (from all in _conAll.AllocationDb
                                           where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId
                                           select new
                                           {
                                               all.AllocationId
                                           }
                                            ).SingleOrDefault();
                                    // handling double records for allocation.
                                    if (AllocationDb == null)
                                    {
                                        //inserting records in database
                                        AllocationDb tempAllDb = new AllocationDb();
                                        tempAllDb.AllocationTermId = NewQueryTerm.AllocationTermId;
                                        tempAllDb.primary = NewQueryTerm._PRIMARY;
                                        tempAllDb.AllocationRangeId = value;
                                        _conAll.AllocationDb.Add(tempAllDb);
                                        _conAll.SaveChanges();
                                    }

                                    //this line code is used to add band for allocations
                                    if (val.main_reg_freq != null)
                                    {
                                        if (val.main_reg_freq != "")
                                        {
                                            if (val.main_reg_freq.Contains(","))
                                            {
                                                var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                foreach (var tempItuReg in itu_reg_freqArray)
                                                {
                                                    InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                    iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
                                                }
                                            }
                                            //Debug.WriteLine("i'm here rrr:" + val.itu_reg_freq);
                                        }
                                    }
                                }

                                else if (Regex.IsMatch(tempAll, @"(?<=\()PS.+"))
                                {
                                    //Debug.WriteLine("this type of condition:" + tempAll);
                                    int index = tempAll.IndexOf("(P");
                                    //var ArrayTempAll = tempAll.Split(index);
                                    //var matchEnd = Regex.Match(OriginalTempAll, @"\)");
                                    string FirstTempAll = tempAll.Substring(0, index);
                                    string SecondTempAll = tempAll.Substring(index + 1, tempAll.Length - 2 - index);

                                    if (char.IsWhiteSpace(FirstTempAll[index - 1]))
                                    {
                                        string subTempAll = FirstTempAll.Substring(0, FirstTempAll.Length - 1);
                                        //Debug.WriteLine("hello cut:" + subTempAll + "==="+ SecondTempAll);
                                        InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                        iAE.InsertNewOne(subTempAll, value);
                                        iAE.InsertFootnoteAllocation(subTempAll, value, SecondTempAll);
                                    }
                                    else
                                    {
                                        //string subTempAll = FirstTempAll.Substring(0, index);
                                        InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                        //Debug.WriteLine("hello:" + FirstTempAll+ "==="+ SecondTempAll);
                                        iAE.InsertNewOne(FirstTempAll, value);
                                        iAE.InsertFootnoteAllocation(FirstTempAll, value, SecondTempAll);
                                    }

                                }
                                else
                                {
                                    //Debug.WriteLine("else type of condition:" + tempAll);
                                    //this regex used for take out the footnotes.
                                    if (Regex.IsMatch(tempAll, @"^\d.*"))
                                    {
                                        if (tempAll[tempAll.Length - 1] == ')')
                                        {
                                            string cutAll = tempAll.Substring(0, tempAll.Length - 1);
                                            //Debug.WriteLine("entered cut:" + cutAll+"==="+val.main_reg+"___"+ NewValueAll);
                                            InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                            iAE.InsertNewOne(NewValueAll, value);

                                            iAE.InsertFootnoteAllocation2(NewValueAll, value, cutAll);


                                            if (val.main_reg_freq != null)
                                            {
                                                if (val.main_reg_freq != "")
                                                {
                                                    if (val.main_reg_freq.Contains(","))
                                                    {
                                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                        foreach (var tempItuReg in itu_reg_freqArray)
                                                        {
                                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewValueAll);
                                                        }
                                                    }

                                                }
                                            }

                                        }
                                        else
                                        {
                                            //Debug.WriteLine("entered:" + tempAll+"==="+val.main_reg+"////"+ NewValueAll);
                                            InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                            iAE.InsertNewOne(NewValueAll, value);

                                            iAE.InsertFootnoteAllocation2(NewValueAll, value, tempAll);


                                            if (val.main_reg_freq != null)
                                            {
                                                if (val.main_reg_freq != "")
                                                {
                                                    if (val.main_reg_freq.Contains(","))
                                                    {
                                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                        foreach (var tempItuReg in itu_reg_freqArray)
                                                        {
                                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewValueAll);
                                                        }
                                                    }

                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (Regex.IsMatch(tempAll, @"(?<=\()[0-9].+"))
                                        {
                                            var match = Regex.Match(tempAll, @"(?<=\()[0-9].+");
                                            string OriginalTempAll = tempAll;
                                            //Debug.WriteLine("match:" + tempAll+"=="+match.Index);
                                            string All = "";
                                            if (char.IsWhiteSpace(tempAll[match.Index - 1]))
                                            {
                                                All = tempAll.Substring(0, match.Index - 2);
                                            }
                                            else
                                            {
                                                All = tempAll.Substring(0, match.Index - 1);
                                            }
                                            NewValueAll = All;
                                            //Debug.WriteLine("match:" + All+"=="+ OriginalTempAll);
                                            InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                            iAE.InsertNewOne(All, value);

                                            if (val.main_reg_freq != null)
                                            {
                                                if (val.main_reg_freq != "")
                                                {
                                                    if (val.main_reg_freq.Contains(","))
                                                    {
                                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                        foreach (var tempItuReg in itu_reg_freqArray)
                                                        {
                                                            //ovdje baca gresku
                                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, All);
                                                        }
                                                    }

                                                }
                                            }

                                            var matchEnd = Regex.Match(OriginalTempAll, @"\)");
                                            //Debug.WriteLine("value:" + matchEnd.Index);
                                            string Footnotes = "";
                                            if (matchEnd.Index == 0)
                                            {
                                                int startIndex = match.Index;
                                                int endIndex = OriginalTempAll.Length - 1;
                                                //Debug.WriteLine("lenght:" + startIndex + "==" + endIndex);
                                                //Debug.WriteLine("ffff" + OriginalTempAll);
                                                Footnotes = OriginalTempAll.Substring(startIndex);
                                                iAE.InsertFootnoteAllocation(All, value, Footnotes);
                                                // Debug.WriteLine("footnote without parentheses:" + Footnotes);
                                            }
                                            else
                                            {

                                                string FootnotesParatheses = OriginalTempAll.Substring(match.Index);
                                                //Debug.WriteLine("ppppp:" + FootnotesParatheses);
                                                if (FootnotesParatheses[FootnotesParatheses.Length - 1] == ')')
                                                {
                                                    string Footnote = FootnotesParatheses.Substring(0, FootnotesParatheses.Length - 1);
                                                    if (char.IsWhiteSpace(Footnote[Footnote.Length - 1]))
                                                    {
                                                        Footnote = Footnote.Substring(0, Footnote.Length - 1);
                                                        iAE.InsertFootnoteAllocation(All, value, Footnote);
                                                        //Debug.WriteLine("footnote cut:" + Footnote);
                                                    }
                                                    else
                                                    {
                                                        //Debug.WriteLine("footnote:" + Footnote);
                                                        iAE.InsertFootnoteAllocation(All, value, Footnote);
                                                    }
                                                }
                                                else
                                                {
                                                    if (FootnotesParatheses.Contains(") "))
                                                    {
                                                        string newFoot = FootnotesParatheses.Substring(0, FootnotesParatheses.Length - 2);
                                                        //Debug.WriteLine("footnote empty:" + newFoot);
                                                        iAE.InsertFootnoteAllocation(All, value, newFoot);
                                                    }
                                                    else
                                                    {
                                                        //Debug.WriteLine("footnote ww:" + FootnotesParatheses);
                                                        iAE.InsertFootnoteAllocation(All, value, FootnotesParatheses);
                                                    }

                                                }

                                            }

                                        }
                                        else if (tempAll.Contains("(PSE"))
                                        {
                                            int index = tempAll.IndexOf("(");
                                            string OriginalTempAll = tempAll;

                                            string All = "";
                                            //BROADCASTING (
                                            //13
                                            if (char.IsWhiteSpace(tempAll[index - 1]))
                                            {
                                                All = tempAll.Substring(0, index - 1);
                                                //Debug.WriteLine("match white:" + All + "==");
                                            }
                                            else
                                            {
                                                All = tempAll.Substring(0, index);
                                                //Debug.WriteLine("match:" + All + "==" + index);
                                            }
                                            NewValueAll = All;
                                            //Debug.WriteLine("match:" + All + "==" + OriginalTempAll);
                                            InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                            iAE.InsertNewOne(All, value);

                                            if (val.main_reg_freq != null)
                                            {
                                                if (val.main_reg_freq != "")
                                                {
                                                    if (val.main_reg_freq.Contains(","))
                                                    {
                                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                        foreach (var tempItuReg in itu_reg_freqArray)
                                                        {
                                                            //ovdje baca gresku
                                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, All);
                                                        }
                                                    }

                                                }
                                            }

                                            var matchEnd = Regex.Match(OriginalTempAll, @"\)");
                                            //Debug.WriteLine("value:" + matchEnd.Index);
                                            string Footnotes = "";
                                            if (matchEnd.Index == 0)
                                            {
                                                int startIndex = index + 1;
                                                int endIndex = OriginalTempAll.Length - 1;
                                                //Debug.WriteLine("lenght:" + startIndex + "==" + endIndex);
                                                //Debug.WriteLine("ffff" + OriginalTempAll);
                                                Footnotes = OriginalTempAll.Substring(startIndex);
                                                iAE.InsertFootnoteAllocation(All, value, Footnotes);
                                                // Debug.WriteLine("footnote without parentheses:" + Footnotes);
                                            }
                                            else
                                            {
                                                int startindex = index + 1;
                                                string FootnotesParatheses = OriginalTempAll.Substring(startindex);
                                                //Debug.WriteLine("ppppp:" + FootnotesParatheses);
                                                if (FootnotesParatheses[FootnotesParatheses.Length - 1] == ')')
                                                {
                                                    string Footnote = FootnotesParatheses.Substring(0, FootnotesParatheses.Length - 1);
                                                    if (char.IsWhiteSpace(Footnote[Footnote.Length - 1]))
                                                    {
                                                        Footnote = Footnote.Substring(0, Footnote.Length - 1);
                                                        iAE.InsertFootnoteAllocation(All, value, Footnote);
                                                        //Debug.WriteLine("footnote cut:" + Footnote);
                                                    }
                                                    else
                                                    {
                                                        //Debug.WriteLine("footnote:" + Footnote);
                                                        iAE.InsertFootnoteAllocation(All, value, Footnote);
                                                    }
                                                }
                                                else
                                                {
                                                    if (FootnotesParatheses.Contains(") "))
                                                    {
                                                        string newFoot = FootnotesParatheses.Substring(0, FootnotesParatheses.Length - 2);
                                                        //Debug.WriteLine("footnote empty:" + newFoot);
                                                        iAE.InsertFootnoteAllocation(All, value, newFoot);
                                                    }
                                                    else
                                                    {
                                                        //Debug.WriteLine("footnote ww:" + FootnotesParatheses);
                                                        iAE.InsertFootnoteAllocation(All, value, FootnotesParatheses);
                                                    }

                                                }

                                            }



                                        }
                                        else if (tempAll.Contains("PSE"))
                                        {
                                            InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                            if (tempAll[tempAll.Length - 1] == ')')
                                            {

                                                string tt = tempAll.Substring(0, tempAll.Length - 1);
                                                iAE.InsertFootnoteAllocation(NewValueAll, value, tt);
                                            }
                                            else
                                            {
                                                iAE.InsertFootnoteAllocation(NewValueAll, value, tempAll);
                                            }



                                            if (val.main_reg_freq != null)
                                            {
                                                if (val.main_reg_freq != "")
                                                {
                                                    if (val.main_reg_freq.Contains(","))
                                                    {
                                                        var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                        foreach (var tempItuReg in itu_reg_freqArray)
                                                        {
                                                            //ovdje baca gresku
                                                            InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                            iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewValueAll);
                                                        }
                                                    }

                                                }
                                            }

                                        }
                                        else
                                        {
                                            //
                                            if (tempAll != "")
                                            {
                                                //Debug.WriteLine("RRRR:" + tempAll);
                                                InsertAllocationExcel iAE = new InsertAllocationExcel(_conAll);
                                                iAE.InsertNewOne(tempAll, value);

                                                if (val.main_reg_freq != null)
                                                {
                                                    if (val.main_reg_freq != "")
                                                    {
                                                        if (val.main_reg_freq.Contains(","))
                                                        {
                                                            var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                                            foreach (var tempItuReg in itu_reg_freqArray)
                                                            {
                                                                InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                                iAe.InsertFootnoteBandAllocation(value, tempItuReg, tempAll);
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
                    }
                    else
                    {
                        //this is consider for allocation which does not contains Parentheses.
                        string valueITu = val.main_reg;
                        var arrayOfValues = valueITu.Split(",");
                        foreach (var temp in arrayOfValues)
                        {
                            if (temp != "")
                            {

                                string tempVa = "";
                                //Debug.WriteLine("www:" + temp);
                                if (char.IsWhiteSpace(temp[0]))
                                {
                                    //if(temp == " MARITIME MOBILE")
                                    //     Debug.WriteLine("wwww:");
                                    int length = temp.Length;
                                    //Debug.WriteLine("RRRR:" + length);
                                    string IsAnotherWhite = temp.Substring(1, length - 1);
                                    if (char.IsWhiteSpace(IsAnotherWhite[0]))
                                    {
                                        length = IsAnotherWhite.Length;
                                        tempVa = IsAnotherWhite.Substring(1, length - 1);
                                    }
                                    else
                                    {
                                        tempVa = IsAnotherWhite;
                                    }


                                    //Debug.WriteLine("www ttt:" + tempVa);
                                }
                                else
                                {
                                    tempVa = temp;
                                }
                                if (tempVa.StartsWith("MOBILE"))
                                {
                                    isprimary = true;
                                }
                                else
                                {
                                    if (IsAllUpper(tempVa))
                                    {
                                        isprimary = true;
                                    }
                                    else
                                    {
                                        isprimary = false;
                                    }
                                }

                                //Debug.WriteLine("value:" + tempVa + "==" + val.higher_freq + "::" + isprimary);

                                var queryTerm
                                    = (from all in _conAll.AllocationTermDb

                                       select new TempAllocationTerm
                                       {
                                           AllocationTermId = all.AllocationTermId,
                                           name = all.name,
                                           _PRIMARY = all._PRIMARY

                                       }
                                 ).ToList();
                                TempAllocationTerm NewQueryTerm = null;
                                if (isprimary)
                                {
                                    NewQueryTerm = queryTerm.Where(a => a.name == tempVa && a._PRIMARY == true).SingleOrDefault();
                                }
                                else
                                {
                                    NewQueryTerm = queryTerm.Where(a => a.name == tempVa && a._PRIMARY == false).SingleOrDefault();
                                }

                                var AllocationDb
                                = (from all in _conAll.AllocationDb
                                   where all.AllocationRangeId == value && all.AllocationTermId == NewQueryTerm.AllocationTermId

                                   select new
                                   {
                                       all.AllocationId

                                   }
                             ).SingleOrDefault();
                                if (AllocationDb == null)
                                {
                                    //inserting records in database
                                    AllocationDb tempAll = new AllocationDb();
                                    tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                                    tempAll.primary = NewQueryTerm._PRIMARY;
                                    tempAll.AllocationRangeId = value;
                                    _conAll.AllocationDb.Add(tempAll);
                                    _conAll.SaveChanges();
                                }
                                //Debug.WriteLine("i'm here rrr:" + val.main_reg);
                                //this line code is used to add band for allocations
                                if (val.main_reg_freq != null)
                                {
                                    if (val.main_reg_freq != "")
                                    {
                                        if (val.main_reg_freq.Contains(","))
                                        {
                                            var itu_reg_freqArray = val.main_reg_freq.Split(",");

                                            foreach (var tempItuReg in itu_reg_freqArray)
                                            {
                                                InsertAllocationExcel iAe = new InsertAllocationExcel(_conAll);
                                                iAe.InsertFootnoteBandAllocation(value, tempItuReg, NewQueryTerm.AllocationTermId);
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

        public void ReadApplicationData()
        {

            var entryPoint = (from ep in _conImport.ImportTempTable

                              select new
                              {
                                  ep.ntfa_id,
                                  ep.lower_freq,
                                  ep.higher_freq,
                                  ep.main_reg,
                                  ep.application,
                                  ep.notes
                              }

                            ).ToList();
            List<string> lowerList = new List<string>();
            List<string> higherList = new List<string>();
            var RootApplication = (from ep in _conApp.RootApplicationDB
                                   where ep.regionCode == "PSE"
                                   select new
                                   {
                                       ep.RootApplicationDBId
                                   }

                           );

            foreach (var val in entryPoint)
            {
                //parsing the lower freq and higher freq. into bytes
                string splitLower = "";
                long resultLower = 0;

                string splitHigher = "";
                long resultHigher = 0;
                if (!lowerList.Contains(val.lower_freq))
                {
                    if (val.lower_freq.EndsWith("GHz"))
                    {
                        var result = val.lower_freq.Split(" GHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000000);
                        //Debug.WriteLine("GHz radi:" + splitLower+"=="+resultLower);
                    }
                    else if (val.lower_freq.EndsWith("MHz"))
                    {
                        var result = val.lower_freq.Split(" MHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000);
                        //Debug.WriteLine("MHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("kHz"))
                    {
                        var result = val.lower_freq.Split(" kHz");
                        splitLower = result[0];
                        //Debug.WriteLine("test:" + splitLower+":");
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }

                        resultLower = (long)(tempLower * 1000);
                        //Debug.WriteLine("kHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("Hz"))
                    {
                        var result = val.lower_freq.Split(" Hz");
                        splitLower = result[0];
                        long tempLower = long.Parse(splitLower);
                        resultLower = tempLower;
                        //Debug.WriteLine("radi:" + splitLower + "==" + resultLower);
                    }
                    lowerList.Add(val.lower_freq);
                }

                if (!higherList.Contains(val.higher_freq))
                {
                    if (val.higher_freq.EndsWith("GHz"))
                    {
                        var result = val.higher_freq.Split(" GHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000000);
                        //Debug.WriteLine("GHz works:" + splitHigher + "=="+ resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("MHz"))
                    {
                        var result = val.higher_freq.Split(" MHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000);
                        //Debug.WriteLine("MHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("kHz"))
                    {
                        var result = val.higher_freq.Split(" kHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }

                        resultHigher = (long)(tempHigher * 1000);
                        //Debug.WriteLine("kHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("Hz"))
                    {
                        var result = val.higher_freq.Split(" Hz");
                        splitHigher = result[0];
                        long tempHigher = long.Parse(splitHigher);
                        resultHigher = tempHigher;
                        //Debug.WriteLine("works:" + splitHigher + "==" + resultHigher);
                    }
                    higherList.Add(val.higher_freq);
                    var ApplicationRangeDb = (from allVal in _conApp.ApplicationRange
                                              where allVal.low == resultLower && allVal.high == resultHigher
                                              && allVal.RootApplicationDBId == RootApplication.FirstOrDefault().RootApplicationDBId
                                              select new
                                              {
                                                  allVal.ApplicationRangeId
                                              }
                          ).FirstOrDefault();
                    int value = 0;
                    if (ApplicationRangeDb == null)
                    {
                        ApplicationRangeDB tempAllRange = new ApplicationRangeDB();
                        tempAllRange.low = resultLower;
                        tempAllRange.high = resultHigher;
                        tempAllRange.RootApplicationDBId = RootApplication.FirstOrDefault().RootApplicationDBId;
                        tempAllRange.LowView = val.lower_freq;
                        tempAllRange.HighView = val.higher_freq;

                        _conApp.ApplicationRange.Add(tempAllRange);
                        _conApp.SaveChanges();
                        var BestproductSale = _conApp.ApplicationRange.ToList()
                           .GroupBy(x => x.ApplicationRangeId)
                           .Select(grp => new
                           {
                               Id = grp.ToList().FirstOrDefault().ApplicationRangeId
                           }).OrderByDescending(x => x.Id).ToList();
                        value = BestproductSale[0].Id;
                    }
                    else
                    {
                        value = ApplicationRangeDb.ApplicationRangeId;
                    }

                    //int value = int.Parse(_conAll.AllocationRangeDb.Select(p => p.AllocationRangeId).LastOrDefault().ToString());
                    //checking the value of application in some case there is special need case to be done manually
                    //i mean on if condition where is in some case dependent on value from allocation.
                    if (val.application != "")
                    {
                        if (val.application == "Active sensors (satellite)")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                                {
                                    //Debug.WriteLine("wwwwwwww:" + val.main_reg + "===" + val.application);
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    var ApplicationTermIdSecond = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 140).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();


                                    ApplicationDB app2 = new ApplicationDB();
                                    app2.ApplicationRangeId = value;
                                    app2.ApplicationTermId = ApplicationTermIdSecond.ApplicationTermsDBId;
                                    app2.comment = "" + val.notes;
                                    _conApp.Application.Add(app2);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();

                                }
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 187).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }

                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("METEOROLOGICAL AIDS"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 75).SingleOrDefault();

                                //Debug.WriteLine("active ttt:" + val.application + "==" + val.main_reg + "::" + value);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;

                        }
                        else if (val.application == "Passive sensors (satellite)")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                //Debug.WriteLine("qqqq:" + val.main_reg + "===" + val.application);
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    var ApplicationTermIdSecond = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 140).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();


                                    ApplicationDB app2 = new ApplicationDB();
                                    app2.ApplicationRangeId = value;
                                    app2.ApplicationTermId = ApplicationTermIdSecond.ApplicationTermsDBId;
                                    app2.comment = "" + val.notes;
                                    _conApp.Application.Add(app2);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();
                                }
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("METEOROLOGICAL AIDS"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 75).SingleOrDefault();

                                //Debug.WriteLine("ttt:" + val.application+"=="+ val.main_reg+"::"+value);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }

                        if (Regex.IsMatch(val.application, Regex.Escape("emergency detection"), RegexOptions.IgnoreCase))
                        {
                            var ValueTrack = (from ev in _conTrack.Tracking_tracing_data_acq
                                              where ev.high_freq == val.higher_freq
                                              select new { ev.id }).SingleOrDefault();

                            if (ValueTrack == null)
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 230).SingleOrDefault();

                                //Debug.WriteLine("rrr" + val.application);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 207).SingleOrDefault();

                                //Debug.WriteLine("wwww" + val.application);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                        }
                        else if (Regex.IsMatch(val.application, Regex.Escape("SRR"), RegexOptions.IgnoreCase))
                        {
                            var ValueTrack = (from ev in _conTrack.TTT
                                              where ev.high_freq == val.higher_freq
                                              select new { ev.id }).SingleOrDefault();

                            if (ValueTrack == null)
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 378).SingleOrDefault();

                                //Debug.WriteLine("rrr" + val.application);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 236).SingleOrDefault();

                                //Debug.WriteLine("wwww" + val.application+"::");
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            continue;
                        }
                        else if (Regex.IsMatch(val.application, Regex.Escape("SAR (communications)"), RegexOptions.IgnoreCase))
                        {

                            if (val.main_reg.Contains("AERONAUTICAL"))
                            {
                                //Debug.WriteLine("qqqq");
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 2).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                //Debug.WriteLine("tttt");
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 125).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (Regex.IsMatch(val.application, Regex.Escape("Weather satellites"), RegexOptions.IgnoreCase))
                        {

                            if (Regex.IsMatch(val.main_reg, Regex.Escape("METEOROLOGICAL"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION"), RegexOptions.IgnoreCase))
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.first_up_lvl_id,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 141).SingleOrDefault();

                                    //Debug.WriteLine("xxxx" + val.application + "pp");
                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.first_up_lvl_id,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 141).SingleOrDefault();


                                    //Debug.WriteLine("xxxx" + val.application + "rr");
                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();

                                }

                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.first_up_lvl_id,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 174).SingleOrDefault();
                                //Debug.WriteLine("xxxx" + val.application + "::222");
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.first_up_lvl_id,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 167).SingleOrDefault();
                                //Debug.WriteLine("xxxx:" + val.application + "::333");
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }

                            continue;
                        }
                        else if (Regex.IsMatch(val.application, Regex.Escape("MFCN"), RegexOptions.IgnoreCase))
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("MOBILE"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("Fixed"), RegexOptions.IgnoreCase))
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.first_up_lvl_id,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 235).SingleOrDefault();
                                    //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                    var appValue = (from ep in _conApp.Application

                                                    select new
                                                    {
                                                        ep.ApplicationId,
                                                        ep.ApplicationRangeId,
                                                        ep.ApplicationTermId

                                                    }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst.ApplicationTermsDBId).SingleOrDefault(); ;

                                    if (appValue == null)
                                    {
                                        ApplicationDB app = new ApplicationDB();
                                        app.ApplicationRangeId = value;
                                        app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                        app.comment = "" + val.notes;
                                        _conApp.Application.Add(app);
                                        _conApp.SaveChanges();
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("is here:"+ value+"=="+ ApplicationTermIdFirst.ApplicationTermsDBId);
                                    }

                                    var ApplicationTermIdFirst2 = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 66).SingleOrDefault();
                                    //Debug.WriteLine("xxxx:" + val.application + "::333555");

                                    var appValue2 = (from ep in _conApp.Application

                                                     select new
                                                     {
                                                         ep.ApplicationId,
                                                         ep.ApplicationRangeId,
                                                         ep.ApplicationTermId

                                                     }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst2.ApplicationTermsDBId).SingleOrDefault(); ;
                                    if (appValue2 == null)
                                    {
                                        ApplicationDB app2 = new ApplicationDB();
                                        app2.ApplicationRangeId = value;
                                        app2.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                        app2.comment = "" + val.notes;
                                        _conApp.Application.Add(app2);
                                        _conApp.SaveChanges();
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("is here 2:" + value + "==" + ApplicationTermIdFirst.ApplicationTermsDBId);
                                    }


                                }
                                else
                                {
                                    var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                    select new
                                                                    {
                                                                        ep.ApplicationTermsDBId,
                                                                        ep.name,
                                                                        ep.first_up_lvl_id,
                                                                        ep.second_up_lvl_id

                                                                    });
                                    var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 92).SingleOrDefault();
                                    //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                    var appValue2 = (from ep in _conApp.Application

                                                     select new
                                                     {
                                                         ep.ApplicationId,
                                                         ep.ApplicationRangeId,
                                                         ep.ApplicationTermId

                                                     }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst2.ApplicationTermsDBId).SingleOrDefault(); ;
                                    if (appValue2 == null)
                                    {
                                        ApplicationDB app2 = new ApplicationDB();
                                        app2.ApplicationRangeId = value;
                                        app2.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                        app2.comment = "" + val.notes;
                                        _conApp.Application.Add(app2);
                                        _conApp.SaveChanges();
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("is here 3:" + value + "==" + ApplicationTermIdFirst2.ApplicationTermsDBId);
                                    }
                                }
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("Fixed"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.first_up_lvl_id,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 235).SingleOrDefault();
                                //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                var appValue2 = (from ep in _conApp.Application

                                                 select new
                                                 {
                                                     ep.ApplicationId,
                                                     ep.ApplicationRangeId,
                                                     ep.ApplicationTermId

                                                 }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst.ApplicationTermsDBId).SingleOrDefault(); ;
                                if (appValue2 == null)
                                {
                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    //Debug.WriteLine("is here 4:" + value + "==" + ApplicationTermIdFirst.ApplicationTermsDBId);
                                }
                            }

                            continue;
                        }
                        else if (val.application == "IMT")
                        {
                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 104).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "GALILEO")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("maritime"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 300).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 191).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            //Debug.WriteLine("pppww:" + val.main_reg);
                            continue;
                        }
                        else if (val.application == "GLONASS")
                        {

                            if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIONAVIGATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 288).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIODETERMINATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 55).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;

                        }
                        else if (val.application == "GPS")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIONAVIGATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 288).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIODETERMINATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 79).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Radiolocation (civil)")
                        {

                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 113).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "Satellite navigation systems")
                        {

                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 288).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "DECT")
                        {
                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 207).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "Weather radar")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("meteorgical"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 141).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 160).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Maritime radar")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIOLOCATION"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 160).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 137).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "AES")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("FIXED-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 129).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 183).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Broadcasting (satellite)")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("BROADCASTING"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 268).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 167).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "SIT/SUT")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("FIXED-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 176).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }

                        else if (val.application == "MWS")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("FIXED"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 85).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;

                        }
                        else
                        {

                            var TempApplicationTermId = (from ep in _conApp.RootApplicationTermsDB

                                                         select new
                                                         {
                                                             ep.ApplicationTermsDBId,
                                                             ep.name
                                                         }

                               );
                            try
                            {
                                var ApplicationTermId = TempApplicationTermId.Where(x => x.name == val.application).SingleOrDefault();
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermId.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //Debug.WriteLine("www" + val.application);
                            }
                        }
                    }
                }
                else
                {
                    if (val.lower_freq.EndsWith("GHz"))
                    {
                        var result = val.lower_freq.Split(" GHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000000);
                        //Debug.WriteLine("GHz radi:" + splitLower+"=="+resultLower);
                    }
                    else if (val.lower_freq.EndsWith("MHz"))
                    {
                        var result = val.lower_freq.Split(" MHz");
                        splitLower = result[0];
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }
                        resultLower = (long)(tempLower * 1000000);
                        //Debug.WriteLine("MHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("kHz"))
                    {
                        var result = val.lower_freq.Split(" kHz");
                        splitLower = result[0];
                        //Debug.WriteLine("test:" + splitLower+":");
                        double tempLower = 0;
                        if (splitLower.Contains(" "))
                        {
                            string ttt = splitLower.Replace(" ", "");
                            splitLower = ttt;
                            tempLower = double.Parse(splitLower);
                        }
                        else
                        {
                            tempLower = double.Parse(splitLower);
                        }

                        resultLower = (long)(tempLower * 1000);
                        //Debug.WriteLine("kHz radi:" + splitLower + "==" + resultLower);
                    }
                    else if (val.lower_freq.EndsWith("Hz"))
                    {
                        var result = val.lower_freq.Split(" Hz");
                        splitLower = result[0];
                        long tempLower = long.Parse(splitLower);
                        resultLower = tempLower;
                        //Debug.WriteLine("radi:" + splitLower + "==" + resultLower);
                    }


                    if (val.higher_freq.EndsWith("GHz"))
                    {
                        var result = val.higher_freq.Split(" GHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000000);
                        //Debug.WriteLine("GHz works:" + splitHigher + "=="+ resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("MHz"))
                    {
                        var result = val.higher_freq.Split(" MHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }
                        resultHigher = (long)(tempHigher * 1000000);
                        //Debug.WriteLine("MHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("kHz"))
                    {
                        var result = val.higher_freq.Split(" kHz");
                        splitHigher = result[0];
                        double tempHigher = 0;
                        if (splitHigher.Contains(" "))
                        {
                            string ttt = splitHigher.Replace(" ", "");
                            splitHigher = ttt;
                            tempHigher = double.Parse(splitHigher);
                        }
                        else
                        {
                            tempHigher = double.Parse(splitHigher);
                        }

                        resultHigher = (long)(tempHigher * 1000);
                        //Debug.WriteLine("kHz works:" + splitHigher + "==" + resultHigher);
                    }
                    else if (val.higher_freq.EndsWith("Hz"))
                    {
                        var result = val.higher_freq.Split(" Hz");
                        splitHigher = result[0];
                        long tempHigher = long.Parse(splitHigher);
                        resultHigher = tempHigher;
                        //Debug.WriteLine("works:" + splitHigher + "==" + resultHigher);
                    }
                    //Debug.WriteLine("is this:" + resultLower + "==" + resultHigher);
                    //this condition is execute when higher frequency is repeated. 
                    var ApplicationRangeDb = (from allVal in _conApp.ApplicationRange
                                              where allVal.low == resultLower && allVal.high == resultHigher
                                              && allVal.RootApplicationDBId == RootApplication.FirstOrDefault().RootApplicationDBId
                                              select new
                                              {
                                                  allVal.ApplicationRangeId
                                              }
                          ).FirstOrDefault();
                    int value = 0;
                    value = ApplicationRangeDb.ApplicationRangeId;
                    if (val.application != "")
                    {
                        if (val.application == "Active sensors (satellite)")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                                {
                                    //Debug.WriteLine("wwwwwwww:" + val.main_reg + "===" + val.application);
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    var ApplicationTermIdSecond = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 140).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();


                                    ApplicationDB app2 = new ApplicationDB();
                                    app2.ApplicationRangeId = value;
                                    app2.ApplicationTermId = ApplicationTermIdSecond.ApplicationTermsDBId;
                                    app2.comment = "" + val.notes;
                                    _conApp.Application.Add(app2);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 170).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();

                                }
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                            {
                                //Debug.WriteLine("wwwwwwww:" + val.main_reg + "===" + val.application);
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 187).SingleOrDefault();


                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("METEOROLOGICAL AIDS"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 75).SingleOrDefault();


                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Passive sensors (satellite)")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                                {
                                    //Debug.WriteLine("wwwwwwww:" + val.main_reg + "===" + val.application);
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    var ApplicationTermIdSecond = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 140).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();


                                    ApplicationDB app2 = new ApplicationDB();
                                    app2.ApplicationRangeId = value;
                                    app2.ApplicationTermId = ApplicationTermIdSecond.ApplicationTermsDBId;
                                    app2.comment = "" + val.notes;
                                    _conApp.Application.Add(app2);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 123).SingleOrDefault();

                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();

                                }
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("SPACE RESEARCH"), RegexOptions.IgnoreCase))
                            {
                                //Debug.WriteLine("wwwwwwww:" + val.main_reg + "===" + val.application);
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 140).SingleOrDefault();


                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("METEOROLOGICAL AIDS"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 75).SingleOrDefault();


                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }


                            continue;
                        }

                        if (Regex.IsMatch(val.application, Regex.Escape("emergency detection"), RegexOptions.IgnoreCase))
                        {
                            var ValueTrack = (from ev in _conTrack.Tracking_tracing_data_acq
                                              where ev.high_freq == val.higher_freq
                                              select new { ev.id }).SingleOrDefault();

                            if (ValueTrack == null)
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 163).SingleOrDefault();

                                //Debug.WriteLine("rrr" + val.application);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            else
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 207).SingleOrDefault();

                                //Debug.WriteLine("wwww" + val.application);
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                        }

                        else if (Regex.IsMatch(val.application, Regex.Escape("SRR"), RegexOptions.IgnoreCase))
                        {
                            var ValueTrack = (from ev in _conTrack.TTT
                                              where ev.high_freq == val.higher_freq
                                              select new { ev.id }).SingleOrDefault();

                            if (ValueTrack == null)
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 250).SingleOrDefault();

                                //Debug.WriteLine("rrr" + val.application+"qqqq");
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.second_up_lvl_id == 167).SingleOrDefault();

                                //Debug.WriteLine("wwww" + val.application + "::");
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            continue;
                        }
                        else if (Regex.IsMatch(val.application, Regex.Escape("Weather satellites"), RegexOptions.IgnoreCase))
                        {

                            if (Regex.IsMatch(val.main_reg, Regex.Escape("METEOROLOGICAL"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION"), RegexOptions.IgnoreCase))
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.first_up_lvl_id,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 100).SingleOrDefault();

                                    //Debug.WriteLine("xxxx" + val.application + "::"+ val.main_reg);
                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();

                                }
                                else
                                {

                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB
                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.first_up_lvl_id,
                                                                       ep.second_up_lvl_id
                                                                   }).ToList();
                                    //Debug.WriteLine("wwww" + val.application + "::"+val.main_reg+":uuuu:"+ TempApplicationTermIdQQ.Count);
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 100).SingleOrDefault();

                                    //Debug.WriteLine("wwww" + val.application + "::");
                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();

                                }

                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("EARTH EXPLORATION"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.first_up_lvl_id,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 174).SingleOrDefault();

                                //Debug.WriteLine("qqqq" + val.application );
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            else
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.first_up_lvl_id,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 167).SingleOrDefault();
                                //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }

                            continue;
                        }
                        else if (Regex.IsMatch(val.application, Regex.Escape("MFCN"), RegexOptions.IgnoreCase))
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("MOBILE"), RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(val.main_reg, Regex.Escape("Fixed"), RegexOptions.IgnoreCase))
                                {
                                    var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                                   select new
                                                                   {
                                                                       ep.ApplicationTermsDBId,
                                                                       ep.name,
                                                                       ep.first_up_lvl_id,
                                                                       ep.second_up_lvl_id

                                                                   });
                                    var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 235).SingleOrDefault();
                                    //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                    var appValue = (from ep in _conApp.Application

                                                    select new
                                                    {
                                                        ep.ApplicationId,
                                                        ep.ApplicationRangeId,
                                                        ep.ApplicationTermId

                                                    }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst.ApplicationTermsDBId).SingleOrDefault(); ;

                                    if (appValue == null)
                                    {
                                        ApplicationDB app = new ApplicationDB();
                                        app.ApplicationRangeId = value;
                                        app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                        app.comment = "" + val.notes;
                                        _conApp.Application.Add(app);
                                        _conApp.SaveChanges();
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("is here: sec" + value + "==" + ApplicationTermIdFirst.ApplicationTermsDBId);
                                    }

                                    var ApplicationTermIdFirst2 = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 66).SingleOrDefault();
                                    //Debug.WriteLine("xxxx:" + val.application + "::333555");

                                    var appValue2 = (from ep in _conApp.Application

                                                     select new
                                                     {
                                                         ep.ApplicationId,
                                                         ep.ApplicationRangeId,
                                                         ep.ApplicationTermId

                                                     }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst2.ApplicationTermsDBId).SingleOrDefault(); ;
                                    if (appValue2 == null)
                                    {
                                        ApplicationDB app2 = new ApplicationDB();
                                        app2.ApplicationRangeId = value;
                                        app2.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                        app2.comment = "" + val.notes;
                                        _conApp.Application.Add(app2);
                                        _conApp.SaveChanges();
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("is here:2 sec" + value + "==" + ApplicationTermIdFirst2.ApplicationTermsDBId);
                                    }


                                }
                                else
                                {
                                    var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                    select new
                                                                    {
                                                                        ep.ApplicationTermsDBId,
                                                                        ep.name,
                                                                        ep.first_up_lvl_id,
                                                                        ep.second_up_lvl_id

                                                                    });
                                    var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 66).SingleOrDefault();
                                    //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                    var appValue2 = (from ep in _conApp.Application

                                                     select new
                                                     {
                                                         ep.ApplicationId,
                                                         ep.ApplicationRangeId,
                                                         ep.ApplicationTermId

                                                     }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst2.ApplicationTermsDBId).SingleOrDefault();
                                    if (appValue2 == null)
                                    {
                                        ApplicationDB app2 = new ApplicationDB();
                                        app2.ApplicationRangeId = value;
                                        app2.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                        app2.comment = "" + val.notes;
                                        _conApp.Application.Add(app2);
                                        _conApp.SaveChanges();
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("is here:3 sec" + value + "==" + ApplicationTermIdFirst2.ApplicationTermsDBId);
                                    }
                                }
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("Fixed"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ = (from ep in _conApp.RootApplicationTermsDB

                                                               select new
                                                               {
                                                                   ep.ApplicationTermsDBId,
                                                                   ep.name,
                                                                   ep.first_up_lvl_id,
                                                                   ep.second_up_lvl_id

                                                               });
                                var ApplicationTermIdFirst = TempApplicationTermIdQQ.Where(x => x.name == val.application && x.first_up_lvl_id == 345).SingleOrDefault();
                                //Debug.WriteLine("xxxx:" + val.application + "::333555");
                                var appValue2 = (from ep in _conApp.Application

                                                 select new
                                                 {
                                                     ep.ApplicationId,
                                                     ep.ApplicationRangeId,
                                                     ep.ApplicationTermId

                                                 }).ToList().Where(x => x.ApplicationRangeId == value && x.ApplicationTermId == ApplicationTermIdFirst.ApplicationTermsDBId).SingleOrDefault();
                                if (appValue2 == null)
                                {
                                    ApplicationDB app = new ApplicationDB();
                                    app.ApplicationRangeId = value;
                                    app.ApplicationTermId = ApplicationTermIdFirst.ApplicationTermsDBId;
                                    app.comment = "" + val.notes;
                                    _conApp.Application.Add(app);
                                    _conApp.SaveChanges();
                                }
                                else
                                {
                                    //Debug.WriteLine("is here:4 sec" + value + "==" + ApplicationTermIdFirst.ApplicationTermsDBId);
                                }
                            }

                            continue;
                        }
                        else if (val.application == "IMT")
                        {
                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 69).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "GALILEO")
                        {
                            //Debug.WriteLine("ppp" + val.main_reg);
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("maritime"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 300).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();

                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 191).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }

                            continue;
                        }
                        else if (val.application == "GLONASS")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIONAVIGATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 191).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIODETERMINATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 79).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "GPS")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIONAVIGATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 191).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIODETERMINATION-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 79).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Radiolocation (civil)")
                        {

                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 113).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "Satellite navigation systems")
                        {

                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 191).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "DECT")
                        {
                            var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                            select new
                                                            {
                                                                ep.ApplicationTermsDBId,
                                                                ep.name,
                                                                ep.first_up_lvl_id,
                                                                ep.second_up_lvl_id

                                                            });
                            var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 306).SingleOrDefault();

                            ApplicationDB app = new ApplicationDB();
                            app.ApplicationRangeId = value;
                            app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                            app.comment = "" + val.notes;
                            _conApp.Application.Add(app);
                            _conApp.SaveChanges();

                            continue;
                        }
                        else if (val.application == "Weather radar")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("meteorgical"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 100).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 113).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Maritime radar")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("RADIOLOCATION"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 113).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 96).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "AES")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("FIXED-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 176).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 183).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "Broadcasting (satellite)")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("BROADCASTING"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.ApplicationTermsDBId == 415).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            else
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.first_up_lvl_id == 167).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "SIT/SUT")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("FIXED-SATELLITE"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 129).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;
                        }
                        else if (val.application == "MWS")
                        {
                            if (Regex.IsMatch(val.main_reg, Regex.Escape("FIXED"), RegexOptions.IgnoreCase))
                            {
                                var TempApplicationTermIdQQ2 = (from ep in _conApp.RootApplicationTermsDB

                                                                select new
                                                                {
                                                                    ep.ApplicationTermsDBId,
                                                                    ep.name,
                                                                    ep.first_up_lvl_id,
                                                                    ep.second_up_lvl_id

                                                                });
                                var ApplicationTermIdFirst2 = TempApplicationTermIdQQ2.Where(x => x.name == val.application && x.second_up_lvl_id == 59).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermIdFirst2.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            continue;

                        }
                        else
                        {

                            var TempApplicationTermId = (from ep in _conApp.RootApplicationTermsDB

                                                         select new
                                                         {
                                                             ep.ApplicationTermsDBId,
                                                             ep.name
                                                         }

                               );
                            try
                            {
                                var ApplicationTermId = TempApplicationTermId.Where(x => x.name == val.application).SingleOrDefault();
                                ApplicationDB app = new ApplicationDB();
                                app.ApplicationRangeId = value;
                                app.ApplicationTermId = ApplicationTermId.ApplicationTermsDBId;
                                app.comment = "" + val.notes;
                                _conApp.Application.Add(app);
                                _conApp.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //Debug.WriteLine("www" + val.application);
                            }
                        }

                    }
                }
            }

        }

        public void ReadDocumentsData()
        {
            using (SqlConnection conn = new SqlConnection(_conf.GetConnectionString("AuthDBContextConnection")))
            {

                conn.Open();


                SqlCommand cmd = new SqlCommand("ReadDocumentsApplication", conn);


                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    //Debug.WriteLine("test method:"+rdr.FieldCount);
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        if (rdr["document"].ToString() != "")
                        {
                            if (!rdr["document"].ToString().Contains(","))
                            {

                                var ValuesDocuments = _conApp.DocumentsDb.ToList();
                                //(from ww in _conApp.DocumentsDb
                                //                   select new DocumentsDb
                                //                   {

                                //                       ww.DocumentsId,
                                //                       ww.Doc_number,
                                //                       ww.Title_of_doc,
                                //                       ww.Hyperlink,
                                //                       ww.Low_freq,
                                //                       ww.High_freq,
                                //                       ww.Application,
                                //                       ww.Type_of_doc
                                //                   }
                                //                            ).ToList();
                               // Debug.WriteLine("testing count of docs:" + ValuesDocuments.Count);

                                var SingleDocuments = ValuesDocuments.Where(x => x.Doc_number.Contains(rdr["document"].ToString())
                                && x.Application.Equals(rdr["application"].ToString())
                                 ).ToList();
                                

                                UploadSingleDoc singleDoc = new UploadSingleDoc(_conApp);
                                singleDoc.ExecuteUpload(SingleDocuments, int.Parse(rdr["ApplicationTermsDBId"].ToString()), int.Parse(rdr["ApplicationId"].ToString()), rdr["document"].ToString());



                                //if (SingleDocuments.Count == 0)
                                //{
                                //    var root = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == int.Parse(rdr["ApplicationTermsDBId"].ToString())).FirstOrDefault();
                                //    if (root.Layer == 1)
                                //    {
                                //        var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == root.ApplicationTermsDBId).ToList();
                                //        var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                                //        var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                                //        var LowStr = "";
                                //        var HighStr = "";
                                //        long Low = 0;
                                //        long High = 0;
                                //        foreach (var tempRoot in valuesRoot)
                                //        {
                                //            var SingleDocumentsLayerOne = ValuesDocuments.Where(x => x.Doc_number.Contains(rdr["document"].ToString()) && x.Application.Equals(tempRoot.name)).ToList();

                                //            foreach (var tempValueLayerOne in SingleDocumentsLayerOne)
                                //            {

                                //                if (tempValueLayerOne.Low_freq.Contains("GHz"))
                                //                {
                                //                    var tempLow = tempValueLayerOne.Low_freq.Split(" GHz");
                                //                    LowStr = tempLow[0];

                                //                    Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                                //                }
                                //                else if (tempValueLayerOne.Low_freq.Contains("MHz"))
                                //                {
                                //                    var tempLow = tempValueLayerOne.Low_freq.Split(" MHz");
                                //                    LowStr = tempLow[0];

                                //                    Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                                //                }
                                //                else if (tempValueLayerOne.Low_freq.Contains("kHz"))
                                //                {
                                //                    var tempLow = tempValueLayerOne.Low_freq.Split(" kHz");
                                //                    LowStr = tempLow[0];

                                //                    Low = (long)Math.Round(1000 * double.Parse(LowStr));
                                //                }

                                //                else if (tempValueLayerOne.Low_freq.Contains("Hz"))
                                //                {
                                //                    var tempLow = tempValueLayerOne.Low_freq.Split(" Hz");
                                //                    LowStr = tempLow[0];
                                //                    Low = long.Parse(LowStr);
                                //                }


                                //                if (tempValueLayerOne.High_freq.Contains("GHz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerOne.High_freq.Split(" GHz");
                                //                    HighStr = tempHIgh[0];
                                //                    //High = (long)double.Parse(HighStr) * 1000000000;
                                //                    High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                                //                }
                                //                else if (tempValueLayerOne.High_freq.Contains("MHz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerOne.High_freq.Split(" MHz");
                                //                    HighStr = tempHIgh[0];
                                //                    //High = (long)double.Parse(HighStr) * 1000000;
                                //                    High = (long)Math.Round(1000000 * double.Parse(HighStr));
                                //                }
                                //                else if (tempValueLayerOne.High_freq.Contains("kHz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerOne.High_freq.Split(" kHz");
                                //                    HighStr = tempHIgh[0];
                                //                    //High = (long)double.Parse(HighStr) * 1000;
                                //                    High = (long)Math.Round(1000 * double.Parse(HighStr));
                                //                }
                                //                else if (tempValueLayerOne.High_freq.Contains("Hz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerOne.High_freq.Split(" Hz");
                                //                    HighStr = tempHIgh[0];
                                //                    High = long.Parse(HighStr);
                                //                }

                                //                if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                                //                {
                                //                    if (tempValueLayerOne.Type_of_doc == "R")
                                //                    {
                                //                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerOne.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                        && x.ApplicationTermId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                //                        if (ApplicationTempNew != null)
                                //                        {
                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValueLayerOne.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValueLayerOne.DocumentsId)
                                //                            {

                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValueLayerOne.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();

                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }
                                //                    }

                                //                    if (tempValueLayerOne.Type_of_doc == "I")
                                //                    {

                                //                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerOne.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                      && x.ApplicationTermId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                //                        if (ApplicationTempNew != null)
                                //                        {
                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValueLayerOne.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValueLayerOne.DocumentsId)
                                //                            {

                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValueLayerOne.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();

                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }
                                //                    }

                                //                }


                                //            }
                                //        }


                                //    }
                                //    else if (root.Layer == 2)
                                //    {
                                //        var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == root.ApplicationTermsDBId).ToList();
                                //        var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                                //        var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                                //        var LowStr = "";
                                //        var HighStr = "";
                                //        long Low = 0;
                                //        long High = 0;
                                //        foreach (var tempRoot in valuesRoot)
                                //        {
                                //            var SingleDocumentsLayerTwo = ValuesDocuments.Where(x => x.Doc_number.Contains(rdr["document"].ToString()) && x.Application.Equals(tempRoot.name)).ToList();

                                //            foreach (var tempValueLayerTwo in SingleDocumentsLayerTwo)
                                //            {

                                //                if (tempValueLayerTwo.Low_freq.Contains("GHz"))
                                //                {
                                //                    var tempLow = tempValueLayerTwo.Low_freq.Split(" GHz");
                                //                    LowStr = tempLow[0];

                                //                    Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                                //                }
                                //                else if (tempValueLayerTwo.Low_freq.Contains("MHz"))
                                //                {
                                //                    var tempLow = tempValueLayerTwo.Low_freq.Split(" MHz");
                                //                    LowStr = tempLow[0];

                                //                    Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                                //                }
                                //                else if (tempValueLayerTwo.Low_freq.Contains("kHz"))
                                //                {
                                //                    var tempLow = tempValueLayerTwo.Low_freq.Split(" kHz");
                                //                    LowStr = tempLow[0];

                                //                    Low = (long)Math.Round(1000 * double.Parse(LowStr));
                                //                }

                                //                else if (tempValueLayerTwo.Low_freq.Contains("Hz"))
                                //                {
                                //                    var tempLow = tempValueLayerTwo.Low_freq.Split(" Hz");
                                //                    LowStr = tempLow[0];
                                //                    Low = long.Parse(LowStr);
                                //                }


                                //                if (tempValueLayerTwo.High_freq.Contains("GHz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerTwo.High_freq.Split(" GHz");
                                //                    HighStr = tempHIgh[0];
                                //                    //High = (long)double.Parse(HighStr) * 1000000000;
                                //                    High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                                //                }
                                //                else if (tempValueLayerTwo.High_freq.Contains("MHz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerTwo.High_freq.Split(" MHz");
                                //                    HighStr = tempHIgh[0];
                                //                    //High = (long)double.Parse(HighStr) * 1000000;
                                //                    High = (long)Math.Round(1000000 * double.Parse(HighStr));
                                //                }
                                //                else if (tempValueLayerTwo.High_freq.Contains("kHz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerTwo.High_freq.Split(" kHz");
                                //                    HighStr = tempHIgh[0];
                                //                    //High = (long)double.Parse(HighStr) * 1000;
                                //                    High = (long)Math.Round(1000 * double.Parse(HighStr));
                                //                }
                                //                else if (tempValueLayerTwo.High_freq.Contains("Hz"))
                                //                {
                                //                    var tempHIgh = tempValueLayerTwo.High_freq.Split(" Hz");
                                //                    HighStr = tempHIgh[0];
                                //                    High = long.Parse(HighStr);
                                //                }

                                //                if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                                //                {
                                //                    if (tempValueLayerTwo.Type_of_doc == "R")
                                //                    {
                                //                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerTwo.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                        && x.ApplicationTermId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                //                        if (ApplicationTempNew != null) { 
                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValueLayerTwo.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValueLayerTwo.DocumentsId)
                                //                            {

                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValueLayerTwo.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();

                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }
                                //                    }

                                //                    if (tempValueLayerTwo.Type_of_doc == "I")
                                //                    {

                                //                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerTwo.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                      && x.ApplicationTermId == tempRoot.ApplicationTermsDBId).SingleOrDefault();
                                //                        if (ApplicationTempNew != null)
                                //                        {
                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValueLayerTwo.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValueLayerTwo.DocumentsId)
                                //                            {


                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValueLayerTwo.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();


                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }
                                //                    }

                                //                }


                                //            }
                                //        }
                                //    }

                                //}
                                //else if (SingleDocuments.Count > 0)
                                //{

                                //    foreach (var tempValue in SingleDocuments)
                                //    {

                                //        var root = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == int.Parse(rdr["ApplicationTermsDBId"].ToString())).FirstOrDefault();

                                //        if (root.Layer == 1)
                                //        {

                                //            var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == root.ApplicationTermsDBId).ToList();
                                //            var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                                //            var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                                //            var LowStr = "";
                                //            var HighStr = "";
                                //            long Low = 0;
                                //            long High = 0;

                                //            if (tempValue.Low_freq.Contains("GHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" GHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                                //            }
                                //            else if (tempValue.Low_freq.Contains("MHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" MHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                                //            }
                                //            else if (tempValue.Low_freq.Contains("kHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" kHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000 * double.Parse(LowStr));
                                //            }

                                //            else if (tempValue.Low_freq.Contains("Hz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" Hz");
                                //                LowStr = tempLow[0];
                                //                Low = long.Parse(LowStr);
                                //            }


                                //            if (tempValue.High_freq.Contains("GHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" GHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000000000;
                                //                High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                                //            }
                                //            else if (tempValue.High_freq.Contains("MHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" MHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000000;
                                //                High = (long)Math.Round(1000000 * double.Parse(HighStr));
                                //            }
                                //            else if (tempValue.High_freq.Contains("kHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" kHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000;
                                //                High = (long)Math.Round(1000 * double.Parse(HighStr));
                                //            }
                                //            else if (tempValue.High_freq.Contains("Hz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" Hz");
                                //                HighStr = tempHIgh[0];
                                //                High = long.Parse(HighStr);
                                //            }

                                //            if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                                //            {
                                //                if (valuesRoot.Count == 0)
                                //                {
                                //                    var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                       && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                                //                    if (tempValue.Type_of_doc == "R")
                                //                    {
                                //                        if (ApplicationTempNew != null)
                                //                        {
                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //                            {

                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValue.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();

                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }
                                //                    }

                                //                    if (tempValue.Type_of_doc == "I")
                                //                    {

                                //                        if (ApplicationTempNew != null)
                                //                        {

                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //                            {


                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValue.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();


                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }
                                //                        }
                                //                }


                                //                //if (tempValue.Type_of_doc == "R")
                                //                //{
                                //                //    if (ApplicationTemp.documents_Id == null)
                                //                //    {
                                //                //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                //        _conApp.Application.Update(ApplicationTemp);
                                //                //        _conApp.SaveChanges();
                                //                //    }
                                //                //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                                //                //    {
                                //                //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                                //                //        if (ApplicationVal != null)
                                //                //        {
                                //                //            if (ApplicationVal.documents_Id == null)
                                //                //            {
                                //                //                ApplicationDB app = new ApplicationDB()
                                //                //                {
                                //                //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                //                //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                                //                //                    comment = null,
                                //                //                    documents_Id = tempValue.DocumentsId

                                //                //                };

                                //                //                _conApp.Application.Add(app);
                                //                //                _conApp.SaveChanges();
                                //                //            }
                                //                //        }
                                //                //    }
                                //                //}


                                //                //if (tempValue.Type_of_doc == "I")
                                //                //{
                                //                //    if (ApplicationTemp.documents_Id == null)
                                //                //    {
                                //                //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                //        _conApp.Application.Update(ApplicationTemp);
                                //                //        _conApp.SaveChanges();
                                //                //    }
                                //                //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                                //                //    {
                                //                //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                                //                //        if (ApplicationVal != null)
                                //                //        {
                                //                //            if (ApplicationVal.documents_Id == null)
                                //                //            {
                                //                //                ApplicationDB app = new ApplicationDB()
                                //                //                {
                                //                //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                //                //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                                //                //                    comment = null,
                                //                //                    documents_Id = tempValue.DocumentsId

                                //                //                };

                                //                //                _conApp.Application.Add(app);
                                //                //                _conApp.SaveChanges();
                                //                //            }
                                //                //        }
                                //                //    }
                                //                //}




                                //            }

                                //            foreach (var temp in valuesRoot)
                                //            {

                                //                // Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                                //                var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                                //                ).ToList();

                                //                foreach (var ttDoc in docTemp)
                                //                {
                                //                    //Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                                //                    var LowStrDoc = "";
                                //                    var HighStrDoc = "";
                                //                    long LowDoc = 0;
                                //                    long HighDoc = 0;
                                //                    if (ttDoc != null)
                                //                    {
                                //                        if (ttDoc.Low_freq.Contains("GHz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" GHz");
                                //                            LowStrDoc = tempLow[0];

                                //                            LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                                //                        }
                                //                        else if (ttDoc.Low_freq.Contains("MHz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" MHz");
                                //                            LowStrDoc = tempLow[0];

                                //                            LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                                //                        }
                                //                        else if (ttDoc.Low_freq.Contains("kHz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" kHz");
                                //                            LowStrDoc = tempLow[0];

                                //                            LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                                //                        }

                                //                        else if (ttDoc.Low_freq.Contains("Hz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" Hz");
                                //                            LowStrDoc = tempLow[0];
                                //                            LowDoc = long.Parse(LowStrDoc);
                                //                        }


                                //                        if (ttDoc.High_freq.Contains("GHz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" GHz");
                                //                            HighStrDoc = tempHIgh[0];
                                //                            //High = (long)double.Parse(HighStr) * 1000000000;
                                //                            HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                                //                        }
                                //                        else if (ttDoc.High_freq.Contains("MHz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" MHz");
                                //                            HighStrDoc = tempHIgh[0];
                                //                            //High = (long)double.Parse(HighStr) * 1000000;
                                //                            HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                                //                        }
                                //                        else if (ttDoc.High_freq.Contains("kHz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" kHz");
                                //                            HighStrDoc = tempHIgh[0];

                                //                            //High = (long)double.Parse(HighStr) * 1000;
                                //                            HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                                //                        }
                                //                        else if (ttDoc.High_freq.Contains("Hz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" Hz");
                                //                            HighStrDoc = tempHIgh[0];
                                //                            HighDoc = long.Parse(HighStrDoc);
                                //                        }

                                //                        if ((ApplicationRange.low >= LowDoc && ApplicationRange.low <= HighDoc) || (ApplicationRange.high >= LowDoc && ApplicationRange.high <= HighDoc))
                                //                        {
                                //                            var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == ttDoc.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                    && x.ApplicationTermId == temp.ApplicationTermsDBId).SingleOrDefault();
                                //                            if (ttDoc.Type_of_doc == "R")
                                //                            {
                                //                                if (ApplicationTempNew != null)
                                //                                {

                                //                                    if (ApplicationTempNew.documents_Id == null)
                                //                                    {
                                                                       
                                //                                        ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //                                        _conApp.Application.Update(ApplicationTempNew);
                                //                                        _conApp.SaveChanges();
                                //                                    }
                                //                                    else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //                                    {
                                                                        
                                //                                        ApplicationDB app = new ApplicationDB()
                                //                                        {
                                //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                            comment = null,
                                //                                            documents_Id = tempValue.DocumentsId

                                //                                        };

                                //                                        _conApp.Application.Add(app);
                                //                                        _conApp.SaveChanges();

                                //                                    }
                                //                                }
                                //                                else
                                //                                {
                                                                    
                                //                                    var queryDocument = (from all in _conApp.Application
                                //                                                         join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                                //                                                         where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                                                         && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                                //                                                         select new
                                //                                                         {
                                //                                                             app_id = all.ApplicationId,
                                //                                                             doc_id = all.documents_Id
                                //                                                         }
                                //                                                         ).ToList();
                                //                                    if (queryDocument.Count == 0)
                                //                                    {
                                //                                        //Debug.WriteLine("pp:");                                                                                 
                                //                                        if (ApplicationTemp.documents_Id == null)
                                //                                        {
                                //                                            //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                                //                                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                            _conApp.Application.Update(ApplicationTemp);
                                //                                            _conApp.SaveChanges();
                                //                                        }
                                //                                        else
                                //                                        {
                                //                                            // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                                //                                            //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                                //                                            //    + "===second doc:" + tempValue.Doc_number);
                                //                                            ApplicationTemp.ApplicationId = 0;
                                //                                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                            _conApp.Application.Add(ApplicationTemp);
                                //                                            _conApp.SaveChanges();
                                //                                        }

                                //                                    }
                                //                                    else
                                //                                    {
                                //                                        //Debug.WriteLine(":yes:"+ ApplicationRange.LowView);
                                //                                    }

                                //                                }

                                //                            }


                                //                            if (ttDoc.Type_of_doc == "I")
                                //                            {
                                //                                if (ApplicationTempNew != null)
                                //                                {
                                //                                    if (ApplicationTempNew.documents_Id == null)
                                //                                    {
                                //                                        ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //                                        _conApp.Application.Update(ApplicationTempNew);
                                //                                        _conApp.SaveChanges();
                                //                                    }
                                //                                    else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //                                    {

                                //                                        ApplicationDB app = new ApplicationDB()
                                //                                        {
                                //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                            comment = null,
                                //                                            documents_Id = tempValue.DocumentsId

                                //                                        };

                                //                                        _conApp.Application.Add(app);
                                //                                        _conApp.SaveChanges();
                                //                                    }
                                //                                }
                                //                                else
                                //                                {
                                //                                    //ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                    //_conApp.Application.Update(ApplicationTemp);
                                //                                    //_conApp.SaveChanges();

                                //                                    var queryDocument = (from all in _conApp.Application
                                //                                                         join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                                //                                                         where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                                                         && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                                //                                                         select new
                                //                                                         {
                                //                                                             app_id = all.ApplicationId,
                                //                                                             doc_id = all.documents_Id
                                //                                                         }
                                //                                                        ).ToList();
                                //                                    if (queryDocument.Count == 0)
                                //                                    {
                                //                                        //Debug.WriteLine("pp:");                                                                                 
                                //                                        if (ApplicationTemp.documents_Id == null)
                                //                                        {
                                //                                            //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                                //                                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                            _conApp.Application.Update(ApplicationTemp);
                                //                                            _conApp.SaveChanges();
                                //                                        }
                                //                                        else
                                //                                        {
                                //                                            // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                                //                                            //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                                //                                            //    + "===second doc:" + tempValue.Doc_number);
                                //                                            ApplicationTemp.ApplicationId = 0;
                                //                                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                            _conApp.Application.Add(ApplicationTemp);
                                //                                            _conApp.SaveChanges();
                                //                                        }

                                //                                    }
                                //                                    else
                                //                                    {
                                                                        
                                //                                    }
                                //                                }

                                //                            }

                                //                        }
                                //                    }
                                //                }

                                //            }
                                //        }
                                //        else if (root.Layer == 2)
                                //        {
                                           
                                //            //Debug.WriteLine("layer 2:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString());
                                //            var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == root.ApplicationTermsDBId).ToList();
                                //            var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                                //            var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                                //            var LowStr = "";
                                //            var HighStr = "";
                                //            long Low = 0;
                                //            long High = 0;
                                //            if (tempValue.Low_freq.Contains("GHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" GHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                                //            }
                                //            else if (tempValue.Low_freq.Contains("MHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" MHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                                //            }
                                //            else if (tempValue.Low_freq.Contains("kHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" kHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000 * double.Parse(LowStr));
                                //            }

                                //            else if (tempValue.Low_freq.Contains("Hz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" Hz");
                                //                LowStr = tempLow[0];
                                //                Low = long.Parse(LowStr);
                                //            }


                                //            if (tempValue.High_freq.Contains("GHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" GHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000000000;
                                //                High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                                //            }
                                //            else if (tempValue.High_freq.Contains("MHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" MHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000000;
                                //                High = (long)Math.Round(1000000 * double.Parse(HighStr));
                                //            }
                                //            else if (tempValue.High_freq.Contains("kHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" kHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000;
                                //                High = (long)Math.Round(1000 * double.Parse(HighStr));
                                //            }
                                //            else if (tempValue.High_freq.Contains("Hz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" Hz");
                                //                HighStr = tempHIgh[0];
                                //                High = long.Parse(HighStr);
                                //            }

                                //            if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                                //            {
                                //                //if (root.ApplicationTermsDBId == 152)
                                //                //{
                                //                //    Debug.WriteLine("low freq:"+ ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //                //}
                                //                var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                           && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                                //                    if (tempValue.Type_of_doc == "R")
                                //                    {
                                //                        if (ApplicationTempNew != null)
                                //                        {

                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                            //if (root.ApplicationTermsDBId == 152)
                                //                            //{
                                //                            //    Debug.WriteLine(" it is null:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //                            //}
                                //                            ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //                            {
                                //                            //if (root.ApplicationTermsDBId == 152)
                                //                            //{
                                //                            //    Debug.WriteLine(" it is diffrent:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //                            //}
                                //                            ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValue.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();

                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                        //if (root.ApplicationTermsDBId == 152)
                                //                        //{
                                //                        //    Debug.WriteLine(" it is simple update:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                                //                        //}
                                //                        var queryDocument = (from all in _conApp.Application
                                //                                             join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                                //                                             where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                                             && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                                //                                             select new
                                //                                             {
                                //                                                 app_id = all.ApplicationId,
                                //                                                 doc_id = all.documents_Id
                                //                                             }
                                //                                             ).ToList();
                                //                        if (queryDocument.Count == 0)
                                //                        {
                                //                            //Debug.WriteLine("pp:");                                                                                 
                                //                            if(ApplicationTemp.documents_Id == null)                                                                                                                                                                                                            
                                //                            {
                                //                                //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                                //                                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTemp);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else
                                //                            {
                                //                               // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                                //                                //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                                //                                //    + "===second doc:" + tempValue.Doc_number);
                                //                                ApplicationTemp.ApplicationId = 0;
                                //                                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Add(ApplicationTemp);
                                //                                _conApp.SaveChanges();
                                //                            }
                                                            
                                //                        }
                                //                        else
                                //                        {
                                //                            //Debug.WriteLine(":yes:"+ ApplicationRange.LowView);
                                //                        }
                                                        
                                //                        }


                                //                    }

                                //                    if (tempValue.Type_of_doc == "I")
                                //                    {
                                //                        if (ApplicationTempNew != null)
                                //                        {
                                //                            if (ApplicationTempNew.documents_Id == null)
                                //                            {
                                //                                ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTempNew);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                                //                            {

                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValue.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                        //ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                        //_conApp.Application.Update(ApplicationTemp);
                                //                        //_conApp.SaveChanges();

                                //                        var queryDocument = (from all in _conApp.Application
                                //                                             join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                                //                                             where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                                             && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                                //                                             select new
                                //                                             {
                                //                                                 app_id = all.ApplicationId,
                                //                                                 doc_id = all.documents_Id
                                //                                             }
                                //                                            ).ToList();
                                //                        if (queryDocument.Count == 0)
                                //                        {
                                //                            //Debug.WriteLine("pp:");                                                                                 
                                //                            if (ApplicationTemp.documents_Id == null)
                                //                            {
                                //                                //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                                //                                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Update(ApplicationTemp);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                            else
                                //                            {
                                //                                // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                                //                                //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                                //                                //    + "===second doc:" + tempValue.Doc_number);
                                //                                ApplicationTemp.ApplicationId = 0;
                                //                                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                                _conApp.Application.Add(ApplicationTemp);
                                //                                _conApp.SaveChanges();
                                //                            }

                                //                        }
                                //                        else
                                //                        {
                                //                            //Debug.WriteLine(":yes:"+ ApplicationRange.LowView);
                                //                        }
                                //                    }

                                //                    }
                                                


                                //            }



                                //            foreach (var temp in valuesRoot)
                                //            {

                                //                //Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                                //                var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                                //                ).ToList();
                                                
                                //                foreach (var ttDoc in docTemp)
                                //                {
                                //                    if (ttDoc != null)
                                //                    {
                                //                        //Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                                //                        var LowStrDoc = "";
                                //                        var HighStrDoc = "";
                                //                        long LowDoc = 0;
                                //                        long HighDoc = 0;
                                //                        if (ttDoc.Low_freq.Contains("GHz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" GHz");
                                //                            LowStrDoc = tempLow[0];

                                //                            LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                                //                        }
                                //                        else if (ttDoc.Low_freq.Contains("MHz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" MHz");
                                //                            LowStrDoc = tempLow[0];

                                //                            LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                                //                        }
                                //                        else if (ttDoc.Low_freq.Contains("kHz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" kHz");
                                //                            LowStrDoc = tempLow[0];

                                //                            LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                                //                        }

                                //                        else if (ttDoc.Low_freq.Contains("Hz"))
                                //                        {
                                //                            var tempLow = ttDoc.Low_freq.Split(" Hz");
                                //                            LowStrDoc = tempLow[0];
                                //                            LowDoc = long.Parse(LowStrDoc);
                                //                        }


                                //                        if (ttDoc.High_freq.Contains("GHz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" GHz");
                                //                            HighStrDoc = tempHIgh[0];
                                //                            //High = (long)double.Parse(HighStr) * 1000000000;
                                //                            HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                                //                        }
                                //                        else if (ttDoc.High_freq.Contains("MHz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" MHz");
                                //                            HighStrDoc = tempHIgh[0];
                                //                            //High = (long)double.Parse(HighStr) * 1000000;
                                //                            HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                                //                        }
                                //                        else if (ttDoc.High_freq.Contains("kHz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" kHz");
                                //                            HighStrDoc = tempHIgh[0];

                                //                            //High = (long)double.Parse(HighStr) * 1000;
                                //                            HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                                //                        }
                                //                        else if (ttDoc.High_freq.Contains("Hz"))
                                //                        {
                                //                            var tempHIgh = ttDoc.High_freq.Split(" Hz");
                                //                            HighStrDoc = tempHIgh[0];
                                //                            HighDoc = long.Parse(HighStrDoc);
                                //                        }

                                //                        if ((ApplicationRange.low >= LowDoc && ApplicationRange.low <= HighDoc) || (ApplicationRange.high >= LowDoc && ApplicationRange.high <= HighDoc))
                                //                        {
                                //                            // Debug.WriteLine("hello im entered");

                                //                            if (root.ApplicationTermsDBId == 152)
                                //                            {
                                //                                Debug.WriteLine("low freq:" + ApplicationRange.LowView+"values of docs:" + tempValue.DocumentsId +"==="+ ttDoc.DocumentsId);
                                //                            }
                                //                            var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == ttDoc.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                                //                            && x.ApplicationTermId == temp.ApplicationTermsDBId).SingleOrDefault();
                                //                            if (ttDoc.Type_of_doc == "R")
                                //                            {
                                //                                if (ApplicationTempNew != null)
                                //                                {
                                //                                    if (ApplicationTempNew.documents_Id == null)
                                //                                    {
                                //                                        ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                //                                        _conApp.Application.Update(ApplicationTempNew);
                                //                                        _conApp.SaveChanges();
                                //                                    }
                                //                                    else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                //                                    {

                                //                                        ApplicationDB app = new ApplicationDB()
                                //                                        {
                                //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                            ApplicationTermId = temp.ApplicationTermsDBId,
                                //                                            comment = null,
                                //                                            documents_Id = ttDoc.DocumentsId

                                //                                        };

                                //                                        _conApp.Application.Add(app);
                                //                                        _conApp.SaveChanges();

                                //                                    }
                                //                                }
                                //                                else
                                //                                {
                                //                                    ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                                //                                    _conApp.Application.Update(ApplicationTemp);
                                //                                    _conApp.SaveChanges();
                                //                                }



                                //                            }

                                //                            if (ttDoc.Type_of_doc == "I")
                                //                            {
                                //                                if (ApplicationTempNew != null)
                                //                                {
                                //                                    if (ApplicationTempNew.documents_Id == null)
                                //                                    {
                                //                                        ApplicationTempNew.documents_Id = ttDoc.DocumentsId;
                                //                                        _conApp.Application.Update(ApplicationTempNew);
                                //                                        _conApp.SaveChanges();
                                //                                    }
                                //                                    else if (ApplicationTempNew.documents_Id != ttDoc.DocumentsId)
                                //                                    {
                                //                                        //Debug.WriteLine("second enter:");

                                //                                        ApplicationDB app = new ApplicationDB()
                                //                                        {
                                //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                                //                                            ApplicationTermId = temp.ApplicationTermsDBId,
                                //                                            comment = null,
                                //                                            documents_Id = ttDoc.DocumentsId

                                //                                        };

                                //                                        _conApp.Application.Add(app);
                                //                                        _conApp.SaveChanges();

                                //                                    }
                                //                                }
                                //                                else
                                //                                {
                                //                                    ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                                //                                    _conApp.Application.Update(ApplicationTemp);
                                //                                    _conApp.SaveChanges();
                                //                                }
                                //                            }

                                //                        }


                                //                    }

                                //                }
                                            
                                //                }
                                            
                                //        }
                                //        else if (root.Layer == 3)
                                //        {
                                //            //Debug.WriteLine("layer 3:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString());
                                //            var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                                //            var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                                //            var LowStr = "";
                                //            var HighStr = "";
                                //            long Low = 0;
                                //            long High = 0;
                                //            if (tempValue.Low_freq.Contains("GHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" GHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                                //            }
                                //            else if (tempValue.Low_freq.Contains("MHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" MHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                                //            }
                                //            else if (tempValue.Low_freq.Contains("kHz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" kHz");
                                //                LowStr = tempLow[0];

                                //                Low = (long)Math.Round(1000 * double.Parse(LowStr));
                                //            }

                                //            else if (tempValue.Low_freq.Contains("Hz"))
                                //            {
                                //                var tempLow = tempValue.Low_freq.Split(" Hz");
                                //                LowStr = tempLow[0];
                                //                Low = long.Parse(LowStr);
                                //            }

                                //            //Debug.WriteLine("high:" + tempValue.High_freq + ":low:" + tempValue.Low_freq);

                                //            if (tempValue.High_freq.Contains("GHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" GHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000000000;
                                //                High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                                //            }
                                //            else if (tempValue.High_freq.Contains("MHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" MHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000000;
                                //                High = (long)Math.Round(1000000 * double.Parse(HighStr));
                                //            }
                                //            else if (tempValue.High_freq.Contains("kHz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" kHz");
                                //                HighStr = tempHIgh[0];
                                //                //High = (long)double.Parse(HighStr) * 1000;
                                //                High = (long)Math.Round(1000 * double.Parse(HighStr));
                                //            }
                                //            else if (tempValue.High_freq.Contains("Hz"))
                                //            {
                                //                var tempHIgh = tempValue.High_freq.Split(" Hz");
                                //                HighStr = tempHIgh[0];
                                //                High = long.Parse(HighStr);
                                //            }

                                //            //Debug.WriteLine("low from doc:" + Low + "===" + ApplicationRange.low);
                                //            //Debug.WriteLine("high from doc:" + High + "===" + ApplicationRange.high+"::"+ApplicationTemp.ApplicationId+"::range::"+ApplicationTemp.ApplicationRangeId);

                                //            if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                                //            {
                                //                var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId
                                //                       && x.ApplicationTermId == ApplicationTemp.ApplicationTermId).SingleOrDefault();

                                //                if (tempValue.Type_of_doc == "R")
                                //                {

                                //                    //Debug.WriteLine("low is correct:" + Low);
                                //                    if (ApplicationTemp.documents_Id == null)
                                //                    {
                                //                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                        _conApp.Application.Update(ApplicationTemp);
                                //                        _conApp.SaveChanges();
                                //                    }


                                //                    if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                                //                    {

                                //                        if (ApplicationVal != null)
                                //                        {
                                //                            if (ApplicationVal.documents_Id == null)
                                //                            {
                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValue.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }

                                //                    }
                                //                }

                                //                if (tempValue.Type_of_doc == "I")
                                //                {

                                //                    //Debug.WriteLine("low is correct:" + Low);
                                //                    if (ApplicationTemp.documents_Id == null)
                                //                    {
                                //                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                        _conApp.Application.Update(ApplicationTemp);
                                //                        _conApp.SaveChanges();
                                //                    }


                                //                    if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                                //                    {

                                                        
                                //                        if (ApplicationVal != null)
                                //                        {
                                //                            if (ApplicationVal.documents_Id == null)
                                //                            {
                                //                                ApplicationDB app = new ApplicationDB()
                                //                                {
                                //                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                                //                                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                                //                                    comment = null,
                                //                                    documents_Id = tempValue.DocumentsId

                                //                                };

                                //                                _conApp.Application.Add(app);
                                //                                _conApp.SaveChanges();
                                //                            }
                                //                        }
                                //                        else
                                //                        {
                                //                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                                //                            _conApp.Application.Update(ApplicationTemp);
                                //                            _conApp.SaveChanges();
                                //                        }

                                //                    }
                                //                }

                                //            }
                                //        }

                                //    }
                                //}

                            }
                            //else
                            //{
                                
                            //    var documents = rdr["document"].ToString().Split(", ");

                            //    var ValuesDocuments = (from ww in _conApp.DocumentsDb
                            //                           select new
                            //                           {

                            //                               ww.DocumentsId,
                            //                               ww.Doc_number,
                            //                               ww.Title_of_doc,
                            //                               ww.Hyperlink,
                            //                               ww.Low_freq,
                            //                               ww.High_freq,
                            //                               ww.Application,
                            //                               ww.Type_of_doc

                            //                           }
                            //                                  ).ToList();

                            //    foreach (var tempDoc in documents)
                            //    {



                            //        var SingleDocuments = ValuesDocuments.Where(x => x.Doc_number.Contains(tempDoc) && x.Application.Equals(rdr["application"].ToString())
                            //         ).ToList();
                            //        if (SingleDocuments.Count == 0)
                            //        {
                            //            var root = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == int.Parse(rdr["ApplicationTermsDBId"].ToString())).FirstOrDefault();
                            //            if (root.Layer == 1)
                            //            {
                            //                var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == root.ApplicationTermsDBId).ToList();
                            //                var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                            //                var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                            //                var LowStr = "";
                            //                var HighStr = "";
                            //                long Low = 0;
                            //                long High = 0;
                            //                foreach (var tempRoot in valuesRoot)
                            //                {
                            //                    var SingleDocumentsLayerOne = ValuesDocuments.Where(x => x.Doc_number.Contains(tempDoc) && x.Application.Equals(tempRoot.name)).ToList();

                            //                    foreach (var tempValueLayerOne in SingleDocumentsLayerOne)
                            //                    {

                            //                        if (tempValueLayerOne.Low_freq.Contains("GHz"))
                            //                        {
                            //                            var tempLow = tempValueLayerOne.Low_freq.Split(" GHz");
                            //                            LowStr = tempLow[0];

                            //                            Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            //                        }
                            //                        else if (tempValueLayerOne.Low_freq.Contains("MHz"))
                            //                        {
                            //                            var tempLow = tempValueLayerOne.Low_freq.Split(" MHz");
                            //                            LowStr = tempLow[0];

                            //                            Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            //                        }
                            //                        else if (tempValueLayerOne.Low_freq.Contains("kHz"))
                            //                        {
                            //                            var tempLow = tempValueLayerOne.Low_freq.Split(" kHz");
                            //                            LowStr = tempLow[0];

                            //                            Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            //                        }

                            //                        else if (tempValueLayerOne.Low_freq.Contains("Hz"))
                            //                        {
                            //                            var tempLow = tempValueLayerOne.Low_freq.Split(" Hz");
                            //                            LowStr = tempLow[0];
                            //                            Low = long.Parse(LowStr);
                            //                        }


                            //                        if (tempValueLayerOne.High_freq.Contains("GHz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerOne.High_freq.Split(" GHz");
                            //                            HighStr = tempHIgh[0];
                            //                            //High = (long)double.Parse(HighStr) * 1000000000;
                            //                            High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            //                        }
                            //                        else if (tempValueLayerOne.High_freq.Contains("MHz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerOne.High_freq.Split(" MHz");
                            //                            HighStr = tempHIgh[0];
                            //                            //High = (long)double.Parse(HighStr) * 1000000;
                            //                            High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            //                        }
                            //                        else if (tempValueLayerOne.High_freq.Contains("kHz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerOne.High_freq.Split(" kHz");
                            //                            HighStr = tempHIgh[0];
                            //                            //High = (long)double.Parse(HighStr) * 1000;
                            //                            High = (long)Math.Round(1000 * double.Parse(HighStr));
                            //                        }
                            //                        else if (tempValueLayerOne.High_freq.Contains("Hz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerOne.High_freq.Split(" Hz");
                            //                            HighStr = tempHIgh[0];
                            //                            High = long.Parse(HighStr);
                            //                        }

                            //                        if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            //                        {
                            //                            var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerOne.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                               && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                            //                            if (tempValueLayerOne.Type_of_doc == "R")
                            //                            {
                            //                                if (ApplicationTempNew != null)
                            //                                {

                            //                                    if (ApplicationTempNew.documents_Id == null)
                            //                                    {
                            //                                        //if (root.ApplicationTermsDBId == 152)
                            //                                        //{
                            //                                        //    Debug.WriteLine(" it is null:low freq:" + ApplicationRange.LowView + "values of docs:" + tempValue.DocumentsId);
                            //                                        //}
                            //                                        ApplicationTempNew.documents_Id = tempValueLayerOne.DocumentsId;
                            //                                        _conApp.Application.Update(ApplicationTempNew);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                    else if (ApplicationTempNew.documents_Id != tempValueLayerOne.DocumentsId)
                            //                                    {
                                                                   
                            //                                        ApplicationDB app = new ApplicationDB()
                            //                                        {
                            //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                            comment = null,
                            //                                            documents_Id = tempValueLayerOne.DocumentsId

                            //                                        };

                            //                                        _conApp.Application.Add(app);
                            //                                        _conApp.SaveChanges();

                            //                                    }
                            //                                }
                            //                                else
                            //                                {
                                                                
                            //                                    var queryDocument = (from all in _conApp.Application
                            //                                                         join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                         where e.Doc_number.Equals(tempValueLayerOne.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                         && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                         select new
                            //                                                         {
                            //                                                             app_id = all.ApplicationId,
                            //                                                             doc_id = all.documents_Id
                            //                                                         }
                            //                                                         ).ToList();
                            //                                    if (queryDocument.Count == 0)
                            //                                    {
                            //                                        //Debug.WriteLine("pp:");                                                                                 
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                            //                                            ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                        else
                            //                                        {
                                                                        
                            //                                            ApplicationTemp.ApplicationId = 0;
                            //                                            ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                            //                                            _conApp.Application.Add(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }

                            //                                    }
                            //                                    else
                            //                                    {
                                                                    
                            //                                    }

                            //                                }


                            //                            }

                            //                            if (tempValueLayerOne.Type_of_doc == "I")
                            //                            {
                            //                                if (ApplicationTempNew != null)
                            //                                {
                            //                                    if (ApplicationTempNew.documents_Id == null)
                            //                                    {
                            //                                        ApplicationTempNew.documents_Id = tempValueLayerOne.DocumentsId;
                            //                                        _conApp.Application.Update(ApplicationTempNew);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                    else if (ApplicationTempNew.documents_Id != tempValueLayerOne.DocumentsId)
                            //                                    {

                            //                                        ApplicationDB app = new ApplicationDB()
                            //                                        {
                            //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                            comment = null,
                            //                                            documents_Id = tempValueLayerOne.DocumentsId

                            //                                        };

                            //                                        _conApp.Application.Add(app);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                }
                            //                                else
                            //                                {
                                                                

                            //                                    var queryDocument = (from all in _conApp.Application
                            //                                                         join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                         where e.Doc_number.Equals(tempValueLayerOne.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                         && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                         select new
                            //                                                         {
                            //                                                             app_id = all.ApplicationId,
                            //                                                             doc_id = all.documents_Id
                            //                                                         }
                            //                                                        ).ToList();
                            //                                    if (queryDocument.Count == 0)
                            //                                    {
                            //                                        //Debug.WriteLine("pp:");                                                                                 
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                            //                                            ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                        else
                            //                                        {
                            //                                            // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                            //                                            //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                            //                                            //    + "===second doc:" + tempValue.Doc_number);
                            //                                            ApplicationTemp.ApplicationId = 0;
                            //                                            ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                            //                                            _conApp.Application.Add(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }

                            //                                    }
                            //                                    else
                            //                                    {
                                                                   
                            //                                    }
                            //                                }

                            //                            }

                            //                            //previous code
                            //                            //if (tempValueLayerOne.Type_of_doc == "R")
                            //                            //{

                            //                            //    if (ApplicationTemp.documents_Id == null)
                            //                            //    {
                            //                            //        ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                            //                            //        _conApp.Application.Update(ApplicationTemp);
                            //                            //        _conApp.SaveChanges();
                            //                            //    }
                            //                            //    else if (ApplicationTemp.documents_Id != tempValueLayerOne.DocumentsId)
                            //                            //    {
                            //                            //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValueLayerOne.DocumentsId).SingleOrDefault();
                            //                            //        if (ApplicationVal != null)
                            //                            //        {
                            //                            //            if (ApplicationVal.documents_Id == null)
                            //                            //            {
                            //                            //                ApplicationDB app = new ApplicationDB()
                            //                            //                {
                            //                            //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                            //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                            //                    comment = null,
                            //                            //                    documents_Id = tempValueLayerOne.DocumentsId

                            //                            //                };

                            //                            //                _conApp.Application.Add(app);
                            //                            //                _conApp.SaveChanges();
                            //                            //            }
                            //                            //        }
                            //                            //    }
                            //                            //}

                            //                            //if (tempValueLayerOne.Type_of_doc == "I")
                            //                            //{
                            //                            //    if (ApplicationTemp.documents_Id == null)
                            //                            //    {
                            //                            //        ApplicationTemp.documents_Id = tempValueLayerOne.DocumentsId;
                            //                            //        _conApp.Application.Update(ApplicationTemp);
                            //                            //        _conApp.SaveChanges();
                            //                            //    }
                            //                            //    else if (ApplicationTemp.documents_Id != tempValueLayerOne.DocumentsId)
                            //                            //    {
                            //                            //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValueLayerOne.DocumentsId).SingleOrDefault();
                            //                            //        if (ApplicationVal != null)
                            //                            //        {
                            //                            //            if (ApplicationVal.documents_Id == null)
                            //                            //            {
                            //                            //                ApplicationDB app = new ApplicationDB()
                            //                            //                {
                            //                            //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                            //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                            //                    comment = null,
                            //                            //                    documents_Id = tempValueLayerOne.DocumentsId

                            //                            //                };

                            //                            //                _conApp.Application.Add(app);
                            //                            //                _conApp.SaveChanges();
                            //                            //            }
                            //                            //        }
                            //                            //    }
                            //                            //}

                            //                        }


                            //                    }
                            //                }


                            //            }
                            //            else if (root.Layer == 2)
                            //            {
                            //                var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == root.ApplicationTermsDBId).ToList();
                            //                var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                            //                var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                            //                var LowStr = "";
                            //                var HighStr = "";
                            //                long Low = 0;
                            //                long High = 0;
                            //                foreach (var tempRoot in valuesRoot)
                            //                {
                            //                    var SingleDocumentsLayerTwo = ValuesDocuments.Where(x => x.Doc_number.Contains(tempDoc) && x.Application.Equals(tempRoot.name)).ToList();

                            //                    foreach (var tempValueLayerTwo in SingleDocumentsLayerTwo)
                            //                    {

                            //                        if (tempValueLayerTwo.Low_freq.Contains("GHz"))
                            //                        {
                            //                            var tempLow = tempValueLayerTwo.Low_freq.Split(" GHz");
                            //                            LowStr = tempLow[0];

                            //                            Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            //                        }
                            //                        else if (tempValueLayerTwo.Low_freq.Contains("MHz"))
                            //                        {
                            //                            var tempLow = tempValueLayerTwo.Low_freq.Split(" MHz");
                            //                            LowStr = tempLow[0];

                            //                            Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            //                        }
                            //                        else if (tempValueLayerTwo.Low_freq.Contains("kHz"))
                            //                        {
                            //                            var tempLow = tempValueLayerTwo.Low_freq.Split(" kHz");
                            //                            LowStr = tempLow[0];

                            //                            Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            //                        }

                            //                        else if (tempValueLayerTwo.Low_freq.Contains("Hz"))
                            //                        {
                            //                            var tempLow = tempValueLayerTwo.Low_freq.Split(" Hz");
                            //                            LowStr = tempLow[0];
                            //                            Low = long.Parse(LowStr);
                            //                        }


                            //                        if (tempValueLayerTwo.High_freq.Contains("GHz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerTwo.High_freq.Split(" GHz");
                            //                            HighStr = tempHIgh[0];
                            //                            //High = (long)double.Parse(HighStr) * 1000000000;
                            //                            High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            //                        }
                            //                        else if (tempValueLayerTwo.High_freq.Contains("MHz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerTwo.High_freq.Split(" MHz");
                            //                            HighStr = tempHIgh[0];
                            //                            //High = (long)double.Parse(HighStr) * 1000000;
                            //                            High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            //                        }
                            //                        else if (tempValueLayerTwo.High_freq.Contains("kHz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerTwo.High_freq.Split(" kHz");
                            //                            HighStr = tempHIgh[0];
                            //                            //High = (long)double.Parse(HighStr) * 1000;
                            //                            High = (long)Math.Round(1000 * double.Parse(HighStr));
                            //                        }
                            //                        else if (tempValueLayerTwo.High_freq.Contains("Hz"))
                            //                        {
                            //                            var tempHIgh = tempValueLayerTwo.High_freq.Split(" Hz");
                            //                            HighStr = tempHIgh[0];
                            //                            High = long.Parse(HighStr);
                            //                        }

                            //                        if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            //                        {
                            //                            var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValueLayerTwo.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                               && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                            //                            if (tempValueLayerTwo.Type_of_doc == "R")
                            //                            {
                            //                                if (ApplicationTempNew != null)
                            //                                {

                            //                                    if (ApplicationTempNew.documents_Id == null)
                            //                                    {
                            //                                        //Debug.WriteLine("www:" + ApplicationRange.LowView + "==" + tempValueLayerTwo.DocumentsId);
                            //                                        ApplicationTempNew.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                                        _conApp.Application.Update(ApplicationTempNew);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                    else if (ApplicationTempNew.documents_Id != tempValueLayerTwo.DocumentsId)
                            //                                    {

                            //                                        ApplicationDB app = new ApplicationDB()
                            //                                        {
                            //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                            comment = null,
                            //                                            documents_Id = tempValueLayerTwo.DocumentsId

                            //                                        };

                            //                                        _conApp.Application.Add(app);
                            //                                        _conApp.SaveChanges();

                            //                                    }
                            //                                }
                            //                                else
                            //                                {

                            //                                    var queryDocument = (from all in _conApp.Application
                            //                                                         join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                         where e.Doc_number.Equals(tempValueLayerTwo.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                         && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                         select new
                            //                                                         {
                            //                                                             app_id = all.ApplicationId,
                            //                                                             doc_id = all.documents_Id
                            //                                                         }
                            //                                                         ).ToList();
                            //                                    if (queryDocument.Count == 0)
                            //                                    {
                            //                                        //Debug.WriteLine("pp:");
                            //                                        //var appName = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                                                                    
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            var appName = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                            //                                            if (appName.name.Equals(tempValueLayerTwo.Application))
                            //                                            {
                            //                                               // Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + ttDoc.DocumentsId + "===" + tempValue.DocumentsId);
                            //                                                ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                                                _conApp.Application.Update(ApplicationTemp);
                            //                                                _conApp.SaveChanges();

                            //                                            }
                            //                                            else
                            //                                            {
                            //                                                if(appName.Layer == 1)
                            //                                                {
                            //                                                    var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.first_up_lvl_id == appName.ApplicationTermsDBId).SingleOrDefault();
                            //                                                    ApplicationDB newInsertApp = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                            //                                                        documents_Id = tempValueLayerTwo.DocumentsId,
                            //                                                        comment = ApplicationTemp.comment,
                            //                                                    };
                            //                                                    _conApp.Application.Add(newInsertApp);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                                else if(appName.Layer == 2)
                            //                                                {
                            //                                                    var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.second_up_lvl_id == appName.ApplicationTermsDBId).SingleOrDefault();
                            //                                                    ApplicationDB newInsertApp = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                            //                                                        documents_Id = tempValueLayerTwo.DocumentsId,
                            //                                                        comment = ApplicationTemp.comment,
                            //                                                    };
                            //                                                    _conApp.Application.Add(newInsertApp);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                                                                            

                            //                                                //_conApp.Application.Add(ApplicationTemp);
                            //                                                //_conApp.SaveChanges();
                            //                                            }


                                                                       
                            //                                        }
                            //                                        else
                            //                                        {

                            //                                            //Debug.WriteLine("sss");
                            //                                            var appName = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                            //                                            if (appName.name.Equals(tempValueLayerTwo.Application))
                            //                                            {
                            //                                                // Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + ttDoc.DocumentsId + "===" + tempValue.DocumentsId);
                            //                                                ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                                                _conApp.Application.Update(ApplicationTemp);
                            //                                                _conApp.SaveChanges();

                            //                                            }
                            //                                            else
                            //                                            {
                            //                                                if (appName.Layer == 1)
                            //                                                {
                            //                                                    var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.first_up_lvl_id == appName.ApplicationTermsDBId).SingleOrDefault();
                            //                                                    ApplicationDB newInsertApp = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                            //                                                        documents_Id = tempValueLayerTwo.DocumentsId,
                            //                                                        comment = null,
                            //                                                    };
                            //                                                    _conApp.Application.Add(newInsertApp);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                                else if (appName.Layer == 2)
                            //                                                {
                            //                                                    var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(tempValueLayerTwo.Application) && x.second_up_lvl_id == appName.ApplicationTermsDBId).SingleOrDefault();
                            //                                                    ApplicationDB newInsertApp = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                        ApplicationTermId = appTerm.ApplicationTermsDBId,
                            //                                                        documents_Id = tempValueLayerTwo.DocumentsId,
                            //                                                        comment = null,
                            //                                                    };
                            //                                                    _conApp.Application.Add(newInsertApp);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                            }
                            //                                        }

                            //                                    }
                            //                                    else
                            //                                    {

                            //                                    }

                            //                                }


                            //                            }

                            //                            if (tempValueLayerTwo.Type_of_doc == "I")
                            //                            {
                            //                                if (ApplicationTempNew != null)
                            //                                {
                            //                                    if (ApplicationTempNew.documents_Id == null)
                            //                                    {
                            //                                        ApplicationTempNew.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                                        _conApp.Application.Update(ApplicationTempNew);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                    else if (ApplicationTempNew.documents_Id != tempValueLayerTwo.DocumentsId)
                            //                                    {

                            //                                        ApplicationDB app = new ApplicationDB()
                            //                                        {
                            //                                            ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                            ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                            comment = null,
                            //                                            documents_Id = tempValueLayerTwo.DocumentsId

                            //                                        };

                            //                                        _conApp.Application.Add(app);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                }
                            //                                else
                            //                                {


                            //                                    var queryDocument = (from all in _conApp.Application
                            //                                                         join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                         where e.Doc_number.Equals(tempValueLayerTwo.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                         && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                         select new
                            //                                                         {
                            //                                                             app_id = all.ApplicationId,
                            //                                                             doc_id = all.documents_Id
                            //                                                         }
                            //                                                        ).ToList();
                            //                                    if (queryDocument.Count == 0)
                            //                                    {
                            //                                        //Debug.WriteLine("pp:");                                                                                 
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                            //                                            ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                        else
                            //                                        {
                            //                                            Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValueLayerTwo.DocumentsId);

                            //                                            Debug.WriteLine("insert into:" + tempValueLayerTwo.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                            //                                                + "===second doc:" + tempValueLayerTwo.Doc_number);
                            //                                            ApplicationTemp.ApplicationId = 0;
                            //                                            ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                                            _conApp.Application.Add(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }

                            //                                    }
                            //                                    else
                            //                                    {

                            //                                    }
                            //                                }

                            //                            }
                            //                            //previous code
                            //                            //if (tempValueLayerTwo.Type_of_doc == "R")
                            //                            //{
                            //                            //    if (ApplicationTemp.documents_Id == null)
                            //                            //    {
                            //                            //        ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                            //        _conApp.Application.Update(ApplicationTemp);
                            //                            //        _conApp.SaveChanges();
                            //                            //    }
                            //                            //    else if (ApplicationTemp.documents_Id != tempValueLayerTwo.DocumentsId)
                            //                            //    {
                            //                            //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValueLayerTwo.DocumentsId).SingleOrDefault();
                            //                            //        if (ApplicationVal != null)
                            //                            //        {
                            //                            //            if (ApplicationVal.documents_Id == null)
                            //                            //            {
                            //                            //                ApplicationDB app = new ApplicationDB()
                            //                            //                {
                            //                            //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                            //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                            //                    comment = null,
                            //                            //                    documents_Id = tempValueLayerTwo.DocumentsId

                            //                            //                };

                            //                            //                _conApp.Application.Add(app);
                            //                            //                _conApp.SaveChanges();
                            //                            //            }
                            //                            //        }
                            //                            //    }
                            //                            //}

                            //                            //if (tempValueLayerTwo.Type_of_doc == "I")
                            //                            //{
                            //                            //    if (ApplicationTemp.documents_Id == null)
                            //                            //    {
                            //                            //        ApplicationTemp.documents_Id = tempValueLayerTwo.DocumentsId;
                            //                            //        _conApp.Application.Update(ApplicationTemp);
                            //                            //        _conApp.SaveChanges();
                            //                            //    }
                            //                            //    else if (ApplicationTemp.documents_Id != tempValueLayerTwo.DocumentsId)
                            //                            //    {
                            //                            //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValueLayerTwo.DocumentsId).SingleOrDefault();
                            //                            //        if (ApplicationVal != null)
                            //                            //        {
                            //                            //            if (ApplicationVal.documents_Id == null)
                            //                            //            {
                            //                            //                ApplicationDB app = new ApplicationDB()
                            //                            //                {
                            //                            //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                            //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                            //                    comment = null,
                            //                            //                    documents_Id = tempValueLayerTwo.DocumentsId

                            //                            //                };

                            //                            //                _conApp.Application.Add(app);
                            //                            //                _conApp.SaveChanges();
                            //                            //            }
                            //                            //        }
                            //                            //    }
                            //                            //}

                            //                        }


                            //                    }
                            //                }
                            //            }

                            //        }
                            //        else if (SingleDocuments.Count > 0)
                            //        {

                            //            foreach (var tempValue in SingleDocuments)
                            //            {

                            //                //Debug.WriteLine("ttt:" + tempValue.Doc_number);
                            //                var root = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == int.Parse(rdr["ApplicationTermsDBId"].ToString())).FirstOrDefault();

                            //                if (root.Layer == 1)
                            //                {
                            //                    // Debug.WriteLine("layer 1:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString()+":app name:"+root.name);
                            //                    var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == root.ApplicationTermsDBId).ToList();
                            //                    var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                            //                    var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                            //                    var LowStr = "";
                            //                    var HighStr = "";
                            //                    long Low = 0;
                            //                    long High = 0;

                            //                    if (tempValue.Low_freq.Contains("GHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" GHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            //                    }
                            //                    else if (tempValue.Low_freq.Contains("MHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" MHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            //                    }
                            //                    else if (tempValue.Low_freq.Contains("kHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" kHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            //                    }

                            //                    else if (tempValue.Low_freq.Contains("Hz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" Hz");
                            //                        LowStr = tempLow[0];
                            //                        Low = long.Parse(LowStr);
                            //                    }


                            //                    if (tempValue.High_freq.Contains("GHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" GHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000000000;
                            //                        High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            //                    }
                            //                    else if (tempValue.High_freq.Contains("MHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" MHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000000;
                            //                        High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            //                    }
                            //                    else if (tempValue.High_freq.Contains("kHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" kHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000;
                            //                        High = (long)Math.Round(1000 * double.Parse(HighStr));
                            //                    }
                            //                    else if (tempValue.High_freq.Contains("Hz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" Hz");
                            //                        HighStr = tempHIgh[0];
                            //                        High = long.Parse(HighStr);
                            //                    }

                            //                    if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            //                    {
                            //                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                              && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                            //                        if (tempValue.Type_of_doc == "R")
                            //                        {
                            //                            if (ApplicationTempNew != null)
                            //                            {

                            //                                if (ApplicationTempNew.documents_Id == null)
                            //                                {

                            //                                    ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                            //                                    _conApp.Application.Update(ApplicationTempNew);
                            //                                    _conApp.SaveChanges();
                            //                                }
                            //                                else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                            //                                {

                            //                                    ApplicationDB app = new ApplicationDB()
                            //                                    {
                            //                                        ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                        ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                        comment = null,
                            //                                        documents_Id = tempValue.DocumentsId

                            //                                    };

                            //                                    _conApp.Application.Add(app);
                            //                                    _conApp.SaveChanges();

                            //                                }
                            //                            }
                            //                            else
                            //                            {

                            //                                var queryDocument = (from all in _conApp.Application
                            //                                                     join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                     where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                     && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                     select new
                            //                                                     {
                            //                                                         app_id = all.ApplicationId,
                            //                                                         doc_id = all.documents_Id
                            //                                                     }
                            //                                                     ).ToList();
                            //                                if (queryDocument.Count == 0)
                            //                                {
                            //                                    //Debug.WriteLine("pp:");                                                                                 
                            //                                    if (ApplicationTemp.documents_Id == null)
                            //                                    {
                            //                                        //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                            //                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                        _conApp.Application.Update(ApplicationTemp);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                    else
                            //                                    {

                            //                                        ApplicationTemp.ApplicationId = 0;
                            //                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                        _conApp.Application.Add(ApplicationTemp);
                            //                                        _conApp.SaveChanges();
                            //                                    }

                            //                                }
                            //                                else
                            //                                {

                            //                                }

                            //                            }


                            //                        }

                            //                        if (tempValue.Type_of_doc == "I")
                            //                        {
                            //                            if (ApplicationTempNew != null)
                            //                            {
                            //                                if (ApplicationTempNew.documents_Id == null)
                            //                                {
                            //                                    ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                            //                                    _conApp.Application.Update(ApplicationTempNew);
                            //                                    _conApp.SaveChanges();
                            //                                }
                            //                                else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                            //                                {

                            //                                    ApplicationDB app = new ApplicationDB()
                            //                                    {
                            //                                        ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                        ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                        comment = null,
                            //                                        documents_Id = tempValue.DocumentsId

                            //                                    };

                            //                                    _conApp.Application.Add(app);
                            //                                    _conApp.SaveChanges();
                            //                                }
                            //                            }
                            //                            else
                            //                            {


                            //                                var queryDocument = (from all in _conApp.Application
                            //                                                     join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                     where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                     && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                     select new
                            //                                                     {
                            //                                                         app_id = all.ApplicationId,
                            //                                                         doc_id = all.documents_Id
                            //                                                     }
                            //                                                    ).ToList();
                            //                                if (queryDocument.Count == 0)
                            //                                {
                            //                                    //Debug.WriteLine("pp:");                                                                                 
                            //                                    if (ApplicationTemp.documents_Id == null)
                            //                                    {
                            //                                        //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                            //                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                        _conApp.Application.Update(ApplicationTemp);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                    else
                            //                                    {
                            //                                        // Debug.WriteLine("it is simple insert:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);

                            //                                        //Debug.WriteLine("insert into:" + tempValue.DocumentsId + "::" + ApplicationTemp.ApplicationId + "==" + ApplicationTemp.ApplicationRangeId + ":first doc:"
                            //                                        //    + "===second doc:" + tempValue.Doc_number);
                            //                                        ApplicationTemp.ApplicationId = 0;
                            //                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                        _conApp.Application.Add(ApplicationTemp);
                            //                                        _conApp.SaveChanges();
                            //                                    }

                            //                                }
                            //                                else
                            //                                {

                            //                                }
                            //                            }

                            //                        }

                            //                        //previous code
                            //                        //if (tempValue.Type_of_doc == "R")
                            //                        //{
                            //                        //    if (ApplicationTemp.documents_Id == null)
                            //                        //    {
                            //                        //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                        //        _conApp.Application.Update(ApplicationTemp);
                            //                        //        _conApp.SaveChanges();
                            //                        //    }
                            //                        //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //                        //    {
                            //                        //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //                        //        if (ApplicationVal != null)
                            //                        //        {
                            //                        //            if (ApplicationVal.documents_Id == null)
                            //                        //            {
                            //                        //                ApplicationDB app = new ApplicationDB()
                            //                        //                {
                            //                        //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                        //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                        //                    comment = null,
                            //                        //                    documents_Id = tempValue.DocumentsId

                            //                        //                };

                            //                        //                _conApp.Application.Add(app);
                            //                        //                _conApp.SaveChanges();
                            //                        //            }
                            //                        //        }
                            //                        //    }
                            //                        //}

                            //                        //if (tempValue.Type_of_doc == "I")
                            //                        //{
                            //                        //    if (ApplicationTemp.documents_Id == null)
                            //                        //    {
                            //                        //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                        //        _conApp.Application.Update(ApplicationTemp);
                            //                        //        _conApp.SaveChanges();
                            //                        //    }
                            //                        //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //                        //    {
                            //                        //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //                        //        if (ApplicationVal != null)
                            //                        //        {
                            //                        //            if (ApplicationVal.documents_Id == null)
                            //                        //            {
                            //                        //                ApplicationDB app = new ApplicationDB()
                            //                        //                {
                            //                        //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                        //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                        //                    comment = null,
                            //                        //                    documents_Id = tempValue.DocumentsId

                            //                        //                };

                            //                        //                _conApp.Application.Add(app);
                            //                        //                _conApp.SaveChanges();
                            //                        //            }
                            //                        //        }
                            //                        //    }
                            //                        //}

                            //                    }

                            //                    foreach (var temp in valuesRoot)
                            //                    {

                            //                        // Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                            //                        var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                            //                        ).ToList();

                            //                        foreach (var ttDoc in docTemp)
                            //                        {
                            //                            //Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                            //                            var LowStrDoc = "";
                            //                            var HighStrDoc = "";
                            //                            long LowDoc = 0;
                            //                            long HighDoc = 0;
                            //                            if (ttDoc != null)
                            //                            {
                            //                                if (ttDoc.Low_freq.Contains("GHz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" GHz");
                            //                                    LowStrDoc = tempLow[0];

                            //                                    LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                            //                                }
                            //                                else if (ttDoc.Low_freq.Contains("MHz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" MHz");
                            //                                    LowStrDoc = tempLow[0];

                            //                                    LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                            //                                }
                            //                                else if (ttDoc.Low_freq.Contains("kHz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" kHz");
                            //                                    LowStrDoc = tempLow[0];

                            //                                    LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                            //                                }

                            //                                else if (ttDoc.Low_freq.Contains("Hz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" Hz");
                            //                                    LowStrDoc = tempLow[0];
                            //                                    LowDoc = long.Parse(LowStrDoc);
                            //                                }


                            //                                if (ttDoc.High_freq.Contains("GHz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" GHz");
                            //                                    HighStrDoc = tempHIgh[0];
                            //                                    //High = (long)double.Parse(HighStr) * 1000000000;
                            //                                    HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                            //                                }
                            //                                else if (ttDoc.High_freq.Contains("MHz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" MHz");
                            //                                    HighStrDoc = tempHIgh[0];
                            //                                    //High = (long)double.Parse(HighStr) * 1000000;
                            //                                    HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                            //                                }
                            //                                else if (ttDoc.High_freq.Contains("kHz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" kHz");
                            //                                    HighStrDoc = tempHIgh[0];

                            //                                    //High = (long)double.Parse(HighStr) * 1000;
                            //                                    HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                            //                                }
                            //                                else if (ttDoc.High_freq.Contains("Hz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" Hz");
                            //                                    HighStrDoc = tempHIgh[0];
                            //                                    HighDoc = long.Parse(HighStrDoc);
                            //                                }

                            //                                if ((ApplicationRange.low >= LowDoc && ApplicationRange.low <= HighDoc) || (ApplicationRange.high >= LowDoc && ApplicationRange.high <= HighDoc))
                            //                                {
                            //                                    if (ttDoc.Type_of_doc == "R")
                            //                                    {
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                        else if (ApplicationTemp.documents_Id != ttDoc.DocumentsId)
                            //                                        {
                            //                                            var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                            //                                            if (ApplicationVal != null)
                            //                                            {
                            //                                                if (ApplicationVal.documents_Id == null)
                            //                                                {
                            //                                                    ApplicationDB app = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                        ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                                                        comment = null,
                            //                                                        documents_Id = ttDoc.DocumentsId

                            //                                                    };

                            //                                                    _conApp.Application.Add(app);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                            }
                            //                                        }

                            //                                    }

                            //                                    if (ttDoc.Type_of_doc == "I")
                            //                                    {
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                        else if (ApplicationTemp.documents_Id != ttDoc.DocumentsId)
                            //                                        {
                            //                                            var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                            //                                            if (ApplicationVal != null)
                            //                                            {
                            //                                                if (ApplicationVal.documents_Id == null)
                            //                                                {
                            //                                                    ApplicationDB app = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                        ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                                                        comment = null,
                            //                                                        documents_Id = ttDoc.DocumentsId

                            //                                                    };

                            //                                                    _conApp.Application.Add(app);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                            }
                            //                                        }

                            //                                    }
                            //                                }
                            //                            }
                            //                        }

                            //                    }
                            //                }
                            //                else if (root.Layer == 2)
                            //                {
                            //                    // Debug.WriteLine("layer 2:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString());

                            //                    var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == root.ApplicationTermsDBId).ToList();
                            //                    var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                            //                    var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                            //                    var LowStr = "";
                            //                    var HighStr = "";
                            //                    long Low = 0;
                            //                    long High = 0;
                            //                    if (tempValue.Low_freq.Contains("GHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" GHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            //                    }
                            //                    else if (tempValue.Low_freq.Contains("MHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" MHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            //                    }
                            //                    else if (tempValue.Low_freq.Contains("kHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" kHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            //                    }

                            //                    else if (tempValue.Low_freq.Contains("Hz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" Hz");
                            //                        LowStr = tempLow[0];
                            //                        Low = long.Parse(LowStr);
                            //                    }


                            //                    if (tempValue.High_freq.Contains("GHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" GHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000000000;
                            //                        High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            //                    }
                            //                    else if (tempValue.High_freq.Contains("MHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" MHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000000;
                            //                        High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            //                    }
                            //                    else if (tempValue.High_freq.Contains("kHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" kHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000;
                            //                        High = (long)Math.Round(1000 * double.Parse(HighStr));
                            //                    }
                            //                    else if (tempValue.High_freq.Contains("Hz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" Hz");
                            //                        HighStr = tempHIgh[0];
                            //                        High = long.Parse(HighStr);
                            //                    }

                            //                    if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            //                    {
                            //                        var ApplicationTempNew = _conApp.Application.Where(x => x.documents_Id == tempValue.DocumentsId && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                              && x.ApplicationTermId == root.ApplicationTermsDBId).SingleOrDefault();
                            //                        if (tempValue.Type_of_doc == "R")
                            //                        {
                            //                            if (ApplicationTempNew != null)
                            //                            {

                            //                                if (ApplicationTempNew.documents_Id == null)
                            //                                {
                            //                                    Debug.WriteLine("testiranje:" + ApplicationRange.LowView + "===" + tempValue.DocumentsId);
                            //                                    ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                            //                                    _conApp.Application.Update(ApplicationTempNew);
                            //                                    _conApp.SaveChanges();
                            //                                }
                            //                                else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                            //                                {

                            //                                    ApplicationDB app = new ApplicationDB()
                            //                                    {
                            //                                        ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                        ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                        comment = null,
                            //                                        documents_Id = tempValue.DocumentsId

                            //                                    };

                            //                                    _conApp.Application.Add(app);
                            //                                    _conApp.SaveChanges();

                            //                                }
                            //                            }
                            //                            else
                            //                            {

                            //                                var queryDocument = (from all in _conApp.Application
                            //                                                     join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                     where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                     && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                     select new
                            //                                                     {
                            //                                                         app_id = all.ApplicationId,
                            //                                                         doc_id = all.documents_Id
                            //                                                     }
                            //                                                     ).ToList();
                            //                                if (queryDocument.Count == 0)
                            //                                {
                            //                                    //Debug.WriteLine("pp:");                                                                                 
                            //                                    if (ApplicationTemp.documents_Id == null)
                            //                                    {
                            //                                        var appName = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                            //                                        if (appName.name.Equals(tempValue.Application))
                            //                                        {
                            //                                            Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId+"==="+ appName.name+":::"+tempValue.Application);
                            //                                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                    }
                            //                                    else
                            //                                    {
                            //                                        //nije ovdje
                            //                                       // Debug.WriteLine("insert :low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId + "===" + ApplicationTemp.ApplicationTermId + ":::" + tempValue.Application);
                            //                                        ApplicationTemp.ApplicationId = 0;
                            //                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                        _conApp.Application.Add(ApplicationTemp);
                            //                                        _conApp.SaveChanges();
                            //                                    }

                            //                                }
                            //                                else
                            //                                {

                            //                                }

                            //                            }


                            //                        }

                            //                        if (tempValue.Type_of_doc == "I")
                            //                        {
                            //                            if (ApplicationTempNew != null)
                            //                            {
                            //                                if (ApplicationTempNew.documents_Id == null)
                            //                                {
                            //                                    ApplicationTempNew.documents_Id = tempValue.DocumentsId;
                            //                                    _conApp.Application.Update(ApplicationTempNew);
                            //                                    _conApp.SaveChanges();
                            //                                }
                            //                                else if (ApplicationTempNew.documents_Id != tempValue.DocumentsId)
                            //                                {

                            //                                    ApplicationDB app = new ApplicationDB()
                            //                                    {
                            //                                        ApplicationRangeId = ApplicationTempNew.ApplicationRangeId,
                            //                                        ApplicationTermId = ApplicationTempNew.ApplicationTermId,
                            //                                        comment = null,
                            //                                        documents_Id = tempValue.DocumentsId

                            //                                    };

                            //                                    _conApp.Application.Add(app);
                            //                                    _conApp.SaveChanges();
                            //                                }
                            //                            }
                            //                            else
                            //                            {


                            //                                var queryDocument = (from all in _conApp.Application
                            //                                                     join e in _conApp.DocumentsDb on all.documents_Id equals e.DocumentsId
                            //                                                     where e.Doc_number.Equals(tempValue.Doc_number) && all.ApplicationRangeId == ApplicationRange.ApplicationRangeId
                            //                                                     && all.ApplicationTermId == ApplicationTemp.ApplicationTermId
                            //                                                     select new
                            //                                                     {
                            //                                                         app_id = all.ApplicationId,
                            //                                                         doc_id = all.documents_Id
                            //                                                     }
                            //                                                    ).ToList();
                            //                                if (queryDocument.Count == 0)
                            //                                {
                            //                                    //Debug.WriteLine("pp:");                                                                                 
                            //                                    if (ApplicationTemp.documents_Id == null)
                            //                                    {
                            //                                        var appName = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                            //                                        if (appName.name.Equals(tempValue.Application))
                            //                                        {
                            //                                            //Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + tempValue.DocumentsId);
                            //                                            ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                    }
                            //                                    else
                            //                                    {
                                                                    
                            //                                        ApplicationTemp.ApplicationId = 0;
                            //                                        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                        _conApp.Application.Add(ApplicationTemp);
                            //                                        _conApp.SaveChanges();
                            //                                    }

                            //                                }
                            //                                else
                            //                                {

                            //                                }
                            //                            }

                            //                        }
                            //                        //previous code
                            //                        //if (tempValue.Type_of_doc == "R")
                            //                        //{
                            //                        //    if (ApplicationTemp.documents_Id == null)
                            //                        //    {
                            //                        //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                        //        _conApp.Application.Update(ApplicationTemp);
                            //                        //        _conApp.SaveChanges();
                            //                        //    }
                            //                        //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //                        //    {

                            //                        //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //                        //        if (ApplicationVal != null)
                            //                        //        {
                            //                        //            if (ApplicationVal.documents_Id == null)
                            //                        //            {
                            //                        //                ApplicationDB app = new ApplicationDB()
                            //                        //                {
                            //                        //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                        //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                        //                    comment = null,
                            //                        //                    documents_Id = tempValue.DocumentsId

                            //                        //                };

                            //                        //                _conApp.Application.Add(app);
                            //                        //                _conApp.SaveChanges();
                            //                        //            }
                            //                        //        }

                            //                        //    }

                            //                        //}

                            //                        //if (tempValue.Type_of_doc == "I")
                            //                        //{
                            //                        //    if (ApplicationTemp.documents_Id == null)
                            //                        //    {
                            //                        //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                        //        _conApp.Application.Update(ApplicationTemp);
                            //                        //        _conApp.SaveChanges();
                            //                        //    }
                            //                        //    else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //                        //    {
                            //                        //        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //                        //        if (ApplicationVal != null)
                            //                        //        {
                            //                        //            if (ApplicationVal.documents_Id == null)
                            //                        //            {
                            //                        //                ApplicationDB app = new ApplicationDB()
                            //                        //                {
                            //                        //                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                        //                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                        //                    comment = null,
                            //                        //                    documents_Id = tempValue.DocumentsId

                            //                        //                };

                            //                        //                _conApp.Application.Add(app);
                            //                        //                _conApp.SaveChanges();
                            //                        //            }
                            //                        //        }
                            //                        //    }

                            //                        //}


                            //                    }


                            //                    foreach (var temp in valuesRoot)
                            //                    {

                            //                        //Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                            //                        var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                            //                        ).ToList();

                            //                        foreach (var ttDoc in docTemp)
                            //                        {
                            //                            if (ttDoc != null)
                            //                            {
                            //                                Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                            //                                var LowStrDoc = "";
                            //                                var HighStrDoc = "";
                            //                                long LowDoc = 0;
                            //                                long HighDoc = 0;
                            //                                if (ttDoc.Low_freq.Contains("GHz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" GHz");
                            //                                    LowStrDoc = tempLow[0];

                            //                                    LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                            //                                }
                            //                                else if (ttDoc.Low_freq.Contains("MHz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" MHz");
                            //                                    LowStrDoc = tempLow[0];

                            //                                    LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                            //                                }
                            //                                else if (ttDoc.Low_freq.Contains("kHz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" kHz");
                            //                                    LowStrDoc = tempLow[0];

                            //                                    LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                            //                                }

                            //                                else if (ttDoc.Low_freq.Contains("Hz"))
                            //                                {
                            //                                    var tempLow = ttDoc.Low_freq.Split(" Hz");
                            //                                    LowStrDoc = tempLow[0];
                            //                                    LowDoc = long.Parse(LowStrDoc);
                            //                                }


                            //                                if (ttDoc.High_freq.Contains("GHz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" GHz");
                            //                                    HighStrDoc = tempHIgh[0];
                            //                                    //High = (long)double.Parse(HighStr) * 1000000000;
                            //                                    HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                            //                                }
                            //                                else if (ttDoc.High_freq.Contains("MHz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" MHz");
                            //                                    HighStrDoc = tempHIgh[0];
                            //                                    //High = (long)double.Parse(HighStr) * 1000000;
                            //                                    HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                            //                                }
                            //                                else if (ttDoc.High_freq.Contains("kHz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" kHz");
                            //                                    HighStrDoc = tempHIgh[0];

                            //                                    //High = (long)double.Parse(HighStr) * 1000;
                            //                                    HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                            //                                }
                            //                                else if (ttDoc.High_freq.Contains("Hz"))
                            //                                {
                            //                                    var tempHIgh = ttDoc.High_freq.Split(" Hz");
                            //                                    HighStrDoc = tempHIgh[0];
                            //                                    HighDoc = long.Parse(HighStrDoc);
                            //                                }



                            //                                if ((ApplicationRange.low >= LowDoc && ApplicationRange.low <= HighDoc) || (ApplicationRange.high >= LowDoc && ApplicationRange.high <= HighDoc))
                            //                                {


                            //                                    if (ttDoc.Type_of_doc == "R")
                            //                                    {
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            var appName = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                            //                                            if (appName.name.Equals(ttDoc.Application))
                            //                                            {
                            //                                                Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + ttDoc.DocumentsId +"==="+tempValue.DocumentsId);
                            //                                                ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                            //                                                _conApp.Application.Update(ApplicationTemp);
                            //                                                _conApp.SaveChanges();
                                                                            
                            //                                            }
                            //                                            else
                            //                                            {
                            //                                                Debug.WriteLine("else:low freq:" + ApplicationRange.LowView + ":values of docs:" + ttDoc.DocumentsId + "===" + tempValue.DocumentsId);
                            //                                                var appNameTemp = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                            //                                                if (appNameTemp.name.Equals(ttDoc.Application))
                            //                                                {
                            //                                                    // Debug.WriteLine("it is simple update:low freq:" + ApplicationRange.LowView + ":values of docs:" + ttDoc.DocumentsId + "===" + tempValue.DocumentsId);
                            //                                                    ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                            //                                                    _conApp.Application.Update(ApplicationTemp);
                            //                                                    _conApp.SaveChanges();

                            //                                                }
                            //                                                else
                            //                                                {
                            //                                                    if (appNameTemp.Layer == 1)
                            //                                                    {
                            //                                                        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(ttDoc.Application) && x.first_up_lvl_id == appName.ApplicationTermsDBId).SingleOrDefault();
                            //                                                        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                            //                                                       x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId &&
                            //                                                       x.documents_Id == ttDoc.DocumentsId
                            //                                                       ).SingleOrDefault();
                            //                                                        if (isExistApp == null)
                            //                                                        {
                            //                                                            ApplicationDB newInsertApp = new ApplicationDB()
                            //                                                            {
                            //                                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                            //                                                                documents_Id = ttDoc.DocumentsId,
                            //                                                                comment = ApplicationTemp.comment,
                            //                                                            };
                            //                                                            _conApp.Application.Add(newInsertApp);
                            //                                                            _conApp.SaveChanges();


                            //                                                            ApplicationTemp.isDeletedApp = true;
                            //                                                            _conApp.Application.Update(ApplicationTemp);
                            //                                                            _conApp.SaveChanges();
                            //                                                        }
                            //                                                    }
                            //                                                    else if (appNameTemp.Layer == 2)
                            //                                                    {
                            //                                                        var appTerm = _conApp.RootApplicationTermsDB.Where(x => x.name.Equals(ttDoc.Application) && x.second_up_lvl_id == appName.ApplicationTermsDBId).SingleOrDefault();
                            //                                                        var isExistApp = _conApp.Application.Where(x => x.ApplicationTermId == appTerm.ApplicationTermsDBId &&
                            //                                                        x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && 
                            //                                                        x.documents_Id == ttDoc.DocumentsId
                            //                                                        ).SingleOrDefault();
                            //                                                        if (isExistApp == null)
                            //                                                        {
                            //                                                            ApplicationDB newInsertApp = new ApplicationDB()
                            //                                                            {
                            //                                                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                                                ApplicationTermId = appTerm.ApplicationTermsDBId,
                            //                                                                documents_Id = ttDoc.DocumentsId,
                            //                                                                comment = ApplicationTemp.comment,
                            //                                                            };
                            //                                                            _conApp.Application.Add(newInsertApp);
                            //                                                            _conApp.SaveChanges();

                            //                                                            ApplicationTemp.isDeletedApp = true;
                            //                                                            _conApp.Application.Update(ApplicationTemp);
                            //                                                            _conApp.SaveChanges();
                            //                                                        }
                            //                                                    }
                            //                                                }

                            //                                            }

                            //                                        }
                            //                                        else if (ApplicationTemp.documents_Id != ttDoc.DocumentsId)
                            //                                        {
                            //                                            //Debug.WriteLine("second enter:");
                            //                                            //try
                            //                                            //{

                            //                                            var ApplicationVal = _conApp.Application.Where(x => x.ApplicationTermId == temp.ApplicationTermsDBId && x.documents_Id == ttDoc.DocumentsId
                            //                                            && x.ApplicationRangeId == ApplicationRange.ApplicationRangeId).SingleOrDefault();
                            //                                            if (ApplicationVal != null)
                            //                                            {
                            //                                                //Debug.WriteLine("second enter not null:");
                            //                                                if (ApplicationVal.documents_Id == null)
                            //                                                {
                            //                                                    ApplicationDB app = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationVal.ApplicationRangeId,
                            //                                                        ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                        comment = null,
                            //                                                        documents_Id = ttDoc.DocumentsId

                            //                                                    };

                            //                                                    _conApp.Application.Add(app);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                                else if (ApplicationVal.documents_Id != ttDoc.DocumentsId)
                            //                                                {
                            //                                                    ApplicationDB app = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationVal.ApplicationRangeId,
                            //                                                        ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                        comment = null,
                            //                                                        documents_Id = ttDoc.DocumentsId
                            //                                                    };

                            //                                                    _conApp.Application.Add(app);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                            }
                            //                                            else if (ApplicationVal == null)
                            //                                            {
                            //                                                //Debug.WriteLine("second enter is null:");
                            //                                                ApplicationDB app = new ApplicationDB()
                            //                                                {
                            //                                                    ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                            //                                                    ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                    comment = null,
                            //                                                    documents_Id = ttDoc.DocumentsId

                            //                                                };

                            //                                                _conApp.Application.Add(app);
                            //                                                _conApp.SaveChanges();
                            //                                            }
                            //                                            //}
                            //                                            //catch (Exception ex)
                            //                                            //{
                            //                                            //    var ApplicationValList = _conApp.Application.Where(x => x.ApplicationTermId == temp.ApplicationTermsDBId && x.documents_Id == ttDoc.DocumentsId).ToList();
                            //                                            //    foreach (var valApp in ApplicationValList)
                            //                                            //    {
                            //                                            //        if (valApp != null)
                            //                                            //        {
                            //                                            //            //Debug.WriteLine("second enter not null:");
                            //                                            //            if (valApp.documents_Id == null)
                            //                                            //            {
                            //                                            //                ApplicationDB app = new ApplicationDB()
                            //                                            //                {
                            //                                            //                    ApplicationRangeId = valApp.ApplicationRangeId,
                            //                                            //                    ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                            //                    comment = null,
                            //                                            //                    documents_Id = ttDoc.DocumentsId

                            //                                            //                };

                            //                                            //                _conApp.Application.Add(app);
                            //                                            //                _conApp.SaveChanges();
                            //                                            //            }
                            //                                            //            else if (valApp.documents_Id != ttDoc.DocumentsId)
                            //                                            //            {
                            //                                            //                ApplicationDB app = new ApplicationDB()
                            //                                            //                {
                            //                                            //                    ApplicationRangeId = valApp.ApplicationRangeId,
                            //                                            //                    ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                            //                    comment = null,
                            //                                            //                    documents_Id = ttDoc.DocumentsId
                            //                                            //                };

                            //                                            //                _conApp.Application.Add(app);
                            //                                            //                _conApp.SaveChanges();
                            //                                            //            }
                            //                                            //        }
                            //                                            //        else if (valApp == null)
                            //                                            //        {
                            //                                            //            //Debug.WriteLine("second enter is null:");
                            //                                            //            ApplicationDB app = new ApplicationDB()
                            //                                            //            {
                            //                                            //                ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                            //                                            //                ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                            //                comment = null,
                            //                                            //                documents_Id = ttDoc.DocumentsId

                            //                                            //            };

                            //                                            //            _conApp.Application.Add(app);
                            //                                            //            _conApp.SaveChanges();
                            //                                            //        }
                            //                                            //    }
                            //                                            //}
                            //                                        }

                            //                                    }

                            //                                    if (ttDoc.Type_of_doc == "I")
                            //                                    {
                            //                                        if (ApplicationTemp.documents_Id == null)
                            //                                        {
                            //                                            ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                            //                                            _conApp.Application.Update(ApplicationTemp);
                            //                                            _conApp.SaveChanges();
                            //                                        }
                            //                                        else if (ApplicationTemp.documents_Id != ttDoc.DocumentsId)
                            //                                        {
                            //                                            //Debug.WriteLine("second enter:");
                            //                                            try
                            //                                            {
                            //                                                var ApplicationVal = _conApp.Application.Where(x => x.ApplicationTermId == temp.ApplicationTermsDBId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                            //                                                if (ApplicationVal != null)
                            //                                                {
                            //                                                    //Debug.WriteLine("second enter not null:");
                            //                                                    if (ApplicationVal.documents_Id == null)
                            //                                                    {
                            //                                                        ApplicationDB app = new ApplicationDB()
                            //                                                        {
                            //                                                            ApplicationRangeId = ApplicationVal.ApplicationRangeId,
                            //                                                            ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                            comment = null,
                            //                                                            documents_Id = ttDoc.DocumentsId

                            //                                                        };

                            //                                                        _conApp.Application.Add(app);
                            //                                                        _conApp.SaveChanges();
                            //                                                    }
                            //                                                    else if (ApplicationVal.documents_Id != ttDoc.DocumentsId)
                            //                                                    {
                            //                                                        ApplicationDB app = new ApplicationDB()
                            //                                                        {
                            //                                                            ApplicationRangeId = ApplicationVal.ApplicationRangeId,
                            //                                                            ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                            comment = null,
                            //                                                            documents_Id = ttDoc.DocumentsId
                            //                                                        };

                            //                                                        _conApp.Application.Add(app);
                            //                                                        _conApp.SaveChanges();
                            //                                                    }
                            //                                                }
                            //                                                else if (ApplicationVal == null)
                            //                                                {
                            //                                                    //Debug.WriteLine("second enter is null:");
                            //                                                    ApplicationDB app = new ApplicationDB()
                            //                                                    {
                            //                                                        ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                            //                                                        ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                        comment = null,
                            //                                                        documents_Id = ttDoc.DocumentsId

                            //                                                    };

                            //                                                    _conApp.Application.Add(app);
                            //                                                    _conApp.SaveChanges();
                            //                                                }
                            //                                            }
                            //                                            catch (Exception ex)
                            //                                            {
                            //                                                var ApplicationValList = _conApp.Application.Where(x => x.ApplicationTermId == temp.ApplicationTermsDBId && x.documents_Id == ttDoc.DocumentsId).ToList();
                            //                                                foreach (var valApp in ApplicationValList)
                            //                                                {
                            //                                                    if (valApp != null)
                            //                                                    {
                            //                                                        //Debug.WriteLine("second enter not null:");
                            //                                                        if (valApp.documents_Id == null)
                            //                                                        {
                            //                                                            ApplicationDB app = new ApplicationDB()
                            //                                                            {
                            //                                                                ApplicationRangeId = valApp.ApplicationRangeId,
                            //                                                                ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                                comment = null,
                            //                                                                documents_Id = ttDoc.DocumentsId

                            //                                                            };

                            //                                                            _conApp.Application.Add(app);
                            //                                                            _conApp.SaveChanges();
                            //                                                        }
                            //                                                        else if (valApp.documents_Id != ttDoc.DocumentsId)
                            //                                                        {
                            //                                                            ApplicationDB app = new ApplicationDB()
                            //                                                            {
                            //                                                                ApplicationRangeId = valApp.ApplicationRangeId,
                            //                                                                ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                                comment = null,
                            //                                                                documents_Id = ttDoc.DocumentsId
                            //                                                            };

                            //                                                            _conApp.Application.Add(app);
                            //                                                            _conApp.SaveChanges();
                            //                                                        }
                            //                                                    }
                            //                                                    else if (valApp == null)
                            //                                                    {

                            //                                                        ApplicationDB app = new ApplicationDB()
                            //                                                        {
                            //                                                            ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                            //                                                            ApplicationTermId = temp.ApplicationTermsDBId,
                            //                                                            comment = null,
                            //                                                            documents_Id = ttDoc.DocumentsId

                            //                                                        };

                            //                                                        _conApp.Application.Add(app);
                            //                                                        _conApp.SaveChanges();
                            //                                                    }
                            //                                                }
                            //                                            }
                            //                                        }

                            //                                    }
                            //                                }

                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //                else if (root.Layer == 3)
                            //                {
                            //                    //Debug.WriteLine("layer 3:" + rdr["ApplicationId"].ToString() + ":app term:" + rdr["ApplicationTermsDBId"].ToString());
                            //                    var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                            //                    var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();

                            //                    var LowStr = "";
                            //                    var HighStr = "";
                            //                    long Low = 0;
                            //                    long High = 0;
                            //                    if (tempValue.Low_freq.Contains("GHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" GHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000000000 * double.Parse(LowStr));
                            //                    }
                            //                    else if (tempValue.Low_freq.Contains("MHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" MHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000000 * double.Parse(LowStr));
                            //                    }
                            //                    else if (tempValue.Low_freq.Contains("kHz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" kHz");
                            //                        LowStr = tempLow[0];

                            //                        Low = (long)Math.Round(1000 * double.Parse(LowStr));
                            //                    }

                            //                    else if (tempValue.Low_freq.Contains("Hz"))
                            //                    {
                            //                        var tempLow = tempValue.Low_freq.Split(" Hz");
                            //                        LowStr = tempLow[0];
                            //                        Low = long.Parse(LowStr);
                            //                    }

                            //                    if (tempValue.High_freq.Contains("GHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" GHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000000000;
                            //                        High = (long)Math.Round(1000000000 * double.Parse(HighStr));

                            //                    }
                            //                    else if (tempValue.High_freq.Contains("MHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" MHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000000;
                            //                        High = (long)Math.Round(1000000 * double.Parse(HighStr));
                            //                    }
                            //                    else if (tempValue.High_freq.Contains("kHz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" kHz");
                            //                        HighStr = tempHIgh[0];
                            //                        //High = (long)double.Parse(HighStr) * 1000;
                            //                        High = (long)Math.Round(1000 * double.Parse(HighStr));
                            //                    }
                            //                    else if (tempValue.High_freq.Contains("Hz"))
                            //                    {
                            //                        var tempHIgh = tempValue.High_freq.Split(" Hz");
                            //                        HighStr = tempHIgh[0];
                            //                        High = long.Parse(HighStr);
                            //                    }

                            //                    //Debug.WriteLine("low from doc:" + Low + "===" + ApplicationRange.low);
                            //                    //Debug.WriteLine("high from doc:" + High + "===" + ApplicationRange.high+"::"+ApplicationTemp.ApplicationId+"::range::"+ApplicationTemp.ApplicationRangeId);

                            //                    if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                            //                    {
                            //                        if (tempValue.Type_of_doc == "R")
                            //                        {

                            //                            //Debug.WriteLine("low is correct:" + Low);
                            //                            if (ApplicationTemp.documents_Id == null)
                            //                            {
                            //                                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                _conApp.Application.Update(ApplicationTemp);
                            //                                _conApp.SaveChanges();
                            //                            }


                            //                            if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //                            {

                            //                                var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //                                if (ApplicationVal != null)
                            //                                {
                            //                                    if (ApplicationVal.documents_Id == null)
                            //                                    {
                            //                                        ApplicationDB app = new ApplicationDB()
                            //                                        {
                            //                                            ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                            ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                                            comment = ApplicationTemp.comment,
                            //                                            documents_Id = tempValue.DocumentsId

                            //                                        };

                            //                                        _conApp.Application.Add(app);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                }

                            //                            }
                            //                        }

                            //                        if (tempValue.Type_of_doc == "I")
                            //                        {

                            //                            //Debug.WriteLine("low is correct:" + Low);
                            //                            if (ApplicationTemp.documents_Id == null)
                            //                            {
                            //                                ApplicationTemp.documents_Id = tempValue.DocumentsId;
                            //                                _conApp.Application.Update(ApplicationTemp);
                            //                                _conApp.SaveChanges();
                            //                            }


                            //                            if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                            //                            {

                            //                                var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                            //                                if (ApplicationVal != null)
                            //                                {
                            //                                    if (ApplicationVal.documents_Id == null)
                            //                                    {
                            //                                        ApplicationDB app = new ApplicationDB()
                            //                                        {
                            //                                            ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                            //                                            ApplicationTermId = ApplicationTemp.ApplicationTermId,
                            //                                            comment = ApplicationTemp.comment,
                            //                                            documents_Id = tempValue.DocumentsId

                            //                                        };

                            //                                        _conApp.Application.Add(app);
                            //                                        _conApp.SaveChanges();
                            //                                    }
                            //                                }

                            //                            }
                            //                        }
                            //                    }
                            //                }

                            //            }
                            //        }
                            //    }
                            //}
                            //here ends reading multiple records of docs from excel.
                        //}

                        //if (rdr["application"].ToString() != "" && rdr["document"].ToString() == "")
                        //{
                        //    //Debug.WriteLine("im here:" + rdr["application"].ToString());
                        //    var ValuesDocuments = (from ww in _conApp.DocumentsDb
                        //                           select new
                        //                           {

                        //                               ww.DocumentsId,
                        //                               ww.Doc_number,
                        //                               ww.Title_of_doc,
                        //                               ww.Hyperlink,
                        //                               ww.Low_freq,
                        //                               ww.High_freq,
                        //                               ww.Application,
                        //                               ww.Type_of_doc

                        //                           }
                        //                                        ).ToList();

                        //    var Documents = ValuesDocuments.Where(x => x.Application.Contains(rdr["application"].ToString())).ToList();

                        //    foreach (var tempValue in Documents)
                        //    {
                        //        var ApplicationTemp = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                        //        var ApplicationRange = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId).SingleOrDefault();
                        //        var LowStr = "";
                        //        var HighStr = "";
                        //        long Low = 0;
                        //        long High = 0;
                        //        if (tempValue.Low_freq.Contains("GHz"))
                        //        {
                        //            var tempLow = tempValue.Low_freq.Split(" GHz");
                        //            LowStr = tempLow[0];
                        //            Low = (long)double.Parse(LowStr) * 1000000000;
                        //        }
                        //        else if (tempValue.Low_freq.Contains("MHz"))
                        //        {

                        //            var tempLow = tempValue.Low_freq.Split(" MHz");
                        //            LowStr = tempLow[0];
                        //            //Debug.WriteLine("low:" + tempValue.Low_freq + "doc:" + tempValue.DocumentsId + "==" + tempValue.Doc_number);
                        //            Low = (long)double.Parse(LowStr) * 1000000;
                        //        }
                        //        else if (tempValue.Low_freq.Contains("kHz"))
                        //        {
                        //            var tempLow = tempValue.Low_freq.Split(" kHz");
                        //            LowStr = tempLow[0];
                        //            Low = (long)double.Parse(LowStr) * 1000;
                        //        }

                        //        else if (tempValue.Low_freq.Contains("Hz"))
                        //        {
                        //            //Debug.WriteLine("qqq:" + tempValue.Low_freq + "===" + rdr["application"].ToString()+"www:"+tempValue.DocumentsId);
                        //            var tempLow = tempValue.Low_freq.Split(" Hz");
                        //            //Debug.WriteLine("www:" + tempValue.Low_freq + "===" + rdr["application"].ToString() + "appId:" + tempValue.Doc_number + "sss" + tempValue.DocumentsId);
                        //            LowStr = tempLow[0];
                        //            Low = long.Parse(LowStr);
                        //        }


                        //        if (tempValue.High_freq.Contains("GHz"))
                        //        {
                        //            //Debug.WriteLine("high:" + tempValue.High_freq+"doc:"+tempValue.DocumentsId+"=="+tempValue.Doc_number);
                        //            var tempLow = tempValue.High_freq.Split(" GHz");
                        //            HighStr = tempLow[0];
                        //            High = (long)double.Parse(HighStr) * 1000000000;
                        //        }
                        //        else if (tempValue.High_freq.Contains("MHz"))
                        //        {
                        //            var tempLow = tempValue.High_freq.Split(" MHz");
                        //            HighStr = tempLow[0];
                        //            //Debug.WriteLine("high:" + tempValue.High_freq + "vvvv" + HighStr + "doc:" + tempValue.DocumentsId + "==" + tempValue.Doc_number);

                        //            High = (long)double.Parse(HighStr) * 1000000;

                        //        }
                        //        else if (tempValue.High_freq.Contains("kHz"))
                        //        {
                        //            var tempLow = tempValue.High_freq.Split(" kHz");
                        //            HighStr = tempLow[0];
                        //            High = (long)double.Parse(HighStr) * 1000;
                        //        }
                        //        else if (tempValue.High_freq.Contains("Hz"))
                        //        {
                        //            var tempLow = tempValue.High_freq.Split(" Hz");
                        //            HighStr = tempLow[0];
                        //            High = long.Parse(HighStr);
                        //        }

                        //        if ((ApplicationRange.low >= Low && ApplicationRange.low <= High) || (ApplicationRange.high >= Low && ApplicationRange.high <= High))
                        //        {
                        //            if (tempValue.Type_of_doc == "R")
                        //            {

                        //                if (ApplicationTemp.documents_Id == null)
                        //                {
                        //                    ApplicationTemp.documents_Id = tempValue.DocumentsId;
                        //                    _conApp.Application.Update(ApplicationTemp);
                        //                    _conApp.SaveChanges();
                        //                }
                        //                else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                        //                {
                        //                    try
                        //                    {
                        //                        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                        //                        if (ApplicationVal != null)
                        //                        {
                        //                            if (ApplicationVal.documents_Id == null)
                        //                            {
                        //                                ApplicationDB app = new ApplicationDB()
                        //                                {
                        //                                    ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                        //                                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                        //                                    comment = null,
                        //                                    documents_Id = tempValue.DocumentsId

                        //                                };

                        //                                _conApp.Application.Add(app);
                        //                                _conApp.SaveChanges();
                        //                            }
                        //                        }
                        //                    }
                        //                    catch (Exception ex)
                        //                    {
                        //                        var ApplicationValList = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).ToList();

                        //                        foreach (var valApp in ApplicationValList)
                        //                        {
                        //                            if (valApp != null)
                        //                            {
                        //                                //Debug.WriteLine("second enter not null:");
                        //                                if (valApp.documents_Id == null)
                        //                                {
                        //                                    ApplicationDB app = new ApplicationDB()
                        //                                    {
                        //                                        ApplicationRangeId = valApp.ApplicationRangeId,
                        //                                        ApplicationTermId = ApplicationTemp.ApplicationTermId,
                        //                                        comment = null,
                        //                                        documents_Id = tempValue.DocumentsId

                        //                                    };

                        //                                    _conApp.Application.Add(app);
                        //                                    _conApp.SaveChanges();
                        //                                }
                        //                                else if (valApp.documents_Id != tempValue.DocumentsId)
                        //                                {
                        //                                    ApplicationDB app = new ApplicationDB()
                        //                                    {
                        //                                        ApplicationRangeId = valApp.ApplicationRangeId,
                        //                                        ApplicationTermId = ApplicationTemp.ApplicationTermId,
                        //                                        comment = null,
                        //                                        documents_Id = tempValue.DocumentsId
                        //                                    };

                        //                                    _conApp.Application.Add(app);
                        //                                    _conApp.SaveChanges();
                        //                                }
                        //                            }
                        //                            else if (valApp == null)
                        //                            {
                        //                                //Debug.WriteLine("second enter is null:");
                        //                                ApplicationDB app = new ApplicationDB()
                        //                                {
                        //                                    ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                        //                                    ApplicationTermId = ApplicationTemp.ApplicationTermId,
                        //                                    comment = null,
                        //                                    documents_Id = tempValue.DocumentsId

                        //                                };

                        //                                _conApp.Application.Add(app);
                        //                                _conApp.SaveChanges();
                        //                            }
                        //                        }
                        //                    }
                        //                }

                        //            }

                        //            if (tempValue.Type_of_doc == "I")
                        //            {

                        //                if (ApplicationTemp.documents_Id == null)
                        //                {
                        //                    ApplicationTemp.documents_Id = tempValue.DocumentsId;
                        //                    _conApp.Application.Update(ApplicationTemp);
                        //                    _conApp.SaveChanges();
                        //                }
                        //                else if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                        //                {
                        //                    var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                        //                    if (ApplicationVal != null)
                        //                    {
                        //                        if (ApplicationVal.documents_Id == null)
                        //                        {
                        //                            ApplicationDB app = new ApplicationDB()
                        //                            {
                        //                                ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                        //                                ApplicationTermId = ApplicationTemp.ApplicationTermId,
                        //                                comment = null,
                        //                                documents_Id = tempValue.DocumentsId

                        //                            };

                        //                            _conApp.Application.Add(app);
                        //                            _conApp.SaveChanges();
                        //                        }
                        //                    }
                        //                }

                        //            }

                        //        }
                        //        var ValRootApp = _conApp.RootApplicationTermsDB.Where(x => x.ApplicationTermsDBId == ApplicationTemp.ApplicationTermId).SingleOrDefault();
                        //        if (ValRootApp.Layer == 1)
                        //        {
                        //            var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.first_up_lvl_id == ValRootApp.ApplicationTermsDBId).ToList();
                        //            var ApplicationTempVar = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                        //            var ApplicationRangeVar = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTempVar.ApplicationRangeId).SingleOrDefault();


                        //            foreach (var temp in valuesRoot)
                        //            {
                        //                var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name).ToList();
                        //                foreach (var ttDoc in docTemp)
                        //                {
                        //                    if (ttDoc != null)
                        //                    {
                        //                        var LowStrDoc = "";
                        //                        var HighStrDoc = "";
                        //                        long LowDoc = 0;
                        //                        long HighDoc = 0;
                        //                        if (ttDoc.Low_freq.Contains("GHz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" GHz");
                        //                            LowStrDoc = tempLow[0];

                        //                            LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                        //                        }
                        //                        else if (ttDoc.Low_freq.Contains("MHz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" MHz");
                        //                            LowStrDoc = tempLow[0];

                        //                            LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                        //                        }
                        //                        else if (ttDoc.Low_freq.Contains("kHz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" kHz");
                        //                            LowStrDoc = tempLow[0];

                        //                            LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                        //                        }

                        //                        else if (ttDoc.Low_freq.Contains("Hz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" Hz");
                        //                            LowStrDoc = tempLow[0];
                        //                            LowDoc = long.Parse(LowStrDoc);
                        //                        }


                        //                        if (ttDoc.High_freq.Contains("GHz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" GHz");
                        //                            HighStrDoc = tempHIgh[0];
                        //                            //High = (long)double.Parse(HighStr) * 1000000000;
                        //                            HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                        //                        }
                        //                        else if (ttDoc.High_freq.Contains("MHz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" MHz");
                        //                            HighStrDoc = tempHIgh[0];
                        //                            //High = (long)double.Parse(HighStr) * 1000000;
                        //                            HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                        //                        }
                        //                        else if (ttDoc.High_freq.Contains("kHz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" kHz");
                        //                            HighStrDoc = tempHIgh[0];

                        //                            //High = (long)double.Parse(HighStr) * 1000;
                        //                            HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                        //                        }
                        //                        else if (ttDoc.High_freq.Contains("Hz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" Hz");
                        //                            HighStrDoc = tempHIgh[0];
                        //                            HighDoc = long.Parse(HighStrDoc);
                        //                        }

                        //                        if ((ApplicationRangeVar.low >= LowDoc && ApplicationRangeVar.low <= HighDoc) || (ApplicationRangeVar.high >= LowDoc && ApplicationRangeVar.high <= HighDoc))
                        //                        {
                        //                            if (ttDoc.Type_of_doc == "R")
                        //                            {

                        //                                if (ApplicationTempVar.documents_Id == null)
                        //                                {
                        //                                    ApplicationTempVar.documents_Id = ttDoc.DocumentsId;
                        //                                    _conApp.Application.Update(ApplicationTemp);
                        //                                    _conApp.SaveChanges();
                        //                                }
                        //                                else if (ApplicationTempVar.documents_Id != ttDoc.DocumentsId)
                        //                                {
                        //                                    var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTempVar.ApplicationRangeId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                        //                                    if (ApplicationVal != null)
                        //                                    {
                        //                                        if (ApplicationVal.documents_Id == null)
                        //                                        {
                        //                                            ApplicationDB app = new ApplicationDB()
                        //                                            {
                        //                                                ApplicationRangeId = ApplicationRangeVar.ApplicationRangeId,
                        //                                                ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                comment = null,
                        //                                                documents_Id = ttDoc.DocumentsId

                        //                                            };

                        //                                            _conApp.Application.Add(app);
                        //                                            _conApp.SaveChanges();
                        //                                        }
                        //                                    }
                        //                                }

                        //                            }

                        //                            if (ttDoc.Type_of_doc == "I")
                        //                            {

                        //                                if (ApplicationTempVar.documents_Id == null)
                        //                                {
                        //                                    ApplicationTempVar.documents_Id = ttDoc.DocumentsId;
                        //                                    _conApp.Application.Update(ApplicationTemp);
                        //                                    _conApp.SaveChanges();
                        //                                }
                        //                                else if (ApplicationTempVar.documents_Id != ttDoc.DocumentsId)
                        //                                {
                        //                                    var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTempVar.ApplicationRangeId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                        //                                    if (ApplicationVal != null)
                        //                                    {
                        //                                        if (ApplicationVal.documents_Id == null)
                        //                                        {
                        //                                            ApplicationDB app = new ApplicationDB()
                        //                                            {
                        //                                                ApplicationRangeId = ApplicationRangeVar.ApplicationRangeId,
                        //                                                ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                comment = null,
                        //                                                documents_Id = ttDoc.DocumentsId

                        //                                            };

                        //                                            _conApp.Application.Add(app);
                        //                                            _conApp.SaveChanges();
                        //                                        }
                        //                                    }
                        //                                }

                        //                            }

                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        else if (ValRootApp.Layer == 2)
                        //        {
                        //            var valuesRoot = _conApp.RootApplicationTermsDB.Where(x => x.second_up_lvl_id == ValRootApp.ApplicationTermsDBId).ToList();
                        //            var ApplicationTempVar = _conApp.Application.Where(x => x.ApplicationId == int.Parse(rdr["ApplicationId"].ToString())).SingleOrDefault();
                        //            var ApplicationRangeVar = _conApp.ApplicationRange.Where(x => x.ApplicationRangeId == ApplicationTempVar.ApplicationRangeId).SingleOrDefault();


                        //            foreach (var temp in valuesRoot)
                        //            {
                        //                var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name).ToList();
                        //                foreach (var ttDoc in docTemp)
                        //                {
                        //                    if (ttDoc != null)
                        //                    {
                        //                        var LowStrDoc = "";
                        //                        var HighStrDoc = "";
                        //                        long LowDoc = 0;
                        //                        long HighDoc = 0;
                        //                        if (ttDoc.Low_freq.Contains("GHz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" GHz");
                        //                            LowStrDoc = tempLow[0];

                        //                            LowDoc = (long)Math.Round(1000000000 * double.Parse(LowStrDoc));
                        //                        }
                        //                        else if (ttDoc.Low_freq.Contains("MHz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" MHz");
                        //                            LowStrDoc = tempLow[0];

                        //                            LowDoc = (long)Math.Round(1000000 * double.Parse(LowStrDoc));
                        //                        }
                        //                        else if (ttDoc.Low_freq.Contains("kHz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" kHz");
                        //                            LowStrDoc = tempLow[0];

                        //                            LowDoc = (long)Math.Round(1000 * double.Parse(LowStrDoc));
                        //                        }

                        //                        else if (ttDoc.Low_freq.Contains("Hz"))
                        //                        {
                        //                            var tempLow = ttDoc.Low_freq.Split(" Hz");
                        //                            LowStrDoc = tempLow[0];
                        //                            LowDoc = long.Parse(LowStrDoc);
                        //                        }


                        //                        if (ttDoc.High_freq.Contains("GHz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" GHz");
                        //                            HighStrDoc = tempHIgh[0];
                        //                            //High = (long)double.Parse(HighStr) * 1000000000;
                        //                            HighDoc = (long)Math.Round(1000000000 * double.Parse(HighStrDoc));

                        //                        }
                        //                        else if (ttDoc.High_freq.Contains("MHz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" MHz");
                        //                            HighStrDoc = tempHIgh[0];
                        //                            //High = (long)double.Parse(HighStr) * 1000000;
                        //                            HighDoc = (long)Math.Round(1000000 * double.Parse(HighStrDoc));
                        //                        }
                        //                        else if (ttDoc.High_freq.Contains("kHz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" kHz");
                        //                            HighStrDoc = tempHIgh[0];

                        //                            //High = (long)double.Parse(HighStr) * 1000;
                        //                            HighDoc = (long)Math.Round(1000 * double.Parse(HighStrDoc));
                        //                        }
                        //                        else if (ttDoc.High_freq.Contains("Hz"))
                        //                        {
                        //                            var tempHIgh = ttDoc.High_freq.Split(" Hz");
                        //                            HighStrDoc = tempHIgh[0];
                        //                            HighDoc = long.Parse(HighStrDoc);
                        //                        }

                        //                        if ((ApplicationRangeVar.low >= LowDoc && ApplicationRangeVar.low <= HighDoc) || (ApplicationRangeVar.high >= LowDoc && ApplicationRangeVar.high <= HighDoc))
                        //                        {
                        //                            if (ttDoc.Type_of_doc == "R")
                        //                            {

                        //                                if (ApplicationTempVar.documents_Id == null)
                        //                                {
                        //                                    ApplicationTempVar.documents_Id = ttDoc.DocumentsId;
                        //                                    _conApp.Application.Update(ApplicationTemp);
                        //                                    _conApp.SaveChanges();
                        //                                }
                        //                                else if (ApplicationTempVar.documents_Id != ttDoc.DocumentsId)
                        //                                {
                        //                                    try
                        //                                    {
                        //                                        var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTempVar.ApplicationRangeId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                        //                                        if (ApplicationVal != null)
                        //                                        {
                        //                                            if (ApplicationVal.documents_Id == null)
                        //                                            {
                        //                                                ApplicationDB app = new ApplicationDB()
                        //                                                {
                        //                                                    ApplicationRangeId = ApplicationRangeVar.ApplicationRangeId,
                        //                                                    ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                    comment = null,
                        //                                                    documents_Id = ttDoc.DocumentsId

                        //                                                };

                        //                                                _conApp.Application.Add(app);
                        //                                                _conApp.SaveChanges();
                        //                                            }
                        //                                        }
                        //                                    }
                        //                                    catch (Exception ex)
                        //                                    {
                        //                                        var ApplicationValList = _conApp.Application.Where(x => x.ApplicationTermId == ApplicationTempVar.ApplicationTermId && x.documents_Id == ttDoc.DocumentsId).ToList();
                        //                                        foreach (var valApp in ApplicationValList)
                        //                                        {
                        //                                            if (valApp != null)
                        //                                            {
                        //                                                //Debug.WriteLine("second enter not null:");
                        //                                                if (valApp.documents_Id == null)
                        //                                                {
                        //                                                    ApplicationDB app = new ApplicationDB()
                        //                                                    {
                        //                                                        ApplicationRangeId = valApp.ApplicationRangeId,
                        //                                                        ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                        comment = null,
                        //                                                        documents_Id = ttDoc.DocumentsId

                        //                                                    };

                        //                                                    _conApp.Application.Add(app);
                        //                                                    _conApp.SaveChanges();
                        //                                                }
                        //                                                else if (valApp.documents_Id != ttDoc.DocumentsId)
                        //                                                {
                        //                                                    ApplicationDB app = new ApplicationDB()
                        //                                                    {
                        //                                                        ApplicationRangeId = valApp.ApplicationRangeId,
                        //                                                        ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                        comment = null,
                        //                                                        documents_Id = ttDoc.DocumentsId
                        //                                                    };

                        //                                                    _conApp.Application.Add(app);
                        //                                                    _conApp.SaveChanges();
                        //                                                }
                        //                                            }
                        //                                            else if (valApp == null)
                        //                                            {
                        //                                                //Debug.WriteLine("second enter is null:");
                        //                                                ApplicationDB app = new ApplicationDB()
                        //                                                {
                        //                                                    ApplicationRangeId = ApplicationTempVar.ApplicationRangeId,
                        //                                                    ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                    comment = null,
                        //                                                    documents_Id = ttDoc.DocumentsId

                        //                                                };

                        //                                                _conApp.Application.Add(app);
                        //                                                _conApp.SaveChanges();
                        //                                            }
                        //                                        }
                        //                                    }

                        //                                }
                        //                            }


                        //                            if (ttDoc.Type_of_doc == "I")
                        //                            {

                        //                                if (ApplicationTempVar.documents_Id == null)
                        //                                {
                        //                                    ApplicationTempVar.documents_Id = ttDoc.DocumentsId;
                        //                                    _conApp.Application.Update(ApplicationTemp);
                        //                                    _conApp.SaveChanges();
                        //                                }
                        //                                else if (ApplicationTempVar.documents_Id != ttDoc.DocumentsId)
                        //                                {
                        //                                    var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTempVar.ApplicationRangeId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                        //                                    if (ApplicationVal != null)
                        //                                    {
                        //                                        if (ApplicationVal.documents_Id == null)
                        //                                        {
                        //                                            ApplicationDB app = new ApplicationDB()
                        //                                            {
                        //                                                ApplicationRangeId = ApplicationRangeVar.ApplicationRangeId,
                        //                                                ApplicationTermId = ApplicationTempVar.ApplicationTermId,
                        //                                                comment = null,
                        //                                                documents_Id = ttDoc.DocumentsId

                        //                                            };

                        //                                            _conApp.Application.Add(app);
                        //                                            _conApp.SaveChanges();
                        //                                        }
                        //                                    }
                        //                                }

                        //                            }

                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}


                        //    //if(ValRootApp.Layer == 1)
                        //    //{

                        //    //}else if(ValRootApp.Layer == 2)
                        //    //{
                        //    //    

                        //    //    foreach (var temp in valuesRoot)
                        //    //    {

                        //    //        //Debug.WriteLine("im here:" + tempValue.Low_freq + "==" + temp.name + "===" + tempValue.Doc_number + "===" + tempValue.High_freq);
                        //    //        var docTemp = _conApp.DocumentsDb.Where(x => x.Application == temp.name && x.Doc_number.Contains(tempValue.Doc_number)
                        //    //        ).ToList();
                        //    //        foreach (var ttDoc in docTemp)
                        //    //        {
                        //    //            if (ttDoc != null)
                        //    //            {
                        //    //                //Debug.WriteLine("im here:==" + temp.name + "===" + tempValue.Doc_number + "===:second low:" + ttDoc.Low_freq + "===" + ttDoc.High_freq + "::values low::" + ApplicationRange.LowView + "==values high==" + ApplicationRange.HighView + ":doc id:" + ApplicationTemp.documents_Id + "==new doc id:" + ttDoc.DocumentsId);
                        //    //                

                        //    //               
                        //    //                    // Debug.WriteLine("hello im entered");
                        //    //                    if (ttDoc.Type_of_doc == "R")
                        //    //                    {
                        //    //                        if (ApplicationTemp.documents_Id == null)
                        //    //                        {
                        //    //                            ApplicationTemp.documents_Id = ttDoc.DocumentsId;
                        //    //                            _conApp.Application.Update(ApplicationTemp);
                        //    //                            _conApp.SaveChanges();
                        //    //                        }
                        //    //                        else if (ApplicationTemp.documents_Id != ttDoc.DocumentsId)
                        //    //                        {
                        //    //                            //Debug.WriteLine("second enter:");
                        //    //                            try
                        //    //                            {
                        //    //                                var ApplicationVal = _conApp.Application.Where(x => x.ApplicationTermId == temp.ApplicationTermsDBId && x.documents_Id == ttDoc.DocumentsId).SingleOrDefault();
                        //    //                                if (ApplicationVal != null)
                        //    //                                {
                        //    //                                    //Debug.WriteLine("second enter not null:");
                        //    //                                    if (ApplicationVal.documents_Id == null)
                        //    //                                    {
                        //    //                                        ApplicationDB app = new ApplicationDB()
                        //    //                                        {
                        //    //                                            ApplicationRangeId = ApplicationVal.ApplicationRangeId,
                        //    //                                            ApplicationTermId = temp.ApplicationTermsDBId,
                        //    //                                            comment = null,
                        //    //                                            documents_Id = ttDoc.DocumentsId

                        //    //                                        };

                        //    //                                        _conApp.Application.Add(app);
                        //    //                                        _conApp.SaveChanges();
                        //    //                                    }
                        //    //                                    else if (ApplicationVal.documents_Id != ttDoc.DocumentsId)
                        //    //                                    {
                        //    //                                        ApplicationDB app = new ApplicationDB()
                        //    //                                        {
                        //    //                                            ApplicationRangeId = ApplicationVal.ApplicationRangeId,
                        //    //                                            ApplicationTermId = temp.ApplicationTermsDBId,
                        //    //                                            comment = null,
                        //    //                                            documents_Id = ttDoc.DocumentsId
                        //    //                                        };

                        //    //                                        _conApp.Application.Add(app);
                        //    //                                        _conApp.SaveChanges();
                        //    //                                    }
                        //    //                                }
                        //    //                                else if (ApplicationVal == null)
                        //    //                                {
                        //    //                                    //Debug.WriteLine("second enter is null:");
                        //    //                                    ApplicationDB app = new ApplicationDB()
                        //    //                                    {
                        //    //                                        ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                        //    //                                        ApplicationTermId = temp.ApplicationTermsDBId,
                        //    //                                        comment = null,
                        //    //                                        documents_Id = ttDoc.DocumentsId

                        //    //                                    };

                        //    //                                    _conApp.Application.Add(app);
                        //    //                                    _conApp.SaveChanges();
                        //    //                                }
                        //    //                            }
                        //    //                            catch (Exception ex)
                        //    //                            {
                        //    //                                var ApplicationValList = _conApp.Application.Where(x => x.ApplicationTermId == temp.ApplicationTermsDBId && x.documents_Id == ttDoc.DocumentsId).ToList();
                        //    //                                foreach (var valApp in ApplicationValList)
                        //    //                                {
                        //    //                                    if (valApp != null)
                        //    //                                    {
                        //    //                                        //Debug.WriteLine("second enter not null:");
                        //    //                                        if (valApp.documents_Id == null)
                        //    //                                        {
                        //    //                                            ApplicationDB app = new ApplicationDB()
                        //    //                                            {
                        //    //                                                ApplicationRangeId = valApp.ApplicationRangeId,
                        //    //                                                ApplicationTermId = temp.ApplicationTermsDBId,
                        //    //                                                comment = null,
                        //    //                                                documents_Id = ttDoc.DocumentsId

                        //    //                                            };

                        //    //                                            _conApp.Application.Add(app);
                        //    //                                            _conApp.SaveChanges();
                        //    //                                        }
                        //    //                                        else if (valApp.documents_Id != ttDoc.DocumentsId)
                        //    //                                        {
                        //    //                                            ApplicationDB app = new ApplicationDB()
                        //    //                                            {
                        //    //                                                ApplicationRangeId = valApp.ApplicationRangeId,
                        //    //                                                ApplicationTermId = temp.ApplicationTermsDBId,
                        //    //                                                comment = null,
                        //    //                                                documents_Id = ttDoc.DocumentsId
                        //    //                                            };

                        //    //                                            _conApp.Application.Add(app);
                        //    //                                            _conApp.SaveChanges();
                        //    //                                        }
                        //    //                                    }
                        //    //                                    else if (valApp == null)
                        //    //                                    {
                        //    //                                        //Debug.WriteLine("second enter is null:");
                        //    //                                        ApplicationDB app = new ApplicationDB()
                        //    //                                        {
                        //    //                                            ApplicationRangeId = ApplicationRange.ApplicationRangeId,
                        //    //                                            ApplicationTermId = temp.ApplicationTermsDBId,
                        //    //                                            comment = null,
                        //    //                                            documents_Id = ttDoc.DocumentsId

                        //    //                                        };

                        //    //                                        _conApp.Application.Add(app);
                        //    //                                        _conApp.SaveChanges();
                        //    //                                    }
                        //    //                                }
                        //    //                            }
                        //    //                        }

                        //    //                    }
                        //    //                }

                        //    //            }
                        //    //        }
                        //    //    }
                        //    //}
                        //    //else if(ValRootApp.Layer == 3)
                        //    //{
                        //    //    if (ApplicationTemp.documents_Id == null)
                        //    //    {
                        //    //        //Debug.WriteLine("update invetory");
                        //    //        ApplicationTemp.documents_Id = tempValue.DocumentsId;
                        //    //        _conApp.Application.Update(ApplicationTemp);
                        //    //        _conApp.SaveChanges();
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        if (ApplicationTemp.documents_Id != tempValue.DocumentsId)
                        //    //        {

                        //    //            var ApplicationVal = _conApp.Application.Where(x => x.ApplicationRangeId == ApplicationTemp.ApplicationRangeId && x.documents_Id == tempValue.DocumentsId).SingleOrDefault();
                        //    //            if (ApplicationVal != null)
                        //    //            {
                        //    //                if (ApplicationVal.documents_Id == null)
                        //    //                {
                        //    //                    ApplicationDB app = new ApplicationDB()
                        //    //                    {
                        //    //                        ApplicationRangeId = ApplicationTemp.ApplicationRangeId,
                        //    //                        ApplicationTermId = ApplicationTemp.ApplicationTermId,
                        //    //                        comment = ApplicationTemp.comment,
                        //    //                        documents_Id = tempValue.DocumentsId

                        //    //                    };
                        //    //                    //Debug.WriteLine("insert invetory");
                        //    //                    _conApp.Application.Add(app);
                        //    //                    _conApp.SaveChanges();
                        //    //                }
                        //    //            }

                        //    //        }
                        //    //    }
                        //    //}
                        //    //Debug.WriteLine("hello:");

                        //    //        }
                        //    //    }

                        //    //Debug.WriteLine("im here:"+rdr["application"]);
                        }
                    }


                }
                conn.Close();
            }
        }

        public void ReadStandardsData()
        {
            using (SqlConnection conn = new SqlConnection(_conf.GetConnectionString("AuthDBContextConnection")))
            {

                conn.Open();


                SqlCommand cmd = new SqlCommand("ReadStandardsApplication", conn);


                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {

                    while (rdr.Read())
                    {
                        if (rdr["standard"].ToString() != "")
                        {

                            if (rdr["standard"].ToString().Contains(","))
                            {
                                var ValuesOfStandards = rdr["standard"].ToString().Split(", ");

                                foreach (var temp in ValuesOfStandards)
                                {

                                    var ValuesStandardsList = (from ep in _conApp.StandardsDb
                                                               where ep.Etsi_standard == temp
                                                               select new
                                                               {
                                                                   ep.Standard_id,
                                                                   ep.Etsi_standard,
                                                                   ep.Title_doc,
                                                                   ep.Hypelink,
                                                                   ep.Low_freq,
                                                                   ep.High_freq,
                                                                   ep.Application,
                                                                   ep.Type_of_Document
                                                               }
                                                                ).ToList();
                                    bool moreThenOneStand = false;
                                    //Debug.WriteLine("dots:"+counter+ "Etsi_standard:" + temp+"=="+ ValuesStandardsList.Count);
                                    foreach (var tempValues in ValuesStandardsList)
                                    {
                                        if (tempValues.Application.Equals(rdr["application"].ToString()))
                                        {
                                            long Low = 0;
                                            long High = 0;
                                            if (tempValues.Low_freq.Contains("GHz"))
                                            {
                                                var lowFreq = tempValues.Low_freq.Split(" GHz");
                                                //var s = string.Concat(lowFreq[0].ToString(), "000000000");
                                                //double val = double.Parse(lowFreq[0].ToString());
                                                string Hi = "";
                                                if (lowFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = lowFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = lowFreq[0].ToString();
                                                }

                                                double val = double.Parse(Hi);

                                                Low = (long)Math.Round(1000000000 * val);
                                            }
                                            else if (tempValues.Low_freq.Contains("MHz"))
                                            {
                                                var lowFreq = tempValues.Low_freq.Split(" MHz");

                                                string Hi = "";
                                                if (lowFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = lowFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = lowFreq[0].ToString();
                                                }

                                                double val = double.Parse(Hi);

                                                Low = (long)Math.Round(1000000 * val);
                                            }
                                            else if (tempValues.Low_freq.Contains("kHz"))
                                            {
                                                var lowFreq = tempValues.Low_freq.Split(" kHz");

                                                //resultLower = (long)(tempLower * 1000);
                                                //var s = string.Concat(, "000");

                                                string Hi = "";
                                                if (lowFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = lowFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = lowFreq[0].ToString();
                                                }

                                                double val = double.Parse(Hi);

                                                Low = (long)Math.Round(1000 * val);
                                            }
                                            else if (tempValues.Low_freq.Contains("Hz"))
                                            {
                                                var lowFreq = tempValues.Low_freq.Split(" Hz");
                                                var s = lowFreq[0].ToString();
                                                //Debug.WriteLine("low:" + tempValues.Low_freq + "doc:" + tempValues.Standard_id + "==" + tempValues.Etsi_standard);
                                                //Debug.WriteLine("test:" + s+"::"+tempValues.Standard_id+"::");
                                                Low = long.Parse(s);
                                            }

                                            if (tempValues.High_freq.Contains("GHz"))
                                            {
                                                var highFreq = tempValues.High_freq.Split(" GHz");
                                                //Debug.WriteLine("val:" + highFreq[0].ToString());
                                                //var s = string.Concat(highFreq[0].ToString(), "000000000");
                                                //double val = double.Parse(highFreq[0].ToString());

                                                string Hi = "";
                                                if (highFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = highFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = highFreq[0].ToString();
                                                }

                                                double val = double.Parse(Hi);

                                                High = (long)Math.Round(1000000000 * val);

                                            }
                                            else if (tempValues.High_freq.Contains("MHz"))
                                            {
                                                var highFreq = tempValues.High_freq.Split(" MHz");
                                                //var s = string.Concat(highFreq[0].ToString(), "000000");
                                                //Debug.WriteLine("ttt:" + highFreq[0].ToString());
                                                string Hi = "";
                                                if (highFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = highFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = highFreq[0].ToString();
                                                }
                                                double val = double.Parse(Hi);

                                                High = (long)Math.Round(1000000 * val);
                                            }
                                            else if (tempValues.High_freq.Contains("kHz"))
                                            {
                                                var highFreq = tempValues.High_freq.Split(" kHz");
                                                //var s = string.Concat(highFreq[0].ToString(), "000");

                                                string Hi = "";
                                                if (highFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = highFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = highFreq[0].ToString();
                                                }

                                                double val = double.Parse(Hi);
                                                High = (long)Math.Round(1000 * val);

                                            }
                                            else if (tempValues.High_freq.Contains("Hz"))
                                            {
                                                var highFreq = tempValues.High_freq.Split(" Hz");
                                                string Hi = "";
                                                if (highFreq[0].ToString().Contains(" "))
                                                {
                                                    Hi = highFreq[0].ToString().Replace(" ", "");
                                                }
                                                else
                                                {
                                                    Hi = highFreq[0].ToString();
                                                }
                                                High = long.Parse(Hi);
                                            }


                                            int appId = int.Parse(rdr["ApplicationId"].ToString());


                                            var ValuesApp = _conApp.Application.Where(x => x.ApplicationId == appId).SingleOrDefault();

                                            var ValueAppRange = (from ww in _conApp.ApplicationRange
                                                                 where ww.ApplicationRangeId == ValuesApp.ApplicationRangeId
                                                                 select new
                                                                 {
                                                                     ww.low,
                                                                     ww.high
                                                                 }
                                                                 ).SingleOrDefault();
                                            if (ValueAppRange.high != null)
                                            {
                                                if ((ValueAppRange.low >= Low && ValueAppRange.low <= High) || (ValueAppRange.high >= Low && ValueAppRange.high <= High))
                                                {
                                                    if (tempValues.Type_of_Document == "R")
                                                    {
                                                        if (ValuesApp.Standard_id == null)
                                                        {
                                                            ValuesApp.Standard_id = tempValues.Standard_id;
                                                            _conApp.Application.Update(ValuesApp);
                                                            _conApp.SaveChanges();
                                                            moreThenOneStand = true;
                                                            //Debug.WriteLine("dots update:");
                                                        }
                                                        else
                                                        {
                                                            //tempValues.Etsi_standard
                                                            if (!moreThenOneStand)
                                                            {
                                                                //ValueAppRange

                                                                moreThenOneStand = true;

                                                                //Debug.WriteLine("first:"  + tempValues.Standard_id + "::"  + tempValues.Etsi_standard+"=="+ValuesApp.ApplicationRangeId);
                                                                if (ValuesApp.documents_Id == null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                                else
                                                                {
                                                                    var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                    if (Docs != null)
                                                                    {
                                                                        ValuesApp.documents_Id = null;
                                                                    }
                                                                }
                                                                ValuesApp.ApplicationId = 0;
                                                                ValuesApp.Standard_id = tempValues.Standard_id;
                                                                _conApp.Application.Add(ValuesApp);
                                                                _conApp.SaveChanges();

                                                            }

                                                            //Debug.WriteLine("dots insert:");
                                                        }
                                                    }

                                                    if (tempValues.Type_of_Document == "I")
                                                    {
                                                        if (ValuesApp.Standard_id == null)
                                                        {
                                                            ValuesApp.Standard_id = tempValues.Standard_id;
                                                            _conApp.Application.Update(ValuesApp);
                                                            _conApp.SaveChanges();
                                                            moreThenOneStand = true;
                                                            //Debug.WriteLine("dots update:");
                                                        }
                                                        else
                                                        {
                                                            //tempValues.Etsi_standard
                                                            if (!moreThenOneStand)
                                                            {
                                                                //ValueAppRange

                                                                moreThenOneStand = true;

                                                                //Debug.WriteLine("second:" + tempValues.Standard_id + "::" + tempValues.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                                if (ValuesApp.documents_Id == null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                                else
                                                                {
                                                                    var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                    if (Docs != null)
                                                                    {
                                                                        ValuesApp.documents_Id = null;
                                                                    }
                                                                }
                                                                ValuesApp.ApplicationId = 0;
                                                                ValuesApp.Standard_id = tempValues.Standard_id;
                                                                _conApp.Application.Add(ValuesApp);
                                                                _conApp.SaveChanges();

                                                            }
                                                            //Debug.WriteLine("dots insert:");
                                                        }
                                                    }

                                                    //rdr["standard"].ToString()
                                                }
                                            }
                                            else
                                            {
                                                if (ValueAppRange.low >= Low)
                                                {
                                                    if (tempValues.Type_of_Document == "R")
                                                    {
                                                        if (ValuesApp.Standard_id == null)
                                                        {
                                                            ValuesApp.Standard_id = tempValues.Standard_id;
                                                            _conApp.Application.Update(ValuesApp);
                                                            _conApp.SaveChanges();
                                                            moreThenOneStand = true;
                                                            //Debug.WriteLine("else low dots update :");
                                                        }
                                                        else
                                                        {
                                                            if (!moreThenOneStand)
                                                            {
                                                                //ValueAppRange

                                                                moreThenOneStand = true;

                                                                //Debug.WriteLine("third:" + tempValues.Standard_id + "::" + tempValues.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                                if (ValuesApp.documents_Id == null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                                else
                                                                {
                                                                    var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                    if (Docs != null)
                                                                    {
                                                                        ValuesApp.documents_Id = null;
                                                                    }
                                                                }
                                                                ValuesApp.ApplicationId = 0;
                                                                ValuesApp.Standard_id = tempValues.Standard_id;
                                                                _conApp.Application.Add(ValuesApp);
                                                                _conApp.SaveChanges();

                                                            }
                                                            //Debug.WriteLine("else low dots insert:");
                                                        }
                                                    }

                                                    if (tempValues.Type_of_Document == "I")
                                                    {
                                                        if (ValuesApp.Standard_id == null)
                                                        {
                                                            ValuesApp.Standard_id = tempValues.Standard_id;
                                                            _conApp.Application.Update(ValuesApp);
                                                            _conApp.SaveChanges();
                                                            moreThenOneStand = true;
                                                            //Debug.WriteLine("else low dots update :");
                                                        }
                                                        else
                                                        {
                                                            if (!moreThenOneStand)
                                                            {
                                                                //ValueAppRange

                                                                moreThenOneStand = true;

                                                                //Debug.WriteLine("fourth:" + tempValues.Standard_id + "::" + tempValues.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                                if (ValuesApp.documents_Id == null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                                else
                                                                {
                                                                    var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                    if (Docs != null)
                                                                    {
                                                                        ValuesApp.documents_Id = null;
                                                                    }
                                                                }
                                                                ValuesApp.ApplicationId = 0;
                                                                ValuesApp.Standard_id = tempValues.Standard_id;
                                                                _conApp.Application.Add(ValuesApp);
                                                                _conApp.SaveChanges();

                                                            }
                                                            //Debug.WriteLine("else low dots insert:");
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                //read standards with only one record.
                                var ValuesStandardsWithOutDots = (from ep in _conApp.StandardsDb
                                                                  where ep.Etsi_standard == rdr["standard"].ToString()
                                                                  select new
                                                                  {
                                                                      ep.Standard_id,
                                                                      ep.Etsi_standard,
                                                                      ep.Title_doc,
                                                                      ep.Hypelink,
                                                                      ep.Low_freq,
                                                                      ep.High_freq,
                                                                      ep.Application,
                                                                      ep.Type_of_Document
                                                                  }
                                      ).ToList();

                                //Debug.WriteLine("nooo:" + counter + "Etsi_standard:" + rdr["standard"].ToString() + "==" + ValuesStandardsWithOutDots.Count);
                                bool moreThenOnceElse = false;
                                foreach (var temp in ValuesStandardsWithOutDots)
                                {
                                    if (temp.Application.Equals(rdr["application"].ToString()))
                                    {
                                        long Low = 0;
                                        long High = 0;
                                        if (temp.Low_freq.Contains("GHz"))
                                        {
                                            var lowFreq = temp.Low_freq.Split(" GHz");
                                            //var s = string.Concat(lowFreq[0].ToString(), "000000000");
                                            //double val = double.Parse(lowFreq[0].ToString());
                                            string Hi = "";
                                            if (lowFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = lowFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = lowFreq[0].ToString();
                                            }

                                            double val = double.Parse(Hi);

                                            Low = (long)Math.Round(1000000000 * val);
                                        }
                                        else if (temp.Low_freq.Contains("MHz"))
                                        {
                                            var lowFreq = temp.Low_freq.Split(" MHz");
                                            //var s = string.Concat(lowFreq[0].ToString(), "000000");
                                            //double val = double.Parse(lowFreq[0].ToString());
                                            //Low = long.Parse(s);


                                            string Hi = "";
                                            if (lowFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = lowFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = lowFreq[0].ToString();
                                            }

                                            double val = double.Parse(Hi);

                                            Low = (long)Math.Round(1000000 * val);
                                        }
                                        else if (temp.Low_freq.Contains("kHz"))
                                        {
                                            var lowFreq = temp.Low_freq.Split(" kHz");

                                            //resultLower = (long)(tempLower * 1000);
                                            //var s = string.Concat(, "000");

                                            string Hi = "";
                                            if (lowFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = lowFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = lowFreq[0].ToString();
                                            }

                                            double val = double.Parse(Hi);

                                            Low = (long)Math.Round(1000 * val);

                                        }
                                        else if (temp.Low_freq.Contains("Hz"))
                                        {
                                            var lowFreq = temp.Low_freq.Split(" Hz");
                                            var s = lowFreq[0].ToString();
                                            //Debug.WriteLine("test:" + s);
                                            Low = long.Parse(s);
                                        }

                                        if (temp.High_freq.Contains("GHz"))
                                        {
                                            var highFreq = temp.High_freq.Split(" GHz");
                                            //Debug.WriteLine("val:" + highFreq[0].ToString());
                                            //var s = string.Concat(highFreq[0].ToString(), "000000000");
                                            //double val = double.Parse(highFreq[0].ToString());

                                            string Hi = "";
                                            if (highFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = highFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = highFreq[0].ToString();
                                            }

                                            double val = double.Parse(Hi);

                                            High = (long)Math.Round(1000000000 * val);
                                        }
                                        else if (temp.High_freq.Contains("MHz"))
                                        {
                                            var highFreq = temp.High_freq.Split(" MHz");
                                            //var s = string.Concat(highFreq[0].ToString(), "000000");
                                            //Debug.WriteLine("ttt:" + highFreq[0].ToString());
                                            string Hi = "";
                                            if (highFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = highFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = highFreq[0].ToString();
                                            }
                                            double val = double.Parse(Hi);
                                            High = (long)Math.Round(1000000 * val);
                                        }
                                        else if (temp.High_freq.Contains("kHz"))
                                        {
                                            var highFreq = temp.High_freq.Split(" kHz");
                                            //var s = string.Concat(highFreq[0].ToString(), "000");

                                            string Hi = "";
                                            if (highFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = highFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = highFreq[0].ToString();
                                            }

                                            double val = double.Parse(Hi);

                                            High = (long)Math.Round(1000 * val);
                                        }
                                        else if (temp.High_freq.Contains("Hz"))
                                        {
                                            var highFreq = temp.High_freq.Split(" Hz");
                                            string Hi = "";
                                            if (highFreq[0].ToString().Contains(" "))
                                            {
                                                Hi = highFreq[0].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                Hi = highFreq[0].ToString();
                                            }
                                            High = long.Parse(Hi);
                                        }

                                        //
                                        int appId = int.Parse(rdr["ApplicationId"].ToString());


                                        var ValuesApp = _conApp.Application.Where(x => x.ApplicationId == appId).SingleOrDefault();




                                        var ValueAppRange = (from ww in _conApp.ApplicationRange
                                                             where ww.ApplicationRangeId == ValuesApp.ApplicationRangeId
                                                             select new
                                                             {
                                                                 ww.low,
                                                                 ww.high
                                                             }
                                                             ).SingleOrDefault();

                                        //Debug.WriteLine("values:" + temp.Application+"=="+ ValueAppRange.low+"::"+ValueAppRange.high+"www low:"+ Low+"qqq: high:"+ High+"pp==" + temp.Etsi_standard);

                                        if (ValueAppRange.high != null)
                                        {
                                            if ((ValueAppRange.low >= Low && ValueAppRange.low <= High) || (ValueAppRange.high >= Low && ValueAppRange.high <= High))
                                            {
                                                // Debug.WriteLine("wwww new line");
                                                if (temp.Type_of_Document == "R")
                                                {
                                                    if (ValuesApp.Standard_id == null)
                                                    {
                                                        //Debug.WriteLine("nooo update:" + ValuesApp.documents_Id);
                                                        ValuesApp.Standard_id = temp.Standard_id;
                                                        _conApp.Application.Update(ValuesApp);
                                                        _conApp.SaveChanges();
                                                        moreThenOnceElse = true;
                                                    }
                                                    else
                                                    {
                                                        if (!moreThenOnceElse)
                                                        {
                                                            //ValueAppRange

                                                            moreThenOnceElse = true;

                                                            Debug.WriteLine("six:" + temp.Standard_id + "::" + temp.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                            if (ValuesApp.documents_Id == null)
                                                            {
                                                                ValuesApp.documents_Id = null;
                                                            }
                                                            else
                                                            {
                                                                var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                if (Docs != null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                            }
                                                            ValuesApp.ApplicationId = 0;
                                                            ValuesApp.Standard_id = temp.Standard_id;
                                                            _conApp.Application.Add(ValuesApp);
                                                            _conApp.SaveChanges();

                                                        }

                                                        // Debug.WriteLine("nooo insert:");
                                                    }
                                                }
                                                if (temp.Type_of_Document == "I")
                                                {
                                                    if (ValuesApp.Standard_id == null)
                                                    {
                                                        //Debug.WriteLine("else low nooo update :" + ValuesApp.documents_Id);
                                                        ValuesApp.Standard_id = temp.Standard_id;
                                                        _conApp.Application.Update(ValuesApp);
                                                        _conApp.SaveChanges();
                                                        moreThenOnceElse = true;
                                                    }
                                                    else
                                                    {

                                                        //Debug.WriteLine("else low nooo insert:");
                                                        if (!moreThenOnceElse)
                                                        {
                                                            //ValueAppRange

                                                            moreThenOnceElse = true;

                                                            Debug.WriteLine("six inv:" + temp.Standard_id + "::" + temp.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                            if (ValuesApp.documents_Id == null)
                                                            {
                                                                ValuesApp.documents_Id = null;
                                                            }
                                                            else
                                                            {
                                                                var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                if (Docs != null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                            }
                                                            ValuesApp.ApplicationId = 0;
                                                            ValuesApp.Standard_id = temp.Standard_id;
                                                            _conApp.Application.Add(ValuesApp);
                                                            _conApp.SaveChanges();

                                                        }
                                                    }
                                                }
                                            }

                                            //rdr["standard"].ToString()
                                        }
                                        else
                                        {

                                            if (ValueAppRange.low >= Low && ValueAppRange.low <= High)
                                            {
                                                if (temp.Type_of_Document == "R")
                                                {
                                                    if (ValuesApp.Standard_id == null)
                                                    {
                                                        //Debug.WriteLine("else low nooo update :" + ValuesApp.documents_Id);
                                                        ValuesApp.Standard_id = temp.Standard_id;
                                                        _conApp.Application.Update(ValuesApp);
                                                        _conApp.SaveChanges();
                                                        moreThenOnceElse = true;
                                                    }
                                                    else
                                                    {

                                                        //Debug.WriteLine("else low nooo insert:");
                                                        if (!moreThenOnceElse)
                                                        {
                                                            //ValueAppRange

                                                            moreThenOnceElse = true;

                                                            Debug.WriteLine("seven:" + temp.Standard_id + "::" + temp.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                            if (ValuesApp.documents_Id == null)
                                                            {
                                                                ValuesApp.documents_Id = null;
                                                            }
                                                            else
                                                            {
                                                                var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                if (Docs != null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                            }
                                                            ValuesApp.ApplicationId = 0;
                                                            ValuesApp.Standard_id = temp.Standard_id;
                                                            _conApp.Application.Add(ValuesApp);
                                                            _conApp.SaveChanges();

                                                        }
                                                    }
                                                }
                                                if (temp.Type_of_Document == "I")
                                                {
                                                    if (ValuesApp.Standard_id == null)
                                                    {
                                                        //Debug.WriteLine("else low nooo update :" + ValuesApp.documents_Id);
                                                        ValuesApp.Standard_id = temp.Standard_id;
                                                        _conApp.Application.Update(ValuesApp);
                                                        _conApp.SaveChanges();
                                                        moreThenOnceElse = true;
                                                    }
                                                    else
                                                    {

                                                        //Debug.WriteLine("else low nooo insert:");
                                                        if (!moreThenOnceElse)
                                                        {
                                                            //ValueAppRange

                                                            moreThenOnceElse = true;

                                                            Debug.WriteLine("eight:" + temp.Standard_id + "::" + temp.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                            if (ValuesApp.documents_Id == null)
                                                            {
                                                                ValuesApp.documents_Id = null;
                                                            }
                                                            else
                                                            {
                                                                var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                                if (Docs != null)
                                                                {
                                                                    ValuesApp.documents_Id = null;
                                                                }
                                                            }
                                                            ValuesApp.ApplicationId = 0;
                                                            ValuesApp.Standard_id = temp.Standard_id;
                                                            _conApp.Application.Add(ValuesApp);
                                                            _conApp.SaveChanges();

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //Debug.WriteLine("im here:" + temp.Etsi_standard+"=="+temp.Application+"==");
                                }
                            }
                        }
                        else
                        {
                            var ValuesStandardsWithOutDots = (from ep in _conApp.StandardsDb
                                                              where ep.Application == rdr["application"].ToString()
                                                              select new
                                                              {
                                                                  ep.Standard_id,
                                                                  ep.Etsi_standard,
                                                                  ep.Title_doc,
                                                                  ep.Hypelink,
                                                                  ep.Low_freq,
                                                                  ep.High_freq,
                                                                  ep.Application,
                                                                  ep.Type_of_Document
                                                              }
                                     ).ToList();

                            //Debug.WriteLine("nooo:" + counter + "Etsi_standard:" + rdr["standard"].ToString() + "==" + ValuesStandardsWithOutDots.Count);
                            bool moreThenOnceElse = false;
                            foreach (var temp in ValuesStandardsWithOutDots)
                            {
                                if (temp.Application.Equals(rdr["application"].ToString()))
                                {
                                    long Low = 0;
                                    long High = 0;
                                    if (temp.Low_freq.Contains("GHz"))
                                    {
                                        var lowFreq = temp.Low_freq.Split(" GHz");
                                        //var s = string.Concat(lowFreq[0].ToString(), "000000000");
                                        //double val = double.Parse(lowFreq[0].ToString());
                                        string Hi = "";
                                        if (lowFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = lowFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = lowFreq[0].ToString();
                                        }

                                        double val = double.Parse(Hi);

                                        Low = (long)Math.Round(1000000000 * val);
                                    }
                                    else if (temp.Low_freq.Contains("MHz"))
                                    {
                                        var lowFreq = temp.Low_freq.Split(" MHz");
                                        //var s = string.Concat(lowFreq[0].ToString(), "000000");
                                        //double val = double.Parse(lowFreq[0].ToString());
                                        //Low = long.Parse(s);


                                        string Hi = "";
                                        if (lowFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = lowFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = lowFreq[0].ToString();
                                        }

                                        double val = double.Parse(Hi);

                                        Low = (long)Math.Round(1000000 * val);
                                    }
                                    else if (temp.Low_freq.Contains("kHz"))
                                    {
                                        var lowFreq = temp.Low_freq.Split(" kHz");

                                        //resultLower = (long)(tempLower * 1000);
                                        //var s = string.Concat(, "000");

                                        string Hi = "";
                                        if (lowFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = lowFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = lowFreq[0].ToString();
                                        }

                                        double val = double.Parse(Hi);

                                        Low = (long)Math.Round(1000 * val);

                                    }
                                    else if (temp.Low_freq.Contains("Hz"))
                                    {
                                        var lowFreq = temp.Low_freq.Split(" Hz");
                                        var s = lowFreq[0].ToString();
                                        //Debug.WriteLine("test:" + s);
                                        Low = long.Parse(s);
                                    }

                                    if (temp.High_freq.Contains("GHz"))
                                    {
                                        var highFreq = temp.High_freq.Split(" GHz");
                                        //Debug.WriteLine("val:" + highFreq[0].ToString());
                                        //var s = string.Concat(highFreq[0].ToString(), "000000000");
                                        //double val = double.Parse(highFreq[0].ToString());

                                        string Hi = "";
                                        if (highFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = highFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = highFreq[0].ToString();
                                        }

                                        double val = double.Parse(Hi);

                                        High = (long)Math.Round(1000000000 * val);
                                    }
                                    else if (temp.High_freq.Contains("MHz"))
                                    {
                                        var highFreq = temp.High_freq.Split(" MHz");
                                        //var s = string.Concat(highFreq[0].ToString(), "000000");
                                        //Debug.WriteLine("ttt:" + highFreq[0].ToString());
                                        string Hi = "";
                                        if (highFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = highFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = highFreq[0].ToString();
                                        }
                                        double val = double.Parse(Hi);
                                        High = (long)Math.Round(1000000 * val);
                                    }
                                    else if (temp.High_freq.Contains("kHz"))
                                    {
                                        var highFreq = temp.High_freq.Split(" kHz");
                                        //var s = string.Concat(highFreq[0].ToString(), "000");

                                        string Hi = "";
                                        if (highFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = highFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = highFreq[0].ToString();
                                        }

                                        double val = double.Parse(Hi);

                                        High = (long)Math.Round(1000 * val);
                                    }
                                    else if (temp.High_freq.Contains("Hz"))
                                    {
                                        var highFreq = temp.High_freq.Split(" Hz");
                                        string Hi = "";
                                        if (highFreq[0].ToString().Contains(" "))
                                        {
                                            Hi = highFreq[0].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            Hi = highFreq[0].ToString();
                                        }
                                        High = long.Parse(Hi);
                                    }

                                    //
                                    int appId = int.Parse(rdr["ApplicationId"].ToString());


                                    var ValuesApp = _conApp.Application.Where(x => x.ApplicationId == appId).SingleOrDefault();

                                    var ValueAppRange = (from ww in _conApp.ApplicationRange
                                                         where ww.ApplicationRangeId == ValuesApp.ApplicationRangeId
                                                         select new
                                                         {
                                                             ww.low,
                                                             ww.high
                                                         }
                                                         ).SingleOrDefault();

                                    //Debug.WriteLine("values:" + temp.Application+"=="+ ValueAppRange.low+"::"+ValueAppRange.high+"www low:"+ Low+"qqq: high:"+ High+"pp==" + temp.Etsi_standard);

                                    if (ValueAppRange.high != null)
                                    {
                                        if ((ValueAppRange.low >= Low && ValueAppRange.low <= High) || (ValueAppRange.high >= Low && ValueAppRange.high <= High))
                                        {
                                            // Debug.WriteLine("wwww new line");

                                            if (temp.Type_of_Document == "I")
                                            {
                                                if (ValuesApp.Standard_id == null)
                                                {
                                                    //Debug.WriteLine("else low nooo update :" + ValuesApp.documents_Id);
                                                    ValuesApp.Standard_id = temp.Standard_id;
                                                    _conApp.Application.Update(ValuesApp);
                                                    _conApp.SaveChanges();
                                                    moreThenOnceElse = true;
                                                }
                                                else
                                                {

                                                    //Debug.WriteLine("else low nooo insert:");
                                                    if (!moreThenOnceElse)
                                                    {
                                                        //ValueAppRange

                                                        moreThenOnceElse = true;

                                                        //Debug.WriteLine("six inv:" + temp.Standard_id + "::" + temp.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                        if (ValuesApp.documents_Id == null)
                                                        {
                                                            ValuesApp.documents_Id = null;
                                                        }
                                                        else
                                                        {
                                                            var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                            if (Docs != null)
                                                            {
                                                                ValuesApp.documents_Id = null;
                                                            }
                                                        }
                                                        ValuesApp.ApplicationId = 0;
                                                        ValuesApp.Standard_id = temp.Standard_id;
                                                        _conApp.Application.Add(ValuesApp);
                                                        _conApp.SaveChanges();

                                                    }
                                                }
                                            }
                                        }

                                        //rdr["standard"].ToString()
                                    }
                                    else
                                    {

                                        if (ValueAppRange.low >= Low && ValueAppRange.low <= High)
                                        {

                                            if (temp.Type_of_Document == "I")
                                            {
                                                if (ValuesApp.Standard_id == null)
                                                {
                                                    //Debug.WriteLine("else low nooo update :" + ValuesApp.documents_Id);
                                                    ValuesApp.Standard_id = temp.Standard_id;
                                                    _conApp.Application.Update(ValuesApp);
                                                    _conApp.SaveChanges();
                                                    moreThenOnceElse = true;
                                                }
                                                else
                                                {

                                                    //Debug.WriteLine("else low nooo insert:");
                                                    if (!moreThenOnceElse)
                                                    {
                                                        //ValueAppRange

                                                        moreThenOnceElse = true;

                                                        //Debug.WriteLine("eight:" + temp.Standard_id + "::" + temp.Etsi_standard + "==" + ValuesApp.ApplicationRangeId);
                                                        if (ValuesApp.documents_Id == null)
                                                        {
                                                            ValuesApp.documents_Id = null;
                                                        }
                                                        else
                                                        {
                                                            var Docs = _conApp.Application.Where(x => x.ApplicationRangeId == ValuesApp.ApplicationRangeId && x.documents_Id == ValuesApp.documents_Id).ToList();

                                                            if (Docs != null)
                                                            {
                                                                ValuesApp.documents_Id = null;
                                                            }
                                                        }
                                                        ValuesApp.ApplicationId = 0;
                                                        ValuesApp.Standard_id = temp.Standard_id;
                                                        _conApp.Application.Add(ValuesApp);
                                                        _conApp.SaveChanges();

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //Debug.WriteLine("im here:" + temp.Etsi_standard+"=="+temp.Application+"==");
                            }
                        }

                    }
                }

                conn.Close();
            }
        }

        bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals('-'))
                {
                    continue;
                }
                else if (input[i].Equals(' '))
                {
                    continue;
                }
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }
    }
}
