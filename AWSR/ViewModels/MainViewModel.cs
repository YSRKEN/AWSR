using System;
using System.ComponentModel;
using System.Windows.Input;

namespace AWSR.ViewModels
{
	class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

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
