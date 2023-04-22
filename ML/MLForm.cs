using OptimaValue.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue.ML
{
    public partial class MLForm : Form
    {
        public MLForm()
        {
            InitializeComponent();
            PopulateTags();
        }

        private void PopulateTags()
        {
            string connectionString = Settings.ConnectionString;
            string selectQuery = "SELECT id, name FROM McValueLog.dbo.tagConfig WHERE active = 1";

            DataTable tagTable = new DataTable();

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommand);
            dataAdapter.Fill(tagTable);

            clbTags.DataSource = tagTable;
            clbTags.DisplayMember = "name";
            clbTags.ValueMember = "id";
        }

        private async void btnPredict_Click(object sender, EventArgs e)
        {
            List<int> selectedTagIds = new List<int>();
            foreach (DataRowView checkedItem in clbTags.CheckedItems)
            {
                selectedTagIds.Add((int)checkedItem["id"]);
            }

            DateTime startTime = dateTimePickerStart.Value;
            DateTime endTime = dateTimePickerEnd.Value;

            // Perform predictions
            var (linearRegressionPredictions, timeSeriesForecastingPredictions) = await MachineLearning.AnalyzeAndForecastValuesAsync(selectedTagIds, startTime, endTime);

            // Display predictions in the DataGridView
            DataTable predictionTable = new DataTable();
            predictionTable.Columns.Add("Prediction Type", typeof(string));
            predictionTable.Columns.Add("Numeric Value", typeof(float));
            predictionTable.Columns.Add("Lower Bound", typeof(float));
            predictionTable.Columns.Add("Upper Bound", typeof(float));

            foreach (var prediction in linearRegressionPredictions)
            {
                predictionTable.Rows.Add("Linear Regression", prediction.NumericValue, DBNull.Value, DBNull.Value);
            }

            foreach (var prediction in timeSeriesForecastingPredictions)
            {
                predictionTable.Rows.Add("Time Series", prediction.ForecastedValues[0], prediction.ForecastedTime, prediction.ConfidenceUpperBound[0]);
            }

            dgvPredictions.DataSource = predictionTable;
        }
    }
}
