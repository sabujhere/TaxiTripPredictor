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
            var builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<TripTimePredictorImpl>().As<ITripPredictor>().SingleInstance(); 
            builder.RegisterType<MainWindowViewModel>().AsSelf();
            builder.RegisterType<EvaluationMetricViewModel>().As<IEvaluationMetricViewModel>();
            builder.RegisterType<TestTripDataPredictionViewModel>().As<ITestTripDataPredictionViewModel>();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<EvaluationMetricView>().AsSelf();
            builder.RegisterType<TestTripTimeView>().AsSelf();

            return builder.Build();
        }
    }
}
