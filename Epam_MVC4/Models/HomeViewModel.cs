using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Epam_MVC4.Controllers;

namespace Epam_MVC4.Models
{

    public class HomeViewModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public int page;

        [Display(Name = "Per page:")]
        public PerPage PerPage { get; set; }

        public IEnumerable<DataProvider> DataProviders { get; }

        //public IEnumerable<SelectListItem> SelectListProviders { get; set; }

        //public SelectList SelectListProvider { get; set; }

        public string ProviderName { get; set; }

        //public int SelectedProviderId { get; set; }
        //public DataProvider SelectedProvider { get; set; }

        public IEnumerable<DataRecord> Table { get; set; }

        public string Query;

        //public IEnumerable<string> ExportFormatList { get; set; }

        public HomeViewModel()
        {
            DataProviders = new DataProviders().GetProviders();
            //SelectedProvider = DataProviders.FirstOrDefault();
            //ProviderName = DataProviders.FirstOrDefault().Name.ToString();
            StartDate = DateTime.Today.AddMonths(-12);
            EndDate = DateTime.Today;
            PerPage = PerPage._20;
            page = 1;

            //ExportFormatList = new Export().GetFormats();

            //SelectListProvider = new SelectList(new DataProviders().GetProviders(), "Id", "Name");

            //SelectListProviders = from item in new DataProviders().GetProviders() select new SelectListItem { Text = item.Name, Value = item.Id.ToString()};
        }
    }

}