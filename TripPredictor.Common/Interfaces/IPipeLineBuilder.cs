using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;

namespace TripPredictor.Common.Interfaces
{
    public interface IPipeLineBuilder
    {
        MLContext Get();
    }
}
