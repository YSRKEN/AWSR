using System;
using System.Collections.Generic;

namespace AWSR.ViewModels
{
	class ResultViewModel : ViewModelBase
	{
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
		List<List<List<double>>> histList;
		public List<List<List<double>>> HistList {
			get {
				return histList;
			}
			set {
				histList = value;
			}
		}
		// コンストラクタ
		public ResultViewModel(List<string> nameList, List<List<List<double>>> histList) {
			// フィールドに初期値を設定する
			NameList = nameList;
			HistList = histList;
		}
	}
}
