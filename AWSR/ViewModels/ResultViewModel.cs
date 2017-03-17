namespace AWSR.ViewModels
{
	class ResultViewModel : ViewModelBase
	{
		// 結果表示用テキスト
		string resultText;
		public string ResultText {
			get { return resultText; }
			set { resultText = value; NotifyPropertyChanged(nameof(ResultText)); }
		}
		// コンストラクタ
		public ResultViewModel(string resultText) {
			// フィールドに初期値を設定する
			ResultText = resultText;
		}
	}
}
