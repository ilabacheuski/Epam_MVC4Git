//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;
//using Epam_MVC4.Components;

//namespace Epam_MVC4.Models
//{

//    public class HomeViewModel
//    {
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name ="Date: from")]
//        public DateTime StartDate { get; set; }
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "to")]
//        public DateTime EndDate { get; set; }

//        public IEnumerable<DataProvider> DataProviders { get; }
        
//        [Display(Name ="Get from:")]
//        public string ProviderName { get; set; }

//        [Display(Name = "Query")]
//        public string Query { get; set; }

//        [Display(Name = "Export to: ")]
//        public IEnumerable<Export> Exports { get; set; }

//        public bool? ShowTable { get; set; }
//        public IEnumerable<DataRecord> Data { get; set; }
//        public int? Page { get; set; }

        

//        public HomeViewModel()
//        {
//            Exports = new Exports().GetExportFormats();
//            DataProviders = new DataProviders().GetProviders();

//            //StartDate = DateTime.Today.AddMonths(-12);
//            //EndDate = DateTime.Today;

//        }
//    }

//}