using AWSR.Models;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace AWSR.ViewModels
{
	class MainViewModel : ViewModelBase
	{
		// デッキビルダーの画面を開く処理
		public ICommand OpenDeckBuilderCommand { get; private set; }

		// 入力するデッキビルダーのデータ
		string inputDeckBuilderText;
		public string InputDeckBuilderText {
			get { return inputDeckBuilderText; }
			set {
				if (inputDeckBuilderText == value)
					return;
				// 入力値が変化した際の処理
				inputDeckBuilderText = value;
				Console.WriteLine(inputDeckBuilderText);
				DeckBuilderInfoText = "";
			}
		}
		// デッキビルダーのデータを整形して出力する
		public string DeckBuilderInfoText {
			get {
				try
				{
					return DeckBuilder.InfoText(InputDeckBuilderText);
				}
				catch
				{
					return "(デッキビルダーの内容が表示されます)";
				}
			}
			set {
				NotifyPropertyChanged(nameof(DeckBuilderInfoText));
			}
		}

		// コンストラクタ
		public MainViewModel() {
			// フィールドに初期値を設定する
			InputDeckBuilderText = "";
			// コマンドを登録する
			OpenDeckBuilderCommand = new CommandBase(
				() => {
					System.Diagnostics.Process.Start("http://kancolle-calc.net/deckbuilder.html");
				}
			);
		}
	}
}
