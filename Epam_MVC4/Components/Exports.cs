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

namespace Epam_MVC4.Components
{
    public class Exports
    {
        private IEnumerable<Export> ExportsFormats;

        public Exports()
        {
            var csv = new CsvExport();
            var xml = new XmlExporter();
            var json = new JsonExport();

            ExportsFormats = new List<Export>() { csv, xml, json };
        }

        public IEnumerable<Export> GetExportFormats()
        {
            return ExportsFormats;
        }

        public Export GetExportByName(string Name)
        {
            return ExportsFormats.First(x => x.Name == Name);
        }
    }

    public abstract class Export
    {
        public abstract string Id { get; }
        public abstract string Name { get; }

        public abstract byte[] GetExportData(IEnumerable<DataRecord> data);
    }

    public class CsvExport : Export
    {
        public override string Id { get; }
        public override string Name { get; }

        public override byte[] GetExportData(IEnumerable<DataRecord> data)
        {

            byte[] result = new byte[] { };

            string content = "";

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

            result = Encoding.ASCII.GetBytes(content);

            return result;
        }
        public CsvExport()
        {
            Id = "CSV";
            Name = "CSV";
        }
    }

    public class XmlExporter : Export
    {
        public override string Id { get; }
        public override string Name { get; }

        public override byte[] GetExportData(IEnumerable<DataRecord> data)
        {

            byte[] result = new byte[] { };

            string content = "";

            XmlSerializer Xml_Serializer = new XmlSerializer(data.GetType());
            using (StringWriter Writer = new StringWriter())
            {
                Xml_Serializer.Serialize(Writer, data);
                content = Writer.ToString();
            }

            result = Encoding.ASCII.GetBytes(content);

            return result;
        }
        public XmlExporter()
        {
            Id = "XML";
            Name = "XML";
        }
    }

    public class JsonExport : Export
    {
        public override string Id { get; }
        public override string Name { get; }

        public override byte[] GetExportData(IEnumerable<DataRecord> data)
        {

            byte[] result = new byte[] { };

            string content = JsonConvert.SerializeObject(data);

            result = Encoding.ASCII.GetBytes(content);

            return result;
        }
        public JsonExport()
        {
            Id = "JSON";
            Name = "JSON";
        }
    }
}