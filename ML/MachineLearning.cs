using Microsoft.ML;
using OptimaValue.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ML.Transforms.TimeSeries;
using Microsoft.ML.Data;
using System.Collections.Generic;

using static Microsoft.ML.ForecastingCatalog;
using System.Linq;
using Microsoft.ML.Transforms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OptimaValue.ML;

public class LogData
{
    [LoadColumn(0)] public float NumericValue;
    [LoadColumn(1)] public DateTime ForecastedTime;
    [LoadColumn(2)] public string TagName;
}

public class LogDataDateTimePrediction
{
    public float NumericValue;
    public DateTime ForecastedTime;
    public string TagName;

    public float NumericLogTime;
}


public class LinearRegressionPrediction
{
    [ColumnName("Score")]
    public float NumericValue;
}

public class TimeSeriesPrediction
{
    public float[] ForecastedValues;
    public float[] ConfidenceLowerBound;
    public float[] ConfidenceUpperBound;
    public DateTime ForecastedTime { get; set; }
    public int Index { get; set; }
    public TimeSeriesPrediction()
    {
        ForecastedTime = DateTime.MinValue;
    }
}



public class LogDataToNumeric
{
    public DateTime LogTime;
    public float NumericLogTime;
}

public class LogDataToNumericMapping
{
    public class LogDataToNumeric
    {
        public float NumericValue;
        public DateTime LogTime;
        public string TagName;

        public float NumericLogTime;
    }

    public static void ConvertLogTimeToNumeric(LogData input, LogDataToNumeric output)
    {
        output.NumericValue = input.NumericValue;
        output.LogTime = input.ForecastedTime;
        output.TagName = input.TagName;
        output.NumericLogTime = (float)(input.ForecastedTime - new DateTime(1970, 1, 1)
            .ToLocalTime()).TotalSeconds;
    }

    public static void ConvertNumericToLogTime(LogDataToNumeric input, LogDataDateTimePrediction output)
    {
        output.NumericValue = input.NumericValue;
        output.TagName = input.TagName;
        output.NumericLogTime = input.NumericLogTime;
        output.ForecastedTime = new DateTime(1970, 1, 1).AddSeconds(input.NumericLogTime)
            .ToLocalTime();
    }


    public static IEstimator<ITransformer> CreateTransformer(MLContext context)
    {
        var customMappingTransformerEstimator = context.Transforms.CustomMapping(
            mapAction: (Action<LogData, LogDataToNumeric>)ConvertLogTimeToNumeric,
            contractName: null,
            inputSchemaDefinition: SchemaDefinition.Create(typeof(LogData)),
            outputSchemaDefinition: SchemaDefinition.Create(typeof(LogDataToNumeric))
        );
        return customMappingTransformerEstimator;
    }

    public static IEstimator<ITransformer> CreateDateTimeTransformer(MLContext context)
    {
        var customMappingTransformerEstimator = context.Transforms.CustomMapping(
            mapAction: (Action<LogDataToNumeric, LogDataDateTimePrediction>)ConvertNumericToLogTime,
            contractName: null,
            inputSchemaDefinition: SchemaDefinition.Create(typeof(LogDataToNumeric)),
            outputSchemaDefinition: SchemaDefinition.Create(typeof(LogDataDateTimePrediction))
        );
        return customMappingTransformerEstimator;
    }


}




