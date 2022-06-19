using LiveCharts.Defaults;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class TrendTag : Form
    {
        private CancellationTokenSource source = new CancellationTokenSource();
        private int tagId;
        private string tagName;
        private DataTable tbl = new();
        private TrendModel trendModel;


        public TrendTag(int _tagId)
        {
            InitializeComponent();
            tagId = _tagId;
            chart.DisableAnimations = true;
            trendModel = trendModel.Create(this, tagId, TimeSpan.FromSeconds(30));
            trendModel.OnValuesUpdated += TrendModel_OnValuesUpdated;
        }

        private void TrendModel_OnValuesUpdated(bool obj)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (chart.AxisX.Count > 0)
                {
                    chart.AxisX.First().MaxValue = trendModel.MaxValueX;
                    chart.AxisX.First().MinValue = trendModel.MinValueX;
                    chart.AxisY.First().MaxValue = trendModel.MaxValueY;
                    chart.AxisY.First().MinValue = trendModel.MinValueY;
                    chart.AxisX.First().Separator = DefaultAxes.CleanSeparator;
                    chart.AxisX.First().LabelFormatter = trendModel.FormatterX;
                    chart.AxisY.First().LabelFormatter = trendModel.FormatterY;
                    chart.AxisY.First().Separator = DefaultAxes.CleanSeparator;

                    var startTime = new DateTime((long)trendModel.MinValueX);
                    var stopTime = new DateTime((long)trendModel.MaxValueX);
                    txtStartTime.Text = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtStopTime.Text = stopTime.ToString("yyyy-MM-dd HH:mm:ss");
                    lblTimeSpan.Text = (stopTime - startTime).ToString();
                    Text = trendModel.TagName;
                }

                if (trendModel.MinValueX < trendModel.MaxValueX)
                {
                    trendModel.LineSeries.Values = trendModel.ChartValuesDateTimePoints;
                    if (chart.Series.Count == 0)
                        chart.Series.Add(trendModel.LineSeries);
                    else
                        chart.Series[0].Values = trendModel.ChartValuesDateTimePoints;
                }

            });
        }


    }
}
