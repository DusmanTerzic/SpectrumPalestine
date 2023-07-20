using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers.Actions
{
    public class DocumentActions
    {

        private ApplicationDBContext _conApp;

        public List<DocumentStandardView> SearchAllDocumentsByType(ApplicationDBContext _tempconApp, string FrequencytableValue,string DocumentType)
        {

            _conApp = _tempconApp;
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
                int DocId = 0;
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
                    DocId = id;
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
                    DocumentId = DocId,
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
                    ds2.DocumentIt = d.MostRecent.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                    ds2.DocumentIt = temp.MostRecent.DocumentId;
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
            List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();

            foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                foreach (var tempAppItemList in values.AppItemsList)
                {

                    foreach (var tempDoc in tempAppItemList.Documents)
                    {
                        if (tempDoc.DocumentIt > 0)
                        {
                            DocumentStandardView doc = new DocumentStandardView();

                            var ll = (from ww in _conApp.DocumentsDb
                                      select new
                                      {
                                          ww.DocumentsId,
                                          ww.Doc_number,
                                          ww.Title_of_doc,
                                          ww.Hyperlink,
                                          ww.Group_doc

                                      }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                            doc.Application = tempAppItemList.Application;
                            doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                            doc.Notes = tempAppItemList.Comment;
                            doc.FrequencyBand = values.LowView + " - " + values.HighView;
                            doc.Link = tempDoc.Hyperlink;
                            doc.Type = ll.Group_doc;
                            listOfDocumentsStand.Add(doc);
                        }
                    }

                    foreach (var tempStand in tempAppItemList.Standards)
                    {
                        if (tempStand.StandardId > 0)
                        {
                            DocumentStandardView stand = new DocumentStandardView();
                            var Standard = (from ww in _conApp.StandardsDb
                                            select new
                                            {
                                                ww.Standard_id,
                                                ww.Etsi_standard,
                                                ww.Title_doc,
                                                ww.Hypelink,
                                                ww.Group_doc,
                                                ww.Low_freq,
                                                ww.High_freq,
                                                ww.Application
                                            }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                            stand.Application = tempAppItemList.Application;
                            stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                            stand.Notes = tempAppItemList.Comment;
                            stand.FrequencyBand = values.LowView + " - " + values.HighView;
                            stand.Link = tempStand.HyperlinkS;
                            stand.Type = Standard.Group_doc;
                            listOfDocumentsStand.Add(stand);
                        }
                    }
                }
            }
            var newListByType = listOfDocumentsStand.Where(x => x.Type.Equals(DocumentType)).ToList();

            return newListByType;
        }

        public List<DocumentStandardView> SearchAllDocumentsFromToByType(ApplicationDBContext _tempconApp, string FrequencytableValue, long tempFrom, long tempTo, string DocumentType)
        {
            //convesion from hz,mhz and ghz
            _conApp = _tempconApp;
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

            List<AllSearchFreqBand> valueValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();
            //Debug.WriteLine("ppp:" + tempFrom + "==" + tempTo + ":count=" + valueValues.Count);
            if (tempFrom == 0 && tempTo == 0)
            {
                AllAppValues = (from x in valueValues select (AllSearchFreqBand)x).ToList();
                //Debug.WriteLine("value:" + ApplicatonVal);

            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                var allY = valueValues.Where(x => x.low >= tempFrom).ToList();
                // Debug.WriteLine("zero:" + tempFrom + "==" + tempTo + ":count=" + allY.Count);
                AllAppValues = allY.Where(x => x.high <= tempTo).ToList();

            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                var allY = valueValues.Where(x => x.high >= tempFrom).ToList();
                AllAppValues = allY.Where(x => x.low <= tempTo).ToList();

            }

            //Debug.WriteLine("values:" + AllAppValues.Count);
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
                int DocId = 0;
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
                    DocId = id;
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
                    DocumentId = DocId,
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
                    ds2.DocumentIt = d.MostRecent.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                    ds2.DocumentIt = temp.MostRecent.DocumentId;
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
            List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();

            foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                foreach (var tempAppItemList in values.AppItemsList)
                {

                    foreach (var tempDoc in tempAppItemList.Documents)
                    {
                        if (tempDoc.DocumentIt > 0)
                        {
                            DocumentStandardView doc = new DocumentStandardView();

                            var ll = (from ww in _conApp.DocumentsDb
                                      select new
                                      {
                                          ww.DocumentsId,
                                          ww.Doc_number,
                                          ww.Title_of_doc,
                                          ww.Hyperlink,
                                          ww.Group_doc

                                      }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                            doc.Application = tempAppItemList.Application;
                            doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                            doc.Notes = tempAppItemList.Comment;
                            doc.FrequencyBand = values.LowView + " - " + values.HighView;
                            doc.Link = tempDoc.Hyperlink;
                            doc.Type = ll.Group_doc;
                            listOfDocumentsStand.Add(doc);
                        }
                    }

                    foreach (var tempStand in tempAppItemList.Standards)
                    {
                        if (tempStand.StandardId > 0)
                        {
                            DocumentStandardView stand = new DocumentStandardView();
                            var Standard = (from ww in _conApp.StandardsDb
                                            select new
                                            {
                                                ww.Standard_id,
                                                ww.Etsi_standard,
                                                ww.Title_doc,
                                                ww.Hypelink,
                                                ww.Group_doc,
                                                ww.Low_freq,
                                                ww.High_freq,
                                                ww.Application
                                            }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                            stand.Application = tempAppItemList.Application;
                            stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                            stand.Notes = tempAppItemList.Comment;
                            stand.FrequencyBand = values.LowView + " - " + values.HighView;
                            stand.Link = tempStand.HyperlinkS;
                            stand.Type = Standard.Group_doc;
                            listOfDocumentsStand.Add(stand);
                        }
                    }
                }
            }
            //Debug.WriteLine("done by type" + listOfDocumentsStand.Count+"=="+ DocumentType);
            //there was typo in column of database Type for group of doc, right way Harmonised Standards wrong way Harmonised standards
            //foreach(var pp in listOfDocumentsStand)
            //{
            //    if(pp.Type == DocumentType)
            //    {
            //        Debug.WriteLine("same:"+pp.CombineTitle);
            //    }
            //    else
            //    {
            //        Debug.WriteLine("diffrent:" + pp.CombineTitle+"=="+pp.Type);
            //    }
            //      
            //}
            var newListByType = listOfDocumentsStand.Where(x => x.Type == DocumentType).ToList();
           // Debug.WriteLine("done by type2:" + newListByType.Count);
            return newListByType;
        }


        public List<DocumentStandardView> SearchAllDocuments(ApplicationDBContext _tempconApp, string FrequencytableValue)
        {
            //Debug.WriteLine("ww:SearchAllDoc");
            _conApp = _tempconApp;
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
                int DocId = 0;
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
                    DocId = id;
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
                    DocumentId = DocId,
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
                    ds2.DocumentIt = d.MostRecent.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                    ds2.DocumentIt = temp.MostRecent.DocumentId;
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
            List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();
           
            foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                foreach (var tempAppItemList in values.AppItemsList)
                {

                    foreach (var tempDoc in tempAppItemList.Documents)
                    {
                        if (tempDoc.DocumentIt > 0)
                        {
                            DocumentStandardView doc = new DocumentStandardView();

                            var ll = (from ww in _conApp.DocumentsDb
                                      select new
                                      {
                                          ww.DocumentsId,
                                          ww.Doc_number,
                                          ww.Title_of_doc,
                                          ww.Hyperlink,
                                          ww.Group_doc

                                      }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                            doc.Application = tempAppItemList.Application;
                            doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                            doc.Notes = tempAppItemList.Comment;
                            doc.FrequencyBand = values.LowView + " - " + values.HighView;
                            doc.Link = tempDoc.Hyperlink;
                            doc.Type = ll.Group_doc;
                            listOfDocumentsStand.Add(doc);
                        }
                    }

                    foreach (var tempStand in tempAppItemList.Standards)
                    {
                        if (tempStand.StandardId > 0)
                        {
                            DocumentStandardView stand = new DocumentStandardView();
                            var Standard = (from ww in _conApp.StandardsDb
                                            select new
                                            {
                                                ww.Standard_id,
                                                ww.Etsi_standard,
                                                ww.Title_doc,
                                                ww.Hypelink,
                                                ww.Group_doc,
                                                ww.Low_freq,
                                                ww.High_freq,
                                                ww.Application
                                            }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                            stand.Application = tempAppItemList.Application;
                            stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                            stand.Notes = tempAppItemList.Comment;
                            stand.FrequencyBand = values.LowView + " - " + values.HighView;
                            stand.Link = tempStand.HyperlinkS;
                            stand.Type = Standard.Group_doc;
                            listOfDocumentsStand.Add(stand);
                        }
                    }
                }
            }

            return listOfDocumentsStand;
        }

        public List<DocumentStandardView> SearchAllDocumentsFromTo(ApplicationDBContext _tempconApp, string FrequencytableValue,long tempFrom, long tempTo)
        {
            //convesion from hz,mhz and ghz
            _conApp = _tempconApp;
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

            List<AllSearchFreqBand> valueValues = (from x in appQuery select (AllSearchFreqBand)x).ToList();
            //Debug.WriteLine("ppp:" + tempFrom + "==" + tempTo + ":count=" + valueValues.Count);
            if (tempFrom == 0 && tempTo == 0)
            {
                AllAppValues = (from x in valueValues select (AllSearchFreqBand)x).ToList();
                //Debug.WriteLine("value:" + ApplicatonVal);

            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                var allY = valueValues.Where(x => x.low >= tempFrom).ToList();
               // Debug.WriteLine("zero:" + tempFrom + "==" + tempTo + ":count=" + allY.Count);
                AllAppValues = allY.Where(x => x.high <= tempTo).ToList();
                
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                var allY = valueValues.Where(x => x.high >= tempFrom).ToList();
                AllAppValues = allY.Where(x => x.low <= tempTo).ToList();
                
            }

            //Debug.WriteLine("values:" + AllAppValues.Count);
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
                int DocId = 0;
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
                    DocId = id;
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
                    DocumentId = DocId,
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
                    ds2.DocumentIt = d.MostRecent.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                                    ds3.DocumentIt = p.DocumentId;
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
                    ds2.DocumentIt = temp.MostRecent.DocumentId;
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
            List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();

            foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
            {
                values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                foreach (var tempAppItemList in values.AppItemsList)
                {

                    foreach (var tempDoc in tempAppItemList.Documents)
                    {
                        if (tempDoc.DocumentIt > 0)
                        {
                            DocumentStandardView doc = new DocumentStandardView();

                            var ll = (from ww in _conApp.DocumentsDb
                                      select new
                                      {
                                          ww.DocumentsId,
                                          ww.Doc_number,
                                          ww.Title_of_doc,
                                          ww.Hyperlink,
                                          ww.Group_doc

                                      }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                            doc.Application = tempAppItemList.Application;
                            doc.Notes = tempAppItemList.Comment;
                            doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                            doc.FrequencyBand = values.LowView + " - " + values.HighView;
                            doc.Link = tempDoc.Hyperlink;
                            doc.Type = ll.Group_doc;
                            listOfDocumentsStand.Add(doc);
                        }
                    }

                    foreach (var tempStand in tempAppItemList.Standards)
                    {
                        if (tempStand.StandardId > 0)
                        {
                            DocumentStandardView stand = new DocumentStandardView();
                            var Standard = (from ww in _conApp.StandardsDb
                                            select new
                                            {
                                                ww.Standard_id,
                                                ww.Etsi_standard,
                                                ww.Title_doc,
                                                ww.Hypelink,
                                                ww.Group_doc,
                                                ww.Low_freq,
                                                ww.High_freq,
                                                ww.Application
                                            }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                            stand.Application = tempAppItemList.Application;
                            stand.Notes = tempAppItemList.Comment;
                            stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                            stand.FrequencyBand = values.LowView + " - " + values.HighView;
                            stand.Link = tempStand.HyperlinkS;
                            stand.Type = Standard.Group_doc;
                            listOfDocumentsStand.Add(stand);
                        }
                    }
                }
            }
            //Debug.WriteLine("done"+ listOfDocumentsStand.Count);

            return listOfDocumentsStand;
        }

        }
}
