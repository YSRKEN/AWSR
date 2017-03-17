using AWSR.Models;
using AWSR.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using static AWSR.Models.Constant;

namespace AWSR.ViewModels
{
	class MainViewModel : ViewModelBase
	{
		ResultView rv = null;

		#region コマンドに関する処理
		// デッキビルダーの画面を開く処理
		public ICommand OpenDeckBuilderCommand { get; private set; }
		// 自艦隊の情報を表示する処理
		public ICommand ShowFriendFleetInfoCommand { get; private set; }
		// 敵艦隊の情報を表示する処理
		public ICommand ShowEnemyFleetInfoCommand { get; private set; }
		// 制空値を表示する処理
		public ICommand ShowAirValueCommand { get; private set; }
		// 撃墜計算を表示する処理
		public ICommand ShowAntiAirPowerCommand { get; private set; }
		// 対空カットイン可否を表示する処理
		public ICommand ShowCutInTypeCommand { get; private set; }
		// 動的解析を行う処理
		public ICommand RunMonteCarloCommand { get; private set; }
		#endregion

		#region プロパティに関する処理
		// 入力するデッキビルダーのデータ
		string inputDeckBuilderText;
		public string InputDeckBuilderText {
			get {
				return inputDeckBuilderText;
			}
			set {
				if (inputDeckBuilderText == value)
					return;
				// 入力値が変化した際の処理
				inputDeckBuilderText = value;
				NotifyPropertyChanged(nameof(InputDeckBuilderText));
			}
		}
		// 自艦隊の陣容
		int friendFleetType;
		public int FriendFleetType {
			get { return friendFleetType; }
			set { friendFleetType = value; }
		}
		// 自艦隊の陣形
		int friendFleetFormation;
		public int FriendFleetFormation {
			get { return friendFleetFormation; }
			set { friendFleetFormation = value; }
		}
		// 入力する基地航空隊のデータ
		string inputAirBaseText;
		public string InputAirBaseText {
			get {
				return inputAirBaseText;
			}
			set {
				if (inputAirBaseText == value)
					return;
				// 入力値が変化した際の処理
				inputAirBaseText = value;
				NotifyPropertyChanged(nameof(InputAirBaseText));
			}
		}
		// 入力する敵艦隊のデータ
		string inputEnemyDataText;
		public string InputEnemyDataText {
			get {
				return inputEnemyDataText;
			}
			set {
				if (inputEnemyDataText == value)
					return;
				// 入力値が変化した際の処理
				inputEnemyDataText = value;
				NotifyPropertyChanged(nameof(InputEnemyDataText));
			}
		}
		// 敵艦隊の陣容
		int enemyFleetType;
		public int EnemyFleetType {
			get { return enemyFleetType; }
			set { enemyFleetType = value; NotifyPropertyChanged(nameof(EnemyFleetType)); }
		}
		// 敵艦隊の陣形
		int enemyFleetFormation;
		public int EnemyFleetFormation {
			get { return enemyFleetFormation; }
			set { enemyFleetFormation = value; NotifyPropertyChanged(nameof(EnemyFleetFormation)); }
		}
		// 反復回数
		int simulationSizeIndex;
		public int SimulationSizeIndex {
			get { return simulationSizeIndex; }
			set { simulationSizeIndex = value; NotifyPropertyChanged(nameof(SimulationSizeIndex)); }
		}
		#endregion

