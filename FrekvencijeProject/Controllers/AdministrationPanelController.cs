
using ExcelDataReader;
using FrekvencijeProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers
{
    public class AdministrationPanelController : Controller
    {
        private readonly StandardsDbContext _context;
        private IConfiguration configuration;
        public IActionResult Index()
        {
            return View();
        }

        public AdministrationPanelController(StandardsDbContext context, IConfiguration conf)
        {
            _context = context;
            configuration = conf;
        }

        


        [HttpPost]
        public IActionResult UploadFiles()
        {
            return RedirectToAction("Index",
                        "UploadExcel");
            
        }
        }
}
