using System;
using System.Threading.Tasks;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using TripPredictor.Common.Interfaces;
using TripPredictor.Services;
using TripPredictor.UI.Events;

namespace TripPredictor.UI.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        #region Private variables

        private string _statusMessage;

        private string _currentTrainingFileName;

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructor
        public bool IsTripTimePredictorImpl
        {
            get;
            set;
        }
        public MainWindowViewModel(
            ITripPredictor predictor,
            IEventAggregator eventAggregator,
            Func<IEvaluationMetricViewModel> evaluationMetricViewModelCreator, 
            Func<ITripDataPredictionViewModel> testTripTimeViewModelCreator)
        {
            _eventAggregator = eventAggregator;
            TripDataPredictionViewModel = testTripTimeViewModelCreator();
            EvaluationMetricViewModel = evaluationMetricViewModelCreator();
            TripPredictor = predictor;
            LoadEvaluationResult = new DelegateCommand(OnLoadTrainingDataExecute, OnLoadTrainingDataCanExecute);
            IsTripTimePredictorImpl = TripPredictor is TripTimePredictorImpl;
        }

        #endregion

        #region Actions

        private bool OnLoadTrainingDataCanExecute()
        {
            return true;
        }

        private async void OnLoadTrainingDataExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Csv files (*.csv)|*.csv" };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentTrainingFileName = openFileDialog.FileName;
                await LoadAsync(CurrentTrainingFileName);
            }
        }

        #endregion

        #region Public Methods

        public async Task LoadAsync(string filePath = null)
        {
            StatusMessage = $"Loading Training data";
            var result = await TripPredictor.LoadTrainingDataAsync(filePath);
            
            if (result)
            {
                StatusMessage = "Loaded Training data successfully";
                _eventAggregator.GetEvent<PredictorUpdatedEvent>()
                    .Publish(TripPredictor);

                var evaluationMetric = TripPredictor.GetEvaluationMetric();
                _eventAggregator.GetEvent<NewEvaluationMetricEvent>()
                    .Publish(evaluationMetric);

                return;
            }
            StatusMessage = "Loaded Training data Failed";
        }

        #endregion

        #region Public Properties

        public ITripPredictor TripPredictor { get; }
        public DelegateCommand LoadEvaluationResult { get; }
        public string CurrentTrainingFileName
        {
            get => _currentTrainingFileName;
            private set
            {
                _currentTrainingFileName = value;
                OnPropertyChanged();
            }
        }

        public IEvaluationMetricViewModel EvaluationMetricViewModel { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ITripDataPredictionViewModel TripDataPredictionViewModel { get; }

        #endregion
    }
}
