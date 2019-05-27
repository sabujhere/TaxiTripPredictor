using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPredictor.Common;

namespace TripPredictor.UI.ViewModel
{
    public interface ITestTripDataPredictionViewModel
    {
        string PredictedResult { get; set; }
        TripData TestTripData { get; set; }
    }
}
