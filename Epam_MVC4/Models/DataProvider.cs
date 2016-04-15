using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using CsvHelper.TypeConversion;

namespace Epam_MVC4.Models
{
    public enum DataFormat
    {
        csv,
        xml,
        json
    }
    public abstract class DataProvider
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        protected string Url { get; set; }

        public abstract string GetUrl();
        public abstract string GetPost(string Query, DateTime StartDate, DateTime EndDate, DataFormat Format = DataFormat.csv);
        public abstract IEnumerable<DataRecord> GetDataFromCSV(string data);
    }

    public class Google : DataProvider
    {
        public Google()
        {
            Id = 0;
            Name = "Google";
            Url = "http://www.google.com/finance/historical";
        }

        public override IEnumerable<DataRecord> GetDataFromCSV(string data)
        {
            TextReader textReader = new StringReader(data);
            var csv = new CsvReader(textReader);
            csv.Configuration.RegisterClassMap<GoogleMap>();

            return csv.GetRecords<DataRecord>().ToList();
        }

        public override string GetPost(string Query, DateTime StartDate, DateTime EndDate, DataFormat Format = DataFormat.csv)
        {
            return "?q=" + Query + "&startdate=" + StartDate.ToString("MMM+d,+yyyy", CultureInfo.InvariantCulture) + "&enddate=" 
                                                + EndDate.ToString("MMM+d,+yyyy", CultureInfo.InvariantCulture) + "&num=30&output=" + Format.ToString();

            
        }

        public override string GetUrl()
        {
            return Url;
        }


    }

    public class Yahoo : DataProvider
    {
        public Yahoo()
        {
            Id = 1;
            Name = "Yahoo";
            Url = "http://real-chart.finance.yahoo.com/table.csv";
        }

        public override IEnumerable<DataRecord> GetDataFromCSV(string data)
        {
            TextReader textReader = new StringReader(data);
            var csv = new CsvReader(textReader);
            csv.Configuration.RegisterClassMap<YahooMap>();

            return csv.GetRecords<DataRecord>().ToList();
        }

        public override string GetPost(string Query, DateTime StartDate, DateTime EndDate, DataFormat Format = DataFormat.csv)
        {//?s=WU&a=01&b=19&c=2010&d=02&e=19&f=2010&g=d&ignore=.csv
            return "?s="+ Query + "&a=" + (StartDate.Month - 1).ToString("00")  + StartDate.ToString(@"&b=dd&c=yyyy") 
                                + @"&d=" + (EndDate.Month - 1).ToString("00")  + EndDate.ToString(@"&e=dd&\f=yyyy") + "&g=d&ignore=." + Format.ToString();
        }

        public override string GetUrl()
        {
            return Url;
        }
    }

    public sealed class GoogleMap : CsvClassMap<DataRecord>
    {
        public GoogleMap()
        {
            Map(m => m.TradeDate).Name("Date").TypeConverter<GoogleDateConverter>();
            Map(m => m.Open).Name("Open").TypeConverter<DoublePointConverter>();
            Map(m => m.High).Name("High").TypeConverter<DoublePointConverter>();
            Map(m => m.Low).Name("Low").TypeConverter<DoublePointConverter>();
            Map(m => m.Close).Name("Close").TypeConverter<DoublePointConverter>();
            Map(m => m.Volume).Name("Volume");
        }
    }

    public sealed class YahooMap : CsvClassMap<DataRecord>
    {
        public YahooMap()
        {
            Map(m => m.TradeDate).Name("Date").TypeConverter<YahooDateConverter>();
            Map(m => m.Open).Name("Open").TypeConverter<DoublePointConverter>();
            Map(m => m.High).Name("High").TypeConverter<DoublePointConverter>();
            Map(m => m.Low).Name("Low").TypeConverter<DoublePointConverter>();
            Map(m => m.Close).Name("Close").TypeConverter<DoublePointConverter>();
            Map(m => m.Volume).Name("Volume");
        }
    }

    public class GoogleDateConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        private const String dateFormat = @"d-MMM-yy";

        public override bool CanConvertFrom(Type type)
        {
            bool ret = typeof(String) == type;
            return ret;
        }

        public override bool CanConvertTo(Type type)
        {
            bool ret = typeof(DateTime) == type;
            return ret;
        }

        public override object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            DateTime newDate = default(System.DateTime);
            try
            {
                newDate = DateTime.ParseExact(text, dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format(@"Error parsing date '{0}': {1}", text, ex.Message));
            }

            return newDate;
        }
       
        public override string ConvertToString(CsvHelper.TypeConversion.TypeConverterOptions options, object value)
        {
            DateTime oldDate = (System.DateTime)value;
            return oldDate.ToString(dateFormat);
        }
    }

    public class YahooDateConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        private const String dateFormat = @"yyyy-MM-dd";

        public override bool CanConvertFrom(Type type)
        {
            bool ret = typeof(String) == type;
            return ret;
        }

        public override bool CanConvertTo(Type type)
        {
            bool ret = typeof(DateTime) == type;
            return ret;
        }

        public override object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            DateTime newDate = default(System.DateTime);
            try
            {
                newDate = DateTime.ParseExact(text, dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format(@"Error parsing date '{0}': {1}", text, ex.Message));
            }

            return newDate;
        }

        public override string ConvertToString(CsvHelper.TypeConversion.TypeConverterOptions options, object value)
        {
            DateTime oldDate = (System.DateTime)value;
            return oldDate.ToString(dateFormat);
        }
    }

    public class DoublePointConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public override bool CanConvertFrom(Type type)
        {
            return typeof(String) == type;
        }
        public override bool CanConvertTo(Type type)
        {
            return typeof(double) == type;
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            return base.ConvertToString(options, value);
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            double ret;
            try
            {
                ret = Double.Parse(text, CultureInfo.InvariantCulture);
                return ret;
            }
            catch (Exception)
            {

                throw;
            }            

            return base.ConvertFromString(options, text);
        }
    }
}