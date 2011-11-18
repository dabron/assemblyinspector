using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;

namespace AssemblyInspector
{
	public class ViewModel : INotifyPropertyChanged
	{
		private string _path;
		private string _name;
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
			Name = path.Substring(_path.LastIndexOf('\\') + 1);

			_info = new AssemblyInformation(path);
			InvokePropertyChanged("Version");
			InvokePropertyChanged("PublicKey");
			InvokePropertyChanged("Type");
			InvokePropertyChanged("Code");
			InvokePropertyChanged("Is32Bit");
			InvokePropertyChanged("Is64Bit");
		}

		public string Path
		{
			get { return _path; }
			private set { _path = value; InvokePropertyChanged("Path"); }
		}

		public string Name
		{
			get { return _name; }
			private set { _name = value; InvokePropertyChanged("Name"); }
		}

		public string Version
		{
			get { return _info != null ? _info.AssemblyVersion : string.Empty; }
		}

		public string PublicKey
		{
			get
			{
				string publicKey = string.Empty;

				if (_info.IsSigned)
				{
					publicKey = _info.PublicKey;
				}
				else if (!string.IsNullOrEmpty(_info.Type))
				{
					publicKey = "<unsigned>";
				}

				return publicKey;
			}
		}

		public string Type
		{
			get {  return _info != null ? _info.Type : string.Empty; }
		}

		public string Code
		{
			get
			{
				string ret = string.Empty;
				if (_info.IsCil)
				{
					if (!string.IsNullOrEmpty(_info.Version))
						ret = ".NET " + _info.Version;
				}
				else
				{
					if (!string.IsNullOrEmpty(_info.Code))
						ret = "Native " + _info.Code;
				}
				return ret;
			}
		}

		public bool Is32Bit
		{
			get { return _info != null && _info.Is32Bit; }
		}

		public bool Is64Bit
		{
			get { return _info != null && _info.Is64Bit; }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void InvokePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}