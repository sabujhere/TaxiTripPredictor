using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using TripPredictor.Common;
using TripPredictor.Common.Interfaces;

namespace TripPredictor.Services
{
    //class TripTimePipeLineBuilderImpl: IPipeLineBuilder
    //{
    //    private MLContext _mlContext;
        
    //    public MLContext Get()
    //    {
    //        if (_mlContext != null)
    //            return _mlContext;
    //        _mlContext = new MLContext(seed: 0);
    //        var dataProcessPipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TripData.TripTime))
    //                 //.Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PUTimeEncoded", inputColumnName: nameof(TripData.PUTime)))
    //                 .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PULocationEncoded", inputColumnName: nameof(TripData.PULocationID)))
    //                 .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DOLocationIDEncoded", inputColumnName: nameof(TripData.DOLocationID)))
    //                 //.Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "TripDistanceEncoded", inputColumnName: nameof(TripData.TripDistance)))
    //                 //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.VendorId)))
    //                 //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.PUTime)))
    //                 //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.PULocationID)))
    //                 //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.DOLocationID)))
    //                 .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripDistance)))
    //                 .Append(_mlContext.Transforms.Concatenate("Features", "PULocationEncoded", "DOLocationIDEncoded",
    //                     /*nameof(TripData.PUTime),*///nameof(TripData.PULocationID), nameof(TripData.DOLocationID),
    //                     nameof(TripData.TripDistance)/*"PUTimeEncoded",*/ /*, "TripDistanceEncoded"*/));

    //        // STEP 3: Set the training algorithm, then create and config the modelBuilder - Selected Trainer (SDCA Regression algorithm)                            
    //        var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
    //        var trainingPipeline = dataProcessPipeline.Append(trainer);
    //    }
    //}
}
