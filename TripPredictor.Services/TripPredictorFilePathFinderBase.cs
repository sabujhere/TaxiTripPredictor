using System.IO;
using TripPredictor.Common;

namespace TripPredictor.Services
{
    public abstract class TripPredictorFilePathFinderBase:IFilePathFinder
    {
        public string TestFileName { get; protected set; }

        public string TrainingFileName { get; protected set; }

        public string ModelFileName { get; protected set; }
        public string GetTestFilePath()
        {
            return GetCompleteFilePath(TestFileName);
        }

        public string GetTrainFilePath()
        {
            return GetCompleteFilePath(TrainingFileName);
        }

        public string GetModelPath()
        {
            return GetCompleteFilePath(ModelFileName);
        }

        private string GetCompleteFilePath(string fileName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", fileName);
        }
    }
}
