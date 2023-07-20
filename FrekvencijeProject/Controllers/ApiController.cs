using FrekvencijeProject.JSON.Allocations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FrekvencijeProject.Models;
using FrekvencijeProject.JSON.Footnote;
using FrekvencijeProject.JSON.ApplicationTerms;
using FrekvencijeProject.Models.ApplicationTerms;
using FrekvencijeProject.JSON.AllocationTerms;
using FrekvencijeProject.JSON.Application;
using FrekvencijeProject.Models.Application;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using ExcelDataReader;
using FrekvencijeProject.Models.Allocation;

namespace FrekvencijeProject.Controllers
{
    public class ApiController : Controller
    {
        private readonly AllocationDBContext _context;
        private readonly ApplicationTermsDBContext _conAppTerms;
        private readonly AllocationTermsDBContext _conAllTerms;
        private readonly ApplicationDBContext _conAppContext;
        //this controller is used for parsing the api from json.
        public ApiController(AllocationDBContext context, ApplicationTermsDBContext conAppTerms,
            AllocationTermsDBContext conAllTerms, ApplicationDBContext conAppContext)
        {
            _context = context;
            _conAppTerms = conAppTerms;
            _conAllTerms = conAllTerms;
            _conAppContext = conAppContext;
        }
        public IActionResult Index()
        {
            //string str = ((object)ViewBag.StatusCode).ToString();
            // Debug.WriteLine("testiranje:");
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> GetAllocations()
        {
            // Debug.WriteLine("values:");
            Root reservation = new Root();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org/allocations/ranges?regionId=2"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Debug.WriteLine("user ok:"+ response.StatusCode);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<Root>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;


                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.allocationRanges[i].allocations.Count; j++)
                            {
                                AllocationTermDb allocationTermTemp = new AllocationTermDb();
                                allocationTermTemp = new AllocationTermDb()
                                {
                                    AllocationTermId = reservation.allocationRanges[i].allocations[j].allocationTerm.id,
                                    name = reservation.allocationRanges[i].allocations[j].allocationTerm.name
                                };

                                if (!_context.AllocationTermDb.Contains(allocationTermTemp))
                                {
                                    _context.AllocationTermDb.Add(allocationTermTemp);
                                    _context.SaveChanges();
                                }


                            }
                        }

                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            RootAllocationDB root = new RootAllocationDB()
                            {
                                regionId = reservation.regionId,
                                regionName = reservation.regionName,
                                regionCode = reservation.regionCode
                            };

