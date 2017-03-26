using System;
using System.Windows.Input;

namespace AWSR.ViewModels
{
	public class CommandBase : ICommand
	{
		// デリゲートを保持するためのフィールド
		private readonly Action action;
		// ICommandを継承したことで生じるプロパティ
		public bool CanExecute(object parameter) => true;
		public event EventHandler CanExecuteChanged;
		// デリゲートを実行するメソッド
		public void Execute(object parameter) { action(); }

		protected virtual void OnCanExecuteChanged(EventArgs e) {
			CanExecuteChanged?.Invoke(this, e);
		}

		// コンストラクタ
		public CommandBase(Action action) { this.action = action; }
	}
}
