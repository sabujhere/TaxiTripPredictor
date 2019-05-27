using Prism.Events;
using TripPredictor.Common;
using TripPredictor.UI.Events;

namespace TripPredictor.UI.ViewModel
{
    public class EvaluationMetricViewModel:ViewModelBase, IEvaluationMetricViewModel
    {
        #region Private variables

        private readonly IEventAggregator _eventAggregator;
        private EvaluationMetric _evaluationMetric;

        #endregion

        #region Constructors

        public EvaluationMetricViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<NewEvaluationMetricEvent>()
                .Subscribe(OnNewEvaluationMetricEvent);
        }

        #endregion

        #region Actions

        private void OnNewEvaluationMetricEvent(EvaluationMetric evaluationMetric)
        {
            EvaluationMetric = evaluationMetric;
        }

        #endregion

        #region Public properties

        public EvaluationMetric EvaluationMetric
        {
            get => _evaluationMetric;
            set
            {
                _evaluationMetric = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
