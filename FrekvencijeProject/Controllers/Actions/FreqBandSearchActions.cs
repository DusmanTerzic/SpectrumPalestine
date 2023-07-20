using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers.Actions
{
    public class FreqBandSearchActions
    {
        private ApplicationDBContext _conApp;
        private AllocationDBContext _conAll;

        

        public List<FreqBandSearchNew> SearchFreqBand(AllocationDBContext _tempConAll,ApplicationDBContext _tempApp, long tempFrom,long tempTo, string FrequencytableValue)
        {
            _conAll = _tempConAll;
            _conApp = _tempApp;
            if (tempFrom == 0 && tempTo == 0)
            {
                var query = (from all in _conAll.AllocationRangeDb
                             join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                             join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                             join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                             join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                             from ct in g.DefaultIfEmpty()
                             where e.AllocationId != null
                             join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                             where val.regionId == int.Parse(FrequencytableValue)

                             select new AllSearchFreqBand
                             {
                                 low = all.low,
                                 high = all.high,
                                 Application ="",
                                 Comment = "",
                                 Allocation = alTerm.name,
                                 Footnote = ct.name,
                                 FootnoteDesc = foot_desc.text_desc,
                                 isBand = ct.isBand,
                                 Primary = e.primary,
                                 regionName = val.regionName,
                                 regionCode = val.regionCode,
                                 regionId = val.regionId,
                                 LowView = all.LowView,
                                 HighView = all.HighView
                             }
                                              ).Union(from all in _conAll.AllocationRangeDb
                                                      join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                                                      join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                                                      join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                                                      where e.AllocationId != null
                                                      where val.regionId == int.Parse(FrequencytableValue)
                                                      select new AllSearchFreqBand
                                                      {
                                                          low = all.low,
                                                          high = all.high,
                                                          Application = "",
                                                          Comment = "",
                                                          Allocation = alTerm.name,
                                                          Footnote = "",
                                                          FootnoteDesc = "",
                                                          isBand = false,
                                                          Primary = e.primary,
                                                          regionName = val.regionName,
                                                          regionCode = val.regionCode,
                                                          regionId = val.regionId,
                                                          LowView = all.LowView,
                                                          HighView = all.HighView
                                                      }
                                                        ).ToList();
                var appQuery = (from all in _conApp.Application
                                join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                                join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                                join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                                where val.regionId == int.Parse(FrequencytableValue)
                                select new AllSearchFreqBand
                                {

                                    low = e.low,
                                    high = e.high,
                                    Application = term.name,
                                    Comment = all.comment,
                                    Allocation = "",
                                    Footnote = "",
                                    FootnoteDesc = "",
                                    isBand = false,
                                    Primary = false,
                                    regionName = val.regionName,
                                    regionCode = val.regionCode,
                                    regionId = val.regionId,
                                    LowView = e.LowView,
                                    HighView = e.HighView
                                }).ToList();
                
                


                List < AllSearchFreqBand> allValues = null;
                List<AllSearchFreqBand> AllAppValues = null;
                if (tempFrom == 0 && tempTo == 0)
                {
                    allValues = (from x in query select (AllSearchFreqBand)x).ToList();
                    AllAppValues = (from x in appQuery select (AllSearchFreqBand)x).ToList(); 
                }
                else if (tempFrom == 0 && tempTo != 0)
                {
                    var allX = query.Where(x => x.low >= tempFrom).ToList();
                    allValues = allX.Where(x => x.low <= tempTo).ToList();
                    var allY = appQuery.Where(x => x.low >= tempFrom).ToList();
                    AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                }
                else if (tempFrom != 0 && tempTo != 0)
                {

                    var allX = query.Where(x => x.high >= tempFrom).ToList();
                    allValues = allX.Where(x => x.low <= tempTo).ToList();
                    var allY = appQuery.Where(x => x.high >= tempFrom).ToList();
                    AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                }

                
                List<FreqBandSearch> allS = new List<FreqBandSearch>();
                foreach (var tempValue in allValues)
                {
                    FreqBandSearch fre = new FreqBandSearch()
                    {
                        low = tempValue.low,
                        high = tempValue.high,
                        Allocation = tempValue.Allocation,
                        isPrimary = tempValue.Primary,
                        Application = tempValue.Application,
                        Footnote = tempValue.Footnote,
                        FootnoteDesc = tempValue.FootnoteDesc,
                        isBand = tempValue.isBand,
                        Comment = tempValue.Comment,
                        regionName = tempValue.regionName,
                        regionCode = tempValue.regionCode,
                        LowView = tempValue.LowView,
                        HighView = tempValue.HighView
                    };
                    //if (tempValue.Primary == true)
                    //{
                    //    if (tempValue.Allocation == "Mobile except aeronautical mobile" || tempValue.Allocation == "Mobile except aeronautical mobile (R)")
                    //    {

                    //        tempValue.Allocation = tempValue.Allocation.Replace("Mobile", "MOBILE");
                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }
                    //    else
                    //    {

                    //        fre.Allocation = tempValue.Allocation.ToUpper();
                    //        fre.isPrimary = true;
                    //    }

                    //}
                    //else
                    //{
                    //    if (tempValue.Allocation == "Mobile except aeronautical mobile" || tempValue.Allocation == "Mobile except aeronautical mobile (R)")
                    //    {

                    //        //tempValue.Allocation = tempValue.Allocation.Replace("Mobile", "MOBILE");
                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }
                    //    else
                    //    {

                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }

                    //}
                    allS.Add(fre);
                }

                foreach (var tempValue in AllAppValues)
                {
                    FreqBandSearch fre = new FreqBandSearch()
                    {
                        low = tempValue.low,
                        high = tempValue.high,
                        Allocation = "",
                        Application = tempValue.Application,
                        Footnote = "",
                        FootnoteDesc = "",
                        Comment = tempValue.Comment,
                        regionName = tempValue.regionName,
                        regionCode = tempValue.regionCode,
                        LowView = tempValue.LowView,
                        HighView = tempValue.HighView
                    };
                    allS.Add(fre);
                }

                List<FreqBandSearchNew> listAl = new List<FreqBandSearchNew>();
                for (int i = 0; i < allS.Count; i++)
                {
                    FootnoteJsonConvert foot = new FootnoteJsonConvert();
                    string comment = "";
                    if (allS[i].GetType().GetProperty("Footnote") != null)
                    {
                        if (allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]) != null)
                        {
                            foot.Footnote = (string)allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]);

                        }
                    }
                    if (allS[i].GetType().GetProperty("FootnoteDesc") != null)
                    {
                        if (allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]) != null)
                        {
                            foot.FootnoteDesc = (string)allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]);

                        }
                    }

                    if (allS[i].GetType().GetProperty("Comment") != null)
                    {
                        if (allS[i].GetType().GetProperty("Comment").GetValue(allS[i]) != null)
                        {

                            comment = (string)allS[i].GetType().GetProperty("Comment").GetValue(allS[i]);
                            //Debug.WriteLine("usao sam:"+ foot);
                        }
                    }
                    if (allS[i].GetType().GetProperty("Allocation") != null)
                    {
                        foot.Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]);
                    }
                    if (allS[i].GetType().GetProperty("isBand") != null)
                    {
                        foot.isBand = (bool)allS[i].GetType().GetProperty("isBand").GetValue(allS[i]);
                        //Debug.WriteLine("usao isBand:" + foot.isBand+"=all="+ foot.Allocation+"::fre="+ allS[i].GetType().GetProperty("LowView").GetValue(allS[i]) + "==foot:"+ foot.Footnote);
                    }
                    if (allS[i].GetType().GetProperty("isPrimary") != null)
                    {
                        foot.isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]);
                    }
                    List<FootnoteJsonConvert> vrijednosti = new List<FootnoteJsonConvert>();
                    List<FootnoteJsonConvert> vrijednostiBand = new List<FootnoteJsonConvert>();
                    if (foot.isBand == true)
                    {

                        vrijednosti.Add(foot);
                    }
                    else
                    {

                        vrijednostiBand.Add(foot);
                    }

                    FreqBandSearchNew al = new FreqBandSearchNew()
                    {
                        low = (long)allS[i].GetType().GetProperty("low").GetValue(allS[i]),
                        high = (long)allS[i].GetType().GetProperty("high").GetValue(allS[i]),
                        Application = (string)allS[i].GetType().GetProperty("Application").GetValue(allS[i]),
                        Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]),
                        isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                        Footnote = vrijednosti,
                        BandFootnote = vrijednostiBand,
                        Comment = comment,
                        regionName = (string)allS[i].GetType().GetProperty("regionName").GetValue(allS[i]),
                        regionCode = (string)allS[i].GetType().GetProperty("regionCode").GetValue(allS[i]),
                        LowView = (string)allS[i].GetType().GetProperty("LowView").GetValue(allS[i]),
                        HighView = (string)allS[i].GetType().GetProperty("HighView").GetValue(allS[i])
                    };
                    //Debug.WriteLine("eve ga:" + al.Application);
                    listAl.Add(al);
                }

                var ordered = listAl.OrderBy(x => x.low).ToList();

                var duplicates = ordered.OrderBy(e => e.low)
                  .GroupBy(e => e.low)
                  .Where(e => e.Count() > 1)
                  .Select(g => new
                  {
                      MostRecent = g.FirstOrDefault(),
                      Others = g.Skip(0).ToList()
                  });

                List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
                foreach (var d in duplicates)
                {
                    //Debug.WriteLine("moze ovo");
                    List<string> listOfApplication = new List<string>();
                    List<string> listOfAllocation = new List<string>();
                    List<FootnoteJsonConvert> listOfFootnote = new List<FootnoteJsonConvert>();
                    List<FootnoteJsonConvert> listOfBandFootnote = new List<FootnoteJsonConvert>();
                    List<string> tempListOfFootnote = new List<string>();
                    List<string> listOfComments = new List<string>();
                    listOfApplication.Add(d.MostRecent.Application);
                    listOfAllocation.Add(d.MostRecent.Allocation);
                    listOfComments.Add(d.MostRecent.Comment);
                    List<FreqBandSearchNew> others = d.Others;
                    FreqBandSearchNew ge = new FreqBandSearchNew();
                    ge.low = d.MostRecent.low;
                    ge.high = d.MostRecent.high;
                    ge.LowView = d.MostRecent.LowView;
                    ge.HighView = d.MostRecent.HighView;
                    ge.Application = d.MostRecent.Application;
                    ge.Allocation = d.MostRecent.Allocation;
                    ge.Footnote = new List<FootnoteJsonConvert>();
                    ge.BandFootnote = new List<FootnoteJsonConvert>();
                    FootnoteJsonConvert ss = new FootnoteJsonConvert()
                    {
                        Allocation = d.MostRecent.Allocation,
                        Footnote = "",
                        FootnoteDesc = "",
                        isBand = false,
                        isPrimary = d.MostRecent.isPrimary
                    };


                    if (d.MostRecent.Footnote.Count > 0)
                    {
                        //Debug.WriteLine("ss:" + ss.Allocation+":var="+ d.MostRecent.LowView);
                        ss.Allocation = d.MostRecent.Allocation;
                        ss.Footnote = d.MostRecent.Footnote[0].Footnote;
                        ss.FootnoteDesc = d.MostRecent.Footnote[0].FootnoteDesc;
                        ss.isBand = d.MostRecent.Footnote[0].isBand;
                        ss.isPrimary = d.MostRecent.Footnote[0].isPrimary;
                        ge.Footnote.Add(ss);
                        //Debug.WriteLine("ss:" + ss.Allocation + ":var=" + d.MostRecent.LowView+"qw:"+ ss.isBand);
                    }
                    else if (d.MostRecent.BandFootnote.Count > 0)
                    {

                        ss.Allocation = d.MostRecent.Allocation;
                        ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                        ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                        ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                        ge.BandFootnote.Add(ss);
                        ss.Footnote = "";
                        ss.FootnoteDesc = "";
                        ss.isBand = false;
                        ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                        //Debug.WriteLine("www:" + ss.Allocation + ":varrr=" + d.MostRecent.LowView+"=="+ss.isBand);
                        ge.Footnote.Add(ss);
                    }
                    else
                    {
                        //Debug.WriteLine("else:" + ss.Allocation + ":var=" + d.MostRecent.LowView + "qw:" + ss.isBand);
                        ge.Footnote.Add(ss);
                    }

                    ge.Comment = d.MostRecent.Comment;
                    bool firstAllocation = false;
                    bool firstFootnote = false;
                    var last = others.Last();
                    foreach (var p in others)
                    {

                        if (d.MostRecent.low == p.low)
                        {


                            if (!listOfApplication.Contains(p.Application))
                            {
                                if (!p.Application.Equals(""))
                                {
                                    //Debug.WriteLine("pp:" + ge.Application + "==" + p.Application);
                                    if (p.Equals(last))
                                    {
                                        ge.Application += p.Application;
                                    }
                                    else
                                    {
                                        ge.Application += p.Application + ", ";
                                    }
                                    
                                    listOfApplication.Add(p.Application);
                                }

                            }
                            if (!listOfAllocation.Contains(p.Allocation))
                            {

                                if (!p.Allocation.Equals(""))
                                {


                                    if (firstAllocation == false)
                                    {

                                        firstAllocation = true;

                                        foreach (var ww in p.Footnote)
                                        {

                                            if (ww.Allocation.Equals(p.Allocation))
                                            {

                                                FootnoteJsonConvert te = new FootnoteJsonConvert();
                                                te.Allocation = ww.Allocation;

                                                if (!ww.Footnote.Equals(""))
                                                {
                                                    //Debug.WriteLine("vrijednosti evo:" + ww.Footnote);
                                                    if (ww.isBand == true)
                                                    {
                                                        te.Footnote += ww.Footnote;
                                                        te.FootnoteDesc += ww.FootnoteDesc;
                                                        te.isBand = ww.isBand;
                                                        te.isPrimary = ww.isPrimary;
                                                        ge.Footnote.Add(te);

                                                    }

                                                }
                                            }

                                        }
                                        if (p.Footnote.Count == 0)
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert();
                                            te.Allocation = p.Allocation;
                                            te.Footnote = "";
                                            te.FootnoteDesc = "";
                                            te.isBand = false;
                                            te.isPrimary = p.isPrimary;
                                            ge.Footnote.Add(te);
                                            //Debug.WriteLine("var:" + p.LowView + "==" + p.Allocation+"prvi:"+p.isPrimary);
                                        }

                                        ge.Allocation += ", " + p.Allocation + ", ";

                                        //firstAllocation = true;
                                        listOfAllocation.Add(p.Allocation);
                                    }
                                    else
                                    {

                                        foreach (var ww in p.Footnote)
                                        {

                                            if (ww.Allocation.Equals(p.Allocation))
                                            {

                                                FootnoteJsonConvert te = new FootnoteJsonConvert();
                                                te.Allocation = ww.Allocation;

                                                if (!ww.Footnote.Equals(""))
                                                {
                                                    //Debug.WriteLine("vrijednosti evo:" + ww.Footnote);
                                                    if (ww.isBand == true)
                                                    {
                                                        te.Footnote += ww.Footnote;
                                                        te.FootnoteDesc += ww.FootnoteDesc;
                                                        te.isBand = ww.isBand;
                                                        te.isPrimary = ww.isPrimary;
                                                        ge.Footnote.Add(te);

                                                    }

                                                }
                                            }

                                        }
                                        if (p.Footnote.Count == 0)
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert();
                                            te.Allocation = p.Allocation;
                                            te.Footnote = "";
                                            te.FootnoteDesc = "";
                                            te.isBand = false;
                                            te.isPrimary = p.isPrimary;
                                            ge.Footnote.Add(te);
                                            //Debug.WriteLine("var:" + p.LowView + "==" + p.Allocation+"vrije:"+p.isPrimary);
                                        }

                                        ge.Allocation += p.Allocation + ", ";

                                        //firstAllocation = true;
                                        listOfAllocation.Add(p.Allocation);
                                    }

                                }

                            }
                            else
                            {
                                foreach (var ww in p.Footnote)
                                {

                                    if (ww.Allocation.Equals(p.Allocation))
                                    {

                                        FootnoteJsonConvert te = new FootnoteJsonConvert();
                                        te.Allocation = ww.Allocation;

                                        if (!ww.Footnote.Equals(""))
                                        {

                                            if (ww.isBand == true)
                                            {
                                                //Debug.WriteLine("vrijednosti else true:"+ p.low+"==" + ww.Footnote);
                                                te.Footnote += ww.Footnote;
                                                te.FootnoteDesc += ww.FootnoteDesc;
                                                te.isBand = ww.isBand;
                                                te.isPrimary = ww.isPrimary;
                                                ge.Footnote.Add(te);

                                            }

                                        }

                                    }

                                }

                                foreach (var fo in p.BandFootnote)
                                {
                                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                                    te.Allocation = fo.Allocation;
                                    if (!fo.Footnote.Equals(""))
                                    {
                                        //Debug.WriteLine("band:" + fo.Footnote);
                                        if (fo.isBand == false)
                                        {
                                            te.Footnote += fo.Footnote;
                                            te.FootnoteDesc += fo.FootnoteDesc;
                                            te.isBand = fo.isBand;
                                            te.isPrimary = fo.isPrimary;
                                            ge.BandFootnote.Add(te);
                                            listOfBandFootnote.Add(te);
                                        }

                                    }
                                }

                            }

                            if (!listOfComments.Contains(p.Comment))
                            {
                                if (!p.Comment.Equals(""))
                                {

                                    ge.Comment += p.Comment + ", ";
                                    listOfComments.Add(p.Comment);
                                }
                            }

                        }

                    }
                    ge.regionCode = d.MostRecent.regionCode;
                    ge.regionName = d.MostRecent.regionName;
                    ge.Footnote.OrderByDescending(e => e.isPrimary == true);
                    listGeneral.Add(ge);
                    //Debug.WriteLine("tt:" + d.MostRecent.low + ", ");
                }

                var Nodupl = ordered.OrderBy(e => e.low)
         .GroupBy(e => e.low)
         .Where(e => e.Count() == 1)
         .Select(g => new
         {
             MostRecent = g.FirstOrDefault(),
             Others = g.Skip(0).ToList()
         });
                // Debug.WriteLine("ne dupli" + Nodupl.Count());
                List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
                foreach (var temp in Nodupl)
                {

                    foreach (var ww in temp.Others)
                    {

                        FootnoteJsonConvert te = new FootnoteJsonConvert();
                        te.Allocation = ww.Allocation;
                        te.Footnote = "";
                        te.FootnoteDesc = "";
                        te.isBand = false;
                        te.isPrimary = ww.isPrimary;
                        ww.Footnote.Add(te);
                        list.Add(ww);
                    }

                    //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
                }
                

                list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
                listGeneral.AddRange(list);

           
                return listGeneral.OrderBy(e => e.low).ToList();

            }else if(tempFrom == 0 && tempTo != 0)
            {
                var query = (from all in _conAll.AllocationRangeDb
                             join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                             join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                             join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                             join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                             from ct in g.DefaultIfEmpty()
                             where e.AllocationId != null
                             join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                             where  val.regionId == int.Parse(FrequencytableValue)

                             select new AllSearchFreqBand
                             {
                                 low = all.low,
                                 high = all.high,
                                 Application = "",
                                 Comment = "",
                                 Allocation = alTerm.name,
                                 Footnote = ct.name,
                                 FootnoteDesc = foot_desc.text_desc,
                                 isBand = ct.isBand,
                                 Primary = e.primary,
                                 regionName = val.regionName,
                                 regionCode = val.regionCode,
                                 regionId = val.regionId,
                                 LowView = all.LowView,
                                 HighView = all.HighView
                             }
                                              ).Union(from all in _conAll.AllocationRangeDb
                                                      join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                                                      join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                                                      join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                                                      where e.AllocationId != null
                                                      where all.low >= tempFrom && all.low <= tempTo && val.regionId == int.Parse(FrequencytableValue)
                                                      select new AllSearchFreqBand
                                                      {
                                                          low = all.low,
                                                          high = all.high,
                                                          Application = "",
                                                          Comment = "",
                                                          Allocation = alTerm.name,
                                                          Footnote = "",
                                                          FootnoteDesc = "",
                                                          isBand = false,
                                                          Primary = e.primary,
                                                          regionName = val.regionName,
                                                          regionCode = val.regionCode,
                                                          regionId = val.regionId,
                                                          LowView = all.LowView,
                                                          HighView = all.HighView
                                                      }
                                                        ).ToList();
                var appQuery = (from all in _conApp.Application
                                join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                                join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                                join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                                where  val.regionId == int.Parse(FrequencytableValue)
                                select new AllSearchFreqBand
                                {

                                    low = e.low,
                                    high = e.high,
                                    Application = term.name,
                                    Comment = all.comment,
                                    Allocation = "",
                                    Footnote = "",
                                    FootnoteDesc = "",
                                    isBand = false,
                                    Primary = false,
                                    regionName = val.regionName,
                                    regionCode = val.regionCode,
                                    regionId = val.regionId,
                                    LowView = e.LowView,
                                    HighView = e.HighView
                                }).ToList();




                List<AllSearchFreqBand> allValues = null;
                List<AllSearchFreqBand> AllAppValues = null;
                if (tempFrom == 0 && tempTo == 0)
                {
                    allValues = (from x in query select (AllSearchFreqBand)x).ToList();
                    AllAppValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();
                }
                else if (tempFrom == 0 && tempTo != 0)
                {
                    var allX = query.Where(x => x.low >= tempFrom).ToList();
                    allValues = allX.Where(x => x.low <= tempTo).ToList();
                    var allY = appQuery.Where(x => x.low >= tempFrom).ToList();
                    AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                }
                else if (tempFrom != 0 && tempTo != 0)
                {

                    var allX = query.Where(x => x.high >= tempFrom).ToList();
                    allValues = allX.Where(x => x.low <= tempTo).ToList();
                    var allY = appQuery.Where(x => x.high >= tempFrom).ToList();
                    AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                }

                //foreach(var tempAll in allValues)
                //{
                //    foreach(var tempApp in AllAppValues)
                //    {
                //        if(tempAll.low == tempApp.low)
                //        {

                //        }
                //    }
                //}


                List<FreqBandSearch> allS = new List<FreqBandSearch>();
                foreach (var tempValue in allValues)
                {
                    FreqBandSearch fre = new FreqBandSearch()
                    {
                        low = tempValue.low,
                        high = tempValue.high,
                        Allocation = tempValue.Allocation,
                        isPrimary = tempValue.Primary,
                        Application = tempValue.Application,
                        Footnote = tempValue.Footnote,
                        FootnoteDesc = tempValue.FootnoteDesc,
                        isBand = tempValue.isBand,
                        Comment = tempValue.Comment,
                        regionName = tempValue.regionName,
                        regionCode = tempValue.regionCode,
                        LowView = tempValue.LowView,
                        HighView = tempValue.HighView
                    };
                    //if (tempValue.Primary == true)
                    //{
                    //    if (tempValue.Allocation == "Mobile except aeronautical mobile" || tempValue.Allocation == "Mobile except aeronautical mobile (R)")
                    //    {

                    //        tempValue.Allocation = tempValue.Allocation.Replace("Mobile", "MOBILE");
                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }
                    //    else
                    //    {

                    //        fre.Allocation = tempValue.Allocation.ToUpper();
                    //        fre.isPrimary = true;
                    //    }

                    //}
                    //else
                    //{
                    //    if (tempValue.Allocation == "Mobile except aeronautical mobile" || tempValue.Allocation == "Mobile except aeronautical mobile (R)")
                    //    {

                    //        //tempValue.Allocation = tempValue.Allocation.Replace("Mobile", "MOBILE");
                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }
                    //    else
                    //    {

                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }

                    //}
                    allS.Add(fre);
                }

                foreach (var tempValue in AllAppValues)
                {
                    FreqBandSearch fre = new FreqBandSearch()
                    {
                        low = tempValue.low,
                        high = tempValue.high,
                        Allocation = "",
                        Application = tempValue.Application,
                        Footnote = "",
                        FootnoteDesc = "",
                        Comment = tempValue.Comment,
                        regionName = tempValue.regionName,
                        regionCode = tempValue.regionCode,
                        LowView = tempValue.LowView,
                        HighView = tempValue.HighView
                    };
                    allS.Add(fre);
                }

                List<FreqBandSearchNew> listAl = new List<FreqBandSearchNew>();
                for (int i = 0; i < allS.Count; i++)
                {
                    FootnoteJsonConvert foot = new FootnoteJsonConvert();
                    string comment = "";
                    if (allS[i].GetType().GetProperty("Footnote") != null)
                    {
                        if (allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]) != null)
                        {
                            foot.Footnote = (string)allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]);

                        }
                    }
                    if (allS[i].GetType().GetProperty("FootnoteDesc") != null)
                    {
                        if (allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]) != null)
                        {
                            foot.FootnoteDesc = (string)allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]);

                        }
                    }

                    if (allS[i].GetType().GetProperty("Comment") != null)
                    {
                        if (allS[i].GetType().GetProperty("Comment").GetValue(allS[i]) != null)
                        {

                            comment = (string)allS[i].GetType().GetProperty("Comment").GetValue(allS[i]);
                            //Debug.WriteLine("usao sam:"+ foot);
                        }
                    }
                    if (allS[i].GetType().GetProperty("Allocation") != null)
                    {
                        foot.Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]);
                    }
                    if (allS[i].GetType().GetProperty("isBand") != null)
                    {
                        foot.isBand = (bool)allS[i].GetType().GetProperty("isBand").GetValue(allS[i]);
                        //Debug.WriteLine("usao isBand:" + foot.isBand+"=all="+ foot.Allocation+"::fre="+ allS[i].GetType().GetProperty("LowView").GetValue(allS[i]) + "==foot:"+ foot.Footnote);
                    }
                    if (allS[i].GetType().GetProperty("isPrimary") != null)
                    {
                        foot.isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]);
                    }
                    List<FootnoteJsonConvert> vrijednosti = new List<FootnoteJsonConvert>();
                    List<FootnoteJsonConvert> vrijednostiBand = new List<FootnoteJsonConvert>();
                    if (foot.isBand == true)
                    {

                        vrijednosti.Add(foot);
                    }
                    else
                    {

                        vrijednostiBand.Add(foot);
                    }

                    FreqBandSearchNew al = new FreqBandSearchNew()
                    {
                        low = (long)allS[i].GetType().GetProperty("low").GetValue(allS[i]),
                        high = (long)allS[i].GetType().GetProperty("high").GetValue(allS[i]),
                        Application = (string)allS[i].GetType().GetProperty("Application").GetValue(allS[i]),
                        Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]),
                        isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                        Footnote = vrijednosti,
                        BandFootnote = vrijednostiBand,
                        Comment = comment,
                        regionName = (string)allS[i].GetType().GetProperty("regionName").GetValue(allS[i]),
                        regionCode = (string)allS[i].GetType().GetProperty("regionCode").GetValue(allS[i]),
                        LowView = (string)allS[i].GetType().GetProperty("LowView").GetValue(allS[i]),
                        HighView = (string)allS[i].GetType().GetProperty("HighView").GetValue(allS[i])
                    };
                    //Debug.WriteLine("eve ga:" + al.Application);
                    listAl.Add(al);
                }

                var ordered = listAl.OrderBy(x => x.low).ToList();

                var duplicates = ordered.OrderBy(e => e.low)
                  .GroupBy(e => e.low)
                  .Where(e => e.Count() > 1)
                  .Select(g => new
                  {
                      MostRecent = g.FirstOrDefault(),
                      Others = g.Skip(0).ToList()
                  });

                List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
                foreach (var d in duplicates)
                {
                    //Debug.WriteLine("moze ovo");
                    List<string> listOfApplication = new List<string>();
                    List<string> listOfAllocation = new List<string>();
                    List<FootnoteJsonConvert> listOfFootnote = new List<FootnoteJsonConvert>();
                    List<FootnoteJsonConvert> listOfBandFootnote = new List<FootnoteJsonConvert>();
                    List<string> tempListOfFootnote = new List<string>();
                    List<string> listOfComments = new List<string>();
                    listOfApplication.Add(d.MostRecent.Application);
                    listOfAllocation.Add(d.MostRecent.Allocation);
                    listOfComments.Add(d.MostRecent.Comment);
                    List<FreqBandSearchNew> others = d.Others;
                    FreqBandSearchNew ge = new FreqBandSearchNew();
                    ge.low = d.MostRecent.low;
                    ge.high = d.MostRecent.high;
                    ge.LowView = d.MostRecent.LowView;
                    ge.HighView = d.MostRecent.HighView;
                    ge.Application = d.MostRecent.Application;
                    ge.Allocation = d.MostRecent.Allocation;
                    ge.Footnote = new List<FootnoteJsonConvert>();
                    ge.BandFootnote = new List<FootnoteJsonConvert>();
                    FootnoteJsonConvert ss = new FootnoteJsonConvert()
                    {
                        Allocation = d.MostRecent.Allocation,
                        Footnote = "",
                        FootnoteDesc = "",
                        isBand = false,
                        isPrimary = d.MostRecent.isPrimary
                    };


                    if (d.MostRecent.Footnote.Count > 0)
                    {
                        //Debug.WriteLine("ss:" + ss.Allocation+":var="+ d.MostRecent.LowView);
                        ss.Allocation = d.MostRecent.Allocation;
                        ss.Footnote = d.MostRecent.Footnote[0].Footnote;
                        ss.FootnoteDesc = d.MostRecent.Footnote[0].FootnoteDesc;
                        ss.isBand = d.MostRecent.Footnote[0].isBand;
                        ss.isPrimary = d.MostRecent.Footnote[0].isPrimary;
                        ge.Footnote.Add(ss);
                        //Debug.WriteLine("ss:" + ss.Allocation + ":var=" + d.MostRecent.LowView+"qw:"+ ss.isBand);
                    }
                    else if (d.MostRecent.BandFootnote.Count > 0)
                    {

                        ss.Allocation = d.MostRecent.Allocation;
                        ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                        ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                        ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                        ge.BandFootnote.Add(ss);
                        ss.Footnote = "";
                        ss.FootnoteDesc = "";
                        ss.isBand = false;
                        ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                        //Debug.WriteLine("www:" + ss.Allocation + ":varrr=" + d.MostRecent.LowView+"=="+ss.isBand);
                        ge.Footnote.Add(ss);
                    }
                    else
                    {
                        //Debug.WriteLine("else:" + ss.Allocation + ":var=" + d.MostRecent.LowView + "qw:" + ss.isBand);
                        ge.Footnote.Add(ss);
                    }

                    ge.Comment = d.MostRecent.Comment;
                    bool firstAllocation = false;
                    bool firstFootnote = false;
                    var last = others.Last();
                    foreach (var p in others)
                    {

                        if (d.MostRecent.low == p.low)
                        {


                            if (!listOfApplication.Contains(p.Application))
                            {
                                if (!p.Application.Equals(""))
                                {
                                    //Debug.WriteLine("pp:" + ge.Application + "==" + p.Application);
                                    if (p.Equals(last))
                                    {
                                        ge.Application += p.Application;
                                    }
                                    else
                                    {
                                        ge.Application += p.Application + ", ";
                                    }

                                    listOfApplication.Add(p.Application);
                                }

                            }
                            if (!listOfAllocation.Contains(p.Allocation))
                            {

                                if (!p.Allocation.Equals(""))
                                {


                                    if (firstAllocation == false)
                                    {

                                        firstAllocation = true;

                                        foreach (var ww in p.Footnote)
                                        {

                                            if (ww.Allocation.Equals(p.Allocation))
                                            {

                                                FootnoteJsonConvert te = new FootnoteJsonConvert();
                                                te.Allocation = ww.Allocation;

                                                if (!ww.Footnote.Equals(""))
                                                {
                                                    //Debug.WriteLine("vrijednosti evo:" + ww.Footnote);
                                                    if (ww.isBand == true)
                                                    {
                                                        te.Footnote += ww.Footnote;
                                                        te.FootnoteDesc += ww.FootnoteDesc;
                                                        te.isBand = ww.isBand;
                                                        te.isPrimary = ww.isPrimary;
                                                        ge.Footnote.Add(te);

                                                    }

                                                }
                                            }

                                        }
                                        if (p.Footnote.Count == 0)
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert();
                                            te.Allocation = p.Allocation;
                                            te.Footnote = "";
                                            te.FootnoteDesc = "";
                                            te.isBand = false;
                                            te.isPrimary = p.isPrimary;
                                            ge.Footnote.Add(te);
                                            //Debug.WriteLine("var:" + p.LowView + "==" + p.Allocation+"prvi:"+p.isPrimary);
                                        }

                                        ge.Allocation += ", " + p.Allocation + ", ";

                                        //firstAllocation = true;
                                        listOfAllocation.Add(p.Allocation);
                                    }
                                    else
                                    {

                                        foreach (var ww in p.Footnote)
                                        {

                                            if (ww.Allocation.Equals(p.Allocation))
                                            {

                                                FootnoteJsonConvert te = new FootnoteJsonConvert();
                                                te.Allocation = ww.Allocation;

                                                if (!ww.Footnote.Equals(""))
                                                {
                                                    //Debug.WriteLine("vrijednosti evo:" + ww.Footnote);
                                                    if (ww.isBand == true)
                                                    {
                                                        te.Footnote += ww.Footnote;
                                                        te.FootnoteDesc += ww.FootnoteDesc;
                                                        te.isBand = ww.isBand;
                                                        te.isPrimary = ww.isPrimary;
                                                        ge.Footnote.Add(te);

                                                    }

                                                }
                                            }

                                        }
                                        if (p.Footnote.Count == 0)
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert();
                                            te.Allocation = p.Allocation;
                                            te.Footnote = "";
                                            te.FootnoteDesc = "";
                                            te.isBand = false;
                                            te.isPrimary = p.isPrimary;
                                            ge.Footnote.Add(te);
                                            //Debug.WriteLine("var:" + p.LowView + "==" + p.Allocation+"vrije:"+p.isPrimary);
                                        }

                                        ge.Allocation += p.Allocation + ", ";

                                        //firstAllocation = true;
                                        listOfAllocation.Add(p.Allocation);
                                    }

                                }

                            }
                            else
                            {
                                foreach (var ww in p.Footnote)
                                {

                                    if (ww.Allocation.Equals(p.Allocation))
                                    {

                                        FootnoteJsonConvert te = new FootnoteJsonConvert();
                                        te.Allocation = ww.Allocation;

                                        if (!ww.Footnote.Equals(""))
                                        {

                                            if (ww.isBand == true)
                                            {
                                                //Debug.WriteLine("vrijednosti else true:"+ p.low+"==" + ww.Footnote);
                                                te.Footnote += ww.Footnote;
                                                te.FootnoteDesc += ww.FootnoteDesc;
                                                te.isBand = ww.isBand;
                                                te.isPrimary = ww.isPrimary;
                                                ge.Footnote.Add(te);

                                            }

                                        }

                                    }

                                }

                                foreach (var fo in p.BandFootnote)
                                {
                                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                                    te.Allocation = fo.Allocation;
                                    if (!fo.Footnote.Equals(""))
                                    {
                                        //Debug.WriteLine("band:" + fo.Footnote);
                                        if (fo.isBand == false)
                                        {
                                            te.Footnote += fo.Footnote;
                                            te.FootnoteDesc += fo.FootnoteDesc;
                                            te.isBand = fo.isBand;
                                            te.isPrimary = fo.isPrimary;
                                            ge.BandFootnote.Add(te);
                                            listOfBandFootnote.Add(te);
                                        }

                                    }
                                }

                            }

                            if (!listOfComments.Contains(p.Comment))
                            {
                                if (!p.Comment.Equals(""))
                                {

                                    ge.Comment += p.Comment + ", ";
                                    listOfComments.Add(p.Comment);
                                }
                            }

                        }

                    }
                    ge.regionCode = d.MostRecent.regionCode;
                    ge.regionName = d.MostRecent.regionName;
                    ge.Footnote.OrderByDescending(e => e.isPrimary == true);
                    listGeneral.Add(ge);
                    //Debug.WriteLine("tt:" + d.MostRecent.low + ", ");
                }

                var Nodupl = ordered.OrderBy(e => e.low)
         .GroupBy(e => e.low)
         .Where(e => e.Count() == 1)
         .Select(g => new
         {
             MostRecent = g.FirstOrDefault(),
             Others = g.Skip(0).ToList()
         });
                // Debug.WriteLine("ne dupli" + Nodupl.Count());
                List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
                foreach (var temp in Nodupl)
                {

                    foreach (var ww in temp.Others)
                    {

                        FootnoteJsonConvert te = new FootnoteJsonConvert();
                        te.Allocation = ww.Allocation;
                        te.Footnote = "";
                        te.FootnoteDesc = "";
                        te.isBand = false;
                        te.isPrimary = ww.isPrimary;
                        ww.Footnote.Add(te);
                        list.Add(ww);
                    }

                    //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
                }


                list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
                listGeneral.AddRange(list);
                return listGeneral;
            }else if(tempFrom != 0 && tempTo != 0)
            {
                var query = (from all in _conAll.AllocationRangeDb
                             join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                             join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                             join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                             join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                             from ct in g.DefaultIfEmpty()
                             where e.AllocationId != null
                             join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                             where  val.regionId == int.Parse(FrequencytableValue)

                             select new AllSearchFreqBand
                             {
                                 low = all.low,
                                 high = all.high,
                                 Application = "",
                                 Comment = "",
                                 Allocation = alTerm.name,
                                 Footnote = ct.name,
                                 FootnoteDesc = foot_desc.text_desc,
                                 isBand = ct.isBand,
                                 Primary = e.primary,
                                 regionName = val.regionName,
                                 regionCode = val.regionCode,
                                 regionId = val.regionId,
                                 LowView = all.LowView,
                                 HighView = all.HighView
                             }
                                              ).Union(from all in _conAll.AllocationRangeDb
                                                      join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                                                      join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                                                      join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                                                      where e.AllocationId != null
                                                      where  val.regionId == int.Parse(FrequencytableValue)
                                                      select new AllSearchFreqBand
                                                      {
                                                          low = all.low,
                                                          high = all.high,
                                                          Application = "",
                                                          Comment = "",
                                                          Allocation = alTerm.name,
                                                          Footnote = "",
                                                          FootnoteDesc = "",
                                                          isBand = false,
                                                          Primary = e.primary,
                                                          regionName = val.regionName,
                                                          regionCode = val.regionCode,
                                                          regionId = val.regionId,
                                                          LowView = all.LowView,
                                                          HighView = all.HighView
                                                      }
                                                        ).ToList();
                var appQuery = (from all in _conApp.Application
                                join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                                join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                                join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                                where  val.regionId == int.Parse(FrequencytableValue)
                                select new AllSearchFreqBand
                                {

                                    low = e.low,
                                    high = e.high,
                                    Application = term.name,
                                    Comment = all.comment,
                                    Allocation = "",
                                    Footnote = "",
                                    FootnoteDesc = "",
                                    isBand = false,
                                    Primary = false,
                                    regionName = val.regionName,
                                    regionCode = val.regionCode,
                                    regionId = val.regionId,
                                    LowView = e.LowView,
                                    HighView = e.HighView
                                }).ToList();




                List<AllSearchFreqBand> allValues = null;
                List<AllSearchFreqBand> AllAppValues = null;
                if (tempFrom == 0 && tempTo == 0)
                {
                    allValues = (from x in query select (AllSearchFreqBand)x).ToList();
                    AllAppValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();
                }
                else if (tempFrom == 0 && tempTo != 0)
                {
                    var allX = query.Where(x => x.low >= tempFrom).ToList();
                    allValues = allX.Where(x => x.low <= tempTo).ToList();
                    var allY = appQuery.Where(x => x.low >= tempFrom).ToList();
                    AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                }
                else if (tempFrom != 0 && tempTo != 0)
                {

                    var allX = query.Where(x => x.high >= tempFrom).ToList();
                    allValues = allX.Where(x => x.low <= tempTo).ToList();
                    var allY = appQuery.Where(x => x.high >= tempFrom).ToList();
                    AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                }

                //foreach(var tempAll in allValues)
                //{
                //    foreach(var tempApp in AllAppValues)
                //    {
                //        if(tempAll.low == tempApp.low)
                //        {

                //        }
                //    }
                //}


                List<FreqBandSearch> allS = new List<FreqBandSearch>();
                foreach (var tempValue in allValues)
                {
                    FreqBandSearch fre = new FreqBandSearch()
                    {
                        low = tempValue.low,
                        high = tempValue.high,
                        Allocation = tempValue.Allocation,
                        isPrimary = tempValue.Primary,
                        Application = tempValue.Application,
                        Footnote = tempValue.Footnote,
                        FootnoteDesc = tempValue.FootnoteDesc,
                        isBand = tempValue.isBand,
                        Comment = tempValue.Comment,
                        regionName = tempValue.regionName,
                        regionCode = tempValue.regionCode,
                        LowView = tempValue.LowView,
                        HighView = tempValue.HighView
                    };
                    //if (tempValue.Primary == true)
                    //{
                    //    if (tempValue.Allocation == "Mobile except aeronautical mobile" || tempValue.Allocation == "Mobile except aeronautical mobile (R)")
                    //    {

                    //        tempValue.Allocation = tempValue.Allocation.Replace("Mobile", "MOBILE");
                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }
                    //    else
                    //    {

                    //        fre.Allocation = tempValue.Allocation.ToUpper();
                    //        fre.isPrimary = true;
                    //    }

                    //}
                    //else
                    //{
                    //    if (tempValue.Allocation == "Mobile except aeronautical mobile" || tempValue.Allocation == "Mobile except aeronautical mobile (R)")
                    //    {

                    //        //tempValue.Allocation = tempValue.Allocation.Replace("Mobile", "MOBILE");
                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }
                    //    else
                    //    {

                    //        fre.Allocation = tempValue.Allocation;
                    //        fre.isPrimary = false;
                    //    }

                    //}
                    allS.Add(fre);
                }

                foreach (var tempValue in AllAppValues)
                {
                    FreqBandSearch fre = new FreqBandSearch()
                    {
                        low = tempValue.low,
                        high = tempValue.high,
                        Allocation = "",
                        Application = tempValue.Application,
                        Footnote = "",
                        FootnoteDesc = "",
                        Comment = tempValue.Comment,
                        regionName = tempValue.regionName,
                        regionCode = tempValue.regionCode,
                        LowView = tempValue.LowView,
                        HighView = tempValue.HighView
                    };
                    allS.Add(fre);
                }

                List<FreqBandSearchNew> listAl = new List<FreqBandSearchNew>();
                for (int i = 0; i < allS.Count; i++)
                {
                    FootnoteJsonConvert foot = new FootnoteJsonConvert();
                    string comment = "";
                    if (allS[i].GetType().GetProperty("Footnote") != null)
                    {
                        if (allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]) != null)
                        {
                            foot.Footnote = (string)allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]);

                        }
                    }
                    if (allS[i].GetType().GetProperty("FootnoteDesc") != null)
                    {
                        if (allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]) != null)
                        {
                            foot.FootnoteDesc = (string)allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]);

                        }
                    }

                    if (allS[i].GetType().GetProperty("Comment") != null)
                    {
                        if (allS[i].GetType().GetProperty("Comment").GetValue(allS[i]) != null)
                        {

                            comment = (string)allS[i].GetType().GetProperty("Comment").GetValue(allS[i]);
                            //Debug.WriteLine("usao sam:"+ foot);
                        }
                    }
                    if (allS[i].GetType().GetProperty("Allocation") != null)
                    {
                        foot.Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]);
                    }
                    if (allS[i].GetType().GetProperty("isBand") != null)
                    {
                        foot.isBand = (bool)allS[i].GetType().GetProperty("isBand").GetValue(allS[i]);
                        //Debug.WriteLine("usao isBand:" + foot.isBand+"=all="+ foot.Allocation+"::fre="+ allS[i].GetType().GetProperty("LowView").GetValue(allS[i]) + "==foot:"+ foot.Footnote);
                    }
                    if (allS[i].GetType().GetProperty("isPrimary") != null)
                    {
                        foot.isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]);
                    }
                    List<FootnoteJsonConvert> vrijednosti = new List<FootnoteJsonConvert>();
                    List<FootnoteJsonConvert> vrijednostiBand = new List<FootnoteJsonConvert>();
                    if (foot.isBand == true)
                    {

                        vrijednosti.Add(foot);
                    }
                    else
                    {

                        vrijednostiBand.Add(foot);
                    }

                    FreqBandSearchNew al = new FreqBandSearchNew()
                    {
                        low = (long)allS[i].GetType().GetProperty("low").GetValue(allS[i]),
                        high = (long)allS[i].GetType().GetProperty("high").GetValue(allS[i]),
                        Application = (string)allS[i].GetType().GetProperty("Application").GetValue(allS[i]),
                        Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]),
                        isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                        Footnote = vrijednosti,
                        BandFootnote = vrijednostiBand,
                        Comment = comment,
                        regionName = (string)allS[i].GetType().GetProperty("regionName").GetValue(allS[i]),
                        regionCode = (string)allS[i].GetType().GetProperty("regionCode").GetValue(allS[i]),
                        LowView = (string)allS[i].GetType().GetProperty("LowView").GetValue(allS[i]),
                        HighView = (string)allS[i].GetType().GetProperty("HighView").GetValue(allS[i])
                    };
                    //Debug.WriteLine("eve ga:" + al.Application);
                    listAl.Add(al);
                }

                var ordered = listAl.OrderBy(x => x.low).ToList();

                var duplicates = ordered.OrderBy(e => e.low)
                  .GroupBy(e => e.low)
                  .Where(e => e.Count() > 1)
                  .Select(g => new
                  {
                      MostRecent = g.FirstOrDefault(),
                      Others = g.Skip(0).ToList()
                  });

                List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
                foreach (var d in duplicates)
                {
                    //Debug.WriteLine("moze ovo");
                    List<string> listOfApplication = new List<string>();
                    List<string> listOfAllocation = new List<string>();
                    List<FootnoteJsonConvert> listOfFootnote = new List<FootnoteJsonConvert>();
                    List<FootnoteJsonConvert> listOfBandFootnote = new List<FootnoteJsonConvert>();
                    List<string> tempListOfFootnote = new List<string>();
                    List<string> listOfComments = new List<string>();
                    listOfApplication.Add(d.MostRecent.Application);
                    listOfAllocation.Add(d.MostRecent.Allocation);
                    listOfComments.Add(d.MostRecent.Comment);
                    List<FreqBandSearchNew> others = d.Others;
                    FreqBandSearchNew ge = new FreqBandSearchNew();
                    ge.low = d.MostRecent.low;
                    ge.high = d.MostRecent.high;
                    ge.LowView = d.MostRecent.LowView;
                    ge.HighView = d.MostRecent.HighView;
                    ge.Application = d.MostRecent.Application;
                    ge.Allocation = d.MostRecent.Allocation;
                    ge.Footnote = new List<FootnoteJsonConvert>();
                    ge.BandFootnote = new List<FootnoteJsonConvert>();
                    FootnoteJsonConvert ss = new FootnoteJsonConvert()
                    {
                        Allocation = d.MostRecent.Allocation,
                        Footnote = "",
                        FootnoteDesc = "",
                        isBand = false,
                        isPrimary = d.MostRecent.isPrimary
                    };


                    if (d.MostRecent.Footnote.Count > 0)
                    {
                        //Debug.WriteLine("ss:" + ss.Allocation+":var="+ d.MostRecent.LowView);
                        ss.Allocation = d.MostRecent.Allocation;
                        ss.Footnote = d.MostRecent.Footnote[0].Footnote;
                        ss.FootnoteDesc = d.MostRecent.Footnote[0].FootnoteDesc;
                        ss.isBand = d.MostRecent.Footnote[0].isBand;
                        ss.isPrimary = d.MostRecent.Footnote[0].isPrimary;
                        ge.Footnote.Add(ss);
                        //Debug.WriteLine("ss:" + ss.Allocation + ":var=" + d.MostRecent.LowView+"qw:"+ ss.isBand);
                    }
                    else if (d.MostRecent.BandFootnote.Count > 0)
                    {

                        ss.Allocation = d.MostRecent.Allocation;
                        ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                        ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                        ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                        ge.BandFootnote.Add(ss);
                        ss.Footnote = "";
                        ss.FootnoteDesc = "";
                        ss.isBand = false;
                        ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                        //Debug.WriteLine("www:" + ss.Allocation + ":varrr=" + d.MostRecent.LowView+"=="+ss.isBand);
                        ge.Footnote.Add(ss);
                    }
                    else
                    {
                        //Debug.WriteLine("else:" + ss.Allocation + ":var=" + d.MostRecent.LowView + "qw:" + ss.isBand);
                        ge.Footnote.Add(ss);
                    }

                    ge.Comment = d.MostRecent.Comment;
                    bool firstAllocation = false;
                    bool firstFootnote = false;
                    var last = others.Last();
                    foreach (var p in others)
                    {

                        if (d.MostRecent.low == p.low)
                        {


                            if (!listOfApplication.Contains(p.Application))
                            {
                                if (!p.Application.Equals(""))
                                {
                                    //Debug.WriteLine("pp:" + ge.Application + "==" + p.Application);
                                    if (p.Equals(last))
                                    {
                                        ge.Application += p.Application;
                                    }
                                    else
                                    {
                                        ge.Application += p.Application + ", ";
                                    }

                                    listOfApplication.Add(p.Application);
                                }

                            }
                            if (!listOfAllocation.Contains(p.Allocation))
                            {

                                if (!p.Allocation.Equals(""))
                                {


                                    if (firstAllocation == false)
                                    {

                                        firstAllocation = true;

                                        foreach (var ww in p.Footnote)
                                        {

                                            if (ww.Allocation.Equals(p.Allocation))
                                            {

                                                FootnoteJsonConvert te = new FootnoteJsonConvert();
                                                te.Allocation = ww.Allocation;

                                                if (!ww.Footnote.Equals(""))
                                                {
                                                    //Debug.WriteLine("vrijednosti evo:" + ww.Footnote);
                                                    if (ww.isBand == true)
                                                    {
                                                        te.Footnote += ww.Footnote;
                                                        te.FootnoteDesc += ww.FootnoteDesc;
                                                        te.isBand = ww.isBand;
                                                        te.isPrimary = ww.isPrimary;
                                                        ge.Footnote.Add(te);

                                                    }

                                                }
                                            }

                                        }
                                        if (p.Footnote.Count == 0)
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert();
                                            te.Allocation = p.Allocation;
                                            te.Footnote = "";
                                            te.FootnoteDesc = "";
                                            te.isBand = false;
                                            te.isPrimary = p.isPrimary;
                                            ge.Footnote.Add(te);
                                            //Debug.WriteLine("var:" + p.LowView + "==" + p.Allocation+"prvi:"+p.isPrimary);
                                        }

                                        ge.Allocation += ", " + p.Allocation + ", ";

                                        //firstAllocation = true;
                                        listOfAllocation.Add(p.Allocation);
                                    }
                                    else
                                    {

                                        foreach (var ww in p.Footnote)
                                        {

                                            if (ww.Allocation.Equals(p.Allocation))
                                            {

                                                FootnoteJsonConvert te = new FootnoteJsonConvert();
                                                te.Allocation = ww.Allocation;

                                                if (!ww.Footnote.Equals(""))
                                                {
                                                    //Debug.WriteLine("vrijednosti evo:" + ww.Footnote);
                                                    if (ww.isBand == true)
                                                    {
                                                        te.Footnote += ww.Footnote;
                                                        te.FootnoteDesc += ww.FootnoteDesc;
                                                        te.isBand = ww.isBand;
                                                        te.isPrimary = ww.isPrimary;
                                                        ge.Footnote.Add(te);

                                                    }

                                                }
                                            }

                                        }
                                        if (p.Footnote.Count == 0)
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert();
                                            te.Allocation = p.Allocation;
                                            te.Footnote = "";
                                            te.FootnoteDesc = "";
                                            te.isBand = false;
                                            te.isPrimary = p.isPrimary;
                                            ge.Footnote.Add(te);
                                            //Debug.WriteLine("var:" + p.LowView + "==" + p.Allocation+"vrije:"+p.isPrimary);
                                        }

                                        ge.Allocation += p.Allocation + ", ";

                                        //firstAllocation = true;
                                        listOfAllocation.Add(p.Allocation);
                                    }

                                }

                            }
                            else
                            {
                                foreach (var ww in p.Footnote)
                                {

                                    if (ww.Allocation.Equals(p.Allocation))
                                    {

                                        FootnoteJsonConvert te = new FootnoteJsonConvert();
                                        te.Allocation = ww.Allocation;

                                        if (!ww.Footnote.Equals(""))
                                        {

                                            if (ww.isBand == true)
                                            {
                                                //Debug.WriteLine("vrijednosti else true:"+ p.low+"==" + ww.Footnote);
                                                te.Footnote += ww.Footnote;
                                                te.FootnoteDesc += ww.FootnoteDesc;
                                                te.isBand = ww.isBand;
                                                te.isPrimary = ww.isPrimary;
                                                ge.Footnote.Add(te);

                                            }

                                        }

                                    }

                                }

                                foreach (var fo in p.BandFootnote)
                                {
                                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                                    te.Allocation = fo.Allocation;
                                    if (!fo.Footnote.Equals(""))
                                    {
                                        //Debug.WriteLine("band:" + fo.Footnote);
                                        if (fo.isBand == false)
                                        {
                                            te.Footnote += fo.Footnote;
                                            te.FootnoteDesc += fo.FootnoteDesc;
                                            te.isBand = fo.isBand;
                                            te.isPrimary = fo.isPrimary;
                                            ge.BandFootnote.Add(te);
                                            listOfBandFootnote.Add(te);
                                        }

                                    }
                                }

                            }

                            if (!listOfComments.Contains(p.Comment))
                            {
                                if (!p.Comment.Equals(""))
                                {

                                    ge.Comment += p.Comment + ", ";
                                    listOfComments.Add(p.Comment);
                                }
                            }

                        }

                    }
                    ge.regionCode = d.MostRecent.regionCode;
                    ge.regionName = d.MostRecent.regionName;
                    ge.Footnote.OrderByDescending(e => e.isPrimary == true);
                    listGeneral.Add(ge);
                    //Debug.WriteLine("tt:" + d.MostRecent.low + ", ");
                }

                var Nodupl = ordered.OrderBy(e => e.low)
         .GroupBy(e => e.low)
         .Where(e => e.Count() == 1)
         .Select(g => new
         {
             MostRecent = g.FirstOrDefault(),
             Others = g.Skip(0).ToList()
         });
                // Debug.WriteLine("ne dupli" + Nodupl.Count());
                List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
                foreach (var temp in Nodupl)
                {

                    foreach (var ww in temp.Others)
                    {

                        FootnoteJsonConvert te = new FootnoteJsonConvert();
                        te.Allocation = ww.Allocation;
                        te.Footnote = "";
                        te.FootnoteDesc = "";
                        te.isBand = false;
                        te.isPrimary = ww.isPrimary;
                        ww.Footnote.Add(te);
                        list.Add(ww);
                    }

                    //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
                }


                list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
                listGeneral.AddRange(list);
                return listGeneral;
            }
            return null;
        }
    }
}
