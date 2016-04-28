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
        [Display(Name ="StartDate")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "EndDate")]
        public DateTime EndDate { get; set; }

        public IEnumerable<DataProvider> DataProviders { get; }
        
        public string ProviderName { get; set; }

        [Display(Name = "Query")]
        public string Query { get; set; }

        public HomeViewModel()
        {
            DataProviders = new DataProviders().GetProviders();
            //StartDate = DateTime.Today.AddMonths(-12);
            //EndDate = DateTime.Today;

        }
    }

}