using System.Windows;

namespace AWSR.Views
{
	/// <summary>
	/// MainView.xaml の相互作用ロジック
	/// </summary>
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
			MouseLeftButtonDown += (o, e) => DragMove();
		}
	}
}
