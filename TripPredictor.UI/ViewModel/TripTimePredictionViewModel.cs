using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using TripPredictor.Common;
using TripPredictor.UI.Events;

namespace TripPredictor.UI.ViewModel
{
    public class TripTimePredictionViewModel: TripDataPredictionViewModelBase
    {
        #region Constructor
        public TripTimePredictionViewModel(IEventAggregator eventAggregator)
            :base(eventAggregator)
        {
        }
        #endregion

        #region Actions

        protected override void OnLoadTestTripTimeSampleDataExecute()
        {
            TestTripData = new TripData()
            {
                VendorId = "1",
                PUTime = Convert.ToDateTime("2017-01-01 09:13:16 AM"),
                DOTime = Convert.ToDateTime("2017-01-01 9:28:56 AM"),
                TripDistance = 6.1f,
                PULocationID = "24",
                DOLocationID = "235",
                TripTime = 0
            };
            PredictedResult = string.Empty;
        }

        protected override void OnPredictTripTimeExecute()
        {
            var score = TripPredictor.GetPredictedResult(TestTripData);
            PredictedResult = $" Drop-off time {TestTripData.PUTime.AddSeconds(score)}";
        }
        #endregion
    }
}
