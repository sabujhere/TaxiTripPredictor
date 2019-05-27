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
    public class TripTimePredictionViewModel:ViewModelBase, ITripDataPredictionViewModel
    {
        #region Private properties

        private readonly IEventAggregator _eventAggregator;

        private TripData _testTripData;

        private string _predictedResult;

        private ITripPredictor _tripTimePredictor;

        #endregion

        #region Public Properties
        public string PredictedResult
        {
            get => _predictedResult;
            set
            {
                _predictedResult = value;
                OnPropertyChanged();
            }
        }

        public TripData TestTripData
        {
            get => _testTripData;
            set
            {
                _testTripData = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand PredictTripTimeCommand { get; }

        public DelegateCommand LoadTestTripTimeSampleDataCommand { get; }
        #endregion

        #region Constructor
        public TripTimePredictionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PredictorUpdatedEvent>()
                .Subscribe(OnTripPredictorUpdated);
            
            PredictTripTimeCommand = new DelegateCommand(OnPredictTripTimeExecute);
            LoadTestTripTimeSampleDataCommand = new DelegateCommand(OnLoadTestTripTimeSampleDataExecute);
        }
        #endregion

        #region Actions

        private void OnLoadTestTripTimeSampleDataExecute()
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

        private void OnPredictTripTimeExecute()
        {
            var score = _tripTimePredictor.GetPredictedResult(TestTripData);
            PredictedResult = $" Drop-off time {TestTripData.PUTime.AddSeconds(score)}";
        }

        private void OnTripPredictorUpdated(ITripPredictor tripTimePredictor)
        {
            _tripTimePredictor = tripTimePredictor;
            PredictedResult = string.Empty;
        }

        #endregion
    }
}
