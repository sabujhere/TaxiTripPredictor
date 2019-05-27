using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using TripPredictor.Common;
using TripPredictor.Common.Interfaces;

namespace TripPredictor.Services
{
    public class TripFarePredictorImpl:ITripPredictor
    {
        #region Private varialbles

        private MLContext _mlContext;
        private TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> _trainedModel;
        private readonly IFilePathFinder _filePathFinder;

        #endregion

        #region Constructor

        public TripFarePredictorImpl(IFilePathFinder filePathFinder)
        {
            _filePathFinder = filePathFinder;
        }

        #endregion

        #region Implementing ITripPredictor

        public async Task<bool> LoadTrainingDataAsync(string trainingDataFilePath = null)
        {
            try
            {
                if (trainingDataFilePath == null)
                    trainingDataFilePath = _filePathFinder.GetTrainFilePath();

                if (!File.Exists(trainingDataFilePath))
                {
                    //TODO: Log this. Added event aggregation
                    return false;
                }

                await Task.Run(() =>
                {
                    _mlContext = new MLContext(seed: 0);
                    IDataView baseTrainingDataView = _mlContext.Data.LoadFromTextFile<TripData>(trainingDataFilePath, hasHeader: true, separatorChar: ',');
                    IDataView trainingDataView = _mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(TripData.FareAmount), lowerBound: 1, upperBound: 150);

                    var dataProcessPipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TripData.FareAmount))
                                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: nameof(TripData.VendorId)))
                                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: nameof(TripData.RateCode)))
                                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: nameof(TripData.PaymentType)))
                                    .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.PassengerCount)))
                                    .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripTime)))
                                    .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripDistance)))
                                    .Append(_mlContext.Transforms.Concatenate("Features", "VendorIdEncoded",
                                        "RateCodeEncoded", "PaymentTypeEncoded", nameof(TripData.PassengerCount)
                                    , nameof(TripData.TripTime), nameof(TripData.TripDistance)));

                    var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
                    var trainingPipeline = dataProcessPipeline.Append(trainer);
                    _trainedModel = trainingPipeline.Fit(trainingDataView);

                    _mlContext.Model.Save(_trainedModel, trainingDataView.Schema, _filePathFinder.GetModelPath());
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return true;
        }

        public EvaluationMetric GetEvaluationMetric()
        {
            //TODO: Put this in base class
            if (_mlContext == null)
                return EvaluationMetric.DefaultEvaluationMetric;

            var filePath = _filePathFinder.GetTestFilePath();
            if (!File.Exists(filePath))
            {
                //TODO: Log this.
                return EvaluationMetric.DefaultEvaluationMetric; ;
            }
            IDataView testDataView = _mlContext.Data.LoadFromTextFile<TripData>(filePath, hasHeader: true, separatorChar: ',');

            IDataView predictions = _trainedModel.Transform(testDataView);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");
            return new EvaluationMetric(_trainedModel.ToString(), metrics);
        }

        public double GetPredictedResult(TripData tripData)
        {
            ITransformer trainedModel = _mlContext.Model.Load(_filePathFinder.GetModelPath(), out var modelInputSchema);
            var predEngine = _mlContext.Model.CreatePredictionEngine<TripData, PredictedValue>(trainedModel);
            return predEngine.Predict(tripData).Value;
        }

        #endregion

    }
}
