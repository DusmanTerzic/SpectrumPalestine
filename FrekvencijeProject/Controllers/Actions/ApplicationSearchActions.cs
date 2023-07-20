using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Ajax;
using FrekvencijeProject.Models.ImportTempTables;
using FrekvencijeProject.Models.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FrekvencijeProject.Controllers.Actions
{
    public class ApplicationSearchActions
    {
        private ApplicationDBContext _conApp;
        private ImportTempTableContext _conImport;

        public List<FreqBandSearchNew> SearchAppAll(ApplicationDBContext _tempApp, string FrequencytableValue)
        {
            //Debug.WriteLine("rez:" + FrequencytableValue);

            _conApp = _tempApp;
            var appQuery = (from all in _conApp.Application
                            join ww in _conApp.DocumentsDb on all.documents_Id equals ww.DocumentsId
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
                                Doc_number = ww.Doc_number,
                                Title_of_doc = ww.Title_of_doc,
                                Hyperlink = ww.Hyperlink,
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
                            }).Union(from all in _conApp.Application
                                     join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                                     join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                                     join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                                     where val.regionId == int.Parse(FrequencytableValue) && all.documents_Id == null
                                     select new AllSearchFreqBand
                                     {

                                         low = e.low,
                                         high = e.high,
                                         Application = term.name,
                                         Comment = all.comment,
                                         Doc_number = "",
                                         Title_of_doc = "",
                                         Hyperlink = "",
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

            List<AllSearchFreqBand> AllAppValues = null;

            AllAppValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();

            //Debug.WriteLine("val:" + AllAppValues.Count);

            List<FreqBandSearch> allS = new List<FreqBandSearch>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearch fre = new FreqBandSearch()
                {

                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = "",
                    Application = tempValue.Application,
                    Doc_number = tempValue.Doc_number,
                    Title_of_doc = tempValue.Title_of_doc,
                    Hyperlink = tempValue.Hyperlink,
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
                string Doc_number = "";
                string Title_of_doc = "";
                string Hyperlink = "";
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
                if (allS[i].GetType().GetProperty("Doc_number") != null)
                {
                    Doc_number = (string)allS[i].GetType().GetProperty("Doc_number").GetValue(allS[i]);
                    Title_of_doc = (string)allS[i].GetType().GetProperty("Title_of_doc").GetValue(allS[i]);
                    Hyperlink = (string)allS[i].GetType().GetProperty("Hyperlink").GetValue(allS[i]);
                }
                //Debug.WriteLine("ww new:" + Doc_number);
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
                    Allocation = "",
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Doc_number = Doc_number,
                    Title_of_doc = Title_of_doc,
                    Hyperlink = Hyperlink,
                    Comment = comment,
                    regionName = (string)allS[i].GetType().GetProperty("regionName").GetValue(allS[i]),
                    regionCode = (string)allS[i].GetType().GetProperty("regionCode").GetValue(allS[i]),
                    LowView = (string)allS[i].GetType().GetProperty("LowView").GetValue(allS[i]),
                    HighView = (string)allS[i].GetType().GetProperty("HighView").GetValue(allS[i])
                };

                //if (al.Application == "PMR/PAMR")
                //{

                //Debug.WriteLine("all:" + al.low + "__" + al.Application + "::" + al.Doc_number);
                //}
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
                ge.AppItemsList = new List<ApplicationConvert>();

                //if (d.MostRecent.low == 3950000)
                //{
                //Debug.WriteLine("low one:" + d.MostRecent.low + "__" + d.MostRecent.Application + "::" + d.MostRecent.Doc_number);
                //}

                //if (d.MostRecent.Doc_number == "ECC/DEC/(19)02" || d.MostRecent.Doc_number == "T/R 25-08")  
                //{

                //    Debug.WriteLine("rrr:" + d.MostRecent.low + "__" + d.MostRecent.Application+"::"+d.MostRecent.Doc_number);
                //}


                ApplicationConvert apC2 = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment,
                    Documents = new List<DocumentsConvert>()
                };

                if (d.MostRecent.Doc_number != "")
                {
                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = d.MostRecent.Doc_number;
                    ds2.Title_of_doc = d.MostRecent.Title_of_doc;
                    ds2.Hyperlink = d.MostRecent.Hyperlink;
                    apC2.Documents.Add(ds2);
                }
                //Debug.WriteLine("ww new:" + ap.Doc_number);
                ge.AppItemsList.Add(apC2);


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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }
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
                                if (p.low == 3950000)
                                {
                                    Debug.WriteLine("wwww:" + p.low + "__" + p.Application + "::" + p.Doc_number);
                                }


                                ApplicationConvert apC3 = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment,
                                    Documents = new List<DocumentsConvert>()
                                };
                                if (p.Doc_number != "")
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();

                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    apC3.Documents.Add(ds3);
                                }
                                ge.AppItemsList.Add(apC3);

                                listOfApplication.Add(p.Application);
                            }

                        }
                        else
                        {
                            //if (p.low == 3950000)
                            //{
                            //    Debug.WriteLine("else:" + p.low + "__" + p.Application + "::" + p.Doc_number);
                            //}

                            if (p.Doc_number != "")
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Document = App.Documents.Where(v => v.Doc_number == p.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
                            }
                            //ge.AppItemsList.update(apC2);
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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
            List<string> valuesOfApp = new List<string>();
            foreach (var temp in Nodupl)
            {


                foreach (var ww in temp.Others)
                {

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();

                    //if (ww.Doc_number == "ECC/DEC/(19)02" || ww.Doc_number == "T/R 25-08")
                    //{
                    //    Debug.WriteLine("others:" + ww.low + "__" + ww.Application + "==" + ww.Doc_number);
                    //}

                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = temp.MostRecent.Doc_number;
                    ds2.Title_of_doc = temp.MostRecent.Title_of_doc;
                    ds2.Hyperlink = temp.MostRecent.Hyperlink;
                    ApplicationConvert apC2 = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment,
                        Documents = new List<DocumentsConvert>()
                    };
                    apC2.Documents.Add(ds2);


                    //ApplicationConvert ap = new ApplicationConvert()
                    //{
                    //    Application = temp.MostRecent.Application,
                    //    Comment = temp.MostRecent.Comment,
                    //    Doc_number = temp.MostRecent.Doc_number,
                    //    Title_of_doc = temp.MostRecent.Title_of_doc,
                    //    Hyperlink = temp.MostRecent.Hyperlink
                    //};
                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);
                    ww.AppItemsList.Add(apC2);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            foreach (var values in listGeneral)
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
            }
            //Debug.WriteLine("radi:" + listGeneral.Count);
            return listGeneral.ToList();

        }


        public List<FreqBandSearchNewDocStand> SearchAppAllNew(ApplicationDBContext _tempApp, string FrequencytableValue)
        {
            _conApp = _tempApp;
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
                                Doc_number = "",
                                Title_of_doc = "",
                                Hyperlink = "",
                                Allocation = "",
                                Footnote = "",
                                FootnoteDesc = "",
                                isBand = false,
                                Primary = false,
                                regionName = val.regionName,
                                regionCode = val.regionCode,
                                regionId = val.regionId,
                                LowView = e.LowView,
                                HighView = e.HighView,
                                documents_Id = all.documents_Id,
                                Standard_id = all.Standard_id
                            }).ToList().OrderBy(p => p.low);
            List<AllSearchFreqBand> AllAppValues = null;

            AllAppValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();


            List<FreqBandSearchDocStand> allS = new List<FreqBandSearchDocStand>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearchDocStand fre = new FreqBandSearchDocStand()
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
                    HighView = tempValue.HighView,
                    documents_Id = tempValue.documents_Id,
                    Standard_id = tempValue.Standard_id
                };
                allS.Add(fre);
            }

            List<FreqBandSearchNewDocStand> listAl = new List<FreqBandSearchNewDocStand>();
            for (int i = 0; i < allS.Count; i++)
            {
                FootnoteJsonConvert foot = new FootnoteJsonConvert();
                string comment = "";
                string Doc_number = "";
                string Title_of_doc = "";
                string Hyperlink = "";

                string Etsi_standard = "";
                string Title_docStand = "";
                string HypelinkStand = "";
                int Standid = 0;
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

                //Debug.WriteLine("ww new:" + Doc_number);
                if (allS[i].documents_Id != null)
                {
                    var appQueryDoc = (from all in _conApp.DocumentsDb
                                       select new
                                       {
                                           documents_id = all.DocumentsId,
                                           Doc_number = all.Doc_number,
                                           Title_of_doc = all.Title_of_doc,
                                           Hyperlink = all.Hyperlink
                                       }).ToList();
                    int id = (int)allS[i].documents_Id;
                    var Doc = appQueryDoc.Where(x => x.documents_id == id).SingleOrDefault();
                    Doc_number = Doc.Doc_number;
                    Title_of_doc = Doc.Title_of_doc;
                    Hyperlink = Doc.Hyperlink;

                }

                if (allS[i].Standard_id != null)
                {
                    var appQueryStand = (from all in _conApp.StandardsDb
                                         select new
                                         {
                                             standard_id = all.Standard_id,
                                             Etsi_standard = all.Etsi_standard,
                                             Title_of_doc = all.Title_doc,
                                             Hyperlink = all.Hypelink
                                         }).ToList();
                    Standid = (int)allS[i].Standard_id;
                    var Stand = appQueryStand.Where(x => x.standard_id == Standid).SingleOrDefault();
                    Etsi_standard = Stand.Etsi_standard;
                    Title_docStand = Stand.Title_of_doc;
                    HypelinkStand = Stand.Hyperlink;
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



                FreqBandSearchNewDocStand al = new FreqBandSearchNewDocStand()
                {
                    low = (long)allS[i].GetType().GetProperty("low").GetValue(allS[i]),
                    high = (long)allS[i].GetType().GetProperty("high").GetValue(allS[i]),
                    Application = (string)allS[i].GetType().GetProperty("Application").GetValue(allS[i]),
                    Allocation = "",
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Doc_number = Doc_number,
                    Title_of_doc = Title_of_doc,
                    Hyperlink = Hyperlink,
                    StandardId = Standid,
                    Etsi_standard = Etsi_standard,
                    Title_docS = Title_docStand,
                    HypelinkS = HypelinkStand,
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

            List<FreqBandSearchNewDocStand> listGeneral = new List<FreqBandSearchNewDocStand>();
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
                List<FreqBandSearchNewDocStand> others = d.Others;
                FreqBandSearchNewDocStand ge = new FreqBandSearchNewDocStand();
                ge.low = d.MostRecent.low;
                ge.high = d.MostRecent.high;
                ge.LowView = d.MostRecent.LowView;
                ge.HighView = d.MostRecent.HighView;
                ge.Application = d.MostRecent.Application;
                ge.Allocation = d.MostRecent.Allocation;
                ge.Footnote = new List<FootnoteJsonConvert>();
                ge.BandFootnote = new List<FootnoteJsonConvert>();
                ge.AppItemsList = new List<ApplicationConvert>();


                ApplicationConvert apC2 = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment,
                    Documents = new List<DocumentsConvert>(),
                    Standards = new List<StandardsConvert>()
                };

                if (d.MostRecent.Doc_number != "")
                {
                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = d.MostRecent.Doc_number;
                    ds2.Title_of_doc = d.MostRecent.Title_of_doc;
                    ds2.Hyperlink = d.MostRecent.Hyperlink;
                    apC2.Documents.Add(ds2);
                }

                if (d.MostRecent.StandardId > 0)
                {
                    //if(ge.LowView == "135.7 kHz")
                    //{
                    //    Debug.WriteLine("135.7 kHz:" + d.MostRecent.StandardId);
                    //}
                    StandardsConvert sc = new StandardsConvert();
                    sc.StandardId = d.MostRecent.StandardId;
                    sc.Etsi_standard = d.MostRecent.Etsi_standard;
                    sc.Title_docS = d.MostRecent.Title_docS;
                    sc.HyperlinkS = d.MostRecent.HypelinkS;
                    apC2.Standards.Add(sc);
                }
                //Debug.WriteLine("ww new:" + ap.Doc_number);
                ge.AppItemsList.Add(apC2);


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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }
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

                                ApplicationConvert apC3 = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment,
                                    Documents = new List<DocumentsConvert>(),
                                    Standards = new List<StandardsConvert>()

                                };
                                if (p.Doc_number != "")
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    apC3.Documents.Add(ds3);
                                }

                                if (p.StandardId > 0)
                                {
                                    //if (ge.LowView == "135.7 kHz")
                                    //{
                                    //    Debug.WriteLine("135.7 kHz ddd:" + p.StandardId);
                                    //}
                                    StandardsConvert sc = new StandardsConvert();
                                    sc.StandardId = p.StandardId;
                                    sc.Etsi_standard = p.Etsi_standard;
                                    sc.Title_docS = p.Title_docS;
                                    sc.HyperlinkS = p.HypelinkS;
                                    apC3.Standards.Add(sc);
                                }
                                ge.AppItemsList.Add(apC3);

                                listOfApplication.Add(p.Application);
                            }

                        }
                        else
                        {


                            if (p.Doc_number != "")
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Document = App.Documents.Where(v => v.Doc_number == p.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
                            }

                            if (p.StandardId > 0)
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Standard = App.Standards.Where(v => v.StandardId == p.StandardId).SingleOrDefault();
                                if (Standard == null)
                                {
                                    //if (ge.LowView == "135.7 kHz")
                                    //{
                                    //    Debug.WriteLine("135.7 kHz stand:" + p.StandardId);
                                    //}


                                    StandardsConvert sc = new StandardsConvert();
                                    sc.StandardId = p.StandardId;
                                    sc.Etsi_standard = p.Etsi_standard;
                                    sc.Title_docS = p.Title_docS;
                                    sc.HyperlinkS = p.HypelinkS;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Standards.Add(sc);
                                }
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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
            List<FreqBandSearchNewDocStand> list = new List<FreqBandSearchNewDocStand>();
            List<string> valuesOfApp = new List<string>();
            foreach (var temp in Nodupl)
            {


                foreach (var ww in temp.Others)
                {

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();

                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = temp.MostRecent.Doc_number;
                    ds2.Title_of_doc = temp.MostRecent.Title_of_doc;
                    ds2.Hyperlink = temp.MostRecent.Hyperlink;

                    StandardsConvert sc = new StandardsConvert();
                    sc.StandardId = temp.MostRecent.StandardId;
                    sc.Etsi_standard = temp.MostRecent.Etsi_standard;
                    sc.Title_docS = temp.MostRecent.Title_docS;
                    sc.HyperlinkS = temp.MostRecent.HypelinkS;


                    ApplicationConvert apC2 = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment,
                        Documents = new List<DocumentsConvert>(),
                        Standards = new List<StandardsConvert>()
                    };
                    apC2.Documents.Add(ds2);
                    apC2.Standards.Add(sc);


                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);
                    ww.AppItemsList.Add(apC2);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            foreach (var values in listGeneral)
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
            }

            //Debug.WriteLine("radi:" + listGeneral.Count);
            return listGeneral.ToList();
        }


        public List<ApplicationView> SearchAppAllNewPerfomance(ApplicationDBContext _tempApp, string FrequencytableValue)
        {
            _conApp = _tempApp;
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
                                isDeletedApp = all.isDeletedApp,
                                Comment = all.comment,
                                Doc_number = "",
                                Title_of_doc = "",
                                Hyperlink = "",
                                Allocation = "",
                                Footnote = "",
                                FootnoteDesc = "",
                                isBand = false,
                                Primary = false,
                                regionName = val.regionName,
                                regionCode = val.regionCode,
                                regionId = val.regionId,
                                LowView = e.LowView,
                                HighView = e.HighView,
                                documents_Id = all.documents_Id,
                                Standard_id = all.Standard_id
                            }).ToList().OrderBy(p => p.low);
            List<AllSearchFreqBand> AllAppValues = null;

            AllAppValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();


            List<FreqBandSearchDocStand> allS = new List<FreqBandSearchDocStand>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearchDocStand fre = new FreqBandSearchDocStand()
                {

                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = "",
                    Application = tempValue.Application,
                    isDeletedApp = tempValue.isDeletedApp,
                    Footnote = "",
                    FootnoteDesc = "",
                    Comment = tempValue.Comment,
                    regionName = tempValue.regionName,
                    regionCode = tempValue.regionCode,
                    LowView = tempValue.LowView,
                    HighView = tempValue.HighView,
                    documents_Id = tempValue.documents_Id,
                    Standard_id = tempValue.Standard_id
                };
                allS.Add(fre);
            }



            var ordered = allS.OrderBy(x => x.low).ToList();

            var duplicates = ordered.OrderBy(e => e.low)
              .GroupBy(e => e.low)
              .Where(e => e.Count() > 1)
              .Select(g => new
              {
                  MostRecent = g.FirstOrDefault(),
                  Others = g.Skip(0).ToList()
              });

            //List<FreqBandSearchNewDocStand> listGeneral = new List<FreqBandSearchNewDocStand>();
            List<ApplicationView> listOfAppGeneral = new List<ApplicationView>();
            foreach (var d in duplicates)
            {

                List<string> listOfApplication = new List<string>();
                if (!listOfApplication.Contains(d.MostRecent.Application))
                {
                    ApplicationView appView = new ApplicationView();

                    appView.low = d.MostRecent.low;
                    appView.high = d.MostRecent.high;

                    appView.LowView = d.MostRecent.LowView;
                    appView.HighView = d.MostRecent.HighView;

                    if (d.MostRecent.LowView == "29.7 MHz")
                    {
                        Debug.WriteLine(d.MostRecent.LowView + "==" + d.MostRecent.Application + "==" + d.MostRecent.isDeletedApp);
                    }

                    appView.Application = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();


                    app.Application = d.MostRecent.Application;
                    app.isDeletedApp = d.MostRecent.isDeletedApp;
                    app.Comment = d.MostRecent.Comment;
                    //Debug.WriteLine(appView.LowView+":rrr:" + app.Application);
                    listOfApplication.Add(d.MostRecent.Application);
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    //appView.Comment = d.MostRecent.Comment;
                    //appView.DocumentId = new List<int>();
                    //appView.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    if (d.MostRecent.documents_Id != null || d.MostRecent.documents_Id > 0)
                    {
                        //appView.DocumentId.Add((int)d.MostRecent.documents_Id);
                        //app.DocumentId.Add((int)d.MostRecent.documents_Id);
                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == d.MostRecent.documents_Id).SingleOrDefault();
                        if (doc.Type_of_doc == "R")
                        {
                            DocumentConvertNew rr = new DocumentConvertNew
                            {
                                DocumentId = (int)d.MostRecent.documents_Id,
                                isRegulatory = true
                            };
                            app.DocumentsAditional.Add(rr);
                        }
                        else if (doc.Type_of_doc == "I")
                        {
                            DocumentConvertNew rr = new DocumentConvertNew
                            {
                                DocumentId = (int)d.MostRecent.documents_Id,
                                isRegulatory = false
                            };
                            app.DocumentsAditional.Add(rr);
                        }
                    }

                    if (d.MostRecent.Standard_id != null || d.MostRecent.Standard_id > 0)
                    {
                        //appView.StandardId.Add((int)d.MostRecent.Standard_id);
                        //app.StandardId.Add((int)d.MostRecent.Standard_id);
                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == d.MostRecent.Standard_id).SingleOrDefault();
                        if (stand.Type_of_Document == "R")
                        {
                            StandardsConvertNew ss = new StandardsConvertNew()
                            {
                                StandardId = (int)d.MostRecent.Standard_id,
                                isRegulatoryStand = true
                            };
                            app.StandardsAditional.Add(ss);
                        }
                        else if (stand.Type_of_Document == "I")
                        {
                            StandardsConvertNew ss = new StandardsConvertNew()
                            {
                                StandardId = (int)d.MostRecent.Standard_id,
                                isRegulatoryStand = false
                            };
                            app.StandardsAditional.Add(ss);
                        }
                    }

                    foreach (var p in d.Others)
                    {

                        if (d.MostRecent.low == p.low)
                        {

                            if (!listOfApplication.Contains(p.Application))
                            {
                                if (p.LowView == "29.7 MHz")
                                {
                                    Debug.WriteLine(p.LowView + "==" + p.Application + "==" + p.isDeletedApp);
                                }
                                if (!p.Application.Equals(""))
                                {


                                    ApplicationConvert app2 = new ApplicationConvert();
                                    app2.Application = p.Application;
                                    app2.isDeletedApp = p.isDeletedApp;
                                    app2.Comment = p.Comment;


                                    //app2.DocumentId = new List<int>();
                                    //app2.StandardId = new List<int>();
                                    app2.DocumentsAditional = new List<DocumentConvertNew>();
                                    app2.StandardsAditional = new List<StandardsConvertNew>();

                                    if (p.documents_Id != null || p.documents_Id > 0)
                                    {

                                        //app2.DocumentId.Add((int)p.documents_Id);

                                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == p.documents_Id).SingleOrDefault();
                                        if (doc.Type_of_doc == "R")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = true
                                            };
                                            app2.DocumentsAditional.Add(rr);
                                        }
                                        else if (doc.Type_of_doc == "I")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = false
                                            };
                                            app2.DocumentsAditional.Add(rr);
                                        }

                                    }

                                    if (p.Standard_id != null || p.Standard_id > 0)
                                    {
                                        //app2.StandardId.Add((int)p.Standard_id);

                                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == p.Standard_id).SingleOrDefault();
                                        if (stand.Type_of_Document == "R")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = true
                                            };
                                            app2.StandardsAditional.Add(ss);
                                        }
                                        else if (stand.Type_of_Document == "I")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = false
                                            };
                                            app2.StandardsAditional.Add(ss);

                                        }
                                    }
                                    appView.Application.Add(app2);



                                    listOfApplication.Add(p.Application);


                                }
                            }
                            else
                            {
                                bool IsFirst = false;
                                if (appView.Application.Count == 0)
                                {
                                    //if (appView.LowView.Equals("9 kHz"))
                                    //{
                                    //    Debug.WriteLine("im here:" + app.DocumentId.Count + "::" + p.documents_Id);

                                    IsFirst = true;
                                    //}
                                    appView.Application.Add(app);

                                }
                                //Debug.WriteLine("im here:" + app.DocumentsAditional.Count + "::" + p.documents_Id);
                                if (!IsFirst)
                                {
                                    var tempApp = appView.Application.Where(x => x.Application == p.Application).SingleOrDefault();
                                    //old way of adding of docId
                                    //if (appView.Application.Find(u => u.Application == p.Application).DocumentId == null)
                                    //{
                                    //    appView.Application.Find(u => u.Application == p.Application).DocumentId = new List<int>();
                                    //}

                                    //if (appView.Application.Find(u => u.Application == p.Application).StandardId == null)
                                    //{
                                    //    appView.Application.Find(u => u.Application == p.Application).StandardId = new List<int>();
                                    //}
                                    if (appView.Application.Find(u => u.Application == p.Application).DocumentsAditional == null)
                                    {
                                        appView.Application.Find(u => u.Application == p.Application).DocumentsAditional = new List<DocumentConvertNew>();
                                    }

                                    if (appView.Application.Find(u => u.Application == p.Application).StandardsAditional == null)
                                    {
                                        appView.Application.Find(u => u.Application == p.Application).StandardsAditional = new List<StandardsConvertNew>();
                                    }




                                    if (p.documents_Id > 0)
                                    {

                                        //app.DocumentId.Add((int)p.documents_Id);
                                        //appView.Application.Find(u => u.Application == p.Application).DocumentId.Add((int)p.documents_Id);

                                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == p.documents_Id).SingleOrDefault();
                                        if (doc.Type_of_doc == "R")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = true
                                            };
                                            //app2.DocumentsAditional.Add(rr);
                                            appView.Application.Find(u => u.Application == p.Application).DocumentsAditional.Add(rr);
                                        }
                                        else if (doc.Type_of_doc == "I")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = false
                                            };
                                            //app2.DocumentsAditional.Add(rr);
                                            appView.Application.Find(u => u.Application == p.Application).DocumentsAditional.Add(rr);
                                        }

                                    }

                                    if (p.Standard_id > 0)
                                    {
                                        //appView.Application.Find(u => u.Application == p.Application).StandardId.Add((int)p.Standard_id);
                                        //app.StandardId.Add((int)p.Standard_id);
                                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == p.Standard_id).SingleOrDefault();

                                        if (stand.Type_of_Document == "R")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = true
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).StandardsAditional.Add(ss);
                                        }
                                        else if (stand.Type_of_Document == "I")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = false
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).StandardsAditional.Add(ss);
                                        }
                                    }
                                }

                                //    //tempApp = app;
                                //    //appView.Application = tempApp ;
                            }

                        }

                    }

                    if (appView.Application.Count == 0)
                    {

                        //if (appView.LowView.Equals("9 kHz"))
                        //{
                        //    Debug.WriteLine("www");
                        //}
                        appView.Application.Add(app);
                    }


                    //if (appView.LowView == "11.3 kHz")
                    //{
                    //    foreach (var ww in appView.Application)
                    //    {
                    //        if (ww.Application == "ISM")
                    //        {
                    //            Debug.WriteLine("www:" + ww.StandardId.Count);
                    //        }
                    //    }

                    //}

                    appView.regionCode = d.MostRecent.regionCode;
                    appView.regionName = d.MostRecent.regionName;

                    listOfAppGeneral.Add(appView);
                }


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
            //List<FreqBandSearchNewDocStand> list = new List<FreqBandSearchNewDocStand>();
            List<ApplicationView> listOfApp = new List<ApplicationView>();
            foreach (var temp in Nodupl)
            {

                ApplicationView appView = new ApplicationView();
                appView.low = temp.MostRecent.low;
                appView.high = temp.MostRecent.high;
                appView.LowView = temp.MostRecent.LowView;
                appView.HighView = temp.MostRecent.HighView;

                if (temp.MostRecent.LowView == "29.7 MHz")
                {
                    Debug.WriteLine("no double:" + temp.MostRecent.LowView + "==" + temp.MostRecent.Application + "==" + temp.MostRecent.isDeletedApp);
                }

                appView.Application = new List<ApplicationConvert>();
                ApplicationConvert ww = new ApplicationConvert();
                ww.Application = temp.MostRecent.Application;
                ww.isDeletedApp = temp.MostRecent.isDeletedApp;
                ww.Comment = temp.MostRecent.Comment;
                //ww.DocumentId = new List<int>();
                //ww.StandardId = new List<int>();
                ww.DocumentsAditional = new List<DocumentConvertNew>();
                ww.StandardsAditional = new List<StandardsConvertNew>();
                if (temp.MostRecent.documents_Id > 0)
                {
                    //appView.DocumentId.Add((int)temp.MostRecent.documents_Id);
                    var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == temp.MostRecent.documents_Id).SingleOrDefault();
                    if (doc.Type_of_doc == "R")
                    {
                        DocumentConvertNew dd = new DocumentConvertNew()
                        {
                            DocumentId = (int)temp.MostRecent.documents_Id,
                            isRegulatory = true
                        };

                        ww.DocumentsAditional.Add(dd);
                    }
                    else if (doc.Type_of_doc == "I")
                    {
                        DocumentConvertNew dd = new DocumentConvertNew()
                        {
                            DocumentId = (int)temp.MostRecent.documents_Id,
                            isRegulatory = false
                        };

                        ww.DocumentsAditional.Add(dd);
                    }
                    //ww.DocumentId.Add((int)temp.MostRecent.documents_Id);
                }

                if (temp.MostRecent.Standard_id > 0)
                {
                    var stand = _conApp.StandardsDb.Where(x => x.Standard_id == temp.MostRecent.Standard_id).SingleOrDefault();
                    if (stand.Type_of_Document == "R")
                    {
                        StandardsConvertNew dd = new StandardsConvertNew()
                        {
                            StandardId = (int)temp.MostRecent.Standard_id,
                            isRegulatoryStand = true
                        };

                        ww.StandardsAditional.Add(dd);
                    }
                    else if (stand.Type_of_Document == "I")
                    {
                        StandardsConvertNew dd = new StandardsConvertNew()
                        {
                            StandardId = (int)temp.MostRecent.Standard_id,
                            isRegulatoryStand = false
                        };

                        ww.StandardsAditional.Add(dd);
                    }


                    //ww.StandardId.Add((int)temp.MostRecent.Standard_id);
                    //appView.StandardId.Add((int)temp.MostRecent.Standard_id);
                }
                appView.regionCode = temp.MostRecent.regionCode;
                appView.regionName = temp.MostRecent.regionName;
                appView.Application.Add(ww);


                listOfAppGeneral.Add(appView);

                //Debug.WriteLine("list of app count:" + temp.Others.Count);
            }
            //var endList = listOfAppGeneral.OrderBy(x => x.Application).ToList();

            return listOfAppGeneral;
        }

        public List<FreqBandSearchNewDocStand> SearchAppAllZeroToNew(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue)
        {
            _conApp = _tempApp;
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
                                Doc_number = "",
                                Title_of_doc = "",
                                Hyperlink = "",
                                Allocation = "",
                                Footnote = "",
                                FootnoteDesc = "",
                                isBand = false,
                                Primary = false,
                                regionName = val.regionName,
                                regionCode = val.regionCode,
                                regionId = val.regionId,
                                LowView = e.LowView,
                                HighView = e.HighView,
                                documents_Id = all.documents_Id,
                                Standard_id = all.Standard_id
                            }).ToList().OrderBy(p => p.low);
            List<AllSearchFreqBand> AllAppValues = null;

            var values = (from x in appQuery select (AllSearchFreqBand)x).ToList();


            var allY = values.Where(x => x.low >= tempFrom).ToList();
            AllAppValues = allY.Where(x => x.low <= tempTo).ToList();

            List<FreqBandSearchDocStand> allS = new List<FreqBandSearchDocStand>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearchDocStand fre = new FreqBandSearchDocStand()
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
                    HighView = tempValue.HighView,
                    documents_Id = tempValue.documents_Id,
                    Standard_id = tempValue.Standard_id
                };
                allS.Add(fre);
            }

            List<FreqBandSearchNewDocStand> listAl = new List<FreqBandSearchNewDocStand>();
            for (int i = 0; i < allS.Count; i++)
            {
                FootnoteJsonConvert foot = new FootnoteJsonConvert();
                string comment = "";
                string Doc_number = "";
                string Title_of_doc = "";
                string Hyperlink = "";

                string Etsi_standard = "";
                string Title_docStand = "";
                string HypelinkStand = "";
                int Standid = 0;
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

                //Debug.WriteLine("ww new:" + Doc_number);
                if (allS[i].documents_Id != null)
                {
                    var appQueryDoc = (from all in _conApp.DocumentsDb
                                       select new
                                       {
                                           documents_id = all.DocumentsId,
                                           Doc_number = all.Doc_number,
                                           Title_of_doc = all.Title_of_doc,
                                           Hyperlink = all.Hyperlink
                                       }).ToList();
                    int id = (int)allS[i].documents_Id;
                    var Doc = appQueryDoc.Where(x => x.documents_id == id).SingleOrDefault();
                    Doc_number = Doc.Doc_number;
                    Title_of_doc = Doc.Title_of_doc;
                    Hyperlink = Doc.Hyperlink;

                }

                if (allS[i].Standard_id != null)
                {
                    var appQueryStand = (from all in _conApp.StandardsDb
                                         select new
                                         {
                                             standard_id = all.Standard_id,
                                             Etsi_standard = all.Etsi_standard,
                                             Title_of_doc = all.Title_doc,
                                             Hyperlink = all.Hypelink
                                         }).ToList();
                    Standid = (int)allS[i].Standard_id;
                    var Stand = appQueryStand.Where(x => x.standard_id == Standid).SingleOrDefault();
                    Etsi_standard = Stand.Etsi_standard;
                    Title_docStand = Stand.Title_of_doc;
                    HypelinkStand = Stand.Hyperlink;
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



                FreqBandSearchNewDocStand al = new FreqBandSearchNewDocStand()
                {
                    low = (long)allS[i].GetType().GetProperty("low").GetValue(allS[i]),
                    high = (long)allS[i].GetType().GetProperty("high").GetValue(allS[i]),
                    Application = (string)allS[i].GetType().GetProperty("Application").GetValue(allS[i]),
                    Allocation = "",
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Doc_number = Doc_number,
                    Title_of_doc = Title_of_doc,
                    Hyperlink = Hyperlink,
                    StandardId = Standid,
                    Etsi_standard = Etsi_standard,
                    Title_docS = Title_docStand,
                    HypelinkS = HypelinkStand,
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

            List<FreqBandSearchNewDocStand> listGeneral = new List<FreqBandSearchNewDocStand>();
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
                List<FreqBandSearchNewDocStand> others = d.Others;
                FreqBandSearchNewDocStand ge = new FreqBandSearchNewDocStand();
                ge.low = d.MostRecent.low;
                ge.high = d.MostRecent.high;
                ge.LowView = d.MostRecent.LowView;
                ge.HighView = d.MostRecent.HighView;
                ge.Application = d.MostRecent.Application;
                ge.Allocation = d.MostRecent.Allocation;
                ge.Footnote = new List<FootnoteJsonConvert>();
                ge.BandFootnote = new List<FootnoteJsonConvert>();
                ge.AppItemsList = new List<ApplicationConvert>();


                ApplicationConvert apC2 = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment,
                    Documents = new List<DocumentsConvert>(),
                    Standards = new List<StandardsConvert>()
                };

                if (d.MostRecent.Doc_number != "")
                {
                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = d.MostRecent.Doc_number;
                    ds2.Title_of_doc = d.MostRecent.Title_of_doc;
                    ds2.Hyperlink = d.MostRecent.Hyperlink;
                    apC2.Documents.Add(ds2);
                }

                if (d.MostRecent.StandardId > 0)
                {

                    StandardsConvert sc = new StandardsConvert();
                    sc.StandardId = d.MostRecent.StandardId;
                    sc.Etsi_standard = d.MostRecent.Etsi_standard;
                    sc.Title_docS = d.MostRecent.Title_docS;
                    sc.HyperlinkS = d.MostRecent.HypelinkS;
                    apC2.Standards.Add(sc);
                }
                //Debug.WriteLine("ww new:" + ap.Doc_number);
                ge.AppItemsList.Add(apC2);


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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }
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

                                ApplicationConvert apC3 = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment,
                                    Documents = new List<DocumentsConvert>(),
                                    Standards = new List<StandardsConvert>()

                                };
                                if (p.Doc_number != "")
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    apC3.Documents.Add(ds3);
                                }

                                if (p.StandardId > 0)
                                {

                                    StandardsConvert sc = new StandardsConvert();
                                    sc.StandardId = p.StandardId;
                                    sc.Etsi_standard = p.Etsi_standard;
                                    sc.Title_docS = p.Title_docS;
                                    sc.HyperlinkS = p.HypelinkS;
                                    apC3.Standards.Add(sc);
                                }
                                ge.AppItemsList.Add(apC3);

                                listOfApplication.Add(p.Application);
                            }

                        }
                        else
                        {


                            if (p.Doc_number != "")
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Document = App.Documents.Where(v => v.Doc_number == p.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
                            }

                            if (p.StandardId > 0)
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Standard = App.Standards.Where(v => v.StandardId == p.StandardId).SingleOrDefault();
                                if (Standard == null)
                                {

                                    StandardsConvert sc = new StandardsConvert();
                                    sc.StandardId = p.StandardId;
                                    sc.Etsi_standard = p.Etsi_standard;
                                    sc.Title_docS = p.Title_docS;
                                    sc.HyperlinkS = p.HypelinkS;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Standards.Add(sc);
                                }
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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
            // Debug.WriteLine("ne dupli" + Nodupl.Count());
            List<FreqBandSearchNewDocStand> list = new List<FreqBandSearchNewDocStand>();
            List<string> valuesOfApp = new List<string>();
            foreach (var temp in Nodupl)
            {


                foreach (var ww in temp.Others)
                {

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();



                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = temp.MostRecent.Doc_number;
                    ds2.Title_of_doc = temp.MostRecent.Title_of_doc;
                    ds2.Hyperlink = temp.MostRecent.Hyperlink;

                    StandardsConvert sc = new StandardsConvert();
                    sc.StandardId = temp.MostRecent.StandardId;
                    sc.Etsi_standard = temp.MostRecent.Etsi_standard;
                    sc.Title_docS = temp.MostRecent.Title_docS;
                    sc.HyperlinkS = temp.MostRecent.HypelinkS;


                    ApplicationConvert apC2 = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment,
                        Documents = new List<DocumentsConvert>(),
                        Standards = new List<StandardsConvert>()
                    };
                    apC2.Documents.Add(ds2);
                    apC2.Standards.Add(sc);


                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);
                    ww.AppItemsList.Add(apC2);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            foreach (var valuesGen in listGeneral)
            {
                valuesGen.AppItemsList = valuesGen.AppItemsList.OrderBy(x => x.Application).ToList();
            }
            //Debug.WriteLine("radi:" + listGeneral.Count);
            return listGeneral.ToList();

        }

        public List<ApplicationView> SearchAppAllZeroToNewPerfomance(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue)
        {
            _conApp = _tempApp;
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
                                isDeletedApp = all.isDeletedApp,
                                Comment = all.comment,
                                Doc_number = "",
                                Title_of_doc = "",
                                Hyperlink = "",
                                Allocation = "",
                                Footnote = "",
                                FootnoteDesc = "",
                                isBand = false,
                                Primary = false,
                                regionName = val.regionName,
                                regionCode = val.regionCode,
                                regionId = val.regionId,
                                LowView = e.LowView,
                                HighView = e.HighView,
                                documents_Id = all.documents_Id,
                                Standard_id = all.Standard_id
                            }).ToList().OrderBy(p => p.low);

            List<AllSearchFreqBand> AllAppValues = null;

            var values = (from x in appQuery select (AllSearchFreqBand)x).ToList();


            var allY = values.Where(x => x.low >= tempFrom).ToList();
            AllAppValues = allY.Where(x => x.low <= tempTo).ToList();

            List<FreqBandSearchDocStand> allS = new List<FreqBandSearchDocStand>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearchDocStand fre = new FreqBandSearchDocStand()
                {

                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = "",
                    Application = tempValue.Application,
                    isDeletedApp = tempValue.isDeletedApp,
                    Footnote = "",
                    FootnoteDesc = "",
                    Comment = tempValue.Comment,
                    regionName = tempValue.regionName,
                    regionCode = tempValue.regionCode,
                    LowView = tempValue.LowView,
                    HighView = tempValue.HighView,
                    documents_Id = tempValue.documents_Id,
                    Standard_id = tempValue.Standard_id
                };
                allS.Add(fre);
            }



            var ordered = allS.OrderBy(x => x.low).ToList();

            var duplicates = ordered.OrderBy(e => e.low)
              .GroupBy(e => e.low)
              .Where(e => e.Count() > 1)
              .Select(g => new
              {
                  MostRecent = g.FirstOrDefault(),
                  Others = g.Skip(0).ToList()
              });

            //List<FreqBandSearchNewDocStand> listGeneral = new List<FreqBandSearchNewDocStand>();
            List<ApplicationView> listOfAppGeneral = new List<ApplicationView>();
            foreach (var d in duplicates)
            {

                List<string> listOfApplication = new List<string>();
                if (!listOfApplication.Contains(d.MostRecent.Application))
                {
                    ApplicationView appView = new ApplicationView();

                    appView.low = d.MostRecent.low;
                    appView.high = d.MostRecent.high;

                    appView.LowView = d.MostRecent.LowView;
                    appView.HighView = d.MostRecent.HighView;

                    appView.Application = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();


                    app.Application = d.MostRecent.Application;
                    app.isDeletedApp = d.MostRecent.isDeletedApp;
                    app.Comment = d.MostRecent.Comment;
                    //Debug.WriteLine(appView.LowView+":rrr:" + app.Application);
                    listOfApplication.Add(d.MostRecent.Application);
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    //appView.Comment = d.MostRecent.Comment;
                    //appView.DocumentId = new List<int>();
                    //appView.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    if (d.MostRecent.documents_Id != null || d.MostRecent.documents_Id > 0)
                    {
                        //appView.DocumentId.Add((int)d.MostRecent.documents_Id);
                        //app.DocumentId.Add((int)d.MostRecent.documents_Id);
                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == d.MostRecent.documents_Id).SingleOrDefault();
                        if (doc.Type_of_doc == "R")
                        {
                            DocumentConvertNew rr = new DocumentConvertNew
                            {
                                DocumentId = (int)d.MostRecent.documents_Id,
                                isRegulatory = true
                            };
                            app.DocumentsAditional.Add(rr);
                        }
                        else if (doc.Type_of_doc == "I")
                        {
                            DocumentConvertNew rr = new DocumentConvertNew
                            {
                                DocumentId = (int)d.MostRecent.documents_Id,
                                isRegulatory = false
                            };
                            app.DocumentsAditional.Add(rr);
                        }
                    }

                    if (d.MostRecent.Standard_id != null || d.MostRecent.Standard_id > 0)
                    {
                        //appView.StandardId.Add((int)d.MostRecent.Standard_id);
                        //app.StandardId.Add((int)d.MostRecent.Standard_id);
                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == d.MostRecent.Standard_id).SingleOrDefault();
                        if (stand.Type_of_Document == "R")
                        {
                            StandardsConvertNew ss = new StandardsConvertNew()
                            {
                                StandardId = (int)d.MostRecent.Standard_id,
                                isRegulatoryStand = true
                            };
                            app.StandardsAditional.Add(ss);
                        }
                        else if (stand.Type_of_Document == "I")
                        {
                            StandardsConvertNew ss = new StandardsConvertNew()
                            {
                                StandardId = (int)d.MostRecent.Standard_id,
                                isRegulatoryStand = false
                            };
                            app.StandardsAditional.Add(ss);
                        }
                    }

                    foreach (var p in d.Others)
                    {

                        if (d.MostRecent.low == p.low)
                        {

                            if (!listOfApplication.Contains(p.Application))
                            {
                                //Debug.WriteLine(p.LowView + "==" + p.Application);
                                if (!p.Application.Equals(""))
                                {


                                    ApplicationConvert app2 = new ApplicationConvert();
                                    app2.Application = p.Application;
                                    app2.isDeletedApp = p.isDeletedApp;
                                    app2.Comment = p.Comment;


                                    //app2.DocumentId = new List<int>();
                                    //app2.StandardId = new List<int>();
                                    app2.DocumentsAditional = new List<DocumentConvertNew>();
                                    app2.StandardsAditional = new List<StandardsConvertNew>();

                                    if (p.documents_Id != null || p.documents_Id > 0)
                                    {

                                        //app2.DocumentId.Add((int)p.documents_Id);

                                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == p.documents_Id).SingleOrDefault();
                                        if (doc.Type_of_doc == "R")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = true
                                            };
                                            app2.DocumentsAditional.Add(rr);
                                        }
                                        else if (doc.Type_of_doc == "I")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = false
                                            };
                                            app2.DocumentsAditional.Add(rr);
                                        }

                                    }

                                    if (p.Standard_id != null || p.Standard_id > 0)
                                    {
                                        //app2.StandardId.Add((int)p.Standard_id);

                                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == p.Standard_id).SingleOrDefault();
                                        if (stand.Type_of_Document == "R")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = true
                                            };
                                            app2.StandardsAditional.Add(ss);
                                        }
                                        else if (stand.Type_of_Document == "I")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = false
                                            };
                                            app2.StandardsAditional.Add(ss);

                                        }
                                    }
                                    appView.Application.Add(app2);



                                    listOfApplication.Add(p.Application);


                                }
                            }
                            else
                            {
                                bool IsFirst = false;
                                if (appView.Application.Count == 0)
                                {
                                    //if (appView.LowView.Equals("9 kHz"))
                                    //{
                                    //    Debug.WriteLine("im here:" + app.DocumentId.Count + "::" + p.documents_Id);

                                    IsFirst = true;
                                    //}
                                    appView.Application.Add(app);

                                }
                                //Debug.WriteLine("im here:" + app.DocumentsAditional.Count + "::" + p.documents_Id);
                                if (!IsFirst)
                                {
                                    var tempApp = appView.Application.Where(x => x.Application == p.Application).SingleOrDefault();
                                    //old way of adding of docId
                                    //if (appView.Application.Find(u => u.Application == p.Application).DocumentId == null)
                                    //{
                                    //    appView.Application.Find(u => u.Application == p.Application).DocumentId = new List<int>();
                                    //}

                                    //if (appView.Application.Find(u => u.Application == p.Application).StandardId == null)
                                    //{
                                    //    appView.Application.Find(u => u.Application == p.Application).StandardId = new List<int>();
                                    //}
                                    if (appView.Application.Find(u => u.Application == p.Application).DocumentsAditional == null)
                                    {
                                        appView.Application.Find(u => u.Application == p.Application).DocumentsAditional = new List<DocumentConvertNew>();
                                    }

                                    if (appView.Application.Find(u => u.Application == p.Application).StandardsAditional == null)
                                    {
                                        appView.Application.Find(u => u.Application == p.Application).StandardsAditional = new List<StandardsConvertNew>();
                                    }




                                    if (p.documents_Id > 0)
                                    {

                                        //app.DocumentId.Add((int)p.documents_Id);
                                        //appView.Application.Find(u => u.Application == p.Application).DocumentId.Add((int)p.documents_Id);

                                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == p.documents_Id).SingleOrDefault();
                                        if (doc.Type_of_doc == "R")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = true
                                            };
                                            //app2.DocumentsAditional.Add(rr);
                                            appView.Application.Find(u => u.Application == p.Application).DocumentsAditional.Add(rr);
                                        }
                                        else if (doc.Type_of_doc == "I")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = false
                                            };
                                            //app2.DocumentsAditional.Add(rr);
                                            appView.Application.Find(u => u.Application == p.Application).DocumentsAditional.Add(rr);
                                        }

                                    }

                                    if (p.Standard_id > 0)
                                    {
                                        //appView.Application.Find(u => u.Application == p.Application).StandardId.Add((int)p.Standard_id);
                                        //app.StandardId.Add((int)p.Standard_id);
                                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == p.Standard_id).SingleOrDefault();

                                        if (stand.Type_of_Document == "R")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = true
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).StandardsAditional.Add(ss);
                                        }
                                        else if (stand.Type_of_Document == "I")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = false
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).StandardsAditional.Add(ss);
                                        }
                                    }
                                }

                                //    //tempApp = app;
                                //    //appView.Application = tempApp ;
                            }

                        }

                    }

                    if (appView.Application.Count == 0)
                    {

                        //if (appView.LowView.Equals("9 kHz"))
                        //{
                        //    Debug.WriteLine("www");
                        //}
                        appView.Application.Add(app);
                    }


                    //if (appView.LowView == "11.3 kHz")
                    //{
                    //    foreach (var ww in appView.Application)
                    //    {
                    //        if (ww.Application == "ISM")
                    //        {
                    //            Debug.WriteLine("www:" + ww.StandardId.Count);
                    //        }
                    //    }

                    //}

                    appView.regionCode = d.MostRecent.regionCode;
                    appView.regionName = d.MostRecent.regionName;

                    listOfAppGeneral.Add(appView);
                }


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
            //List<FreqBandSearchNewDocStand> list = new List<FreqBandSearchNewDocStand>();
            List<ApplicationView> listOfApp = new List<ApplicationView>();
            foreach (var temp in Nodupl)
            {

                ApplicationView appView = new ApplicationView();
                appView.low = temp.MostRecent.low;
                appView.high = temp.MostRecent.high;
                appView.LowView = temp.MostRecent.LowView;
                appView.HighView = temp.MostRecent.HighView;
                appView.Application = new List<ApplicationConvert>();

                ApplicationConvert ww = new ApplicationConvert();
                ww.Application = temp.MostRecent.Application;
                ww.isDeletedApp = temp.MostRecent.isDeletedApp;
                ww.Comment = temp.MostRecent.Comment;
                //ww.DocumentId = new List<int>();
                //ww.StandardId = new List<int>();
                ww.DocumentsAditional = new List<DocumentConvertNew>();
                ww.StandardsAditional = new List<StandardsConvertNew>();
                if (temp.MostRecent.documents_Id > 0)
                {
                    //appView.DocumentId.Add((int)temp.MostRecent.documents_Id);
                    var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == temp.MostRecent.documents_Id).SingleOrDefault();
                    if (doc.Type_of_doc == "R")
                    {
                        DocumentConvertNew dd = new DocumentConvertNew()
                        {
                            DocumentId = (int)temp.MostRecent.documents_Id,
                            isRegulatory = true
                        };

                        ww.DocumentsAditional.Add(dd);
                    }
                    else if (doc.Type_of_doc == "I")
                    {
                        DocumentConvertNew dd = new DocumentConvertNew()
                        {
                            DocumentId = (int)temp.MostRecent.documents_Id,
                            isRegulatory = false
                        };

                        ww.DocumentsAditional.Add(dd);
                    }
                    //ww.DocumentId.Add((int)temp.MostRecent.documents_Id);
                }

                if (temp.MostRecent.Standard_id > 0)
                {
                    var stand = _conApp.StandardsDb.Where(x => x.Standard_id == temp.MostRecent.Standard_id).SingleOrDefault();
                    if (stand.Type_of_Document == "R")
                    {
                        StandardsConvertNew dd = new StandardsConvertNew()
                        {
                            StandardId = (int)temp.MostRecent.Standard_id,
                            isRegulatoryStand = true
                        };

                        ww.StandardsAditional.Add(dd);
                    }
                    else if (stand.Type_of_Document == "I")
                    {
                        StandardsConvertNew dd = new StandardsConvertNew()
                        {
                            StandardId = (int)temp.MostRecent.Standard_id,
                            isRegulatoryStand = false
                        };

                        ww.StandardsAditional.Add(dd);
                    }


                    //ww.StandardId.Add((int)temp.MostRecent.Standard_id);
                    //appView.StandardId.Add((int)temp.MostRecent.Standard_id);
                }
                appView.regionCode = temp.MostRecent.regionCode;
                appView.regionName = temp.MostRecent.regionName;
                appView.Application.Add(ww);


                listOfAppGeneral.Add(appView);

                //Debug.WriteLine("list of app count:" + temp.Others.Count);
            }
            //var endList = listOfAppGeneral.OrderBy(x => x.Application).ToList();

            //foreach(var pp in listOfAppGeneral)
            //{
            //    foreach(var ww in pp.Application)
            //    {
            //        Debug.WriteLine("ttt:" + pp.LowView + "==" + ww.Application + ":::" + pp.HighView);
            //    }
            //}


            return listOfAppGeneral;
        }

        List<FreqBandSearchNewDocStand> SearchAppAllFromToNew(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue)
        {
            _conApp = _tempApp;
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
                                Doc_number = "",
                                Title_of_doc = "",
                                Hyperlink = "",
                                Allocation = "",
                                Footnote = "",
                                FootnoteDesc = "",
                                isBand = false,
                                Primary = false,
                                regionName = val.regionName,
                                regionCode = val.regionCode,
                                regionId = val.regionId,
                                LowView = e.LowView,
                                HighView = e.HighView,
                                documents_Id = all.documents_Id,
                                Standard_id = all.Standard_id
                            }).ToList().OrderBy(p => p.low);
            List<AllSearchFreqBand> AllAppValues = null;

            var values = (from x in appQuery select (AllSearchFreqBand)x).ToList();


            var allY = values.Where(x => x.high >= tempFrom).ToList();
            AllAppValues = allY.Where(x => x.low <= tempTo).ToList();

            List<FreqBandSearchDocStand> allS = new List<FreqBandSearchDocStand>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearchDocStand fre = new FreqBandSearchDocStand()
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
                    HighView = tempValue.HighView,
                    documents_Id = tempValue.documents_Id,
                    Standard_id = tempValue.Standard_id
                };
                allS.Add(fre);
            }

            List<FreqBandSearchNewDocStand> listAl = new List<FreqBandSearchNewDocStand>();
            for (int i = 0; i < allS.Count; i++)
            {
                FootnoteJsonConvert foot = new FootnoteJsonConvert();
                string comment = "";
                string Doc_number = "";
                string Title_of_doc = "";
                string Hyperlink = "";

                string Etsi_standard = "";
                string Title_docStand = "";
                string HypelinkStand = "";
                int Standid = 0;
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

                //Debug.WriteLine("ww new:" + Doc_number);
                if (allS[i].documents_Id != null)
                {
                    var appQueryDoc = (from all in _conApp.DocumentsDb
                                       select new
                                       {
                                           documents_id = all.DocumentsId,
                                           Doc_number = all.Doc_number,
                                           Title_of_doc = all.Title_of_doc,
                                           Hyperlink = all.Hyperlink
                                       }).ToList();
                    int id = (int)allS[i].documents_Id;
                    var Doc = appQueryDoc.Where(x => x.documents_id == id).SingleOrDefault();
                    Doc_number = Doc.Doc_number;
                    Title_of_doc = Doc.Title_of_doc;
                    Hyperlink = Doc.Hyperlink;

                }

                if (allS[i].Standard_id != null)
                {
                    var appQueryStand = (from all in _conApp.StandardsDb
                                         select new
                                         {
                                             standard_id = all.Standard_id,
                                             Etsi_standard = all.Etsi_standard,
                                             Title_of_doc = all.Title_doc,
                                             Hyperlink = all.Hypelink
                                         }).ToList();
                    Standid = (int)allS[i].Standard_id;
                    var Stand = appQueryStand.Where(x => x.standard_id == Standid).SingleOrDefault();
                    Etsi_standard = Stand.Etsi_standard;
                    Title_docStand = Stand.Title_of_doc;
                    HypelinkStand = Stand.Hyperlink;
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



                FreqBandSearchNewDocStand al = new FreqBandSearchNewDocStand()
                {
                    low = (long)allS[i].GetType().GetProperty("low").GetValue(allS[i]),
                    high = (long)allS[i].GetType().GetProperty("high").GetValue(allS[i]),
                    Application = (string)allS[i].GetType().GetProperty("Application").GetValue(allS[i]),
                    Allocation = "",
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Doc_number = Doc_number,
                    Title_of_doc = Title_of_doc,
                    Hyperlink = Hyperlink,
                    StandardId = Standid,
                    Etsi_standard = Etsi_standard,
                    Title_docS = Title_docStand,
                    HypelinkS = HypelinkStand,
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

            List<FreqBandSearchNewDocStand> listGeneral = new List<FreqBandSearchNewDocStand>();
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
                List<FreqBandSearchNewDocStand> others = d.Others;
                FreqBandSearchNewDocStand ge = new FreqBandSearchNewDocStand();
                ge.low = d.MostRecent.low;
                ge.high = d.MostRecent.high;
                ge.LowView = d.MostRecent.LowView;
                ge.HighView = d.MostRecent.HighView;
                ge.Application = d.MostRecent.Application;
                ge.Allocation = d.MostRecent.Allocation;
                ge.Footnote = new List<FootnoteJsonConvert>();
                ge.BandFootnote = new List<FootnoteJsonConvert>();
                ge.AppItemsList = new List<ApplicationConvert>();


                ApplicationConvert apC2 = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment,
                    Documents = new List<DocumentsConvert>(),
                    Standards = new List<StandardsConvert>()
                };

                if (d.MostRecent.Doc_number != "")
                {
                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = d.MostRecent.Doc_number;
                    ds2.Title_of_doc = d.MostRecent.Title_of_doc;
                    ds2.Hyperlink = d.MostRecent.Hyperlink;
                    apC2.Documents.Add(ds2);
                }

                if (d.MostRecent.StandardId > 0)
                {

                    StandardsConvert sc = new StandardsConvert();
                    sc.StandardId = d.MostRecent.StandardId;
                    sc.Etsi_standard = d.MostRecent.Etsi_standard;
                    sc.Title_docS = d.MostRecent.Title_docS;
                    sc.HyperlinkS = d.MostRecent.HypelinkS;
                    apC2.Standards.Add(sc);
                }
                //Debug.WriteLine("ww new:" + ap.Doc_number);
                ge.AppItemsList.Add(apC2);


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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }
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

                                ApplicationConvert apC3 = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment,
                                    Documents = new List<DocumentsConvert>(),
                                    Standards = new List<StandardsConvert>()

                                };
                                if (p.Doc_number != "")
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    apC3.Documents.Add(ds3);
                                }

                                if (p.StandardId > 0)
                                {

                                    StandardsConvert sc = new StandardsConvert();
                                    sc.StandardId = p.StandardId;
                                    sc.Etsi_standard = p.Etsi_standard;
                                    sc.Title_docS = p.Title_docS;
                                    sc.HyperlinkS = p.HypelinkS;
                                    apC3.Standards.Add(sc);
                                }
                                ge.AppItemsList.Add(apC3);

                                listOfApplication.Add(p.Application);
                            }

                        }
                        else
                        {


                            if (p.Doc_number != "")
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Document = App.Documents.Where(v => v.Doc_number == p.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
                            }

                            if (p.StandardId > 0)
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Standard = App.Standards.Where(v => v.StandardId == p.StandardId).SingleOrDefault();
                                if (Standard == null)
                                {

                                    StandardsConvert sc = new StandardsConvert();
                                    sc.StandardId = p.StandardId;
                                    sc.Etsi_standard = p.Etsi_standard;
                                    sc.Title_docS = p.Title_docS;
                                    sc.HyperlinkS = p.HypelinkS;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Standards.Add(sc);
                                }
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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
            // Debug.WriteLine("ne dupli" + Nodupl.Count());
            List<FreqBandSearchNewDocStand> list = new List<FreqBandSearchNewDocStand>();
            List<string> valuesOfApp = new List<string>();
            foreach (var temp in Nodupl)
            {


                foreach (var ww in temp.Others)
                {

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();



                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = temp.MostRecent.Doc_number;
                    ds2.Title_of_doc = temp.MostRecent.Title_of_doc;
                    ds2.Hyperlink = temp.MostRecent.Hyperlink;

                    StandardsConvert sc = new StandardsConvert();
                    sc.StandardId = temp.MostRecent.StandardId;
                    sc.Etsi_standard = temp.MostRecent.Etsi_standard;
                    sc.Title_docS = temp.MostRecent.Title_docS;
                    sc.HyperlinkS = temp.MostRecent.HypelinkS;


                    ApplicationConvert apC2 = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment,
                        Documents = new List<DocumentsConvert>(),
                        Standards = new List<StandardsConvert>()
                    };
                    apC2.Documents.Add(ds2);
                    apC2.Standards.Add(sc);


                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);
                    ww.AppItemsList.Add(apC2);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            foreach (var valuesGen in listGeneral)
            {
                valuesGen.AppItemsList = valuesGen.AppItemsList.OrderBy(x => x.Application).ToList();
            }
            //Debug.WriteLine("radi:" + listGeneral.Count);
            return listGeneral.ToList();

        }

        public List<ApplicationView> SearchAppAllFromToNewPerfomance(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue)
        {

            _conApp = _tempApp;
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
                                isDeletedApp = all.isDeletedApp,
                                Comment = all.comment,
                                colorCode = term.ColorCode,
                                TermId = all.TermId,
                                Doc_number = "",
                                Title_of_doc = "",
                                Hyperlink = "",
                                Allocation = "",
                                Footnote = "",
                                FootnoteDesc = "",
                                isBand = false,
                                Primary = false,
                                regionName = val.regionName,
                                regionCode = val.regionCode,
                                regionId = val.regionId,
                                LowView = e.LowView,
                                HighView = e.HighView,
                                documents_Id = all.documents_Id,
                                Standard_id = all.Standard_id,
                            }).ToList().OrderBy(p => p.low);

            List<AllSearchFreqBand> AllAppValues = null;

            var values = (from x in appQuery select (AllSearchFreqBand)x).ToList();



            var allY = values.Where(x => x.high >= tempFrom).ToList();


            AllAppValues = allY.Where(x => x.low <= tempTo).ToList();



            List<FreqBandSearchDocStand> allS = new List<FreqBandSearchDocStand>();

            foreach (var tempValue in AllAppValues)
            {
                //Debug.WriteLine("ww:" + tempValue.Doc_number);
                FreqBandSearchDocStand fre = new FreqBandSearchDocStand()
                {

                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = "",
                    Application = tempValue.Application,
                    isDeletedApp = tempValue.isDeletedApp,
                    Footnote = tempValue.Footnote,
                    FootnoteDesc = tempValue.FootnoteDesc,
                    Comment = tempValue.Comment,
                    colorCode = tempValue.colorCode,
                    TermId = tempValue.TermId,
                    regionName = tempValue.regionName,
                    regionCode = tempValue.regionCode,
                    LowView = tempValue.LowView,
                    HighView = tempValue.HighView,
                    documents_Id = tempValue.documents_Id,
                    Standard_id = tempValue.Standard_id
                };
                allS.Add(fre);
            }
            var ordered = allS.OrderBy(x => x.low).ToList();
            var duplicates = ordered.OrderBy(e => e.low)
                            .GroupBy(e => e.low)
                            .Where(e => e.Count() > 1)
                            .Select(g => new
                            {
                                MostRecent = g.FirstOrDefault(),
                                Others = g.Skip(0).ToList()
                            });

            List<ApplicationView> listOfAppGeneral = new List<ApplicationView>();
            bool InitialAppAdded = false;
            foreach (var d in duplicates)
            {
                List<string> listOfApplication = new List<string>();
                if (!listOfApplication.Contains(d.MostRecent.Application))
                {
                    ApplicationView appView = new ApplicationView
                    {
                        low = d.MostRecent.low,
                        high = d.MostRecent.high,
                        LowView = d.MostRecent.LowView,
                        HighView = d.MostRecent.HighView,
                        Application = new List<ApplicationConvert>()
                    };
                    ApplicationConvert app = new ApplicationConvert
                    {
                        Application = d.MostRecent.Application,
                        isDeletedApp = d.MostRecent.isDeletedApp,
                        Comment = d.MostRecent.Comment,
                        colorCode = d.MostRecent.colorCode,
                        TermId = d.MostRecent.TermId,
                    };
                    Footer footer = new Footer();
                    footer.footTitle = d.MostRecent.Footnote;
                    footer.footDiscription = d.MostRecent.FootnoteDesc;

                    listOfApplication.Add(d.MostRecent.Application);
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    if (d.MostRecent.documents_Id != null || d.MostRecent.documents_Id > 0)
                    {
                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == d.MostRecent.documents_Id).SingleOrDefault();
                        if (doc.Type_of_doc == "R")
                        {
                            DocumentConvertNew rr = new DocumentConvertNew
                            {
                                DocumentId = (int)d.MostRecent.documents_Id,
                                isRegulatory = true
                            };
                            app.DocumentsAditional.Add(rr);
                        }
                        else if (doc.Type_of_doc == "I")
                        {
                            DocumentConvertNew rr = new DocumentConvertNew
                            {
                                DocumentId = (int)d.MostRecent.documents_Id,
                                isRegulatory = false
                            };
                            app.DocumentsAditional.Add(rr);
                        }
                    }

                    if (d.MostRecent.Standard_id != null || d.MostRecent.Standard_id > 0)
                    {
                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == d.MostRecent.Standard_id).SingleOrDefault();
                        if (stand.Type_of_Document == "R")
                        {
                            StandardsConvertNew ss = new StandardsConvertNew()
                            {
                                StandardId = (int)d.MostRecent.Standard_id,
                                isRegulatoryStand = true
                            };
                            app.StandardsAditional.Add(ss);
                        }
                        else if (stand.Type_of_Document == "I")
                        {
                            StandardsConvertNew ss = new StandardsConvertNew()
                            {
                                StandardId = (int)d.MostRecent.Standard_id,
                                isRegulatoryStand = false
                            };
                            app.StandardsAditional.Add(ss);
                        }
                    }

                    foreach (var p in d.Others)
                    {
                        if (d.MostRecent.low == p.low)
                        {
                            if (!listOfApplication.Contains(p.Application))
                            {
                                if (!p.Application.Equals(""))
                                {
                                    ApplicationConvert app2 = new ApplicationConvert
                                    {
                                        Application = p.Application,
                                        isDeletedApp = p.isDeletedApp,
                                        Comment = p.Comment,
                                        colorCode = p.colorCode,
                                        TermId = p.TermId,
                                        DocumentsAditional = new List<DocumentConvertNew>(),
                                        StandardsAditional = new List<StandardsConvertNew>()
                                    };

                                    if (p.documents_Id != null || p.documents_Id > 0)
                                    {
                                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == p.documents_Id).SingleOrDefault();
                                        if (doc.Type_of_doc == "R")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = true
                                            };
                                            app2.DocumentsAditional.Add(rr);
                                        }
                                        else if (doc.Type_of_doc == "I")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = false
                                            };
                                            app2.DocumentsAditional.Add(rr);
                                        }
                                    }
                                    if (p.Standard_id != null || p.Standard_id > 0)
                                    {
                                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == p.Standard_id).SingleOrDefault();
                                        if (stand.Type_of_Document == "R")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = true
                                            };
                                            app2.StandardsAditional.Add(ss);
                                        }
                                        else if (stand.Type_of_Document == "I")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = false
                                            };
                                            app2.StandardsAditional.Add(ss);
                                        }
                                    }
                                    appView.Application.Add(app2);
                                    listOfApplication.Add(p.Application);
                                }
                            }
                            else
                            {
                                bool IsFirst = false;
                                if (appView.Application.Count == 0)
                                {
                                    IsFirst = true;
                                    appView.Application.Add(app);
                                }
                                if (!IsFirst)
                                {
                                    var tempApp = appView.Application.Where(x => x.Application == p.Application).SingleOrDefault();
                                    if (appView.Application.Find(u => u.Application == p.Application).DocumentsAditional == null)
                                    {
                                        appView.Application.Find(u => u.Application == p.Application).DocumentsAditional = new List<DocumentConvertNew>();
                                    }
                                    if (appView.Application.Find(u => u.Application == p.Application).StandardsAditional == null)
                                    {
                                        appView.Application.Find(u => u.Application == p.Application).StandardsAditional = new List<StandardsConvertNew>();
                                    }
                                    if (p.documents_Id > 0)
                                    {
                                        var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == p.documents_Id).SingleOrDefault();
                                        if (doc.Type_of_doc == "R")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = true
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).DocumentsAditional.Add(rr);
                                        }
                                        else if (doc.Type_of_doc == "I")
                                        {
                                            DocumentConvertNew rr = new DocumentConvertNew
                                            {
                                                DocumentId = (int)p.documents_Id,
                                                isRegulatory = false
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).DocumentsAditional.Add(rr);
                                        }
                                    }

                                    if (p.Standard_id > 0)
                                    {
                                        var stand = _conApp.StandardsDb.Where(x => x.Standard_id == p.Standard_id).SingleOrDefault();

                                        if (stand.Type_of_Document == "R")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = true
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).StandardsAditional.Add(ss);
                                        }
                                        else if (stand.Type_of_Document == "I")
                                        {
                                            StandardsConvertNew ss = new StandardsConvertNew()
                                            {
                                                StandardId = (int)p.Standard_id,
                                                isRegulatoryStand = false
                                            };
                                            appView.Application.Find(u => u.Application == p.Application).StandardsAditional.Add(ss);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    if (appView.Application.Count == 0)
                    {
                        appView.Application.Add(app);
                    }
                    appView.footers.Add(footer);
                    appView.regionCode = d.MostRecent.regionCode;
                    appView.regionName = d.MostRecent.regionName;
                    listOfAppGeneral.Add(appView);
                }
            }

            var Nodupl = ordered.OrderBy(e => e.low)
                        .GroupBy(e => e.low)
                        .Where(e => e.Count() == 1)
                        .Select(g => new
                        {
                            MostRecent = g.FirstOrDefault(),
                            Others = g.Skip(0).ToList()
                        });
            List<ApplicationView> listOfApp = new List<ApplicationView>();
            foreach (var temp in Nodupl)
            {

                ApplicationView appView = new ApplicationView
                {
                    low = temp.MostRecent.low,
                    high = temp.MostRecent.high,
                    LowView = temp.MostRecent.LowView,
                    HighView = temp.MostRecent.HighView,
                    Application = new List<ApplicationConvert>()
                };
                ApplicationConvert ww = new ApplicationConvert
                {
                    Application = temp.MostRecent.Application,
                    isDeletedApp = temp.MostRecent.isDeletedApp,
                    Comment = temp.MostRecent.Comment,
                    colorCode = temp.MostRecent.colorCode,
                    TermId = temp.MostRecent.TermId,
                    DocumentsAditional = new List<DocumentConvertNew>(),
                    StandardsAditional = new List<StandardsConvertNew>()
                };
                if (temp.MostRecent.documents_Id > 0)
                {
                    var doc = _conApp.DocumentsDb.Where(x => x.DocumentsId == temp.MostRecent.documents_Id).SingleOrDefault();
                    if (doc.Type_of_doc == "R")
                    {
                        DocumentConvertNew dd = new DocumentConvertNew()
                        {
                            DocumentId = (int)temp.MostRecent.documents_Id,
                            isRegulatory = true
                        };

                        ww.DocumentsAditional.Add(dd);
                    }
                    else if (doc.Type_of_doc == "I")
                    {
                        DocumentConvertNew dd = new DocumentConvertNew()
                        {
                            DocumentId = (int)temp.MostRecent.documents_Id,
                            isRegulatory = false
                        };

                        ww.DocumentsAditional.Add(dd);
                    }
                }

                if (temp.MostRecent.Standard_id > 0)
                {
                    var stand = _conApp.StandardsDb.Where(x => x.Standard_id == temp.MostRecent.Standard_id).SingleOrDefault();
                    if (stand.Type_of_Document == "R")
                    {
                        StandardsConvertNew dd = new StandardsConvertNew()
                        {
                            StandardId = (int)temp.MostRecent.Standard_id,
                            isRegulatoryStand = true
                        };

                        ww.StandardsAditional.Add(dd);
                    }
                    else if (stand.Type_of_Document == "I")
                    {
                        StandardsConvertNew dd = new StandardsConvertNew()
                        {
                            StandardId = (int)temp.MostRecent.Standard_id,
                            isRegulatoryStand = false
                        };

                        ww.StandardsAditional.Add(dd);
                    }
                }
                appView.regionCode = temp.MostRecent.regionCode;
                appView.regionName = temp.MostRecent.regionName;
                appView.Application.Add(ww);
                listOfAppGeneral.Add(appView);
            }
            listOfAppGeneral.OrderBy(x => x.low);
            return listOfAppGeneral;
        }

        List<FreqBandSearchNew> SearchAppAllZeroTo(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue)
        {

            _conApp = _tempApp;
            var appQuery = (from all in _conApp.Application
                            join ww in _conApp.DocumentsDb on all.documents_Id equals ww.DocumentsId
                            join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                            join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                            join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                            where val.regionId == int.Parse(FrequencytableValue)
                            select new AllSearchFreqBand
                            {

                                low = e.low,
                                high = e.high,
                                Application = term.name,
                                Doc_number = ww.Doc_number,
                                Title_of_doc = ww.Title_of_doc,
                                Hyperlink = ww.Hyperlink,
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
                            }).Union(from all in _conApp.Application
                                     join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                                     join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                                     join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                                     where val.regionId == int.Parse(FrequencytableValue) && all.documents_Id == null
                                     select new AllSearchFreqBand
                                     {

                                         low = e.low,
                                         high = e.high,
                                         Application = term.name,
                                         Comment = all.comment,
                                         Doc_number = "",
                                         Title_of_doc = "",
                                         Hyperlink = "",
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

            List<AllSearchFreqBand> AllAppValues = null;

            var allY = appQuery.Where(x => x.low >= tempFrom).ToList();
            AllAppValues = allY.Where(x => x.low <= tempTo).ToList();



            List<FreqBandSearch> allS = new List<FreqBandSearch>();

            foreach (var tempValue in AllAppValues)
            {
                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = "",
                    Application = tempValue.Application,
                    Doc_number = tempValue.Doc_number,
                    Title_of_doc = tempValue.Title_of_doc,
                    Hyperlink = tempValue.Hyperlink,
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
                string Doc_number = "";
                string Title_of_doc = "";
                string Hyperlink = "";
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
                if (allS[i].GetType().GetProperty("Doc_number") != null)
                {
                    Doc_number = (string)allS[i].GetType().GetProperty("Doc_number").GetValue(allS[i]);
                    Title_of_doc = (string)allS[i].GetType().GetProperty("Title_of_doc").GetValue(allS[i]);
                    Hyperlink = (string)allS[i].GetType().GetProperty("Hyperlink").GetValue(allS[i]);
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
                    Allocation = "",
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Doc_number = Doc_number,
                    Title_of_doc = Title_of_doc,
                    Hyperlink = Hyperlink,
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
                ge.AppItemsList = new List<ApplicationConvert>();

                ApplicationConvert apC2 = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment,
                    Documents = new List<DocumentsConvert>()
                };
                if (d.MostRecent.Doc_number != "")
                {
                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = d.MostRecent.Doc_number;
                    ds2.Title_of_doc = d.MostRecent.Title_of_doc;
                    ds2.Hyperlink = d.MostRecent.Hyperlink;
                    apC2.Documents.Add(ds2);
                }
                //Debug.WriteLine("ww new:" + ap.Doc_number);
                ge.AppItemsList.Add(apC2);



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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }

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

                                ApplicationConvert apC3 = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment,
                                    Documents = new List<DocumentsConvert>()
                                };
                                if (p.Doc_number != "")
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    apC3.Documents.Add(ds3);
                                }
                                //Debug.WriteLine("ww new:" + ap.Doc_number);
                                ge.AppItemsList.Add(apC3);


                                listOfApplication.Add(p.Application);
                            }

                        }
                        else
                        {
                            if (p.Doc_number != "")
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Document = App.Documents.Where(v => v.Doc_number == p.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();

                    ApplicationConvert apC3 = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment,
                        Documents = new List<DocumentsConvert>()
                    };
                    if (temp.MostRecent.Doc_number != "")
                    {
                        DocumentsConvert ds3 = new DocumentsConvert();
                        ds3.Doc_number = temp.MostRecent.Doc_number;
                        ds3.Title_of_doc = temp.MostRecent.Title_of_doc;
                        ds3.Hyperlink = temp.MostRecent.Hyperlink;
                        apC3.Documents.Add(ds3);
                    }
                    //Debug.WriteLine("ww new:" + ap.Doc_number);


                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);
                    ww.AppItemsList.Add(apC3);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            //Debug.WriteLine("radi:" + listGeneral.Count);

            foreach (var values in listGeneral)
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
            }


            return listGeneral.OrderBy(e => e.low).ToList();
        }

        public List<FreqBandSearchNew> SearchAppAllFromTo(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue)
        {
            _conApp = _tempApp;
            var appQuery = (from all in _conApp.Application
                            join ww in _conApp.DocumentsDb on all.documents_Id equals ww.DocumentsId
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
                                Doc_number = ww.Doc_number,
                                Title_of_doc = ww.Title_of_doc,
                                Hyperlink = ww.Hyperlink,
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
                            }).Union(from all in _conApp.Application
                                     join e in _conApp.ApplicationRange on all.ApplicationRangeId equals e.ApplicationRangeId
                                     join term in _conApp.RootApplicationTermsDB on all.ApplicationTermId equals term.ApplicationTermsDBId
                                     join val in _conApp.RootApplicationDB on e.RootApplicationDBId equals val.RootApplicationDBId
                                     where val.regionId == int.Parse(FrequencytableValue) && all.documents_Id == null
                                     select new AllSearchFreqBand
                                     {

                                         low = e.low,
                                         high = e.high,
                                         Application = term.name,
                                         Comment = all.comment,
                                         Doc_number = "",
                                         Title_of_doc = "",
                                         Hyperlink = "",
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

            //appQuery.Union

            List<AllSearchFreqBand> AllAppValues = null;

            var allY = appQuery.Where(x => x.high >= tempFrom).ToList();
            AllAppValues = allY.Where(x => x.low <= tempTo).ToList();



            List<FreqBandSearch> allS = new List<FreqBandSearch>();

            foreach (var tempValue in AllAppValues)
            {
                FreqBandSearch fre = new FreqBandSearch()
                {
                    low = tempValue.low,
                    high = tempValue.high,
                    Allocation = "",
                    Application = tempValue.Application,
                    Doc_number = tempValue.Doc_number,
                    Title_of_doc = tempValue.Title_of_doc,
                    Hyperlink = tempValue.Hyperlink,
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
                string Doc_number = "";
                string Title_of_doc = "";
                string Hyperlink = "";

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

                if (allS[i].GetType().GetProperty("Doc_number") != null)
                {
                    Doc_number = (string)allS[i].GetType().GetProperty("Doc_number").GetValue(allS[i]);
                    Title_of_doc = (string)allS[i].GetType().GetProperty("Title_of_doc").GetValue(allS[i]);
                    Hyperlink = (string)allS[i].GetType().GetProperty("Hyperlink").GetValue(allS[i]);
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
                    Allocation = "",
                    isPrimary = (bool)allS[i].GetType().GetProperty("isPrimary").GetValue(allS[i]),
                    Footnote = vrijednosti,
                    BandFootnote = vrijednostiBand,
                    Doc_number = Doc_number,
                    Title_of_doc = Title_of_doc,
                    Hyperlink = Hyperlink,
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
                ge.AppItemsList = new List<ApplicationConvert>();



                ApplicationConvert apC2 = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment,
                    Documents = new List<DocumentsConvert>()
                };

                if (d.MostRecent.Doc_number != "")
                {
                    DocumentsConvert ds2 = new DocumentsConvert();
                    ds2.Doc_number = d.MostRecent.Doc_number;
                    ds2.Title_of_doc = d.MostRecent.Title_of_doc;
                    ds2.Hyperlink = d.MostRecent.Hyperlink;
                    apC2.Documents.Add(ds2);
                }
                //Debug.WriteLine("ww new:" + ap.Doc_number);
                ge.AppItemsList.Add(apC2);


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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }
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
                                ApplicationConvert apC3 = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment,
                                    Documents = new List<DocumentsConvert>()
                                };
                                if (p.Doc_number != "")
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    apC3.Documents.Add(ds3);
                                }
                                //Debug.WriteLine("ww new:" + ap.Doc_number);
                                ge.AppItemsList.Add(apC3);

                                listOfApplication.Add(p.Application);
                            }

                        }
                        else
                        {
                            if (p.Doc_number != "")
                            {
                                var App = ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault();

                                var Document = App.Documents.Where(v => v.Doc_number == p.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = p.Doc_number;
                                    ds3.Title_of_doc = p.Title_of_doc;
                                    ds3.Hyperlink = p.Hyperlink;
                                    ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();

                    ApplicationConvert apC3 = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment,
                        Documents = new List<DocumentsConvert>()
                    };

                    if (temp.MostRecent.Doc_number != "")
                    {
                        DocumentsConvert ds3 = new DocumentsConvert();
                        ds3.Doc_number = temp.MostRecent.Doc_number;
                        ds3.Title_of_doc = temp.MostRecent.Title_of_doc;
                        ds3.Hyperlink = temp.MostRecent.Hyperlink;
                        apC3.Documents.Add(ds3);
                    }


                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);

                    ww.AppItemsList.Add(apC3);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            //Debug.WriteLine("radi:" + listGeneral.Count);

            foreach (var values in listGeneral)
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
            }


            return listGeneral.OrderBy(e => e.low).ToList();
        }

        public List<FreqBandSearchNewDocStand> SearchAppOnSecondLevelProcedureNew(IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;

            List<FreqBandSearchNewDocStand> TestList = new List<FreqBandSearchNewDocStand>();
            FreqBandSearchNewDocStand test = null;
            //Console.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "EmptyStoredProcedure.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            string val = "'" + ApplicatonVal + "';";
            string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);


            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode
                               }
                              ).ToList();
            List<long> lowList = new List<long>();
            //Debug.WriteLine("ukupno:" + importQuery.Count);
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new FreqBandSearchNewDocStand();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;
                    test.Application = temp.name;
                    test.Allocation = "";
                    test.isPrimary = false;
                    test.Footnote = null;
                    test.BandFootnote = null;

                    test.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    app.Standards = new List<StandardsConvert>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {
                            DocumentsConvert ds3 = new DocumentsConvert();
                            ds3.DocumentIt = values.DocumentsId;
                            ds3.Doc_number = values.Doc_number;
                            ds3.Title_of_doc = values.Title_of_doc;
                            ds3.Hyperlink = values.Hyperlink;
                            app.Documents.Add(ds3);

                        }

                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();

                        if (values.Standard_id > 0)
                        {

                            StandardsConvert sc3 = new StandardsConvert();
                            sc3.StandardId = values.Standard_id;
                            sc3.Etsi_standard = values.Etsi_standard;
                            sc3.Title_docS = values.Title_doc;
                            sc3.HyperlinkS = values.Hypelink;
                            app.Standards.Add(sc3);
                            //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);

                        }

                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    test.AppItemsList.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    app.Standards = new List<StandardsConvert>();
                    //temp.low
                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {
                                //Debug.WriteLine("id standard:" + values.Standard_id+"::"+temp.low);
                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Standard = App.Standards.Where(v => v.StandardId == values.Standard_id).SingleOrDefault();
                                //var Document = App.Standards.Where(v => v. == values.Doc_number).SingleOrDefault();
                                if (Standard == null)
                                {
                                    StandardsConvert sc3 = new StandardsConvert();
                                    sc3.StandardId = values.Standard_id;
                                    sc3.Etsi_standard = values.Etsi_standard;
                                    sc3.Title_docS = values.Title_doc;
                                    sc3.HyperlinkS = values.Hypelink;
                                    //App.Standards.Add(sc3);
                                    TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault().Standards.Add(sc3);
                                }
                            }
                        }


                    }

                    //temp.low
                    if (temp.documents_Id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {


                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.DocumentIt == values.DocumentsId).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = values.Doc_number;
                                    ds3.Title_of_doc = values.Title_of_doc;
                                    ds3.Hyperlink = values.Hyperlink;
                                    App.Documents.Add(ds3);
                                    //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                    TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault().Documents.Add(ds3);
                                }
                            }
                        }


                    }

                    //app.Application = temp.name;
                    app.Comment = temp.comment;
                    //TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Add(app);
                    //var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    //if (obj != null) obj.AppItemsList.Add(app);

                }

            }
            return TestList;


        }


        public List<ApplicationView> SearchAppOnSecondLevelProcedureNewPerfomance(IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;

            List<ApplicationView> TestList = new List<ApplicationView>();
            List<string> nameOfApp = new List<string>();
            ApplicationView test = null;
            //Debug.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "EmptyStoredProcedure.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            string val = "'" + ApplicatonVal + "';";
            string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);


            var importQuery = (from all in _conImport.storedTableApplicationProcedure

                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode,
                                   isDeletedApp = all.isDeletedApp
                               }
                              ).ToList().OrderBy(x => x.low);
            List<long> lowList = new List<long>();
            //Debug.WriteLine("ukupno:" + importQuery.Count);
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {

                    test = new ApplicationView();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;


                    test.Application = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink,
                                          all.Type_of_doc
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {
                            if (values.Type_of_doc == "R")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = (int)values.DocumentsId,
                                    isRegulatory = true
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            else if (values.Type_of_doc == "I")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = (int)values.DocumentsId,
                                    isRegulatory = false
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            //app.DocumentId.Add(values.DocumentsId);

                        }

                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink,
                                          all.Type_of_Document
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();

                        if (values.Standard_id > 0)
                        {
                            if (values.Type_of_Document == "R")
                            {
                                StandardsConvertNew ss = new StandardsConvertNew()
                                {
                                    StandardId = values.Standard_id,
                                    isRegulatoryStand = true
                                };
                                app.StandardsAditional.Add(ss);
                            }
                            else if (values.Type_of_Document == "I")
                            {
                                StandardsConvertNew ss = new StandardsConvertNew()
                                {
                                    StandardId = values.Standard_id,
                                    isRegulatoryStand = false
                                };
                                app.StandardsAditional.Add(ss);
                            }


                        }

                    }
                    nameOfApp = new List<string>();
                    nameOfApp.Add(temp.name);
                    app.Application = temp.name;
                    app.Comment = temp.comment;
                    app.isDeletedApp = temp.isDeletedApp;

                    test.Application.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    //if(test.LowView == "29.7 MHz")
                    //{
                    //    Debug.WriteLine("about:" + app.Application + ":app is in:" + temp.isDeletedApp);
                    //}

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();

                    if (!nameOfApp.Contains(temp.name))
                    {

                        app.Application = temp.name;
                        nameOfApp.Add(temp.name);

                        app.Comment = temp.comment;
                        app.isDeletedApp = temp.isDeletedApp;

                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Add(app);
                    }

                    //if (temp.LowView == "30.005 MHz")
                    //{
                    //    Debug.WriteLine("about 2:" + app.Application + ":app is in:" + temp.isDeletedApp+"==="+ temp.name);
                    //}


                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink,
                                          all.Type_of_Document
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {

                            //Debug.WriteLine("id standard:" + values.Standard_id+"::"+temp.low);
                            var App = tempObj.Application.Where(x => x.Application == temp.name).SingleOrDefault();
                            if (App.Standards != null)
                            {
                                var Standard = App.Standards.Where(v => v.StandardId == values.Standard_id).SingleOrDefault();
                                //var Document = App.Standards.Where(v => v. == values.Doc_number).SingleOrDefault();
                                if (Standard == null)
                                {

                                    //App.Standards.Add(sc3);
                                    if (values.Type_of_Document == "R")
                                    {
                                        StandardsConvertNew ss = new StandardsConvertNew()
                                        {
                                            StandardId = (int)values.Standard_id,
                                            isRegulatoryStand = true
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                        //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardId.Add(values.Standard_id);
                                    }
                                    else if (values.Type_of_Document == "I")
                                    {
                                        StandardsConvertNew ss = new StandardsConvertNew()
                                        {
                                            StandardId = (int)values.Standard_id,
                                            isRegulatoryStand = false
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                        //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardId.Add(values.Standard_id);
                                    }

                                }

                            }
                            else
                            {
                                if (values.Type_of_Document == "R")
                                {
                                    StandardsConvertNew ss = new StandardsConvertNew()
                                    {
                                        StandardId = (int)values.Standard_id,
                                        isRegulatoryStand = true
                                    };
                                    TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                    //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardId.Add(values.Standard_id);
                                }
                                else if (values.Type_of_Document == "I")
                                {
                                    StandardsConvertNew ss = new StandardsConvertNew()
                                    {
                                        StandardId = (int)values.Standard_id,
                                        isRegulatoryStand = false
                                    };
                                    TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                    //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardId.Add(values.Standard_id);
                                }
                            }
                        }


                    }

                    //temp.low
                    if (temp.documents_Id != null)
                    {

                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink,
                                          all.Type_of_doc
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        //Debug.WriteLine("doc id:" + temp.documents_Id + ":freq:" + temp.LowView + ":app:" + temp.name);

                        foreach (var tempObj in objElse)
                        {

                            var App = tempObj.Application.Where(x => x.Application == temp.name).SingleOrDefault();
                            if (App.Documents != null)
                            {
                                var Document = App.Documents.Where(v => v.DocumentIt == values.DocumentsId).SingleOrDefault();
                                if (Document == null)
                                {
                                    if (values.Type_of_doc == "R")
                                    {
                                        DocumentConvertNew dd = new DocumentConvertNew()
                                        {
                                            DocumentId = values.DocumentsId,
                                            isRegulatory = true
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                    }
                                    else if (values.Type_of_doc == "I")
                                    {
                                        DocumentConvertNew dd = new DocumentConvertNew()
                                        {
                                            DocumentId = values.DocumentsId,
                                            isRegulatory = false
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                    }

                                    //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                    //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentId.Add(values.DocumentsId);
                                }
                            }
                            else
                            {
                                if (values.Type_of_doc == "R")
                                {
                                    DocumentConvertNew dd = new DocumentConvertNew()
                                    {
                                        DocumentId = values.DocumentsId,
                                        isRegulatory = true
                                    };
                                    TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                }
                                else if (values.Type_of_doc == "I")
                                {
                                    DocumentConvertNew dd = new DocumentConvertNew()
                                    {
                                        DocumentId = values.DocumentsId,
                                        isRegulatory = false
                                    };
                                    TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                }
                            }


                        }


                    }


                    //if (temp.LowView == "29.7 MHz")
                    //{
                    //    Debug.WriteLine("about 2:" + app.Application + "app is in" + temp.isDeletedApp);
                    //}


                    //TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Add(app);
                    //var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    //if (obj != null) obj.AppItemsList.Add(app);

                }

            }

            //foreach(var allRecords in TestList)
            //{
            //    if(allRecords.LowView== "29.7 MHz")
            //    {
            //        foreach(var appunder in allRecords.Application)
            //        {
            //            Debug.WriteLine("this is freq:" + allRecords.LowView + "::" + appunder.Application);
            //        }
            //    }
            //}
            return TestList;


        }

        List<FreqBandSearchNew> SearchAppOnSecondLevelProcedure(IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;

            List<FreqBandSearchNew> TestList = new List<FreqBandSearchNew>();
            FreqBandSearchNew test = null;
            //Console.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "EmptyStoredProcedure.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            string val = "'" + ApplicatonVal + "';";
            string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);



            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode
                               }
                              ).ToList();
            List<long> lowList = new List<long>();
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new FreqBandSearchNew();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;

                    test.Allocation = "";
                    test.isPrimary = false;
                    test.Footnote = null;
                    test.BandFootnote = null;

                    test.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {
                            DocumentsConvert ds3 = new DocumentsConvert();
                            ds3.Doc_number = values.Doc_number;
                            ds3.Title_of_doc = values.Title_of_doc;
                            ds3.Hyperlink = values.Hyperlink;
                            app.Documents.Add(ds3);

                        }




                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    test.AppItemsList.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    //temp.low
                    if (temp.documents_Id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {


                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.Doc_number == values.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = values.Doc_number;
                                    ds3.Title_of_doc = values.Title_of_doc;
                                    ds3.Hyperlink = values.Hyperlink;
                                    app.Documents.Add(ds3);
                                    //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                }
                            }
                        }


                    }
                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    if (obj != null) obj.AppItemsList.Add(app);

                }

            }
            return TestList;

        }

        public List<FreqBandSearchNewDocStand> SearchAppOnSecondLevelProcedureFromZeroNew(long tempFrom, long tempTo, IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;

            List<FreqBandSearchNewDocStand> TestList = new List<FreqBandSearchNewDocStand>();
            FreqBandSearchNewDocStand test = null;
            //Console.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "StoredProcedureFromZero.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            //string val = "'" + ApplicatonVal + "';";
            //string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);

            string val = "'" + ApplicatonVal + "'," + tempFrom + "," + tempTo + ";";
            string ValueApplication = string.Concat(data, val);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);


            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode
                               }
                              ).ToList();
            List<long> lowList = new List<long>();
            //Debug.WriteLine("ukupno:" + importQuery.Count);
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new FreqBandSearchNewDocStand();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;
                    //Debug.WriteLine("ukupno:" + temp.name);
                    test.Application = temp.name;
                    test.Allocation = "";
                    test.isPrimary = false;
                    test.Footnote = null;
                    test.BandFootnote = null;

                    test.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    app.Standards = new List<StandardsConvert>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {
                            DocumentsConvert ds3 = new DocumentsConvert();
                            ds3.DocumentIt = values.DocumentsId;
                            ds3.Doc_number = values.Doc_number;
                            ds3.Title_of_doc = values.Title_of_doc;
                            ds3.Hyperlink = values.Hyperlink;
                            app.Documents.Add(ds3);

                        }

                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();

                        if (values.Standard_id > 0)
                        {

                            StandardsConvert sc3 = new StandardsConvert();
                            sc3.StandardId = values.Standard_id;
                            sc3.Etsi_standard = values.Etsi_standard;
                            sc3.Title_docS = values.Title_doc;
                            sc3.HyperlinkS = values.Hypelink;
                            app.Standards.Add(sc3);
                            //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);

                        }

                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    test.AppItemsList.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    app.Standards = new List<StandardsConvert>();
                    //temp.low
                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {
                                //Debug.WriteLine("id standard:" + values.Standard_id+"::"+temp.low);
                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Standard = App.Standards.Where(v => v.StandardId == values.Standard_id).SingleOrDefault();
                                //var Document = App.Standards.Where(v => v. == values.Doc_number).SingleOrDefault();
                                if (Standard == null)
                                {
                                    StandardsConvert sc3 = new StandardsConvert();
                                    sc3.StandardId = values.Standard_id;
                                    sc3.Etsi_standard = values.Etsi_standard;
                                    sc3.Title_docS = values.Title_doc;
                                    sc3.HyperlinkS = values.Hypelink;
                                    //App.Standards.Add(sc3);
                                    TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault().Standards.Add(sc3);
                                }
                            }
                        }


                    }

                    //temp.low
                    if (temp.documents_Id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {


                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.DocumentIt == values.DocumentsId).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = values.Doc_number;
                                    ds3.Title_of_doc = values.Title_of_doc;
                                    ds3.Hyperlink = values.Hyperlink;
                                    App.Documents.Add(ds3);
                                    //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                    TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault().Documents.Add(ds3);
                                }
                            }
                        }


                    }

                    //app.Application = temp.name;
                    app.Comment = temp.comment;
                    //TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Add(app);
                    //var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    //if (obj != null) obj.AppItemsList.Add(app);

                }

            }
            return TestList;
        }


        public List<ApplicationView> SearchAppOnSecondLevelProcedureFromZeroNewPerfomance(long tempFrom, long tempTo, IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {

            _conImport = conImport;
            _conApp = conApp;

            List<ApplicationView> TestList = new List<ApplicationView>();
            ApplicationView test = null;
            //Console.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "StoredProcedureFromZero.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            //string val = "'" + ApplicatonVal + "';";
            //string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);

            string val = "'" + ApplicatonVal + "'," + tempFrom + "," + tempTo + ";";
            string ValueApplication = string.Concat(data, val);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);


            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode,
                                   isDeletedApp = all.isDeletedApp
                               }
                              ).ToList();
            List<long> lowList = new List<long>();
            //Debug.WriteLine("ukupno:" + importQuery.Count);
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new ApplicationView();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;
                    //Debug.WriteLine("ukupno:" + temp.name);


                    test.Application = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink,
                                          all.Type_of_doc
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {

                            if (values.Type_of_doc == "R")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = (int)values.DocumentsId,
                                    isRegulatory = true
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            else if (values.Type_of_doc == "I")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = (int)values.DocumentsId,
                                    isRegulatory = false
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            //app.DocumentId.Add(values.DocumentsId);

                        }

                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink,
                                          all.Type_of_Document
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();

                        if (values.Standard_id > 0)
                        {
                            if (values.Type_of_Document == "R")
                            {
                                StandardsConvertNew dd = new StandardsConvertNew()
                                {
                                    StandardId = (int)values.Standard_id,
                                    isRegulatoryStand = true
                                };
                                app.StandardsAditional.Add(dd);
                            }
                            else if (values.Type_of_Document == "I")
                            {
                                StandardsConvertNew dd = new StandardsConvertNew()
                                {
                                    StandardId = (int)values.Standard_id,
                                    isRegulatoryStand = false
                                };
                                app.StandardsAditional.Add(dd);
                            }

                            //app.StandardId.Add(values.Standard_id);
                            //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);

                        }

                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;
                    app.isDeletedApp = temp.isDeletedApp;

                    test.Application.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    //temp.low
                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink,
                                          all.Type_of_Document
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application.Equals(temp.name))
                            {
                                //Debug.WriteLine("id standard:" + values.Standard_id+"::"+temp.low);
                                var App = tempObj.Application.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Standard = App.Standards.Where(v => v.StandardId == values.Standard_id).SingleOrDefault();
                                //var Document = App.Standards.Where(v => v. == values.Doc_number).SingleOrDefault();
                                if (Standard == null)
                                {
                                    if (values.Type_of_Document == "R")
                                    {
                                        StandardsConvertNew ss = new StandardsConvertNew()
                                        {
                                            StandardId = (int)values.Standard_id,
                                            isRegulatoryStand = true
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                    }
                                    else if (values.Type_of_Document == "I")
                                    {
                                        StandardsConvertNew ss = new StandardsConvertNew()
                                        {
                                            StandardId = (int)values.Standard_id,
                                            isRegulatoryStand = false
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                    }
                                    //App.Standards.Add(sc3);
                                    //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardId.Add(values.Standard_id);
                                }
                            }
                        }


                    }

                    //temp.low
                    if (temp.documents_Id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink,
                                          all.Type_of_doc
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application.Equals(temp.name))
                            {


                                var App = tempObj.Application.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.DocumentIt == values.DocumentsId).SingleOrDefault();
                                if (Document == null)
                                {
                                    if (values.Type_of_doc == "R")
                                    {
                                        DocumentConvertNew dd = new DocumentConvertNew()
                                        {
                                            DocumentId = (int)values.DocumentsId,
                                            isRegulatory = true
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                    }
                                    else if (values.Type_of_doc == "I")
                                    {
                                        DocumentConvertNew dd = new DocumentConvertNew()
                                        {
                                            DocumentId = (int)values.DocumentsId,
                                            isRegulatory = false
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                    }
                                    //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                    //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentId.Add(Document.DocumentIt);
                                }
                            }
                        }


                    }

                    //app.Application = temp.name;
                    app.Comment = temp.comment;
                    app.isDeletedApp = temp.isDeletedApp;

                }

            }
            return TestList;
        }


        List<FreqBandSearchNew> SearchAppOnSecondLevelProcedureFromZero(long tempFrom, long tempTo, IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;
            List<FreqBandSearchNew> TestList = new List<FreqBandSearchNew>();
            FreqBandSearchNew test = null;


            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "StoredProcedureFromZero.sql");
            string data = System.IO.File.ReadAllText(Servpath);

            string val = "'" + ApplicatonVal + "'," + tempFrom + "," + tempTo + ";";
            string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("dada:" + ValueApplication);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);



            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode
                               }
                                  ).ToList();
            List<long> lowList = new List<long>();
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new FreqBandSearchNew();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;

                    test.Allocation = "";
                    test.isPrimary = false;
                    test.Footnote = null;
                    test.BandFootnote = null;

                    test.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();

                        if (values.Doc_number != "")
                        {
                            DocumentsConvert ds3 = new DocumentsConvert();
                            ds3.Doc_number = values.Doc_number;
                            ds3.Title_of_doc = values.Title_of_doc;
                            ds3.Hyperlink = values.Hyperlink;
                            app.Documents.Add(ds3);

                        }
                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    test.AppItemsList.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    if (temp.documents_Id != null)
                    {
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();

                        //DocumentsConvert dc = new DocumentsConvert();
                        //dc.Doc_number = values.Doc_number;
                        //dc.Title_of_doc = values.Title_of_doc;
                        //dc.Hyperlink = values.Hyperlink;
                        //app.Documents.Add(dc);

                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {


                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.Doc_number == values.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = values.Doc_number;
                                    ds3.Title_of_doc = values.Title_of_doc;
                                    ds3.Hyperlink = values.Hyperlink;
                                    app.Documents.Add(ds3);
                                }
                            }
                        }
                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;
                    var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    if (obj != null) obj.AppItemsList.Add(app);
                    //var tempApp = TestList.Find(x => x.low == temp.low);
                }

            }
            return TestList;


        }


        public List<FreqBandSearchNewDocStand> SearchAppOnSecondLevelProcedureFromLowHighNew(long tempFrom, long tempTo, IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;

            List<FreqBandSearchNewDocStand> TestList = new List<FreqBandSearchNewDocStand>();
            FreqBandSearchNewDocStand test = null;
            //Console.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "SearchApplicationFromLowHigh.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            //string val = "'" + ApplicatonVal + "';";
            //string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);

            string val = "'" + ApplicatonVal + "'," + tempFrom + "," + tempTo + ";";
            string ValueApplication = string.Concat(data, val);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);


            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode
                               }
                              ).ToList();
            List<long> lowList = new List<long>();
            //Debug.WriteLine("ukupno:" + importQuery.Count);
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new FreqBandSearchNewDocStand();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;
                    //Debug.WriteLine("ukupno:" + temp.name);
                    test.Application = temp.name;
                    test.Allocation = "";
                    test.isPrimary = false;
                    test.Footnote = null;
                    test.BandFootnote = null;

                    test.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    app.Standards = new List<StandardsConvert>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {
                            DocumentsConvert ds3 = new DocumentsConvert();
                            ds3.DocumentIt = values.DocumentsId;
                            ds3.Doc_number = values.Doc_number;
                            ds3.Title_of_doc = values.Title_of_doc;
                            ds3.Hyperlink = values.Hyperlink;
                            app.Documents.Add(ds3);

                        }

                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();

                        if (values.Standard_id > 0)
                        {

                            StandardsConvert sc3 = new StandardsConvert();
                            sc3.StandardId = values.Standard_id;
                            sc3.Etsi_standard = values.Etsi_standard;
                            sc3.Title_docS = values.Title_doc;
                            sc3.HyperlinkS = values.Hypelink;
                            app.Standards.Add(sc3);
                            //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);

                        }

                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    test.AppItemsList.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    app.Standards = new List<StandardsConvert>();
                    //temp.low
                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {
                                //Debug.WriteLine("id standard:" + values.Standard_id+"::"+temp.low);
                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Standard = App.Standards.Where(v => v.StandardId == values.Standard_id).SingleOrDefault();
                                //var Document = App.Standards.Where(v => v. == values.Doc_number).SingleOrDefault();
                                if (Standard == null)
                                {
                                    StandardsConvert sc3 = new StandardsConvert();
                                    sc3.StandardId = values.Standard_id;
                                    sc3.Etsi_standard = values.Etsi_standard;
                                    sc3.Title_docS = values.Title_doc;
                                    sc3.HyperlinkS = values.Hypelink;
                                    //App.Standards.Add(sc3);
                                    TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault().Standards.Add(sc3);
                                }
                            }
                        }


                    }

                    //temp.low
                    if (temp.documents_Id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {


                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.DocumentIt == values.DocumentsId).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = values.Doc_number;
                                    ds3.Title_of_doc = values.Title_of_doc;
                                    ds3.Hyperlink = values.Hyperlink;
                                    App.Documents.Add(ds3);
                                    //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);
                                    TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault().Documents.Add(ds3);
                                }
                            }
                        }


                    }

                    //app.Application = temp.name;
                    app.Comment = temp.comment;
                    //TestList.FirstOrDefault(x => x.low == temp.low).AppItemsList.Add(app);
                    //var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    //if (obj != null) obj.AppItemsList.Add(app);

                }

            }
            return TestList;

        }


        public List<ApplicationView> SearchAppOnSecondLevelProcedureFromLowHighNewPerfomance(long tempFrom, long tempTo, IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {

            _conImport = conImport;
            _conApp = conApp;

            List<ApplicationView> TestList = new List<ApplicationView>();
            ApplicationView test = null;
            //Console.WriteLine("just check:");

            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "SearchApplicationFromLowHigh.sql");

            //Debug.WriteLine("just check:"+ Servpath);
            string data = System.IO.File.ReadAllText(Servpath);

            //string val = "'" + ApplicatonVal + "';";
            //string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("second qqqq:" + ValueApplication);

            string val = "'" + ApplicatonVal + "'," + tempFrom + "," + tempTo + ";";
            string ValueApplication = string.Concat(data, val);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);


            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode,
                                   isDeletedApp = all.isDeletedApp
                               }
                              ).ToList();
            List<long> lowList = new List<long>();
            //Debug.WriteLine("ukupno:" + importQuery.Count);
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new ApplicationView();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;
                    //Debug.WriteLine("ukupno:" + temp.name);


                    test.Application = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    if (temp.documents_Id != null)
                    {

                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink,
                                          all.Type_of_doc
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();
                        //Debug.WriteLine("im here:"+values.Doc_number);

                        if (values.Doc_number != "")
                        {
                            if (values.Type_of_doc == "R")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = values.DocumentsId,
                                    isRegulatory = true
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            else if (values.Type_of_doc == "I")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = values.DocumentsId,
                                    isRegulatory = false
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            //app.DocumentId.Add(values.DocumentsId);

                        }

                    }
                    else
                    {
                        //app.Doc_number = null;
                    }

                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink,
                                          all.Type_of_Document
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();

                        if (values.Standard_id > 0)
                        {
                            if (values.Type_of_Document == "R")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = (int)values.Standard_id,
                                    isRegulatory = true
                                };
                                app.DocumentsAditional.Add(dd);
                            }
                            else if (values.Type_of_Document == "I")
                            {
                                DocumentConvertNew dd = new DocumentConvertNew()
                                {
                                    DocumentId = (int)values.Standard_id,
                                    isRegulatory = false
                                };
                                app.DocumentsAditional.Add(dd);
                            }

                            //app.StandardId.Add(values.Standard_id);
                            //ge.AppItemsList.Where(x => x.Application == p.Application).SingleOrDefault().Documents.Add(ds3);

                        }

                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;
                    app.isDeletedApp = temp.isDeletedApp;

                    test.Application.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {

                    ApplicationConvert app = new ApplicationConvert();
                    //app.DocumentId = new List<int>();
                    //app.StandardId = new List<int>();
                    app.DocumentsAditional = new List<DocumentConvertNew>();
                    app.StandardsAditional = new List<StandardsConvertNew>();
                    //temp.low
                    if (temp.Standard_id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.StandardsDb
                                      select new
                                      {
                                          all.Standard_id,
                                          all.Etsi_standard,
                                          all.Title_doc,
                                          all.Hypelink,
                                          all.Type_of_Document
                                      }).Where(x => x.Standard_id == temp.Standard_id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application.Equals(temp.name))
                            {
                                //Debug.WriteLine("id standard:" + values.Standard_id+"::"+temp.low);
                                var App = tempObj.Application.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Standard = App.Standards.Where(v => v.StandardId == values.Standard_id).SingleOrDefault();
                                //var Document = App.Standards.Where(v => v. == values.Doc_number).SingleOrDefault();
                                if (Standard == null)
                                {
                                    if (values.Type_of_Document == "R")
                                    {
                                        StandardsConvertNew ss = new StandardsConvertNew()
                                        {
                                            StandardId = (int)values.Standard_id,
                                            isRegulatoryStand = true
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                    }
                                    else if (values.Type_of_Document == "I")
                                    {
                                        StandardsConvertNew ss = new StandardsConvertNew()
                                        {
                                            StandardId = (int)values.Standard_id,
                                            isRegulatoryStand = false
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardsAditional.Add(ss);
                                    }
                                    //TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().StandardId.Add(values.Standard_id);
                                }
                            }
                        }


                    }

                    //temp.low
                    if (temp.documents_Id != null)
                    {
                        // DocumentsConvert ds = new DocumentsConvert();
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink,
                                          all.Type_of_doc
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();


                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application.Equals(temp.name))
                            {


                                var App = tempObj.Application.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.DocumentIt == values.DocumentsId).SingleOrDefault();
                                if (Document == null)
                                {
                                    if (values.Type_of_doc == "R")
                                    {
                                        DocumentConvertNew dd = new DocumentConvertNew()
                                        {
                                            DocumentId = (int)values.DocumentsId,
                                            isRegulatory = true
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                    }
                                    else if (values.Type_of_doc == "I")
                                    {
                                        DocumentConvertNew dd = new DocumentConvertNew()
                                        {
                                            DocumentId = (int)values.DocumentsId,
                                            isRegulatory = false
                                        };
                                        TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentsAditional.Add(dd);
                                    }

                                    // TestList.FirstOrDefault(x => x.low == temp.low).Application.Where(x => x.Application == temp.name).SingleOrDefault().DocumentId.Add(values.DocumentsId);
                                }
                            }
                        }


                    }

                    //app.Application = temp.name;
                    app.Comment = temp.comment;
                    app.isDeletedApp = temp.isDeletedApp;

                }

            }
            return TestList;

        }


        public List<FreqBandSearchNew> SearchAppOnSecondLevelProcedureFromLowHigh(long tempFrom, long tempTo, IConfiguration conf, string ApplicatonVal, ImportTempTableContext conImport, ApplicationDBContext conApp)
        {
            _conImport = conImport;
            _conApp = conApp;
            List<FreqBandSearchNew> TestList = new List<FreqBandSearchNew>();
            FreqBandSearchNew test = null;


            SqlConnection conn = new SqlConnection(conf.GetConnectionString("AuthDBContextConnection"));
            var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        "SearchApplicationFromLowHigh.sql");
            string data = System.IO.File.ReadAllText(Servpath);

            string val = "'" + ApplicatonVal + "'," + tempFrom + "," + tempTo + ";";
            string ValueApplication = string.Concat(data, val);
            //Debug.WriteLine("dada:" + ValueApplication);
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = ValueApplication;
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            System.Threading.Thread.Sleep(300);



            var importQuery = (from all in _conImport.storedTableApplicationProcedure
                               select new storedTableApplicationProcedure
                               {
                                   ApplicationId = all.ApplicationId,
                                   ApplicationRangeId = all.ApplicationRangeId,
                                   ApplicationTermId = all.ApplicationTermId,
                                   comment = all.comment,
                                   documents_Id = all.documents_Id,
                                   Standard_id = all.Standard_id,
                                   low = all.low,
                                   high = all.high,
                                   RootApplicationDBId = all.RootApplicationDBId,
                                   LowView = all.LowView,
                                   HighView = all.HighView,
                                   name = all.name,
                                   regionId = all.regionId,
                                   regionName = all.regionName,
                                   regionCode = all.regionCode
                               }
                                  ).ToList();
            List<long> lowList = new List<long>();
            foreach (var temp in importQuery)
            {
                if (!lowList.Contains(temp.low))
                {
                    test = new FreqBandSearchNew();
                    test.low = (long)temp.low;
                    test.high = (long)temp.high;

                    test.Allocation = "";
                    test.isPrimary = false;
                    test.Footnote = null;
                    test.BandFootnote = null;

                    test.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    if (temp.documents_Id != null)
                    {
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();

                        if (values.Doc_number != "")
                        {
                            DocumentsConvert ds3 = new DocumentsConvert();
                            ds3.Doc_number = values.Doc_number;
                            ds3.Title_of_doc = values.Title_of_doc;
                            ds3.Hyperlink = values.Hyperlink;
                            app.Documents.Add(ds3);

                        }
                    }
                    else
                    {
                        //app.Doc_number = null;
                    }
                    app.Application = temp.name;
                    app.Comment = temp.comment;
                    test.AppItemsList.Add(app);
                    test.regionName = temp.regionName;
                    test.regionCode = temp.regionCode;
                    test.LowView = temp.LowView;
                    test.HighView = temp.HighView;

                    TestList.Add(test);
                    lowList.Add(temp.low);
                }
                else
                {
                    ApplicationConvert app = new ApplicationConvert();
                    app.Documents = new List<DocumentsConvert>();
                    if (temp.documents_Id != null)
                    {
                        var values = (from all in _conApp.DocumentsDb
                                      select new
                                      {
                                          all.DocumentsId,
                                          all.Doc_number,
                                          all.Title_of_doc,
                                          all.Hyperlink
                                      }).Where(x => x.DocumentsId == temp.documents_Id).SingleOrDefault();

                        var objElse = TestList.Where(x => x.low == temp.low).ToList();

                        foreach (var tempObj in objElse)
                        {
                            if (tempObj.Application == temp.name)
                            {


                                var App = tempObj.AppItemsList.Where(x => x.Application == temp.name).SingleOrDefault();
                                var Document = App.Documents.Where(v => v.Doc_number == values.Doc_number).SingleOrDefault();
                                if (Document == null)
                                {
                                    DocumentsConvert ds3 = new DocumentsConvert();
                                    ds3.Doc_number = values.Doc_number;
                                    ds3.Title_of_doc = values.Title_of_doc;
                                    ds3.Hyperlink = values.Hyperlink;
                                    app.Documents.Add(ds3);
                                }
                            }
                        }
                    }

                    app.Application = temp.name;
                    app.Comment = temp.comment;

                    var obj = TestList.FirstOrDefault(x => x.low == temp.low);
                    if (obj != null) obj.AppItemsList.Add(app);
                    //var tempApp = TestList.Find(x => x.low == temp.low);
                }

            }
            return TestList;

        }

        public List<FreqBandSearchNew> SearchAppOnSecondLevel(ApplicationDBContext _tempApp, long tempFrom, long tempTo, string FrequencytableValue, string ApplicatonVal)
        {
            _conApp = _tempApp;
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
            List<AllSearchFreqBand> AllAppValues = null;
            if (tempFrom == 0 && tempTo == 0)
            {
                AllAppValues = appQuery.Where(x => x.Application.Equals(ApplicatonVal)).ToList();
                //Debug.WriteLine("value:" + ApplicatonVal);

            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                var allY = appQuery.Where(x => x.low >= tempFrom).ToList();
                var ValuesApp = allY.Where(x => x.high <= tempTo).ToList();
                AllAppValues = ValuesApp.Where(x => x.Application.Equals(ApplicatonVal)).ToList();
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                var allY = appQuery.Where(x => x.high >= tempFrom).ToList();
                var ValuesApp = allY.Where(x => x.low <= tempTo).ToList();
                AllAppValues = ValuesApp.Where(x => x.Application.Equals(ApplicatonVal)).ToList();
            }

            List<FreqBandSearch> allS = new List<FreqBandSearch>();

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
                    Allocation = "",
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
                ge.AppItemsList = new List<ApplicationConvert>();
                ApplicationConvert ap = new ApplicationConvert()
                {
                    Application = d.MostRecent.Application,
                    Comment = d.MostRecent.Comment
                };
                ge.AppItemsList.Add(ap);
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
                if (others.Count > 0)
                {
                    ge.Application += ", ";
                }
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
                                ApplicationConvert apC = new ApplicationConvert()
                                {
                                    Application = p.Application,
                                    Comment = p.Comment
                                };
                                ge.AppItemsList.Add(apC);
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
                ge.AppItemsList.OrderBy(x => x.Application);
                //ge.Footnote.OrderByDescending(e => e.isPrimary == true);
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

                    temp.MostRecent.AppItemsList = new List<ApplicationConvert>();
                    ApplicationConvert ap = new ApplicationConvert()
                    {
                        Application = temp.MostRecent.Application,
                        Comment = temp.MostRecent.Comment
                    };
                    FootnoteJsonConvert te = new FootnoteJsonConvert();
                    te.Allocation = ww.Allocation;
                    te.Footnote = "";
                    te.FootnoteDesc = "";
                    te.isBand = false;
                    te.isPrimary = ww.isPrimary;
                    ww.Footnote.Add(te);
                    ww.AppItemsList.Add(ap);
                    ww.AppItemsList.OrderBy(x => x.Application);
                    list.Add(ww);
                }

                //Debug.WriteLine("values:" + ww.Allocation + "==" + ww.low+"ggg:"+i++);
            }


            //list.OrderBy(e => e.Footnote.OrderByDescending(s => s.isPrimary == true));
            listGeneral.AddRange(list);

            //Debug.WriteLine("radi:" + listGeneral.Count);
            return listGeneral.OrderBy(e => e.low).ToList();

        }
    }
}

