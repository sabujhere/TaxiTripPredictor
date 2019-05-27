using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using TripPredictor.Common;

namespace TripPredictor.Services
{
    public class TripTimePredictorImpl:ITripPredictor
    {
        private MLContext _mlContext;
        private TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> _trainedModel;
        private string _modelPath = Path.Combine(Path.GetTempPath(), "Data");
        public TripTimePredictorImpl()
        {
            
        }
        public async Task<bool> LoadTrainingDataAsync(string trainingDataFileName = null)
        {
            try
            {
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

                IDataView baseTrainingDataView = _mlContext.Data.LoadFromTextFile<TripData>(filePath, hasHeader: true, separatorChar: ',');
                var dataProcessPipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TripData.TripTime))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PULocationEncoded", inputColumnName: nameof(TripData.PULocationID)))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DOLocationIDEncoded", inputColumnName: nameof(TripData.DOLocationID)))
                    .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripDistance)))
                    .Append(_mlContext.Transforms.Concatenate("Features", "PULocationEncoded", "DOLocationIDEncoded",
                        nameof(TripData.TripDistance)));
                
                var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
                var trainingPipeline = dataProcessPipeline.Append(trainer);
                IDataView trainingDataView = _mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(TripData.TripTime));

                _modelPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", "TaxiTripTimeModel.zip");
                //File.Exists(_modelPath)

                _trainedModel = await Task.Run(new Func<TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>>(
                    () =>
                    {
                        var test = trainingPipeline.Fit(trainingDataView);

                        return test;
                    }
                    ));
                _mlContext.Model.Save(_trainedModel, trainingDataView.Schema, _modelPath);
                
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
