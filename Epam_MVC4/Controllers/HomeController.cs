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

    // TODO: 1. Лучше не использовать глобальные переменные TempData, ViewBag, Session. Во первых, потому что они затрудняют юнит-тестирование. Во-вторых: TempData - данные передаются с сервера на клиент и обратно, ненужного трафика лучше избегать; ViewBag - у нас MVC фреймворк, данные с контроллера на view должны передаваться только через модель; Session - данные храняться в памяти сервера, придет миллион пользователей и память закончиться, если сервер перегрузиться, то все данные о сессиях теряются.

    // TODO: 2. StringReader, StringWriter, HttpWebRequest, HttpWebResponse, Stream являются IDisposable, их надо закрывать после использования, лучше для этого использовать using.

    // TODO: 3. Приведение типов является плохой практикой, этого надо избегать.

    // TODO: 4. ExportsController - непонятно зачем он наследуется от Controller.И сама логика типов экспорта размазана по всему приложению. Если надо будет добавить новый формат, то придется исправлять не в одном месте.А по идее должно быть просто - добавили новый класс-формат и все заработало из коробки.

    // DONE: 5. Использование CsvHelper только добавляет код, без него можно на чистом C# распарсить в несколько строчек, а с этой библиотекой код превращается в ненужную кашу c маппингами, конфигурациями и конвертерами.

    // TODO: 6. HomeCоntroller.GetQuotes(string Query, string Provider, DateTime StartDate, DateTime EndDate, PerPage PerPage = PerPage._20) - вместо string Provider можно сделать так, чтобы сразу передавался DataProvider Provider(не обязательно, но желательно)

    // DONE: 7. Сам DataProvider спроектирован с непонятной идеей, половину работы за него приходиться делать контроллеру.Я бы сделал там только один метод типа IEnumerable<DataRecord> GetData(), который мне сразу возвращает то что надо.В этом случае такой провайдер можно будет без проблем переиспользовать, а в текущем виде он никому не нужен, потому что к нему надо еще кучу кода дописывать.Причем лежит DataProvider в папке Models, хотя никакого отношения к моделям не имеет.

    // DONE: 8. С DataProvider та же проблема, что и с экспортом. При добавлении нового DataProvider надо модифицировать код в очень многих местах.А должно быть просто - создал новый наследник DataProvider - и все везде сразу заработало правильно.

    // TODO: 9. _Table.cshtml - там логика во view (@if (Model.Count() != 0)...). View должно только распечатывать модель, никаких условий и логики там быть не должно.Вся логика должны быть только в контроллере, потому что его можно легко протестировать по тому, что он передает в модели.А view тестировать сложно.

    // TODO: 10. Конечно, передавать html через ajax сейчас не модно.Лучше использовать json.Тогда сразу появится сервис, с которым интересно поработать :)

    // DONE: 11. Весь проект закоммичен за 3 коммита.Насколько я понимаю, то культуры работы с git все еще нет, когда код коммититься законченными небольшими кусками.

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
                _TableViewModel tModel = new _TableViewModel();
                tModel.Data = new List<DataRecord>();
                return PartialView("_Table", tModel);                
            }

            ViewBag.ProviderId = new SelectList(hvm.DataProviders, "Id", "Name");

            return View("Index", hvm);
        }

        public void Export(ExportFormat SelectedFormat)
        {
            IEnumerable<DataRecord> data = GetQuotesRepository().GetData();


            var export = new Export();

            byte[] byteArray = export.GetExportData(data, SelectedFormat);

            Response.Clear();
            Response.AppendHeader("Content-Disposition", "filename=export." + SelectedFormat.ToString());
            Response.AppendHeader("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(byteArray);
        }

        [HttpPost]
        public ActionResult GetQuotes(string Query, 
            //[Bind(Include ="Id")]
            string ProviderName, DateTime StartDate, DateTime EndDate, PerPage PerPage = PerPage._20)
        {
            //string Query, string ProviderName, DateTime StartDate, DateTime EndDate, PerPage PerPage = PerPage._20
            //string Query = hvm.Query;
            //int ProviderName = hvm.SelectedProviderId;
            //DateTime StartDate = hvm.StartDate;
            //DateTime EndDate = hvm.EndDate;
            //PerPage PerPage = PerPage._20;

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

            var provider = hvm.DataProviders.First(x => x.Name == ProviderName);
            hvm.StartDate = StartDate;
            hvm.EndDate = EndDate;
            hvm.PerPage = PerPage;
            hvm.page = 1;
            //hvm.ProviderName = provider;

            if (Request.IsAjaxRequest())
            {
                IEnumerable<DataRecord> data = provider.GetData(Query, StartDate, EndDate);

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

        public ActionResult Error()
        {
            ViewBag.ErrorMsg = "There was an error";
            return View("_Error");
        }

        //[HttpPost]
        //public ActionResult Temp(HomeViewModel model)
        //{

        //    return View("Index");
        //}

        [HttpPost]
        public ActionResult Temp(DataProvider provider)
        {

            return View("Index");
        }

        //private DataProvider GetDataProvider(string Name)
        //{
        //    return hvm.DataProviders.First(x => x.Name == Name);
        //}

        //private string GetCSV(string url)
        //{
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

        //    StreamReader sr = new StreamReader(resp.GetResponseStream());
        //    string results = sr.ReadToEnd();
        //    sr.Close();

        //    return results;
        //}

        //private IEnumerable<DataRecord> GetData(string Quote, string ProviderName, DateTime StartDate, DateTime EndDate)
        //{
        //    DataProvider provider = GetDataProvider(ProviderName);
        //    string doc;
        //    try
        //    {
        //        doc = GetCSV(provider.GetUrl() + provider.GetPost(Quote, StartDate, EndDate));
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }

        //    if (String.IsNullOrEmpty(doc)) return null;

        //    IEnumerable<DataRecord> result = provider.GetDataFromCSV(doc);

        //    return result;
        //}

    }
}