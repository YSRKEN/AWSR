using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AWSR.ViewModels
{
	delegate void drawChart(List<List<double>> drawHist);
	class ResultViewModel : ViewModelBase
	{
		// リストボックスをクリックした際の動作
		public ICommand ClickListBoxCommand { get; private set; }
		// チェックボックスをクリックした際の動作
		public ICommand ClickCheckBoxCommand { get; private set; }

		#region プロパティ
		// 画面左のリストボックス
		List<string> nameList;
		public List<string> NameList {
			get {
				return nameList;
			}
			set {
				nameList = value;
				NotifyPropertyChanged(nameof(NameList));
			}
		}
		// 画面右のグラフデータ
		List<List<List<double>>> histList;
		public List<List<List<double>>> HistList {
			get {
				return histList;
			}
			set {
				histList = value;
			}
		}
		// nスロット目を有効にするかのフラグ
		ObservableCollection<bool> isEnabledChartData = new ObservableCollection<bool>(new bool[] { false, false, false, false });
		public ObservableCollection<bool> IsEnabledChartData {
			get {
				return isEnabledChartData;
			}
			set {
				isEnabledChartData = value;
				NotifyPropertyChanged(nameof(IsEnabledChartData));
			}
		}
		// nスロット目をチェックするかのフラグ
		ObservableCollection<bool> isCheckedChartData = new ObservableCollection<bool>(new bool[] { false, false, false, false });
		public ObservableCollection<bool> IsCheckedChartData {
			get {
				return isCheckedChartData;
			}
			set {
				isCheckedChartData = value;
				ClickCheckBox();
				NotifyPropertyChanged(nameof(IsCheckedChartData));
			}
		}
		// リストボックスの現在選択しているインデックス
		private int listBoxSelectedIndex;
		public int ListBoxSelectedIndex {
			get {
				return listBoxSelectedIndex;
			}
			set {
				if(listBoxSelectedIndex != value) {
					listBoxSelectedIndex = value;
					ClickListBox();
				}
			}
		}
		public drawChart DrawChart { get; set; }
		#endregion

		#region メソッド
		private void ClickListBox() {
			if (ListBoxSelectedIndex < 0)
				return;
			// 画面下のリストボックスを初期化する
			var drawHist = HistList[ListBoxSelectedIndex];
			for(int i = 0; i < 4; ++i) {
				isEnabledChartData[i] = false;
				isCheckedChartData[i] = false;
			}
			for (int i = 0; i < drawHist.Count; ++i) {
				isEnabledChartData[i] = true;
				isCheckedChartData[i] = true;
			}
			// グラフを描画する
			DrawChart(drawHist);
		}
		private void ClickCheckBox() {
			if (ListBoxSelectedIndex < 0)
				return;
			// グラフを描画する
			var drawHist = HistList[ListBoxSelectedIndex];
			DrawChart(drawHist);
		}
		#endregion

		// コンストラクタ
		public ResultViewModel(List<string> nameList, List<List<List<double>>> histList) {
			// フィールドに初期値を設定する
			NameList = nameList;
			HistList = histList;
			ListBoxSelectedIndex = -1;
			// コマンドを初期化する
			ClickListBoxCommand = new CommandBase(ClickListBox);
			ClickCheckBoxCommand = new CommandBase(ClickCheckBox);
		}
		public void SetDelegate(drawChart drawChart) {
			this.DrawChart = drawChart;
		}
	}
}