		#region メソッドに関する処理
		// 自艦隊のデータを作成する
		private Fleet FriendFleet(string inputDeckBuilderText) {
			// とりあえず読み込む
			var friendFleet = DeckBuilder.ToFleet(inputDeckBuilderText);
			// 陣形を設定する
			friendFleet.Formation = (Formation)friendFleetFormation;
			// 艦隊の陣容を設定する
			switch (friendFleetType) {
			case 0:
				// 普段は何もしないが、第3艦隊以降が存在する際は切り捨てる
				if(friendFleet.Unit.Count >= 3)
					friendFleet.Unit.RemoveRange(2, friendFleet.Unit.Count - 2);
				break;
			case 1:
				// 通常艦隊なので、最初の艦隊以外は切り捨てる
				friendFleet.Unit.RemoveRange(1, friendFleet.Unit.Count - 1);
				break;
			case 2:
				// 連合艦隊なので、最初から2つ分の艦隊以外は切り捨てる
				// 通常艦隊のデータを入れると例外が発生することに注意
				friendFleet.Unit.RemoveRange(2, friendFleet.Unit.Count - 2);
				break;
			}
			return friendFleet;
		}
		// 敵艦隊のデータを作成する
		private Fleet EnemyFleet(string inputEnemyDataText) {
			// とりあえず読み込む
			var enemyFleet = EnemyData.ToFleet(inputEnemyDataText);
			// 陣形を設定する
			if (enemyFleetFormation != 0)
				enemyFleet.Formation = (Formation)(enemyFleetFormation - 1);
			// 艦隊の陣容を設定する
			switch (enemyFleetType) {
			case 0:
				// 何もしない
				break;
			case 1:
				// 通常艦隊なので、最初の艦隊以外は切り捨てる
				enemyFleet.Unit.RemoveRange(1, enemyFleet.Unit.Count - 1);
				break;
			case 2:
				// 連合艦隊なので、最初から2つ分の艦隊以外は切り捨てる
				// 通常艦隊のデータを入れると例外が発生することに注意
				enemyFleet.Unit.RemoveRange(2, enemyFleet.Unit.Count - 2);
				break;
			}
			return enemyFleet;
		}
		// デッキビルダーの画面を開く処理
		private void OpenDeckBuilder() {
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				System.Diagnostics.Process.Start($"http://kancolle-calc.net/deckbuilder.html?predeck={friendFleet.ToDeckBuilderText()}");
			}
			catch {
				System.Diagnostics.Process.Start("http://kancolle-calc.net/deckbuilder.html");
			}
		}
		// 自艦隊の情報を表示する処理
		private void ShowFriendFleetInfo() {
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				MessageBox.Show($"【自艦隊】\n{friendFleet.InfoText()}", "航空戦シミュレーションR");
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "航空戦シミュレーションR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		// 敵艦隊の情報を表示する処理
		private void ShowEnemyFleetInfo() {
			try {
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				MessageBox.Show($"【敵艦隊】\n{enemyFleet.InfoText()}", "航空戦シミュレーションR");
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "航空戦シミュレーションR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		// 制空値を表示する処理
		private void ShowAirValue() {
			string output = "【制空計算】\n";
			// 仮読み込み
			int unitCount = 1;
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				unitCount = UnitCount(friendFleet.Unit.Count, enemyFleet.Unit.Count);
			}catch {}
			// 自艦隊
			output += "自艦隊：";
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				output += friendFleet.AirValue(unitCount).ToString();
			}
			catch{
				output += "(入力データに誤りがあります)";
			}
			output += "\n";
			// 敵艦隊
			output += "敵艦隊：";
			try {
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				output += enemyFleet.AirValue(unitCount).ToString();
			}
			catch {
				output += "(入力データに誤りがあります)";
			}
			// 表示
			MessageBox.Show(output, "航空戦シミュレーションR");
		}
		// 撃墜計算を表示する処理
		private void ShowAntiAirPower() {
			string output = "【撃墜計算】\n";
			// 仮読み込み
			int unitCount = 1;
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				unitCount = UnitCount(friendFleet.Unit.Count, enemyFleet.Unit.Count);
			}
			catch { }
			// 自艦隊
			output += "自艦隊：\n";
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				output += friendFleet.AntiAirText();
			}
			catch {
				output += "(入力データに誤りがあります)";
			}
			output += "\n";
			// 敵艦隊
			output += "敵艦隊：\n";
			try {
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				output += enemyFleet.AntiAirText();
			}
			catch {
				output += "(入力データに誤りがあります)";
			}
			// 表示
			MessageBox.Show(output, "航空戦シミュレーションR");
		}
		// 対空カットイン可否を表示する処理
		private void ShowCutInType() {
			string output = "【対空カットイン可否】\n";
			// 自艦隊
			output += "自艦隊：\n";
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				output += friendFleet.CutInText();
			}
			catch {
				output += "(入力データに誤りがあります)";
			}
			output += "\n";
			// 敵艦隊
			output += "敵艦隊：\n";
			try {
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				output += enemyFleet.CutInText();
			}
			catch {
				output += "(入力データに誤りがあります)";
			}
			// 表示
			MessageBox.Show(output, "航空戦シミュレーションR");
		}
		// 動的解析を行う処理
		private void RunMonteCarlo() {
			string output = "";
			//try {
				// 艦隊を読み込み
				var friendFleet = FriendFleet(InputDeckBuilderText);
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				// 時間を記録する
				var sw = new System.Diagnostics.Stopwatch();
				sw.Start();
				// モンテカルロシミュレーションを行う
				var simulationSize = new int[]{ 10000, 100000, 1000000, 10000000, 100000000};
				output = Simulator.MonteCarlo(friendFleet, enemyFleet, simulationSize[SimulationSizeIndex]);
				// 先頭に計算時間を追加する
				sw.Stop();
				output = $"経過時間：{Math.Round(sw.Elapsed.TotalSeconds, 1)}秒\n" + output;
				// 結果を表示する
				rv?.Close();
				rv = new ResultView();
				List<string> nameList;
				List<List<List<double>>> histList;
				Simulator.ResultData(friendFleet, enemyFleet, simulationSize[SimulationSizeIndex], out nameList, out histList);
				var rvm = new ResultViewModel(nameList, histList);
				rv.DataContext = rvm;
				rv.Show();
			/*}
			catch {
				output = "自艦隊 or 敵艦隊が正常に読み込めませんでした.";
			}*/
			// 表示
			MessageBox.Show(output, "航空戦シミュレーションR");
		}
		#endregion

		// コンストラクタ
		public MainViewModel() {
			// フィールドに初期値を設定する
			InputDeckBuilderText = "{\"version\":4,\"f1\":{\"s1\":{\"id\":\"330\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":122,\"rf\":\"10\"},\"i2\":{\"id\":122,\"rf\":\"10\"},\"i3\":{\"id\":106,\"rf\":\"10\"}}},\"s2\":{\"id\":\"346\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":122,\"rf\":\"10\"},\"i2\":{\"id\":122,\"rf\":\"10\"},\"i3\":{\"id\":106,\"rf\":\"10\"}}},\"s3\":{\"id\":\"357\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":122,\"rf\":\"10\"},\"i2\":{\"id\":122,\"rf\":\"10\"},\"i3\":{\"id\":106,\"rf\":\"10\"}}},\"s4\":{\"id\":\"428\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":135,\"rf\":\"10\"},\"i2\":{\"id\":173,\"rf\":0},\"i3\":{\"id\":124,\"rf\":0},\"i4\":{\"id\":135,\"rf\":\"10\"}}},\"s5\":{\"id\":\"278\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":191,\"rf\":0},\"i2\":{\"id\":53,\"rf\":0,\"mas\":7},\"i3\":{\"id\":157,\"rf\":0,\"mas\":7},\"i4\":{\"id\":189,\"rf\":0,\"mas\":7}}},\"s6\":{\"id\":\"461\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":191,\"rf\":0},\"i2\":{\"id\":110,\"rf\":0,\"mas\":7},\"i3\":{\"id\":56,\"rf\":0,\"mas\":7},\"i4\":{\"id\":110,\"rf\":0,\"mas\":7}}}}}";
			FriendFleetType = 0;
			FriendFleetFormation = 0;
			InputAirBaseText = "";
			InputEnemyDataText = "{\n\t\"formation\": \"circle\",\n	\"fleet\": [\n\t\t[544,544,528,554,515,515]\n\t]\n}\n";
			EnemyFleetType = 0;
			EnemyFleetFormation = 0;
			SimulationSizeIndex = 0;
			// コマンドを登録する
			OpenDeckBuilderCommand = new CommandBase(OpenDeckBuilder);
			ShowFriendFleetInfoCommand = new CommandBase(ShowFriendFleetInfo);
			ShowEnemyFleetInfoCommand = new CommandBase(ShowEnemyFleetInfo);
			ShowAirValueCommand = new CommandBase(ShowAirValue);
			ShowAntiAirPowerCommand = new CommandBase(ShowAntiAirPower);
			ShowCutInTypeCommand = new CommandBase(ShowCutInType);
			RunMonteCarloCommand = new CommandBase(RunMonteCarlo);
		}
	}
}
