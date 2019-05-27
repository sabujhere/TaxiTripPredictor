using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using TaxiTripDataAnalyzer.Common;

namespace TripDataConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if(!args.Any())
                Console.WriteLine("Nothing to convert here.");
            try
            {
                string filePath = args[0];

                if (!File.Exists(filePath))
                    Console.WriteLine($"Could not find file: {filePath}.");

                var outputDirectoryPath = Path.Combine(Path.GetDirectoryName(filePath), $"Processed-{Path.GetFileName(filePath)}");
                if (File.Exists(outputDirectoryPath))
                    Console.WriteLine($"File was processed: {filePath}.");


                using (var writer = new StreamWriter(outputDirectoryPath))
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(GetTripDatas(filePath));
                    Console.WriteLine($"Please see the output file in location: {outputDirectoryPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        public static IEnumerable<TripData> GetTripDatas(string rawDataFilePath, int numMaxRecords = 10000)
        {
            IEnumerable<TripData> records =
                File.ReadAllLines(rawDataFilePath)
                    .Skip(1)
                    .Select(x => x.Split(','))
                    .Select(x => new TripData()
                    {
                        VendorId = x[0],
                        PUTime = Convert.ToDateTime(x[1]),
                        DOTime = Convert.ToDateTime(x[2]),
                        PassengerCount = float.Parse(x[3], CultureInfo.InvariantCulture),
                        TripDistance = float.Parse(x[4], CultureInfo.InvariantCulture),
                        RateCode = x[5],
                        PULocationID = x[6],
                        DOLocationID = x[7],
                        PaymentType = x[8],
                        FareAmount = float.Parse(x[9], CultureInfo.InvariantCulture),
                        TripTime = (Convert.ToDateTime(x[2]) - Convert.ToDateTime(x[1])).TotalSeconds,

                    })
                    .Take<TripData>(numMaxRecords);

            return records;
        }
    }
}
