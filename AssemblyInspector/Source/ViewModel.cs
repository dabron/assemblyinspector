using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;

namespace AssemblyInspector
{
	public class ViewModel : INotifyPropertyChanged
	{
		private const float Highlighted = 1f;
		private const float Unhighlighted = 0.25f;

		private string _path;
		private AssemblyInformation _info;
		private readonly ICommand _browseCommand;

		public ViewModel()
		{
			_browseCommand = new BrowseCommand(this);
		}

		public ICommand BrowseCommand { get { return _browseCommand; } }

		public void Browse()
		{
			var dialog = new OpenFileDialog
			{
				Filter = "*.exe,*.dll|*.exe;*.dll"
			};

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Update(dialog.FileName);
			}
		}

		private void Update(string path)
		{
			Path = path;

			_info = new AssemblyInformation(path);
			InvokePropertyChanged("Name");
			InvokePropertyChanged("Version");
			InvokePropertyChanged("Culture");
			InvokePropertyChanged("PublicKeyToken");
			InvokePropertyChanged("Architecture");
			InvokePropertyChanged("IsExe");
			InvokePropertyChanged("IsDll");
			InvokePropertyChanged("Is32Bit");
			InvokePropertyChanged("Is64Bit");
			InvokePropertyChanged("IsSigned");
			InvokePropertyChanged("IsValid");
		}

		public string Path
		{
			get { return _path; }
			private set { _path = value; InvokePropertyChanged("Path"); }
		}

		public string Name { get { return _info != null ? _info.Name : string.Empty; } }
		public string Version { get { return _info != null ? _info.Version : string.Empty; } }
		public string Culture { get {  return _info != null ? _info.Culture : string.Empty; } }
		public string PublicKeyToken { get { return _info != null ? _info.PublicKeyToken : string.Empty; } }
		public string Architecture { get { return _info != null ? _info.Architecture : string.Empty; } }
		public float IsExe { get { return _info != null && _info.FileType == "EXECUTABLE IMAGE" ? Highlighted : Unhighlighted; } }
		public float IsDll { get { return _info != null && _info.FileType == "DLL" ? Highlighted : Unhighlighted; } }
		public float Is32Bit { get { return _info != null && _info.Is32Bit ? Highlighted : Unhighlighted; } }
		public float Is64Bit { get { return _info != null && _info.Is64Bit ? Highlighted : Unhighlighted; } }
		public float IsSigned { get { return _info != null && _info.IsSigned ? Highlighted : Unhighlighted; } }
		public float IsValid { get { return _info != null && _info.IsValid ? Highlighted : Unhighlighted; } }

		public event PropertyChangedEventHandler PropertyChanged;

		private void InvokePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}