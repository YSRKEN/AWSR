using AWSR.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AWSR.ViewModels
{
	class MainViewModel : ViewModelBase
	{
		#region コマンドに関する処理
		// デッキビルダーの画面を開く処理
		public ICommand OpenDeckBuilderCommand { get; private set; }
		// 自艦隊の情報を表示する処理
		public ICommand ShowFriendFleetInfoCommand { get; private set; }
		#endregion

		#region プロパティに関する処理
		// 入力するデッキビルダーのデータ
		string inputDeckBuilderText;
		public string InputDeckBuilderText {
			get => inputDeckBuilderText;
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
			get => inputAirBaseText;
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
			get => inputEnemyDataText;
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
			set { enemyFleetType = value; }
		}
		// 敵艦隊の陣形
		int enemyFleetFormation;
		public int EnemyFleetFormation {
			get { return enemyFleetFormation; }
			set { enemyFleetFormation = value; }
		}
		#endregion

		// 自艦隊の情報を表示する処理
		private void ShowFriendFleetInfo() {
			try {
				MessageBox.Show(DeckBuilder.InfoText(InputDeckBuilderText), "航空戦シミュレーションR");
			}
			catch {
				MessageBox.Show("入力データに誤りがあります.", "航空戦シミュレーションR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			
		}

		// コンストラクタ
		public MainViewModel() {
			// フィールドに初期値を設定する
			InputDeckBuilderText = "{\"version\":4,\"f1\":{\"s1\":{\"id\":\"330\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":122,\"rf\":\"10\"},\"i2\":{\"id\":122,\"rf\":\"10\"},\"i3\":{\"id\":106,\"rf\":\"10\"}}},\"s2\":{\"id\":\"346\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":122,\"rf\":\"10\"},\"i2\":{\"id\":122,\"rf\":\"10\"},\"i3\":{\"id\":106,\"rf\":\"10\"}}},\"s3\":{\"id\":\"357\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":122,\"rf\":\"10\"},\"i2\":{\"id\":122,\"rf\":\"10\"},\"i3\":{\"id\":106,\"rf\":\"10\"}}},\"s4\":{\"id\":\"428\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":135,\"rf\":\"10\"},\"i2\":{\"id\":173,\"rf\":0},\"i3\":{\"id\":124,\"rf\":0},\"i4\":{\"id\":135,\"rf\":\"10\"}}},\"s5\":{\"id\":\"278\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":191,\"rf\":0},\"i2\":{\"id\":53,\"rf\":0,\"mas\":7},\"i3\":{\"id\":157,\"rf\":0,\"mas\":7},\"i4\":{\"id\":189,\"rf\":0,\"mas\":7}}},\"s6\":{\"id\":\"461\",\"lv\":99,\"luck\":-1,\"items\":{\"i1\":{\"id\":191,\"rf\":0},\"i2\":{\"id\":110,\"rf\":0,\"mas\":7},\"i3\":{\"id\":56,\"rf\":0,\"mas\":7},\"i4\":{\"id\":110,\"rf\":0,\"mas\":7}}}}}";
			FriendFleetType = 0;
			FriendFleetFormation = 0;
			InputAirBaseText = "";
			InputEnemyDataText = "{\n\t\"formation\": \"circle\",\n	\"fleet\": [\n\t\t[544,544,528,554,515,515]\n\t]\t}\n";
			EnemyFleetType = 0;
			EnemyFleetFormation = 0;
			// コマンドを登録する
			OpenDeckBuilderCommand = new CommandBase(
				() => {
					System.Diagnostics.Process.Start("http://kancolle-calc.net/deckbuilder.html");
				}
			);
			ShowFriendFleetInfoCommand = new CommandBase(ShowFriendFleetInfo);
		}
	}
}
