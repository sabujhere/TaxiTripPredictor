using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using TripPredictor.Common;
using TripPredictor.UI.Events;

namespace TripPredictor.UI.ViewModel
{
    public abstract class TripDataPredictionViewModelBase:ViewModelBase, ITripDataPredictionViewModel
    {
        #region Private properties

        private readonly IEventAggregator _eventAggregator;

        private TripData _testTripData;

        private string _predictedResult;

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

        private ITripPredictor _tripPredictor;

        protected ITripPredictor TripPredictor
        {
            get => _tripPredictor;
            set
            {
                _tripPredictor = value;
                OnPropertyChanged();
                PredictTripTimeCommand.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand PredictTripTimeCommand { get; }

        public DelegateCommand LoadTestTripTimeSampleDataCommand { get; }
        #endregion

        protected TripDataPredictionViewModelBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PredictorUpdatedEvent>()
                .Subscribe(OnTripPredictorUpdated);

            PredictTripTimeCommand = new DelegateCommand(OnPredictTripTimeExecute, OnPredictTripTimeCanExecute);
            LoadTestTripTimeSampleDataCommand = new DelegateCommand(OnLoadTestTripTimeSampleDataExecute);
        }

       

        #region Actions

        protected abstract void OnLoadTestTripTimeSampleDataExecute();

        protected abstract void OnPredictTripTimeExecute();

        private bool OnPredictTripTimeCanExecute()
        {
            return TripPredictor != null;
        }

        protected  void OnTripPredictorUpdated(ITripPredictor tripTimePredictor)
        {
            TripPredictor = tripTimePredictor;
            PredictedResult = string.Empty;
        }
        
        #endregion
    }
}
