using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPredictor.Common;

namespace TripPredictor.UI.ViewModel
{
    public class TestTripFarePredictionViewModel:ViewModelBase,ITestTripDataPredictionViewModel
    {
        public string PredictedResult { get; set; }
        public TripData TestTripData { get; set; }
    }
}
