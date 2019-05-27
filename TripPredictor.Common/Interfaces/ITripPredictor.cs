using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPredictor.Common
{
    public interface ITripPredictor
    {
        Task<bool> LoadTrainingDataAsync(string trainingDataFilePath = null);

        EvaluationMetric GetEvaluationMetric();

        double GetPredictedResult(TripData tripData);
    }
}
