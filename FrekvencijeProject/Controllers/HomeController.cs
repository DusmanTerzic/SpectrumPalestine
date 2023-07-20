using FrekvencijeProject.Areas.Identity.Data;
using FrekvencijeProject.JSON.Allocations;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Ajax;
using FrekvencijeProject.Models.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;


namespace FrekvencijeProject.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsersDBContext _context;
        UserManager<ApplicationUser> _UserManager;
        object user;
        private readonly ApplicationDBContext _conApp;
        private readonly AllocationDBContext _conAll;

        [Display(Name = "Search by")]
        public string General { get; set; }

        public string GeneralValue { get; set; }

        public List<SelectListItem> GeneralsList = new List<SelectListItem>
        {
            new SelectListItem { Text = "General", Value = "1" ,Selected = true },
            new SelectListItem { Text = "Allocations", Value = "2" },
            new SelectListItem { Text = "Applications", Value = "3" },
            new SelectListItem { Text = "Documents", Value = "4" },
            new SelectListItem { Text = "Interfaces", Value = "5" },
            new SelectListItem { Text = "Right of use", Value = "6" }
        };

        [Display(Name = "Frequency")]
        public string Frequency { get; set; }

        [Display(Name = "From")]
        public string FromText { get; set; }
        public string From { get; set; }

        [Display(Name = "To")]
        public string ToText { get; set; }

        public string To { get; set; }
        
        public string FrequencySizeValue { get; set; }

        public List<SelectListItem> FrequencySizesList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Hz", Value = "1" },
            new SelectListItem { Text = "kHz", Value = "2" },
            new SelectListItem { Text = "MHz", Value = "3",Selected = true},
            new SelectListItem { Text = "GHz", Value = "4" }
        };

        [Display(Name = "Frequency Table")]
        public string FrequencyTable { get; set; }


        public string FrequencytableValue { get; set; }

        public List<SelectListItem> FrequencyTablesList = new List<SelectListItem>
        {
            //new SelectListItem { Text = "--Arab States (ACA)--", Value = "1" },
            new SelectListItem { Text = "--Palestine (PSE)--", Value = "1" },
            new SelectListItem { Text = "- ITU (Region 1) -", Value = "2" },
            new SelectListItem { Text = "Algeria", Value = "3" },
            new SelectListItem { Text = "Bahrain", Value = "4" },
            new SelectListItem { Text = "Comoros", Value = "5" },
            new SelectListItem { Text = "Djibouti", Value = "6" },
            new SelectListItem { Text = "Egypt", Value = "7" },
            new SelectListItem { Text = "Iraq", Value = "8" },
            new SelectListItem { Text = "Jordan", Value = "9" },
            new SelectListItem { Text = "Kuwait", Value = "10" },
            new SelectListItem { Text = "Lebanon", Value = "11" },
            new SelectListItem { Text = "Libya", Value = "12" },
            new SelectListItem { Text = "Mauritania", Value = "13" },
            new SelectListItem { Text = "Morocco", Value = "14" },
            new SelectListItem { Text = "Oman", Value = "15" },
            //new SelectListItem { Text = "Palestine", Value = "16",Selected = true },
            new SelectListItem { Text = "Qatar", Value = "16" },
            new SelectListItem { Text = "Saudi Arabia", Value = "17" },
            new SelectListItem { Text = "Somalia", Value = "18" },
            new SelectListItem { Text = "Sudan", Value = "19" },
            new SelectListItem { Text = "Syria", Value = "20" },
            new SelectListItem { Text = "Tunisia", Value = "21" },
            new SelectListItem { Text = "United Arab Emirates", Value = "22" },
            new SelectListItem { Text = "Yemen", Value = "23" },
           
        };

        public HomeController(ILogger<HomeController> logger, UsersDBContext context, UserManager<ApplicationUser> UserManager
            , IHttpContextAccessor httpContextAccessor, ApplicationDBContext conApp, AllocationDBContext conAll)
        {
            _logger = logger;
            _context = context;
            _UserManager = UserManager;
            user = httpContextAccessor.HttpContext.User;
            _conApp = conApp;
            _conAll = conAll;
        }


        [HttpPost]
        public IActionResult CultureManagment(string culture,string returnURl)
        {
            //Debug.WriteLine("test:" + culture);
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(30)
                }
                );
            //Debug.WriteLine("test:" + returnURl);
            // return RedirectToAction(nameof(Index));
            return LocalRedirect(returnURl);
        }
        //on loading the start page check the logged user.
        public IActionResult Index()
        {
            ///GetSelectListItems
            //Debug.WriteLine("test se poziva dva puta :");
            var selected = FrequencySizesList.Where(x => x.Text == "MHz").First();
            selected.Selected = true;
            var entryPoint = (from ep in _context.AspNetUsers
                              join e in _context.AspNetUserRoles on ep.Id equals e.UserId
                              join t in _context.AspNetRoles on e.RoleId equals t.Id
                              where t.Name == "Administrator" && ep.UserName == _UserManager.GetUserName((System.Security.Claims.ClaimsPrincipal)user)
                              select
                                  t.Name

                             ).FirstOrDefault();
            if (entryPoint != null)
            {
                return View("~/Views/AdministrationPanel/Index.cshtml");
            }
            else
            {
                return View();
            }
        }
        //check is logged user is administrator this method is called from the view.
        public string GetUserAdmin()
        {
           // Debug.WriteLine("user :"+ _UserManager.GetUserName((System.Security.Claims.ClaimsPrincipal)user));
            string value = "";
            
            var entryPoint = (from ep in _context.AspNetUsers
                              join e in _context.AspNetUserRoles on ep.Id equals e.UserId
                              join t in _context.AspNetRoles on e.RoleId equals t.Id
                              where t.Name == "Administrator" && ep.UserName == _UserManager.GetUserName((System.Security.Claims.ClaimsPrincipal) user)
                              select 
                                  t.Name
                              
                              ).FirstOrDefault();
            if (entryPoint != null)
            {
                //Debug.WriteLine("value :" + entryPoint+"=korisnik:"+ _UserManager.GetUserName((System.Security.Claims.ClaimsPrincipal)user));
                return entryPoint;
            }
            else
            {
                return "";
            }
        }
        
        

        

        [HttpPost]
        //using the jquey of search on first level.
        public IActionResult GeneralSearchNew(string GeneralValue, string From, string To, int FrequencySizeValue, string[] FrequencytableValue)
        {

            if(FrequencytableValue.Length == 0 && GeneralValue == "1")
            {
                return View("Index");
            }
            Debug.WriteLine("pozvao:"+ GeneralValue + ","+ From+","+ To+","+ FrequencySizeValue+","+ FrequencytableValue.Length);
            this.GeneralValue = "" + GeneralValue;

            string tempValue = GeneralsList.Where(p => p.Value.Equals(this.GeneralValue)).First().Text;
            this.From = From;
            this.To = To;
            this.FrequencySizeValue =""+  FrequencySizeValue;
            this.FrequencytableValue =""+  FrequencytableValue;
            long tempFrom = 0;
            long tempTo = 0;
           
            if (From != null)
            {
                tempFrom = long.Parse(From);
                From = "" + tempFrom;
            }
            if (To != null)
            {
                tempTo = long.Parse(To);
                To = "" + tempTo;
            }

            if (this.FrequencySizeValue == "2")
            {
                if (From != null)
                {
                    var s = string.Concat(From, "000");

                    long value = long.Parse(s);
                    tempFrom = value;
                    From = "" + tempFrom;
                }

                if (To != null)
                {
                    var s = string.Concat(To, "000");


                    long valueTo = long.Parse(s);
                    // Debug.WriteLine("value:" + valueTo);
                    tempTo = valueTo;
                    // Debug.WriteLine("value 2:" + tempTo);
                    To = "" + tempTo;
                }
                
            }
            else if (this.FrequencySizeValue == "3")
            {
                if (From != null)
                {
                    var s = string.Concat(From, "000000");

                    long value = long.Parse(s);
                    tempFrom = value;

                    From = "" + tempFrom;
                }

                if (To != null)
                {
                    var s = string.Concat(To, "000000");

                    long value = long.Parse(s);
                    tempTo = value;
                    To = "" + tempTo;
                }
                
            }
            else if (this.FrequencySizeValue == "4")
            {
                if (From != null)
                {
                    var s = string.Concat(From, "000000000");

                    long value = long.Parse(s);
                    tempFrom = value;

                    From = "" + tempFrom;
                }

                if (To != null)
                {
                    var s = string.Concat(To, "000000000");

                    long value = long.Parse(s);
                    tempTo = value;
                    To = "" + tempTo;
                }
              
            }

            // Debug.WriteLine("gene last:" + GeneralValue + "," + From + "," + To + "," + FrequencySizeValue + "," + FrequencytableValue);
            if (GeneralValue != "")
            {
                //according the selected value from meny search by change the page. 
                if (GeneralValue == "1")
                {
                    //Debug.WriteLine("gene last:" + GeneralValue);
                    if (tempTo == 0)
                    {
                        //return the highest record 
                        var BestproductSale = _conAll.AllocationRangeDb.ToList()
                       .GroupBy(x => x.high)
                       .Select(grp => new
                       {
                           High = grp.ToList().FirstOrDefault().high

                       })
                       .OrderByDescending(x => x.High).ToList();

                        tempTo = BestproductSale[0].High;
                        To = "" + tempTo;

                        var HighView = (from all in _conAll.AllocationRangeDb
                                        where all.high == BestproductSale[0].High
                                        select new
                                        {
                                            all.HighView
                                        }
                            ).FirstOrDefault();

                        if (HighView.HighView.Contains("GHz"))
                        {
                            FrequencySizeValue = 4;
                            this.FrequencySizeValue = "" + FrequencySizeValue;
                        }
                        else if (HighView.HighView.Contains("MHz"))
                        {
                            FrequencySizeValue = 3;
                            this.FrequencySizeValue = "" + FrequencySizeValue;
                        }
                        else if (HighView.HighView.Contains("kHz"))
                        {
                            FrequencySizeValue = 2;
                            this.FrequencySizeValue = "" + FrequencySizeValue;
                        }
                        else if (HighView.HighView.Contains("Hz"))
                        {
                            FrequencySizeValue = 1;
                            this.FrequencySizeValue = "" + FrequencySizeValue;
                        }
                    }


                        TempData["general"] = true;
                        TempData["tempFrom"] = ""+tempFrom;
                        TempData["tempTo"] = ""+tempTo;
                        TempData["FrequencytableValue"] = FrequencytableValue[0];
                        TempData["FrequencySize"] = this.FrequencySizeValue;
                   
                    return Redirect("~/FreqBand/Index");
                   

                }
                else if (GeneralValue == "2")
                {

                    TempData["al"] = true;
                    //TempData["pp"] = "proba";
                    //return RedirectToAction("Index", "AllocationSearch");
                    if (tempFrom == 0 && tempTo == 0)
                    {
                        var BestproductSale = _conAll.AllocationRangeDb.ToList()
                       .GroupBy(x => x.high)
                       .Select(grp => new
                       {
                           High = grp.ToList().FirstOrDefault().high
                           
                       })
                       .OrderByDescending(x => x.High).ToList();
                        
                            tempTo = BestproductSale[0].High;
                            To = "" + tempTo;

                        var HighView = (from all in _conAll.AllocationRangeDb
                                        where all.high == BestproductSale[0].High
                                        select new
                                        {
                                            all.HighView
                                        }
                            ).FirstOrDefault();

                        if (HighView.HighView.Contains("GHz"))
                        {
                            FrequencySizeValue = 4;
                        }
                        else if (HighView.HighView.Contains("MHz"))
                        {
                            FrequencySizeValue = 3;
                        }else if (HighView.HighView.Contains("kHz"))
                        {
                            FrequencySizeValue = 2;
                        }
                        else if (HighView.HighView.Contains("Hz"))
                        {
                            FrequencySizeValue = 1;
                        }
                        


                        TempData["tempFrom"] = ""+tempFrom;
                        TempData["tempTo"] =""+ tempTo;
                        IEnumerable<string> values = FrequencytableValue.ToList();
                        TempData["FrequencytableValue"] = values;
                        //Debug.WriteLine("test:" + (IEnumerable<string>)TempData["FrequencytableValue"]);
                        TempData["FrequencySize"] = "" + FrequencySizeValue;
                        return Redirect("~/AllocationSearch/Index");
                        
                    }
                    else if (tempFrom == 0 && tempTo != 0)
                    {

                        TempData["tempFrom"] = "" + tempFrom;
                        TempData["tempTo"] = "" + tempTo;
                        IEnumerable<string> values = FrequencytableValue.ToList();
                        TempData["FrequencytableValue"] = values;
                        TempData["FrequencySize"] = "" +FrequencySizeValue;
                        return Redirect("~/AllocationSearch/Index");
                    }
                    else if (tempFrom != 0 && tempTo != 0)
                    {
                        //Debug.WriteLine("pp");
                        TempData["tempFrom"] = "" + tempFrom;
                        TempData["tempTo"] = "" + tempTo;
                        IEnumerable<string> values = FrequencytableValue.ToList();
                        TempData["FrequencytableValue"] = values;
                        TempData["FrequencySize"] = "" + FrequencySizeValue;
                        return Redirect("~/AllocationSearch/Index");
                        
                    }

                }
                else if (GeneralValue == "3")
                {
                    if (From != "" && To != "")
                    {
                        if (this.FrequencySizeValue != "0")
                        {
                            if (tempTo == 0)
                            {
                                //return the highest record 
                                var BestproductSale = _conApp.ApplicationRange.ToList()
                               .GroupBy(x => x.high)
                               .Select(grp => new
                               {
                                   High = grp.ToList().FirstOrDefault().high

                               })
                               .OrderByDescending(x => x.High).ToList();

                                tempTo = BestproductSale[0].High;

                                var HighView = (from all in _conApp.ApplicationRange
                                                where all.high == BestproductSale[0].High
                                                select new
                                                {
                                                    all.HighView
                                                }
                            ).FirstOrDefault();

                            

                            if (HighView.HighView.Contains("GHz"))
                            {
                                FrequencySizeValue = 4;
                            }
                            else if (HighView.HighView.Contains("MHz"))
                            {
                                FrequencySizeValue = 3;
                            }
                            else if (HighView.HighView.Contains("kHz"))
                            {
                                FrequencySizeValue = 2;
                            }
                            else if (HighView.HighView.Contains("Hz"))
                            {
                                FrequencySizeValue = 1;
                            }
                        }

                            TempData["app"] = true;
                            TempData["tempFrom"] = "" + tempFrom;
                            TempData["tempTo"] = "" + tempTo;
                            TempData["FrequencytableValue"] = FrequencytableValue[0];
                            TempData["FrequencySize"] = "" + FrequencySizeValue;
                            return Redirect("~/ApplicationSearch/Index");
                            
                        }
                    }

                }
                else if (GeneralValue == "4")
                {
                    if (From != "" && To != "")
                    {
                        if (this.FrequencySizeValue != "0")
                        {
                            if (tempTo == 0)
                            {
                                //return the highest record 
                                var BestproductSale = _conApp.ApplicationRange.ToList()
                               .GroupBy(x => x.high)
                               .Select(grp => new
                               {
                                   High = grp.ToList().FirstOrDefault().high

                               })
                               .OrderByDescending(x => x.High).ToList();

                                tempTo = BestproductSale[0].High;

                                var HighView = (from all in _conApp.ApplicationRange
                                                where all.high == BestproductSale[0].High
                                                select new
                                                {
                                                    all.HighView
                                                }
                            ).FirstOrDefault();

                                if (HighView.HighView.Contains("GHz"))
                                {
                                    FrequencySizeValue = 4;
                                }
                                else if (HighView.HighView.Contains("MHz"))
                                {
                                    FrequencySizeValue = 3;
                                }
                                else if (HighView.HighView.Contains("kHz"))
                                {
                                    FrequencySizeValue = 2;
                                }
                                else if (HighView.HighView.Contains("Hz"))
                                {
                                    FrequencySizeValue = 1;
                                }

                            }
                            TempData["doc"] = true;
                            TempData["tempFrom"] = "" + tempFrom;
                            TempData["tempTo"] = "" + tempTo;
                            TempData["FrequencytableValue"] = FrequencytableValue[0];
                            TempData["FrequencySize"] = "" + FrequencySizeValue;
                            return Redirect("~/Document/Index");

                        }
                    }

                }else if(GeneralValue == "5")
                {
                    if (From != "" && To != "")
                    {
                        if (this.FrequencySizeValue != "0")
                        {
                            if (tempTo == 0)
                            {
                                //return the highest record 
                                var BestproductSale = _conApp.ApplicationRange.ToList()
                               .GroupBy(x => x.high)
                               .Select(grp => new
                               {
                                   High = grp.ToList().FirstOrDefault().high

                               })
                               .OrderByDescending(x => x.High).ToList();

                                tempTo = BestproductSale[0].High;

                                var HighView = (from all in _conApp.ApplicationRange
                                                where all.high == BestproductSale[0].High
                                                select new
                                                {
                                                    all.HighView
                                                }
                            ).FirstOrDefault();

                                if (HighView.HighView.Contains("GHz"))
                                {
                                    FrequencySizeValue = 4;
                                }
                                else if (HighView.HighView.Contains("MHz"))
                                {
                                    FrequencySizeValue = 3;
                                }
                                else if (HighView.HighView.Contains("kHz"))
                                {
                                    FrequencySizeValue = 2;
                                }
                                else if (HighView.HighView.Contains("Hz"))
                                {
                                    FrequencySizeValue = 1;
                                }

                            }
                            TempData["inter"] = true;
                            TempData["tempFrom"] = "" + tempFrom;
                            TempData["tempTo"] = "" + tempTo;
                            TempData["FrequencytableValue"] = FrequencytableValue[0];
                            TempData["FrequencySize"] = "" + FrequencySizeValue;
                            return Redirect("~/Interfaces/Index");

                        }
                    }
                }
                else if (GeneralValue == "6")
                {
                    if (From != "" && To != "")
                    {
                        if (this.FrequencySizeValue != "0")
                        {
                            if (tempTo == 0)
                            {
                                //return the highest record 
                                var BestproductSale = _conApp.ApplicationRange.ToList()
                               .GroupBy(x => x.high)
                               .Select(grp => new
                               {
                                   High = grp.ToList().FirstOrDefault().high

                               })
                               .OrderByDescending(x => x.High).ToList();

                                tempTo = BestproductSale[0].High;

                                var HighView = (from all in _conApp.ApplicationRange
                                                where all.high == BestproductSale[0].High
                                                select new
                                                {
                                                    all.HighView
                                                }
                            ).FirstOrDefault();

                                if (HighView.HighView.Contains("GHz"))
                                {
                                    FrequencySizeValue = 4;
                                }
                                else if (HighView.HighView.Contains("MHz"))
                                {
                                    FrequencySizeValue = 3;
                                }
                                else if (HighView.HighView.Contains("kHz"))
                                {
                                    FrequencySizeValue = 2;
                                }
                                else if (HighView.HighView.Contains("Hz"))
                                {
                                    FrequencySizeValue = 1;
                                }

                            }
                            TempData["rightOfUse"] = true;
                            TempData["tempFrom"] = "" + tempFrom;
                            TempData["tempTo"] = "" + tempTo;
                            TempData["FrequencytableValue"] = FrequencytableValue[0];
                            TempData["FrequencySize"] = "" + FrequencySizeValue;
                            return Redirect("~/RightOfUse/Index");

                        }
                    }
                }

            }
            else
            {
                // Debug.WriteLine("proba");
                
            }

            return Json("");
        }


        //private static IEnumerable<T> CreateEmptyEnumerable(IEnumerable<T> query1)
        //{
        //    return Enumerable.Empty<T>();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}