public class MachineLearning
{
    public static async Task<DataTable> FetchDataAsync(List<int> tagIds, DateTime startTime, DateTime endTime)
    {
        string connectionString = Settings.ConnectionString;
        string selectQuery = $@"
                SELECT L.NumericValue, L.LogTime, T.name as TagName
                FROM McValueLog.dbo.logValues AS L
                INNER JOIN McValueLog.dbo.tagConfig AS T ON L.tag_id = T.id
                WHERE L.tag_id IN ({string.Join(",", tagIds)}) AND L.LogTime BETWEEN @startTime AND @endTime
                ORDER BY L.LogTime ASC
            ";

        DataTable dataTable = new DataTable();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@startTime", startTime);
                selectCommand.Parameters.AddWithValue("@endTime", endTime);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommand);
                await Task.Run(() => dataAdapter.Fill(dataTable));
            }
        }

        return dataTable;
    }


    public static IDataView PreprocessData(MLContext context, DataTable dataTable)
    {
        var data = dataTable.AsEnumerable().Select(row => new LogData
        {
            NumericValue = Convert.ToSingle(row["NumericValue"]),
            ForecastedTime = Convert.ToDateTime(row["LogTime"]),
            TagName = Convert.ToString(row["TagName"])
        }).ToList();

        Debug.WriteLine($"DataTable row count: {dataTable.Rows.Count}");
        Debug.WriteLine($"List<LogData> count: {data.Count}");

        var dataView = context.Data.LoadFromEnumerable(data);

        return dataView;
    }







    public static (IDataView trainingData, IDataView testingData) SplitData(MLContext context, IDataView dataView)
    {
        var rowCount = dataView.GetRowCount().GetValueOrDefault(0);
        var dataEnumerable = context.Data.CreateEnumerable<LogData>(dataView, false);
        var trainRowCount = (int)(rowCount * 0.8);
        var trainEnumerable = dataEnumerable.Take(trainRowCount);
        var testEnumerable = dataEnumerable.Skip(trainRowCount);


        var trainingData = context.Data.LoadFromEnumerable(trainEnumerable);
        var testingData = context.Data.LoadFromEnumerable(testEnumerable);

        return (trainingData, testingData);
    }


    public static List<TimeSeriesPrediction> ForecastValues(MLContext context, IDataView trainingData)
    {
        // Apply custom mapping to trainingData
        var customMappingTransformerEstimator = LogDataToNumericMapping.CreateTransformer(context);
        var transformedTrainingData = customMappingTransformerEstimator.Fit(trainingData).Transform(trainingData);

        // Get the row count of trainingData
        var trainingDataEnumerable = context.Data.CreateEnumerable<LogData>(trainingData, false);
        long trainingDataRowCount = trainingDataEnumerable.Count();

        // Get the row count of transformedTrainingData
        var transformedTrainingDataEnumerable = context.Data.CreateEnumerable<LogDataToNumericMapping.LogDataToNumeric>(transformedTrainingData, false);
        long transformedTrainingDataRowCount = transformedTrainingDataEnumerable.Count();

        Debug.WriteLine($"TrainingData row count: {trainingDataRowCount}");
        Debug.WriteLine($"TransformedTrainingData row count: {transformedTrainingDataRowCount}");


        int minWindowSize = 2; // Set a minimum window size
        long rowCount = trainingDataEnumerable.Count();
        if (rowCount < 2)
        {
            throw new InvalidOperationException("Not enough data for forecasting. The training data must have at least 2 rows.");
        }

        // Calculate windowSize as 10% of rowCount or minWindowSize, whichever is larger
        int windowSize = Math.Max(minWindowSize, (int)(rowCount * 0.1));

        // Ensure windowSize is smaller than the total number of rows in the training data
        windowSize = Math.Min(windowSize, (int)rowCount - 1);

        // Ensure windowSize is at least 2 and less than the series length
        windowSize = Math.Max(2, Math.Min(windowSize, (int)rowCount - 1));


        // Prepare data for forecasting
        var forecastingPipeline = customMappingTransformerEstimator
            .Append(context.Transforms.Categorical.OneHotEncoding("TagNameEncoded", "TagName"))
            .Append(context.Transforms.Concatenate("Features", "NumericLogTime", "TagNameEncoded"))
            .Append(context.Transforms.NormalizeMinMax("Features"))
            .Append(context.Transforms.CopyColumns("Label", "NumericValue"))
            .Append(context.Transforms.CopyColumns("ForecastedTime", "ForecastedTime")) // Change this line
            .Append(context.Transforms.DropColumns("NumericValue"))
            .Append(context.Forecasting.ForecastBySsa("ForecastedValues", "Label"
                , windowSize: windowSize, seriesLength: (int)rowCount, trainSize: windowSize * 2 + 1, horizon: 1
                , confidenceLowerBoundColumn: nameof(TimeSeriesPrediction.ConfidenceLowerBound)
                , confidenceUpperBoundColumn: nameof(TimeSeriesPrediction.ConfidenceUpperBound)))
            .Append(LogDataToNumericMapping.CreateDateTimeTransformer(context));


        // Train the forecasting model
        var forecastingModel = forecastingPipeline.Fit(transformedTrainingData);

        // Make future predictions
        var forecastEngine = forecastingModel.CreateTimeSeriesEngine<LogDataToNumericMapping.LogDataToNumeric, TimeSeriesPrediction>(context);
        var forecasts = forecastEngine.Predict();

        // Extract the predictions and confidence intervals
        List<TimeSeriesPrediction> predictions = new List<TimeSeriesPrediction>();
        for (int i = 0; i < forecasts.ForecastedValues.Length; i++)
        {
            var lastTwoTimestamps = transformedTrainingDataEnumerable
                .OrderByDescending(ld => ld.LogTime)
                .Take(2)
                .Select(ld => ld.LogTime)
                .ToArray();

            var timeDiff = lastTwoTimestamps[0] - lastTwoTimestamps[1];
            var forecastedTime = lastTwoTimestamps[0].Add(timeDiff * (i + 1));

            predictions.Add(new TimeSeriesPrediction
            {
                ForecastedValues = new float[] { forecasts.ForecastedValues[i] },
                ConfidenceLowerBound = new float[] { forecasts.ConfidenceLowerBound[i] },
                ConfidenceUpperBound = new float[] { forecasts.ConfidenceUpperBound[i] },
                ForecastedTime = forecastedTime
            });
        }

        return predictions;
    }


    public static List<LinearRegressionPrediction> AdvancedAnalysis(MLContext context, IDataView trainingData, IDataView testingData)
    {
        // Prepare data for training
        var customMappingTransformerEstimator = LogDataToNumericMapping.CreateTransformer(context);

        var pipeline = customMappingTransformerEstimator
            .Append(context.Transforms.Concatenate("Features", "NumericLogTime")
            .Append(context.Transforms.Categorical.OneHotEncoding("TagNameEncoded", "TagName"))
            .Append(context.Transforms.Concatenate("Features", "Features", "TagNameEncoded"))
            .Append(context.Transforms.NormalizeMinMax("Features"))
            .Append(context.Transforms.CopyColumns("Label", "NumericValue"))
            .Append(context.Regression.Trainers.OnlineGradientDescent())
            .Append(context.Transforms.CopyColumns("Score", "NumericValue"))
            .Append(context.Transforms.DropColumns("NumericValue")));




        // Train the model
        var model = pipeline.Fit(trainingData);


        // Evaluate the model
        var transformedData = model.Transform(testingData);
        var metrics = context.Regression.Evaluate(transformedData);

        Console.WriteLine($"R-squared: {metrics.RSquared:0.##}");
        Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError:#.##}");
        Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError:#.##}");
        Console.WriteLine($"Root Mean Squared Error: {metrics.RootMeanSquaredError:#.##}");

        List<LinearRegressionPrediction> predictions = context.Data.CreateEnumerable<LinearRegressionPrediction>(transformedData, reuseRowObject: false).ToList();

        return predictions;
    }



    public static async Task<(List<LinearRegressionPrediction> linearRegressionPredictions, List<TimeSeriesPrediction> timeSeriesForecastingPredictions)> AnalyzeAndForecastValuesAsync(List<int> tagIds, DateTime startTime, DateTime endTime)
    {
        DataTable dataTable = await FetchDataAsync(tagIds, startTime, endTime);
        Debug.WriteLine($"DataTable row count: {dataTable.Rows.Count}"); // Debug: Check dataTable row count

        MLContext context = new MLContext();
        IDataView dataView = PreprocessData(context, dataTable);
        Debug.WriteLine($"DataView row count: {dataView.GetRowCount().GetValueOrDefault(0)}"); // Debug: Check dataView row count

        (IDataView trainingData, IDataView testingData) = SplitData(context, dataView);
        Debug.WriteLine($"TrainingData row count: {trainingData.GetRowCount().GetValueOrDefault(0)}"); // Debug: Check trainingData row count
        Debug.WriteLine($"TestingData row count: {testingData.GetRowCount().GetValueOrDefault(0)}"); // Debug: Check testingData row count

        // Perform linear regression
        var linearRegressionPredictions = AdvancedAnalysis(context, trainingData, testingData);

        // Perform time series forecasting
        var timeSeriesForecastingPredictions = ForecastValues(context, trainingData);

        return (linearRegressionPredictions, timeSeriesForecastingPredictions);
    }

}
