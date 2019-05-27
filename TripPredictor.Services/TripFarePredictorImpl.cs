using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPredictor.Common;

namespace TripPredictor.Services
{
    public class TripFarePredictorImpl:ITripPredictor
    {
        public Task<bool> LoadTrainingDataAsync(string trainingDataFileName = null)
        {
            throw new NotImplementedException();
        }

        public EvaluationMetric GetEvaluationMetric(string testDataFileName = null)
        {
            throw new NotImplementedException();
        }

        public double GetPredictedResult(TripData tripData)
        {
            throw new NotImplementedException();
        }
    }
}
