using System;

namespace AssemblyInspector
{
	internal static class Program
	{
		[STAThread] 
		private static void Main()
		{
			new View().ShowDialog();
		}
	}
}