using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace AWSR.ViewModels
{
	internal delegate void copyChart();
	internal delegate void drawChart(List<List<double>> drawHist);
	internal delegate void sendFriend(string str);
	internal delegate void sendLandBase(string str);
	internal delegate void sendEnemy(string str);

	internal class ResultViewModel : ViewModelBase
	{
		// リストボックスをクリックした際の動作
		public ICommand ClickListBoxCommand { get; }
		// チェックボックスをクリックした際の動作
		public ICommand ClickCheckBoxCommand { get; }
		// テキストをコピー
		public ICommand CopyTextCommand { get; }
		// 画像をコピー
		public ICommand CopyPictureCommand { get; }
		// 自艦隊・基地航空隊・敵艦隊・解析結果をコピー
		public ICommand CopyFriendCommand   { get; }
		public ICommand CopyLandBaseCommand { get; }
		public ICommand CopyEnemyCommand    { get; }
		public ICommand ShowResultCommand   { get; }
		// 自艦隊・基地航空隊・敵艦隊をメイン画面に転送
		public ICommand SendFriendCommand   { get; }
		public ICommand SendLandBaseCommand { get; }
		public ICommand SendEnemyCommand    { get; }

		#region プロパティ
		// 画面左のリストボックス
		private List<string> nameList;
		public List<string> NameList {
			get { return nameList; }

			set {
				nameList = value;
				NotifyPropertyChanged(nameof(NameList));
			}
		}
		// 画面右のグラフデータ
		private List<List<List<double>>> histList;
		public List<List<List<double>>> HistList {
			get { return histList; }

			set { histList = value; }
		}
		// nスロット目を有効にするかのフラグ
		private ObservableCollection<bool> isEnabledChartData = new ObservableCollection<bool>(new bool[] { false, false, false, false });
		public ObservableCollection<bool> IsEnabledChartData {
			get { return isEnabledChartData; }

			set {
				isEnabledChartData = value;
				NotifyPropertyChanged(nameof(IsEnabledChartData));
			}
		}
		// nスロット目をチェックするかのフラグ
		private ObservableCollection<bool> isCheckedChartData = new ObservableCollection<bool>(new bool[] { false, false, false, false });
		public ObservableCollection<bool> IsCheckedChartData {
			get { return isCheckedChartData; }

			set {
				isCheckedChartData = value;
				ClickCheckBox();
				NotifyPropertyChanged(nameof(IsCheckedChartData));
			}
		}
		// リストボックスの現在選択しているインデックス
		private int listBoxSelectedIndex;
		public int ListBoxSelectedIndex {
			get { return listBoxSelectedIndex; }

			set {
				if (listBoxSelectedIndex != value) {
					listBoxSelectedIndex = value;
					ClickListBox();
				}
			}
		}
		// チャート操作用デリゲート
		public drawChart DrawChart { get; private set; }
		public copyChart CopyChart { get; private set; }
		public sendFriend DeleGateSendFriend { get; private set; }
		public sendLandBase DeleGateSendLandBase { get; private set; }
		public sendEnemy DeleGateSendEnemy { get; private set; }
		// 入力及び出力テキスト
		public string InputDeckBuilderText { get; }
		public string InputAirBaseText { get; }
		public string InputEnemyDataText { get; }
		public string ResultText { get; }
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

		private void CopyText() {
			if (ListBoxSelectedIndex < 0)
				return;
			try {
				string output = $"{NameList[ListBoxSelectedIndex]}\n残存数";
				var drawHist = HistList[ListBoxSelectedIndex];
				for(int i = 0; i < drawHist.Count; ++i) {
					output += $",{i + 1}スロット目";
				}
				output += "\n";
				var temp = Enumerable.Repeat(0.0, drawHist.Count).ToList();
				for(int n = 0; ; ++n) {
					bool getDataFlg = false;
					for(int i = 0; i < drawHist.Count; ++i) {
						if(drawHist[i].Count > n) {
							temp[i] += drawHist[i][n];
							getDataFlg = true;
						}
					}
					if (!getDataFlg)
						break;
					output += $"{n}";
					for (int i = 0; i < drawHist.Count; ++i) {
						output += $",{temp[i]}";
					}
					output += "\n";
				}
				Clipboard.SetText(output);
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void CopyPicture() {
			if (ListBoxSelectedIndex < 0)
				return;
			try {
				CopyChart();
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void CopyFriend() {
			try {
				Clipboard.SetText(InputDeckBuilderText);
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void CopyLandBase() {
			try {
				Clipboard.SetText(InputAirBaseText);
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void CopyEnemy() {
			try {
				Clipboard.SetText(InputEnemyDataText);
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void ShowResult() {
			MessageBox.Show(ResultText, "AWSR", MessageBoxButton.OK);
		}

		private void SendFriend() {
			DeleGateSendFriend(InputDeckBuilderText);
		}

		private void SendLandBase() {
			DeleGateSendLandBase(InputAirBaseText);
		}

		private void SendEnemy() {
			DeleGateSendEnemy(InputEnemyDataText);
		}
		#endregion

		// コンストラクタ
		public ResultViewModel(
			List<string> nameList,
			List<List<List<double>>> histList,
			string inputDeckBuilderText,
			string inputAirBaseText,
			string inputEnemyDataText,
			string resultText) {
			// フィールドに初期値を設定する
			NameList = nameList;
			HistList = histList;
			InputDeckBuilderText = inputDeckBuilderText;
			InputAirBaseText = inputAirBaseText;
			InputEnemyDataText = inputEnemyDataText;
			ResultText = resultText;
			ListBoxSelectedIndex = -1;
			// コマンドを初期化する
			ClickListBoxCommand = new CommandBase(ClickListBox);
			ClickCheckBoxCommand = new CommandBase(ClickCheckBox);
			CopyTextCommand = new CommandBase(CopyText);
			CopyPictureCommand = new CommandBase(CopyPicture);
			CopyFriendCommand   = new CommandBase(CopyFriend);
			CopyLandBaseCommand = new CommandBase(CopyLandBase);
			CopyEnemyCommand    = new CommandBase(CopyEnemy);
			ShowResultCommand   = new CommandBase(ShowResult);
			SendFriendCommand   = new CommandBase(SendFriend);
			SendLandBaseCommand = new CommandBase(SendLandBase);
			SendEnemyCommand    = new CommandBase(SendEnemy);
		}

		public void SetDelegate(
			drawChart drawChart,
			copyChart copyChart,
			sendFriend sendFriend,
			sendLandBase sendLandBase,
			sendEnemy sendEnemy) {
			this.DrawChart = drawChart;
			this.CopyChart = copyChart;
			this.DeleGateSendFriend = sendFriend;
			this.DeleGateSendLandBase = sendLandBase;
			this.DeleGateSendEnemy = sendEnemy;
		}
	}
}
