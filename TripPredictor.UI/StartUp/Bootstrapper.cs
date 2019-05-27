using Autofac;
using Prism.Events;
using TripPredictor.Common;
using TripPredictor.Services;
using TripPredictor.UI.ViewModel;
using TripPredictor.UI.Views;

namespace TripPredictor.UI.StartUp
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            bool isLoadingTripFarePredictor = true;
            var builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            
            builder.RegisterType<MainWindowViewModel>().AsSelf();
            builder.RegisterType<EvaluationMetricViewModel>().As<IEvaluationMetricViewModel>();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<EvaluationMetricView>().AsSelf();

            builder.RegisterType<TestTripTimeView>().AsSelf();

            if (isLoadingTripFarePredictor)
            {
                builder.RegisterType<TripFarePredictorImpl>().As<ITripPredictor>().SingleInstance();
                builder.RegisterType<TripFarePredictionViewModel>().As<ITripDataPredictionViewModel>();
                builder.RegisterType<TestTripFareView>().AsSelf();
            }
            else
            {
                builder.RegisterType<TripTimePredictionViewModel>().As<ITripPredictor>().SingleInstance();
                builder.RegisterType<TripTimePredictionViewModel>().As<ITripDataPredictionViewModel>();
                builder.RegisterType<TestTripTimeView>().AsSelf();
            }

            return builder.Build();
        }
    }
}
