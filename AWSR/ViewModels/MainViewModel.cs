using AWSR.Models;
using AWSR.Views;
using Microsoft.Win32;
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
		// デッキビルダー形式でコピーする処理
		public ICommand CopyDeckBuilderFormatCommand { get; private set; }
		// 独自形式でコピーする処理
		public ICommand CopyFriendDataFormatCommand { get; private set; }
		// 基地航空隊を読み込む処理
		public ICommand OpenLandBaseFileCommand { get; private set; }
		// 敵艦隊を読み込む処理
		public ICommand OpenEnemyFileCommand { get; private set; }
		// 自艦隊の情報を表示する処理
		public ICommand ShowFriendFleetInfoCommand { get; private set; }
		// 基地航空隊の情報を表示する処理
		public ICommand ShowLandBaseInfoCommand { get; private set; }
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
		// 基地航空隊を使用するか？
		public bool IsLandBaseUse { get; set; }
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
			Fleet friendFleet = null;
			try {
				// デッキビルダー形式と仮定
				friendFleet = DeckBuilder.ToFleet(inputDeckBuilderText);
			}
			catch {
				// 独自形式と仮定
				friendFleet = FriendData.ToFleet(inputDeckBuilderText);
			}
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
		private LandBase LandBaseFleet(string inputAirBaseText) {
			// とりあえず読み込む
			var landBase = LandBaseData.ToLandBase(inputAirBaseText);
			return landBase;
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
		// 
		private void CopyDeckBuilderFormat() {
			Fleet friendFleet = null;
			try {
				friendFleet = FriendFleet(InputDeckBuilderText);
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			try {
				Clipboard.SetText(friendFleet.ToDeckBuilderText());
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		private void CopyFriendDataFormat() {
			Fleet friendFleet = null;
			try {
				friendFleet = FriendFleet(InputDeckBuilderText);
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			try {
				Clipboard.SetText(friendFleet.ToFriendDataText());
			}
			catch {
				MessageBox.Show("クリップボードにコピーできませんでした.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		// 基地航空隊のデータを開く処理
		private void OpenLandBaseFile() {
			var ofd = new OpenFileDialog();
			ofd.FileName = "enemy.bas";
			ofd.Filter = "基地航空隊データファイル(*.bas)|*.bas|すべてのファイル(*.*)|*.*";
			ofd.AddExtension = true;
			if ((bool)ofd.ShowDialog()) {
				try {
					using (var stream = ofd.OpenFile())
					using (var sr = new System.IO.StreamReader(stream))
						InputAirBaseText = sr.ReadToEnd();
				}
				catch {
					MessageBox.Show("基地航空隊データの読み込みに失敗しました.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		// 敵艦隊のデータを開く処理
		private void OpenEnemyFile() {
			var ofd = new OpenFileDialog();
			ofd.FileName = "enemy.enm";
			ofd.Filter = "敵艦隊データファイル(*.enm)|*.enm|敵艦隊データファイル(*.json)|*.json|すべてのファイル(*.*)|*.*";
			ofd.AddExtension = true;
			if ((bool)ofd.ShowDialog()) {
				try {
					using (var stream = ofd.OpenFile())
					using (var sr = new System.IO.StreamReader(stream))
						InputEnemyDataText = sr.ReadToEnd();
				}
				catch {
					MessageBox.Show("敵艦隊データの読み込みに失敗しました.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		// 自艦隊の情報を表示する処理
		private void ShowFriendFleetInfo() {
			try {
				var friendFleet = FriendFleet(InputDeckBuilderText);
				MessageBox.Show($"【自艦隊】\n{friendFleet.InfoText()}", "AWSR");
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		// 基地航空隊の情報を表示する処理
		private void ShowLandBaseInfo() {
			try {
				var landBase = LandBaseFleet(InputAirBaseText);
				MessageBox.Show($"【基地航空隊】\n{landBase.InfoText()}", "AWSR");
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		// 敵艦隊の情報を表示する処理
		private void ShowEnemyFleetInfo() {
			try {
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				MessageBox.Show($"【敵艦隊】\n{enemyFleet.InfoText()}", "AWSR");
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "AWSR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
			// 自艦隊
			output += "基地航空隊：";
			try {
				var landBase = LandBaseFleet(InputAirBaseText);
				output += landBase.AirValueText;
			}
			catch {
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
			MessageBox.Show(output, "AWSR");
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
			MessageBox.Show(output, "AWSR");
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
			MessageBox.Show(output, "AWSR");
		}
		// 動的解析を行う処理
		private void RunMonteCarlo() {
			string output = "";
			try {
				// 艦隊を読み込み
				var friendFleet = FriendFleet(InputDeckBuilderText);
				LandBase landBase = null;
				if (IsLandBaseUse) {
					landBase = LandBaseFleet(InputAirBaseText);
				}
				var enemyFleet = EnemyFleet(InputEnemyDataText);
				// 時間を記録する
				var sw = new System.Diagnostics.Stopwatch();
				sw.Start();
				// モンテカルロシミュレーションを行う
				var simulationSize = new int[]{ 10000, 100000, 1000000, 10000000, 100000000};
				output = Simulator.MonteCarlo(friendFleet, enemyFleet, landBase, simulationSize[SimulationSizeIndex]);
				// 先頭に計算時間を追加する
				sw.Stop();
				output = $"経過時間：{Math.Round(sw.Elapsed.TotalSeconds, 1)}秒\n" + output;
				// 結果を表示する
				List<string> nameList;
				List<List<List<double>>> histList;
				Simulator.ResultData(friendFleet, enemyFleet, landBase, simulationSize[SimulationSizeIndex], out nameList, out histList);
				//rv?.Close();
				var rvm = new ResultViewModel(
					nameList,
					histList,
					InputDeckBuilderText,
					InputAirBaseText,
					InputEnemyDataText,
					output);
				rv = new ResultView();
				rv.DataContext = rvm;
				rvm.SetDelegate(rv.DrawChart, rv.CopyChart);
				rv.Show();
			}
			catch {
				output = "自艦隊 or 敵艦隊が正常に読み込めませんでした.";
			}
			// 表示
			MessageBox.Show(output, "AWSR");
		}
		#endregion

		// コンストラクタ
		public MainViewModel() {
			// フィールドに初期値を設定する
			InputDeckBuilderText = "艦隊,艦番,艦名,レベル,装備1,改修1,熟練1,装備2,改修2,熟練2,装備3,改修3,熟練3,装備4,改修4,熟練4,装備X,改修X,熟練X\n1,1,Iowa改,142,試製46cm連装砲,10,0,16inch三連装砲 Mk.7,10,0,紫雲,0,7,九一式徹甲弾,10,0,応急修理要員,0,0\n1,2,Italia,139,381mm/50 三連装砲,6,0,381mm/50 三連装砲,5,0,紫雲,0,7,一式徹甲弾,6,0,応急修理要員,0,0\n1,3,Roma改,140,381mm/50 三連装砲,5,0,381mm/50 三連装砲,6,0,紫雲,0,7,二式水戦改,0,7,応急修理要員,0,0\n1,4,Bismarck drei,153,38cm連装砲改,10,0,16inch三連装砲 Mk.7,6,0,紫雲,0,7,九一式徹甲弾,10,0,応急修理要員,0,0\n1,5,千歳航改二,133,零式艦戦53型(岩本隊),10,7,烈風改,0,7,烈風改,0,7,零式艦戦52型(熟練),10,7,応急修理要員,0,0\n1,6,千代田航改二,134,烈風(六〇一空),0,7,烈風改,0,7,零式艦戦52型(熟練),10,7,彩雲,0,7,応急修理要員,0,0\n2,1,綾波改二,125,61cm五連装(酸素)魚雷,4,0,61cm五連装(酸素)魚雷,6,0,探照灯,10,0,,,,応急修理要員,0,0\n2,2,摩耶改二,128,90mm単装高角砲,10,0,13号対空電探改,10,0,61cm五連装(酸素)魚雷,4,0,61cm五連装(酸素)魚雷,0,0,25mm三連装機銃 集中配備,8,0\n2,3,夕立改二,147,試製61cm六連装(酸素)魚雷,7,0,試製61cm六連装(酸素)魚雷,10,0,照明弾,0,0,,,,応急修理要員,0,0\n2,4,阿武隈改二,138,61cm五連装(酸素)魚雷,4,0,61cm五連装(酸素)魚雷,4,0,甲標的,0,0,,,,応急修理要員,0,0\n2,5,Prinz Eugen改,150,20.3cm(3号)連装砲,10,0,61cm五連装(酸素)魚雷,7,0,九八式水上偵察機(夜偵),10,7,61cm五連装(酸素)魚雷,6,0,応急修理要員,0,0\n2,6,北上改二,141,61cm五連装(酸素)魚雷,0,0,61cm五連装(酸素)魚雷,0,0,甲標的,0,0,,,,応急修理要員,0,0\n";
			FriendFleetType = 0;
			FriendFleetFormation = 0;
			InputAirBaseText = "航空戦回数,装備1,改修1,熟練1,装備2,改修2,熟練2,装備3,改修3,熟練3,装備4,改修4,熟練4\n2,零式艦戦52型(熟練),10,7,一式陸攻 二二型甲,0,0,一式陸攻 三四型,0,0,一式陸攻 三四型,0,0\n2,零式艦戦52型(熟練),10,7,銀河,0,0,一式陸攻 二二型甲,0,0,一式陸攻(野中隊),0,0\n";
			IsLandBaseUse = false;
			InputEnemyDataText = "輪形陣\n2\n深海双子棲姫-3,空母棲姫(艦載機赤),空母棲姫(艦載機赤),戦艦タ級flagship,戦艦タ級flagship,補給ワ級flagship\n軽巡ヘ級flagship,軽巡ツ級elite,駆逐ハ級後期型elite,駆逐ハ級後期型elite,駆逐ハ級後期型elite,駆逐ハ級後期型elite\n";
			EnemyFleetType = 0;
			EnemyFleetFormation = 0;
			SimulationSizeIndex = 0;
			// コマンドを登録する
			OpenDeckBuilderCommand = new CommandBase(OpenDeckBuilder);
			CopyDeckBuilderFormatCommand = new CommandBase(CopyDeckBuilderFormat);
			CopyFriendDataFormatCommand = new CommandBase(CopyFriendDataFormat);
			OpenLandBaseFileCommand = new CommandBase(OpenLandBaseFile);
			OpenEnemyFileCommand = new CommandBase(OpenEnemyFile);
			ShowFriendFleetInfoCommand = new CommandBase(ShowFriendFleetInfo);
			ShowLandBaseInfoCommand = new CommandBase(ShowLandBaseInfo);
			ShowEnemyFleetInfoCommand = new CommandBase(ShowEnemyFleetInfo);
			ShowAirValueCommand = new CommandBase(ShowAirValue);
			ShowAntiAirPowerCommand = new CommandBase(ShowAntiAirPower);
			ShowCutInTypeCommand = new CommandBase(ShowCutInType);
			RunMonteCarloCommand = new CommandBase(RunMonteCarlo);
		}
	}
}
