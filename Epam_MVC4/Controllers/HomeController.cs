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
using Epam_MVC4.Components;
using Newtonsoft.Json;
using System.Xml.Serialization;
using Newtonsoft.Json.Converters;

namespace Epam_MVC4.Controllers
{

    // DONE?: 1. Лучше не использовать глобальные переменные TempData, ViewBag, Session. Во первых, потому что они затрудняют юнит-тестирование. Во-вторых: TempData - данные передаются с сервера на клиент и обратно, ненужного трафика лучше избегать; ViewBag - у нас MVC фреймворк, данные с контроллера на view должны передаваться только через модель; Session - данные храняться в памяти сервера, придет миллион пользователей и память закончиться, если сервер перегрузиться, то все данные о сессиях теряются.

    // DONE: 2. StringReader, StringWriter, HttpWebRequest, HttpWebResponse, Stream являются IDisposable, их надо закрывать после использования, лучше для этого использовать using.

    // DONE?: 3. Приведение типов является плохой практикой, этого надо избегать.

    // DONE: 4. ExportsController - непонятно зачем он наследуется от Controller.И сама логика типов экспорта размазана по всему приложению. Если надо будет добавить новый формат, то придется исправлять не в одном месте.А по идее должно быть просто - добавили новый класс-формат и все заработало из коробки.

    // DONE: 5. Использование CsvHelper только добавляет код, без него можно на чистом C# распарсить в несколько строчек, а с этой библиотекой код превращается в ненужную кашу c маппингами, конфигурациями и конвертерами.

    // DONE?: 6. HomeCоntroller.GetQuotes(string Query, string Provider, DateTime StartDate, DateTime EndDate, PerPage PerPage = PerPage._20) - вместо string Provider можно сделать так, чтобы сразу передавался DataProvider Provider(не обязательно, но желательно)

    // DONE: 7. Сам DataProvider спроектирован с непонятной идеей, половину работы за него приходиться делать контроллеру.Я бы сделал там только один метод типа IEnumerable<DataRecord> GetData(), который мне сразу возвращает то что надо.В этом случае такой провайдер можно будет без проблем переиспользовать, а в текущем виде он никому не нужен, потому что к нему надо еще кучу кода дописывать.Причем лежит DataProvider в папке Models, хотя никакого отношения к моделям не имеет.

    // DONE: 8. С DataProvider та же проблема, что и с экспортом. При добавлении нового DataProvider надо модифицировать код в очень многих местах.А должно быть просто - создал новый наследник DataProvider - и все везде сразу заработало правильно.

    // DONE: 9. _Table.cshtml - там логика во view (@if (Model.Count() != 0)...). View должно только распечатывать модель, никаких условий и логики там быть не должно.Вся логика должны быть только в контроллере, потому что его можно легко протестировать по тому, что он передает в модели.А view тестировать сложно.

    // DONE: 10. Конечно, передавать html через ajax сейчас не модно.Лучше использовать json.Тогда сразу появится сервис, с которым интересно поработать :)

    // DONE: 11. Весь проект закоммичен за 3 коммита.Насколько я понимаю, то культуры работы с git все еще нет, когда код коммититься законченными небольшими кусками.

    // TODO: 12. All the time buzz data provider to get data.

    // TODO: 13. Polymorphism in Export Controllers

    // TODO: GET everywhere

    // TODO: Exports

    public class HomeController : Controller
    {
        private HomeViewModel hvm = new HomeViewModel();

        public ActionResult Index(HomeViewModel model)
        {
            if(!ModelState.IsValid)
            {
                RedirectToAction("Error");
            }

            if (model.StartDate == default(DateTime)) model.StartDate = DateTime.Today.AddMonths(-12);
            if (model.EndDate == default(DateTime)) model.EndDate = DateTime.Today;

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Test(HomeViewModel model)
        {
            if(model.StartDate == default(DateTime)) model.StartDate = DateTime.Today.AddMonths(-12);
            if(model.EndDate == default(DateTime)) model.EndDate = DateTime.Today;
            return View("Index", model);
        }

        [HttpGet]
        public void Export(string Query, string ProviderName, DateTime StartDate, DateTime EndDate, string SelectedFormat)
        {
            var provider = hvm.DataProviders.First(x => x.Name == ProviderName);

            var data = provider.GetData(Query, StartDate, EndDate);

            var ExportsFactory = new Exports();

            var export = ExportsFactory.GetExportByName(SelectedFormat);

            byte[] byteArray = export.GetExportData(data);

            Response.Clear();
            Response.AppendHeader("Content-Disposition", "filename=export." + SelectedFormat.ToString());
            Response.AppendHeader("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(byteArray);
        }

        [HttpGet]
        public ContentResult GetQuotes(string Query, string ProviderName, DateTime StartDate, DateTime EndDate)
        {
            var provider = hvm.DataProviders.First(x => x.Name == ProviderName);
        
            _TableViewModel tModel = new _TableViewModel();
            tModel.Data = provider.GetData(Query, StartDate, EndDate);
            tModel.ShowTable = (tModel.Data.Count() != 0);

            var Exports = new Exports();

            tModel.Exports = Exports.GetExportFormats();

            var json = JsonConvert.SerializeObject(tModel, Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            return new ContentResult { Content = json, ContentType = "application/json" };
        }

        public ActionResult Error()
        {
            ErrorViewModel err = new ErrorViewModel("There was an error");
            return View("_Error", err);
        }
    }
}