using System;
using System.Windows.Forms;

namespace NosAssistant2;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();
		Application.Run(new GUI());
	}
}
