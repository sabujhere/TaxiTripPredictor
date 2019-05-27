using System.Threading.Tasks;

namespace TripPredictor.Common.Interfaces
{
    public interface ITripPredictor
    {
        Task<bool> LoadTrainingDataAsync(string trainingDataFilePath = null);

        EvaluationMetric GetEvaluationMetric();

        double GetPredictedResult(TripData tripData);
    }
}
