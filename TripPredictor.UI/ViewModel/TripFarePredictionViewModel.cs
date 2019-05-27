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
    public class TripFarePredictionViewModel : TripDataPredictionViewModelBase
    {
        public TripFarePredictionViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
           
        }

        #region Actions
        protected override void OnLoadTestTripTimeSampleDataExecute()
        {
            TestTripData = new TripData()
            {
                VendorId = "1",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 940,
                TripDistance = 6.1f,
                PaymentType = "2",
                FareAmount = 15.5f
            };
            PredictedResult = string.Empty;
        }

        protected override void OnPredictTripTimeExecute()
        {
            TestTripData.FareAmount = 0;
            var score = TripPredictor.GetPredictedResult(TestTripData);
            PredictedResult = $"{score}";
        }
        #endregion
    }
}
