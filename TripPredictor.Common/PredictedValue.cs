using Microsoft.ML.Data;

namespace TripPredictor.Common
{
    public class PredictedValue
    {
        [ColumnName("Score")]
        public float Value { get; set; }
    }
}
