using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using TripPredictor.Common;
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

        public MainWindowViewModel(
            ITripPredictor predictor,
            IEventAggregator eventAggregator,
            Func<IEvaluationMetricViewModel> evaluationMetricViewModelCreator, 
            Func<ITestTripDataPredictionViewModel> testTripTimeViewModelCreator)
        {
            _eventAggregator = eventAggregator;
            TestTripDataPredictionViewModel = testTripTimeViewModelCreator();
            EvaluationMetricViewModel = evaluationMetricViewModelCreator();
            TripPredictor = predictor;
            LoadEvaluationResult = new DelegateCommand(OnLoadTrainingDataExecute, OnLoadTrainingDataCanExecute);
        }

        #endregion

        #region Actions

        private bool OnLoadTrainingDataCanExecute()
        {
            return true;
        }

        private void OnLoadTrainingDataExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Csv files (*.csv)|*.csv" };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentTrainingFileName = openFileDialog.FileName;
            }
        }

        #endregion

        #region Public Methods

        public async Task LoadAsync()
        {
            StatusMessage = $"Loading {nameof(MainWindowViewModel)}";
            var result = await TripPredictor.LoadTrainingDataAsync();
            
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

        public ITestTripDataPredictionViewModel TestTripDataPredictionViewModel { get; }

        #endregion
    }
}
