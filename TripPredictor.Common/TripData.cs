using System;
using Microsoft.ML.Data;

namespace TripPredictor.Common
{
    public class TripData
    {
        [LoadColumn(0)]
        public string VendorId { get; set; }

        [LoadColumn(1)]
        public DateTime PUTime { get; set; }

        [LoadColumn(2)]
        public DateTime DOTime { get; set; }

        [LoadColumn(3)]
        public double PassengerCount { get; set; }

        [LoadColumn(4)]
        public float TripDistance { get; set; }

        [LoadColumn(5)]
        public string RateCode { get; set; }

        [LoadColumn(6)]
        public string PULocationID { get; set; }

        [LoadColumn(7)]
        public string DOLocationID { get; set; }

        [LoadColumn(8)]
        public string PaymentType { get; set; }

        [LoadColumn(9)]
        public double FareAmount { get; set; }

        [LoadColumn(10)]
        public float TripTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(VendorId)}: {VendorId}, {nameof(PUTime)}: {PUTime}, {nameof(DOTime)}: {DOTime}, " +
                   $"{nameof(PassengerCount)}: {PassengerCount}, {nameof(TripDistance)}: {TripDistance}, " +
                   $"{nameof(RateCode)}: {RateCode}, {nameof(PULocationID)}: {PULocationID}, " +
                   $"{nameof(DOLocationID)}: {DOLocationID}, {nameof(PaymentType)}: {PaymentType}, " +
                   $"{nameof(FareAmount)}: {FareAmount}, {nameof(TripTime)}: {TripTime}";
        }

        public string GetStringValues(char separator = ',')
        {
            return $"{VendorId}{separator}{PUTime}{separator}{DOTime}{separator}" +
                   $"{PassengerCount}{separator}{TripDistance}{separator}" +
                   $"{RateCode}{separator}{PULocationID}{separator}" +
                   $"{DOLocationID}{separator}{PaymentType}{separator}" +
                   $"{FareAmount}{separator}{TripTime}";
        }

        public string GetHeaderValues(char separator = ',')
        {
            return $"{nameof(VendorId)}{separator}{nameof(PUTime)}{separator}{nameof(DOTime)}{separator}" +
                   $"{nameof(PassengerCount)}{separator}{nameof(TripDistance)}{separator}" +
                   $"{nameof(RateCode)}{separator}{nameof(PULocationID)}{separator}" +
                   $"{nameof(DOLocationID)}{separator}{nameof(PaymentType)}{separator}" +
                   $"{nameof(FareAmount)}{separator}{nameof(TripTime)}";
        }
    }
}