                            int value = _context.RootAllocationDB.Where(x => x.regionId == root.regionId).Count();
                            if (value == 0)
                            {
                                _context.RootAllocationDB.Add(root);
                                _context.SaveChanges();
                            }
                        }


                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            RootAllocationDB valueRoot = _context.RootAllocationDB.Where(x => x.regionId == reservation.regionId).SingleOrDefault();
                            AllocationRangeDb all = new AllocationRangeDb()
                            {
                                low = reservation.allocationRanges[i].low,
                                high = reservation.allocationRanges[i].high,
                                RootAllocationDBId = valueRoot.RootAllocationDBId
                            };
                            int value = _context.AllocationRangeDb.Where(x => x.low == all.low && x.high == all.high).Count();
                            if (value == 0)
                            {
                                // Debug.Write("vrijednost AllocationRangeDb:" + value);
                                _context.AllocationRangeDb.Add(all);
                                _context.SaveChanges();
                            }
                        }




                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.allocationRanges[i].allocations.Count; j++)
                            {
                                long low = reservation.allocationRanges[i].low;
                                long high = reservation.allocationRanges[i].high;
                                var temp = reservation.allocationRanges[i].allocations[j];


                                int value = _context.AllocationRangeDb.Where(x => x.low == low && x.high == high).Count();

                                if (value == 1)
                                {
                                    // Debug.Write("vrijednost:" + value);
                                    var allocationRange = _context.AllocationRangeDb.Where(x => x.low == low && x.high == high).SingleOrDefault();
                                    // Debug.Write("vrijednost all:" + allocationRange.AllocationRangeId);
                                    AllocationTermDb tempAllTerm = null;
                                    if (reservation.allocationRanges[i].allocations[j].allocationTerm.id > 0)
                                    {
                                        //Debug.WriteLine("vrijednost allocationTerm id if uslov:" + reservation.allocationRanges[i].allocations[j].allocationTerm.id);
                                        tempAllTerm = new AllocationTermDb()
                                        {
                                            AllocationTermId = reservation.allocationRanges[i].allocations[j].allocationTerm.id,
                                            name = reservation.allocationRanges[i].allocations[j].allocationTerm.name
                                        };
                                    }
                                    // Debug.WriteLine("vrijednost allocationTerm id:" + reservation.allocationRanges[i].allocations[j].allocationTerm.id);

                                    AllocationDb allocation = new AllocationDb()
                                    {
                                        allocationRangeDb = allocationRange,
                                        primary = reservation.allocationRanges[i].allocations[j].primary,
                                        AllocationTermId = tempAllTerm.AllocationTermId

                                    };

                                    if (!_context.AllocationDb.Contains(allocation))
                                    {


                                        var AllocationCount = _context.AllocationDb.Where(y => y.AllocationRangeId == allocationRange.AllocationRangeId && y.AllocationTermId == reservation.allocationRanges[i].allocations[j].allocationTerm.id).Count();
                                        if (AllocationCount == 0)
                                        {
                                            _context.AllocationDb.Add(allocation);
                                            _context.SaveChanges();
                                        }

                                    }
                                }
                            }

                        }

                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.allocationRanges[i].allocations.Count; j++)
                            {
                                for (int h = 0; h < reservation.allocationRanges[i].allocations[j].footnotes.Count; h++)
                                {
                                    if (reservation.allocationRanges[i].allocations[j].footnotes[h].id > 0)
                                    {
                                        var allocationRange = _context.AllocationRangeDb.Where(x => x.low == reservation.allocationRanges[i].low).SingleOrDefault();
                                        var Allocation = _context.AllocationDb.Where(y => y.AllocationRangeId == allocationRange.AllocationRangeId && y.AllocationTermId == reservation.allocationRanges[i].allocations[j].allocationTerm.id).SingleOrDefault();

                                        // Debug.WriteLine("vrijednost allocation id:" + Allocation.AllocationId);

                                        FootnoteAllocation footnoteAllocation = new FootnoteAllocation()
                                        {
                                            id = reservation.allocationRanges[i].allocations[j].footnotes[h].id,
                                            name = reservation.allocationRanges[i].allocations[j].footnotes[h].name,
                                            isBand = true,
                                            AllocationId = Allocation.AllocationId
                                            //ovdje ne treba allocation
                                            //allocatio = reservation.allocationRanges[i].allocations[j].
                                        };
                                        var allIdCount = _context.FootnoteAllocation.Where(x => x.AllocationId == Allocation.AllocationId && x.id == footnoteAllocation.id).Count();
                                        if (allIdCount == 0)
                                        {
                                            //if(Allocation.AllocationId == 17)
                                            //{
                                            //    Debug.WriteLine("vrijednost allocation id:" + Allocation.AllocationId);
                                            //}
                                            _context.FootnoteAllocation.Add(footnoteAllocation);
                                            _context.SaveChanges();
                                        }
                                    }

                                }
                            }
                        }

                        //kraj
                    }



                    else
                    {
                        // Debug.WriteLine("user not ok:" + response.StatusCode);
                        
                        ViewBag.StatusCode = response.Content.ToString();
                    }

                }
            }
            return View("Index", reservation);
        }


        [HttpPost]
        public async Task<IActionResult> GetAllocations2()
        {
            // Debug.WriteLine("values:");
            Root reservation = new Root();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org/allocations/ranges?regionId=1"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Debug.WriteLine("user ok:"+ response.StatusCode);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<Root>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;


                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.allocationRanges[i].allocations.Count; j++)
                            {
                                AllocationTermDb allocationTermTemp = new AllocationTermDb();
                                allocationTermTemp = new AllocationTermDb()
                                {
                                    AllocationTermId = reservation.allocationRanges[i].allocations[j].allocationTerm.id,
                                    name = reservation.allocationRanges[i].allocations[j].allocationTerm.name
                                };

                                if (!_context.AllocationTermDb.Contains(allocationTermTemp))
                                {
                                    _context.AllocationTermDb.Add(allocationTermTemp);
                                    _context.SaveChanges();
                                }


                            }
                        }

                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            RootAllocationDB root = new RootAllocationDB()
                            {
                                regionId = reservation.regionId,
                                regionName = reservation.regionName,
                                regionCode = reservation.regionCode
                            };

                            int value = _context.RootAllocationDB.Where(x => x.regionId == root.regionId).Count();
                            if (value == 0)
                            {
                                _context.RootAllocationDB.Add(root);
                                _context.SaveChanges();
                            }
                        }


                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            RootAllocationDB valueRoot = _context.RootAllocationDB.Where(x => x.regionId == reservation.regionId).SingleOrDefault();
                            AllocationRangeDb all = new AllocationRangeDb()
                            {
                                low = reservation.allocationRanges[i].low,
                                high = reservation.allocationRanges[i].high,
                                RootAllocationDBId = valueRoot.RootAllocationDBId,
                                LowView = null,
                                HighView = null
                                
                            };
                            int value = _context.AllocationRangeDb.Where(x => x.low == all.low && x.high == all.high && x.RootAllocationDBId == all.RootAllocationDBId).Count();
                            if (value == 0)
                            {
                                // Debug.Write("vrijednost AllocationRangeDb:" + value);
                                _context.AllocationRangeDb.Add(all);
                                _context.SaveChanges();
                            }
                        }




                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.allocationRanges[i].allocations.Count; j++)
                            {
                                long low = reservation.allocationRanges[i].low;
                                long high = reservation.allocationRanges[i].high;
                                var temp = reservation.allocationRanges[i].allocations[j];
                               


                                int value = _context.AllocationRangeDb.Where(x => x.low == low && x.high == high && x.RootAllocationDBId == 2).Count();

                                if (value == 1)
                                {
                                    // Debug.Write("vrijednost:" + value);
                                    var allocationRange = _context.AllocationRangeDb.Where(x => x.low == low && x.high == high && x.RootAllocationDBId == 2).SingleOrDefault();
                                    // Debug.Write("vrijednost all:" + allocationRange.AllocationRangeId);
                                    AllocationTermDb tempAllTerm = null;
                                    if (reservation.allocationRanges[i].allocations[j].allocationTerm.id > 0)
                                    {
                                        //Debug.WriteLine("vrijednost allocationTerm id if uslov:" + reservation.allocationRanges[i].allocations[j].allocationTerm.id);
                                        tempAllTerm = new AllocationTermDb()
                                        {
                                            AllocationTermId = reservation.allocationRanges[i].allocations[j].allocationTerm.id,
                                            name = reservation.allocationRanges[i].allocations[j].allocationTerm.name
                                        };
                                    }
                                    // Debug.WriteLine("vrijednost allocationTerm id:" + reservation.allocationRanges[i].allocations[j].allocationTerm.id);

                                    AllocationDb allocation = new AllocationDb()
                                    {
                                        allocationRangeDb = allocationRange,
                                        primary = reservation.allocationRanges[i].allocations[j].primary,
                                        AllocationTermId = tempAllTerm.AllocationTermId

                                    };

                                    if (!_context.AllocationDb.Contains(allocation))
                                    {

                                        //Debug.Write("value");
                                        var AllocationCount = _context.AllocationDb.Where(y => y.AllocationRangeId == allocationRange.AllocationRangeId && y.AllocationTermId == reservation.allocationRanges[i].allocations[j].allocationTerm.id).Count();
                                        if (AllocationCount == 0)
                                        {
                                            //Debug.Write("radi upis");
                                            _context.AllocationDb.Add(allocation);
                                            _context.SaveChanges();
                                        }

                                    }
                                }
                            }

                        }

                        for (int i = 0; i < reservation.allocationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.allocationRanges[i].allocations.Count; j++)
                            {
                                for (int h = 0; h < reservation.allocationRanges[i].allocations[j].footnotes.Count; h++)
                                {
                                    if (reservation.allocationRanges[i].allocations[j].footnotes[h].id > 0)
                                    {
                                        var allocationRange = _context.AllocationRangeDb.Where(x => x.low == reservation.allocationRanges[i].low && x.high == reservation.allocationRanges[i].high && x.RootAllocationDBId == 2).SingleOrDefault();
                                        var Allocation = _context.AllocationDb.Where(y => y.AllocationRangeId == allocationRange.AllocationRangeId && y.AllocationTermId == reservation.allocationRanges[i].allocations[j].allocationTerm.id).SingleOrDefault();

                                        // Debug.WriteLine("vrijednost allocation id:" + Allocation.AllocationId);

                                        FootnoteAllocation footnoteAllocation = new FootnoteAllocation()
                                        {
                                            id = reservation.allocationRanges[i].allocations[j].footnotes[h].id,
                                            name = reservation.allocationRanges[i].allocations[j].footnotes[h].name,
                                            isBand = true,
                                            AllocationId = Allocation.AllocationId
                                            //ovdje ne treba allocation
                                            //allocatio = reservation.allocationRanges[i].allocations[j].
                                        };
                                        var allIdCount = _context.FootnoteAllocation.Where(x => x.AllocationId == Allocation.AllocationId && x.id == footnoteAllocation.id).Count();
                                        if (allIdCount == 0)
                                        {
                                            //if(Allocation.AllocationId == 17)
                                            //{
                                            //    Debug.WriteLine("vrijednost allocation id:" + Allocation.AllocationId);
                                            //}
                                            _context.FootnoteAllocation.Add(footnoteAllocation);
                                            _context.SaveChanges();
                                        }
                                    }

                                }
                            }
                        }

                        //kraj
                    }



                    else
                    {
                       

                        ViewBag.StatusCode = response.Content.ToString();
                    }

                }
            }
            return View("Index", reservation);
        }


        [HttpPost]
        public async Task<IActionResult> GetFootnotes()
        {
            RootF reservation = new RootF();
            Root tempRoot = new Root();


            //var webRequest = WebRequest.Create("https://testapi.cept.org/footnotes/ranges?regionId=2") as HttpWebRequest;


            //webRequest.ContentType = "application/json";
            //webRequest.UserAgent = "Nothing";
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            //if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            //{
            //    ViewBag.StatusCode = myHttpWebResponse.StatusCode;

            //    //using (var s = myHttpWebResponse.GetResponseStream())
            //    //{
            //    //    using (var sr = new StreamReader(s))
            //    //    {
            //    //        var contributorsAsJson = sr.ReadToEnd();
            //    //        var contributors = JsonConvert.DeserializeObject<List<RootF>>(contributorsAsJson);
            //    //        // contributors.ForEach(Console.WriteLine);
            //    //    }
            //    //}
            //}
            //else
            //{
            //    ViewBag.StatusCode = myHttpWebResponse.StatusCode;
            //}

            


            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org/footnotes/ranges?regionId=2"))
                {
                    //HttpClient client = new HttpClient();

                    //client.BaseAddress = new Uri("https://testapi.cept.org/");
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //var response = client.GetAsync("footnotes/ranges?regionId=2").Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<RootF>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;
                        for (int i = 0; i < reservation.footnoteRanges.Count; i++)
                        {

                            var query = from root in _context.RootAllocationDB
                                        from AllocationRangeDb in root.allocationRanges
                                        from AllocationDb in AllocationRangeDb.allocationsDb
                                        from FootnoteAllocation in AllocationDb.footnotes
                                        where AllocationRangeDb.low == reservation.footnoteRanges[i].frequencyMin &&
                                        AllocationRangeDb.high == reservation.footnoteRanges[i].frequencyMax
                                        select new
                                        {
                                            AllocationDb.AllocationId

                                        };
                            var value = query.FirstOrDefault();
                            if (value != null)
                            {
                                string famount = value.ToString();

                                // Debug.WriteLine("broj:" + famount);
                                for (int j = 0; j < reservation.footnoteRanges[i].footnotes.Count; j++)
                                {
                                    //Debug.WriteLine("foot id:" + reservation.footnoteRanges[i].footnotes[j].id);
                                    //Debug.WriteLine("allocation id:" + valueAll.AllocationId);
                                    FootnoteAllocation footNote = new FootnoteAllocation()
                                    {
                                        id = reservation.footnoteRanges[i].footnotes[j].id,
                                        name = reservation.footnoteRanges[i].footnotes[j].name,
                                        isBand = false,
                                        AllocationId = value.AllocationId
                                    };
                                    var allIdCount = _context.FootnoteAllocation.Where(x => x.AllocationId == value.AllocationId && x.id == footNote.id).Count();
                                    if (allIdCount == 0)
                                    {
                                        // Debug.WriteLine("allocation id:" + value.AllocationId);
                                        _context.FootnoteAllocation.Add(footNote);
                                        _context.SaveChanges();
                                    }

                                }
                            }
                            else
                            {

                                var queryAll = from root in _context.RootAllocationDB
                                               from AllocationRangeDb in root.allocationRanges
                                               from AllocationDb in AllocationRangeDb.allocationsDb
                                               where AllocationRangeDb.low == reservation.footnoteRanges[i].frequencyMin &&
                                               AllocationRangeDb.high == reservation.footnoteRanges[i].frequencyMax
                                               select new
                                               {
                                                   AllocationDb.AllocationId

                                               };
                                var valueAll = queryAll.FirstOrDefault();
                                for (int j = 0; j < reservation.footnoteRanges[i].footnotes.Count; j++)
                                {
                                    //Debug.WriteLine("foot id:" + reservation.footnoteRanges[i].footnotes[j].id);
                                    //Debug.WriteLine("allocation id:" + valueAll.AllocationId);
                                    FootnoteAllocation footNote = new FootnoteAllocation()
                                    {
                                        id = reservation.footnoteRanges[i].footnotes[j].id,
                                        name = reservation.footnoteRanges[i].footnotes[j].name,
                                        isBand = false,
                                        AllocationId = valueAll.AllocationId
                                    };
                                    var allIdCount = _context.FootnoteAllocation.Where(x => x.AllocationId == valueAll.AllocationId && x.id == footNote.id).Count();
                                    if (allIdCount == 0)
                                    {
                                        // Debug.WriteLine("allocation id:" + valueAll.AllocationId);
                                        _context.FootnoteAllocation.Add(footNote);
                                        _context.SaveChanges();
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.StatusCode = response.StatusCode;
                    }
                }
            }
            return View("Index", tempRoot);
        }

        [HttpPost]
        public async Task<IActionResult> GetFootnotes2()
        {
            RootF reservation = new RootF();
            Root tempRoot = new Root();


            //var webRequest = WebRequest.Create("https://testapi.cept.org/footnotes/ranges?regionId=1") as HttpWebRequest;


            //webRequest.ContentType = "application/json";
            //webRequest.UserAgent = "Nothing";
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            //if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            //{
            //    ViewBag.StatusCode = myHttpWebResponse.StatusCode;

            //    //using (var s = myHttpWebResponse.GetResponseStream())
            //    //{
            //    //    using (var sr = new StreamReader(s))
            //    //    {
            //    //        var contributorsAsJson = sr.ReadToEnd();
            //    //        var contributors = JsonConvert.DeserializeObject<List<RootF>>(contributorsAsJson);
            //    //        // contributors.ForEach(Console.WriteLine);
            //    //    }
            //    //}
            //}
            //else
            //{
            //    ViewBag.StatusCode = myHttpWebResponse.StatusCode;
            //}




            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org/footnotes/ranges?regionId=1"))
                {
                    //HttpClient client = new HttpClient();

                    //client.BaseAddress = new Uri("https://testapi.cept.org/");
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //var response = client.GetAsync("footnotes/ranges?regionId=2").Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<RootF>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;
                        for (int i = 0; i < reservation.footnoteRanges.Count; i++)
                        {

                            var query = from root in _context.RootAllocationDB
                                        from AllocationRangeDb in root.allocationRanges
                                        from AllocationDb in AllocationRangeDb.allocationsDb
                                        from FootnoteAllocation in AllocationDb.footnotes
                                        where AllocationRangeDb.low == reservation.footnoteRanges[i].frequencyMin &&
                                        AllocationRangeDb.high == reservation.footnoteRanges[i].frequencyMax
                                        && root.regionId == 1
                                        select new
                                        {
                                            AllocationDb.AllocationId

                                        };
                            var value = query.FirstOrDefault();
                            if (value != null)
                            {
                                string famount = value.ToString();

                                // Debug.WriteLine("broj:" + value.AllocationId);
                                for (int j = 0; j < reservation.footnoteRanges[i].footnotes.Count; j++)
                                {
                                    //Debug.WriteLine("foot id:" + reservation.footnoteRanges[i].footnotes[j].id);
                                    //Debug.WriteLine("allocation id:" + valueAll.AllocationId);
                                    FootnoteAllocation footNote = new FootnoteAllocation()
                                    {
                                        id = reservation.footnoteRanges[i].footnotes[j].id,
                                        name = reservation.footnoteRanges[i].footnotes[j].name,
                                        isBand = false,
                                        AllocationId = value.AllocationId
                                    };
                                    var allIdCount = _context.FootnoteAllocation.Where(x => x.AllocationId == value.AllocationId && x.id == footNote.id).Count();
                                    if (allIdCount == 0)
                                    {
                                        // Debug.WriteLine("allocation id:" + value.AllocationId);
                                        _context.FootnoteAllocation.Add(footNote);
                                        _context.SaveChanges();
                                    }

                                }
                            }
                            else
                            {

                                var queryAll = from root in _context.RootAllocationDB
                                               from AllocationRangeDb in root.allocationRanges
                                               from AllocationDb in AllocationRangeDb.allocationsDb
                                               where AllocationRangeDb.low == reservation.footnoteRanges[i].frequencyMin &&
                                               AllocationRangeDb.high == reservation.footnoteRanges[i].frequencyMax
                                               && root.regionId == 1
                                               select new
                                               {
                                                   AllocationDb.AllocationId

                                               };
                                var valueAll = queryAll.FirstOrDefault();
                                for (int j = 0; j < reservation.footnoteRanges[i].footnotes.Count; j++)
                                {
                                    //Debug.WriteLine("foot id:" + reservation.footnoteRanges[i].footnotes[j].id);
                                    //Debug.WriteLine("allocation id:" + valueAll.AllocationId);
                                    FootnoteAllocation footNote = new FootnoteAllocation()
                                    {
                                        id = reservation.footnoteRanges[i].footnotes[j].id,
                                        name = reservation.footnoteRanges[i].footnotes[j].name,
                                        isBand = false,
                                        AllocationId = valueAll.AllocationId
                                    };
                                    var allIdCount = _context.FootnoteAllocation.Where(x => x.AllocationId == valueAll.AllocationId && x.id == footNote.id).Count();
                                    if (allIdCount == 0)
                                    {
                                        // Debug.WriteLine("allocation id:" + valueAll.AllocationId);
                                        _context.FootnoteAllocation.Add(footNote);
                                        _context.SaveChanges();
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.StatusCode = response.StatusCode;
                    }
                }
            }
            return View("Index", tempRoot);
        }


        [HttpPost]
        public async Task<IActionResult> GetApplicationTerms()
        {
            RootApplicationTerms reservation = new RootApplicationTerms();
            Root tempRoot = new Root();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org/applications/terms"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<RootApplicationTerms>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;

                        for (int i = 0; i < reservation.terms.Count; i++)
                        {
                            ApplicationTermsDB appTermTemp = new ApplicationTermsDB();
                            appTermTemp = new ApplicationTermsDB()
                            {
                                ApplicationTermsDBId = reservation.terms[i].id,
                                name = reservation.terms[i].name
                            };

                            if (!_conAppTerms.RootApplicationTermsDB.Contains(appTermTemp))
                            {
                                _conAppTerms.RootApplicationTermsDB.Add(appTermTemp);
                                _conAppTerms.SaveChanges();
                            }
                        }
                    }
                }
            }
            return View("Index", tempRoot);
        }


        [HttpPost]
        public async Task<IActionResult> GetAllocationTerms()
        {
            RootAllocationTerms reservation = new RootAllocationTerms();
            Root tempRoot = new Root();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org//allocations/terms"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<RootAllocationTerms>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;

                        for (int i = 0; i < reservation.terms.Count; i++)
                        {
                            FrekvencijeProject.Models.AllocationTerms.AllocationTermDb appTermTemp =
                                new FrekvencijeProject.Models.AllocationTerms.AllocationTermDb()

                                {
                                    AllocationTermId = reservation.terms[i].id,
                                    name = reservation.terms[i].name
                                };

                            if (!_conAllTerms.AllocationTermDb.Contains(appTermTemp))
                            {
                                _conAllTerms.AllocationTermDb.Add(appTermTemp);
                                _conAllTerms.SaveChanges();
                            }
                        }
                    }
                }
            }

            return View("Index", tempRoot);
        }


        [HttpPost]
        public async Task<IActionResult> GetApplication()
        {
            RootApplication reservation = new RootApplication();
            Root tempRoot = new Root();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://testapi.cept.org/applications/ranges?regionId=1"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<RootApplication>(apiResponse);
                        ViewBag.StatusCode = response.StatusCode;
                        RootApplicationDB root = new RootApplicationDB();
                        root.regionId = reservation.regionId;
                        root.regionName = reservation.regionName;
                        root.regionCode = reservation.regionCode;
                        if (!_conAppContext.RootApplicationDB.Contains(root))
                        {
                            _conAppContext.RootApplicationDB.Add(root);
                            _conAppContext.SaveChanges();
                        }

                        for (int i = 0; i < reservation.applicationRanges.Count; i++)
                        {
                            RootApplicationDB valueRoot = _conAppContext.RootApplicationDB.Where(x => x.regionId == reservation.regionId).SingleOrDefault();
                            ApplicationRangeDB appRange = new ApplicationRangeDB()
                            {
                                low = reservation.applicationRanges[i].low,
                                high = reservation.applicationRanges[i].high,
                                RootApplicationDBId = valueRoot.RootApplicationDBId

                            };

                            if (!_conAppContext.ApplicationRange.Contains(appRange))
                            {
                                _conAppContext.ApplicationRange.Add(appRange);
                                _conAppContext.SaveChanges();
                            }

                        }

                        for (int i = 0; i < reservation.applicationRanges.Count; i++)
                        {
                            for (int j = 0; j < reservation.applicationRanges[i].applications.Count; j++)
                            {

                                ApplicationRangeDB valueRange = _conAppContext.ApplicationRange.Where(x => x.low == reservation.applicationRanges[i].low
                                && x.high == reservation.applicationRanges[i].high).SingleOrDefault();

                                ApplicationDB app = new ApplicationDB()
                                {

                                    ApplicationRangeId = valueRange.ApplicationRangeId,
                                    ApplicationTermId = reservation.applicationRanges[i].applications[j].applicationTerm.id,
                                    comment = reservation.applicationRanges[i].applications[j].comment
                                };

                                if (!_conAppContext.Application.Contains(app))
                                {
                                    _conAppContext.Application.Add(app);
                                    _conAppContext.SaveChanges();
                                }
                            }
                            
                        }

                    }
                }
            }

            return View("Index", tempRoot);
        }

        [HttpPost]
        public IActionResult AddFootnoteDesc()
        {
            Root tempRoot = new Root();
            string FileName = @"Files\\DB_footnotes.xlsx";
            //var workbook = new GrapeCity.Documents.Excel.Workbook();
            //workbook.Open(@"Files\\DB_footnotes.xlsx");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    while (reader.Read()) //Each row of the file
                    {
                        //
                        //Debug.WriteLine("evo me:"+reader.GetValue(0).ToString());
                        //Debug.WriteLine("druga:" + reader.GetValue(1).ToString());
                        if (reader.GetValue(0) != null)
                        {

                            Footnote_description footDesc = new Footnote_description();
                            footDesc.name = reader.GetValue(0).ToString();
                            footDesc.text_desc = reader.GetValue(1).ToString();
                            if (reader.GetValue(2) != null)
                            {
                                footDesc.type = reader.GetValue(2).ToString();
                            }
                            else
                            {
                                footDesc.type = "";
                            }

                            //Debug.WriteLine("trece:" + reader.GetValue(2).ToString());

                            if (reader.GetValue(3) != null)
                            {
                                footDesc.relevance = true;
                            }
                            else
                            {
                                footDesc.relevance = false;
                            }
                            if (!_context.Footnote_description.Contains(footDesc))
                            {
                                Debug.WriteLine("ww");
                                _context.Footnote_description.Add(footDesc);
                                _context.SaveChanges();
                            }
                            else
                            {
                                //Debug.WriteLine("zadnje:");
                            }
                        }
                        //Debug.WriteLine("zadnje:" + reader.GetValue(3).ToString());

                    }
                }
            }


                    
                    return View("Index", tempRoot);
        }
    }
}
