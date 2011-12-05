using System.Windows;

namespace AssemblyInspector
{
	public partial class View : Window
	{
		public View()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			DataContext = new ViewModel();
		}
	}
}