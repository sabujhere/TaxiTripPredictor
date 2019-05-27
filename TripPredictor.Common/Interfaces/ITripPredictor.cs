using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPredictor.Common
{
    public interface ITripPredictor
    {
        Task<bool> LoadTrainingDataAsync(string trainingDataFileName = null);

        EvaluationMetric GetEvaluationMetric(string testDataFileName = null);

        double GetPredictedResult(TripData tripData);
    }
}
