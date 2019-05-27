using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace TripPredictor.Common
{
    public class PredictedValue
    {
        [ColumnName("Score")]
        public float Value { get; set; }
    }
}
