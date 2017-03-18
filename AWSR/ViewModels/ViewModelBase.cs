using System.ComponentModel;

namespace AWSR.ViewModels
{
	class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string parameter) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parameter));
	}
}
