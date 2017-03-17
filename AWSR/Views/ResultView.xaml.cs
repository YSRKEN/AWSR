using AWSR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace AWSR.Views
{
	/// <summary>
	/// ResultView.xaml の相互作用ロジック
	/// </summary>
	public partial class ResultView : Window
	{
		// グラフを書き換える
		private void NameList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			// 初期化前は何もしない
			if (ProbChart == null)
				return;
			if (NameListBox.SelectedIndex < 0)
				return;
			var bindData = DataContext as ResultViewModel;
			var drawHist = bindData.HistList[NameListBox.SelectedIndex];
			// グラフエリアを初期化する
			ProbChart.Series.Clear();
			ProbChart.Legends.Clear();
			// グラフエリアの罫線色を設定する
			ProbChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
			ProbChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
			int maximumX = 0;
			for (int i = 0; i < drawHist.Count; ++i) {
				var series = new Series();
				series.Name = $"{i + 1}スロット目";
				series.ChartType = SeriesChartType.Line;
				series.BorderWidth = 2;
				double sum = 0.0;
				for (int j = 0; j < drawHist[i].Count; ++j) {
					sum += drawHist[i][j];
					series.Points.AddXY(j, sum);
					maximumX = Math.Max(maximumX, j);
				}
				ProbChart.Series.Add(series);
				var legend = new Legend();
				legend.DockedToChartArea = "ChartArea";
				legend.Alignment = StringAlignment.Far;
				ProbChart.Legends.Add(legend);
			}
			// スケールを調整する
			{
				var axisX = ProbChart.ChartAreas[0].AxisX;
				axisX.Title = "残存数";
				axisX.Minimum = 0.0;
				axisX.Maximum = (maximumX + 9) / 10 * 10;
				axisX.Interval = 5.0;
			}
			{
				var axisY = ProbChart.ChartAreas[0].AxisY;
				axisY.Title = "積み上げ比率[％]";
				axisY.Minimum = 0.0;
				axisY.Maximum = 110.0;
				axisY.Interval = 10.0;
			}
		}
		// コンストラクタ
		public ResultView() {
			InitializeComponent();
		}
	}
}
