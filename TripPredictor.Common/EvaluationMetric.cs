using Microsoft.ML.Data;

namespace TripPredictor.Common
{
    public class EvaluationMetric
    {
        public EvaluationMetric(string name, RegressionMetrics metrics)
        {
            Name = name;
            RSquared = metrics.RSquared;
            MeanAbsoluteError = metrics.MeanAbsoluteError;
            MeanSquaredError = metrics.MeanSquaredError;
            RootMeanSquaredError = metrics.RootMeanSquaredError;
            LossFunction = metrics.LossFunction;
        }

        private EvaluationMetric()
        {

        }

        public string Name { get; private set; }

        public double LossFunction { get; private set; }

        public double RSquared { get; private set; }

        public double MeanAbsoluteError { get; private set; }

        public double MeanSquaredError { get; private set; }

        public double RootMeanSquaredError { get; private set; }


        public static readonly EvaluationMetric DefaultEvaluationMetric = new EvaluationMetric()
        {
            Name = "Default",
            RSquared = 0.0,
            MeanAbsoluteError = 0.0,
            MeanSquaredError = 0.0,
            RootMeanSquaredError = 0.0,
            LossFunction = 0.0
        };


        protected bool Equals(EvaluationMetric other)
        {
            return string.Equals(Name, other.Name) && LossFunction.Equals(other.LossFunction) && 
                   RSquared.Equals(other.RSquared) && MeanAbsoluteError.Equals(other.MeanAbsoluteError) && 
                   MeanSquaredError.Equals(other.MeanSquaredError) && RootMeanSquaredError.Equals(other.RootMeanSquaredError);

        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EvaluationMetric) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LossFunction.GetHashCode();
                hashCode = (hashCode * 397) ^ RSquared.GetHashCode();
                hashCode = (hashCode * 397) ^ MeanAbsoluteError.GetHashCode();
                hashCode = (hashCode * 397) ^ MeanSquaredError.GetHashCode();
                hashCode = (hashCode * 397) ^ RootMeanSquaredError.GetHashCode();
                return hashCode;
            }
        }
    }
}
