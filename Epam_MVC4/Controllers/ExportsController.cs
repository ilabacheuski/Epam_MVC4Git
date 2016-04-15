using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Epam_MVC4.Controllers
{
    public class ExportsController : Controller
    {
        private List<string> Exports = new List<string>();

        public ExportsController()
        {
            Exports.Add("CSV");
            Exports.Add("JSON");
            Exports.Add("XML");
        }

        public List<string> GetExports()
        {
            return Exports;
        }
    }
}