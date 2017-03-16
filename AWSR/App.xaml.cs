namespace AWSR
{
	using System.Windows;
	using AWSR.Views;
	using AWSR.ViewModels;
	using AWSR.Models;

	/// <summary>
	/// App.xaml の相互作用ロジック
	/// (参考→http://yujiro15.net/YKSoftware/MVVM_Tree.html)
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			// メイン画面を再生して表示する
			var w = new MainView();
			var vm = new MainViewModel();
			w.DataContext = vm;
			w.Show();
			// 乱数を初期化
			Simulator.Initialize();
			// データベースを読み込む
			try {
				DataBase.Initialize();
			}
			catch {
				MessageBox.Show("データベースが正常に読み込めませんでした.\nアプリを終了します.", "航空戦シミュレーションR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				w.Close();
			}
		}
	}
}
