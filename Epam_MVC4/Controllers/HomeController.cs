using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Epam_MVC4.Models;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using CsvHelper;
using PagedList;
using PagedList.Mvc;
using Epam_MVC4.Controllers;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Epam_MVC4.Controllers
{
    public class HomeController : Controller
    {
        private HomeViewModel hvm = new HomeViewModel();

        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            string error = (string)TempData["Error"];
            if (error != null)
            {
                ViewBag.Error = (string)TempData["Error"];
                return PartialView("_Error");
            }

            if (page != 1) hvm.page = page;

            var t_perPage = TempData["PerPage"];
            var perPage = t_perPage == null ? PerPage._20 : (PerPage)t_perPage;
            hvm.PerPage = perPage;

            var repository = GetQuotesRepository();
            hvm.Table = repository.GetOnePageOfData(page, perPage);

            ViewBag.PagedList = repository.GetPagedList(page, perPage);

            ViewBag.Exports = new ExportsController().GetExports();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Table", hvm.Table);                
            }

            return View("Index", hvm);
        }

        public void Export(string Format)
        {
            IEnumerable<DataRecord> data = GetQuotesRepository().GetData();

            string content = "";

            switch (Format)
            {
                case "CSV":
                    TextWriter textWriter = new StringWriter();
                    var csv = new CsvWriter(textWriter);
                    csv.Configuration.HasHeaderRecord = true;
                    foreach (var item in data)
                    {
                        csv.WriteRecord(item);
                    }
                    content = textWriter.ToString();
                    break;
                case "JSON":
                    content = JsonConvert.SerializeObject(data);
                    break;
                case "XML":
                    XmlSerializer Xml_Serializer = new XmlSerializer(data.GetType());
                    StringWriter Writer = new StringWriter();
                    Xml_Serializer.Serialize(Writer, data);
                    content = Writer.ToString();
                    break;
                default:
                    content = "";
                    break;

            }

            byte[] byteArray = Encoding.ASCII.GetBytes(content);

            Response.Clear();
            Response.AppendHeader("Content-Disposition", "filename=export." + Format.ToLower());
            Response.AppendHeader("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(byteArray);
        }

        [HttpPost]
        public ActionResult GetQuotes(string Query, string Provider, DateTime StartDate, DateTime EndDate, PerPage PerPage = PerPage._20)
        {
            if (String.IsNullOrWhiteSpace(Query))
            {
                ModelState.AddModelError("Query", "Please input a querry!");
            }

            if (StartDate > EndDate)
            {
                ModelState.AddModelError("StartDate", "Start date must be less than end date.");
            }

            var repository = GetQuotesRepository();

            if (!ModelState.IsValid)
            {
                hvm.Table = new List<DataRecord>();
                ViewBag.ErrorMsg = "There was error in your request";
                return PartialView("_Error");
            }

            hvm.StartDate = StartDate;
            hvm.EndDate = EndDate;
            hvm.PerPage = PerPage;
            hvm.page = 1;
            hvm.Provider = GetDataProvider(Provider);

            if (Request.IsAjaxRequest())
            {
                IEnumerable<DataRecord> data = GetData(Query, Provider, StartDate, EndDate);             

                if (data == null)
                {
                    TempData["Error"] = "There was an error getting quotes!";
                    return RedirectToAction("Index");
                }

                repository.AddToRepository(data);

                Session["QuotesRepository"] = repository;

                TempData["PerPage"] = PerPage;

                return RedirectToAction("Index");

            }

            hvm.Table = repository.GetData();
            return View("Index", hvm);
            
        }

        private QuotesRepository GetQuotesRepository()
        {
            var repository = (QuotesRepository)Session["QuotesRepository"];
            if (repository == null)
            {
                repository = new QuotesRepository();
                Session["QuotesRepository"] = repository;
            }
            return repository;
        }

        private DataProvider GetDataProvider(string Name)
        {
            return hvm.DataProviders.First(x => x.Name == Name);
        }

        private string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        private IEnumerable<DataRecord> GetData(string Quote, string Provider, DateTime StartDate, DateTime EndDate)
        {
            DataProvider provider = GetDataProvider(Provider);
            string doc;
            try
            {
                doc = GetCSV(provider.GetUrl() + provider.GetPost(Quote, StartDate, EndDate));
            }
            catch (Exception e)
            {
                return null;
            }

            if (String.IsNullOrEmpty(doc)) return null;

            IEnumerable<DataRecord> result = provider.GetDataFromCSV(doc);

            return result;
        }

    }
}