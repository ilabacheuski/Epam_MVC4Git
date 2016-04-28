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
using System.Net;
using Epam_MVC4.Models;

namespace Epam_MVC4.Components
{
    public enum DataFormat
    {
        csv,
        xml,
        json
    }

    public class DataProviders
    {
        private IEnumerable<DataProvider> Providers { get; }

        public DataProviders()
        {
            var google = new Google();
            var yahoo = new Yahoo();

            Providers = new List<DataProvider>() { google, yahoo };
        }

        public IEnumerable<DataProvider> GetProviders()
        {
            return Providers;
        }

        public DataProvider GetProviderByName(string name)
        {
            return Providers.First(x => x.Name == name);
        }
    }

    public class DataProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        protected string Url { get; set; }

        public virtual IEnumerable<DataRecord> GetData(string query, DateTime startDate, DateTime endDate, DataFormat format = DataFormat.csv) { return new List<DataRecord>(); }
    }

    public class Google : DataProvider
    {
        public Google()
        {
            Id = 0;
            Name = "Google";
            Url = "http://www.google.com/finance/historical";
        }

        public override IEnumerable<DataRecord> GetData(string query, DateTime startDate, DateTime endDate, DataFormat format = DataFormat.csv)
        {
            List<DataRecord> data = new List<DataRecord>();

            string queryURL = "?q=" + query + "&startdate=" + startDate.ToString("MMM+d,+yyyy", CultureInfo.InvariantCulture) + "&enddate="
                                                + endDate.ToString("MMM+d,+yyyy", CultureInfo.InvariantCulture) + "&num=30&output=" + format.ToString();

            string results;
            var req = (HttpWebRequest)WebRequest.Create(Url + queryURL);
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
                sr.Close();
            }

            using (var textReader = new StringReader(results))
            {
                string line;

                if ((line = textReader.ReadLine()) == null) return data; // Skip headers

                while((line = textReader.ReadLine()) != null)
                {
                    var record = new DataRecord();
                    string[] values = line.Split(',');

                    DateTime TradeDate = default(DateTime);
                    double Open, Low, High, Close;
                    uint Volume;

                    if(values.Length >=6)
                    {
                        DateTime tradeDate = default(System.DateTime);
                        try
                        {
                            const string dateFormat = @"d-MMM-yy";
                            tradeDate = DateTime.ParseExact(values[0], dateFormat, CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format(@"Error parsing date '{0}': {1}", values[0], ex.Message));
                            continue;
                        }

                        if (!Double.TryParse(values[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Open))  continue;
                        if (!Double.TryParse(values[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out High))   continue;
                        if (!Double.TryParse(values[3], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Low))  continue;
                        if (!Double.TryParse(values[4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Close)) continue;
                        if (!uint.TryParse(values[5], out Volume)) continue;

                        record.TradeDate = tradeDate;
                        record.Open = Open;
                        record.Low = Low;
                        record.High = High;
                        record.Close = Close;
                        record.Volume = Volume;

                        data.Add(record);
                    }
                }
            }

            return data;
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

        public override IEnumerable<DataRecord> GetData(string query, DateTime startDate, DateTime endDate, DataFormat format = DataFormat.csv)
        {
            List<DataRecord> data = new List<DataRecord>();

            string queryURL = "?s=" + query + "&a=" + (startDate.Month - 1).ToString("00") + startDate.ToString(@"&b=dd&c=yyyy")
                                + @"&d=" + (endDate.Month - 1).ToString("00") + endDate.ToString(@"&e=dd&\f=yyyy") + "&g=d&ignore=." + format.ToString();

            string results;
            var req = (HttpWebRequest)WebRequest.Create(Url + queryURL);
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
                sr.Close();
            }

            using (var textReader = new StringReader(results))
            {
                string line;

                if ((line = textReader.ReadLine()) == null) return data; // Skip headers

                while ((line = textReader.ReadLine()) != null)
                {
                    var record = new DataRecord();
                    string[] values = line.Split(',');

                    double Open, Low, High, Close;
                    uint Volume;

                    if (values.Length >= 6)
                    {
                        DateTime tradeDate = default(DateTime);
                        try
                        {
                            const string dateFormat = @"yyyy-MM-dd";
                            tradeDate = DateTime.ParseExact(values[0], dateFormat, CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format(@"Error parsing date '{0}': {1}", values[0], ex.Message));
                            continue;
                        }

                        if (!Double.TryParse(values[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Open)) continue;
                        if (!Double.TryParse(values[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out High)) continue;
                        if (!Double.TryParse(values[3], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Low)) continue;
                        if (!Double.TryParse(values[4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Close)) continue;
                        if (!uint.TryParse(values[5], out Volume)) continue;

                        record.TradeDate = tradeDate;
                        record.Open = Open;
                        record.Low = Low;
                        record.High = High;
                        record.Close = Close;
                        record.Volume = Volume;

                        data.Add(record);
                    }
                }
            }

            return data;
        }
    }
}