using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AWSR
{
	using AWSR.Views;
	using AWSR.ViewModels;
	/// <summary>
	/// App.xaml の相互作用ロジック
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
