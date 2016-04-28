using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Epam_MVC4.Components;

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

        public IEnumerable<DataProvider> DataProviders { get; }

        public virtual DataProvider SelectedProvider { get; set; }
        
        public string ProviderName { get; set; }

        public IEnumerable<DataRecord> Table { get; set; }

        public string Query;

        public virtual int ProviderId { get; set; }

        public HomeViewModel()
        {
            DataProviders = new DataProviders().GetProviders();
            StartDate = DateTime.Today.AddMonths(-12);
            EndDate = DateTime.Today;
            page = 1;

        }
    }

}