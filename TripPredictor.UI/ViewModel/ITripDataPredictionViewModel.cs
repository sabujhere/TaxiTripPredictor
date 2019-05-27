using TripPredictor.Common;

namespace TripPredictor.UI.ViewModel
{
    public interface ITripDataPredictionViewModel
    {
        string PredictedResult { get; set; }
        TripData TestTripData { get; set; }
    }
}
