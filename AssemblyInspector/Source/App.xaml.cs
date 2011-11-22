using System.Windows;

namespace AssemblyInspector
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var view = new View();
			view.Show();
		}
	}
}