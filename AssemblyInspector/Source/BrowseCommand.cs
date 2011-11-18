using System;
using System.Windows.Input;

namespace AssemblyInspector
{
	public class BrowseCommand : ICommand
	{
		private readonly ViewModel _vm;

		public BrowseCommand(ViewModel vm)
		{
			_vm = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			_vm.Browse();
		}

		private void InvokeCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
			{
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}
	}
}