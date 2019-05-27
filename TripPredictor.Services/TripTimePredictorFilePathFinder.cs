namespace TripPredictor.Services
{
    public class TripTimePredictorFilePathFinder : TripPredictorFilePathFinderBase
    {
        public TripTimePredictorFilePathFinder()
        {
            TestFileName = "Test_yellow_tripdata_2017-01-001.csv";
            TrainingFileName = "Training_yellow_tripdata_2017-01-000.csv";
            ModelFileName = "TaxiTripTimeModel.zip";
        }
    }
}
