using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Regression_TaxiFarePrediction.DataStructures;
using TripPredictor.Common;

namespace TripPredictor.Services
{
    public class TripFarePredictorImpl:ITripPredictor
    {
        private MLContext _mlContext;
        private TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> _trainedModel;
        private string _modelPath = Path.Combine(Path.GetTempPath(), "Data");

        public async Task<bool> LoadTrainingDataAsync(string trainingDataFileName = null)
        {
            try
            {
                _modelPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", "TaxiTripFareModel.zip");
                if (trainingDataFileName == null)
                    trainingDataFileName = "Training_yellow_tripdata_2017-01-000.csv";
                var filePath = GetCompleteFilePath(trainingDataFileName);
                if (!File.Exists(filePath))
                {
                    //TODO: Log this. Added event aggregation
                    return false;
                }
                if (_mlContext != null)
                {
                    //Todo Retrain the model
                }
                _mlContext = new MLContext(seed: 0);

                // STEP 1: Common data loading configuration
                IDataView baseTrainingDataView = _mlContext.Data.LoadFromTextFile<TripData>(filePath, hasHeader: true, separatorChar: ',');
                IDataView trainingDataView = _mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(TripData.FareAmount), lowerBound: 1, upperBound: 150);

                // STEP 2: Common data process configuration with pipeline data transformations
                var dataProcessPipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TripData.FareAmount))
                                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: nameof(TripData.VendorId)))
                                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: nameof(TripData.RateCode)))
                                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: nameof(TripData.PaymentType)))
                                .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.PassengerCount)))
                                .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripTime)))
                                .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripDistance)))
                                .Append(_mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PaymentTypeEncoded", nameof(TripData.PassengerCount)
                                , nameof(TripData.TripTime), nameof(TripData.TripDistance)));
                        
                var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
                var trainingPipeline = dataProcessPipeline.Append(trainer);

                
                _trainedModel = await Task.Run(new Func<TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>>(
                    () =>
                    {
                        var test = trainingPipeline.Fit(trainingDataView);

                        return test;
                    }
                ));
                _mlContext.Model.Save(_trainedModel, trainingDataView.Schema, _modelPath);

                var taxiTripSample = new TripData()
                {
                    VendorId = "1",
                    RateCode = "1",
                    PassengerCount = 1,
                    TripTime = 940,
                    TripDistance = 6.1f,
                    PaymentType = "2",
                    FareAmount = 0 // To predict. Actual/Observed = 15.5
                };

                ITransformer trainedModel1 = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

                // Create prediction engine related to the loaded trained model
                var predEngine = _mlContext.Model.CreatePredictionEngine<TripData, PredictedValue>(trainedModel1);

                //Score
                var resultprediction = predEngine.Predict(taxiTripSample);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return true;
        }

        public EvaluationMetric GetEvaluationMetric(string testDataFileName = null)
        {

            //TODO: Put this in base class
            if (_mlContext == null)
                return EvaluationMetric.DefaultEvaluationMetric;
            if (testDataFileName == null)
                testDataFileName = "Test_yellow_tripdata_2017-01-001.csv";
            var filePath = GetCompleteFilePath(testDataFileName);
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
            ITransformer trainedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            var predEngine = _mlContext.Model.CreatePredictionEngine<TripData, PredictedValue>(trainedModel);

            //Score
            return predEngine.Predict(tripData).Value;
        }

        private string GetCompleteFilePath(string fileName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", fileName);
        }
    }
}
