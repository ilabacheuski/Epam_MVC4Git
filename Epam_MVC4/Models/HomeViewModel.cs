using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public DataProvider Provider;

        public IEnumerable<DataRecord> Table { get; set; }

        public string Query;

        public HomeViewModel()
        {
            DataProvider google = new Google();
            DataProvider yahoo = new Yahoo();

            DataProviders = new List<DataProvider>() { google, yahoo };

            Provider = DataProviders.FirstOrDefault();
            StartDate = DateTime.Today.AddMonths(-12);
            EndDate = DateTime.Today;
            PerPage = PerPage._20;
            page = 1;
        }
    }

}