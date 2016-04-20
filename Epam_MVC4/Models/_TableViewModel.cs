using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Epam_MVC4.Controllers;

namespace Epam_MVC4.Models
{
    public class _TableViewModel
    {
        public virtual IEnumerable<ExportFormat> Formats { get; set; }
        public virtual IEnumerable<DataRecord> Data { get; set; }
        public virtual ExportFormat SelectedFormat { get; set; }
        public virtual string DisplayCSS { get; set; }

        public _TableViewModel()
        {
            Formats = Enum.GetValues(typeof(ExportFormat)).Cast<ExportFormat>();
        }
    }
}