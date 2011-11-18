using System;
using System.Windows.Input;

namespace AssemblyInspector
{
	public class UpdateCommand : ICommand
	{
		private readonly ViewModel _vm;

		public UpdateCommand(ViewModel vm)
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
			_vm.Update();
		}
	}

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
	}
}