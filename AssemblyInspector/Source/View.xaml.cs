using System.Windows;

namespace AssemblyInspector
{
	public partial class View : Window
	{
		public View()
		{
			InitializeComponent();
			DataContext = new ViewModel();
		}
	}
}