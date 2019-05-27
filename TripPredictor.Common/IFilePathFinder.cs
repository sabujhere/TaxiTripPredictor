namespace TripPredictor.Common
{
    public interface IFilePathFinder
    {
        string GetTestFilePath();
        string GetTrainFilePath();
        string GetModelPath();
    }
}
