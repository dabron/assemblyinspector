using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;

namespace AssemblyInspector
{
	public class ViewModel : INotifyPropertyChanged
	{
		private string _path;
		private readonly Model _model;
		private readonly ICommand _browseCommand;
		private readonly ICommand _updateCommand;

		public ViewModel()
		{
			_model = new Model();
			_browseCommand = new BrowseCommand(this);
			_updateCommand = new UpdateCommand(this);
		}

		public ICommand BrowseCommand { get { return _browseCommand; } }
		public ICommand UpdateCommand { get { return _updateCommand; } }

		public void Browse()
		{
			FileDialog dialog = new OpenFileDialog
			{
				Filter = "*.exe,*.dll|*.exe;*.dll"
			};

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Path = dialog.FileName;
				Update();
			}
		}

		public void Update()
		{
			_model.PopulateData(_path);
            InvokePropertyChanged("Version");
			InvokePropertyChanged("Type");
			InvokePropertyChanged("Code");
			InvokePropertyChanged("Is32Bit");
			InvokePropertyChanged("Is64Bit");
			InvokePropertyChanged("IsSigned");
		}

		public string Name { get { return string.IsNullOrEmpty(_path) ? string.Empty : _path.Substring(_path.LastIndexOf('\\') + 1); } }
        public string Version { get { return _model.AssemblyVersion; } }
		public string Type { get { return _model.Type; } }
		public bool Is32Bit { get { return _model.Is32Bit; } }
		public bool Is64Bit { get { return _model.Is64Bit; } }
		public bool IsSigned { get { return _model.IsSigned; } }

		public string Path
		{
			get { return _path; }
			set { _path = value; InvokePropertyChanged("Path"); InvokePropertyChanged("Name"); }
		}

		public string Code
		{
			get
			{
				string ret = string.Empty;
				if (_model.IsCil)
				{
					if (!string.IsNullOrEmpty(_model.Version))
						ret = ".NET " + _model.Version;
				}
				else
				{
					if (!string.IsNullOrEmpty(_model.Code))
						ret = "Native " + _model.Code;
				}
				return ret;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void InvokePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}