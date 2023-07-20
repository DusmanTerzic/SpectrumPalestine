using ExcelDataReader;
using FrekvencijeProject.Controllers.UploadFIles;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Document;
using FrekvencijeProject.Models.Interfaces;
using FrekvencijeProject.Models.RightOfUse;
using FrekvencijeProject.Models.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FrekvencijeProject.Controllers
{
    public class UploadExcelController : Controller
    {
        private IConfiguration configuration;

        private readonly ImportTempTableContext _conImport;
        private readonly AllocationDBContext _conAll;
        private readonly ApplicationDBContext _conApp;
        private readonly Tracking_tracing_data_acqDBContext _conTracking;
        private readonly ImportTempInterfacesDBContext _conInterfaces;
        private readonly ImportTempRightOfUseDBContext _conRightOfUse;
        public IActionResult Index()
        {
            return View();
        }

        public UploadExcelController(IConfiguration conf, ImportTempTableContext conImport, AllocationDBContext conAll, ApplicationDBContext conApp, Tracking_tracing_data_acqDBContext conTrack,
            ImportTempInterfacesDBContext conInterfaces, ImportTempRightOfUseDBContext conRightOfUse)
        {
            configuration = conf;
            _conImport = conImport;
            _conAll = conAll;
            _conApp = conApp;
            _conTracking = conTrack;
            _conInterfaces = conInterfaces;
            _conRightOfUse = conRightOfUse;
        }


        [HttpPost]
        public async Task<IActionResult> UploadFileCustomDocuments(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return View("Index");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                //Debug.WriteLine("testiing i have entered");
                await file.CopyToAsync(stream);
            }
            //Debug.WriteLine("wwww:" + path);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string conString = this.configuration.GetConnectionString("ExcelConString");
            DataTable dt = new DataTable();
            conString = string.Format(conString, path);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;
            //            //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //            //do not change TABLE_NAME it has to be default
                        string sheetName = dtExcelSchema.Rows[1]["TABLE_NAME"].ToString();
                        connExcel.Close();
            //            //Debug.WriteLine("name:" + sheetName);
            //            //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }

            conString = this.configuration.GetConnectionString("AuthDBContextConnection");
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
            //        //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.StandardsDbCopy";

            //        //[OPTIONAL]: Map the Excel columns with that of the database table.
                    sqlBulkCopy.ColumnMappings.Add("ETSI STANDARD", "Etsi_standard");
                    sqlBulkCopy.ColumnMappings.Add("Version", "Version");
                    sqlBulkCopy.ColumnMappings.Add("Title of document", "Title_doc");
                    sqlBulkCopy.ColumnMappings.Add("Hypelink", "Hypelink");
                    sqlBulkCopy.ColumnMappings.Add("Type of Document", "Type_of_Document");
                    sqlBulkCopy.ColumnMappings.Add("Group", "Group_doc");
                    sqlBulkCopy.ColumnMappings.Add("Low Frequency", "Low_freq");
                    sqlBulkCopy.ColumnMappings.Add("High Frequency", "High_freq");
                    sqlBulkCopy.ColumnMappings.Add("Application", "Application");

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
            



            
            int counter = 0;

            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataTable specificWorkSheet0 = reader.AsDataSet().Tables[0];

                    foreach (var temp in specificWorkSheet0.Rows)
                    {
                        if (counter > 0)
                        {
                            DocumentsDbCopy doc = new DocumentsDbCopy();
                            //ImportTempTable imp = new ImportTempTable();

                            doc.Doc_number = "" + ((DataRow)temp)[0];

                            if (((DataRow)temp)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)temp)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }
                            if (((DataRow)temp)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)temp)[2];

                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)temp)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)temp)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }



                            if (((DataRow)temp)[4] != "")
                            {

                                doc.Group_doc = "" + ((DataRow)temp)[4];
                                //Debug.WriteLine("ttt:" + reader.GetValue(5).GetType());

                            }
                            else
                            {
                                doc.Group_doc = "";
                            }


                            if (((DataRow)temp)[5] != "")
                            {
                                doc.Application = "" + ((DataRow)temp)[5];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)temp)[6] != "")
                            {
                                doc.Application2 = "" + ((DataRow)temp)[6];
                            }
                            else
                            {
                                doc.Application2 = "";
                            }


                            if (((DataRow)temp)[7] != "")
                            {
                                doc.Application3 = "" + ((DataRow)temp)[7];
                            }
                            else
                            {
                                doc.Application3 = "";
                            }

                            if (((DataRow)temp)[8] != "")
                            {
                                doc.Application4 = "" + ((DataRow)temp)[8];
                            }
                            else
                            {
                                doc.Application4 = "";
                            }

                            if (((DataRow)temp)[9] != "")
                            {
                                doc.Application5 = "" + ((DataRow)temp)[9];
                            }
                            else
                            {
                                doc.Application5 = "";
                            }
                            if (((DataRow)temp)[10] != "")
                            {
                                doc.Application6 = "" + ((DataRow)temp)[10];
                            }
                            else
                            {
                                doc.Application6 = "";
                            }

                            if (((DataRow)temp)[11] != "")
                            {
                                doc.Application7 = "" + ((DataRow)temp)[11];
                            }
                            else
                            {
                                doc.Application7 = "";
                            }
                            if (((DataRow)temp)[12] != "")
                            {
                                doc.Application8 = "" + ((DataRow)temp)[12];
                            }
                            else
                            {
                                doc.Application8 = "";
                            }

                            if (((DataRow)temp)[13] != "")
                            {
                                doc.Application9 = "" + ((DataRow)temp)[13];
                            }
                            else
                            {
                                doc.Application9 = "";
                            }

                            if (((DataRow)temp)[14] != "")
                            {
                                doc.Application10 = "" + ((DataRow)temp)[14];
                            }
                            else
                            {
                                doc.Application10 = "";
                            }

                            if (((DataRow)temp)[15] != "")
                            {
                                doc.Application11 = "" + ((DataRow)temp)[15];
                            }
                            else
                            {
                                doc.Application11 = "";
                            }

                            if (((DataRow)temp)[16] != "")
                            {
                                doc.Application12 = "" + ((DataRow)temp)[16];
                            }
                            else
                            {
                                doc.Application12 = "";
                            }
                            _conApp.DocumentsDbCopy.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter++;
                        }
                    }
                }
            }


            System.IO.File.Delete(path);


            return RedirectToAction("Files");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //form for upload the file and reading the current file this is consider for the documents and standards

            if (file == null || file.Length == 0)
                return View("Index");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                //Debug.WriteLine("testiing i have entered");
                await file.CopyToAsync(stream);
            }
            //Debug.WriteLine("wwww:" + path);
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //string conString = this.configuration.GetConnectionString("ExcelConString");
            //DataTable dt = new DataTable();
            //conString = string.Format(conString, path);

            //using (OleDbConnection connExcel = new OleDbConnection(conString))
            //{
            //    using (OleDbCommand cmdExcel = new OleDbCommand())
            //    {
            //        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
            //        {
            //            cmdExcel.Connection = connExcel;

            //            //Get the name of First Sheet.
            //            connExcel.Open();
            //            DataTable dtExcelSchema;
            //            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //            //do not change TABLE_NAME it has to be default
            //            string sheetName = dtExcelSchema.Rows[1]["TABLE_NAME"].ToString();
            //            connExcel.Close();
            //            //Debug.WriteLine("name:" + sheetName);
            //            //Read Data from First Sheet.
            //            connExcel.Open();
            //            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
            //            odaExcel.SelectCommand = cmdExcel;
            //            odaExcel.Fill(dt);
            //            connExcel.Close();
            //        }
            //    }
            //}

            //conString = this.configuration.GetConnectionString("AuthDBContextConnection");
            //using (SqlConnection con = new SqlConnection(conString))
            //{
            //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
            //    {
            //        //Set the database table name.
            //        sqlBulkCopy.DestinationTableName = "dbo.StandardsDb";

            //        //[OPTIONAL]: Map the Excel columns with that of the database table.
            //        sqlBulkCopy.ColumnMappings.Add("ETSI STANDARD", "Etsi_standard");
            //        sqlBulkCopy.ColumnMappings.Add("Version", "Version");
            //        sqlBulkCopy.ColumnMappings.Add("Title of document", "Title_doc");
            //        sqlBulkCopy.ColumnMappings.Add("Hypelink", "Hypelink");
            //        sqlBulkCopy.ColumnMappings.Add("Type of Document", "Type_of_Document");
            //        sqlBulkCopy.ColumnMappings.Add("Group", "Group_doc");
            //        sqlBulkCopy.ColumnMappings.Add("Low Frequency", "Low_freq");
            //        sqlBulkCopy.ColumnMappings.Add("High Frequency", "High_freq");
            //        sqlBulkCopy.ColumnMappings.Add("Application", "Application");

            //        con.Open();
            //        sqlBulkCopy.WriteToServer(dt);
            //        con.Close();
            //    }
            //}
            //conString = this.configuration.GetConnectionString("ExcelConString");
            //dt = new DataTable();
            //conString = string.Format(conString, path);

            //using (OleDbConnection connExcel = new OleDbConnection(conString))
            //{
            //    using (OleDbCommand cmdExcel = new OleDbCommand())
            //    {
            //        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
            //        {
            //            cmdExcel.Connection = connExcel;

            //            //Get the name of First Sheet.
            //            connExcel.Open();
            //            DataTable dtExcelSchema;
            //            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //            //do not change TABLE_NAME it has to be default
            //            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            //            connExcel.Close();
            //            //Debug.WriteLine("name:" + sheetName);
            //            //Read Data from First Sheet.
            //            connExcel.Open();
            //            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
            //            odaExcel.SelectCommand = cmdExcel;
            //            odaExcel.Fill(dt);
            //            connExcel.Close();
            //        }
            //    }
            //}

            //conString = this.configuration.GetConnectionString("AuthDBContextConnection");
            //using (SqlConnection con = new SqlConnection(conString))
            //{
            //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
            //    {
            //        //Set the database table name.
            //        sqlBulkCopy.DestinationTableName = "dbo.DocumentsDb";

            //        //[OPTIONAL]: Map the Excel columns with that of the database table.
            //        sqlBulkCopy.ColumnMappings.Add("Document Number", "Doc_number");
            //        sqlBulkCopy.ColumnMappings.Add("Title of document", "Title_of_doc");
            //        sqlBulkCopy.ColumnMappings.Add("Hyperlink", "Hyperlink");
            //        sqlBulkCopy.ColumnMappings.Add("Type of Document", "Type_of_doc");
            //        sqlBulkCopy.ColumnMappings.Add("Group", "Group_doc");
            //        sqlBulkCopy.ColumnMappings.Add("Application", "Application");
            //        sqlBulkCopy.ColumnMappings.Add("Application2", "Application2");
            //        sqlBulkCopy.ColumnMappings.Add("Application3", "Application3");
            //        sqlBulkCopy.ColumnMappings.Add("Application4", "Application4");
            //        sqlBulkCopy.ColumnMappings.Add("Application5", "Application5");
            //        sqlBulkCopy.ColumnMappings.Add("Application6", "Application6");
            //        sqlBulkCopy.ColumnMappings.Add("Application7", "Application7");
            //        sqlBulkCopy.ColumnMappings.Add("Application8", "Application8");
            //        sqlBulkCopy.ColumnMappings.Add("Application9", "Application9");
            //        sqlBulkCopy.ColumnMappings.Add("Application10", "Application10");
            //        sqlBulkCopy.ColumnMappings.Add("Application11", "Application11");
            //        sqlBulkCopy.ColumnMappings.Add("Application12", "Application12");



            //        con.Open();
            //        sqlBulkCopy.WriteToServer(dt);
            //        con.Close();
            //    }
            //}
            
            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int counter = 0;
                    int counter16 = 0;
                    int counter14 = 0;
                    int counter13 = 0;
                    int counter12 = 0;
                    int counter11 = 0;
                    int counter10 = 0;
                    int counter9 = 0;
                    int counter8 = 0;
                    int counter7 = 0;
                    int counter6 = 0;
                    int counter5 = 0;
                    int counter4 = 0;
                    int counter3 = 0;
                    int counter2 = 0;
                    int counter1 = 0;
                    //ETSI
                    DataTable specificWorkSheet16 = reader.AsDataSet().Tables[16];
                    
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet16.Rows)
                    {

                        if (counter16 > 0)
                        {
                            StandardsDb doc = new StandardsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Etsi_standard = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Etsi_standard = "";
                            }


                            if (((DataRow)row)[1] != "")
                            {
                                doc.Part = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Part = "";
                            }
                             
                            if (((DataRow)row)[2] != "")
                            {
                                doc.Version = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Version = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {

                                doc.Title_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Title_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {

                                doc.Hypelink = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Hypelink = "";
                            }


                            if (((DataRow)row)[5] != "")
                            {

                                doc.Type_of_Document = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Type_of_Document = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                
                                doc.Group_doc = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[7] != "")
                            {

                                doc.Low_freq = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {

                                doc.High_freq = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }

                            if (((DataRow)row)[9] != "")
                            {

                                doc.Application = "" + ((DataRow)row)[9];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)row)[10] != "")
                            {

                                doc.Comment = "" + ((DataRow)row)[10];
                            }
                            else
                            {
                                doc.Comment = "";
                                
                            }

                            if (((DataRow)row)[11] != "")
                            {
                                
                                doc.ValidFrom = "" + ((DataRow)row)[11];
                            }
                            else
                            {
                                doc.ValidFrom = "";
                                
                            }

                            if (((DataRow)row)[12] != "")
                            {
                                doc.Expiry = "" + ((DataRow)row)[12];
                            }
                            else
                            {
                                doc.Expiry = "";
                            }

                            _conApp.StandardsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter16++;
                        }

                    }

                    //ETSI SRdoc
                    DataTable specificWorkSheet = reader.AsDataSet().Tables[15];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet.Rows)
                    {

                        if (counter > 0)
                        {
                            StandardsDb doc = new StandardsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Etsi_standard = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Etsi_standard = "";
                            }


                            if (((DataRow)row)[1] != "")
                            {
                                doc.Part = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Part = "";
                            }
                            doc.Version = "";
                            if (((DataRow)row)[2] != "")
                            {
                                doc.Title_doc = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Title_doc = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Hypelink = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Hypelink = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Type_of_Document = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Type_of_Document = "";
                            }


                            if (((DataRow)row)[5] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[7] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)row)[9] != "")
                            {
                                doc.Comment = "" + ((DataRow)row)[9];
                            }
                            else
                            {
                                doc.Comment = "";
                            }

                            if (((DataRow)row)[10] != "")
                            {
                                doc.ValidFrom = "" + ((DataRow)row)[10];
                            }
                            else
                            {
                                doc.ValidFrom = "";
                            }

                            if (((DataRow)row)[11] != "")
                            {
                                doc.Expiry = "" + ((DataRow)row)[11];
                            }
                            else
                            {
                                doc.Expiry = "";
                            }


                            _conApp.StandardsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter++;
                        }

                    }


                    //Other
                    DataTable specificWorkSheet14 = reader.AsDataSet().Tables[14];

                    foreach (var temp in specificWorkSheet14.Rows)
                    {
                        if (counter14 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)temp)[1] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)temp)[1];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)temp)[2] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)temp)[2];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)temp)[3] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)temp)[3];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)temp)[4] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)temp)[4];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)temp)[5] != "")
                            {
                                doc.High_freq = "" + ((DataRow)temp)[5];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)temp)[6] != "")
                            {
                                doc.Application = "" + ((DataRow)temp)[6];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)temp)[7] != "")
                            {
                                doc.Comment = "" + ((DataRow)temp)[7];
                            }
                            else
                            {
                                doc.Comment = "";
                            }

                            if (((DataRow)temp)[8] != "")
                            {
                                doc.ValidFrom = "" + ((DataRow)temp)[8];
                            }
                            else
                            {
                                doc.ValidFrom = "";
                            }

                            if (((DataRow)temp)[9] != "")
                            {
                                doc.Expiry = "" + ((DataRow)temp)[9];
                            }
                            else
                            {
                                doc.Expiry = "";
                            }

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter14++;
                        }

                    }
                    //RIS model tab
                    DataTable specificWorkSheet13 = reader.AsDataSet().Tables[13];

                    foreach (var temp in specificWorkSheet13.Rows)
                    {
                        if (counter13 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)temp)[1] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)temp)[1];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)temp)[2] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)temp)[2];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)temp)[3] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)temp)[3];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)temp)[4] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)temp)[4];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)temp)[5] != "")
                            {
                                doc.High_freq = "" + ((DataRow)temp)[5];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)temp)[6] != "")
                            {
                                doc.Application = "" + ((DataRow)temp)[6];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)temp)[7] != "")
                            {
                                doc.Comment = "" + ((DataRow)temp)[7];
                            }
                            else
                            {
                                doc.Comment = "";
                            }

                            if (((DataRow)temp)[8] != "")
                            {
                                doc.ValidFrom = "" + ((DataRow)temp)[8];
                            }
                            else
                            {
                                doc.ValidFrom = "";
                            }

                            if (((DataRow)temp)[9] != "")
                            {
                                doc.Expiry = "" + ((DataRow)temp)[9];
                            }
                            else
                            {
                                doc.Expiry = "";
                            }

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter13++;
                        }

                    }
                    //Third party tab
                    DataTable specificWorkSheet12 = reader.AsDataSet().Tables[12];

                    foreach (var temp in specificWorkSheet12.Rows)
                    {
                        if (counter12 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)temp)[1] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)temp)[1];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)temp)[2] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)temp)[2];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)temp)[3] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)temp)[3];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)temp)[4] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)temp)[4];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)temp)[5] != "")
                            {
                                doc.High_freq = "" + ((DataRow)temp)[5];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)temp)[6] != "")
                            {
                                doc.Application = "" + ((DataRow)temp)[6];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)temp)[7] != "")
                            {
                                doc.Comment = "" + ((DataRow)temp)[7];
                            }
                            else
                            {
                                doc.Comment = "";
                            }

                            if (((DataRow)temp)[8] != "")
                            {
                                doc.ValidFrom = "" + ((DataRow)temp)[8];
                            }
                            else
                            {
                                doc.ValidFrom = "";
                            }

                            if (((DataRow)temp)[9] != "")
                            {
                                doc.Expiry = "" + ((DataRow)temp)[9];
                            }
                            else
                            {
                                doc.Expiry = "";
                            }

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter12++;
                        }

                    }

                    //CEPT report
                    DataTable specificWorkSheet11 = reader.AsDataSet().Tables[11];

                    foreach (var temp in specificWorkSheet11.Rows)
                    {
                        if (counter11 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)temp)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)temp)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)temp)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)temp)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)temp)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)temp)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)temp)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)temp)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)temp)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)temp)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)temp)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)temp)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)temp)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)temp)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)temp)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)temp)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }


                            doc.Comment = "";

                            doc.ValidFrom = "";

                            doc.Expiry = "";


                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter11++;
                        }

                    }

                    //T-R tab
                    DataTable specificWorkSheet10 = reader.AsDataSet().Tables[10];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet10.Rows)
                    {

                        if (counter10 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }


                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }


                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }

                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }


                            doc.Comment = "";
                            doc.ValidFrom = "";
                            doc.Expiry = "";


                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter10++;
                        }

                        //Debug.WriteLine();
                    }
                }
            }

            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    
                    int counter9 = 0;
                    int counter8 = 0;
                    int counter7 = 0;
                    int counter6 = 0;
                    int counter5 = 0;
                    int counter4 = 0;
                    int counter3 = 0;
                    int counter2 = 0;
                    int counter1 = 0;
                    //ERC-REP tab
                    DataTable specificWorkSheet9 = reader.AsDataSet().Tables[9];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet9.Rows)
                    {

                        if (counter9 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            doc.Comment = "";

                            doc.ValidFrom = "";

                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter9++;
                        }

                    }

                    //ERC-REC
                    DataTable specificWorkSheet8 = reader.AsDataSet().Tables[8];
                    //Debug.WriteLine("name of sheet:" + specificWorkSheet8.TableName);
                    foreach (var row in specificWorkSheet8.Rows)
                    {

                        if (counter8 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {
                                doc.Comment = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                doc.Comment = "";
                            }

                            doc.ValidFrom = "";

                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter8++;
                        }

                    }

                    //ECC-REC
                    DataTable specificWorkSheet7 = reader.AsDataSet().Tables[7];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet7.Rows)
                    {

                        if (counter7 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {
                                doc.Comment = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                doc.Comment = "";
                            }


                            doc.ValidFrom = "";

                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter7++;
                        }

                    }

                    //ECTRA-DEC
                    DataTable specificWorkSheet6 = reader.AsDataSet().Tables[6];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet6.Rows)
                    {

                        if (counter6 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }


                            doc.Comment = "";
                            doc.ValidFrom = "";

                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter6++;
                        }

                    }

                    //ERC-DEC tab
                    DataTable specificWorkSheet5 = reader.AsDataSet().Tables[5];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet5.Rows)
                    {

                        if (counter5 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }


                            doc.Comment = "";
                            doc.ValidFrom = "";

                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter5++;
                        }

                    }

                    //ECC-DEC
                    DataTable specificWorkSheet4 = reader.AsDataSet().Tables[4];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet4.Rows)
                    {

                        if (counter4 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }


                            doc.Comment = "";
                            doc.ValidFrom = "";

                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter4++;
                        }

                    }
                    //ECO-REP 
                    DataTable specificWorkSheet3 = reader.AsDataSet().Tables[3];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet3.Rows)
                    {

                        if (counter3 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {
                                doc.Comment = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                doc.Comment = "";
                            }

                            
                            doc.ValidFrom = "";
                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter3++;
                        }

                    }

                    //ECC-REP
                    DataTable specificWorkSheet2 = reader.AsDataSet().Tables[2];
                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet2.Rows)
                    {

                        if (counter2 > 0)
                        {
                            DocumentsDb doc = new DocumentsDb();
                            if (((DataRow)row)[0] != "")
                            {
                                doc.Doc_number = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                doc.Doc_number = "";
                            }

                            if (((DataRow)row)[1] != "")
                            {
                                doc.Title_of_doc = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                doc.Title_of_doc = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                doc.Hyperlink = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                doc.Hyperlink = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {
                                doc.Type_of_doc = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                doc.Type_of_doc = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {
                                doc.Group_doc = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                doc.Group_doc = "";
                            }

                            if (((DataRow)row)[5] != "")
                            {
                                doc.Low_freq = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                doc.Low_freq = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {
                                doc.High_freq = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                doc.High_freq = "";
                            }


                            if (((DataRow)row)[7] != "")
                            {
                                doc.Application = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                doc.Application = "";
                            }

                            

                            doc.Comment = "";
                            doc.ValidFrom = "";
                            doc.Expiry = "";

                            _conApp.DocumentsDb.Add(doc);
                            _conApp.SaveChanges();
                        }
                        else
                        {
                            counter2++;
                        }

                    }

                //    DataTable specificWorkSheet1 = reader.AsDataSet().Tables[1];
                //    ////Debug.WriteLine("name:" + specificWorkSheet.TableName);
                //    foreach (var row in specificWorkSheet1.Rows)
                //    {

                //        if (counter1 > 0)
                //        {
                //            if (((DataRow)row)[0] != "")
                //            {
                //                string value = "" + ((DataRow)row)[0];
                //                if (value.Length >= 10)
                //                {

                //                    string TempStand = value.Substring(0, 10);
                //                    string ContainsEN = "";

                //                    //Console.WriteLine(authorName);
                //                    if (TempStand.Contains("EN"))
                //                    {
                //                        int Index = value.IndexOf("EN");
                //                        if (Index == 0)
                //                        {
                //                            ContainsEN = value.Substring(0, 10);
                //                        }
                //                        else
                //                        {
                //                            ContainsEN = value.Substring(Index, 10);
                //                        }
                //                        StandardsDbEN doc = new StandardsDbEN();
                //                        doc.Etsi_standard = ContainsEN;

                //                        doc.Part = "";
                //                        doc.Version = "";

                //                        if (((DataRow)row)[0] != "")
                //                        {
                //                            doc.Title_doc = "" + ((DataRow)row)[0];
                //                        }
                //                        else
                //                        {
                //                            doc.Title_doc = "";
                //                        }

                //                        if (((DataRow)row)[1] != "")
                //                        {
                //                            doc.Hypelink = "" + ((DataRow)row)[1];
                //                        }
                //                        else
                //                        {
                //                            doc.Hypelink = "";
                //                        }



                //                        if (((DataRow)row)[2] != "")
                //                        {
                //                            doc.Low_freq = "" + ((DataRow)row)[2];
                //                        }
                //                        else
                //                        {
                //                            doc.Low_freq = "";
                //                        }

                //                        if (((DataRow)row)[3] != "")
                //                        {
                //                            doc.High_freq = "" + ((DataRow)row)[3];
                //                        }
                //                        else
                //                        {
                //                            doc.High_freq = "";
                //                        }


                //                        if (((DataRow)row)[4] != "")
                //                        {
                //                            doc.Application = "" + ((DataRow)row)[4];
                //                        }
                //                        else
                //                        {
                //                            doc.Application = "";
                //                        }

                //                        doc.Type_of_Document = "R";
                //                        if (((DataRow)row)[5] != "")
                //                        {
                //                            doc.Group_doc = "" + ((DataRow)row)[5];
                //                        }
                //                        else
                //                        {
                //                            doc.Group_doc = "";
                //                        }

                //                        if (((DataRow)row)[7] != "")
                //                        {
                //                            doc.Comment = "" + ((DataRow)row)[7];
                //                        }
                //                        else
                //                        {
                //                            doc.Comment = "";
                //                        }

                //                        doc.ValidFrom = "";

                //                        doc.Expiry = "";

                //                        _conApp.StandardsDbEN.Add(doc);
                //                        _conApp.SaveChanges();
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            counter1++;
                //        }

                //    }


                }
            }
                    //do
                    //{
                    //    while (reader.Read())
                    //    {
                    //        if (counter > 0)
                    //        {
                    //            try
                    //            {
                                    

                                    //DocumentsDb doc = new DocumentsDb();
                                    ////ImportTempTable imp = new ImportTempTable();
                                    //doc.Doc_number = reader.GetString(0);

                                    //doc.Title_of_doc = reader.GetString(1);

                                    //if (reader.GetValue(2) != null)
                                    //{
                                    //    doc.Hyperlink = reader.GetValue(2).ToString();

                                    //}
                                    //else
                                    //{
                                    //    doc.Hyperlink = "";
                                    //}

                                    //if (reader.GetValue(3) != null)
                                    //{
                                    //    doc.Type_of_doc = reader.GetValue(3).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Type_of_doc = "";
                                    //}



                                    //if (reader.GetValue(4) != null)
                                    //{

                                    //    doc.Group_doc = "" + reader.GetValue(4).ToString();

                                    //    //Debug.WriteLine("ttt:" + reader.GetValue(5).GetType());

                                    //}
                                    //else
                                    //{
                                    //    doc.Group_doc = "";
                                    //}


                                    //if (reader.GetValue(5) != null)
                                    //{
                                    //    doc.Application = reader.GetString(5).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application = "";
                                    //}

                                    //if (reader.GetValue(6) != null)
                                    //{
                                    //    doc.Application2 = reader.GetString(6).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application2 = "";
                                    //}


                                    //if (reader.GetValue(7) != null)
                                    //{
                                    //    doc.Application3 = reader.GetString(7).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application3 = "";
                                    //}

                                    //if (reader.GetValue(8) != null)
                                    //{
                                    //    doc.Application4 = reader.GetString(8).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application4 = "";
                                    //}

                                    //if (reader.GetValue(9) != null)
                                    //{
                                    //    doc.Application5 = reader.GetString(9).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application5 = "";
                                    //}
                                    //if (reader.GetValue(10) != null)
                                    //{
                                    //    doc.Application6 = reader.GetString(10).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application6 = "";
                                    //}

                                    //if (reader.GetValue(11) != null)
                                    //{
                                    //    doc.Application7 = reader.GetString(11).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application7 = "";
                                    //}
                                    //if (reader.GetValue(12) != null)
                                    //{
                                    //    doc.Application8 = reader.GetString(12).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application8 = "";
                                    //}

                                    //if (reader.GetValue(13) != null)
                                    //{
                                    //    doc.Application9 = reader.GetString(13).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application9 = "";
                                    //}

                                    //if (reader.GetValue(14) != null)
                                    //{
                                    //    doc.Application10 = reader.GetString(14).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application10 = "";
                                    //}

                                    //if (reader.GetValue(15) != null)
                                    //{
                                    //    doc.Application11 = reader.GetString(15).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application11 = "";
                                    //}

                                    //if (reader.GetValue(16) != null)
                                    //{
                                    //    doc.Application12 = reader.GetString(16).ToString();
                                    //}
                                    //else
                                    //{
                                    //    doc.Application12 = "";
                                    //}
                                    //_conApp.DocumentsDb.Add(doc);
                                    //_conApp.SaveChanges();
                                    //_conImport.Add(imp);
                                    //_conImport.SaveChanges();

                                    //add record to xml file.      

                                    // reader.GetDouble(0);
                    //            }catch(Exception ex)
                    //            {

                    //            }
                    //            }
                    //        else
                    //        {
                    //            counter++;
                    //        } 
                           
                    //    }
                    //} while (reader.NextResult());

               // }
            //}
                    //delete current file from server
                    System.IO.File.Delete(path);


            return RedirectToAction("Files");
        }
        [HttpPost]
        public async Task<IActionResult> UploadFileServices(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return View("Index");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        file.FileName);
            //add file on server.
            using (var stream = new FileStream(path, FileMode.Create))
            {
                //Debug.WriteLine("testiing i have entered");
                await file.CopyToAsync(stream);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string conString = this.configuration.GetConnectionString("ExcelConString");
            DataTable dt = new DataTable();
            conString = string.Format(conString, path);
            DataSet dataSet = new DataSet();
            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        //do not change TABLE_NAME it has to be default
                        string sheetName = dtExcelSchema.Rows[2]["TABLE_NAME"].ToString();
                        connExcel.Close();
                        //Debug.WriteLine("name:" + sheetName);
                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                         var Servpath = Path.Combine(
                        Directory.GetCurrentDirectory(), "Files",
                        "db_services.xml");
                        
                        dataSet.Tables.Add(dt);
                        dataSet.WriteXml(@Servpath);
                        //dataGridView1.DataSource = dt.Tables[0];
                        connExcel.Close();
                    }
                }
            }
            DataTable dtDatabase = new DataTable();
            DataSet dataSetDatabase = new DataSet();
            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        //dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        dtExcelSchema = connExcel.GetSchema("Tables");

                        //do not change TABLE_NAME it has to be default
                        //excel file have to contain one more empty sheet.
                        //the number of current rows will not match. DB Services.xlsx
                        //Debug.WriteLine("wwww:" + dtExcelSchema.Rows.Count);
                        string sheetName = "";
                        if (dtExcelSchema.Rows.Count > 0)
                        {
                            sheetName = dtExcelSchema.Rows[2]["TABLE_NAME"].ToString();
                        }

                        dtExcelSchema.Clear();
                        dtExcelSchema.Dispose();
                        //string sheetName = dtExcelSchema.Rows[5]["TABLE_NAME"].ToString();
                        connExcel.Close();
                        //Debug.WriteLine("name:" + sheetName);
                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dtDatabase);

                        dataSetDatabase.Tables.Add(dtDatabase);
                       
                        connExcel.Close();
                    }
                }
            }

            DataTable dtDatabaseSecondary = new DataTable();
            DataSet dataSetDatabaseSecondary = new DataSet();
            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        //dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        dtExcelSchema = connExcel.GetSchema("Tables");

                        //do not change TABLE_NAME it has to be default
                        //excel file have to contain one more empty sheet.
                        //the number of current rows will not match. DB Services.xlsx
                        //Debug.WriteLine("wwww:" + dtExcelSchema.Rows.Count);
                        string sheetName = "";
                        if (dtExcelSchema.Rows.Count > 0)
                        {
                            sheetName = dtExcelSchema.Rows[3]["TABLE_NAME"].ToString();
                        }

                        dtExcelSchema.Clear();
                        dtExcelSchema.Dispose();
                        //string sheetName = dtExcelSchema.Rows[5]["TABLE_NAME"].ToString();
                        connExcel.Close();
                        //Debug.WriteLine("name:" + sheetName);
                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dtDatabaseSecondary);

                        dataSetDatabaseSecondary.Tables.Add(dtDatabaseSecondary);

                        var Servpath = Path.Combine(
                       Directory.GetCurrentDirectory(), "wwwroot/Files",
                       "db_services_secondary.xml");

                        dataSetDatabaseSecondary.WriteXml(@Servpath);

                        connExcel.Close();
                    }
                }
            }


            //Debug.WriteLine("values count:" + dataSetDatabase.Tables.Count);
            //read data from excel and add into table
            UploadServicesTerms usTerms = new UploadServicesTerms(_conAll);
            usTerms.AddPrimaryValues(dataSetDatabase);

            usTerms.AddSecondaryValues(dataSetDatabaseSecondary);
            return RedirectToAction("Files");
        }

        

        [HttpPost]
        public async Task<IActionResult> UploadFileAllData(IFormFile file)
        {
            //form for the upload the file and reading the current file, this is consider for all data allocation their relation to the footnotes and
            //application related documents and standards for current application. 


            //if there is records in temporary table cleaned up before insert.
            //using (var tran = _conImport.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        string cmd = $"TRUNCATE TABLE [FrekvencijeProject].[dbo].[ImportTempTable]";
            //        _conImport.Database.ExecuteSqlCommand(cmd);
            //        tran.Commit();
            //    }
            //    catch (Exception ex)
            //    {

            //        tran.Rollback();
            //    }
            //}

            //using (var tran = _conImport.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        string cmd = $"truncate table [FrekvencijeProject].[dbo].[FootnoteAllocation]; delete from  [FrekvencijeProject].[dbo].[AllocationDb];  DBCC CHECKIDENT ([AllocationDb], RESEED, 0); delete from  [FrekvencijeProject].[dbo].[AllocationRangeDb]; DBCC CHECKIDENT ([AllocationRangeDb], RESEED, 0);";
            //        _conImport.Database.ExecuteSqlCommand(cmd);
            //        tran.Commit();
            //    }
            //    catch (Exception ex)
            //    {

            //        tran.Rollback();
            //    }
            //}

            using (var tran = _conImport.Database.BeginTransaction())
            {
                try
                {
                    string cmd = $"truncate table [FrekvencijeProject].[dbo].[Application]; delete from  [FrekvencijeProject].[dbo].[ApplicationRange];  DBCC CHECKIDENT ([ApplicationRange], RESEED, 0);";
                    _conImport.Database.ExecuteSqlCommand(cmd);
                    tran.Commit();
                }
                catch (Exception ex)
                {

                    tran.Rollback();
                }
            }


            if (file == null || file.Length == 0)
                return View("Index");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                //Debug.WriteLine("testiing i have entered");
                await file.CopyToAsync(stream);
            }
            //Debug.WriteLine("wwww:" + path);
            //i just installed new package for reading excel file the previous has problem with length of string.
            //if records contains more than 256 characters it cut off.
            //NuGet\Install - Package ExcelDataReader - Version 3.6.0

            var XMlServerPath = Path.Combine(
                      Directory.GetCurrentDirectory(), "wwwroot/Files",
                      "db_all_data.xml");
            XDocument doc =
      XDocument.Parse("<root></root>");
            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods

                    int counter = 0;
                    do
                    {
                        while (reader.Read())
                        {
                            if (counter > 0)
                            {
                                //if (reader.GetString(0) != null)
                                //{
                                //    ImportTempTable imp = new ImportTempTable();
                                //    imp.lower_freq = reader.GetString(0);

                                //    imp.higher_freq = reader.GetString(1);

                                //    imp.itu_reg = reader.GetString(2);
                                //    if (reader.GetValue(3) != null)
                                //    {
                                //        imp.itu_reg_freq = reader.GetValue(3).ToString();
                                //    }
                                //    else
                                //    {
                                //        imp.itu_reg_freq = "";
                                //    }

                                //    imp.main_reg = reader.GetString(4);

                                //    if (reader.GetValue(5) != null)
                                //    {

                                //        if (reader.GetValue(5).GetType() == typeof(double))
                                //        {
                                //            imp.main_reg_freq = "" + reader.GetDouble(5);
                                //        }
                                //        else
                                //        {
                                //            imp.main_reg_freq = reader.GetString(5).ToString();
                                //        }
                                //        //Debug.WriteLine("ttt:" + reader.GetValue(5).GetType());

                                //    }
                                //    else
                                //    {
                                //        imp.main_reg_freq = "";
                                //    }


                                //    if (reader.GetValue(6) != null)
                                //    {
                                //        imp.document = reader.GetString(6).ToString();
                                //    }
                                //    else
                                //    {
                                //        imp.document = "";
                                //    }

                                //    if (reader.GetValue(7) != null)
                                //    {
                                //        imp.application = reader.GetString(7).ToString();
                                //    }
                                //    else
                                //    {
                                //        imp.application = "";
                                //    }


                                //    if (reader.GetValue(8) != null)
                                //    {
                                //        imp.standard = reader.GetString(8).ToString();
                                //    }
                                //    else
                                //    {
                                //        imp.standard = "";
                                //    }

                                //    if (reader.GetValue(9) != null)
                                //    {
                                //        imp.notes = reader.GetString(9).ToString();
                                //    }
                                //    else
                                //    {
                                //        imp.notes = "";
                                //    }


                                //    _conImport.Add(imp);
                                //    _conImport.SaveChanges();

                                //    //add record to xml file.      
                                //    XElement elem = new XElement("element");

                                //    elem.Add(new XElement("lower_freq", imp.lower_freq));
                                //    elem.Add(new XElement("higher_freq", imp.higher_freq));
                                //    elem.Add(new XElement("itu_reg", imp.itu_reg));
                                //    elem.Add(new XElement("itu_reg_freq", imp.itu_reg_freq));
                                //    elem.Add(new XElement("main_reg", imp.main_reg));
                                //    elem.Add(new XElement("main_reg_freq", imp.main_reg_freq));
                                //    elem.Add(new XElement("document", imp.document));
                                //    elem.Add(new XElement("application", imp.application));
                                //    elem.Add(new XElement("standard", imp.standard));
                                //    elem.Add(new XElement("notes", imp.notes));
                                //    doc.Root.Add(elem);
                                //    //Debug.WriteLine("oo:" + reader.GetString(0)+"=="+reader.GetString(1));
                                //    doc.Save(XMlServerPath);
                                //}
                                // reader.GetDouble(0);
                            }
                            counter++;
                        }
                    } while (reader.NextResult());


                }
            }
            //doc.Save(XMlServerPath);

            //delete current file from server
            System.IO.File.Delete(path);
            //parsing data for allocation from temporary table and fill the database. 
            UploadData upD = new UploadData(_conImport, _conAll,_conApp,_conTracking, configuration);
            //upD.ReadDataItu();
            //upD.ReadDataPalestine();
            upD.ReadApplicationData();
            upD.ReadDocumentsData();
            //upD.ReadStandardsData();
            return RedirectToAction("Files");
        }

        [HttpPost]
        public async Task<IActionResult> UploadInterfacesData(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return View("Index");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                //Debug.WriteLine("testiing i have entered");
                await file.CopyToAsync(stream);
            }


            using (var tran = _conImport.Database.BeginTransaction())
            {
                try
                {
                    string cmd = $"truncate table [FrekvencijeProject].[dbo].[ImportTempInterfaces];";
                    _conInterfaces.Database.ExecuteSqlCommand(cmd);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
            }


            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int counter = 0;
                    
                    DataTable specificWorkSheet0 = reader.AsDataSet().Tables[0];

                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet0.Rows)
                    {
                        if (counter == 0)
                        {
                            counter++;
                        }
                        else if (counter > 0)
                        {

                            ImportTempInterfaces interfacesValue = new ImportTempInterfaces();
                            if (((DataRow)row)[0] != "")
                            {
                                interfacesValue.Country = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                interfacesValue.Country = "";
                            }


                            if (((DataRow)row)[1] != "")
                            {
                                interfacesValue.RadiocommunicationService = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                interfacesValue.RadiocommunicationService = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                interfacesValue.AllocationNotes = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                interfacesValue.AllocationNotes = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {

                                interfacesValue.Application = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                interfacesValue.Application = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {

                                interfacesValue.ApplicationNotes = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                interfacesValue.ApplicationNotes = "";
                            }


                            if (((DataRow)row)[5] != "")
                            {

                                interfacesValue.LowerFrequency = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                interfacesValue.LowerFrequency = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {

                                interfacesValue.UpperFrequency = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                interfacesValue.UpperFrequency = "";
                            }

                            if (((DataRow)row)[7] != "")
                            {

                                interfacesValue.FrequencyBandNotes = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                interfacesValue.FrequencyBandNotes = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {

                                interfacesValue.Channeling = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                interfacesValue.Channeling = "";
                            }

                            if (((DataRow)row)[9] != "")
                            {

                                interfacesValue.ChannelingNotes = "" + ((DataRow)row)[9];
                            }
                            else
                            {
                                interfacesValue.ChannelingNotes = "";
                            }

                            if (((DataRow)row)[10] != "")
                            {

                                interfacesValue.Modulation = "" + ((DataRow)row)[10];
                            }
                            else
                            {
                                interfacesValue.Modulation = "";

                            }

                            if (((DataRow)row)[11] != "")
                            {

                                interfacesValue.ModulationNotes = "" + ((DataRow)row)[11];
                            }
                            else
                            {
                                interfacesValue.ModulationNotes = "";

                            }

                            if (((DataRow)row)[12] != "")
                            {
                                interfacesValue.DirectionSeparation = "" + ((DataRow)row)[12];
                            }
                            else
                            {
                                interfacesValue.DirectionSeparation = "";
                            }

                            if (((DataRow)row)[13] != "")
                            {
                                interfacesValue.DirectionSeparationNotes = "" + ((DataRow)row)[13];
                            }
                            else
                            {
                                interfacesValue.DirectionSeparationNotes = "";
                            }


                            if (((DataRow)row)[14] != "")
                            {
                                interfacesValue.TransmitPower = "" + ((DataRow)row)[14];
                            }
                            else
                            {
                                interfacesValue.TransmitPower = "";
                            }

                            if (((DataRow)row)[15] != "")
                            {
                                interfacesValue.TransmitPowerNotes = "" + ((DataRow)row)[15];
                            }
                            else
                            {
                                interfacesValue.TransmitPowerNotes = "";
                            }

                            if (((DataRow)row)[16] != "")
                            {
                                interfacesValue.ChannelAccessRules = "" + ((DataRow)row)[16];
                            }
                            else
                            {
                                interfacesValue.ChannelAccessRules = "";
                            }

                            if (((DataRow)row)[17] != "")
                            {
                                interfacesValue.ChannelAccessNotes = "" + ((DataRow)row)[17];
                            }
                            else
                            {
                                interfacesValue.ChannelAccessNotes = "";
                            }

                            if (((DataRow)row)[18] != "")
                            {
                                interfacesValue.AuthorisationRegime = "" + ((DataRow)row)[18];
                            }
                            else
                            {
                                interfacesValue.AuthorisationRegime = "";
                            }

                            if (((DataRow)row)[19] != "")
                            {
                                interfacesValue.AuthorisationRegimeNotes = "" + ((DataRow)row)[19];
                            }
                            else
                            {
                                interfacesValue.AuthorisationRegimeNotes = "";
                            }


                            if (((DataRow)row)[20] != "")
                            {
                                interfacesValue.AdditionalEssentialRequirements = "" + ((DataRow)row)[20];
                            }
                            else
                            {
                                interfacesValue.AdditionalEssentialRequirements = "";
                            }

                            if (((DataRow)row)[21] != "")
                            {
                                interfacesValue.AdditionalEssentialRequirementsNotes = "" + ((DataRow)row)[21];
                            }
                            else
                            {
                                interfacesValue.AdditionalEssentialRequirementsNotes = "";
                            }

                            if (((DataRow)row)[22] != "")
                            {
                                interfacesValue.FrequencyPlanningAssumptions = "" + ((DataRow)row)[22];
                            }
                            else
                            {
                                interfacesValue.FrequencyPlanningAssumptions = "";
                            }

                            if (((DataRow)row)[23] != "")
                            {
                                interfacesValue.FrequencyPlanningAssumptionsNotes = "" + ((DataRow)row)[23];
                            }
                            else
                            {
                                interfacesValue.FrequencyPlanningAssumptionsNotes = "";
                            }

                            if (((DataRow)row)[24] != "")
                            {
                                interfacesValue.PlannedChanges = "" + ((DataRow)row)[24];
                            }
                            else
                            {
                                interfacesValue.PlannedChanges = "";
                            }

                            if (((DataRow)row)[25] != "")
                            {
                                interfacesValue.PlannedChangesNotes = "" + ((DataRow)row)[25];
                            }
                            else
                            {
                                interfacesValue.PlannedChangesNotes = "";
                            }

                            if (((DataRow)row)[26] != "")
                            {
                                interfacesValue.Reference = "" + ((DataRow)row)[26];
                            }
                            else
                            {
                                interfacesValue.Reference = "";
                            }

                            if (((DataRow)row)[27] != "")
                            {
                                interfacesValue.ReferenceNotes = "" + ((DataRow)row)[27];
                            }
                            else
                            {
                                interfacesValue.ReferenceNotes = "";
                            }

                            if (((DataRow)row)[28] != "")
                            {
                                interfacesValue.Notification = "" + ((DataRow)row)[28];
                            }
                            else
                            {
                                interfacesValue.Notification = "";
                            }

                            if (((DataRow)row)[29] != "")
                            {
                                interfacesValue.NotificationNotes = "" + ((DataRow)row)[29];
                            }
                            else
                            {
                                interfacesValue.NotificationNotes = "";
                            }

                            if (((DataRow)row)[30] != "")
                            {
                                interfacesValue.Remarks = "" + ((DataRow)row)[30];
                            }
                            else
                            {
                                interfacesValue.Remarks = "";
                            }

                            if (((DataRow)row)[31] != "")
                            {
                                interfacesValue.RemarksNotes = "" + ((DataRow)row)[31];
                            }
                            else
                            {
                                interfacesValue.RemarksNotes = "";
                            }
                            //Debug.WriteLine("qqq:" + interfacesValue.Application + "===" + interfacesValue.LowerFrequency);

                            _conInterfaces.ImportTempInterfaces.Add(interfacesValue);
                            _conInterfaces.SaveChanges();

                        }

                    }
                }
            }

            return RedirectToAction("Files");
        }


        [HttpPost]
        public async Task<IActionResult> UploadRightOfUseData(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return View("Index");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Files",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                //Debug.WriteLine("testiing i have entered");
                await file.CopyToAsync(stream);
            }


            using (var tran = _conImport.Database.BeginTransaction())
            {
                try
                {
                    string cmd = $"truncate table [FrekvencijeProject].[dbo].[ImportTempRightOfUse];";
                    _conInterfaces.Database.ExecuteSqlCommand(cmd);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
            }


            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int counter = 0;

                    DataTable specificWorkSheet0 = reader.AsDataSet().Tables[0];

                    //Debug.WriteLine("name:" + specificWorkSheet.TableName);
                    foreach (var row in specificWorkSheet0.Rows)
                    {
                        if (counter == 0)
                        {
                            counter++;
                        }
                        else if (counter > 0)
                        {

                            ImportTempRightOfUse rightOfUse = new ImportTempRightOfUse();
                            if (((DataRow)row)[0] != "")
                            {
                                rightOfUse.Duplex = "" + ((DataRow)row)[0];
                            }
                            else
                            {
                                rightOfUse.Duplex = "";
                            }


                            if (((DataRow)row)[1] != "")
                            {
                                rightOfUse.DownLowerFrequency = "" + ((DataRow)row)[1];
                            }
                            else
                            {
                                rightOfUse.DownLowerFrequency = "";
                            }

                            if (((DataRow)row)[2] != "")
                            {
                                rightOfUse.DownUpperFrequency = "" + ((DataRow)row)[2];
                            }
                            else
                            {
                                rightOfUse.DownUpperFrequency = "";
                            }

                            if (((DataRow)row)[3] != "")
                            {

                                rightOfUse.UpLinkLowerFrequency = "" + ((DataRow)row)[3];
                            }
                            else
                            {
                                rightOfUse.UpLinkLowerFrequency = "";
                            }

                            if (((DataRow)row)[4] != "")
                            {

                                rightOfUse.UpLinkUpperFrequency = "" + ((DataRow)row)[4];
                            }
                            else
                            {
                                rightOfUse.UpLinkUpperFrequency = "";
                            }


                            if (((DataRow)row)[5] != "")
                            {

                                rightOfUse.Application = "" + ((DataRow)row)[5];
                            }
                            else
                            {
                                rightOfUse.Application = "";
                            }

                            if (((DataRow)row)[6] != "")
                            {

                                rightOfUse.Technology = "" + ((DataRow)row)[6];
                            }
                            else
                            {
                                rightOfUse.Technology = "";
                            }

                            if (((DataRow)row)[7] != "")
                            {

                                rightOfUse.LicenceHolder = "" + ((DataRow)row)[7];
                            }
                            else
                            {
                                rightOfUse.LicenceHolder = "";
                            }

                            if (((DataRow)row)[7] != "")
                            {
                                string tempValue = ""+ ((DataRow)row)[7];
                                if(tempValue == "Jawwal")
                                {
                                    rightOfUse.LicenceHolderLink = "www.jawal.ps";
                                }
                                else if(tempValue == "Ooredoo")
                                {
                                    rightOfUse.LicenceHolderLink = "www.ooredoo.ps";
                                }
                                else if(tempValue == "PALTEL")
                                {
                                    rightOfUse.LicenceHolderLink = "www.paltel.ps";
                                }
                               
                                //interfacesValue.Channeling = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                rightOfUse.LicenceHolderLink = "";
                            }

                            if (((DataRow)row)[8] != "")
                            {

                                rightOfUse.StartDate = "" + ((DataRow)row)[8];
                            }
                            else
                            {
                                rightOfUse.StartDate = "";
                            }

                            if (((DataRow)row)[9] != "")
                            {

                                rightOfUse.ExpiryDate = "" + ((DataRow)row)[9];
                            }
                            else
                            {
                                rightOfUse.ExpiryDate = "";

                            }

                            if (((DataRow)row)[10] != "")
                            {
                                string tempVal = ""+((DataRow)row)[10];
                                
                                
                                if (tempVal == "Westbank")
                                {
                                    rightOfUse.Location = "Regional coverage";
                                }else if(tempVal == "Gaza")
                                {
                                    rightOfUse.Location = "Regional coverage";
                                }
                                else
                                {
                                    rightOfUse.Location = "" + ((DataRow)row)[10];
                                }
                            }
                            else
                            {
                                rightOfUse.Location = "";

                            }

                            if (((DataRow)row)[10] != "")
                            {
                                string tempValue = "" + ((DataRow)row)[10];
                                if(tempValue == "National coverage")
                                {
                                    rightOfUse.LocationLink = "";
                                }
                                else
                                {
                                    rightOfUse.LocationLink = "" + ((DataRow)row)[10];
                                }
                                
                            }
                            else
                            {
                                rightOfUse.LocationLink = "";
                            }

                            if (((DataRow)row)[11] != "")
                            {
                                rightOfUse.SpectrumTrading = "" + ((DataRow)row)[11];
                            }
                            else
                            {
                                rightOfUse.SpectrumTrading = "";
                            }


                            if (((DataRow)row)[12] != "")
                            {
                                rightOfUse.Country = "" + ((DataRow)row)[12];
                            }
                            else
                            {
                                rightOfUse.Country = "";
                            }

                            if (((DataRow)row)[13] != "")
                            {
                                rightOfUse.ShortComment = "" + ((DataRow)row)[13];
                            }
                            else
                            {
                                rightOfUse.ShortComment = "";
                            }
                            if (rightOfUse.Duplex != "")
                            {
                                _conRightOfUse.ImportTempRightOfUse.Add(rightOfUse);
                                _conRightOfUse.SaveChanges();
                            }
                           

                        }

                    }
                }
            }

            return RedirectToAction("Files");
        }

        bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals('-'))
                {
                    continue;
                }else if(input[i].Equals(' ')){
                    continue;
                }
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }
    }
}
