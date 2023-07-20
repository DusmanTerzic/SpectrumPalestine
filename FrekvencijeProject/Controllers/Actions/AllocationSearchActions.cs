using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers.Actions
{
    public class AllocationSearchActions
    {
        private  AllocationDBContext _conAll;
        //string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(this.FrequencytableValue)).First().Text;
        //ovaj kod je za freq table da li palestine ili itu
        public List<FreqBandSearchNew> FirstLevelSearch(AllocationDBContext _tempConAll,long tempFrom,long tempTo, string tempValueAllocation,string FreqTable)
        {
            _conAll = _tempConAll;
            //Debug.WriteLine("tu sam");
            var queryTo
                     = (from all in _conAll.AllocationRangeDb
                        join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                        join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                        join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                        join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                        from ct in g.DefaultIfEmpty()
                        where e.AllocationId != null
                        join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                        select new AllSearchFreqBand
                        {
                            low = all.low,
                            high = all.high,
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

                                     select new AllSearchFreqBand
                                     {
                                         low = all.low,
                                         high = all.high,
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
                                                    ).Where(x => x.regionId == int.Parse(FreqTable)).ToList();

            var AllReg = queryTo.Where(x => x.Allocation.ToLower().Contains(tempValueAllocation.ToLower())).ToList();
            List<AllSearchFreqBand> allValues = null;
            if (tempFrom == 0 && tempTo == 0)
            {
                allValues = (from x in AllReg select (AllSearchFreqBand)x).ToList();
            }
             else if (tempFrom == 0 && tempTo != 0)
            {
                var allX = AllReg.Where(x => x.low >= tempFrom).ToList();
                allValues = allX.Where(x => x.low <= tempTo).ToList();
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
               
                var allX = AllReg.Where(x => x.high >= tempFrom).ToList();
                 allValues = allX.Where(x => x.low <= tempTo).ToList();
            }



                //Debug.WriteLine("proba:" + allS.Count);
                List<FreqBandSearch> allS = new List<FreqBandSearch>();
            foreach (var tempValue in allValues)
            {
                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = tempValue.Allocation,
                    isPrimary = tempValue.Primary,
                    Application = "",
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    isBand = tempValue.isBand,
                    Comment = "",
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

            //Debug.WriteLine("test:"+duplicates.Count());
            List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
            foreach (var d in duplicates)
            {
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
            //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);

            list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);
            return listGeneral;
   
        }
        
        public List<FreqBandSearchNew> SecondLevelSearch(AllocationDBContext _tempConAll, long tempFrom, long tempTo, string tempValueAllocation, string FreqTable)
        {
            
            _conAll = _tempConAll;
            var queryTo
                     = (from all in _conAll.AllocationRangeDb
                        join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                        join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                        join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                        join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                        from ct in g.DefaultIfEmpty()
                        where e.AllocationId != null
                        join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                        select new AllSearchFreqBand
                        {
                            low = all.low,
                            high = all.high,
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

                                     select new AllSearchFreqBand
                                     {
                                         low = all.low,
                                         high = all.high,
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
                                                    ).Where(x => x.regionId == int.Parse(FreqTable)).ToList();

            var AllReg = queryTo.Where(x => x.Allocation.ToLower().Contains(tempValueAllocation.ToLower())).ToList();
            List<AllSearchFreqBand> allValues = null;
            if (tempFrom == 0 && tempTo == 0)
            {
                allValues = (from x in AllReg select (AllSearchFreqBand)x).ToList();
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                var allX = AllReg.Where(x => x.low >= tempFrom).ToList();
                allValues = allX.Where(x => x.low <= tempTo).ToList();
            }
            else if (tempFrom != 0 && tempTo != 0)
            {

                var allX = AllReg.Where(x => x.high >= tempFrom).ToList();
                allValues = allX.Where(x => x.low <= tempTo).ToList();
            }
            
            //Debug.WriteLine("proba:" + allS.Count);
            List<FreqBandSearch> allS = new List<FreqBandSearch>();
            foreach (var tempValue in allValues)
            {
                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = tempValue.Allocation,
                    isPrimary = tempValue.Primary,
                    Application = "",
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    isBand = tempValue.isBand,
                    Comment = "",
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
            //Debug.WriteLine("test:"+duplicates.Count());
            
            List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
            foreach (var d in duplicates)
            {
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
                                                //else
                                                //{
                                                //    var item = ge.Footnote.FirstOrDefault(o => o.Allocation.Contains(ww.Allocation));
                                                //    if (item != null)
                                                //    {
                                                //        te.Allocation = "";
                                                //        ge.Footnote.Add(te);
                                                //    }
                                                //    else
                                                //    {
                                                //        ge.Footnote.Add(te);
                                                //    }
                                                //}
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
                                                //else
                                                //{
                                                //    var item = ge.Footnote.FirstOrDefault(o => o.Allocation.Contains(ww.Allocation));
                                                //    if (item != null)
                                                //    {
                                                //        te.Allocation = "";
                                                //        ge.Footnote.Add(te);
                                                //    }
                                                //    else
                                                //    {
                                                //        ge.Footnote.Add(te);
                                                //    }
                                                //}
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
                                            //var item = ge.Footnote.FirstOrDefault(o => o.Allocation.Contains(ww.Allocation));
                                            //if (item != null)
                                            //{
                                            //    te.Allocation = "";
                                            //    ge.Footnote.Add(te);
                                            //}
                                            //else
                                            //{
                                            //    ge.Footnote.Add(te);
                                            //}

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
            
            //Debug.WriteLine("dupli" + listGeneral.Count());
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
            //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);


            list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);
            return listGeneral;
           
        }
        public List<FreqBandSearchNew> SearchValuesAll(AllocationDBContext _tempConAll, string ValueAll)
        {
            _conAll = _tempConAll;
            var queryTo
                             = (from all in _conAll.AllocationRangeDb
                                join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                                join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                                join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                                join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                                from ct in g.DefaultIfEmpty()
                                where e.AllocationId != null

                                join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                                select new AllSearchFreqBand
                                {
                                    low = all.low,
                                    high = all.high,
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

                                                            select new AllSearchFreqBand
                                                            {
                                                                low = all.low,
                                                                high = all.high,
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
                                                    ).Where(x => x.regionName.Equals(ValueAll)).ToList();
            //Debug.WriteLine("tema:" + queryTo.Count);

            List<AllSearchFreqBand> allValues = (from x in queryTo select (AllSearchFreqBand)x).ToList();

            List<FreqBandSearch> allS = new List<FreqBandSearch>();
            foreach (var tempValue in allValues)
            {
                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = tempValue.Allocation,
                    isPrimary = tempValue.Primary,
                    Application = "",
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    isBand = tempValue.isBand,
                    Comment = "",
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

                listAl.Add(al);
            }

            //Debug.WriteLine("proba:" + allS.Count);
            //List<AllocationSearch> listAl = new List<AllocationSearch>();

            var ordered = listAl.OrderBy(x => x.low).ToList();
            var duplicates = ordered.OrderBy(e => e.low)
       .GroupBy(e => e.low)
       .Where(e => e.Count() > 1)
       .Select(g => new
       {
           MostRecent = g.FirstOrDefault(),
           Others = g.Skip(0).ToList()
       });
            //Debug.WriteLine("test:"+duplicates.Count());
            List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
            foreach (var d in duplicates)
            {
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
                }
                else if (d.MostRecent.BandFootnote.Count > 0)
                {
                    //Debug.WriteLine("www:" + ss.Allocation + ":varrr=" + d.MostRecent.LowView);
                    ss.Allocation = d.MostRecent.Allocation;
                    ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                    ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                    ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                    ge.BandFootnote.Add(ss);
                    ss.Footnote = "";
                    ss.FootnoteDesc = "";
                    ss.isBand = false;
                    ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                    ge.Footnote.Add(ss);
                }
                else
                {

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
                                            //var item = ge.Footnote.FirstOrDefault(o => o.Allocation.Contains(ww.Allocation));
                                            //if (item != null)
                                            //{
                                            //    te.Allocation = "";
                                            //    ge.Footnote.Add(te);
                                            //}
                                            //else
                                            //{
                                            //    ge.Footnote.Add(te);
                                            //}

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

                ge.Footnote.OrderBy(e => e.isPrimary == true);
                ge.regionCode = d.MostRecent.regionCode;
                ge.regionName = d.MostRecent.regionName;
                listGeneral.Add(ge);
                //Debug.WriteLine("tt:" + d.MostRecent.low + ", ");
            }

            //Debug.WriteLine("dupli" + listGeneral.Count());
            var Nodupl = ordered.OrderBy(e => e.low)
       .GroupBy(e => e.low)
       .Where(e => e.Count() == 1)
       .Select(g => new
       {
           MostRecent = g.FirstOrDefault(),
           Others = g.Skip(0).ToList()
       });

            // Debug.WriteLine("ne dupli" + Nodupl.Count());
            foreach (var temp in Nodupl)
            {

                List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
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
                    //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
                }

                list.OrderBy(e => e.Footnote.OrderBy(s => s.isPrimary == true));
                listGeneral.AddRange(list);
            }
            return listGeneral;
        }

        public List<FreqBandSearchNew> SearchValuesZeroTo(AllocationDBContext _tempConAll,string tempFrom,string tempTo, string ValueAll)
        {
            _conAll = _tempConAll;
            long From = long.Parse(tempFrom);
            long To = long.Parse(tempTo);
            var queryTo
                            = (from all in _conAll.AllocationRangeDb
                               join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                               join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                               join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                               join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                               from ct in g.DefaultIfEmpty()
                               where e.AllocationId != null
                                //where val.regionName.Equals()
                                join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                               select new AllSearchFreqBand
                               {
                                   low = all.low,
                                   high = all.high,
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

                                                           select new AllSearchFreqBand
                                                           {
                                                               low = all.low,
                                                               high = all.high,
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
                                                   )
                                                   //.Where(x => x.Term.ToLower().Contains(AllocationAll.ToLower()) && x.low >= long.Parse(From) && x.low <= long.Parse(To)).ToList();
                                                   .Where(x => x.regionName.Equals(ValueAll)).ToList();
            //.Where(x => x.low >= tempFrom).ToList();
            var tempList = queryTo.Where(x => x.low >= From).ToList();
            var allValues = tempList.Where(x => x.low <= To).ToList();

            //Debug.WriteLine("proba:" + allS.Count);

            //ukupno: 243
            //ukupno :494

            List<FreqBandSearch> allS = new List<FreqBandSearch>();
            foreach (var tempValue in allValues)
            {

                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = tempValue.Allocation,
                    isPrimary = tempValue.Primary,
                    Application = "",
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    isBand = tempValue.isBand,
                    Comment = "",
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


                //Debug.WriteLine("ww="+al.low+":temp:" + foot.Allocation + "==" + foot.Footnote);
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

            var valuesTempList = duplicates.ToList();
            foreach (var d in duplicates)
            {
                //Debug.WriteLine("var:" + duplicates.Count());
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
                }
                else if (d.MostRecent.BandFootnote.Count > 0)
                {
                    //Debug.WriteLine("www:" + ss.Allocation + ":varrr=" + d.MostRecent.LowView);
                    ss.Allocation = d.MostRecent.Allocation;
                    ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                    ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                    ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                    ge.BandFootnote.Add(ss);
                    ss.Footnote = "";
                    ss.FootnoteDesc = "";
                    ss.isBand = false;
                    ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                    ge.Footnote.Add(ss);
                }
                else
                {

                    ge.Footnote.Add(ss);
                }
                ge.Comment = d.MostRecent.Comment;
                bool firstAllocation = false;

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

                                //    //foreach (var qwer in p.Footnote)
                                //    //{
                                //    //    Debug.WriteLine("evo me:" + p.low + "::" + p.Allocation + "==" + qwer.Allocation + "values:" + qwer.Footnote);


                                //Debug.WriteLine("prvi ulaz:" + p.Allocation);

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
                            }

                        }
                        else
                        {
                            //foreach (var qwer in p.Footnote)
                            //{
                            //    Debug.WriteLine("va:" + p.low + "::" + p.Allocation + "===" + qwer.Allocation + "values:" + qwer.Footnote);
                            //}

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
                listGeneral.Add(ge);
                //Debug.WriteLine("tt:" + d.MostRecent.low + ", ");
            }

            //Debug.WriteLine("dupli" + listGeneral.Count());
            var Nodupl = ordered.OrderBy(e => e.low)
       .GroupBy(e => e.low)
       .Where(e => e.Count() == 1)
       .Select(g => new
       {
           MostRecent = g.FirstOrDefault(),
           Others = g.Skip(0).ToList()
       });
            // Debug.WriteLine("ne dupli" + Nodupl.Count());
            foreach (var temp in Nodupl)
            {
                List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
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

                    //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
                }
                listGeneral.AddRange(list);
            }
            return listGeneral;
            
        }
        public List<FreqBandSearchNew> SearchValuesFromTo(AllocationDBContext _tempConAll, string tempFrom, string tempTo, string ValueAll, List<SelectListItem> ListOfTables,string FrequencytableValue)
        {
            _conAll = _tempConAll;
            var queryTo
                             = (from all in _conAll.AllocationRangeDb
                                join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                                join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                                join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                                join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                                from ct in g.DefaultIfEmpty()
                                where e.AllocationId != null
                                join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                                select new AllSearchFreqBand
                                {
                                    low = all.low,
                                    high = all.high,
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

                                                            select new AllSearchFreqBand
                                                            {
                                                                low = all.low,
                                                                high = all.high,
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

                                                    )


                                                    //.Where(x => x.Term.ToLower().Contains(AllocationAll.ToLower()) && x.low >= long.Parse(From) && x.low <= long.Parse(To)).ToList();
                                                    //.Where(x => x.high >= tempFrom).ToList();
                                                    .Where(x => x.regionName.Equals(ValueAll)).ToList();
            //Debug.WriteLine("proba tt:" + queryTo.Count);
            var tempList = queryTo.Where(x => x.low >= int.Parse(tempFrom)).ToList();

            var allValues = tempList.Where(x => x.low <= int.Parse(tempTo)).ToList();

            //Debug.WriteLine("proba:" + allS.Count);

            List<FreqBandSearch> allS = new List<FreqBandSearch>();
            foreach (var tempValue in allValues)
            {

                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = tempValue.Allocation,
                    isPrimary = tempValue.Primary,
                    Application = "",
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    isBand = tempValue.isBand,
                    Comment = "",
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
            //Debug.WriteLine("test:"+duplicates.Count());
            List<FreqBandSearchNew> listGeneral = new List<FreqBandSearchNew>();
            
            foreach (var d in duplicates)
            {
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
                ge.FrequencyTablesList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
               
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
                }
                else if (d.MostRecent.BandFootnote.Count > 0)
                {
                    //Debug.WriteLine("www:" + ss.Allocation + ":varrr=" + d.MostRecent.LowView);
                    ss.Allocation = d.MostRecent.Allocation;
                    ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                    ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                    ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                    ge.BandFootnote.Add(ss);
                    ss.Footnote = "";
                    ss.FootnoteDesc = "";
                    ss.isBand = false;
                    ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                    ge.Footnote.Add(ss);
                }
                else
                {

                    ge.Footnote.Add(ss);
                }

                ge.Comment = d.MostRecent.Comment;
                bool firstAllocation = false;
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

                                //foreach (var qwer in p.Footnote)
                                //{
                                //    Debug.WriteLine("evo me:" + p.low + "::" + p.Allocation + "==" + qwer.Allocation + "values:" + qwer.Footnote);

                                //}

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

                            }

                        }
                        else
                        {
                            //foreach (var qwer in p.Footnote)
                            //{
                            //    Debug.WriteLine("va:" + p.low + "::" + p.Allocation + "===" + qwer.Allocation + "values:" + qwer.Footnote);
                            //}


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
                listGeneral.Add(ge);
                //Debug.WriteLine("tt:" + d.MostRecent.low + ", ");
            }

            //Debug.WriteLine("dupli" + listGeneral.Count());
            var Nodupl = ordered.OrderBy(e => e.low)
       .GroupBy(e => e.low)
       .Where(e => e.Count() == 1)
       .Select(g => new
       {
           MostRecent = g.FirstOrDefault(),
           Others = g.Skip(0).ToList()
       });
            // Debug.WriteLine("ne dupli" + Nodupl.Count());
            foreach (var temp in Nodupl)
            {
                List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
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

                    //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
                }
                listGeneral.AddRange(list);
            }

           
            return listGeneral;
        }
        public List<AsiaPacific> getlistOfAsiaPacific(AllocationDBContext _conAll, string[] FreqTableVal, long tempFrom, long tempTo)
        {
            List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
            try
            {
                for (int i = 0; i < FreqTableVal.Length; i++)
                {
                    List<FreqBandSearchNew> listGeneral = SearchValues(_conAll, tempFrom, tempTo, FreqTableVal[i]);
                    AsiaPacific asia = new AsiaPacific();
                    asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                    asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                    listOfAsiaPacific.Add(asia);

                }
            }
            catch (Exception)
            {
                throw;
            }
            return listOfAsiaPacific;
        }

        public List<FreqBandSearchNew> SearchValues(AllocationDBContext _tempConAll, long tempFrom, long tempTo, string FreqTable)
        {
            _conAll = _tempConAll;
            var queryTo
                     = (from all in _conAll.AllocationRangeDb
                        join e in _conAll.AllocationDb on all.AllocationRangeId equals e.AllocationRangeId
                        join alTerm in _conAll.AllocationTermDb on e.AllocationTermId equals alTerm.AllocationTermId
                        join val in _conAll.RootAllocationDB on all.RootAllocationDBId equals val.RootAllocationDBId
                        join foot in _conAll.FootnoteAllocation on e.AllocationId equals foot.AllocationId into g
                        from ct in g.DefaultIfEmpty()
                        where e.AllocationId != null
                        join foot_desc in _conAll.Footnote_description on ct.FootDescId equals foot_desc.id_foot_desc
                        select new AllSearchFreqBand
                        {
                            low = all.low,
                            high = all.high,
                            Allocation = alTerm.name,
                            TermId = e.TermId,
                            colorCode = e.colorCode,
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

                                     select new AllSearchFreqBand
                                     {
                                         low = all.low,
                                         high = all.high,
                                         Allocation = alTerm.name,
                                         TermId = e.TermId,
                                         colorCode = e.colorCode,
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
                                                    ).Where(x => x.regionId == int.Parse(FreqTable)).ToList();

            var AllReg = queryTo.ToList();
            List<AllSearchFreqBand> allValues = null;
            if (tempFrom == 0 && tempTo == 0)
            {
                allValues = AllReg;
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                var allX = AllReg.Where(x => x.low >= tempFrom).ToList();
                allValues = allX.Where(x => x.low <= tempTo).ToList();
            }
            else if (tempFrom != 0 && tempTo != 0)
            {

                var allX = AllReg.Where(x => x.high >= tempFrom).ToList();
                allValues = allX.Where(x => x.low <= tempTo).ToList();
            }

            List<FreqBandSearch> allS = new List<FreqBandSearch>();
            foreach (var tempValue in allValues)
            {
                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = tempValue.Allocation,
                    TermId = tempValue.TermId,
                    colorCode = tempValue.colorCode,
                    isPrimary = tempValue.Primary,
                    Application = "",
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    isBand = tempValue.isBand,
                    Comment = "",
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
                if (allS[i].GetType().GetProperty("Footnote") != null && allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]) != null)
                {
                    foot.Footnote = (string)allS[i].GetType().GetProperty("Footnote").GetValue(allS[i]);
                }
                if (allS[i].GetType().GetProperty("FootnoteDesc") != null && allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]) != null)
                {
                    foot.FootnoteDesc = (string)allS[i].GetType().GetProperty("FootnoteDesc").GetValue(allS[i]);
                }

                if (allS[i].GetType().GetProperty("Comment") != null && allS[i].GetType().GetProperty("Comment").GetValue(allS[i]) != null)
                {
                    comment = (string)allS[i].GetType().GetProperty("Comment").GetValue(allS[i]);
                }
                if (allS[i].GetType().GetProperty("Allocation") != null)
                {
                    foot.Allocation = (string)allS[i].GetType().GetProperty("Allocation").GetValue(allS[i]);
                }
                if (allS[i].GetType().GetProperty("colorCode") != null)
                {
                    foot.colorCode = (string)allS[i].GetType().GetProperty("colorCode").GetValue(allS[i]);
                }
                if (allS[i].GetType().GetProperty("TermId") != null)
                {
                    foot.TermId = (int?)allS[i].GetType().GetProperty("TermId").GetValue(allS[i]);
                }
                if (allS[i].GetType().GetProperty("isBand") != null)
                {
                    foot.isBand = (bool)allS[i].GetType().GetProperty("isBand").GetValue(allS[i]);
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
                    TermId = (int?)allS[i].GetType().GetProperty("TermId").GetValue(allS[i]),
                    colorCode = (string)allS[i].GetType().GetProperty("colorCode").GetValue(allS[i]),
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Comment = comment,
                    regionName = (string)allS[i].GetType().GetProperty("regionName").GetValue(allS[i]),
                    regionCode = (string)allS[i].GetType().GetProperty("regionCode").GetValue(allS[i]),
                    LowView = (string)allS[i].GetType().GetProperty("LowView").GetValue(allS[i]),
                    HighView = (string)allS[i].GetType().GetProperty("HighView").GetValue(allS[i])
                };

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
                ge.colorCode = d.MostRecent.colorCode;
                ge.TermId = d.MostRecent.TermId;
                ge.Footnote = new List<FootnoteJsonConvert>();
                ge.BandFootnote = new List<FootnoteJsonConvert>();
                FootnoteJsonConvert ss = new FootnoteJsonConvert()
                {
                    Allocation = d.MostRecent.Allocation,
                    colorCode = d.MostRecent.colorCode,
                    TermId = d.MostRecent.TermId,
                    Footnote = "",
                    FootnoteDesc = "",
                    isBand = false,
                    isPrimary = d.MostRecent.isPrimary
                };


                if (d.MostRecent.Footnote.Count > 0)
                {
                    ss.Allocation = d.MostRecent.Allocation;
                    ss.colorCode = d.MostRecent.colorCode;
                    ss.TermId = d.MostRecent.TermId;
                    ss.Footnote = d.MostRecent.Footnote[0].Footnote;
                    ss.FootnoteDesc = d.MostRecent.Footnote[0].FootnoteDesc;
                    ss.isBand = d.MostRecent.Footnote[0].isBand;
                    ss.isPrimary = d.MostRecent.Footnote[0].isPrimary;
                    ge.Footnote.Add(ss);
                }
                else if (d.MostRecent.BandFootnote.Count > 0)
                {

                    ss.Allocation = d.MostRecent.Allocation;
                    ss.colorCode = d.MostRecent.colorCode;
                    ss.TermId = d.MostRecent.TermId;
                    ss.Footnote = d.MostRecent.BandFootnote[0].Footnote;
                    ss.FootnoteDesc = d.MostRecent.BandFootnote[0].FootnoteDesc;
                    ss.isBand = d.MostRecent.BandFootnote[0].isBand;
                    ge.BandFootnote.Add(ss);
                    ss.Footnote = "";
                    ss.FootnoteDesc = "";
                    ss.isBand = false;
                    ss.isPrimary = d.MostRecent.BandFootnote[0].isPrimary;
                    ge.Footnote.Add(ss);
                }
                else
                {
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

                                            if (!ww.Footnote.Equals("") && ww.isBand == true)
                                            {
                                                te.Footnote += ww.Footnote;
                                                te.FootnoteDesc += ww.FootnoteDesc;
                                                te.isBand = ww.isBand;
                                                te.isPrimary = ww.isPrimary;
                                                ge.Footnote.Add(te);
                                            }
                                        }

                                    }
                                    if (p.Footnote.Count == 0)
                                    {
                                        FootnoteJsonConvert te = new FootnoteJsonConvert
                                        {
                                            Allocation = p.Allocation,
                                            colorCode = p.colorCode,
                                            TermId = p.TermId,
                                            Footnote = "",
                                            FootnoteDesc = "",
                                            isBand = false,
                                            isPrimary = p.isPrimary
                                        };
                                        ge.Footnote.Add(te);
                                    }

                                    ge.Allocation += ", " + p.Allocation + ", ";
                                    listOfAllocation.Add(p.Allocation);
                                    ge.count = listOfAllocation.Count();
                                }
                                else
                                {
                                    foreach (var ww in p.Footnote)
                                    {

                                        if (ww.Allocation.Equals(p.Allocation))
                                        {
                                            FootnoteJsonConvert te = new FootnoteJsonConvert
                                            {
                                                Allocation = ww.Allocation
                                            };

                                            if (!ww.Footnote.Equals(""))
                                            {
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
                                        FootnoteJsonConvert te = new FootnoteJsonConvert
                                        {
                                            Allocation = p.Allocation,
                                            colorCode = p.colorCode,
                                            TermId = p.TermId,
                                            Footnote = "",
                                            FootnoteDesc = "",
                                            isBand = false,
                                            isPrimary = p.isPrimary
                                        };
                                        ge.Footnote.Add(te);
                                    }
                                    ge.Allocation += p.Allocation + ", ";
                                    listOfAllocation.Add(p.Allocation);
                                    ge.count = listOfAllocation.Count;
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
                                    if (!ww.Footnote.Equals("") && ww.isBand == true)
                                    {
                                        te.Footnote += ww.Footnote;
                                        te.FootnoteDesc += ww.FootnoteDesc;
                                        te.isBand = ww.isBand;
                                        te.isPrimary = ww.isPrimary;
                                        ge.Footnote.Add(te);
                                    }
                                }
                            }
                            foreach (var fo in p.BandFootnote)
                            {
                                FootnoteJsonConvert te = new FootnoteJsonConvert
                                {
                                    Allocation = fo.Allocation,
                                    colorCode = fo.colorCode,
                                    TermId = fo.TermId
                                };
                                if (!fo.Footnote.Equals("") && fo.isBand == false)
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
                        if (!listOfComments.Contains(p.Comment) && !p.Comment.Equals(""))
                        {
                            ge.Comment += p.Comment + ", ";
                            listOfComments.Add(p.Comment);
                        }
                    }
                }
                ge.regionCode = d.MostRecent.regionCode;
                ge.regionName = d.MostRecent.regionName;
                ge.Footnote = ge.Footnote.OrderByDescending(t => t.isPrimary).ToList();
                listGeneral.Add(ge);
            }
            var Nodupl = ordered.OrderBy(e => e.low)
                        .GroupBy(e => e.low)
                        .Where(e => e.Count() == 1)
                            .Select(g => new
                            {
                                MostRecent = g.FirstOrDefault(),
                                Others = g.Skip(0).ToList()
                            });

            List<FreqBandSearchNew> list = new List<FreqBandSearchNew>();
            foreach (var temp in Nodupl)
            {
                foreach (var ww in temp.Others)
                {
                    FootnoteJsonConvert te = new FootnoteJsonConvert
                    {
                        Allocation = ww.Allocation,
                        colorCode = ww.colorCode,
                        TermId = ww.TermId,
                        Footnote = "",
                        FootnoteDesc = "",
                        isBand = false,
                        isPrimary = ww.isPrimary
                    };

                    ww.Footnote.Add(te);
            
                    list.Add(ww);
                }
            }
            listGeneral.AddRange(list);
            
            return listGeneral;
        }

    }
}
