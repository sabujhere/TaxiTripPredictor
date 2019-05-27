using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPredictor.Common
{
    public interface IFilePathFinder
    {
        string GetTestFilePath();
        string GetTrainFilePath();
        string GetModelPath();
    }
}
