using System;
using System.Windows.Input;

namespace AWSR.ViewModels
{
	public class CommandBase : ICommand
	{
		// デリゲートを保持するためのフィールド
		Action action;
		// ICommandを継承したことで生じるプロパティ
		public bool CanExecute(object parameter) => true;
		public event EventHandler CanExecuteChanged;
		// デリゲートを実行するメソッド
		public void Execute(object parameter) { action(); }
		// コンストラクタ
		public CommandBase(Action action) { this.action = action; }
	}
}
