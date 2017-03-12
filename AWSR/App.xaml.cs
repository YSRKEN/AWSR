namespace AWSR
{
	using System.Windows;
	using AWSR.Views;
	using AWSR.ViewModels;
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// (参考→http://yujiro15.net/YKSoftware/MVVM_Tree.html)
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var w = new MainView();
			var vm = new MainViewModel();

			w.DataContext = vm;
			w.Show();
		}
	}
}
