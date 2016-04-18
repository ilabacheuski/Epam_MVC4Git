using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Epam_MVC4.Models;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using CsvHelper;
using Newtonsoft.Json;

namespace Epam_MVC4.Controllers
{
    public class ExportsController
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

    public enum ExportFormat
    {
        CSV,
        JSON,
        XML
    }

    public class Export
    {
        private IEnumerable<string> Formats;

        public Export()
        {
            var f = new List<string>();
            var values = Enum.GetNames(typeof(ExportFormat));

            foreach(var format in values)
            {
                f.Add(format);
            }
            Formats = f;            
        }

        public IEnumerable<string> GetFormats()
        {
            return Formats;
        }

        public byte[] GetExportData(IEnumerable<DataRecord> data, ExportFormat format)
        {
            byte[] result = new byte[] { };

            string content = "";

            switch (format)
            {
                case ExportFormat.CSV:
                    using (TextWriter textWriter = new StringWriter())
                    using (var csv = new CsvWriter(textWriter))
                    {
                        csv.Configuration.HasHeaderRecord = true;
                        foreach (var item in data)
                        {
                            csv.WriteRecord(item);
                        }
                        content = textWriter.ToString();
                    }
                    break;
                case ExportFormat.JSON:
                    content = JsonConvert.SerializeObject(data);
                    break;
                case ExportFormat.XML:
                    XmlSerializer Xml_Serializer = new XmlSerializer(data.GetType());
                    using (StringWriter Writer = new StringWriter())
                    {
                        Xml_Serializer.Serialize(Writer, data);
                        content = Writer.ToString();
                    }                    
                    break;
                default:
                    content = "";
                    break;
            }

            result = Encoding.ASCII.GetBytes(content);

            return result;
        }
    }
}