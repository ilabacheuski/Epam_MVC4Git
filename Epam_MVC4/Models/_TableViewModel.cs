using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Epam_MVC4.Components;

namespace Epam_MVC4.Models
{
    public class _TableViewModel
    {
        public bool ShowTable { get; set; }
        public IEnumerable<DataRecord> Data { get; set; }
        public IEnumerable<Export> Exports { get; set; }

        public _TableViewModel()
        {
            Exports = new Exports().GetExportFormats();
        }
    }
}