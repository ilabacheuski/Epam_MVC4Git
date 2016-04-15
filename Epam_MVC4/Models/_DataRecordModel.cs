using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PagedList;
using PagedList.Mvc;

namespace Epam_MVC4.Models
{
    public class DataRecord
    {
        [Key]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name ="Date")]
        public DateTime TradeDate { get; set; }

        [Display(Name = "Open")]
        public double Open { get; set; }

        [Display(Name = "Low")]
        public double Low { get; set; }

        [Display(Name = "High")]
        public double High { get; set; }

        [Display(Name = "Close")]
        public double Close { get; set; }

        [Display(Name = "Volume")]
        [DisplayFormat(DataFormatString ="{0:#,#}")]
        public uint Volume { get; set; }
        
    }

    public class QuotesRepository
    {
        private IEnumerable<DataRecord> Data = new List<DataRecord>();

        public IEnumerable<DataRecord> GetData()
        {
            return Data;
        }

        public IEnumerable<DataRecord> GetOnePageOfData(int page = 1, PerPage perPage = PerPage._20)
        {

            var onePage = Data.ToPagedList(page, (int)perPage);

            return onePage.ToList();
        }

        public void AddToRepository(IEnumerable<DataRecord> data)
        {
            Data = data;
        }

        public IPagedList GetPagedList(int page, PerPage perPage)
        {
            return Data.ToPagedList(page, (int)perPage);
        }
    }

    public enum PerPage
    {
        [Display(Name = "20")]
        _20 = 20,
        [Display(Name = "50")]
        _50 = 50,
        [Display(Name = "100")]
        _100 = 100,
        [Display(Name = "all")]
        _all = 1000
    }
}