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

                // STEP 1: Common data loading configuration
                IDataView baseTrainingDataView = _mlContext.Data.LoadFromTextFile<TripData>(filePath, hasHeader: true, separatorChar: ',');

                //Sample code of removing extreme data like "outliers" for FareAmounts higher than $150 and lower than $1 which can be error-data 
                //var cnt = baseTrainingDataView.GetColumn<float>(nameof(TripData.FareAmount)).Count();
               
                //var cnt2 = trainingDataView.GetColumn<float>(nameof(TripData.FareAmount)).Count();

                // STEP 2: Common data process configuration with pipeline data transformations
                var dataProcessPipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TripData.TripTime))
                    //.Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PUTimeEncoded", inputColumnName: nameof(TripData.PUTime)))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PULocationEncoded", inputColumnName: nameof(TripData.PULocationID)))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DOLocationIDEncoded", inputColumnName: nameof(TripData.DOLocationID)))
                    //.Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "TripDistanceEncoded", inputColumnName: nameof(TripData.TripDistance)))
                    //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.VendorId)))
                    //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.PUTime)))
                    //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.PULocationID)))
                    //.Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.DOLocationID)))
                    .Append(_mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TripData.TripDistance)))
                    .Append(_mlContext.Transforms.Concatenate("Features", "PULocationEncoded", "DOLocationIDEncoded",
                        /*nameof(TripData.PUTime),*///nameof(TripData.PULocationID), nameof(TripData.DOLocationID),
                        nameof(TripData.TripDistance)/*"PUTimeEncoded",*/ /*, "TripDistanceEncoded"*/));

                // STEP 3: Set the training algorithm, then create and config the modelBuilder - Selected Trainer (SDCA Regression algorithm)                            
                var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
                var trainingPipeline = dataProcessPipeline.Append(trainer);
                IDataView trainingDataView = _mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(TripData.TripTime));

                _modelPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", "TaxiFareModel.zip");
                //File.Exists(_modelPath)
                // STEP 4: Train the model fitting to the DataSet
                //The pipeline is trained on the dataset that has been loaded and transformed.
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
            var taxiTripSample = new TripData()
            {
                VendorId = "1",
                PUTime = Convert.ToDateTime("2017-01-01 09:13:16"),
                TripDistance = 6.1f,
                PULocationID = "24",
                DOLocationID = "235",
                TripTime = 0// To predict. Actual/Observed = 15.5
            };
            ITransformer trainedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            var predEngine = _mlContext.Model.CreatePredictionEngine<TripData, PredictedValue>(trainedModel);

            //Score
            var score = predEngine.Predict(taxiTripSample).Value;
            var dropOffTime = taxiTripSample.PUTime.AddSeconds(score);
            return score;
        }

        private string GetCompleteFilePath(string fileName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", fileName);
        }
    }
}
