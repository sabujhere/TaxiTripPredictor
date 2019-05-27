using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TripPredictor.Common;

namespace TripPredictor.UI.Events
{
    public class NewEvaluationMetricEvent : PubSubEvent<EvaluationMetric>
    {
    }

    public class PredictorUpdatedEvent : PubSubEvent<ITripPredictor>
    {

    }
    public class StatusEvent : PubSubEvent<string>
    {

    }
}
