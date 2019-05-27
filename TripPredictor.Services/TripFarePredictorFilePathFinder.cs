namespace TripPredictor.Services
{
    public class TripFarePredictorFilePathFinder: TripPredictorFilePathFinderBase
    {
        public TripFarePredictorFilePathFinder()
        {
            TestFileName = "Test_yellow_tripdata_2017-01-001.csv";
            TrainingFileName = "Training_yellow_tripdata_2017-01-000.csv";
            ModelFileName = "TaxiTripFareModel.zip";
        }
    }
}
